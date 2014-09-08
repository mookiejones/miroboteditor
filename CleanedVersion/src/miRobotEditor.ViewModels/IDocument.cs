﻿using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using miRobotEditor.Core.Interfaces;
using miRobotEditor.EditorControl;
using miRobotEditor.EditorControl.Languages;
using miRobotEditor.GUI.Editor;

namespace miRobotEditor.ViewModels
{
    public interface IDocument
    {
        void Load(string filepath);
        void SelectText(IVariable variable);
        Visibility Visibility { get; set; }       
        AbstractLanguageClass FileLanguage { get; set; }
        Editor TextBox { get; set; }
        string FilePath { get; set; }
        ImageSource IconSource { get; set; }
        string FileName { get;  }
        string Title { get; set; }
        bool IsDirty { get; set; }
        string ContentId { get; set; }
        bool IsSelected { get; set; }
        bool IsActive { get; set; }
       
        ICommand CloseCommand { get;  }        
    }
}