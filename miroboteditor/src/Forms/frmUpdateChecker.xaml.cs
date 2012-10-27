using System;
using System.Net;
using System.Windows;

namespace miRobotEditor.Forms
{
    /// <summary>
    /// Interaction logic for frmUpdateChecker.xaml
    /// </summary>
    public partial class FrmUpdateChecker : Window
    {

        public bool AskForUpdates { get; set; }

        private static FrmUpdateChecker _instance;

        public static FrmUpdateChecker Instance
        {
            get { return _instance ?? (_instance = new FrmUpdateChecker()); }
            set { _instance = value; }
        }



        /// <summary>
        /// Gets amount of time since last update
        /// </summary>
        /// <returns>Days as Integer</returns>
        private int DaysSinceUpdate()
        {
            return Math.Abs((Properties.Settings.Default.LastUpdate - DateTime.Now).Days);
        }


        private UpdateVersion CheckForUpdates()
        {

            var result = new UpdateVersion();
            try
            {

                string link = @"https://sites.google.com/site/dmcautomation/home/software/dmc-robot-editor";
            using (var client = new WebClient())
            {
                var contents = client.DownloadString(link);
                Console.WriteLine(contents);

            }
            }
            catch (Exception)
            {
                throw;
            }


            return result;

        }

        internal class UpdateVersion
        {
            public string Version { get; set; }
            public string Link { get; set; }

        }

        private bool UpdateAvailable()
        {
            if (DaysSinceUpdate() < 7)
                return false;

            var version = CheckForUpdates();


            return true;


        }

        public FrmUpdateChecker()
        {
            // Return if automatic updates are turned off
            if (!Properties.Settings.Default.CheckForUpdates)
                return;

            // Check to see if updates are available
            if (UpdateAvailable())
                InitializeComponent();

        }
        public FrmUpdateChecker(string updateversion)
        {

            //TODO Test for updater to work correctly
            string productName = System.Windows.Forms.Application.ProductName;
            InitializeComponent();
            Title = String.Format("{0} Updater",productName);
            lblUpdateLabel.Text = String.Format("A New version of {0} {1} is available. Would you like to Update?",productName,updateversion);
        }

        private void CheckChanged(object sender, RoutedEventArgs e)
        {
            if (ckDontAsk.IsChecked != null) AskForUpdates = (bool)ckDontAsk.IsChecked;
        }
    }
}
