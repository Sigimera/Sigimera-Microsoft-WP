using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SigimeraModel.CrisisModel;
using SigimeraModel;
using System.Device.Location;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Phone.Tasks;


namespace Sigimera
{
    public partial class PushDetail : PhoneApplicationPage
    {
        RootObject selectedItem = new RootObject();
        string id_selected;
        public PushDetail()
        {
            InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            try
            {
                string strId = string.Empty;
                if (NavigationContext.QueryString.TryGetValue("Id", out strId))
                {
                    id_selected = strId;
                    FetchAndShowEarthquakeEvent();
                }
                
            }
            catch (Exception ex)
            {

            }
        }
        private void FetchAndShowEarthquakeEvent()
        {
            try
            {
                pgbRequesting.IsIndeterminate = true;
                pgbRequesting.Visibility = System.Windows.Visibility.Visible;

                WebClient wc = new WebClient();
                wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(EarthquakeResponseRecieved);

                //Requesting specific event
                wc.DownloadStringAsync(new Uri(string.Format(Shared.CRISIS_INDIVIDUAL_URL, id_selected, Shared.AUTH_TOKEN)));
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured while requesting.", "Error", MessageBoxButton.OK);
            }
        }
        private void crisis_maptap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (selectedItem != null)
            {
                NavigationService.Navigate(new Uri("/MapPage.xaml", UriKind.Relative));
            }
        }

        private void EarthquakeResponseRecieved(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Result != null)
                {
                    selectedItem = JsonConvert.DeserializeObject<RootObject>(e.Result);
                
                }
                this.DataContext = selectedItem;
                string format_string = "yyyy-MM-dd'T'HH:mm:ss'Z'";
                DateTime new_date = DateTime.ParseExact(selectedItem.dc_date, format_string, null);
                App.dateTime = new_date.ToString();
                App.alert_level = selectedItem.crisis_alertLevel;
                App.subject = selectedItem.subject;
                App.longitude = selectedItem.foaf_based_near[0];
                App.latitude = selectedItem.foaf_based_near[1];
               
                if (selectedItem.gn_parentCountry[0] == null)
                {
                    App.place = "No information about the parent country yet";

                }
                else
                {
                    App.place = selectedItem.gn_parentCountry[0];
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
                //App.CrisisViewModel.LoadCrisis(1);
            } 
        }
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
            if (selectedItem != null)
            {
                ShareStatusTask shareStatusTask = new ShareStatusTask();
                shareStatusTask.Status = selectedItem.dc_description;
                shareStatusTask.Show();
            }
        }

        private void ApplicationBarIconButton_Click_3(object sender, EventArgs e)
        {
            if (selectedItem != null)
            {
                EmailComposeTask emailComposeTask = new EmailComposeTask();
                emailComposeTask.To = "Enter email";
                emailComposeTask.Subject = "Sigimera crisis details";
                emailComposeTask.Body = selectedItem.dc_description;
                emailComposeTask.Show();
            }
        }

        /// <summary>
        /// Refresh
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplicationBarIconButton_Click_2(object sender, EventArgs e)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("Sigimera Crisis Details").Append("\n");
            sb.Append(selectedItem.dc_description).Append("\n");
            SmsComposeTask smsComposeTask = new SmsComposeTask();
            smsComposeTask.To = "";
            smsComposeTask.Body = sb.ToString();
            smsComposeTask.Show();
        }

        #endregion
    }
}