using System.Runtime.InteropServices;
using System.Windows.Input;
using miRobotEditor.ViewModel;

namespace miRobotEditor.GUI
{
   public class StatusBarViewModel:ViewModelBase
   {


       public StatusBarViewModel()
       {
           //Get Initial Key Status
           ManageKeys(null, null);

           Instance = this;
       }

       private static StatusBarViewModel _instance;
       public static StatusBarViewModel Instance { get { return _instance ?? new StatusBarViewModel(); } set { _instance = value; } }

       private int _line = 0;
       private int _column = 0;
       private int _offset = 0;
       private string _robot = string.Empty;
       private string _name = string.Empty;
       public string Robot { get { return _robot; } set { _robot = value;RaisePropertyChanged("Robot"); } }
       private string _filesave = string.Empty;
       public string FileSave { get { return _filesave; } set { _filesave = value; RaisePropertyChanged("FileSave"); } }
        public int Line { get { return _line; } set { _line = value;RaisePropertyChanged("Line"); } }
        public int Column { get { return _column; } set { _column = value; RaisePropertyChanged("Column"); } }
        public int Offset { get { return _offset; } set { _offset = value; RaisePropertyChanged("Offset"); } }
        public string Name{get{return _name;}set{_name = value;RaisePropertyChanged("Name");}}
    
        #region Status Bar Items

        [DllImport("user32.dll")]
        internal static extern short GetKeyState(int keyCode);

        #region Properties
        private bool _isScrollPressed;
        private bool _isNumPressed;
        private bool _isInsPressed;
        private bool _isCapsPressed;

        public bool IsScrollPressed
        {
            get { return _isScrollPressed; }
            set
            {
                _isScrollPressed = value;
                RaisePropertyChanged("IsScrollPressed");
            }
        }
        public bool IsNumPressed
        {
            get { return _isNumPressed; }
            set
            {
                _isNumPressed = value;
                RaisePropertyChanged("IsNumPressed");
            }
        }
        public bool IsInsPressed
        {
            get { return _isInsPressed; }
            set
            {
                _isInsPressed = value;
                RaisePropertyChanged("IsInsPressed");
            }
        }
        public bool IsCapsPressed
        {
            get { return _isCapsPressed; }
            set
            {
                _isCapsPressed = value;
                RaisePropertyChanged("IsCapsPressed");
            }
        }
        #endregion

        private void ManageKeys(object sender, KeyEventArgs e)
        {
            IsCapsPressed = GetKeyState((int)VKeyStates.CapsKey) != 0;
            IsInsPressed = GetKeyState((int)VKeyStates.InsKey) != 0;
            IsNumPressed = GetKeyState((int)VKeyStates.NumKey) != 0;
            IsScrollPressed = GetKeyState((int)VKeyStates.ScrollKey) != 0;
        }

        private enum VKeyStates
        {
            /// <summary>
            /// the caps lock key
            /// </summary>
            CapsKey = 0x14,
            /// <summary>
            /// the numlock key
            /// </summary>
            NumKey = 0x90,
            /// <summary>
            /// the scroll key
            /// </summary>
            ScrollKey = 0x91,
            /// <summary>
            /// the ins key
            /// </summary>
            InsKey = 0x2d
        }

        #endregion

    }
}
