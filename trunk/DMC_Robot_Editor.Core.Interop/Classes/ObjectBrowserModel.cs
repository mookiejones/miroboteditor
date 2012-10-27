using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using miRobotEditor.Controls;

namespace miRobotEditor.Classes
{
    class ObjectBrowserModel
    {
        private static ObjectBrowserModel _instance = new ObjectBrowserModel();
        public static ObjectBrowserModel Instance
        {
            get { return _instance; }
            set { _instance = value;}
        }

        #region Members
       public List<IVariable> Functions { get; set; }
       public List<IVariable> Fields { get; set; }
       public List<IVariable> Positions { get; set; }
       public List<IVariable> AllVariables { get; set; }

        public ObservableCollection<FileModel> Files { get; set; }
        #endregion


        /// <summary>
        /// Root Path of Instance
        /// </summary>
        public DirectoryInfo RootPath
        {
            get;
            set;
        }


        private void Initialize()
        {
            Functions = new List<IVariable>();
            Fields = new List<IVariable>();
            Positions = new List<IVariable>();
            AllVariables=new List<IVariable>();
            Files = new ObservableCollection<FileModel>();
        }

        public ObjectBrowserModel()
        {
            Initialize();
        }
        public ObjectBrowserModel(string directory)
        {

            Initialize();
            RootPath = new DirectoryInfo(directory);
            GetFiles(RootPath.FullName);

            AllVariables.AddRange(Functions);
            AllVariables.AddRange(Fields);
            AllVariables.AddRange(Positions);

            Instance = this;
        }


        private void GetVariables(string filename)
        {
            var Robot = DummyDoc.ActiveEditor.FileLanguage;

            Functions.AddRange(VariableBase.GetVariables(filename, Robot.MethodRegex, Global.imgMethod));
            Positions.AddRange(VariableBase.GetVariables(filename, Robot.XYZRegex, Global.imgXYZ));
            Fields.AddRange(VariableBase.GetVariables(filename, Robot.FieldRegex, Global.imgField));
        }

        //TODO Move this to FileLanguage Classes along with variables
        private void GetFiles(string dir)
        {
            if (RootPath.Exists)
                foreach (string d in Directory.GetDirectories(dir))
                {
                    foreach (string f in Directory.GetFiles(d))
                    {
                        var info = new FileInfo(f);
                        var backup = DummyDoc.ActiveEditor.FileLanguage.GetFile(info);
                        if (backup != null)
                        {
                            GetVariables(f);

                            Files.Add(backup);
                        }
                    }
                    GetFiles(d);
                }
        }




    }
}
