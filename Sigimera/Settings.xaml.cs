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

namespace Sigimera
{
    public partial class Settings : PhoneApplicationPage
    {
        BackgroundWorker bg = new BackgroundWorker();

        public Settings()
        {
            InitializeComponent();

            TiltEffect.SetIsTiltEnabled(this, true);

        }

        /// <summary>
        /// Cancel
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
        /// Save
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            try
            {
                //Number of Earthquake Events
                AppSettings.StoreSetting("NumberOfEarthquakeEvents", txtNumberOfEarthquakeEvents.Text);

                //Sort BY
                AppSettings.StoreSetting("SortBy", lstSortBy.SelectedIndex.ToString());

                //Magnitude Filter
                AppSettings.StoreSetting("NotificationMagnitude", txtMagnitude.Text);

                //Notification Time
                AppSettings.StoreSetting("NotificationTime", lstNotificationTime.SelectedIndex.ToString());

                //Notification Type
                AppSettings.StoreSetting("NotificationType", lstNotification.SelectedIndex.ToString());

            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured. We apologize for inconvenience.", "Error", MessageBoxButton.OK);
            }
            finally
            {
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            string value;

            //Magnitude Filter
            if (AppSettings.TryGetSetting<string>("NotificationMagnitude", out value))
            {
                sldMagnitude.Value = Convert.ToInt32(value);
            }
            else
            {
                sldMagnitude.Value = 6;
            }

            //Notification Time 
            if (AppSettings.TryGetSetting<string>("NotificationTime", out value))
            {
                lstNotificationTime.SelectedIndex = Convert.ToInt32(value); ;
            }
            else
            {
                lstNotificationTime.SelectedIndex = 0;
            }

            //Number of Earthquake Events
            if (AppSettings.TryGetSetting<string>("NumberOfEarthquakeEvents", out value))
            {
                sldNumberOfEarthquakeEvetns.Value = Convert.ToInt32(value);
            }
            else
            {
                sldNumberOfEarthquakeEvetns.Value = 20;
            }

            //Sort By
            if (AppSettings.TryGetSetting<string>("SortBy", out value))
            {
                lstSortBy.SelectedIndex = Convert.ToInt32(value);
            }
            else
            {
                lstSortBy.SelectedIndex = 0;
            }

            object val;

            //Notification Type
            if (AppSettings.TryGetSetting<string>("NotificationType", out value))
            {
                lstNotification.SelectedIndex = Convert.ToInt32(value);
            }
            else
            {
                lstNotification.SelectedIndex = 2;
            }

            bg.WorkerSupportsCancellation = true;

            bg.DoWork -= DoWork;
            bg.DoWork += DoWork;

            bg.RunWorkerCompleted -= bg_RunWorkerCompleted;
            bg.RunWorkerCompleted += bg_RunWorkerCompleted;

        }

        //Set as default
        private void ApplicationBarIconButton_Click_2(object sender, EventArgs e)
        {
            try
            {
                //Number of Earthquake Events to Retrieve
                sldNumberOfEarthquakeEvetns.Value = 20;

                //Magnitude Filter
                sldMagnitude.Value = 6;

                lstSortBy.SelectedIndex = 0;

                //Notification Time (past 30 minutes)
                lstNotificationTime.SelectedIndex = 0;

                //Notification Type
                lstNotification.SelectedIndex = 2;

            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured. We apologize for inconvenience.", "Error", MessageBoxButton.OK);
            }
        }

        private void backgroundFilter_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            try
            {
                if (backgroundFilter.IsChecked == true)
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

        private void btnClearData_Click(object sender, RoutedEventArgs e)
        {
            if (App.IsTrial)
            {
                NavigationService.Navigate(new Uri("/BuyPage.xaml", UriKind.Relative));
            }
            else
            {
                LayoutRoot.IsHitTestVisible = false; // Restore mouse hit

                pgbDeleting.IsIndeterminate = true;
                pgbDeleting.Visibility = System.Windows.Visibility.Visible;

                ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = false;
                ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled = false;
                ((ApplicationBarIconButton)ApplicationBar.Buttons[2]).IsEnabled = false;

                bg.RunWorkerAsync();
            }
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (bg.IsBusy)
            {
                e.Cancel = true;                    // Cancel subsequent BackKey navigation
                MessageBox.Show("Application is busy in clearing database. Please wait for a while.", "Information", MessageBoxButton.OK);
            }
            base.OnBackKeyPress(e); // Call base
        }

        void bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs args)
        {
            LayoutRoot.IsHitTestVisible = true; // Restore mouse hit

            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = true;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled = true;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[2]).IsEnabled = true;

            pgbDeleting.Visibility = System.Windows.Visibility.Collapsed;

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

    }
}