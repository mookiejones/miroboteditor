using CommunityToolkit.Mvvm.ComponentModel;
using GalaSoft.MvvmLight;

namespace miRobotEditor.ViewModel
{
    public abstract class AbstractPaneViewModel : ObservableRecipient
    {
        private bool _isVisible = true;

        public bool IsVisible { get =>_isVisible; set=>SetProperty(ref _isVisible,value); }
    }
}