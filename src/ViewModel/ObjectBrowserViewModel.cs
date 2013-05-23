using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using miRobotEditor.ViewModel;
using System.IO;
using System.Linq;
using miRobotEditor.Classes;
using miRobotEditor.GUI;
using System.ComponentModel;
using System.Text.RegularExpressions;
namespace miRobotEditor.ViewModel
{
    public class ObjectBrowserViewModel : ToolViewModel
    {
        #region Constructors
        public ObjectBrowserViewModel() : base("Object Browser") { Initialize(); }
        public ObjectBrowserViewModel(string directory) : base("Object Browser")
        {
            Initialize();
        }
        #endregion

        #region Properties
        private static ObjectBrowserViewModel _instance;
        public static ObjectBrowserViewModel Instance { get { return _instance ?? new ObjectBrowserViewModel(); } set { _instance = value; } }

        private IVariable _selectedVariable;
        public IVariable SelectedVariable { get { return _selectedVariable; } set { _selectedVariable = value; miRobotEditor.Workspace.Instance.OpenFile(value); RaisePropertyChanged("SelectedVariable"); } }

        private int _progress = 0;
        public int Progress { get { return _progress; } set { _progress = value; RaisePropertyChanged("Progress"); } }

        private int _progressMax = 0;
        public int ProgressMax { get { return _progressMax; } set { _progressMax = value; RaisePropertyChanged("ProgressMax"); } }
        #endregion

        public const string ToolContentId = "ObjectBrowserTool";

        //TODO This could easily be rewritten for the grid




        string GetDirectory()
        {

            //TODO Add Archive Functionality
            var dialog = new System.Windows.Forms.FolderBrowserDialog { Description = "Select Root Directory for Instance" };

            var fn = Workspace.Instance.ActiveEditor.TextBox.Filename;

            if (Directory.Exists(fn))
                dialog.SelectedPath = Path.GetDirectoryName(fn);

            return ((dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) ? dialog.SelectedPath : string.Empty);
        }

     

        void Initialize()
        {
            ContentId = ToolContentId;
            IconSource = Utilities.GetIcon(Global.IconObjectBrowser);
            DefaultPane = DefaultToolPane.Bottom;
        }


        public object DataContext { get { return Workspace.Instance.ActiveEditor; } }

        public ReadOnlyCollection<IVariable> GetVarForFile(string filename)
        {

            // if (Positions != null) return Positions;

            var ws = Workspace.Instance.ActiveEditor as KukaViewModel;

            if (ws == null) return null;

            var v = ws.Data.Variables;

            List<IVariable> result = new List<IVariable>();
            foreach (var p in v.Where(p => p.Type == "e6pos"))
                result.Add(p);



            return new ReadOnlyCollection<IVariable>(result);
        }


    }

}
