﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace RobotEditor
{
    public interface IEditorDocument
    {
        Visibility Visibility { get; set; }
     //   AbstractLanguageClass FileLanguage { get; set; }
    //    Editor TextBox { get; set; }
        string FilePath { get; set; }
        ImageSource IconSource { get; set; }
        string FileName { get; }
        string Title { get; set; }
        bool IsDirty { get; set; }
        string ContentId { get; set; }
        bool IsSelected { get; set; }
        bool IsActive { get; set; }

        void Close();

        void Load(string filepath);
    //    void SelectText(IVariable variable);
    }
}
