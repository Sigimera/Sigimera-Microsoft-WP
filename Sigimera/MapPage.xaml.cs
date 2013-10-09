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
using Microsoft.Phone.Controls.Maps;
using SigimeraModel; 

namespace Sigimera
{
    public partial class MapPage : PhoneApplicationPage
    {
        string place;
        double magnitude;
        double longitude;
        double latitude;
        DateTime dateTime;
        double depth;
        SolidColorBrush OneToFourMagnitudePushPin = new SolidColorBrush(Colors.Green);
        SolidColorBrush FiveToSixMagnitudePushPin = new SolidColorBrush(Colors.Orange);
        SolidColorBrush SixPlusMagnitudePushPin = new SolidColorBrush(Colors.Red);

        public MapPage()
        {
            InitializeComponent();

        }

        // When page is navigated to set data context to selected item in list
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                magnitude = Convert.ToDouble(NavigationContext.QueryString["Magnitude"]);
                longitude = Convert.ToDouble(NavigationContext.QueryString["Longitude"]);
                latitude = Convert.ToDouble(NavigationContext.QueryString["Latitude"]);
                dateTime = Convert.ToDateTime(NavigationContext.QueryString["DateTime"]);
                depth = Convert.ToDouble(NavigationContext.QueryString["Depth"]);

                map1.Center = new GeoCoordinate(latitude, longitude);
                map1.ZoomBarVisibility = System.Windows.Visibility.Visible;
                map1.ZoomLevel = 5;
                map1.Mode = new Microsoft.Phone.Controls.Maps.AerialMode();

                LoadMagnitudePushpinColorCodes();

                AddSimplePushPin();
            }
            catch (Exception ex)
            {
            }
        }

        private void AddSimplePushPin()
        {
            //Add a push pin
            Pushpin pin = new Pushpin();
            pin.Location = new GeoCoordinate(latitude, longitude);
            pin.Foreground = new SolidColorBrush(Colors.White);

            if (magnitude <= 4)
            {
                pin.Background = OneToFourMagnitudePushPin;
            }
            else if (magnitude > 4 && magnitude <= 6)
            {
                pin.Background = FiveToSixMagnitudePushPin;
            }
            else
            {
                pin.Background = SixPlusMagnitudePushPin;
            }

            pin.Content = String.Format("{0}\nMagnitude: {1}\n{2}\nDepth: {3} km", place, magnitude.ToString(), dateTime.ToString(), depth.ToString());

            map1.Children.Add(pin);
        }

        private void AddCircle()
        {
            Pushpin pin = new Pushpin();
            pin.Location = new GeoCoordinate(latitude, longitude);
            ImageBrush image = new ImageBrush()
            {
                ImageSource = new System.Windows.Media.Imaging.BitmapImage
                                  (new Uri("circle.png",UriKind.Relative))
            };

            //---draw an ellipse inside the pushpin and fill it with the image---
            pin.Content = new Rectangle()
            {
                Fill = image,
                //                StrokeThickness = 10,
                Height = 100,
                Width = 100,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Opacity = 0.5

            };

            //---add the pushpin to the map---
            map1.Children.Add(pin);
        }

        private void LoadMagnitudePushpinColorCodes()
        {
            object value;

            //1-4 Pushpin color
            if (AppSettings.TryGetSetting<object>("1-4Mag", out value))
            {
                OneToFourMagnitudePushPin.Color = (Color)value;
            }
            else
            {
                OneToFourMagnitudePushPin.Color = Colors.Green;
            }

            //1-4 Pushpin color
            if (AppSettings.TryGetSetting<object>("5-6Mag", out value))
            {
                FiveToSixMagnitudePushPin.Color = (Color)value;
            }
            else
            {
                FiveToSixMagnitudePushPin.Color = Colors.Orange;
            }

            //1-4 Pushpin color
            if (AppSettings.TryGetSetting<object>("6PlusMag", out value))
            {
                SixPlusMagnitudePushPin.Color = (Color)value;
            }
            else
            {
                SixPlusMagnitudePushPin.Color = Colors.Red;
            }
        }

    }
}