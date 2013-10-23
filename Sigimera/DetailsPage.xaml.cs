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
using System.Windows.Navigation;
using SigimeraModel.CrisisModel;
using Microsoft.Phone.Tasks;
using SigimeraModel;
using Newtonsoft.Json;

namespace Sigimera
{
    public partial class DetailsPage : PhoneApplicationPage
    {
        RootObject selectedItem = new RootObject();
        int crisis_id;
        public DetailsPage()
        {
            InitializeComponent();
        }

                // When page is navigated to set data context to selected item in list
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                string strId = string.Empty;
                if (NavigationContext.QueryString.TryGetValue("Id", out strId))
                {
                    selectedItem = App.CrisisViewModel.LoadSingleCrisis(Convert.ToInt32(strId));
                    
                }
                this.DataContext = selectedItem;
                string format_string = "yyyy-MM-dd'T'HH:mm:ss'Z'";
                DateTime new_date = DateTime.ParseExact(selectedItem.dc_date, format_string, null);
                App.dateTime = new_date.ToString();
                App.alert_level = selectedItem.crisis_alertLevel;
                App.subject = selectedItem.subject;
                crisis_id = selectedItem.CrisisId - 1;
                App.latitude = App.coordinates[0,crisis_id];
                App.longitude = App.coordinates[1, crisis_id];
            }
            catch (Exception ex)
            {

            }
        }
        private void crisis_maptap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (selectedItem != null)
            {
                NavigationService.Navigate(new Uri("/MapPage.xaml", UriKind.Relative));
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