﻿using System.Diagnostics;
using System.ComponentModel;

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Canvas1
{
    // MP! Automated paging ideas by Noemata.

    public sealed partial class Page1 : Page, INotifyPropertyChanged
    {
        public Page1()
        {
            // Note: NavigationCacheMode.Disabled is the default
            //NavigationCacheMode = NavigationCacheMode.Enabled;
            InitializeComponent();
            DataContext = null;
            Loaded += OnLoaded;
            Trace.WriteLine("Page 1 not cached.");

            RegisterRendering();
        }

        int redrawCycle = 0;
        private void OnRendering(object sender, object e)
        {
            redrawCycle++;
            // When NavigationCacheMode.Disabled, we need to give the UI time to render and respond to input.  Is there a better/faster way to do this?
            if (MainWindow.Context.AutoPage && redrawCycle == 4)
            {
                UnRegisterRendering();
                MainWindow.RootFrame.Navigate(typeof(Page2));
            }

            // Stop rendering if UI is going idle.
            if (!MainWindow.Context.AutoPage && redrawCycle > 4)
                UnRegisterRendering();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            UnRegisterRendering();
        }

        ~Page1()
        {
            Trace.WriteLine("Page1 cleared.");
        }

        private void OnLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (DataContext == null)
            {
                Trace.WriteLine("Loading Page 1 data ...");

                DataContext = this;
            }

            MainWindow.Context.CheckMem();

            if (MainWindow.Context.AutoPage && NavigationCacheMode != NavigationCacheMode.Disabled)
                MainWindow.RootFrame.Navigate(typeof(Page2));
        }

        private void OnClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // Need to prevent the OnRender page change from becoming additive.  Required when rendering is active.
            if (redrawCycle > 4 || NavigationCacheMode == NavigationCacheMode.Enabled)
                MainWindow.RootFrame.Navigate(typeof(Page2));
        }

        void RegisterRendering()
        {
            if (NavigationCacheMode == NavigationCacheMode.Disabled)
                Microsoft.UI.Xaml.Media.CompositionTarget.Rendering += OnRendering;
        }

        void UnRegisterRendering()
        {
            if (NavigationCacheMode == NavigationCacheMode.Disabled)
                Microsoft.UI.Xaml.Media.CompositionTarget.Rendering -= OnRendering;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}