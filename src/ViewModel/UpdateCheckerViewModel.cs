using System;
using System.Net;
using System.Reflection;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using miRobotEditor.Commands;
using miRobotEditor.Handlers;
using miRobotEditor.Properties;

namespace miRobotEditor.ViewModel
{
    public class UpdateCheckerViewModel : ViewModelBase
    {
        public UpdateCheckerViewModel()
        {
            // Should I check for updates
            CheckForUpdates();
        }

        public UpdateVersion Version { get; set; }

        #region Commands

        private RelayCommand _cancelcommand;
        private RelayCommand _updatecommand;

        public ICommand UpdateCommand => _updatecommand ?? (_updatecommand = new RelayCommand(param => Update(), param => true));

        public ICommand CancelCommand => _cancelcommand ?? (_cancelcommand = new RelayCommand(param => Cancel(), param => Version != null));

        #endregion

        #region Properties

        private bool _updateapplication;

        public bool UpdateApplication
        {
            get { return _updateapplication; }
            set
            {
                _updateapplication = value;
                RaisePropertyChanged("UpdateApplication");
            }
        }

        public string UpdateText => Version != null
                    ? String.Format("A New version of {0} {1} is available. Would you like to Update?", ProductName,
                        Version.Version)
                    : string.Empty;

        public string Title => String.Format("{0} Updater", ProductName);

        public string ProductName => App.ProductName;


        public bool AskForUpdates
        {
            get { return Settings.Default.CheckForUpdates; }
            set
            {
                Settings.Default.CheckForUpdates = value;
                RaisePropertyChanged("AskForUpdates");
            }
        }

        #endregion

        public event UpdateRequiredHandler UpdateRequired;

        private void RaiseUpdateRequired()
        {
            if (UpdateRequired != null)
                UpdateRequired(this, new EventArgs());
        }

        public void Update()
        {
            UpdateApplication = true;
        }

        public void Cancel()
        {
        }


        private void CheckForUpdates()
        {
            try
            {
                const string link = @"https://sites.google.com/site/dmcautomation/home/software/dmc-robot-editor";
                using (var client = new WebClient())
                {
                    string contents = client.DownloadString(link);
                    const string dlLink = "Latest Version</div><div><br /></div><div><br /></div><div>-<a href=\"";
                    string dlink = contents.Substring(contents.IndexOf(dlLink, StringComparison.Ordinal) + dlLink.Length);

                    dlink = dlink.Substring(0, dlink.IndexOf("\"", StringComparison.Ordinal));

                    const string dlVersion = "DMC Robot Editor V";

                    string dversion =
                        contents.Substring(contents.IndexOf(dlVersion, StringComparison.Ordinal) + dlVersion.Length);

                    dversion = dversion.Substring(0, dversion.IndexOf("<", StringComparison.Ordinal));

                    Version = new UpdateVersion
                    {
                        Link = dlink,
                        Version = dversion,
                        Current = Assembly.GetEntryAssembly().GetName().Version
                    };
                }


                if (Version.IsOld)
                    RaiseUpdateRequired();
            }
// ReSharper disable EmptyGeneralCatchClause
            catch
// ReSharper restore EmptyGeneralCatchClause
            {
            }
        }

        public class UpdateVersion
        {
            public bool IsOld
            {
                get
                {
                    if (Current.Major > Major) return true;
                    if (Current.Minor > Minor) return true;
                    return Current.Revision > Revision;
                }
            }


            public Version Current { get; set; }

            public string Version
            {
                get { return String.Format("{0}.{1}.{2}.{3}", Major, Minor, MinorRevision, Revision); }
                set
                {
                    string[] v = value.Split('.');
                    Major = Convert.ToInt32(v[0]);
                    Minor = Convert.ToInt32(v[1]);
                    MinorRevision = Convert.ToInt32(v[2]);
                    Revision = Convert.ToInt32(v[3]);
                }
            }

            public int Major { get; set; }
            public int Minor { get; set; }
            public int MinorRevision { get; set; }
            public int Revision { get; set; }
            public string Link { get; set; }
        }
    }
}