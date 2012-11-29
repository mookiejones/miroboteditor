using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ICSharpCode.AvalonEdit;

namespace miRobotEditor.GUI
{
    [Localizable(false),Serializable]
    public class TextEditorOptions : ICSharpCode.AvalonEdit.TextEditorOptions,INotifyPropertyChanged 
    {

        public TextEditorOptions()
        {
            Instance = this;
        }

        public static TextEditorOptions Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TextEditorOptions();
                return _instance;
            }
            set { _instance = value; }
        }

        private static TextEditorOptions _instance;

        bool _enableFolding = true;



        [DefaultValue(true)]
        public bool EnableFolding
        {
            get { return _enableFolding; }
            set
            {
                if (_enableFolding != value)
                {
                    _enableFolding = value;
                    OnPropertyChanged("EnableFolding");
                }
            }
        }

        bool _mouseWheelZoom = true;

        [DefaultValue(true)]
        public bool MouseWheelZoom
        {
            get { return _mouseWheelZoom; }
            set
            {
                if (_mouseWheelZoom != value)
                {
                    _mouseWheelZoom = value;
                    OnPropertyChanged("MouseWheelZoom");
                }
            }
        }

        private bool _enableAnimations = true;
        public bool EnableAnimations { get { return _enableAnimations; } set { _enableAnimations = value;OnPropertyChanged("EnableAnimations"); } }

        private bool _showlinenumbers = true;
        public bool ShowLineNumbers
        {
            get { return _showlinenumbers; }
            set { _showlinenumbers = value;OnPropertyChanged("ShowLineNumbers"); }
        }

        private bool _highlightcurrentline = true;
         public bool HighlightCurrentLine { get { return _highlightcurrentline; } set { _highlightcurrentline = value; } }

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
