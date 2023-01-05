using System.Collections.ObjectModel;
using miRobotEditor.ViewModel;

namespace miRobotEditor.Classes
{
    public class InfoFile 
    {
        private readonly ReadOnlyObservableCollection<Technology> _readonlyTechnology = null;
        private readonly ObservableCollection<Technology> _technologies = new ObservableCollection<Technology>();
        private string _archiveDate = string.Empty;
        private string _archiveDiskNo = string.Empty;
        private string _archiveID = string.Empty;
        private string _archiveKssVersion = string.Empty;
        private string _archiveRobotName = string.Empty;
        private string _archiveRobotSerial = string.Empty;
        private string _archiveconfigtype = string.Empty;
        private string _archivename = string.Empty;


        public string ArchiveName { get; set; }
        public string ArchiveConfigType { get; set; }
        public string ArchiveDiskNo { get; set; }
        public string ArchiveID { get; set; }

        public string ArchiveDate { get; set; }

        public string RobotName { get; set; }
        public string RobotSerial { get; set; }
        public string KSSVersion { get; set; }




        public ReadOnlyObservableCollection<Technology> Technologies => _readonlyTechnology ?? new ReadOnlyObservableCollection<Technology>(_technologies);
    }
}