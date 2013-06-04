using System.Collections.ObjectModel;

namespace miRobotEditor.ViewModel
{
    public class InfoFile:ViewModelBase
    {
        private string _archivename = string.Empty;
        private string _archiveconfigtype = string.Empty;
        private string _archiveDiskNo = string.Empty;
        private string _archiveID = string.Empty;
        private string _archiveDate = string.Empty;
        private string _archiveRobotName = string.Empty;
        private string _archiveRobotSerial = string.Empty;
        private string _archiveKssVersion = string.Empty;

        public string ArchiveName { get { return _archivename; } set { _archivename = value; RaisePropertyChanged("ArchiveName"); } }
        public string ArchiveConfigType { get { return _archiveconfigtype; } set { _archiveconfigtype = value; RaisePropertyChanged("ArchiveConfigType"); } }
        public string ArchiveDiskNo { get { return _archiveDiskNo; } set { _archiveDiskNo = value; RaisePropertyChanged("ArchiveDiskNo"); } }
        public string ArchiveID { get { return _archiveID; } set { _archiveID = value; RaisePropertyChanged("ArchiveID"); } }
        public string ArchiveDate { get { return _archiveDate; } set { _archiveDate = value; RaisePropertyChanged("ArchiveDate"); } }
        public string RobotName { get { return _archiveRobotName; } set { _archiveRobotName = value; RaisePropertyChanged("RobotName"); } }
        public string RobotSerial { get { return _archiveRobotSerial; } set { _archiveRobotSerial = value; RaisePropertyChanged("RobotSerial"); } }
        public string KSSVersion { get { return _archiveKssVersion; } set { _archiveKssVersion = value; RaisePropertyChanged("KSSVersion"); } }



        private readonly ObservableCollection<Technology> _technologies = new ObservableCollection<Technology>();
        readonly ReadOnlyObservableCollection<Technology> _readonlyTechnology = null;
        public ReadOnlyObservableCollection<Technology> Technologies { get { return _readonlyTechnology ?? new ReadOnlyObservableCollection<Technology>(_technologies); } }

    }
}