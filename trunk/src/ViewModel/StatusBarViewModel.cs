using System.Runtime.InteropServices;
using System.Windows.Input;
using miRobotEditor.Commands;
namespace miRobotEditor.ViewModel
{
   public class StatusBarViewModel:ViewModelBase
   {


       #region Constructor
       public StatusBarViewModel()
       {

           Instance = this;

           //Get Initial Key Status
           ManageKeys(this, null);
       }
       #endregion

       #region Commands

       private RelayCommand _keyPressedCommand;
       public ICommand KeyPressedCommand
       {
           get
           {
               return _keyPressedCommand ?? (_keyPressedCommand = new RelayCommand(param => ManageKeys(param,null), param => true));
           }
       }

       #endregion


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

       

        #region Properties
        public bool IsScrollPressed { get; set; }
        public bool IsNumPressed { get; set; }
        public bool IsInsPressed { get; set; }
        public bool IsCapsPressed { get; set; }
        #endregion

        public void ManageKeys(object sender, KeyEventArgs e)
        {
            IsCapsPressed = NativeMethods.GetKeyState((int)VKeyStates.CapsKey) != 0;
            IsInsPressed = NativeMethods.GetKeyState((int)VKeyStates.InsKey) != 0;
            IsNumPressed = NativeMethods.GetKeyState((int)VKeyStates.NumKey) != 0;
            IsScrollPressed = NativeMethods.GetKeyState((int)VKeyStates.ScrollKey) != 0;

            RaisePropertyChanged("IsInsPressed");
            RaisePropertyChanged("IsNumPressed");
            RaisePropertyChanged("IsScrollPressed");

            RaisePropertyChanged("IsCapsPressed");


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
