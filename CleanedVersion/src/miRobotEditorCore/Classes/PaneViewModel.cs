using System;
using CommunityToolkit.Mvvm.ComponentModel;
 

namespace miRobotEditor.Core.Classes
{
    public partial class PaneViewModel:ObservableRecipient
    {
// ReSharper disable once UnusedParameter.Local
        protected PaneViewModel(string filename = "")
        {
            
        }

        [ObservableProperty]
        private string contentId = string.Empty;
      

        [ObservableProperty]
        private string title = string.Empty;
         
        [ObservableProperty]
        private string name = string.Empty;
         
        
      
    }
}
