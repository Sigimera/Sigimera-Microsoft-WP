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
using Microsoft.Phone.Shell;

using Microsoft.Phone.Maps.Toolkit;
using Sigimera;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps;

namespace Sigimera
{
    public partial class MainPage : PhoneApplicationPage
    {
        #region | Variables |

        bool successfullyLoaded = false;
        int counter=0;
        double shortest_distance;
        double latitude2 = 0.0;
        double latitude1;
        double distance;
        double longitude2 = 0.0;
        double temp_latitude;
        double temp_longitude;
        int set_shortest_distance = 0;
        double map_latitude, map_longitude;
        string map_alertlevel;
        DateTime map_datetime;
        string format_string = "yyyy-MM-dd'T'HH:mm:ss'Z'";
        GeoCoordinateWatcher watcher;
        Pushpin pin;
        int crisis_count=0;
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
            // Set the data context of the listbox control to the sample data
            this.DataContext = App.CrisisViewModel;
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
                    App.CrisisViewModel.LoadTileData();
                    LoadMap();
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
        private void EarthquakeResponseRecieved(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Result != null)
                {
                    GeoCoordinateWatcher watcher = new GeoCoordinateWatcher();
                    watcher.TryStart(false, TimeSpan.FromMilliseconds(10));
                    GeoCoordinate coord = watcher.Position.Location;
                    List<RootObject> listRootObjects = JsonConvert.DeserializeObject<List<RootObject>>(e.Result);
                    foreach (RootObject crisisItem in listRootObjects)
                    {

                        App.coordinates[0, crisis_count] = crisisItem.foaf_based_near[1];
                        App.coordinates[1, crisis_count] = crisisItem.foaf_based_near[0];
                        crisis_count++;
                        if (!DataCommunication.CrisisExists(crisisItem._id))
                        {
                            DataCommunication.AddCrisis(crisisItem);
                        }
                        if (counter == 0)
                        {
                            TextBlockCountry.Text = crisisItem.gn_parentCountry[0];
                            TextBlockAlertLevel.Text = crisisItem.crisis_alertLevel;
                            TextBlockSubject.Text = crisisItem.subject;
                            map_alertlevel = crisisItem.crisis_alertLevel;
                            map_longitude = crisisItem.foaf_based_near[0];
                            map_latitude = crisisItem.foaf_based_near[1];
                            map_datetime = DateTime.ParseExact(crisisItem.dc_date, format_string, null);

                            counter = 1;
                        }
                        if (coord.IsUnknown != true)
                        {
                            latitude2 = coord.Latitude;
                            longitude2 = coord.Longitude;
                                var R = 6372.8; // In kilometers
                                temp_latitude = crisisItem.foaf_based_near[1];
                                temp_longitude = crisisItem.foaf_based_near[0];
                                var dLat = toRadians(latitude2 - temp_latitude);
                                var dLon = toRadians(longitude2 - temp_longitude);
                                latitude1 = toRadians(temp_latitude);
                                latitude2 = toRadians(latitude2);

                                var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(latitude1) * Math.Cos(latitude2);
                                var c = 2 * Math.Asin(Math.Sqrt(a));
                                distance = R * 2 * Math.Asin(Math.Sqrt(a));
                                if (distance < 0.0)
                                {
                                    distance = -(distance);
                                }
                                if (set_shortest_distance == 0)
                                {
                                    if (distance != 0)
                                    {
                                        shortest_distance = distance;
                                        set_shortest_distance = 1;
                                    }
                                }
                                else if (distance !=0)
                                {
                                    if(distance<shortest_distance)
                                    {
                                        shortest_distance = distance;
                                    }
                                    
                                }

                            }
                        if (shortest_distance <= 10000)
                        {
                            System.Text.StringBuilder sb = new System.Text.StringBuilder();
                            sb.Append(Convert.ToInt32(shortest_distance)).Append(" km\nNear Crisis");
                            App.CrisisViewModel.first_crisis_at = sb.ToString();
                        }
                        else
                        {
                            System.Text.StringBuilder sb = new System.Text.StringBuilder();
                            sb.Append("no\n").Append("Nearest Crisis");
                            App.CrisisViewModel.first_crisis_at = sb.ToString();
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
        private void LoadMap()
        {
            try
            {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("Date: ").Append(map_datetime.ToString());
            sampleMap.Center = new GeoCoordinate(map_latitude, map_longitude);
            if (map_alertlevel == "Green")
            {
                    pin = new Pushpin
                    {
                        GeoCoordinate = new GeoCoordinate(map_latitude,map_longitude),
                        Background= new SolidColorBrush(Colors.Green),
                        Content = sb.ToString(),
                        Foreground= new SolidColorBrush(Colors.Black)
                    };
                }
                else if (map_alertlevel == "Orange")
                {
                    pin = new Pushpin
                    {
                        GeoCoordinate = new GeoCoordinate(map_latitude, map_longitude),
                        Background = new SolidColorBrush(Colors.Orange),
                        Content = sb.ToString(),
                        Foreground = new SolidColorBrush(Colors.Black)
                    };
                }
                else if(map_alertlevel=="Red")
                {
                   pin = new Pushpin
                    {
                        GeoCoordinate = new GeoCoordinate(map_latitude, map_longitude),
                        Background = new SolidColorBrush(Colors.Red),
                        Content = sb.ToString(),
                        Foreground = new SolidColorBrush(Colors.Black)
                    };


                }
                MapOverlay overlay = new MapOverlay
                {
                    GeoCoordinate = new GeoCoordinate(map_latitude, map_longitude),
                    Content = new Ellipse
                    {
                        Fill = new SolidColorBrush(Colors.Red),
                        Width = 40,
                        Height = 40
                    }
                };


                overlay.Content = pin;
                MapLayer layer = new MapLayer();
                layer.Add(overlay);
                sampleMap.Layers.Add(layer);


            }
            catch(Exception e)
            {
               
            }
        }
            
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
        private void sampleMap_Loaded(object sender, RoutedEventArgs e)
        {
            MapsSettings.ApplicationContext.ApplicationId = "<applicationid>";
            MapsSettings.ApplicationContext.AuthenticationToken = "<authenticationtoken>";
        }  

        public static double toRadians(double angle)
        {
            return Math.PI * angle / 180.0;
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

        /// <summary>
        /// Review
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplicationBarMenuItem_Click_2(object sender, EventArgs e)
        {
            MarketplaceReviewTask review = new MarketplaceReviewTask();
            review.Show();
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            if (ListBoxLatestCrisis.SelectedItem != null)
            {
                RootObject rootObject = (RootObject)ListBoxLatestCrisis.SelectedItem;
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("Sigimera Crisis Details").Append("\n");
                sb.Append(rootObject.dc_description).Append("\n");
                SmsComposeTask smsComposeTask = new SmsComposeTask();
                smsComposeTask.To = "";
                smsComposeTask.Body = sb.ToString();
                smsComposeTask.Show();
            }
            else
            {
                SmsComposeTask smsComposeTask = new SmsComposeTask();
                smsComposeTask.To = "";
                smsComposeTask.Body = "Download Sigimera for Windows Phone 8 to get crisis alerts";
                smsComposeTask.Show();
            }
        }
        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            if (ListBoxLatestCrisis.SelectedItem != null)
            {
                RootObject rootObject = (RootObject)ListBoxLatestCrisis.SelectedItem;
                ShareStatusTask shareStatusTask = new ShareStatusTask();
                shareStatusTask.Status = rootObject.dc_description;
                shareStatusTask.Show();
            }
            else
            {
                ShareStatusTask shareStatusTask = new ShareStatusTask();
                shareStatusTask.Status = "Download Sigimera for Windows Phone 8 to get crisis alerts";
                shareStatusTask.Show();
            }
        }

       
        private void ApplicationBarIconButton_Click_2(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Settings.xaml", UriKind.Relative));
        }

       
        private void ApplicationBarMenuItem_Click_3(object sender, EventArgs e)
        {
            //Call to fetch any new events and then re-bind the list
            FetchAndShowLatestEarthquakeEvents();
            App.CrisisViewModel.LoadTileData();
            LoadMap();
        }

        private void ApplicationBarIconButton_Click_3(object sender, EventArgs e)
        {
            if (ListBoxLatestCrisis.SelectedItem != null)
            {
                RootObject rootObject = (RootObject)ListBoxLatestCrisis.SelectedItem;
                
                EmailComposeTask emailComposeTask = new EmailComposeTask();
                emailComposeTask.To = "Enter email";
                emailComposeTask.Subject = "Sigimera crisis details";
                emailComposeTask.Body = rootObject.dc_description;
                emailComposeTask.Show();
            }
            else
            {
                EmailComposeTask emailComposeTask = new EmailComposeTask();
                emailComposeTask.To = "Enter email";
                emailComposeTask.Subject = "Sigimera";
                emailComposeTask.Body = "Download Sigimera for Windows Phone 8 to get crisis alerts";
                emailComposeTask.Show();
            }
        }
        #endregion

        #region | Authentication |

        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(TextBoxUsername.Text) || string.IsNullOrEmpty(TextBoxPassword.Password))
                {
                    TextBlockError.Text = "Please provide a username or password.";
                    if (string.IsNullOrEmpty(TextBoxUsername.Text))
                    {
                        TextBoxUsername.Focus();
                    }
                    else if (string.IsNullOrEmpty(TextBoxPassword.Password))
                    {
                        TextBoxPassword.Focus();
                    }
                }
                else
                {
                    var request = HttpWebRequest.Create(Shared.URL_GENERATE_TOKEN);

                    //Sigimera makes use of basic authentication
                    SetBasicAuthHeader(request, TextBoxUsername.Text, TextBoxPassword.Password);
                    request.Method = "POST";

                    //var req = (IAsyncResult)request.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), request);

                    SetLoginProcessing(true);
                    TextBlockError.Text = "Authenticating...";

                    var result = (IAsyncResult)request.BeginGetResponse(new AsyncCallback(GetResponseCallback), request);
                }
            }
            catch (Exception ex)
            {
                TextBlockError.Text = "An error occurred. We apologize for inconvenience.";

                SetLoginProcessing(false);
            }
        }

