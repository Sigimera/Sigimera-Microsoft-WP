﻿using System;
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

                    AuthenticateUser();
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
            if (ListBoxLatestCrisis.SelectedItem != null)
            {
                RootObject selectedItem = (RootObject)ListBoxLatestCrisis.SelectedItem;

                string url = string.Format("/DetailsPage.xaml?Id={0}", selectedItem.CrisisId);
                NavigationService.Navigate(new Uri(url, UriKind.Relative));
            }
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

        #region | Authentication |

        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(TextBoxUsername.Text) || string.IsNullOrEmpty(TextBoxPassword.Text))
                {
                    TextBlockError.Text = "Please provide a username or password.";
                    if (string.IsNullOrEmpty(TextBoxUsername.Text))
                    {
                        TextBoxUsername.Focus();
                    }
                    else if (string.IsNullOrEmpty(TextBoxPassword.Text))
                    {
                        TextBoxPassword.Focus();
                    }
                }
                else
                {
                    var request = HttpWebRequest.Create(Shared.GENERATE_TOKEN_URL);
                    request.Method = "POST";

                    var req = (IAsyncResult)request.BeginGetRequestStream(GetRequestStreamCallback, request);
                }
            }
            catch (Exception ex)
            {
                TextBlockError.Text = "An error occurred. We apologize for inconvenience.";
            }
        }

        private void GetRequestStreamCallback(IAsyncResult asynchronousResult)
        {
            try
            {

                string credentials = string.Format("email={0}&password={1}", TextBoxUsername.Text, TextBoxPassword.Text);

                HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;

                // End the operation
                Stream postStream = request.EndGetRequestStream(asynchronousResult);

                // Convert the string into a byte array.
                byte[] postBytes = Encoding.UTF8.GetBytes(credentials);

                // Write to the request stream.
                postStream.Write(postBytes, 0, postBytes.Length);
                postStream.Close();

                // Start the asynchronous operation to get the response
                var result = (IAsyncResult)request.BeginGetResponse(GetResponseCallback, request);
            }
            catch (Exception ex)
            {
                TextBlockError.Text = "An error occurred while requesting. We apologize for inconvenience.";
            }
        }

        private void GetResponseCallback(IAsyncResult result)
        {
            try
            {
                var request = (HttpWebRequest)result.AsyncState;
                var response = request.EndGetResponse(result);

                using (var stream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        //Grab the autehntication token 
                        var serializer = new DataContractJsonSerializer(typeof(AuthenticationToken));
                        AuthenticationToken authenticationToken = (AuthenticationToken)serializer.ReadObject(stream);

                        Dispatcher.BeginInvoke(() =>
                        {
                            //Store the token
                            AppSettings.StoreSetting("AuthentToken", authenticationToken.auth_token);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                TextBlockError.Text = "An error occurred while authorizing. We apologize for inconvenience.";
            }

        }

        #endregion

        /// <summary>
        /// This method authenticates user given that an authentication token exists in the database or no
        /// </summary>
        private void AuthenticateUser()
        {
            //Try to read the token from isolated storage
            if (AppSettings.TryGetSetting("AuthentToken", out Shared.USER_AUTH_TOKEN))
            {
                StackPanelLogin.Visibility = System.Windows.Visibility.Collapsed;
                StackPanelPostLogin.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                StackPanelLogin.Visibility = System.Windows.Visibility.Visible;
                StackPanelPostLogin.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}