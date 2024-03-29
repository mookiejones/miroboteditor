﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.OleDb;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Ionic.Zip;
using miRobotEditor.Classes;
using miRobotEditor.Core;
using Application = System.Windows.Forms.Application;
using MessageBox = System.Windows.Forms.MessageBox;
using RelayCommand = miRobotEditor.Commands.RelayCommand;

namespace miRobotEditor.ViewModel
{
    public sealed class ArchiveInfoViewModel : ToolViewModel
    {
        #region Constructor

        public ArchiveInfoViewModel() : base("ArchiveInfoViewModel")
        {
            _root = new ObservableCollection<DirectoryInfo>();

            RaisePropertyChanged("DigInVisibility");
            RaisePropertyChanged("DigOutVisibility");
            RaisePropertyChanged("AnInVisibility");
            RaisePropertyChanged("AnOutVisibility");
            DefaultPane = DefaultToolPane.Right;

            Width = 250;

            Height = 600;
        }

        #endregion

        #region Properties

        public Visibility DigInVisibility => Inputs.Count > 0 ? Visibility.Visible : Visibility.Hidden;

        public Visibility DigOutVisibility => Outputs.Count > 0 ? Visibility.Visible : Visibility.Hidden;

        public Visibility AnInVisibility => AnIn.Count > 0 ? Visibility.Visible : Visibility.Hidden;

        public Visibility AnOutVisibility => AnOut.Count > 0 ? Visibility.Visible : Visibility.Hidden;


        public Visibility DigitalVisibility => ((DigInVisibility == Visibility.Visible) && (DigOutVisibility == Visibility.Visible))
                    ? Visibility.Visible
                    : Visibility.Collapsed;

        public Visibility AnalogVisibility => ((AnOutVisibility == Visibility.Visible) || (AnInVisibility == Visibility.Visible))
                    ? Visibility.Visible
                    : Visibility.Collapsed;

        private Visibility _flagVisibility = Visibility.Collapsed;

        public Visibility FlagVisibility
        {
            get { return _flagVisibility; }
            set
            {
                _flagVisibility = value;
                RaisePropertyChanged("FlagVisibility");
            }
        }

        private Visibility _timerVisibility = Visibility.Collapsed;

        public Visibility TimerVisibility
        {
            get { return _timerVisibility; }
            set
            {
                _timerVisibility = value;
                RaisePropertyChanged("TimerVisibility");
            }
        }

        private Visibility _cyclicFlagVisibility = Visibility.Collapsed;

        public Visibility CyclicFlagVisibility
        {
            get { return _cyclicFlagVisibility; }
            set
            {
                _cyclicFlagVisibility = value;
                RaisePropertyChanged("CyclicFlagVisibility");
            }
        }

        private Visibility _counterVisibility = Visibility.Collapsed;

        public Visibility CounterVisibility
        {
            get { return _counterVisibility; }
            set
            {
                _counterVisibility = value;
                RaisePropertyChanged("CounterVisibility");
            }
        }

        private readonly ProgressBarViewModel _progress = new ProgressBarViewModel();

        private InfoFile _info = new InfoFile();

        public InfoFile Info
        {
            get { return _info; }
            set
            {
                _info = value;
                RaisePropertyChanged("Info");
            }
        }


        public string DirectoryPath { get; set; }
        private string _archivePath = " ";

        public string ArchivePath
        {
            get { return _archivePath; }
            set
            {
                _archivePath = value;
                RaisePropertyChanged("ArchivePath");
            }
        }


        private string _filecount = string.Empty;

        public string FileCount
        {
            get { return _filecount; }
            set
            {
                _filecount = value;
                RaisePropertyChanged("FileCount");
            }
        }

        public ZipFile ArchiveZip { get; set; }
        private string _buffersize = string.Empty;

        public string BufferSize
        {
            get { return _buffersize; }
            set
            {
                _buffersize = value;
                RaisePropertyChanged("BufferSize");
            }
        }

