using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RobotEditor.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
        }


        private readonly IEnumerable<ToolViewModel> _readonlyTools = null;
        private readonly ObservableCollection<ToolViewModel> _tools = new ObservableCollection<ToolViewModel>();
        public IEnumerable<ToolViewModel> Tools
        {
            get { return _readonlyTools ?? new ObservableCollection<ToolViewModel>(_tools); }


        }

        #region Files

        private readonly ObservableCollection<IEditorDocument> _files = new ObservableCollection<IEditorDocument>();
        private readonly ReadOnlyObservableCollection<IEditorDocument> _readonlyFiles = null;

        public IEnumerable<IEditorDocument> Files
        {
            get { return _readonlyFiles ?? new ReadOnlyObservableCollection<IEditorDocument>(_files); }
        }

        #endregion
    }
}