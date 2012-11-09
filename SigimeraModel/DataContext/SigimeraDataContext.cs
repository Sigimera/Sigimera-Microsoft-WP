using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using System.Data.Linq;
using SigimeraModel.CrisisModel;

namespace SigimeraModel
{
    public class SigimeraDataContext : DataContext
    {
        public SigimeraDataContext(string connectionString)
            : base(connectionString)
        {
        }

        public Table<RootObject> CrisisItems
        {
            get
            {
                return this.GetTable<RootObject>();
            }
        }

    }
}