        private string _dbFile = string.Empty;
        public string DataBaseFile { get; set; }
// ReSharper disable ConvertToConstant.Local
// ReSharper disable FieldCanBeMadeReadOnly.Local
        private bool _isKRC2 = true;
// ReSharper restore FieldCanBeMadeReadOnly.Local
// ReSharper restore ConvertToConstant.Local

        private string _database = string.Empty;

        public string DataBase
        {
            get { return _database; }
            set
            {
                _database = value;
                RaisePropertyChanged("DataBase");
            }
        }

        public string InfoFile { get; set; }


        private readonly ObservableCollection<DirectoryInfo> _root = new ObservableCollection<DirectoryInfo>();
        private readonly ReadOnlyObservableCollection<DirectoryInfo> _readonlyRoot = null;

        public ReadOnlyObservableCollection<DirectoryInfo> Root => _readonlyRoot ?? new ReadOnlyObservableCollection<DirectoryInfo>(_root);

        private static string StartupPath => Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

        private DirectoryInfo _rootpath;

        public DirectoryInfo RootPath
        {
            get { return _rootpath; }
            set
            {
                _rootpath = value;
                RaisePropertyChanged("RootPath");
            }
        }

        private string _languageText = String.Empty;

        public string LanguageText
        {
            get { return _languageText; }
            set
            {
                _languageText = value;
                RaisePropertyChanged("LanguageText");
            }
        }

        private string _databaseText = String.Empty;

        public string DatabaseText
        {
            get { return _databaseText; }
            set
            {
                _databaseText = value;
                RaisePropertyChanged("DatabaseText");
            }
        }


// ReSharper disable InconsistentNaming
        public ObservableCollection<Item> _inputs = new ObservableCollection<Item>();
// ReSharper restore InconsistentNaming
        private readonly ReadOnlyObservableCollection<Item> _readonlyinputs = null;

        public ReadOnlyObservableCollection<Item> Inputs => _readonlyinputs ?? new ReadOnlyObservableCollection<Item>(_inputs);

        private readonly ObservableCollection<Item> _outputs = new ObservableCollection<Item>();
        private readonly ReadOnlyObservableCollection<Item> _readonlyOutputs = null;

        public ReadOnlyObservableCollection<Item> Outputs => _readonlyOutputs ?? new ReadOnlyObservableCollection<Item>(_outputs);

        private readonly ObservableCollection<Item> _anin = new ObservableCollection<Item>();
        private readonly ReadOnlyObservableCollection<Item> _readonlyAnIn = null;

        public ReadOnlyObservableCollection<Item> AnIn => _readonlyAnIn ?? new ReadOnlyObservableCollection<Item>(_anin);

        private readonly ObservableCollection<Item> _anout = new ObservableCollection<Item>();
        private readonly ReadOnlyObservableCollection<Item> _readonlyAnOut = null;

        public ReadOnlyObservableCollection<Item> AnOut => _readonlyAnOut ?? new ReadOnlyObservableCollection<Item>(_anout);

        private readonly ObservableCollection<Item> _timer = new ObservableCollection<Item>();
        private readonly ReadOnlyObservableCollection<Item> _readonlyTimer = null;

        public ReadOnlyObservableCollection<Item> Timer => _readonlyTimer ?? new ReadOnlyObservableCollection<Item>(_timer);

        private readonly ObservableCollection<Item> _flags = new ObservableCollection<Item>();
        private readonly ReadOnlyObservableCollection<Item> _readonlyFlags = null;

        public ReadOnlyObservableCollection<Item> Flags => _readonlyFlags ?? new ReadOnlyObservableCollection<Item>(_flags);

        private readonly ObservableCollection<Item> _cycflags = new ObservableCollection<Item>();
        private readonly ReadOnlyObservableCollection<Item> _readonlyCycFlags = null;

        public ReadOnlyObservableCollection<Item> CycFlags => _readonlyCycFlags ?? new ReadOnlyObservableCollection<Item>(_cycflags);

        private readonly ObservableCollection<Item> _counter = new ObservableCollection<Item>();
        private readonly ReadOnlyObservableCollection<Item> _readonlyCounter = null;

