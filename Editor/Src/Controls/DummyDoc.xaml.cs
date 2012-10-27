using System;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Threading;
using miRobotEditor.GUI.Editor;
using miRobotEditor.Robot;

namespace miRobotEditor.Controls
{
    using Classes;
    using System.ComponentModel;

    /// <summary>
    /// Document Editor
    /// </summary>
    public partial class DummyDoc : UserControl, INotifyPropertyChanged
    {
        public AbstractRobotClass Robot { get; set; }
        private static DummyDoc _instance;

        public static DummyDoc ActiveEditor
        {
            get { return _instance ?? (_instance = new DummyDoc()); }
            set { _instance = value; }
        }

        #region Public Events

        public event UpdateFunctionEventHandler TextUpdated;

        #endregion

        #region Properties

        public FileInfo _file { get; private set; }

        public Editor Source
        {
            get { return source; }
        }

        public Editor Data
        {
            get { return data; }
        }


        public Editor TextBox { get; set; }

        #endregion

        public DummyDoc()
        {
            InitializeComponent();
            DataContext = this;
            ShowGrid = false;
            Robot = new RobotBase();
            Source.Parent = this;
            Data.Parent = this;
        }

        public void Close()
        {
        }

        private void GetRobotType()
        {
            if (_file != null)
            {
                switch (_file.Extension.ToLower())
                {
                    case ".as":
                    case ".pg":
                        Robot = new Kawasaki(_file);
                        break;
                    case ".rt":
                    case ".src":
                    case ".dat":
                    case ".sub":
                        Robot = new KUKA(_file);
                        break;
                    case ".mod":
                    case ".prg":
                        Robot = new ABB(_file);
                        break;
                    case ".bas":
                        Robot = new VBA(_file);
                        break;
                    case ".ls":
                        Robot = new Fanuc(_file);
                        break;
                    default:
                        Robot = new RobotBase(_file);
                        break;
                }
            }
            else
            {
                Robot = new RobotBase();
            }

        }

        private bool ShowGrid
        {
            set
            {
                switch (value)
                {
                    case true:
                        Data.Load(_file.DirectoryName + "\\" + Robot.DataName);
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

        public void Load(FileInfo file)
        {
            _file = file;

            GetRobotType();
            grid.IsAnimated = false;
            //TODO Set Icon For File

            if ((Robot is KUKA) && (!String.IsNullOrEmpty(Robot.SourceText)) && (Robot.SourceName != null))
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => Source.Load(_file.DirectoryName + "\\" + Robot.SourceName)));
            else
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => Source.Load(_file.FullName)));

            if ((Robot is KUKA) && (!String.IsNullOrEmpty(Robot.DataText)) && (Source.Text!=Robot.DataText))
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => ShowGrid = true));
                Dispatcher.BeginInvoke(DispatcherPriority.Normal,new Action(() => Data.Load(_file.DirectoryName + "\\" + Robot.DataName)));
            }
            else
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => ShowGrid = false));
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
                if (sender is Editor)
                    TextBox = sender as Editor;
        //TODO Remember why im calling this at this point. It resets the "View As" Setting
        //        GetRobotType();
                TextUpdated(sender, e);
            }
        }

        private void Is_VisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is Editor)
            {
                TextBox = sender as Editor;
                GetRobotType();

            }
        }

        public void Find()
        {
            TextBox.ShowFindDialog();
        }

        private void TextChanged(object sender, EventArgs e)
        {
            if (sender is Editor)
            {
                grid.FocusColor = Robot.FocusedColor;
                grid.UnfocusColor = Robot.UnfocusedColor;
                grid.UpdateLayout();
                TextBox = sender as Editor;
                Robot.RawText = Source.Text + Data.Text;
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
    }
}
