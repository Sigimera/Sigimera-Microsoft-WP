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
using Microsoft.Phone.Scheduler;
using System.ComponentModel;
using Microsoft.Phone.Shell;
using System.Windows.Navigation;
using SigimeraModel;
using System.IO;
using System.Text;
using Microsoft.Phone.Info;
using Microsoft.Phone.Notification;
using System.Device.Location;

namespace Sigimera
{
    public partial class Settings : PhoneApplicationPage
    {
        BackgroundWorker bg = new BackgroundWorker();
        // Holds the push channel that is created or found.
        HttpNotificationChannel pushChannel;
        // The name of our push channel.
        string channelName = "Sigimera Push Channel";
        GeoCoordinateWatcher watcher;
        GeoCoordinate cooridnate;

        #region | Constructor |

        public Settings()
        {
            InitializeComponent();

            TiltEffect.SetIsTiltEnabled(this, true);

            //Call below checks if a push channel exists, if there exists one,
            //it opens it up, if there exists none then then it tries to retreive URI 
            //to register push notification
            PushChannelRegistration();
        }

        #endregion

        #region | Page Event [Load] |

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            string value;

            //Number of Earthquake Events
            if (AppSettings.TryGetSetting<string>("NumberOfEarthquakeEvents", out value))
            {
                //sldNumberOfEarthquakeEvetns.Value = Convert.ToInt32(value);
            }
            else
            {
                //sldNumberOfEarthquakeEvetns.Value = 20;
            }

            object val;

            bg.WorkerSupportsCancellation = true;

            bg.DoWork -= DoWork;
            bg.DoWork += DoWork;

            bg.RunWorkerCompleted -= bg_RunWorkerCompleted;
            bg.RunWorkerCompleted += bg_RunWorkerCompleted;

        }

        #endregion

        #region | Clear Data |

        private void btnClearData_Click(object sender, RoutedEventArgs e)
        {
            SetProcessing(true);
            bg.RunWorkerAsync();
        }

