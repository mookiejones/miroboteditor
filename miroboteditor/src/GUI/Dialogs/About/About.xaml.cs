using System;
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
            string localUrl = Properties.Resources.Website;
            Hyperlink.NavigateUri = new Uri(localUrl);
            HyperlinkText.Text = localUrl;
        }

        
        private void OnOKClick(object sender, RoutedEventArgs e)
        {
            base.Close();
        }

        private void HandleLinkClick(object sender, RequestNavigateEventArgs e)
        {
            Hyperlink hyperlink = (Hyperlink)sender;
            try
            {
                string fileName = hyperlink.NavigateUri.ToString();
                Process.Start(new ProcessStartInfo(fileName));
                e.Handled = true;
            }
            catch (Exception ex)
            {
                OutputMessages.Messages.Add(new OutputWindowMessage().Add("Error In About Window",ex.ToString(),null));
            }
        }
    }
}
