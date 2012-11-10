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

namespace Sigimera
{
    public partial class DetailsPage : PhoneApplicationPage
    {
        RootObject selectedItem = new RootObject();

        public DetailsPage()
        {
            InitializeComponent();
            //this.DataContext = selectedItem;
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
            }
            catch (Exception ex)
            {

            }
        }
    }
}