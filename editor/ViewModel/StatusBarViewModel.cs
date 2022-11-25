using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RobotEditor.Classes;

namespace RobotEditor.ViewModel
{
    public sealed class StatusBarViewModel : ViewModelBase
    {



        private static StatusBarViewModel _instance;
        private bool _isCapsPressed;
        private bool _isInsPressed;
        private bool _isNumPressed;
        private bool _isScrollPressed;
        private RelayCommand<object> _keyPressedCommand;

        public StatusBarViewModel()
        {
            GetInitialKeyState();
        }




        public RelayCommand<object> KeyPressedCommand
        {
            get
            {
                return _keyPressedCommand ??
                       (_keyPressedCommand = new RelayCommand<object>(param => ManageKeys(param, null)));
            }
        }

        public static StatusBarViewModel Instance
        {
            get
            {
                return _instance ?? (_instance = new StatusBarViewModel());
            }
        }

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

        public void ManageKeys(object sender, KeyEventArgs e)
        {
            if (e != null)
            {
                var key = e.Key;
                if (key != Key.Capital)
                {
                    if (key != Key.Insert)
                    {
                        switch (key)
                        {
                            case Key.NumLock:
                                IsNumPressed = e.IsToggled;
                                break;
                            case Key.Scroll:
                                IsScrollPressed = e.IsToggled;
                                break;
                        }
                    }
                    else
                    {
                        IsInsPressed = e.IsToggled;
                    }
                }
                else
                {
                    IsCapsPressed = e.IsToggled;
                }
            }
        }

        private void GetInitialKeyState()
        {
            IsCapsPressed = (NativeMethods.GetKeyState(20) != 0);
            IsInsPressed = (NativeMethods.GetKeyState(45) != 0);
            IsNumPressed = (NativeMethods.GetKeyState(144) != 0);
            IsScrollPressed = (NativeMethods.GetKeyState(145) != 0);
        }

        private enum VKeyStates
        {
            CapsKey = 20,
            NumKey = 144,
            ScrollKey,
            InsKey = 45
        }
    }
}