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
using Microsoft.Phone.Tasks;

namespace Sigimera
{
    public partial class AboutPage : PhoneApplicationPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Back
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        /// <summary>
        /// Send Email
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            try
            {
                Microsoft.Phone.Tasks.EmailComposeTask emailComposeTask = new Microsoft.Phone.Tasks.EmailComposeTask();
                emailComposeTask.To = "Usman.Ur.Rehman.Ahmed@outlook.com";
                emailComposeTask.Subject = "Feedback: Sigimera";
                emailComposeTask.Body = "Contact for feature request, suggestions, bug report or feedback.";
                emailComposeTask.Show(); // Launches send mail screen
            }
            catch (Exception ex)
            {
                //Do Nothing
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MarketplaceSearchTask marketplaceSearchTask = new MarketplaceSearchTask();
                marketplaceSearchTask.SearchTerms = "Usman ur Rehman Ahmed";
                marketplaceSearchTask.Show();
            }
            catch (Exception ex)
            {
                //Do Nothing
            }
        }

    }
}