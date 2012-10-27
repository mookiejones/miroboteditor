using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using miRobotEditor.Classes;
using miRobotEditor.Core;
using miRobotEditor.Core.EventArgs;
using miRobotEditor.Languages;

namespace miRobotEditor.WPFPresentation.Controls
{
    /// <summary>
    /// Interaction logic for EditorDocument.xaml
    /// </summary>
    public partial class EditorDocument : UserControl, IDocument
    {
        public object Host { get; set; }
        private string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; OnPropertyChanged("Title"); }
        }

        public AbstractLanguageClass FileLanguage { get; set; }
        private static EditorDocument _instance;

        public static EditorDocument ActiveEditor
        {
            get { return _instance ?? (_instance = new EditorDocument()); }
            set { _instance = value; }
        }

        #region Public Events

        public event UpdateFunctionEventHandler TextUpdated;

        #endregion

        #region Properties

        public System.IO.FileInfo _file { get; private set; }

        public EditorControl Source
        {
            get { return source; }
        }

        public EditorControl Data
        {
            get { return data; }
        }


        public EditorControl TextBox { get; set; }

        #endregion

        public EditorDocument()
        {
            InitializeComponent();
            DataContext = this;
            ShowGrid = false;
            FileLanguage = new LanguageBase();
            Source.HostControl = this;
            Data.HostControl = this;
        }
        public void Close()
        {
        }

        private bool ShowGrid
        {
            set
            {
                switch (value)
                {
                    case true:
                        Data.Load(_file.DirectoryName + "\\" + FileLanguage.DataName);
                        Data.Visibility = Visibility.Visible;
                        grid.Visibility = Visibility.Visible;
                        Grid.SetRow(grid, 1);
                        Grid.SetRow(data, 2);
                        break;
                    case false:
                        Data.Visibility = Visibility.Collapsed;
                        grid.Visibility = Visibility.Collapsed;
                        break;
                }
            }
        }
        public void CanReload(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((_file != null) && (_file.Exists));
        }

        public void ExecuteReload(object sender, ExecutedRoutedEventArgs e)
        {
            Load(_file);
        }

        public void Load(FileInfo file)
        {
            _file = file;

            FileLanguage = AbstractLanguageClass.GetRobotType(_file);
            grid.IsAnimated = false;
            //TODO Set Icon For File

            //TODO Get Files Based in language 
            //TODO Possibly set as content


            if ((FileLanguage is KUKA) && (!String.IsNullOrEmpty(FileLanguage.SourceText)) && (FileLanguage.SourceName != null))
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => Source.Load(_file.DirectoryName + "\\" + FileLanguage.SourceName)));
            else
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => Source.Load(_file.FullName)));
            if ((FileLanguage is KUKA) && (!String.IsNullOrEmpty(FileLanguage.DataText)) && (Source.Text != FileLanguage.DataText))
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => ShowGrid = true));
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => Data.Load(_file.DirectoryName + "\\" + FileLanguage.DataName)));
            }
            else
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => ShowGrid = false));
            }

            // Select Original File
            TextBox = source.File == file ? source : data;
            grid.IsAnimated = true;
        }

        private void ReloadFile(object sender, EventArgs e)
        {
            // Dispatcher is used because of the FileChanged Events
            if (Source.File != null)
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => Source.Reload()));
            // Dispatcher is used because of the FileChanged Events
            if (data.File != null)
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => Data.Reload()));
        }

        private void raiseupdated(object sender, FunctionEventArgs e)
        {
            if (TextUpdated != null)
            {
                if (sender is EditorControl)
                    TextBox = sender as EditorControl;
                //TODO Remember why im calling this at this point. It resets the "View As" Setting
                //        GetRobotType();
                TextUpdated(sender, e);
            }
        }

        private void Is_VisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is EditorControl)
            {
                TextBox = sender as EditorControl;
                FileLanguage = AbstractLanguageClass.GetRobotType(_file);
            }
        }

        public void Find()
        {
            TextBox.ShowFindDialog();
        }

        private void TextChanged(object sender, EventArgs e)
        {
            if (sender is EditorControl)
            {
                grid.FocusColor = FileLanguage.FocusedColor;
                grid.UnfocusColor = FileLanguage.UnfocusedColor;
                grid.UpdateLayout();
                TextBox = sender as EditorControl;
                FileLanguage.RawText = Source.Text + Data.Text;
            }
            else
                throw new NotImplementedException();

            raiseupdated(TextBox, new FunctionEventArgs(TextBox.Text));
        }

        private void grid_Loaded(object sender, RoutedEventArgs e)
        {
            grid.IsAnimated = false;
            grid.Collapse();
            grid.IsCollapsed = true;
            grid.IsAnimated = true;

        }


        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public ITextBufferVersion Version { get; private set; }
        public ITextBuffer CreateSnapshot()
        {
            throw new NotImplementedException();
        }

        public ITextBuffer CreateSnapshot(int offset, int length)
        {
            throw new NotImplementedException();
        }

        public TextReader CreateReader()
        {
            throw new NotImplementedException();
        }

        public TextReader CreateReader(int offset, int length)
        {
            throw new NotImplementedException();
        }

        public int TextLength { get; private set; }
        string IDocument.Text { get; set; }
        public int TotalNumberOfLines { get; private set; }
        public IDocumentLine GetLine(int lineNumber)
        {
            throw new NotImplementedException();
        }

        public IDocumentLine GetLineForOffset(int offset)
        {
            throw new NotImplementedException();
        }

        public int PositionToOffset(int line, int column)
        {
            throw new NotImplementedException();
        }

        public Location OffsetToPosition(int offset)
        {
            throw new NotImplementedException();
        }

        
        public void Insert(int offset, string text)
        {
            throw new NotImplementedException();
        }

        public void Insert(int offset, string text, AnchorMovementType defaultAnchorMovementType)
        {
            throw new NotImplementedException();
        }

        public void Remove(int offset, int length)
        {
            throw new NotImplementedException();
        }

        public void Replace(int offset, int length, string newText)
        {
            throw new NotImplementedException();
        }

        public void StartUndoableAction()
        {
            throw new NotImplementedException();
        }

        public void EndUndoableAction()
        {
            throw new NotImplementedException();
        }

        public IDisposable OpenUndoGroup()
        {
            throw new NotImplementedException();
        }

        public ITextAnchor CreateAnchor(int offset)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<TextChangeEventArgs> Changing;
        public event EventHandler<TextChangeEventArgs> Changed;
        string ITextBuffer.Text { get; private set; }
        public event EventHandler TextChanged;
        public char GetCharAt(int offset)
        {
            throw new NotImplementedException();
        }

        public string GetText(int offset, int length)
        {
            throw new NotImplementedException();
        }

        public object GetService(Type serviceType)
        {
            throw new NotImplementedException();
        }
    }
}
