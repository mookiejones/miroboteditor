﻿using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows.Media;
using System.Xml;
using System.Xml.Serialization;
using GalaSoft.MvvmLight.Messaging;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using miRobotEditor.Languages;

namespace miRobotEditor.GUI
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

       
        #endregion


        #region Overrides
        /*
        public override bool ShowSpaces
        {
            get { return base.ShowSpaces; }
            set
            {
                base.ShowSpaces = value;
                OnPropertyChanged("ShowSpaces");
            }
        }
        */
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

        #endregion

        #region Colors

        [NonSerialized] private bool _allowScrollingBelowDocument;
        [NonSerialized] private Color _backgroundColor = Colors.White; // new SolidColorBrush { Color = Colors.White };

        /// <summary>
        ///     Border Color of editor window
        /// </summary>
        private Color _borderColor = Colors.Transparent;

        private double _borderThickness;
        [NonSerialized] private Color _fontColor = Colors.Black;
        [NonSerialized] private Color _lineNumbersFontColor = Colors.Gray;

        /// <summary>
        ///     Color of Line Numbers
        /// </summary>
        private Color _lineNumbersForeground = Colors.Gray;

        [NonSerialized] private Color _selectedBorderColor = Colors.Orange;
        [NonSerialized] private Color _selectedFontColor = Colors.White;

        /// <summary>
        ///     Selected Text Background
        /// </summary>
        [NonSerialized] private Color _selectedTextBackground = Colors.SteelBlue;
            // { Color = Colors.SteelBlue, Opacity = 0.7 };

        [NonSerialized] private Color _selectedTextBorderColor = Colors.Orange;
        [NonSerialized] private Color _selectedlinecolor = Colors.Yellow;

        public Color SelectedTextBackground
        {
            get { return _selectedTextBackground; }
            set
            {
                _selectedTextBackground = value;
                OnPropertyChanged("SelectedTextBackground");
            }
        }

        public Color BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                _backgroundColor = value;
                OnPropertyChanged("BackgroundColor");
            }
        }

        public Color FontColor
        {
            get { return _fontColor; }
            set
            {
                _fontColor = value;
                OnPropertyChanged("FontColor");
            }
        }

        public Color SelectedFontColor
        {
            get { return _selectedFontColor; }
            set
            {
                _selectedFontColor = value;
                OnPropertyChanged("SelectedFontColor");
            }
        }

        public Color SelectedBorderColor
        {
            get { return _selectedBorderColor; }
            set
            {
                _selectedBorderColor = value;
                OnPropertyChanged("SelectedBorderColor");
            }
        }

        public bool AllowScrollingBelowDocument
        {
            get { return _allowScrollingBelowDocument; }
            set
            {
                _allowScrollingBelowDocument = value;
                OnPropertyChanged("AllowScrollingBelowDocument");
            }
        }

        public Color LineNumbersFontColor
        {
            get { return _lineNumbersFontColor; }
            set
            {
                _lineNumbersFontColor = value;
                OnPropertyChanged("LineNumbersFontColor");
            }
        }

        /// <summary>
        ///     Border Color of editor window
        /// </summary>
        public Color BorderColor
        {
            get { return _borderColor; }
            set
            {
                _borderColor = value;
                OnPropertyChanged("BorderColor");
            }
        }

        /// <summary>
        ///     Color of Line Numbers
        /// </summary>
        public Color LineNumbersForeground
        {
            get { return _lineNumbersForeground; }
            set
            {
                _lineNumbersForeground = value;
                OnPropertyChanged("LineNumbersForeground");
            }
        }


        /// <summary>
        ///     Color of Border for selected text
        /// </summary>
        public Color SelectedTextBorderColor
        {
            get { return _selectedTextBorderColor; }
            set
            {
                _selectedTextBorderColor = value;
                OnPropertyChanged("SelectedTextBorderColor");
            }
        }


        
        #region SelectedBorderThickness
        /// <summary>
        /// The <see cref="SelectedBorderThickness" /> property's name.
        /// </summary>
        public const string SelectedBorderThicknessPropertyName = "SelectedBorderThickness";

        private double _selectedBorderThickness = 1;

        /// <summary>
        /// Sets and gets the SelectedBorderThickness property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double SelectedBorderThickness
        {
            get
            {
                return _selectedBorderThickness;
            }

            set
            {
                if (_selectedBorderThickness == value)
                {
                    return;
                }
                _selectedBorderThickness = value;
                OnPropertyChanged(SelectedBorderThicknessPropertyName);
            }
        }
        #endregion
      

        public double BorderThickness
        {
            get { return _borderThickness; }
            set
            {
                _borderThickness = value;
                OnPropertyChanged("BorderThickness");
            }
        }

        /// <summary>
        ///     Color of Currently Selected Line
        /// </summary>
        public Color HighlightedLineColor
        {
            get { return _selectedlinecolor; }
            set
            {
                _selectedlinecolor = value;
                OnPropertyChanged("HighlightedLineColor");
            }
        }

        #endregion

        #region Brushes

        private Color _foldToolTipBackgroundBorderColor = Colors.WhiteSmoke;
            //{ BorderBrush = Brushes.Black, Background = Brushes.WhiteSmoke };

        [NonSerialized] private Color _foldToolTipBackgroundColor = Colors.Red;
            //{ BorderBrush = Brushes.Black, Background = Brushes.WhiteSmoke };

        private double _foldToolTipBorderThickness = 1;

        public Color FoldToolTipBackgroundColor
        {
            get { return _foldToolTipBackgroundColor; }
            set
            {
                _foldToolTipBackgroundColor = value;
                OnPropertyChanged("FoldToolTipBackgroundColor");
            }
        }

        public Color FoldToolTipBackgroundBorderColor
        {
            get { return _foldToolTipBackgroundBorderColor; }
            set
            {
                _foldToolTipBackgroundBorderColor = value;
                OnPropertyChanged("FoldToolTipBackgroundBorderColor");
            }
        }

        public double FoldToolTipBorderThickness
        {
            get { return _foldToolTipBorderThickness; }
            set
            {
                _foldToolTipBorderThickness = value;
                OnPropertyChanged("FoldToolTipBorderThickness");
            }
        }

        #endregion

        #region Boolean

        private bool _wrapWords = true;

        public bool WrapWords
        {
            get { return _wrapWords; }
            set
            {
                _wrapWords = value;
                OnPropertyChanged("WrapWords");
            }
        }

        #endregion

        #region Timestamp format

        private string _timestampFormat = "ddd MMM d hh:mm:ss yyyy";

        public string TimestampFormat
        {
            get { return _timestampFormat; }
            set
            {
                _timestampFormat = value;
                OnPropertyChanged("TimestampFormat");
                OnPropertyChanged("TimestampSample");
            }
        }

        #endregion

        #region Timestamp Sample

        public string TimestampSample => DateTime.Now.ToString(_timestampFormat);

        #endregion

        private bool _enableAnimations = true;
        private bool _enableFolding = true;

        //Background

        private bool _highlightcurrentline = true;
        private bool _mouseWheelZoom = true;
        private bool _showlinenumbers = true;

        private static string OptionsPath
        {
            get
            {
                var path = App.StartupPath;
                return Path.Combine(path, "Options.xml");
            }
        }

        private static EditorOptions CreateInstance()
        {
            ReadXml();
            return _instance;
        }

        private static EditorOptions _instance;
        public static EditorOptions Instance
        {
            get { return _instance ?? (CreateInstance()); }

            set { _instance = value; }
        }

        public override bool HighlightCurrentLine
        {
            get { return _highlightcurrentline; }
            set { _highlightcurrentline = value; }
        }


        /// <summary>
        ///     Enable Code Folding
        /// </summary>
        [DefaultValue(true)]
        public bool EnableFolding
        {
            get { return _enableFolding; }
            set
            {
                _enableFolding = value;
                OnPropertyChanged("EnableFolding");
            }
        }


        /// <summary>
        ///     Allow Zooming of window by using MouseWheel
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

        public bool EnableAnimations
        {
            get { return _enableAnimations; }
            set
            {
                _enableAnimations = value;
                OnPropertyChanged("EnableAnimations");
            }
        }

        public bool ShowLineNumbers
        {
            get { return _showlinenumbers; }
            set
            {
                _showlinenumbers = value;
                OnPropertyChanged("ShowLineNumbers");
            }
        }

        #region Syntax Highlighting

        /// <summary>
        ///     Loads all of syntax Highlighting
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ext"></param>
        [Localizable(false)]
        private static void Register(string name, string[] ext)
        {
            string filename = String.Format("miRobotEditor.Controls.SyntaxHighlighting.{0}Highlight.xshd", name);
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(filename))
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

        #endregion

        public string Title => "Text Editor Options";

        ~EditorOptions()
        {
            WriteXml();
        }

        private void WriteXml()
        {
            var s = new XmlSerializer(typeof (EditorOptions));
            TextWriter writer = new StreamWriter(OptionsPath);
            s.Serialize(writer, this);
            writer.Close();
        }

        private static void ReadXml()
        {

            var path = OptionsPath;

            if (!File.Exists(path))
                return ;
            try
            {

            using (var stream = new StreamReader(path))
            {
                var serializer = new XmlSerializer(typeof(EditorOptions));
                _instance = (EditorOptions) serializer.Deserialize(stream);
            }

            }

            catch(Exception ex)
            {
                // Send Message For Reading.
                Messenger.Default.Send<Exception>(ex);
            }


        }
    }

    public class GlobalOptions : IOptions
    {
        private static GlobalOptions _instance;

        private GlobalOptions()
        {
            FlyoutOpacity = .85;
        }

        public static GlobalOptions Instance
        {
            get { return _instance ?? (_instance = new GlobalOptions()); }
            set { _instance = value; }
        }

        #region Flyout Options

        [DefaultValue(0.75)]
        public double FlyoutOpacity { get; set; }

        #endregion

        [Localizable(false)]
        public string Title => "Global Options";
    }
}