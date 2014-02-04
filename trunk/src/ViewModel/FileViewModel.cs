using System.Windows.Media;
using System.IO;
using miRobotEditor.Core;

namespace miRobotEditor.ViewModel
{
  public  class FileViewModel : PaneViewModel
    {
#pragma warning disable 169
        static ImageSourceConverter _isc = new ImageSourceConverter();
#pragma warning restore 169
        public FileViewModel(string filePath)
        {
            FilePath = filePath;
            Title = FileName;
    
        }
      
        public FileViewModel()
        {
            IsDirty = true;
            Title = FileName;
        }

        

        #region FilePath
        /// <summary>
        /// The <see cref="FilePath" /> property's name.
        /// </summary>
        public const string FilePathPropertyName = "FilePath";

        private string _filePath = string.Empty;

        /// <summary>
        /// Sets and gets the FilePath property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string FilePath
        {
            get
            {
                return _filePath;
            }

            set
            {
                if (_filePath == value)
                {
                    return;
                }

                RaisePropertyChanging(FilePathPropertyName);
                _filePath = value;
                RaisePropertyChanged(FilePathPropertyName);
                RaisePropertyChanged("FilePath");
                RaisePropertyChanged("FileName");
                RaisePropertyChanged("Title");

                if (File.Exists(_filePath))
                {
                    ContentId = _filePath;
                }
            }
        }
        #endregion
        
    

        public string FileName
        {
            get
            {
                if (FilePath == null)
                    return "Noname" + (IsDirty ? "*" : "");

                return Path.GetFileName(FilePath) + (IsDirty ? "*" : "");
            }
        }



        #region IsDirty

        private bool _isDirty;
        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                if (_isDirty == value) return;
                _isDirty = value;
                RaisePropertyChanged("IsDirty");
                RaisePropertyChanged("FileName");
            }
        }

        #endregion


    }
 
}