        void bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs args)
        {
            SetProcessing(false);

            bg = null;
            bg = new BackgroundWorker();
            bg.WorkerSupportsCancellation = true;
            bg.DoWork += new DoWorkEventHandler(DoWork);
            bg.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bg_RunWorkerCompleted);
        }

        private void DoWork(Object sender, DoWorkEventArgs args)
        {
            DataCommunication.ClearRecords();
        }

        #endregion

        #region | Back Button Handling |

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (bg.IsBusy)
            {
                e.Cancel = true;                    // Cancel subsequent BackKey navigation
                MessageBox.Show("Application is busy in clearing database. Please wait for a while.", "Information", MessageBoxButton.OK);
            }
            base.OnBackKeyPress(e); // Call base
        }

        #endregion

        #region | Push Channel Registration |

        private void PushChannelRegistration()
        {
            // Try to find the push channel.
            pushChannel = HttpNotificationChannel.Find(channelName);

            // If the channel was not found, then create a new connection to the push service.
            if (pushChannel == null)
            {
                pushChannel = new HttpNotificationChannel(channelName);

                // Register for all the events before attempting to open the channel.
                pushChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(PushChannel_ChannelUriUpdated);
                pushChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(PushChannel_ErrorOccurred);

                // Register for this notification only if you need to receive the notifications while your application is running.
                //pushChannel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(PushChannel_ShellToastNotificationReceived);

                pushChannel.Open();

                // Bind this new channel for toast events.
                pushChannel.BindToShellToast();

                //Update UI that user channel is being requested
                SetProcessing(true);
                TextBlockError.Text = "Requesting push notification channel...";
            }
            else
            {
                // The channel was already open, so just register for all the events.
                //pushChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(PushChannel_ChannelUriUpdated);
                pushChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(PushChannel_ErrorOccurred);

                // Register for this notification only if you need to receive the notifications while your application is running.
                //pushChannel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(PushChannel_ShellToastNotificationReceived);

                //Store channel URI in app setttings
                AppSettings.StoreSetting(Shared.SETTING_CHANNEL_URI, pushChannel.ChannelUri.ToString());

                //Dispatcher.BeginInvoke(() =>
                //{
                //    //Enable Register/unregister toggle button
                ToggleSwitchRegisterUnregister.IsEnabled = true;
                //});

                //Push channel exists, if the DEVICE ID was retrieved last time
                //if there is a device id present in devie setting then enable push notification
                string deviceId = string.Empty;
                if (AppSettings.TryGetSetting(Shared.SETTING_DEVICE_ID, out deviceId))
                {
                    App.DEVICE_ID = deviceId;
                    ToggleSwitchRegisterUnregister.IsChecked = true;
                }
                else
                {
                    ToggleSwitchRegisterUnregister.IsChecked = false;
                }
            }
        }

        void PushChannel_ChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
        {
            //Store channel URI in app setttings
            AppSettings.StoreSetting(Shared.SETTING_CHANNEL_URI, e.ChannelUri.ToString());

            Dispatcher.BeginInvoke(() =>
            {
                //Enable Register/unregister toggle button
                ToggleSwitchRegisterUnregister.IsEnabled = true;

                SetProcessing(false);
                TextBlockError.Text = "";
            });
        }

        void PushChannel_ErrorOccurred(object sender, NotificationChannelErrorEventArgs e)
        {
            // Error handling logic for your particular application would be here.
            Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show(String.Format("A push notification {0} error occurred.  {1} ({2}) {3}",
                        e.ErrorType, e.Message, e.ErrorCode, e.ErrorAdditionalData));
                    SetProcessing(false);
                    TextBlockError.Text = "";
                });
        }

        #endregion

        #region | Register/Unregister |

        private void ToggleSwitchRegisterUnregister_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (ToggleSwitchRegisterUnregister.IsChecked == true)
            {
                RequestDeviceRegisgration();
            }
            else
            {
                RequestDeviceUnregistration();
            }
        }

        #region | Registration |
        
        private void RequestDeviceRegisgration()
        {
            try
            {
                SetProcessing(true);

                var request = HttpWebRequest.Create(Shared.URL_REGISTER_DEVICE);
                request.Method = "POST";

                var req = (IAsyncResult)request.BeginGetRequestStream(new AsyncCallback(GetDeviceRegistrationRequestStreamCallback), request);
            }
            catch (Exception ex)
            {
                TextBlockError.Text = "An error occurred. Please check your internet connection and try later.";
                ToggleSwitchRegisterUnregister.IsChecked = false;
                SetProcessing(false);
            }
        }

        private void GetDeviceRegistrationRequestStreamCallback(IAsyncResult asynchronousResult)
        {
            try
            {
                string deviceName = string.Empty;
                string reg_Uri = string.Empty;
                object device;

                //DeviceStatus.DeviceName;
                if (DeviceExtendedProperties.TryGetValue("DeviceName", out device))
                {
                    deviceName = device.ToString();
                }

                //Only continue if registration URI is retrieved
                if (AppSettings.TryGetSetting(Shared.SETTING_CHANNEL_URI, out reg_Uri))
                {
                    string dataToPost = string.Empty;
                    dataToPost = string.Format("auth_token={0}&reg_uri={1}&device_name={2}&windows_api_level=7.5", App.USER_AUTH_TOKEN, reg_Uri, deviceName);

                    HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;

                    // End the operation
                    Stream postStream = request.EndGetRequestStream(asynchronousResult);

                    // Convert the string into a byte array.
                    byte[] postBytes = Encoding.UTF8.GetBytes(dataToPost);

                    // Write to the request stream.
                    postStream.Write(postBytes, 0, postBytes.Length);
                    postStream.Close();

                    // Start the asynchronous operation to get the response
                    var result = (IAsyncResult)request.BeginGetResponse(new AsyncCallback(GetDeviceRegistrationResponseCallback), request);
                }
            }
            catch (Exception ex)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    TextBlockError.Text = "An error occurred while requesting device registration. We apologize for inconvenience.";
                    ToggleSwitchRegisterUnregister.IsChecked = false;
                    SetProcessing(false);
                });
            }
        }

        private void GetDeviceRegistrationResponseCallback(IAsyncResult result)
        {
            try
            {
                var request = (HttpWebRequest)result.AsyncState;
                var response = request.EndGetResponse(result);

                string deviceId = string.Empty;

                if (!string.IsNullOrEmpty(response.Headers["Location"]))
                {
                    App.DEVICE_ID = response.Headers["Location"].Replace("http://api.sigimera.org/v1/mpn/", string.Empty);
                    AppSettings.StoreSetting(Shared.SETTING_DEVICE_ID, App.DEVICE_ID);

                    btnUpdateLocation.IsEnabled = true;
                }

                response.Close();

            }
            catch (Exception ex)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    TextBlockError.Text = "An error occurred while registering device. We apologize for inconvenience.";
                    ToggleSwitchRegisterUnregister.IsChecked = false;
                    SetProcessing(false);
                });
            }
            finally
            {
                Dispatcher.BeginInvoke(() =>
                {
                    TextBlockError.Text = "";
                    SetProcessing(false);
                });
            }
        }

        #endregion
        
        #region | Unregistration |

        private void RequestDeviceUnregistration()
        {
            try
            {
                SetProcessing(true);

                var request = HttpWebRequest.Create(string.Format(Shared.URL_UNREGISTER_DEVICE, App.DEVICE_ID));
                request.Method = "DELETE";

                var req = (IAsyncResult)request.BeginGetRequestStream(new AsyncCallback(GetDeviceUnRegistrationRequestStreamCallback), request);
            }
            catch (Exception ex)
            {
                TextBlockError.Text = "An error occurred. Please check your internet connection and try later.";
                ToggleSwitchRegisterUnregister.IsChecked = true;
                SetProcessing(false);
            }
        }

        private void GetDeviceUnRegistrationRequestStreamCallback(IAsyncResult asynchronousResult)
        {
            try
            {
                string dataToPost = string.Empty;
                dataToPost = string.Format("auth_token={0}", App.USER_AUTH_TOKEN);

                HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;

                // End the operation
                Stream postStream = request.EndGetRequestStream(asynchronousResult);

                // Convert the string into a byte array.
                byte[] postBytes = Encoding.UTF8.GetBytes(dataToPost);

                // Write to the request stream.
                postStream.Write(postBytes, 0, postBytes.Length);
                postStream.Close();

                // Start the asynchronous operation to get the response
                var result = (IAsyncResult)request.BeginGetResponse(new AsyncCallback(GetDeviceUnRegistrationResponseCallback), request);
            }
            catch (Exception ex)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    TextBlockError.Text = "An error occurred while requesting device unregistration. We apologize for inconvenience.";
                    ToggleSwitchRegisterUnregister.IsChecked = true;
                    SetProcessing(false);
                });
            }
        }

        private void GetDeviceUnRegistrationResponseCallback(IAsyncResult result)
        {
            try
            {
                var request = (HttpWebRequest)result.AsyncState;
                var response = request.EndGetResponse(result);

                AppSettings.DeleteSetting(Shared.SETTING_DEVICE_ID);
                App.DEVICE_ID = string.Empty;

                btnUpdateLocation.IsEnabled = false;

                response.Close();
            }
            catch (Exception ex)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    TextBlockError.Text = "An error occurred while unregistering device. We apologize for inconvenience.";
                    ToggleSwitchRegisterUnregister.IsChecked = true;
                    SetProcessing(false);
                });
            }
            finally
            {
                Dispatcher.BeginInvoke(() =>
                {
                    TextBlockError.Text = "";
                    SetProcessing(false);
                });
            }
        }

        #endregion

        private void SetProcessing(bool isProcessing)
        {
            if (isProcessing)
            {
                LayoutRoot.IsHitTestVisible = false;
                pgbProcessing.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                LayoutRoot.IsHitTestVisible = true;
                pgbProcessing.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        #endregion

        #region | Push Notification |

        private void ToggleSwitchPushNotification_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            try
            {
                if (ToggleSwitchPushNotification.IsChecked == true)
                {

                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred. We apologize for inconvenience.", "Error", MessageBoxButton.OK);
            }
        }


        #endregion

        #region | Update Location |

        private void btnUpdateLocation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default);
                if (watcher.Permission == GeoPositionPermission.Granted)
                {
                    SetProcessing(true);
                    watcher.MovementThreshold = 20;
                }
                TextBlockError.Text = "Updating location...";

                watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);
                watcher.Start();
            }
            catch (Exception ex)
            {
                TextBlockError.Text = "An error occurred. Please check your internet connection and try later.";
                SetProcessing(false);
            }
        }

        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            try
            {

                cooridnate = e.Position.Location;
                watcher.Stop();

                //Now request location update

                var request = HttpWebRequest.Create(string.Format(Shared.URL_UPDATE_DEVICE, App.DEVICE_ID));
                request.Method = "PUT";

                var req = (IAsyncResult)request.BeginGetRequestStream(new AsyncCallback(GetDeviceUpdateRequestStreamCallback), request);
            }
            catch (Exception ex)
            {
                TextBlockError.Text = "An error occurred while retrieving location. Please check your internet connection and try later.";
                SetProcessing(false);
            }
        }

        private void GetDeviceUpdateRequestStreamCallback(IAsyncResult asynchronousResult)
        {
            try
            {
                string dataToPost = string.Empty;
                dataToPost = string.Format("auth_token={0}&lat={1}&lon={2}", App.USER_AUTH_TOKEN,cooridnate.Latitude,cooridnate.Longitude);

                HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;

                // End the operation
                Stream postStream = request.EndGetRequestStream(asynchronousResult);

                // Convert the string into a byte array.
                byte[] postBytes = Encoding.UTF8.GetBytes(dataToPost);

                // Write to the request stream.
                postStream.Write(postBytes, 0, postBytes.Length);
                postStream.Close();

                // Start the asynchronous operation to get the response
                var result = (IAsyncResult)request.BeginGetResponse(new AsyncCallback(GetDeviceUpdateResponseCallback), request);
            }
            catch (Exception ex)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    TextBlockError.Text = "An error occurred while requesting device location update. We apologize for inconvenience.";
                    ToggleSwitchRegisterUnregister.IsChecked = true;
                    SetProcessing(false);
                });
            }
        }

        private void GetDeviceUpdateResponseCallback(IAsyncResult result)
        {
            try
            {
                var request = (HttpWebRequest)result.AsyncState;
                var response = request.EndGetResponse(result);

                response.Close();

                TextBlockError.Text = "Your location is updated.";
                SetProcessing(false);
            }
            catch (Exception ex)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    TextBlockError.Text = "An error occurred while updating device location. We apologize for inconvenience.";
                    ToggleSwitchRegisterUnregister.IsChecked = true;
                    SetProcessing(false);
                });
            }
            finally
            {
                Dispatcher.BeginInvoke(() =>
                {
                    TextBlockError.Text = "";
                    SetProcessing(false);
                });
            }
        }

        #endregion

    }
}