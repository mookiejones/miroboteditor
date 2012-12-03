using System;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using miRobotEditor.Classes;
using miRobotEditor.Commands;
using miRobotEditor.Properties;
using Application = System.Windows.Forms.Application;
namespace miRobotEditor.Forms
{
    /// <summary>
    /// Interaction logic for frmUpdateChecker.xaml
    /// </summary>
    public partial class FrmUpdateChecker : Window
    {
        private UpdateCheckerViewModel vm = new UpdateCheckerViewModel();

        public FrmUpdateChecker()
        {
            InitializeComponent();

            vm.UpdateRequired += VmUpdateRequired;
        }

        void VmUpdateRequired()
        {
            this.DataContext = vm;
            this.Activate();
            this.ShowDialog();
        }
    }

    public class UpdateCheckerViewModel:ViewModelBase
    {

        public event UpdateRequiredHandler UpdateRequired;
        private void RaiseUpdateRequired()
        {
            if (UpdateRequired != null)
                UpdateRequired();
        }
        public  UpdateVersion Version {get;set;}

        #region Commands
     
        private  RelayCommand _updatecommand;
        public  ICommand UpdateCommand
        {
            get { return _updatecommand ?? (_updatecommand = new RelayCommand(param => Update(), param => true)); }
        }
        private RelayCommand _cancelcommand;

        public ICommand CancelCommand
        {
            get { return _cancelcommand ?? (_cancelcommand = new RelayCommand(param => Cancel(), param => Version!=null)); }
        }
        #endregion


        #region Properties
        private bool _updateapplication;
        public bool UpdateApplication
        {
            get { return _updateapplication; }
            set
            {
                _updateapplication = value;
                OnPropertyChanged("UpdateApplication");
            }
        }
        public string UpdateText
        {
            get
            {
                return String.Format("A New version of {0} {1} is available. Would you like to Update?", ProductName, Version.Version);
            }
        }

        public string Title
        {
            get { return String.Format("{0} Updater", ProductName); }
        }

        public string ProductName
        {
            get { return Application.ProductName; }
        }


        public bool AskForUpdates
        {
            get { return Settings.Default.CheckForUpdates; }
            set { Settings.Default.CheckForUpdates = value; OnPropertyChanged("AskForUpdates"); }
        }

        #endregion

        public void Update()
        {
            UpdateApplication = true;
        }

        public void Cancel()
        {
            
        }



        public UpdateCheckerViewModel()
        {
            // Should I check for updates
            CheckForUpdates();
        }


        public  class UpdateVersion
        {

           public bool IsOld
           {
               get
               {
                   if (Current.Major > Major) return true;
                   if (Current.Minor > Minor) return true;
                   if (Current.Revision > Revision) return true;

                   return false;
               }
           }


            public Version Current
            {
                get;set;
            }
            public string Version
            {
                get { return String.Format("{0}.{1}.{2}.{3}", Major, Minor, MinorRevision, Revision); }
                set 
                {
                    var v = value.Split('.');
                    Major = Convert.ToInt32(v[0]);
                    Minor = Convert.ToInt32(v[1]);
                    MinorRevision = Convert.ToInt32(v[2]);
                    Revision = Convert.ToInt32(v[3]);
                }
            }
            public int Major {get;set;}
            public int Minor{get;set;}
            public int MinorRevision { get; set; }
            public int Revision{get;set;}
            public string Link { get; set; }
        }

        private void CheckForUpdates()
        {

            try
            {
                UpdateCheckerViewModel.UpdateVersion result;
                const string link = @"https://sites.google.com/site/dmcautomation/home/software/dmc-robot-editor";
                using (var client = new WebClient())
                {
                    var contents = client.DownloadString(link);
                    const string dlLink = "Latest Version</div><div><br /></div><div><br /></div><div>-<a href=\"";
                    var dlink = contents.Substring(contents.IndexOf(dlLink, StringComparison.Ordinal) + dlLink.Length);

                    dlink = dlink.Substring(0, dlink.IndexOf("\"", StringComparison.Ordinal));

                    const string dlVersion = "DMC Robot Editor V";

                    var dversion =
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
            catch
            {
            }
        }
    }
    public delegate void UpdateRequiredHandler();

}
