﻿using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using miRobotEditor.Languages;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Media;
using System.Xml;
using System.Xml.Serialization;

namespace miRobotEditor.GUI.Editor
{
    public interface IOptions
    {
        String Title { get; }
    }

    [Localizable(false), Serializable]
    public sealed class EditorOptions : TextEditorOptions, IOptions
    {
        #region Constructor

        public EditorOptions()
        {
            //            ShowSpaces = true;
            RegisterSyntaxHighlighting();
        }

        #endregion Constructor

        ~EditorOptions()
        {
            WriteXml();
        }

        private static string OptionsPath { get { return Path.Combine(App.StartupPath, "Options.xml"); } }

        private void WriteXml()
        {
            var s = new XmlSerializer(typeof(EditorOptions));
            TextWriter writer = new StreamWriter(OptionsPath);
            s.Serialize(writer, this);
            writer.Close();
        }

        private static EditorOptions ReadXml()
        {
            var result = new EditorOptions();

            if (!File.Exists(OptionsPath))
                return result;

            // Create Temporary File
            var filename = Path.GetTempFileName();
            filename = Path.ChangeExtension(filename, "xml");
            File.Copy(OptionsPath, filename);

            var s = new XmlSerializer(typeof(EditorOptions));
            var fs = new FileStream(filename, FileMode.Open);
            //            var fs = new FileStream(OptionsPath, FileMode.Open);

            try
            {
                result = (EditorOptions)s.Deserialize(fs);
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch
            // ReSharper restore EmptyGeneralCatchClause
            {
            }
            finally
            {
                fs.Close();
                File.Delete(filename);
            }

            return result;
        }

        public static EditorOptions Instance
        {
            get
            {
                return _instance ?? (_instance = ReadXml());
            }
            set { _instance = value; }
        }

        private static EditorOptions _instance;

        #region Overrides

        public override bool ShowSpaces { get { return base.ShowSpaces; } set { base.ShowSpaces = value; OnPropertyChanged("ShowSpaces"); } }

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

        #endregion Overrides

        //Background

        #region Colors

        /// <summary>
        /// Selected Text Background
        /// </summary>
        [NonSerialized]
        private Color _selectedTextBackground = Colors.SteelBlue;// { Color = Colors.SteelBlue, Opacity = 0.7 };

        public Color SelectedTextBackground { get { return _selectedTextBackground; } set { _selectedTextBackground = value; OnPropertyChanged("SelectedTextBackground"); } }

        [NonSerialized]
        private Color _backgroundColor = Colors.White;// new SolidColorBrush { Color = Colors.White };

        public Color BackgroundColor { get { return _backgroundColor; } set { _backgroundColor = value; OnPropertyChanged("BackgroundColor"); } }

        [NonSerialized]
        private Color _fontColor = Colors.Black;

        public Color FontColor { get { return _fontColor; } set { _fontColor = value; OnPropertyChanged("FontColor"); } }

        [NonSerialized]
        private Color _selectedFontColor = Colors.White;

        public Color SelectedFontColor { get { return _selectedFontColor; } set { _selectedFontColor = value; OnPropertyChanged("SelectedFontColor"); } }

        [NonSerialized]
        private Color _selectedBorderColor = Colors.Orange;

        public Color SelectedBorderColor { get { return _selectedBorderColor; } set { _selectedBorderColor = value; OnPropertyChanged("SelectedBorderColor"); } }

        [NonSerialized]
        private bool _allowScrollingBelowDocument;

        public bool AllowScrollingBelowDocument { get { return _allowScrollingBelowDocument; } set { _allowScrollingBelowDocument = value; OnPropertyChanged("AllowScrollingBelowDocument"); } }

        [NonSerialized]
        private Color _lineNumbersFontColor = Colors.Gray;

        public Color LineNumbersFontColor { get { return _lineNumbersFontColor; } set { _lineNumbersFontColor = value; OnPropertyChanged("LineNumbersFontColor"); } }

        /// <summary>
        /// Border Color of editor window
        /// </summary>
        private Color _borderColor = Colors.Transparent;

        /// <summary>
        /// Border Color of editor window
        /// </summary>
        public Color BorderColor { get { return _borderColor; } set { _borderColor = value; OnPropertyChanged("BorderColor"); } }

        /// <summary>
        /// Color of Line Numbers
        /// </summary>
        private Color _lineNumbersForeground = Colors.Gray;

        /// <summary>
        /// Color of Line Numbers
        /// </summary>
        public Color LineNumbersForeground { get { return _lineNumbersForeground; } set { _lineNumbersForeground = value; OnPropertyChanged("LineNumbersForeground"); } }

        [NonSerialized]
        private Color _selectedTextBorderColor = Colors.Orange;

        /// <summary>
        /// Color of Border for selected text
        /// </summary>
        public Color SelectedTextBorderColor { get { return _selectedTextBorderColor; } set { _selectedTextBorderColor = value; OnPropertyChanged("SelectedTextBorderColor"); } }

        private double _selectedBorderThickness = 1;

        public double SelectedBorderThickness { get { return _selectedBorderThickness; } set { _selectedBorderThickness = value; OnPropertyChanged("SelectedBorderThickness"); } }

        private double _borderThickness;

        public double BorderThickness { get { return _borderThickness; } set { _borderThickness = value; OnPropertyChanged("BorderThickness"); } }

        [NonSerialized]
        private Color _selectedlinecolor = Colors.Yellow;

        /// <summary>
        /// Color of Currently Selected Line
        /// </summary>
        public Color HighlightedLineColor { get { return _selectedlinecolor; } set { _selectedlinecolor = value; OnPropertyChanged("HighlightedLineColor"); } }

        #endregion Colors

        #region Brushes

        [NonSerialized]
        private Color _foldToolTipBackgroundColor = Colors.Red;//{ BorderBrush = Brushes.Black, Background = Brushes.WhiteSmoke };

        public Color FoldToolTipBackgroundColor { get { return _foldToolTipBackgroundColor; } set { _foldToolTipBackgroundColor = value; OnPropertyChanged("FoldToolTipBackgroundColor"); } }

        private Color _foldToolTipBackgroundBorderColor = Colors.WhiteSmoke;//{ BorderBrush = Brushes.Black, Background = Brushes.WhiteSmoke };

        public Color FoldToolTipBackgroundBorderColor { get { return _foldToolTipBackgroundBorderColor; } set { _foldToolTipBackgroundBorderColor = value; OnPropertyChanged("FoldToolTipBackgroundBorderColor"); } }

        private double _foldToolTipBorderThickness = 1;

        public double FoldToolTipBorderThickness { get { return _foldToolTipBorderThickness; } set { _foldToolTipBorderThickness = value; OnPropertyChanged("FoldToolTipBorderThickness"); } }

        #endregion Brushes

        #region Boolean

        private bool _wrapWords = true;

        public bool WrapWords { get { return _wrapWords; } set { _wrapWords = value; OnPropertyChanged("WrapWords"); } }

        #endregion Boolean

        private bool _highlightcurrentline = true;

        public bool HighlightCurrentLine { get { return _highlightcurrentline; } set { _highlightcurrentline = value; } }

        #region Syntax Highlighting

        /// <summary>
        /// Loads all of syntax Highlighting
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ext"></param>
        [Localizable(false)]
        private static void Register(string name, string[] ext)
        {
            var filename = String.Format("miRobotEditor.Controls.SyntaxHighlighting.{0}Highlight.xshd", name);
            using (var s = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(filename))
            {
                if (s == null)
                    throw new InvalidOperationException("Could not find embedded resource");
                //KUKAHighlight.xshd
                IHighlightingDefinition customHighlighting;
                using (var reader = new XmlTextReader(s))
                    customHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);

                HighlightingManager.Instance.RegisterHighlighting(name, ext, customHighlighting);
            }
        }