        private void SetBasicAuthHeader(WebRequest request, String userName, String userPassword)
        {
            string authInfo = userName + ":" + userPassword;
            authInfo = Convert.ToBase64String(Encoding.UTF8.GetBytes(authInfo));
            request.Headers["Authorization"] = "Basic " + authInfo;
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
                            AppSettings.StoreSetting(Shared.SETTING_USER_AUTH_TOKEN, authenticationToken.auth_token);

                            //Store the token in App class reference so that it can now be used
                            App.USER_AUTH_TOKEN = authenticationToken.auth_token;

                            ((ApplicationBarIconButton)ApplicationBar.Buttons[2]).IsEnabled = true;

                            //Clear text fields
                            TextBoxUsername.Text = string.Empty;
                            TextBoxPassword.Password = string.Empty;

                            //Show the corresponding panels
                            StackPanelLogin.Visibility = System.Windows.Visibility.Collapsed;
                            StackPanelPostLogin.Visibility = System.Windows.Visibility.Visible;
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "The remote server returned an error: NotFound.")
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        TextBlockError.Text = "Login failed. Please check your credentials and try again.";
                        TextBoxUsername.Focus();
                    });
                }
                else
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        TextBlockError.Text = "An error occurred while authorizing. We apologize for inconvenience.";
                    });
                }
                Dispatcher.BeginInvoke(() =>
                {
                    SetLoginProcessing(false);
                });
            }
        }

        /// <summary>
        /// This method authenticates user given that an authentication token exists in the database or no
        /// </summary>
        private void AuthenticateUser()
        {
            //Try to read the token from isolated storage
            
            if (AppSettings.TryGetSetting(Shared.SETTING_USER_AUTH_TOKEN, out App.USER_AUTH_TOKEN))
            {
                StackPanelLogin.Visibility = System.Windows.Visibility.Collapsed;
                StackPanelPostLogin.Visibility = System.Windows.Visibility.Visible;
                ((ApplicationBarIconButton)ApplicationBar.Buttons[2]).IsEnabled = true;
            }
            else
            {
                StackPanelLogin.Visibility = System.Windows.Visibility.Visible;
                StackPanelPostLogin.Visibility = System.Windows.Visibility.Collapsed;
                ((ApplicationBarIconButton)ApplicationBar.Buttons[2]).IsEnabled = false;
            }
        }

        private void SetLoginProcessing(bool isProcessing)
        {
            if (isProcessing)
            {
                ButtonLogin.IsEnabled = false;
                TextBoxUsername.IsEnabled = false;
                TextBoxPassword.IsEnabled = false;
                ProgressBarLogin.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                ButtonLogin.IsEnabled = true;
                TextBoxUsername.IsEnabled = true;
                TextBoxPassword.IsEnabled = true;
                ProgressBarLogin.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        #endregion

        #region | Logout |

        private void HlinkLogout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Delete settings
                AppSettings.DeleteSetting(Shared.SETTING_USER_AUTH_TOKEN);

                //The token is no there
                App.USER_AUTH_TOKEN = string.Empty;

                //Post login
                StackPanelLogin.Visibility = System.Windows.Visibility.Visible;
                StackPanelPostLogin.Visibility = System.Windows.Visibility.Collapsed;
                ProgressBarLogin.Visibility = System.Windows.Visibility.Collapsed;
                this.TextBoxUsername.IsEnabled = true;
                this.TextBoxPassword.IsEnabled = true;
                this.TextBlockError.Text = " ";
                this.ButtonLogin.IsEnabled = true;


                ((ApplicationBarIconButton)ApplicationBar.Buttons[2]).IsEnabled = false;
            }
            catch (Exception ex)
            {

            }
        }

        #endregion
    }
}