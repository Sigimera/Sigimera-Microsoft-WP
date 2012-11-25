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
        #region Connection String 

        public const string CONNECTION_STRING = @"Data Source='isostore:/Sigimera.sdf';Password=geek-salvation;";

        #endregion

        #region Setting properties
        
        public const string SETTING_USER_AUTH_TOKEN = "SETTING_USER_AUTH_TOKEN";
        public const string SETTING_CHANNEL_URI = "SETTING_CHANNEL_URI";
        public const string SETTING_DEVICE_REGISTRATION_ID = "SETTING_DEVICE_REGISTRATION_ID";
        public const string SETTING_DEVICE_ID = "SETTING_DEVICE_ID";

        #endregion

        #region Setting Values

        public const string AUTH_TOKEN = "iL75qm8o1FpysThDYwVt";
        public const string CHANNEL_URI = "CHANNEL_URI";
        public const string DEVICE_REGISTRATION_ID = "DEVICE_REGISTRATION_ID";

        #endregion

        public const string CRISIS_LIST_URL = @"http://api.sigimera.org/v1/crises?auth_token={0}&output=short";

        //The call below creates a token if exists none otherwise old one is returned
        public const string URL_GENERATE_TOKEN = @"http://api.sigimera.org/v1/tokens.json";
        public const string URL_REGISTER_DEVICE = @"http://api.sigimera.org/v1/mpn.json";   //POST
        public const string URL_UNREGISTER_DEVICE = @"http://api.sigimera.org/v1/mpn/{0}/";   //DELETE
        public const string URL_UPDATE_DEVICE = @"http://api.sigimera.org/v1/mpn/{0}/";   //PUT
            ///reg_uri
            ///device_name
            ///windows_api_level
            ///lat
            ///lon

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>

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
