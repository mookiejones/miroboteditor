using System.Windows;
using System.Windows.Media;
using GalaSoft.MvvmLight.Command;
using miRobotEditor.Classes;
using miRobotEditor.GUI;
using miRobotEditor.Languages;

namespace miRobotEditor.ViewModel
{
   

    public interface IDocument
    {

      
        void Load(string filepath);
        void SelectText(IVariable variable);
        Visibility Visibility { get; set; }       
        AbstractLanguageClass FileLanguage { get; set; }
        GUI.EditorClass TextBox { get; set; }
        string FilePath { get; set; }
        ImageSource IconSource { get; set; }
        string FileName { get;  }
        string Title { get; set; }
        bool IsDirty { get; set; }
        string ContentId { get; set; }
        bool IsSelected { get; set; }
        bool IsActive { get; set; }
       
        RelayCommand CloseCommand { get;  }        
    }
}