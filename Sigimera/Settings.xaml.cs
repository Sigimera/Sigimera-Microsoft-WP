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

namespace Sigimera
{
    public partial class Settings : PhoneApplicationPage
    {
        BackgroundWorker bg = new BackgroundWorker();

        #region | Constructor |

        public Settings()
        {
            InitializeComponent();

            TiltEffect.SetIsTiltEnabled(this, true);

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
            LayoutRoot.IsHitTestVisible = false;

            pgbProcessing.IsIndeterminate = true;
            pgbProcessing.Visibility = System.Windows.Visibility.Visible;

            bg.RunWorkerAsync();
        }

        void bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs args)
        {
            LayoutRoot.IsHitTestVisible = true; // Restore mouse hit

            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = true;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled = true;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[2]).IsEnabled = true;

            pgbProcessing.Visibility = System.Windows.Visibility.Collapsed;

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

        #region | Register/Unregister |

        private void ToggleSwitchRegisterUnregister_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (ToggleSwitchRegisterUnregister.IsChecked == true)
            {
                RegisterDevice();
            }
            else
            {
                UnregisterDevice();
            }
        }

        private void RegisterDevice()
        {
            try
            {
                LayoutRoot.IsHitTestVisible = false;
                var request = HttpWebRequest.Create(Shared.GENERATE_TOKEN_URL);
                request.Method = "POST";

                var req = (IAsyncResult)request.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), request);
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
                string deviceName = string.Empty;
                string reg_Uri = string.Empty;
                object device;

                if (DeviceExtendedProperties.TryGetValue("DeviceName", out device))
                {
                    deviceName = device.ToString();
                }

                string dataToPost = string.Empty;
                dataToPost = string.Format("reg_uri={0}&device_name={1}&windows_api_level=7.5", reg_Uri, deviceName);

                HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;

                // End the operation
                Stream postStream = request.EndGetRequestStream(asynchronousResult);

                // Convert the string into a byte array.
                byte[] postBytes = Encoding.UTF8.GetBytes(dataToPost);

                // Write to the request stream.
                postStream.Write(postBytes, 0, postBytes.Length);
                postStream.Close();

                // Start the asynchronous operation to get the response
                var result = (IAsyncResult)request.BeginGetResponse(new AsyncCallback(GetResponseCallback), request);
            }
            catch (Exception ex)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    TextBlockError.Text = "An error occurred while requesting device registration. We apologize for inconvenience.";
                });
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
                    }
                }
            }
            catch (Exception ex)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    TextBlockError.Text = "An error occurred while registering device. We apologize for inconvenience.";
                });
            }
        }

        private void UnregisterDevice()
        {
            LayoutRoot.IsHitTestVisible = false;
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


    }
}