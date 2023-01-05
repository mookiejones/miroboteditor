using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using miRobotEditor.Core.Classes;
using miRobotEditor.Core.Classes.Messaging;
using miRobotEditor.Core.Interfaces;

namespace miRobotEditor.UI.Windows
{
    /// <summary>
    /// Interaction logic for ObjectBrowserView.xaml
    /// </summary>
    public partial class ObjectBrowserView : UserControl
    {
        public ObjectBrowserView()
        {
            InitializeComponent();
        }
    }
    public sealed partial class ObjectBrowserViewModel : ToolViewModel
    {
        #region Constructors
        public ObjectBrowserViewModel():base("Object Browser") { Initialize(); }
        // ReSharper disable UnusedParameter.Local
        public ObjectBrowserViewModel(string directory):base("Object Browser")
        // ReSharper restore UnusedParameter.Local
        {
            Initialize();
        }
        #endregion

        #region Properties
        private static ObjectBrowserViewModel _instance;
        public static ObjectBrowserViewModel Instance { get { return _instance ?? new ObjectBrowserViewModel("Object Browser"); } set { _instance = value; } }



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
            get=>_selectedVariable;
            

            set
            {
                SetProperty(ref _selectedVariable, value);
                
                var msg = new OpenFileMessage(value);
                WeakReferenceMessenger.Default.Send(msg);
            }
        }
        #endregion


        #region Progress
        /// <summary>
        /// The <see cref="Progress" /> property's name.
        /// </summary>
        private const string ProgressPropertyName = "Progress";

        [ObservableProperty]
        private int progress = -1;

       
        #endregion


    
        [ObservableProperty]
        private int progressMax = -1;

       
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
            throw new NotImplementedException();/*
            var main = ServiceLocator.Current.GetInstance<MainViewModel>();
            var ws = main.ActiveEditor as KukaViewModel;


            if (ws == null) return null;



            var v = ws.Data.Variables;

            var result = v.Where(p => p.Type == "e6pos").ToList();


            return new ReadOnlyCollection<IVariable>(result);
                                                 * */
        }


    }

}
