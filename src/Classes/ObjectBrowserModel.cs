using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using miRobotEditor.Controls;

namespace miRobotEditor.Classes
{
	class ObjectBrowserModel:ViewModelBase
    {
		
    	private static ObjectBrowserModel _instance;
    	public static ObjectBrowserModel Instance
    	{
    		get{if (_instance==null)_instance=new ObjectBrowserModel();return _instance;}
    		set{_instance=value;}
    	}
    	
        #region Members
        private List<IVariable> _functions;
        private List<IVariable> _fields;
        private List<IVariable> _positions;
        private List<IVariable> _allvariables;
        
        public List<IVariable> Functions { get{return _functions;} set{_functions=value;OnPropertyChanged("Functions"); }}
       public List<IVariable> Fields { get{return _fields;} set{_fields=value;OnPropertyChanged("Fields"); }}
       public List<IVariable> Positions { get{return _positions;} set{_positions=value;OnPropertyChanged("Positions"); }}
       public List<IVariable> AllVariables  { get{return _allvariables;} set{_allvariables=value;OnPropertyChanged("AllVariables"); }}

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
        
        
        //
        public List<IVariable> GetVarForFile(string filename)
        {
        	List<IVariable> result = new List<IVariable>();
        	 var Robot = DummyDoc.Instance.FileLanguage;
        	 
        //	result.AddRange(VariableBase.GetVariables(filename, Robot.MethodRegex, Global.imgMethod));
            result.AddRange(VariableBase.GetVariables(filename, Robot.XYZRegex, Global.imgXYZ));
            result.AddRange(VariableBase.GetVariables(filename, Robot.FieldRegex, Global.imgField));
            
            return result;
        }

        private void GetVariables(string filename)
        {
            var Robot = DummyDoc.Instance.FileLanguage;

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
                        var backup = DummyDoc.Instance.FileLanguage.GetFile(info);
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
