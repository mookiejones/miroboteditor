using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using miRobotEditor.Classes;
using miRobotEditor.Core;
using Global = miRobotEditor.Classes.Global;
using Utilities = miRobotEditor.Classes.Utilities;

namespace miRobotEditor.ViewModel
{
    public class ObjectBrowserViewModel : ToolViewModel
    {
        #region Constructors
        public ObjectBrowserViewModel() : base("Object Browser") { Initialize(); }
// ReSharper disable UnusedParameter.Local
        public ObjectBrowserViewModel(string directory) : base("Object Browser")
// ReSharper restore UnusedParameter.Local
        {
            Initialize();
        }
        #endregion

        #region Properties
        private static ObjectBrowserViewModel _instance;
        public static ObjectBrowserViewModel Instance { get { return _instance ?? new ObjectBrowserViewModel(); } set { _instance = value; } }

        private IVariable _selectedVariable;
        public IVariable SelectedVariable { get { return _selectedVariable; } set { _selectedVariable = value; Workspace.Instance.OpenFile(value); RaisePropertyChanged(); } }

        private int _progress;
        public int Progress { get { return _progress; } set { _progress = value; RaisePropertyChanged(); } }

        private int _progressMax;
        public int ProgressMax { get { return _progressMax; } set { _progressMax = value; RaisePropertyChanged(); } }
        #endregion

        public const string ToolContentId = "ObjectBrowserTool";

        //TODO This could easily be rewritten for the grid




// ReSharper disable UnusedMember.Local
        string GetDirectory()
// ReSharper restore UnusedMember.Local
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


        public ReadOnlyCollection<IVariable> GetVarForFile(string filename)
        {

            // if (Positions != null) return Positions;

            var ws = Workspace.Instance.ActiveEditor as KukaViewModel;

            if (ws == null) return null;

            var v = ws.Data.Variables;

            var result = v.Where(p => p.Type == "e6pos").ToList();


            return new ReadOnlyCollection<IVariable>(result);
        }


    }

}
