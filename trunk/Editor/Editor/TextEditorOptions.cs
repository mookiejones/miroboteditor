using System;
using System.ComponentModel;
using ICSharpCode.AvalonEdit;

namespace miRobotEditor.GUI.Editor
{
    [Localizable(false),Serializable]
    public class TextEditorOptions:ICSharpCode.AvalonEdit.TextEditorOptions
    {

        public TextEditorOptions()
        {
            _instance = this;
        }

        private static  TextEditorOptions _instance;
        public static TextEditorOptions Instance
        {
            get { return _instance; }
            set { _instance = value; }
        }

        bool highlightcurrentline = true;

        private bool enableanimations = true;
        public bool EnableAnimations
        {
            get { return this.enableanimations; }
            set { enableanimations = value;
            OnPropertyChanged("EnableAnimations");}
        }

        [DefaultValue(true)]
        public bool HighlightCurrentLine
        {
            get { return highlightcurrentline; }
            set
            {
                if (highlightcurrentline != value)
                {
                    highlightcurrentline = value;
                    OnPropertyChanged("HighlightCurrentLine");
                }
            }
        }

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

        public void BindToTextEditor(Editor editor)
        {
            editor.Options = this;
            editor.SetBinding(Control.FontFamilyProperty, new Binding("FontFamily") { Source = this });
            editor.SetBinding(Control.FontSizeProperty, new Binding("FontSize") { Source = this });
            editor.SetBinding(TextEditor.ShowLineNumbersProperty, new Binding("ShowLineNumbers") { Source = this });
            editor.SetBinding(TextEditor.WordWrapProperty, new Binding("WordWrap") { Source = this });
        }

    }
}
