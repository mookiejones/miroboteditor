using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CommonServiceLocator;
using miRobotEditor.Classes;
using miRobotEditor.Enums;
using miRobotEditor.Interfaces;
using miRobotEditor.Utilities;

namespace miRobotEditor.ViewModel
{
    public sealed class ObjectBrowserViewModel : ToolViewModel
    {
        private const string ToolContentId = "ObjectBrowserTool";
        private int _progress;
        private int _progressMax;
        private IVariable _selectedVariable;

        #region Properties

        public IVariable SelectedVariable
        {
            get { return _selectedVariable; }
            set
            {
                _selectedVariable = value;
                var instance = ServiceLocator.Current.GetInstance<MainViewModel>();
                instance.OpenFile(value);
                RaisePropertyChanged("SelectedVariable");
            }
        }

        public int Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                RaisePropertyChanged("Progress");
            }
        }

        public int ProgressMax
        {
            get { return _progressMax; }
            set
            {
                _progressMax = value;
                RaisePropertyChanged("ProgressMax");
            }
        }

        #endregion

        public ObjectBrowserViewModel()
            : base("Object Browser")
        {
            DefaultPane = DefaultToolPane.Bottom;
            Initialize();
        }

        private string GetDirectory()
        {
            var folderBrowserDialog = new FolderBrowserDialog
            {
                Description = @"Select Root Directory for Instance"
            };
            var instance = ServiceLocator.Current.GetInstance<MainViewModel>();
            var filename = instance.ActiveEditor.TextBox.Filename;
            if (Directory.Exists(filename))
            {
                folderBrowserDialog.SelectedPath = Path.GetDirectoryName(filename);
            }
            return (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                ? folderBrowserDialog.SelectedPath
                : string.Empty;
        }

        private void Initialize()
        {
            ContentId = ToolContentId;
            IconSource=ImageHelper.GetIcon("pack://application:,,/Images/resources-objectbrowser.png");
            //  IconSource = Utilities.GetIcon("pack://application:,,/Images/resources-objectbrowser.png");
            DefaultPane = DefaultToolPane.Bottom;
        }

        public ReadOnlyCollection<IVariable> GetVarForFile(string filename)
        {
            var instance = ServiceLocator.Current.GetInstance<MainViewModel>();
            var kukaViewModel = instance.ActiveEditor as KukaViewModel;
            ReadOnlyCollection<IVariable> result;
            if (kukaViewModel == null)
            {
                result = null;
            }
            else
            {
                var variables = kukaViewModel.Data.Variables;
                var list = (
                    from p in variables
                    where p.Type == "e6pos"
                    select p).ToList<IVariable>();
                result = new ReadOnlyCollection<IVariable>(list);
            }
            return result;
        }
    }
}