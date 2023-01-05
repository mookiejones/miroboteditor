using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace miRobotEditor.Core.Classes
{
    public partial class FileViewModel:PaneViewModel
    {
        protected FileViewModel(string filepath) : base(filepath)
        {
            
     
        }


        public string FilePath { get; set; }

        [ObservableProperty]
        private string fileName=string.Empty;
       
         
         [ObservableProperty]
        private bool isDirty;
         
    }
}
