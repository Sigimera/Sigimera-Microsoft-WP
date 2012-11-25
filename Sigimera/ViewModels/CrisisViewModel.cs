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

namespace Sigimera
{
    public class CrisisViewModel : INotifyPropertyChanged
    {
        public CrisisViewModel()
        {
            this.Items = new ObservableCollection<RootObject>();
            this._allTimeCrisis = "N/A";
            this._crisisToday = "N/A";
            this._latestCrisis = "N/A";
            this._nearestCrisis = "N/A";
        }

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<RootObject> Items { get; private set; }

        private string _nearestCrisis;
        public string NearestCrisis
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
                    NotifyPropertyChanged("NearestCrisis");
                }
            }
        }

        private string _crisisToday;
        public string CrisisToday
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
                    NotifyPropertyChanged("CrisisToday");
                }
            }
        }

        private string _latestCrisis;
        public string LatestCrisis
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
                    NotifyPropertyChanged("LatestCrisis");
                }
            }
        }

        private string _allTimeCrisis;
        public string AllTimeCrisis
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
                    NotifyPropertyChanged("AllTimeCrisis");
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