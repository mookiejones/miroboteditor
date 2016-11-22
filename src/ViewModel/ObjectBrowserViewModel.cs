using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Practices.ServiceLocation;
using miRobotEditor.Classes;
using miRobotEditor.Core;
using robot_editor.Interfaces;
using Global = miRobotEditor.Classes.Global;
using Utilities = miRobotEditor.Classes.Utilities;

namespace miRobotEditor.ViewModel
{
    public class ObjectBrowserViewModel : ToolViewModel
    {
        #region Constructors

        public ObjectBrowserViewModel() : base("Object Browser")
        {
            Initialize();
        }

        #endregion

        #region Properties

        #region Selected Variable

        /// <summary>
        ///     The <see cref="SelectedVariable" /> property's name.
        /// </summary>
        public const string SelectedVariablePropertyName = "SelectedVariable";

        private IVariable _selectedVariable;

        /// <summary>
        ///     Sets and gets the SelectedVariable property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public IVariable SelectedVariable
        {
            get { return _selectedVariable; }

            set
            {
                if (_selectedVariable == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedVariablePropertyName);
                var main = ServiceLocator.Current.GetInstance<MainViewModel>();
                main.OpenFile(value);
                _selectedVariable = value;
                RaisePropertyChanged(SelectedVariablePropertyName);
            }
        }

        #endregion

        #region Progress

        /// <summary>
        ///     The <see cref="Progress" /> property's name.
        /// </summary>
        public const string ProgressPropertyName = "Progress";

        private int _progress = -1;

        /// <summary>
        ///     Sets and gets the Progress property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public int Progress
        {
            get { return _progress; }

            set
            {
                if (_progress == value)
                {
                    return;
                }

                RaisePropertyChanging(ProgressPropertyName);
                _progress = value;
                RaisePropertyChanged(ProgressPropertyName);
            }
        }

        #endregion

        #region ProgressMax

        /// <summary>
        ///     The <see cref="ProgressMax" /> property's name.
        /// </summary>
        public const string ProgressMaxPropertyName = "ProgressMax";

        private int _progressMax = -1;

        /// <summary>
        ///     Sets and gets the ProgressMax property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public int ProgressMax
        {
            get { return _progressMax; }

            set
            {
                if (_progressMax == value)
                {
                    return;
                }

                RaisePropertyChanging(ProgressMaxPropertyName);
                _progressMax = value;
                RaisePropertyChanged(ProgressMaxPropertyName);
            }
        }

        #endregion

        #endregion

        public const string ToolContentId = "ObjectBrowserTool";

        //TODO This could easily be rewritten for the grid


// ReSharper disable UnusedMember.Local
        private string GetDirectory()
// ReSharper restore UnusedMember.Local
        {
            //TODO Add Archive Functionality
            var dialog = new FolderBrowserDialog {Description = "Select Root Directory for Instance"};

            var main = ServiceLocator.Current.GetInstance<MainViewModel>();
            string fn = main.ActiveEditor.TextBox.Filename;

            if (Directory.Exists(fn))
                dialog.SelectedPath = Path.GetDirectoryName(fn);

            return ((dialog.ShowDialog() == DialogResult.OK) ? dialog.SelectedPath : string.Empty);
        }


        private void Initialize()
        {
            ContentId = ToolContentId;
            IconSource = Utilities.GetIcon(Global.IconObjectBrowser);
            DefaultPane = DefaultToolPane.Bottom;
        }


        public ReadOnlyCollection<IVariable> GetVarForFile(string filename)
        {
            // if (Positions != null) return Positions;
            var main = ServiceLocator.Current.GetInstance<MainViewModel>();
            var ws = main.ActiveEditor as KukaViewModel;

            if (ws == null) return null;

            ReadOnlyObservableCollection<IVariable> v = ws.Data.Variables;

            List<IVariable> result = v.Where(p => p.Type == "e6pos").ToList();


            return new ReadOnlyCollection<IVariable>(result);
        }
    }
}