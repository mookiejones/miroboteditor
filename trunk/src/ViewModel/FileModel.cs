using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using miRobotEditor.Classes;
using miRobotEditor.Classes.Messages;
using miRobotEditor.Languages;
using miRobotEditor.Properties;
using miRobotEditor.WindowMessages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace miRobotEditor.ViewModel
{
    public class FileModel:ViewModelBase
    {

        private IDocument _activeEditor;

        public IDocument ActiveEditor
        {
            get
            {
                return _activeEditor;
            }
            set
            {
                _activeEditor = value;

                lock (_activeEditor)
                {
                    //TODO 8/19/2013 Changed this and moved inside the lock;

                    RaisePropertyChanged("ActiveEditor");
                    RaisePropertyChanged("Title");
                    //            if (ActiveEditorChanged != null)
                    //                ActiveEditorChanged(this, EventArgs.Empty);
                }
            }
        }
        public FileModel()
        {
            Files = new ObservableCollection<IDocument>();
        }

        private ObservableCollection<IDocument> Files{get;set;}




        private void TextBox_FilenameChanged(object sender, EventArgs e)
        {
            var editor = sender as EditorClass;
            Debug.Assert(editor != null, "editor != null");
            var fileViewModel = Files.FirstOrDefault(fi => fi.ContentId == editor.Tag.ToString());

            Debug.Assert(fileViewModel != null, "fileViewModel != null");

            fileViewModel.ContentId = editor.Filename;
            RaisePropertyChanged("Title");

            Console.WriteLine();
        }



        private void OpenFile(OpenFileMessage msg)
        {
            OpenFile(msg.FileName);
        }



        public IDocument Open(string filepath)
        {
            var fileViewModel = OpenFile(filepath);

            ActiveEditor = fileViewModel;
            ActiveEditor.TextBox.FilenameChanged += TextBox_FilenameChanged;

            ActiveEditor.IsActive = true;
            return fileViewModel;
        }



        /// <summary>
        /// Gets document type of file to open
        /// </summary>
        /// <param name="filepath">Destination of file</param>
        /// <returns>Document Type</returns>
        private IDocument OpenFile(string filepath)
        {
            var fileViewModel = Files.FirstOrDefault(fm => fm.ContentId == filepath);

            if (fileViewModel != null)
            {
                fileViewModel.IsSelected = true;
                fileViewModel.IsActive = true;
                ActiveEditor = fileViewModel;
                return fileViewModel;
            }

            fileViewModel = AbstractLanguageClass.GetViewModel(filepath);

            if (File.Exists(filepath))
            {
                fileViewModel.Load(filepath);
                fileViewModel.TextBox.Tag = filepath;
                // Add file to Recent list
                RecentFileList.Instance.InsertFile(filepath);
                System.Windows.Shell.JumpList.AddToRecentCategory(filepath);
            }

            // 7/23/2013 Changed order
            Files.Add(fileViewModel);
            fileViewModel.IsActive = true;
            fileViewModel.IsSelected = true;

            ActiveEditor = fileViewModel;
            return fileViewModel;
        }

        public void OpenFile(IVariable variable)
        {
            // Am i using dock or ActiveEditor?

            var fileViewModel = Open(variable.Path);

            fileViewModel.SelectText(variable);
            //            ActiveEditor.TextBox.SelectText(variable);
        }




        public void AddNewFile()
        {
            Files.Add(new DocumentViewModel(null));
            ActiveEditor = Files.Last();
        }

        public void LoadFile(IList<string> args)
        {
            // Argument 0 is The Path of the main application so i start with argument 1

            for (int i = 1; i < args.Count; i++)
                Open(args[i]);
        }

        /// <summary>
        /// Open file from menu entry
        /// </summary>
        /// <param name="param"></param>
        private void OnOpen(object param)
        {
            var gfm = new GetFileMessage("", Settings.Default.Filter, true, GotFileName);

            Messenger.Default.Send<GetFileMessage>(gfm);

          //  var path = Path.GetDirectoryName(ActiveEditor.FilePath);
          //  var dir = Directory.Exists(path) ? path : "C:\\";
          //  var dlg = new OpenFileDialog
          //  {
          //      // Find a way to check for network directory
          //      //                InitialDirectory="C:\\",
          //      Filter = Findahome.DefaultFilter,
          //      Multiselect = true,
          //      FilterIndex = Settings.Default.Filter,
          //  };
          //
          //  if (!dlg.ShowDialog().GetValueOrDefault()) return;
          //  foreach (var file in dlg.FileNames)
          //      Open(file);
        }

        private void GotFileName()
        {
            throw new NotImplementedException();
        }

        private void GotFileName(GetFileMessage msg)
        {
            if (msg.Result)
                foreach (var file in msg.FileNames)
                    Open(file);
        }

    }
}