        private static void RegisterSyntaxHighlighting()
        {
            Register("KUKA", Languages.KUKA.Ext.ToArray());
            Register("KAWASAKI", Kawasaki.EXT.ToArray());
            Register("Fanuc", Fanuc.EXT.ToArray());
            Register("ABB", ABB.EXT.ToArray());
        }

        #endregion Syntax Highlighting

        private bool _enableFolding = true;

        /// <summary>
        /// Enable Code Folding
        /// </summary>
        [DefaultValue(true)]
        public bool EnableFolding { get { return _enableFolding; } set { _enableFolding = value; OnPropertyChanged("EnableFolding"); } }

        private bool _mouseWheelZoom = true;

        /// <summary>
        /// Allow Zooming of window by using MouseWheel
        /// </summary>
        [DefaultValue(true)]
        public bool MouseWheelZoom
        {
            get { return _mouseWheelZoom; }
            set
            {
                if (_mouseWheelZoom == value) return;
                _mouseWheelZoom = value;
                OnPropertyChanged("MouseWheelZoom");
            }
        }

        private bool _enableAnimations = true;

        public bool EnableAnimations { get { return _enableAnimations; } set { _enableAnimations = value; OnPropertyChanged("EnableAnimations"); } }

        private bool _showlinenumbers = true;

        public bool ShowLineNumbers
        {
            get { return _showlinenumbers; }
            set { _showlinenumbers = value; OnPropertyChanged("ShowLineNumbers"); }
        }

        public string Title { get { return "Text Editor Options"; } }
    }

    public class GlobalOptions : IOptions
    {
        private GlobalOptions()
        {
            FlyoutOpacity = .85;
        }

        [Localizable(false)]
        public string Title { get { return "Global Options"; } }

        private static GlobalOptions _instance;

        public static GlobalOptions Instance
        {
            get { return _instance ?? (_instance = new GlobalOptions()); }
            set { _instance = value; }
        }

        #region Flyout Options

        [DefaultValue(0.75)]
        public double FlyoutOpacity { get; set; }

        #endregion Flyout Options
    }
}