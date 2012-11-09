using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Xml.Linq;
using System.IO;
using SigimeraModel;
using System.Device.Location;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using SigimeraModel.CrisisModel;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Phone.Tasks;

namespace Sigimera
{
    public partial class MainPage : PhoneApplicationPage
    {
        #region | Variables |

        bool successfullyLoaded = false;
        GeoCoordinateWatcher watcher;

        #endregion

        #region | Properties |


        #endregion

        #region | Constructor |

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            TiltEffect.SetIsTiltEnabled(this, true);

            using (SigimeraDataContext context = new SigimeraDataContext(Shared.CONNECTION_STRING))
            {
                if (context.DatabaseExists())
                {
                    successfullyLoaded = true;
                }
                else
                {
                    // create database if it does not exist
                    context.CreateDatabase();

                    successfullyLoaded = true;
                }
            }

            DataContext = App.CrisisViewModel;

            // Set the data context of the listbox control to the sample data
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);

        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {

                if (successfullyLoaded)
                {
                    //Load Map
                    //Center align to your location if permission to use location is granted in application
                    watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default);
                    if (watcher.Permission == GeoPositionPermission.Granted)
                    {
                        watcher.MovementThreshold = 20;
                    }
                    watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);
                    watcher.Start();

                    LoadLatestCrisis();
                }
                else
                {
                    MessageBox.Show("Communication with database could not be made. App will now exit.", "Error", MessageBoxButton.OK);
                    if (NavigationService.CanGoBack)
                    {
                        NavigationService.GoBack();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while restoring user settings. Earthquakes record will still be loaded against default settings.", "Error", MessageBoxButton.OK);
            }

        }

        #endregion

        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            watcher.Stop();
        }

        private void ListBoxLatestCrisis_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

        #region | Methods [private] |

        private void FetchAndShowLatestEarthquakeEvents()
        {
            try
            {
                pgbRequesting.IsIndeterminate = true;
                pgbRequesting.Visibility = System.Windows.Visibility.Visible;

                WebClient wc = new WebClient();
                wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(EarthquakeResponseRecieved);

                //Always request ten new events
                wc.DownloadStringAsync(new Uri(string.Format(Shared.CRISIS_LIST_URL, Shared.AUTH_TOKEN)));
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured while requesting.", "Error", MessageBoxButton.OK);
            }
        }

        private void EarthquakeResponseRecieved(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Result != null)
                {
                    List<RootObject> listRootObjects = JsonConvert.DeserializeObject<List<RootObject>>(e.Result);

                    foreach (RootObject crisisItem in listRootObjects)
                    {
                        if (!DataCommunication.CrisisExists(crisisItem._id))
                        {
                            DataCommunication.AddCrisis(crisisItem);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Do Nothing
            }
            finally
            {
                pgbRequesting.Visibility = System.Windows.Visibility.Collapsed;
                //Explicitly load data again from local database
                App.CrisisViewModel.LoadCrisis(10);
            }
        }

        private void LoadLatestCrisis()
        {
            if (!App.CrisisViewModel.IsDataLoaded)
            {
                App.CrisisViewModel.LoadCrisis(10);
            }

            //If no item could be found in database load from server
            if (App.CrisisViewModel.Items.Count == 0)
            {
                FetchAndShowLatestEarthquakeEvents();
            }
        }

        #endregion

        #region | Appplicar Bar |

        /// <summary>
        /// About
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Buy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplicationBarMenuItem_Click_2(object sender, EventArgs e)
        {
            MarketplaceDetailTask marketplaceDetailTask = new MarketplaceDetailTask();
            marketplaceDetailTask.ContentType = MarketplaceContentType.Applications;
            //marketplaceDetailTask.ContentIdentifier = "";
            marketplaceDetailTask.Show();
        }

        /// <summary>
        /// Review
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            MarketplaceReviewTask review = new MarketplaceReviewTask();
            review.Show();
        }

        /// <summary>
        /// Share
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            if (ListBoxLatestCrisis.SelectedItem != null)
            {
                RootObject rootObject = (RootObject)ListBoxLatestCrisis.SelectedItem;
                ShareStatusTask shareStatusTask = new ShareStatusTask();
                shareStatusTask.Status = rootObject.dc_description;
                shareStatusTask.Show();
            }
        }

        /// <summary>
        /// Settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplicationBarIconButton_Click_2(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Settings.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Refresh
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplicationBarIconButton_Click_3(object sender, EventArgs e)
        {
            //Call to fetch any new events and then re-bind the list
            FetchAndShowLatestEarthquakeEvents();
        }

        #endregion

    }
}