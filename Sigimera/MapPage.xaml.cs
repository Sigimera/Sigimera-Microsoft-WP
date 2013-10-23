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
using System.Device.Location;
using Microsoft.Phone.Maps;
using SigimeraModel;
using SigimeraModel.CrisisModel;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Maps.Controls;
using System.Windows.Media.Imaging;
using Windows.Devices.Geolocation;
using Newtonsoft.Json;
using Microsoft.Phone.Controls.Maps.Platform;
using Microsoft.Phone.Maps.Toolkit;
using System.Collections.ObjectModel;
namespace Sigimera
{
    public partial class MapPage : PhoneApplicationPage
    {
        const int MIN_ZOOM_LEVEL = 3;
        const int MAX_ZOOM_LEVEL = 16;
        Pushpin pin;

        public MapPage()
        {
            InitializeComponent();
            
        }

        // When page is navigated to set data context to selected item in list
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            ShowCrisisOnTheMap();
            
       }
        private void sampleMap_Loaded(object sender, RoutedEventArgs e)
        {
            MapsSettings.ApplicationContext.ApplicationId = "<applicationid>";
            MapsSettings.ApplicationContext.AuthenticationToken = "<authenticationtoken>";
        }   
         private  void ShowCrisisOnTheMap()

        {
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                if (App.place != null)
                {
                    sb.Append("Place: ").Append(App.place).Append("\n");
                }
                sb.Append("Subject: ").Append(App.subject).Append("\n");
                sb.Append("Date: ").Append(App.dateTime);
                sampleMap.Center = new GeoCoordinate(App.latitude, App.longitude);
                sampleMap.ZoomLevel = 8;
                sampleMap.CartographicMode = MapCartographicMode.Aerial;
                if (App.alert_level == "Green")
                {
                    pin = new Pushpin
                    {
                        GeoCoordinate = new GeoCoordinate(App.latitude,App.longitude),
                        Background= new SolidColorBrush(Colors.Green),
                        Content = sb.ToString(),
                        Foreground= new SolidColorBrush(Colors.Black)
                    };
                }
                else if (App.alert_level == "Orange")
                {
                    pin = new Pushpin
                    {
                        GeoCoordinate = new GeoCoordinate(App.latitude, App.longitude),
                        Background = new SolidColorBrush(Colors.Orange),
                        Content = sb.ToString(),
                        Foreground = new SolidColorBrush(Colors.Black)
                    };
                }
                else
                {
                   pin = new Pushpin
                    {
                        GeoCoordinate = new GeoCoordinate(App.latitude, App.longitude),
                        Background = new SolidColorBrush(Colors.Red),
                        Content = sb.ToString(),
                        Foreground = new SolidColorBrush(Colors.Black)
                    };


                }
                MapOverlay overlay = new MapOverlay
                {
                    GeoCoordinate = new GeoCoordinate(App.latitude,App.longitude),
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
                MessageBox.Show(e.Message);
            }
            
        }
         #region ApplicationBar
         private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }

        private void ApplicationBarMenuItem_Click_2(object sender, EventArgs e)
        {
            MarketplaceReviewTask review = new MarketplaceReviewTask();
            review.Show();
        }
        void ZoomIn(object sender, EventArgs e)
        {
            if (sampleMap.ZoomLevel < MAX_ZOOM_LEVEL)
            {
                sampleMap.ZoomLevel++;
               // sampleMap.Center = new GeoCoordinate(App.latitude, App.longitude);
            }
        }

        void ZoomOut(object sender, EventArgs e)
        {
           if (sampleMap.ZoomLevel > MIN_ZOOM_LEVEL)
            {
                sampleMap.ZoomLevel--;
              //  sampleMap.Center = new GeoCoordinate(App.latitude, App.longitude);
            }
        }
        void AerialMode(object sender, EventArgs e)
        {
            sampleMap.CartographicMode = MapCartographicMode.Aerial;
        }
        void RoadMode(object sender, EventArgs e)
        {
            sampleMap.CartographicMode = MapCartographicMode.Road;
        }
    #endregion
    }
}