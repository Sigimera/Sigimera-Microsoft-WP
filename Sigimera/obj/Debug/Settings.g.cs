﻿#pragma checksum "C:\Users\asmapc\Documents\GitHub\Sigimera-Microsoft-WP\Sigimera\Settings.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "17620B2C56DEEA8A696BAC4F63094C82"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18051
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace Sigimera {
    
    
    public partial class Settings : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.StackPanel TitlePanel;
        
        internal Microsoft.Phone.Controls.PerformanceProgressBar pgbProcessing;
        
        internal System.Windows.Controls.TextBlock PageTitle;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal System.Windows.Controls.TextBlock TextBlockError;
        
        internal Microsoft.Phone.Controls.ToggleSwitch ToggleSwitchRegisterUnregister;
        
        internal Microsoft.Phone.Controls.ToggleSwitch ToggleSwitchPushNotification;
        
        internal System.Windows.Controls.Button btnUpdateLocation;
        
        internal System.Windows.Controls.Button btnClearData;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/Sigimera;component/Settings.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.TitlePanel = ((System.Windows.Controls.StackPanel)(this.FindName("TitlePanel")));
            this.pgbProcessing = ((Microsoft.Phone.Controls.PerformanceProgressBar)(this.FindName("pgbProcessing")));
            this.PageTitle = ((System.Windows.Controls.TextBlock)(this.FindName("PageTitle")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.TextBlockError = ((System.Windows.Controls.TextBlock)(this.FindName("TextBlockError")));
            this.ToggleSwitchRegisterUnregister = ((Microsoft.Phone.Controls.ToggleSwitch)(this.FindName("ToggleSwitchRegisterUnregister")));
            this.ToggleSwitchPushNotification = ((Microsoft.Phone.Controls.ToggleSwitch)(this.FindName("ToggleSwitchPushNotification")));
            this.btnUpdateLocation = ((System.Windows.Controls.Button)(this.FindName("btnUpdateLocation")));
            this.btnClearData = ((System.Windows.Controls.Button)(this.FindName("btnClearData")));
        }
    }
}
