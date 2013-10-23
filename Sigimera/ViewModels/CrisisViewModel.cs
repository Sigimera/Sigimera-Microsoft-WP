using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

using System.Linq;
using System.Data.Linq;
using SigimeraModel;
using SigimeraModel.CrisisModel;
using System.Net;
using Newtonsoft.Json;
using Windows.Devices.Geolocation;
using System.Device.Location;

namespace Sigimera
{
    public class CrisisViewModel : INotifyPropertyChanged
    {
        public CrisisViewModel()
        {
           
           this.Items = new ObservableCollection<RootObject>();
        }

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<RootObject> Items { get; private set; }
        public CrisisViewModel temporary;

        private string _nearestCrisis;
        public string first_crisis_at
        {
            get
            {
                return _nearestCrisis;
            }
            set
            {
                if (value != _nearestCrisis)
                {
                    _nearestCrisis = value;
                    NotifyPropertyChanged("first_crisis_at");
                }
            }
        }
        private string _latestCrisis;
        public string latest_crisis_at
        {
            get
            {
                return _latestCrisis;
            }
            set
            {
                if (value != _latestCrisis)
                {
                    _latestCrisis = value;
                    NotifyPropertyChanged("latest_crisis_at");
                }
            }
        }
        private string _allTimeCrisis;
        public string total_crises
        {
            get
            {
                return _allTimeCrisis;
            }
            set
            {
                if (value != _allTimeCrisis)
                {
                    _allTimeCrisis = value;
                    NotifyPropertyChanged("total_crises");
                }
            }
        }


        private string _crisisToday;
        public string today_crises
        {
            get
            {
                return _crisisToday;
            }
            set
            {
                if (value != _crisisToday)
                {
                    _crisisToday = value;
                    NotifyPropertyChanged("today_crises");
                }
            }
        }
        private string _uploadedImages;
        public string uploaded_images 
        {
            get
            {
                return _uploadedImages;
            }
            set
            {
                if (value != _uploadedImages)
                {
                    _uploadedImages = value;
                    NotifyPropertyChanged("uploaded_images ");
                }
            }
        }

        private string _postedComments;
        public string posted_comments
        {
            get
            {
                return _postedComments;
            }
            set
            {
                if (value != _postedComments)
                {
                    _postedComments = value;
                    NotifyPropertyChanged("posted_comments");
                }
            }
        }

        private string _reportedLocations;
        public string reported_locations
        {
            get
            {
                return _reportedLocations;
            }
            set
            {
                if (value != _reportedLocations)
                {
                    _reportedLocations = value;
                    NotifyPropertyChanged("reported_locations");
                }
            }
        }
        private string _reportedMissingPeople;
        public string reported_missing_people
        {
            get
            {
                return _reportedMissingPeople;
            }
            set
            {
                if (value != _reportedMissingPeople)
                {
                    _reportedMissingPeople = value;
                    NotifyPropertyChanged("reported_missing_people");
                }
            }
        }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadCrisis(int crisisCount)
        {
            this.Items.Clear();

            List<RootObject> listRootObjects = DataCommunication.GetCrisis(crisisCount);
            foreach(RootObject rootObject in listRootObjects)
            {
                this.Items.Add(rootObject);
                
            }

            //Load Home page post login data here as well
            this.IsDataLoaded = true;
        }

                /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public RootObject LoadSingleCrisis(int crisisId)
        {
            return DataCommunication.GetSingleCrisis(crisisId);
        }
        public void LoadTileData()
        {
            try
            {
                WebClient wc = new WebClient();
                wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(TileDataResponseRecieved);
                wc.DownloadStringAsync(new Uri(string.Format(Shared.STATS_MAIN_URL, Shared.AUTH_TOKEN)));
            }
            catch (Exception e)
            {
                MessageBox.Show("An error occured while loading data.", "Error", MessageBoxButton.OK);
            }
        }
        public void TileDataResponseRecieved(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Result != null)
                {
                    temporary = new CrisisViewModel();
                    temporary = JsonConvert.DeserializeObject<CrisisViewModel>(e.Result);
                 }
               
                if (temporary.today_crises != null)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.Append(temporary.today_crises).Append(" crises\nToday");
                    this.today_crises = sb.ToString();
                }
                if (temporary.total_crises != null)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.Append(temporary.total_crises).Append(" crises since 7.March 2012");
                    this.total_crises = sb.ToString();
                }
                if (temporary.latest_crisis_at != null)
                {
                    DateTime system_time = DateTime.Now.ToUniversalTime();
                    string format_string = "yyyy-MM-dd'T'HH:mm:ss'Z'";
                    DateTime new_time = DateTime.ParseExact(temporary.latest_crisis_at, format_string, null);
                    System.TimeSpan difference = system_time - new_time;
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.Append(difference.Hours).Append(" hours ago\nLatest Crisis");
                    this.latest_crisis_at = sb.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Data could not be loaded. Please refresh the page", "Error", MessageBoxButton.OK);
            }

        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
       
    }
}