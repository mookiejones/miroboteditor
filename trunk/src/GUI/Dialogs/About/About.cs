﻿using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Navigation;
using miRobotEditor.Classes;

namespace miRobotEditor.GUI.Dialogs.About
{
    
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About( )
        {
            InitializeComponent();
            var localUrl = Properties.Resources.Website;
            Hyperlink.NavigateUri = new Uri(localUrl);
            HyperlinkText.Text = localUrl;
        }

        
        private void OnOKClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void HandleLinkClick(object sender, RequestNavigateEventArgs e)
        {
            var hyperlink = (Hyperlink)sender;
            try
            {
                var fileName = hyperlink.NavigateUri.ToString();
                Process.Start(new ProcessStartInfo(fileName));
                e.Handled = true;
            }
            catch (Exception ex)
            {
                MessageViewModel.Instance.Add("Error In About Window", ex.ToString(), null);
            }
        }
    }
}