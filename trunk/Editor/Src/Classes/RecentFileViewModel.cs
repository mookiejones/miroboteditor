using System;
using System.Windows.Input;
using miRobotEditor.Classes;

namespace DMC_Robot_Editor.Classes
{
    public class RecentFileViewModel
    {
        private int _index;
        private string _fileName;
        private IFileHandler _fileHandler;

        public RecentFileViewModel(int index, string fileName, IFileHandler fileHandler)
        {
            _index = index;
            _fileName = fileName;
            _fileHandler = fileHandler;
        }

        public string FileName
        {
            get { return string.Format("_{0} - {1}", _index + 1, _fileName); }
        }

        public ICommand Open
        {
            get
            {
                return MakeCommand
                    .Do(() => _fileHandler.Open(_fileName));
            }
        }
    }
}


