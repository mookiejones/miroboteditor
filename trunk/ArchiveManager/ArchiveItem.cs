using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveManager
{
    class ArchiveItem:ViewModelBase
    {
        private string _title = string.Empty;
        private string _description = string.Empty;
        private string _directory = string.Empty;

        public string Title { get { return _title; } set { _title = value; RaisePropertyChanged("Title"); } }
        public string Description { get { return _description; } set { _description = value; RaisePropertyChanged("Description"); } }
        public string Directory { get { return _directory; } set { _directory = value; RaisePropertyChanged("Directory"); } }




    }
}
