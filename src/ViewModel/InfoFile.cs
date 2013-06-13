using System.Collections.ObjectModel;
using miRobotEditor.Core;

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

        public string ArchiveName { get { return _archivename; } set { _archivename = value; RaisePropertyChanged(); } }
        public string ArchiveConfigType { get { return _archiveconfigtype; } set { _archiveconfigtype = value; RaisePropertyChanged(); } }
        public string ArchiveDiskNo { get { return _archiveDiskNo; } set { _archiveDiskNo = value; RaisePropertyChanged(); } }
        public string ArchiveID { get { return _archiveID; } set { _archiveID = value; RaisePropertyChanged(); } }
        public string ArchiveDate { get { return _archiveDate; } set { _archiveDate = value; RaisePropertyChanged(); } }
        public string RobotName { get { return _archiveRobotName; } set { _archiveRobotName = value; RaisePropertyChanged(); } }
        public string RobotSerial { get { return _archiveRobotSerial; } set { _archiveRobotSerial = value; RaisePropertyChanged(); } }
        public string KSSVersion { get { return _archiveKssVersion; } set { _archiveKssVersion = value; RaisePropertyChanged(); } }



        private readonly ObservableCollection<Technology> _technologies = new ObservableCollection<Technology>();
        readonly ReadOnlyObservableCollection<Technology> _readonlyTechnology = null;
        public ReadOnlyObservableCollection<Technology> Technologies { get { return _readonlyTechnology ?? new ReadOnlyObservableCollection<Technology>(_technologies); } }

    }
}