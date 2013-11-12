﻿using System.ComponentModel;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using miRobotEditor.Classes;

namespace miRobotEditor.ViewModel
{
    [Localizable(false)]
    public class StatusBarViewModel:ViewModelBase
   {


       #region Constructor
       public StatusBarViewModel()
       {


           //Get Initial Key Status
          GetInitialKeyState();
       }
       #endregion

       #region Commands


       private RelayCommand<object> _keyPressedCommand;
       public RelayCommand<object> KeyPressedCommand
       {
           get
           {
               return _keyPressedCommand ?? (_keyPressedCommand = new RelayCommand<object>(ExecuteManageKeys));
           }
       }

       private void ExecuteManageKeys(object sender)
       {
           ManageKeys(sender,null);
       }

       #endregion

       #region Static Properties
           private static StatusBarViewModel _instance;
           public static StatusBarViewModel Instance { get { return _instance ?? (_instance=new StatusBarViewModel()); } }
       #endregion

       #region Status Bar Items



        private bool _isScrollPressed;
        public bool IsScrollPressed { get { return _isScrollPressed; } set { _isScrollPressed = value;RaisePropertyChanged("IsScrollPressed"); } }
       private bool _isNumPressed;
       public bool IsNumPressed { get { return _isNumPressed; } set { _isNumPressed = value; RaisePropertyChanged("IsNumPressed"); } }
       private bool _isInsPressed;
       public bool IsInsPressed { get { return _isInsPressed; } set { _isInsPressed = value; RaisePropertyChanged("IsInsPressed"); } }
       private bool _isCapsPressed;
       public bool IsCapsPressed { get { return _isCapsPressed; } set { _isCapsPressed = value; RaisePropertyChanged("IsCapsPressed"); } }

       
        #endregion

        public void ManageKeys(object sender, KeyEventArgs e)
        {
            if (e == null) return;
            switch (e.Key)
            {
                case Key.Capital:
                    IsCapsPressed = e.IsToggled;
                    break;
                case Key.Insert:
                    IsInsPressed = e.IsToggled;
                    break;
                case Key.NumLock:
                    IsNumPressed = e.IsToggled;
                    break;
                case Key.Scroll:
                    IsScrollPressed = e.IsToggled;
                    break;
            }
        }


       void GetInitialKeyState()
        {
            IsCapsPressed = NativeMethods.GetKeyState((int)VKeyStates.CapsKey) != 0;
            IsInsPressed = NativeMethods.GetKeyState((int)VKeyStates.InsKey) != 0;
            IsNumPressed = NativeMethods.GetKeyState((int)VKeyStates.NumKey) != 0;
            IsScrollPressed = NativeMethods.GetKeyState((int)VKeyStates.ScrollKey) != 0;
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

    }
}
