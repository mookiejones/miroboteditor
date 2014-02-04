using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Navigation;
using miRobotEditor.Core;
using miRobotEditor.Resources;
using miRobotEditor.Resources.StringResources;
using miRobotEditor.ViewModel;

namespace miRobotEditor.GUI.Dialogs.About
{
    
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
// ReSharper disable RedundantExtendsListEntry
    public partial class About : Window
// ReSharper restore RedundantExtendsListEntry
    {
        public About( )
        {
            InitializeComponent();
            var localUrl = Findahome.Website;
            Hyperlink.NavigateUri = new Uri(localUrl);
            HyperlinkText.Text = localUrl;
        }

        
        private void OnOkClick(object sender, RoutedEventArgs e)
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
                MessageViewModel.Instance.Add("Error In About Window", ex.ToString(), MsgIcon.Error);
            }
        }
    }
}