        public ReadOnlyObservableCollection<Item> Counter => _readonlyCounter ?? new ReadOnlyObservableCollection<Item>(_counter);

        #endregion

        #region Commands

        private RelayCommand _openCommand;

        public ICommand OpenCommand => _openCommand ?? (_openCommand = new RelayCommand(param => Open(), param => true));

        private RelayCommand _importCommand;

        public ICommand ImportCommand => _importCommand ?? (_importCommand = new RelayCommand(param => Import(), param => true));

        private RelayCommand _loadCommand;

        public ICommand LoadCommand => _loadCommand ?? (_loadCommand = new RelayCommand(param => GetSignals(), param => true));

        #endregion

        //TODO need to make this into allowing either a folder or a zip file.

        private void Import()
        {
            var ofd = new OpenFileDialog
            {
                Title = "Select Archive",
                Filter = "KUKA Archive (*.zip)|*.zip",
                Multiselect = false
            };
            ofd.ShowDialog();

            _root.Clear();
            if (!File.Exists(ofd.FileName)) return;
            ArchivePath = ofd.FileName;
            ArchiveZip = new ZipFile(ofd.FileName);
            RaisePropertyChanged("ArchiveZip");
            GetAllLangtextFromDatabase();
            UnpackZip();
            GetFiles(DirectoryPath);
            GetAMInfo();
            GetSignals();
        }

// ReSharper disable UnusedMember.Local
        private void ReadZip()
// ReSharper restore UnusedMember.Local
        {
            foreach (ZipEntry z in ArchiveZip.EntriesSorted.Where(z => z.IsDirectory))
                Console.WriteLine(z.FileName);


            foreach (ZipEntry e in ArchiveZip.Entries.Where(e => e.IsDirectory))
                Console.WriteLine(e.FileName);


            RaisePropertyChanged("Root");
        }

        private static bool CheckPathExists(string path)
        {
            // if archive path exists, allow to delete or rename
            if (!Directory.Exists(path)) return false;

            DialogResult msgResult =
                MessageBox.Show(
                    String.Format("The path of {0} \r\n allready exists. Do you want to Delete the path?", path),
                    "Archive Exists", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button3);

            switch (msgResult)
            {
                case DialogResult.Yes:
                    Directory.Delete(path, true);
                    return false;
                case DialogResult.No:
                case DialogResult.Cancel:
                    return true;
            }
            return false;
        }

        private void UnpackZip()
        {
// ReSharper disable AssignNullToNotNullAttribute
            DirectoryPath = Path.Combine(StartupPath, Path.GetFileNameWithoutExtension(ArchivePath));
// ReSharper restore AssignNullToNotNullAttribute
            bool exists = CheckPathExists(DirectoryPath);
            if (exists) return;

            //  This call to ExtractAll() assumes:
            //   - none of the entries are password-protected.
            //   - want to extract all entries to current working directory
            //   - none of the files in the zip already exist in the directory;
            //     if they do, the method will throw.
            ArchiveZip.ExtractAll(DirectoryPath);

            _root.Add(new DirectoryInfo(DirectoryPath));
        }

// ReSharper disable UnusedMember.Local
        private void GetDirectories()
// ReSharper restore UnusedMember.Local
        {
//            Root = new ObservableCollection<DirectoryRecord>{  new DirectoryRecord { Info = new DirectoryInfo(folder)
        }

        private void GetAMInfo()
        {
            if (!File.Exists(InfoFile)) return;
            string[] file = File.ReadAllLines(InfoFile);
            foreach (var sp in file.Select(f => f.Split('=')).Where(sp => sp.Length > 0))
            {
                switch (sp[0])
                {
                    case "Name":
                        Info.ArchiveName = sp[1];
                        break;
                    case "Config":
                        Info.ArchiveConfigType = sp[1];
                        break;
                    case "DiskNo":
                        Info.ArchiveDiskNo = sp[1];
                        break;
                    case "ID":
                        Info.ArchiveID = sp[1];
                        break;
                    case "Date":
                        Info.ArchiveDate = sp[1];
                        break;
                    case "RobName":
                        Info.RobotName = sp[1];
                        break;
                    case "IRSerialNr":
                        Info.RobotSerial = sp[1];
                        break;
                    case "Version":
                        Info.KSSVersion = sp[1];
                        break;
                }
            }
        }

