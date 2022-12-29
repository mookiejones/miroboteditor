using System;
using System.IO;

namespace miRobotEditor.ViewModel
{
    public abstract class FileViewModel : PaneViewModel
    {
        #region FilePath

        private string _filePath = String.Empty;

        /// <summary>
        ///     Sets and gets the FilePath property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public string FilePath
        {
            get => _filePath;

            set
            {
                if (_filePath == value)
                {
                    return;
                }

                // ReSharper disable once ExplicitCallerInfoArgument

                _filePath = value;
                // ReSharper disable once ExplicitCallerInfoArgument
                OnPropertyChanged(nameof(FilePath) );
                // ReSharper disable once ExplicitCallerInfoArgument
                OnPropertyChanged(nameof(FileName));
                // ReSharper disable once ExplicitCallerInfoArgument
                OnPropertyChanged(nameof(Title));
                if (File.Exists(_filePath))
                {
                    ContentId = _filePath;
                }
            }
        }

        #endregion

        #region FileName

        /// <summary>
        ///     Sets and gets the FileName property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public string FileName
        {
            get
            {
                string result;
                if (FilePath == null)
                {
                    result = "Noname" + (IsDirty ? "*" : "");
                }
                else
                {
                    result = Path.GetFileName(FilePath) + (IsDirty ? "*" : "");
                }
                return result;
            }
        }

        #endregion

        #region IsDirty

        private bool _isDirty;

        /// <summary>
        ///     Sets and gets the IsDirty property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public bool IsDirty
        {
            get => _isDirty;

            set
            {
                if (_isDirty == value)
                {
                    return;
                }

                // ReSharper disable once ExplicitCallerInfoArgument

                _isDirty = value;
                // ReSharper disable once ExplicitCallerInfoArgument
                OnPropertyChanged(nameof(IsDirty));
                // ReSharper disable once ExplicitCallerInfoArgument
                OnPropertyChanged(nameof(FileName));
            }
        }

        #endregion

        /*
                private static ImageSourceConverter _isc = new ImageSourceConverter();
        */

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
    }
}