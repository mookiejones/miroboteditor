using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using ICSharpCode.AvalonEdit;

namespace miRobotEditor.GUI
{
    [Serializable]
    public class TextEditorOptions : ICSharpCode.AvalonEdit.TextEditorOptions,INotifyPropertyChanged 
    {
        public static TextEditorOptions Instance { get { return _instance; } set { _instance = value; } }

        private static TextEditorOptions _instance = new TextEditorOptions();

        bool enableFolding = true;

        [DefaultValue(true)]
        public bool EnableFolding
        {
            get { return enableFolding; }
            set
            {
                if (enableFolding != value)
                {
                    enableFolding = value;
                    OnPropertyChanged("EnableFolding");
                }
            }
        }

        bool mouseWheelZoom = true;

        [DefaultValue(true)]
        public bool MouseWheelZoom
        {
            get { return mouseWheelZoom; }
            set
            {
                if (mouseWheelZoom != value)
                {
                    mouseWheelZoom = value;
                    OnPropertyChanged("MouseWheelZoom");
                }
            }
        }

        private bool _enableAnimations = true;
        public bool EnableAnimations { get { return _enableAnimations; } set { _enableAnimations = value;OnPropertyChanged("EnableAnimations"); } }

        private bool _highlightcurrentline = true;
        public bool HighlightCurrentLine { get { return _highlightcurrentline; } set { _highlightcurrentline = value; } }


        public void BindToTextEditor(Editor.Editor  editor)
        {
            editor.Options = this;
            editor.SetBinding(Control.FontFamilyProperty, new Binding("FontFamily") { Source = this });
            editor.SetBinding(Control.FontSizeProperty, new Binding("FontSize") { Source = this });
            editor.SetBinding(TextEditor.ShowLineNumbersProperty, new Binding("ShowLineNumbers") { Source = this });
            editor.SetBinding(TextEditor.WordWrapProperty, new Binding("WordWrap") { Source = this });
        }

    }
}