        private void GetFlags()
        {
            string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DataBaseFile + ";";
            const string cmdText =
                "SELECT Items.KeyString, Messages.[String] FROM (Items INNER JOIN Messages ON Items.Key_id = Messages.Key_id)WHERE (Items.[Module] = 'FLAG')";
            using (var oldDbConnection = new OleDbConnection(connectionString))
            {
                oldDbConnection.Open();
                using (var oleDbCommand = new OleDbCommand(cmdText, oldDbConnection))
                {
                    using (OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader())
                    {
                        while (oleDbDataReader != null && oleDbDataReader.Read())
                        {
                            string temp = oleDbDataReader.GetValue(0).ToString();
                            var sig = new Item(String.Format("$FLAG[{0}]", temp.Substring(8)),
                                oleDbDataReader.GetValue(1).ToString());
                            _flags.Add(sig);
                        }
                    }
                }
            }
            FlagVisibility = Flags.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            RaisePropertyChanged("FlagVisibility");
        }

        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
// ReSharper disable UnusedMember.Local
        private List<Item> GetValues(string cmd, int index)
// ReSharper restore UnusedMember.Local
        {
            var result = new List<Item>();
            string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DataBaseFile + ";";
            string cmdText =
                String.Format(
                    "SELECT Items.KeyString, Messages.[String] FROM (Items INNER JOIN Messages ON Items.Key_id = Messages.Key_id)WHERE (Items.[Module] = '{0}')",
                    cmd);
            using (var oldDbConnection = new OleDbConnection(connectionString))
            {
                oldDbConnection.Open();
                using (var oleDbCommand = new OleDbCommand(cmdText, oldDbConnection))
                {
                    using (OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader())
                    {
                        while (oleDbDataReader != null && oleDbDataReader.Read())
                        {
                            string temp = oleDbDataReader.GetValue(0).ToString();
                            var sig = new Item(String.Format("${1}[{0}]", temp.Substring(index), cmd),
                                oleDbDataReader.GetValue(1).ToString());
                            result.Add(sig);
                        }
                    }
                }
            }
            return result;
        }

        private void GetTimers()
        {
            string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DataBaseFile + ";";
            const string cmdText =
                "SELECT Items.KeyString, Messages.[String] FROM (Items INNER JOIN Messages ON Items.Key_id = Messages.Key_id)WHERE (Items.[Module] = 'TIMER')";
            using (var oldDbConnection = new OleDbConnection(connectionString))
            {
                oldDbConnection.Open();
                using (var oleDbCommand = new OleDbCommand(cmdText, oldDbConnection))
                {
                    using (OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader())
                    {
                        while (oleDbDataReader != null && oleDbDataReader.Read())
                        {
                            string temp = oleDbDataReader.GetValue(0).ToString();
                            var sig = new Item(String.Format("$TIMER[{0}]", temp.Substring(9)),
                                oleDbDataReader.GetValue(1).ToString());
                            _timer.Add(sig);
                        }
                    }
                }
            }
            TimerVisibility = Timer.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            RaisePropertyChanged("TimerVisibility");
        }

// ReSharper disable UnusedMember.Local
        private void GetSignalsFromDataBase()
// ReSharper restore UnusedMember.Local
        {
            var ofd = new OpenFileDialog
            {
                Title = "Select Database",
                Filter = "KUKA Connection Files (kuka_con.mdb)|kuka_con.mdb|All files (*.*)|*.*",
                Multiselect = false
            };
            ofd.ShowDialog();
            LanguageText = string.Empty;

            DataBaseFile = ofd.FileName;
            GetSignals();
        }

