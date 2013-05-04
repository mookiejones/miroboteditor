using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System.Windows.Media;
using miRobotEditor.Languages;
namespace miRobotEditor.GUI
{
    [Localizable(false),Serializable]
    public class TextEditorOptions : ICSharpCode.AvalonEdit.TextEditorOptions
    {
        //#region Overrides

        //private bool _convertTabsToSpaces = false;
        //public override bool ConvertTabsToSpaces{get{return _convertTabsToSpaces;}set{_convertTabsToSpaces=value;OnPropertyChanged("ConvertTabsToSpaces");}}

        //private bool _cutCopyWholeLine = false;
        //public override bool CutCopyWholeLine { get { return _cutCopyWholeLine; } set { _cutCopyWholeLine = value; OnPropertyChanged("CutCopyWholeLine"); } }

        //private bool _enableEmailHyperLinks = true;
        //public override bool EnableEmailHyperlinks { get { return _enableEmailHyperLinks; } set { _enableEmailHyperLinks = value; OnPropertyChanged("EnableEmailHyperlinks"); } }

      
        //private bool _enableHyperlinks = true;
        //public override bool EnableHyperlinks { get { return _enableHyperlinks; } set { _enableHyperlinks = value; OnPropertyChanged("EnableHyperlinks"); } }

        //private bool _enableVirtualSpace = false;
        //public override bool EnableVirtualSpace { get { return _enableVirtualSpace; } set { _enableVirtualSpace = value; OnPropertyChanged("EnableVirtualSpace"); } }

        //private bool _requireControlModifierForHyperlinkClick = true;
        //public override bool RequireControlModifierForHyperlinkClick { get { return _requireControlModifierForHyperlinkClick; } set { _requireControlModifierForHyperlinkClick = value; OnPropertyChanged("RequireControlModifierForHyperlinkClick"); } }

        //private bool _showEndOfLine = true;
        //public override bool ShowEndOfLine { get { return _showEndOfLine; } set { _showEndOfLine = value; OnPropertyChanged("ShowEndOfLine"); } }

        //private bool _showTabs = true;
        //public override bool ShowTabs { get { return _showTabs; } set { _showTabs = value; OnPropertyChanged("ShowTabs"); } }
    
     
        //#endregion

        private bool _wrapWords = true;
        public  bool WrapWords { get { return _wrapWords; } set { _wrapWords = value; OnPropertyChanged("WrapWords"); } }

        private Brush _selectedlinecolor = Brushes.Yellow;

        /// <summary>
        /// Color of Currently Selected Line
        /// </summary>
        public Brush HighlightedLineColor { get { return _selectedlinecolor; } set { _selectedlinecolor = value; OnPropertyChanged("HighlightedLineColor"); } }
        //System.Windows.SystemColors.HighlightColor
        private Brush _selectedTextBackground = new SolidColorBrush{ Color=Colors.SteelBlue, Opacity=0.7};
        public Brush SelectedTextBackground { get { return _selectedlinecolor; } set { _selectedlinecolor = value; OnPropertyChanged("SelectedTextBackground"); } }

        private Brush _selectedTextForeground = new SolidColorBrush { Color = System.Windows.SystemColors.HighlightColor, Opacity = 0.7 };
        public Brush SelectedTextForeground { get { return _selectedTextForeground; } set { _selectedTextForeground = value; OnPropertyChanged("SelectedTextForeground"); } }

        private Border _foldToolTipBackground = new Border { BorderBrush = Brushes.Black, Background = Brushes.WhiteSmoke };
        public Border FoldToolTipBackground { get { return _foldToolTipBackground; } set { _foldToolTipBackground = value; OnPropertyChanged("FoldToolTipBackground"); } }


        private Pen _selectedTextBorder = new Pen{Brush=Brushes.Orange, Thickness=1};
        public Pen SelectedTextBorder { get { return _selectedTextBorder; } set { _selectedTextBorder = value; OnPropertyChanged("SelectedTextBorder"); } }

        private System.Text.Encoding _encoding = System.Text.Encoding.UTF8;
        public System.Text.Encoding Encoding { get { return _encoding; } set { _encoding=value; OnPropertyChanged("Encoding"); } }

        private bool _highlightcurrentline = true;
        public bool HighlightCurrentLine { get { return _highlightcurrentline; } set { _highlightcurrentline = value; } }
        public override bool ShowSpaces
        {
            get
            {
                return base.ShowSpaces;
            }
            set
            {
                base.ShowSpaces = value;
            }
        }

    		#region Syntax Highlighting
    	
    	//TODO Should this go into the options? does it need to be run everytime for each editor?

        /// <summary>
        /// Loads all of syntax Highlighting
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ext"></param>
        [Localizable(false)]
        private void Register(string name, string[] ext)
        {
            var filename = String.Format("miRobotEditor.Controls.SyntaxHighlighting.{0}Highlight.xshd", name);
            using (var s = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(filename))
            {
                if (s == null)
                    throw new InvalidOperationException("Could not find embedded resource");
                //KUKAHighlight.xshd
                IHighlightingDefinition customHighlighting;
                using (var reader = new XmlTextReader(s))
                {
                    customHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }

                HighlightingManager.Instance.RegisterHighlighting(name, ext, customHighlighting);
            }

        }

        private void RegisterSyntaxHighlighting()
        {
            Register("KUKA", Languages.KUKA.Ext.ToArray());
            Register("KAWASAKI", Kawasaki.EXT.ToArray());
            Register("Fanuc", Fanuc.EXT.ToArray());
            Register("ABB", ABB.EXT.ToArray());
        }

    	#endregion
    	
        public TextEditorOptions()
        {
            Instance = this;
            RegisterSyntaxHighlighting();
        }

        public static TextEditorOptions Instance
        {
            get { return _instance ?? (_instance = new TextEditorOptions()); }
            set { _instance = value; }
        }

        private static TextEditorOptions _instance;

        bool _enableFolding = true;


        /// <summary>
        /// Enable Code Folding
        /// </summary>
        [DefaultValue(true)]
        public bool EnableFolding{ get { return _enableFolding;}set{_enableFolding = value;OnPropertyChanged("EnableFolding");}}

        bool _mouseWheelZoom = true;

        /// <summary>
        /// Allow Zooming of window by using MouseWheel
        /// </summary>
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
    }
}
