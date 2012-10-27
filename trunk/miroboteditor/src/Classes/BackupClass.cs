using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using miRobotEditor.Controls;
using miRobotEditor.Enums;
using miRobotEditor.Robot;

namespace miRobotEditor.Classes
{
    public class BackupClass
    {
        private static ObservableCollection<string> _variables= new ObservableCollection<string>(); 

        public static ObservableCollection<string> Variables { get { return _variables; } set { _variables = value; } }
        private static ObservableCollection<VariableItem> _functions = new ObservableCollection<VariableItem>();

        public static ObservableCollection<VariableItem> Functions { get { return _functions; } set { _functions = value; } }
        private static ObservableCollection<BackupFile> _files = new ObservableCollection<BackupFile>();


        /// <summary>
        /// Gets or sets a value that indicates the CollapseMode.
        /// </summary>
        public static ObservableCollection<BackupFile> Files
        {
            get { return _files; }          
            set { _files=value; }
        }
      

      
        private void UnpackArchive()
        {
            throw new NotImplementedException();
        }

        private string GetDirectory()
        {
            var fbd = new FolderBrowserDialog();
            fbd.Description = "Select Root Directory for Instance";
            fbd.SelectedPath="C:\\work\\backups\\tampa";

            var result = fbd.ShowDialog();

            return (result == DialogResult.OK) ? fbd.SelectedPath : string.Empty;
        }
        
        private string GetArchive()
        {
            return String.Empty;
        }
        private void GetVariables()
        {
            //TODO: This is going to need to be threaded

        }
        public void ShowWizard(string menuselection)
        {

            string dir = GetDirectory();
            if (!Directory.Exists(dir))
            {
                OutputMessages.Add("Directory not valid",String.Format("{0} Doesnt Exist or None Selected",dir),null);
                return;
            }

            RootPath= new DirectoryInfo(dir);

            GetFiles(RootPath.FullName);
            GetVariables();
        }


        /// <summary>
        /// Root Path of Instance
        /// </summary>
        public DirectoryInfo RootPath
        {
            get; set;
        }

        //TODO Move this to Robot Classes along with variables
        private void GetFiles(string dir)
        {
            if (RootPath.Exists)
                foreach(string d in Directory.GetDirectories(dir))
                {
                    foreach(string f in Directory.GetFiles(d))
                    {
                        var info = new FileInfo(f);

                        var backup = DummyDoc.ActiveEditor.Robot.GetFile(info);
                        if (backup != null)
                        {
                            Files.Add(backup);
                        }
                    }
                    GetFiles(d);
                }
        }    
    }
    public class BackupFile:DependencyObject
    {
        public BitmapImage Icon { get; set; }
        public FileInfo File { get; set; }
    }

    

}
