using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using miRobotEditor.Core;
using miRobotEditor.Core.Classes;
using miRobotEditor.Core.Interfaces;
using miRobotEditor.Core;
using miRobotEditor.EditorControl.Languages;
using Utilities = miRobotEditor.Core.Classes.Utilities;

namespace miRobotEditor.Core
{
    public class ObjectBrowserViewModel : ToolViewModel
    {
        #region Constructors
        public ObjectBrowserViewModel(){ Initialize(); }
// ReSharper disable UnusedParameter.Local
        public ObjectBrowserViewModel(string directory) 
// ReSharper restore UnusedParameter.Local
        {
            Initialize();
        }
        #endregion

        #region Properties
        private static ObjectBrowserViewModel _instance;
        public static ObjectBrowserViewModel Instance { get { return _instance ?? new ObjectBrowserViewModel(); } set { _instance = value; } }


        
        #region SelectedVariable
        /// <summary>
        /// The <see cref="SelectedVariable" /> property's name.
        /// </summary>
        private const string SelectedVariablePropertyName = "SelectedVariable";

        private IVariable _selectedVariable = null;

        /// <summary>
        /// Sets and gets the SelectedVariable property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public IVariable SelectedVariable
        {
            get
            {
                return _selectedVariable;
            }

            set
            {
                if (_selectedVariable == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedVariablePropertyName);
                _selectedVariable = value;


                var main = ServiceLocator.Current.GetInstance<MainViewModel>();
                throw new NotImplementedException();
//                Workspace.Instance.OpenFile(value);
                RaisePropertyChanged(SelectedVariablePropertyName);
            }
        }
        #endregion

        
        #region Progress
        /// <summary>
        /// The <see cref="Progress" /> property's name.
        /// </summary>
        private const string ProgressPropertyName = "Progress";

        private int _progress = -1;

        /// <summary>
        /// Sets and gets the Progress property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int Progress
        {
            get
            {
                return _progress;
            }

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
        /// The <see cref="ProgressMax" /> property's name.
        /// </summary>
        private const string ProgressMaxPropertyName = "ProgressMax";

        private int _progressMax = -1;

        /// <summary>
        /// Sets and gets the ProgressMax property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int ProgressMax
        {
            get
            {
                return _progressMax;
            }

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
        string GetDirectory()
// ReSharper restore UnusedMember.Local
        {
            throw new NotImplementedException("Need to remove system.windows.forms");

            ////TODO Add Archive Functionality
            //var dialog = new System.Windows.Forms.FolderBrowserDialog { Description = "Select Root Directory for Instance" };

            //var fn = Workspace.Instance.ActiveEditor.TextBox.Filename;

            //if (Directory.Exists(fn))
            //    dialog.SelectedPath = Path.GetDirectoryName(fn);

            //return ((dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) ? dialog.SelectedPath : string.Empty);
        }

     

        void Initialize()
        {
            ContentId = ToolContentId;

        }


        public ReadOnlyCollection<IVariable> GetVarForFile(string filename)
        {

            // if (Positions != null) return Positions;

            var main = ServiceLocator.Current.GetInstance<MainViewModel>();
            var ws = main.ActiveEditor as KukaViewModel;


            if (ws == null) return null;



            var v = ws.Data.Variables;

            var result = v.Where(p => p.Type == "e6pos").ToList();


            return new ReadOnlyCollection<IVariable>(result);
        }


    }

}
