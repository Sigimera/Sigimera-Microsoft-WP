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
    public class CrisisViewModel
    {
        public CrisisViewModel()
        {
            this.Items = new ObservableCollection<RootObject>();
        }

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<RootObject> Items { get; private set; }

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

            this.IsDataLoaded = true;
        }

                /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public RootObject LoadSingleCrisis(int crisisId)
        {
            return DataCommunication.GetSingleCrisis(crisisId);
        }
    }
}