        private void GetSignals()
        {
            if (File.Exists(DataBaseFile))
            {
                string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DataBaseFile + ";";
                const string cmdText =
                    "SELECT Items.KeyString, Messages.[String] FROM (Items INNER JOIN Messages ON Items.Key_id = Messages.Key_id)WHERE (Items.[Module] = 'IO')";
                using (var oldDbConnection = new OleDbConnection(connectionString))
                {
                    oldDbConnection.Open();
                    using (var oleDbCommand = new OleDbCommand(cmdText, oldDbConnection))
                    {
                        using (OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader())
                        {
                            while (oleDbDataReader != null && oleDbDataReader.Read())
                            {
                                string temp = oleDbDataReader.GetValue(0).ToString();
                                Item sig;

                                switch (temp.Substring(0, temp.IndexOf("_")))
                                {
                                    case "IN":
                                        sig = new Item(String.Format("$IN[{0}]", temp.Substring(3)),
                                            oleDbDataReader.GetValue(1).ToString());

                                        _inputs.Add(sig);
                                        LanguageText += sig + "\r\n";
                                        break;
                                    case "OUT":
                                        sig = new Item(String.Format("$OUT[{0}]", temp.Substring(4)),
                                            oleDbDataReader.GetValue(1).ToString());
                                        _outputs.Add(sig);
                                        LanguageText += sig + "\r\n";
                                        break;
                                    case "ANIN":
                                        sig = new Item(String.Format("$ANIN[{0}]", temp.Substring(5)),
                                            oleDbDataReader.GetValue(1).ToString());
                                        _anin.Add(sig);
                                        LanguageText += sig + "\r\n";
                                        break;
                                    case "ANOUT":
                                        sig = new Item(String.Format("$ANOUT[{0}]", temp.Substring(6)),
                                            oleDbDataReader.GetValue(1).ToString());
                                        _anout.Add(sig);
                                        LanguageText += sig + "\r\n";
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            GetFlags();
            GetTimers();
            GetAllLangtextFromDatabase();
            RaisePropertyChanged("DigInVisibility");
            RaisePropertyChanged("DigOutVisibility");
            RaisePropertyChanged("AnalogVisibility");
            RaisePropertyChanged("DigitalVisibility");
        }

        private void GetAllLangtextFromDatabase()
        {
            LanguageText = string.Empty;

            if (!File.Exists(DataBaseFile)) return;
            string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DataBaseFile + ";";
            const string cmdText =
                "SELECT i.keystring, m.string FROM ITEMS i, messages m where i.key_id=m.key_id and m.language_id=99";
            using (var oldDbConnection = new OleDbConnection(connectionString))
            {
                oldDbConnection.Open();
                using (var oleDbCommand = new OleDbCommand(cmdText, oldDbConnection))
                {
                    using (OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader())
                    {
                        while (oleDbDataReader != null && oleDbDataReader.Read())
                        {
                            string str1 = oleDbDataReader.GetValue(0).ToString();
                            string str2 = oleDbDataReader.GetValue(1).ToString();
                            DataBase += string.Format("{0} {1}\r\n", str1, str2);
                        }
                    }
                }
            }
        }

// ReSharper disable UnusedMember.Local
        private void ImportFile(string sFile, bool bCsv)
// ReSharper restore UnusedMember.Local
        {
            if (!File.Exists(sFile))
                return;


            // MyProject.Forms.FormMain.LangtextOpen(this.sFileDb);
            DialogResult result = MessageBox.Show("Delete existing long texts?", "Import File", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                //DeleteAllLanguageText
            }


            string[] array = File.ReadAllLines(sFile);
            string text = " ";
            if (bCsv)
                text = ";";


            var progress = new ProgressBarViewModel {Maximum = array.Length, Value = 0, IsVisible = true};

            Application.DoEvents();
            string[] array2 = array;
            checked
            {
                foreach (string text2 in array2)
                {
                    if (text2.Contains(text))
                    {
                        string[] array3 = text2.Split(text.ToCharArray(), 2);

                        if (array3[0].StartsWith("$IN[", StringComparison.CurrentCultureIgnoreCase))
                            array3[0] = "IN_" + array3[0].Substring(4, array3[0].Length - 5);
                        else if (array3[0].StartsWith("$OUT[", StringComparison.CurrentCultureIgnoreCase))
                            array3[0] = "OUT_" + array3[0].Substring(5, array3[0].Length - 6);
                        else if (array3[0].StartsWith("$TIMER[", StringComparison.CurrentCultureIgnoreCase))
                            array3[0] = "TimerText" + array3[0].Substring(7, array3[0].Length - 8);
                        else if (array3[0].StartsWith("$COUNT_I[", StringComparison.CurrentCultureIgnoreCase))
                            array3[0] = "CounterText" + array3[0].Substring(9, array3[0].Length - 10);
                        else if (array3[0].StartsWith("$FLAG[", StringComparison.CurrentCultureIgnoreCase))
                            array3[0] = "FlagText" + array3[0].Substring(6, array3[0].Length - 7);
                        else if (array3[0].StartsWith("$CYC_FLAG[", StringComparison.CurrentCultureIgnoreCase))
                            array3[0] = "NoticeText" + array3[0].Substring(10, array3[0].Length - 11);
                        else if (array3[0].StartsWith("$ANIN[", StringComparison.CurrentCultureIgnoreCase))
                            array3[0] = "ANIN_" + array3[0].Substring(6, array3[0].Length - 7);
                        else if (array3[0].StartsWith("$ANOUT[", StringComparison.CurrentCultureIgnoreCase))
                            array3[0] = "ANOUT_" + array3[0].Substring(6, array3[0].Length - 8);
                        //     MyProject.Forms.FormMain.LangtextWrite("IO", array3[0], array3[1]);
                    }
                    progress.Value++;
                }
                progress.IsVisible = false;
                InitGrids();
            }
        }

        private void Open()
        {
            var openFileDialog = new OpenFileDialog();
            OpenFileDialog openFileDialog2 = openFileDialog;

            openFileDialog2.Filter = "longtext (*.mdb)|*.mdb";


            openFileDialog2.Multiselect = false;
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;
            _dbFile = openFileDialog.FileName;
            DataBase = _dbFile;
            InitGrids();
        }

#pragma warning disable 649
        private string[] _langText;
#pragma warning restore 649

// ReSharper disable UnusedMember.Local
        private void Export(string filename, bool iscsv)
// ReSharper restore UnusedMember.Local
        {
            string text = "";
            string text2 = " ";
            _progress.Maximum = 9551;
            _progress.Value = 0;
            _progress.IsVisible = true;

            string pre = _isKRC2 ? string.Empty : "$";
            string post = _isKRC2 ? "_" : "]";
            if (iscsv)
                text2 = ";";

            checked
            {
                if (_isKRC2)
                {
                    for (int i = 1; i < 4096; i++)
                    {
                        if (!String.IsNullOrEmpty(_langText[i]))
                            text = string.Format("{0}IN{1}{2}{3}{4}\r\n", pre, post, i, text2, _langText[i]);

                        _progress.Value++;
                    }

                    for (int i = 1; i < 4096; i++)
                    {
                        if (!String.IsNullOrEmpty(_langText[i]))
                            text = string.Format("{0}OUT{1}{2}{3}{4}\r\n", pre, post, i, text2, _langText[i]);

                        _progress.Value++;
                    }

                    for (int i = 1; i < 32; i++)
                    {
                        if (!String.IsNullOrEmpty(_langText[i]))
                            text = string.Format("{0}ANIN{1}{2}{3}{4}\r\n", pre, post, i, text2, _langText[i]);

                        _progress.Value++;
                    }

                    for (int i = 1; i < 32; i++)
                    {
                        if (!String.IsNullOrEmpty(_langText[i]))
                            text = string.Format("{0}ANOUT{1}{2}{3}{4}\r\n", pre, post, i, text2, _langText[i]);

                        _progress.Value++;
                    }
                    for (int i = 1; i < 20; i++)
                    {
                        if (!String.IsNullOrEmpty(_langText[i]))
                            text = string.Format("{0}{1}{2}{3}{4}\r\n", _isKRC2 ? "TimerText" : "$Timer[", post, i,
                                text2, _langText[i]);

                        _progress.Value++;
                    }

                    for (int i = 1; i < 20; i++)
                    {
                        if (!String.IsNullOrEmpty(_langText[i]))
                            text = string.Format("{0}{1}{2}{3}{4}\r\n", _isKRC2 ? "CounterText" : "$Counter[", post, i,
                                text2, _langText[i]);

                        _progress.Value++;
                    }

                    for (int i = 1; i < 999; i++)
                    {
                        if (!String.IsNullOrEmpty(_langText[i]))
                            text = string.Format("{0}{1}{2}{3}{4}\r\n", _isKRC2 ? "FlagText" : "$FLAG[", post, i, text2,
                                _langText[i]);

                        _progress.Value++;
                    }

                    for (int i = 1; i < 256; i++)
                    {
                        if (!String.IsNullOrEmpty(_langText[i]))
                            text = string.Format("{0}{1}{2}{3}{4}\r\n", _isKRC2 ? "NoticeText" : "$CycFlag[", post, i,
                                text2, _langText[i]);

                        _progress.Value++;
                    }
                }

                File.WriteAllText(filename, text);
                _progress.IsVisible = false;
            }
        }

        private void InitGrids()
        {
            _progress.Maximum = 5403;
            _progress.Value = 0;
            _progress.IsVisible = true;

            //            MyProject.Forms.FormMain.LangtextFillBuffer(this.sFileDb);
#pragma warning disable 219
            int num = 1;
#pragma warning restore 219
            checked
            {
                for (int i = 1; i < 4096; i++)
                {
                    _inputs.Add(new Item(String.Format("$IN[{0}]", i), string.Empty));
                    _outputs.Add(new Item(String.Format("$OUT[{0}]", i), string.Empty));

                    _progress.Value++;

                    num++;
                }

                for (int i = 1; i < 32; i++)
                {
                    _anin.Add(new Item(String.Format("$ANIN[{0}]", i), string.Empty));
                    _anout.Add(new Item(String.Format("$ANOUT[{0}]", i), string.Empty));
                    _progress.Value++;
                }

                for (int i = 1; i < 20; i++)
                {
                    _timer.Add(new Item(String.Format("$TIMER[{0}]", i), string.Empty));
                    _counter.Add(new Item(String.Format("$COUNT_I[{0}]", i), string.Empty));
                    _progress.Value++;
                }
                for (int i = 1; i < 999; i++)
                {
                    _flags.Add(new Item(String.Format("$FLAG[{0}]", i), string.Empty));
                    _progress.Value++;
                }
                for (int i = 1; i < 256; i++)
                {
                    _cycflags.Add(new Item(String.Format("$CYCFLAG[{0}]", i), string.Empty));
                    _progress.Value++;
                }

                _progress.IsVisible = false;
            }
        }

        private void GetFiles(string dir)
        {
            // Does AM.INI Exist?
            if (File.Exists(dir + "\\am.ini"))
                InfoFile = dir + "\\am.ini";

            if (File.Exists(dir + "\\C\\KRC\\Data\\kuka_con.mdb"))
                DataBaseFile = dir + "\\C\\KRC\\Data\\kuka_con.mdb";
            if ((File.Exists(InfoFile)) && (File.Exists(DataBaseFile)))
                return;

            foreach (string d in Directory.GetDirectories(dir))
            {
                foreach (string f in Directory.GetFiles(d))
                {
                    string fileName = Path.GetFileName(f);
                    if (fileName == null) continue;
                    string file = fileName.ToLower();

#if TRACE
                    Console.WriteLine(file);
#endif
                    switch (file)
                    {
                        case "am.ini":
                            InfoFile = f;
                            break;
                        case "kuka_con.mdb":
                            DataBaseFile = f;
                            break;
                    }
                }

                GetFiles(d);
            }
        }
    }
}