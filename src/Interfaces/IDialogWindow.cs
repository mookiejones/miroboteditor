using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
namespace miRobotEditor.Interfaces
{
    class IDialogWindow
    {
        string Title { get; set; }
        string Description { get; set; }
        ICommand OKCommand { get; set; }
        ICommand CancelCommand { get; set; }
    }
}
