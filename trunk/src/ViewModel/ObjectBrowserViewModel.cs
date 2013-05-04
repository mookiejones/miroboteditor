using System;
using System.Collections.Generic;
using miRobotEditor.ViewModel;
using System.IO;
using miRobotEditor.Classes;
using miRobotEditor.GUI;
namespace miRobotEditor.ViewModel
{
    public class ObjectBrowserViewModel:ViewModelBase
    {

        //TODO This could easily be rewritten for the grid
    	
    	 private System.Windows.Visibility _ShouldBeVisible = System.Windows.Visibility.Collapsed;
    	 public System.Windows.Visibility ShouldBeVisible{get {return _ShouldBeVisible;}set{_ShouldBeVisible=value;RaisePropertyChanged("ShouldBeVisible");}}
        private static ObjectBrowserViewModel _instance;
        public static ObjectBrowserViewModel Instance { get { return _instance ?? new ObjectBrowserViewModel(); } set { _instance = value; } }

        public void ShowWizard(string name)
        {
            //TODO Need to create actual wizard that will let you choose the options that you are looking for within here. 
            string directory = this.GetDirectory();
            if (!Directory.Exists(directory))
            {
                MessageViewModel.Instance.Add("Directory not valid", string.Format("{0} Doesnt Exist or None Selected", directory), MSGIcon.ERROR, true);
            }
            else
            {
                SetContext(directory);
            }
        }

        string GetDirectory()
        {

            //TODO Add Archive Functionality
            var dialog = new System.Windows.Forms.FolderBrowserDialog { Description = "Select Root Directory for Instance" };

            var fn = DummyDoc.Instance.TextBox.Filename;

            if (Directory.Exists(fn))
                dialog.SelectedPath=Path.GetDirectoryName(fn);


            return ((dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) ? dialog.SelectedPath : string.Empty);
        }

        void SetContext(string directory)
        {
            Instance = this;
            this.RootPath = new DirectoryInfo(directory);
            this.GetFiles(this.RootPath.FullName);
            this.AllVariables.AddRange(this.Functions);
            this.AllVariables.AddRange(this.Fields);
            this.AllVariables.AddRange(this.Positions);
            ShouldBeVisible = AllVariables.Count > 0 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }


        
        private DirectoryInfo _rootpath = null;
		public DirectoryInfo RootPath{get{return _rootpath;}set{_rootpath=value;RaisePropertyChanged("RootPath");}}

        public ObjectBrowserViewModel(){}
        public ObjectBrowserViewModel(string directory)
        {

        }
        
        private void GetFiles(string dir)
        {
        	if (Files==null)
        		Files = new List<FileModel>();
            if (this.RootPath.Exists)
            {
                foreach (var d in Directory.GetDirectories(dir))
                {
                    foreach (var f in Directory.GetFiles(d))
                    {
                        var file = new FileInfo(f);
                        var item = DummyDoc.Instance.FileLanguage.GetFile(file.FullName);
                        if (item != null)
                        {
                            this.GetVariables(f);
                            this.Files.Add(item);
                        }
                    }
                    this.GetFiles(d);
                }
            }
        }
        public List<FileModel> Files { get; set; }

        private void GetVariables(string filename)
        {
            var fileLanguage = DummyDoc.Instance.FileLanguage;
            this.Functions.AddRange(VariableBase.GetVariables(filename, fileLanguage.MethodRegex, Global.ImgMethod));
            this.Positions.AddRange(VariableBase.GetVariables(filename, fileLanguage.XYZRegex, Global.ImgXyz));
            this.Fields.AddRange(VariableBase.GetVariables(filename, fileLanguage.FieldRegex, Global.ImgField));
        }

 

 

 

        private List<IVariable> _allvariables = new List<IVariable>();
        public List<IVariable> AllVariables { get { return _allvariables; } set { _allvariables = value;RaisePropertyChanged("AllVariables"); } } 
   		
        private List<IVariable> _functions = new List<IVariable>();
        public List<IVariable> Functions { get { return _functions; } set { _functions = value;RaisePropertyChanged("Functions"); } } 
  
          private List<IVariable> _fields = new List<IVariable>();
        public List<IVariable> Fields { get { return _fields; } set { _fields = value;RaisePropertyChanged("Fields"); } } 
  
          private List<IVariable> _positions = new List<IVariable>();
        public List<IVariable> Positions { get { return _positions; } set { _positions = value;RaisePropertyChanged("Positions"); } } 
  
        public List<IVariable> GetVarForFile(string filename)
        {
            throw new NotImplementedException();
        }
    }
}
