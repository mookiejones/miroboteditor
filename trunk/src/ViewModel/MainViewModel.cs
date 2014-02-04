using System;
using System.Linq;
using System.Text.RegularExpressions;
using GalaSoft.MvvmLight;
using MahApps.Metro;

namespace miRobotEditor.ViewModel
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

        #region Fields
        [NonSerialized]
        private Accent _currentAccent = ThemeManager.DefaultAccents.First(x => x.Name == "Blue");

        public Accent CurrentAccent { get { return _currentAccent; } set { _currentAccent = value; } }

        private Theme _currentTheme = ThemeManager.ThemeIsDark ? Theme.Dark : Theme.Light;

        public Theme Theme { get; set; }
        #endregion
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
    }
}