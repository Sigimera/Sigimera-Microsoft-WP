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
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace SigimeraModel
{
    public static class Shared
    {
        public const string CONNECTION_STRING = @"Data Source='isostore:/Sigimera.sdf';Password=geek-salvation;";

        public const string AUTH_TOKEN = "iL75qm8o1FpysThDYwVt";
        public const string CRISIS_LIST_URL = @"http://api.sigimera.org/v1/crises?auth_token={0}&output=short";

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
        {
            ObservableCollection<T> obsColl = new ObservableCollection<T>();
            foreach (T element in source)
            {
                obsColl.Add(element);
            }
            return obsColl;
        }
    }
}
