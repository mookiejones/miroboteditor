using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.OleDb;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using Ionic.Zip;
using miRobotEditor.Core;

namespace miRobotEditor.ViewModel
{
    public class IOViewModel:ViewModelBase
    {
        #region Properties

        public Visibility DigInVisibility { get { return Inputs.Count > 0 ? Visibility.Visible : Visibility.Hidden; } }
        public Visibility DigOutVisibility { get { return Outputs.Count > 0 ? Visibility.Visible : Visibility.Hidden; } }
        public Visibility AnInVisibility { get { return AnIn.Count > 0 ? Visibility.Visible : Visibility.Hidden; } }
        public Visibility AnOutVisibility { get { return AnOut.Count > 0 ? Visibility.Visible : Visibility.Hidden; } }


        public Visibility DigitalVisibility { get { return ((DigInVisibility == Visibility.Visible) && (DigOutVisibility == Visibility.Visible)) ? Visibility.Visible : Visibility.Collapsed; } }
        public Visibility AnalogVisibility
        {
            get
            {
                return ((AnOutVisibility == Visibility.Visible) || (AnInVisibility == Visibility.Visible)) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private Visibility _flagVisibility = Visibility.Collapsed;
        public Visibility FlagVisibility { get { return _flagVisibility; } set { _flagVisibility = value; RaisePropertyChanged(); } }

        private Visibility _timerVisibility = Visibility.Collapsed;
        public Visibility TimerVisibility { get { return _timerVisibility; } set { _timerVisibility = value; RaisePropertyChanged(); } }

        private Visibility _cyclicFlagVisibility = Visibility.Collapsed;
        public Visibility CyclicFlagVisibility { get { return _cyclicFlagVisibility; } set { _cyclicFlagVisibility = value; RaisePropertyChanged(); } }

        private Visibility _counterVisibility = Visibility.Collapsed;
        public Visibility CounterVisibility { get { return _counterVisibility; } set { _counterVisibility = value; RaisePropertyChanged(); } }

        private InfoFile _info = new InfoFile();
        public InfoFile Info { get { return _info; } set { _info = value; RaisePropertyChanged(); } }


        public string DirectoryPath { get; set; }
        private string _archivePath = " ";
        public string ArchivePath { get { return _archivePath; } set { _archivePath = value; RaisePropertyChanged(); } }


        private string _filecount = string.Empty;
        public string FileCount { get { return _filecount; } set { _filecount = value; RaisePropertyChanged(); } }

        public ZipFile ArchiveZip { get; set; }
        private string _buffersize = string.Empty;

        public string BufferSize { get { return _buffersize; } set { _buffersize = value; RaisePropertyChanged(); } }

        public string DataBaseFile { get; set; }
        // ReSharper disable ConvertToConstant.Local
        // ReSharper disable FieldCanBeMadeReadOnly.Local
        // ReSharper restore FieldCanBeMadeReadOnly.Local
        // ReSharper restore ConvertToConstant.Local

        private string _database = string.Empty;
        public string DataBase { get { return _database; } set { _database = value; RaisePropertyChanged(); } }

        public string InfoFile { get; set; }


        readonly ObservableCollection<DirectoryInfo> _root = new ObservableCollection<DirectoryInfo>();
        readonly ReadOnlyObservableCollection<DirectoryInfo> _readonlyRoot = null;
        public ReadOnlyObservableCollection<DirectoryInfo> Root { get { return _readonlyRoot ?? new ReadOnlyObservableCollection<DirectoryInfo>(_root); } }

        private DirectoryInfo _rootpath;
        public DirectoryInfo RootPath { get { return _rootpath; } set { _rootpath = value; RaisePropertyChanged(); } }
        private string _languageText = String.Empty;
        public string LanguageText { get { return _languageText; } set { _languageText = value; RaisePropertyChanged(); } }
        private string _databaseText = String.Empty;
        public string DatabaseText { get { return _databaseText; } set { _databaseText = value; RaisePropertyChanged(); } }




        readonly List<Item> _inputs = new List<Item>();
        readonly ReadOnlyCollection<Item> _readonlyinputs = null;
        public ReadOnlyCollection<Item> Inputs { get { return _readonlyinputs ?? new ReadOnlyCollection<Item>(_inputs); } }

        readonly List<Item> _outputs = new List<Item>();
        readonly ReadOnlyCollection<Item> _readonlyOutputs = null;
        public ReadOnlyCollection<Item> Outputs { get { return _readonlyOutputs ?? new ReadOnlyCollection<Item>(_outputs); } }

        readonly List<Item> _anin = new List<Item>();
        readonly ReadOnlyCollection<Item> _readonlyAnIn = null;
        public ReadOnlyCollection<Item> AnIn { get { return _readonlyAnIn ?? new ReadOnlyCollection<Item>(_anin); } }

        readonly List<Item> _anout = new List<Item>();
        readonly ReadOnlyCollection<Item> _readonlyAnOut = null;
        public ReadOnlyCollection<Item> AnOut { get { return _readonlyAnOut ?? new ReadOnlyCollection<Item>(_anout); } }

        readonly List<Item> _timer = new List<Item>();
        readonly ReadOnlyCollection<Item> _readonlyTimer = null;
        public ReadOnlyCollection<Item> Timer { get { return _readonlyTimer ?? new ReadOnlyCollection<Item>(_timer); } }

        readonly List<Item> _flags = new List<Item>();
        readonly ReadOnlyCollection<Item> _readonlyFlags = null;
        public ReadOnlyCollection<Item> Flags { get { return _readonlyFlags ?? new ReadOnlyCollection<Item>(_flags); } }

        readonly List<Item> _cycflags = new List<Item>();
        readonly ReadOnlyCollection<Item> _readonlyCycFlags = null;
        public ReadOnlyCollection<Item> CycFlags { get { return _readonlyCycFlags ?? new ReadOnlyCollection<Item>(_cycflags); } }

        readonly List<Item> _counter = new List<Item>();
        readonly ReadOnlyCollection<Item> _readonlyCounter = null;
        public ReadOnlyCollection<Item> Counter { get { return _readonlyCounter ?? new ReadOnlyCollection<Item>(_counter); } }

        #endregion

        private readonly BackgroundWorker _backgroundWorker;

        #region Constructor
        public IOViewModel(string filename)
        {
            DataBaseFile = filename;
            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.DoWork +=_backgroundWorker_DoWork;
            _backgroundWorker.RunWorkerCompleted += _backgroundWorker_RunWorkerCompleted;
            _backgroundWorker.RunWorkerAsync();
        }
        #endregion

        #region Background Worker
        void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
             {
                 GetSignals();
                 GetTimers();
                 GetAllLangtextFromDatabase();
             }
        void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
            {
                RaisePropertyChanged("Inputs");
                RaisePropertyChanged("Outputs");
                RaisePropertyChanged("AnIn");
                RaisePropertyChanged("AnOut");
                RaisePropertyChanged("Counter");
                RaisePropertyChanged("Flags");
                RaisePropertyChanged("Timer");
            }
        #endregion
        /// <summary>
        /// Gets Signals from kuka_con.mdb
        /// </summary>
        void GetSignals()
        {
            if (File.Exists(DataBaseFile))
            {
                var connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DataBaseFile + ";";
                const string cmdText = "SELECT Items.KeyString, Messages.[String] FROM (Items INNER JOIN Messages ON Items.Key_id = Messages.Key_id)WHERE (Items.[Module] = 'IO')";
                using (var oldDbConnection = new OleDbConnection(connectionString))
                {
                    oldDbConnection.Open();
                    using (var oleDbCommand = new OleDbCommand(cmdText, oldDbConnection))
                    {
                        using (var oleDbDataReader = oleDbCommand.ExecuteReader())
                        {
                            while (oleDbDataReader != null && oleDbDataReader.Read())
                            {
                                var temp = oleDbDataReader.GetValue(0).ToString();
                                Item sig;
                                var des = oleDbDataReader.GetValue(1).ToString();
                                var description = des=="|EMPTY|"?"Spare":des;
                                switch (temp.Substring(0, temp.IndexOf("_")))
                                {
                                    case "IN":
                                        sig = new Item(String.Format("$IN[{0}]", temp.Substring(3)), description);
                                        _inputs.Add(sig);
                                        LanguageText += sig + "\r\n";
                                        break;
                                    case "OUT":
                                        sig = new Item(String.Format("$OUT[{0}]", temp.Substring(4)), description);
                                        if (!_outputs.Contains(sig))
                                        _outputs.Add(sig);
                                        LanguageText += sig + "\r\n";
                                        break;
                                    case "ANIN":
                                        sig = new Item(String.Format("$ANIN[{0}]", temp.Substring(5)), description);
                                        _anin.Add(sig);
                                        LanguageText += sig + "\r\n";
                                        break;
                                    case "ANOUT":
                                        sig = new Item(String.Format("$ANOUT[{0}]", temp.Substring(6)), description);
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

        /// <summary>
        /// Gets Flags from kuka_con.mdb
        /// </summary>
        void GetFlags()
        {
            if (!File.Exists(DataBaseFile)) return;
            var connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DataBaseFile + ";";
            const string cmdText = "SELECT Items.KeyString, Messages.[String] FROM (Items INNER JOIN Messages ON Items.Key_id = Messages.Key_id)WHERE (Items.[Module] = 'FLAG')";
            using (var oldDbConnection = new OleDbConnection(connectionString))
            {
                oldDbConnection.Open();
                using (var oleDbCommand = new OleDbCommand(cmdText, oldDbConnection))
                {
                    using (var oleDbDataReader = oleDbCommand.ExecuteReader())
                    {
                        while (oleDbDataReader != null && oleDbDataReader.Read())
                        {
                            var temp = oleDbDataReader.GetValue(0).ToString();
                            var sig = new Item(String.Format("$FLAG[{0}]", temp.Substring(8)), oleDbDataReader.GetValue(1).ToString());
                            _flags.Add(sig);
                        }
                    }
                }
            }
            FlagVisibility = Flags.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            RaisePropertyChanged("FlagVisibility");

        }
        /// <summary>
        /// Gets Timers from kuka_con.mdb
        /// </summary>
        void GetTimers()
        {
            var connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DataBaseFile + ";";
            const string cmdText = "SELECT Items.KeyString, Messages.[String] FROM (Items INNER JOIN Messages ON Items.Key_id = Messages.Key_id)WHERE (Items.[Module] = 'TIMER')";
            using (var oldDbConnection = new OleDbConnection(connectionString))
            {
                oldDbConnection.Open();
                using (var oleDbCommand = new OleDbCommand(cmdText, oldDbConnection))
                {
                    using (var oleDbDataReader = oleDbCommand.ExecuteReader())
                    {
                        while (oleDbDataReader != null && oleDbDataReader.Read())
                        {
                            var temp = oleDbDataReader.GetValue(0).ToString();
                            var sig = new Item(String.Format("$TIMER[{0}]", temp.Substring(9)), oleDbDataReader.GetValue(1).ToString());
                            _timer.Add(sig);
                        }
                    }
                }
            }
            TimerVisibility = Timer.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            RaisePropertyChanged("TimerVisibility");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        // ReSharper disable UnusedMember.Local
        List<Item> GetValues(string cmd, int index)
        // ReSharper restore UnusedMember.Local
        {
            var result = new List<Item>();
            var connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DataBaseFile + ";";
            var cmdText = String.Format("SELECT Items.KeyString, Messages.[String] FROM (Items INNER JOIN Messages ON Items.Key_id = Messages.Key_id)WHERE (Items.[Module] = '{0}')", cmd);
            using (var oldDbConnection = new OleDbConnection(connectionString))
            {
                oldDbConnection.Open();
                using (var oleDbCommand = new OleDbCommand(cmdText, oldDbConnection))
                {
                    using (var oleDbDataReader = oleDbCommand.ExecuteReader())
                    {
                        while (oleDbDataReader != null && oleDbDataReader.Read())
                        {
                            var temp = oleDbDataReader.GetValue(0).ToString();
                            var sig = new Item(String.Format("${1}[{0}]", temp.Substring(index), cmd), oleDbDataReader.GetValue(1).ToString());
                            result.Add(sig);
                        }
                    }
                }
            }
            return result;
        }

        // ReSharper disable UnusedMember.Local
        void GetSignalsFromDataBase()
        // ReSharper restore UnusedMember.Local
        {
            var ofd = new OpenFileDialog { Title = "Select Database", Filter = "KUKA Connection Files (kuka_con.mdb)|kuka_con.mdb|All files (*.*)|*.*", Multiselect = false };
            ofd.ShowDialog();
            LanguageText = string.Empty;

            DataBaseFile = ofd.FileName;
            GetSignals();
        }
        private void GetAllLangtextFromDatabase()
        {
            LanguageText = string.Empty;

            if (!File.Exists(DataBaseFile)) return;
            var connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DataBaseFile + ";";
            const string cmdText = "SELECT i.keystring, m.string FROM ITEMS i, messages m where i.key_id=m.key_id and m.language_id=99";
            using (var oldDbConnection = new OleDbConnection(connectionString))
            {
                oldDbConnection.Open();
                using (var oleDbCommand = new OleDbCommand(cmdText, oldDbConnection))
                {
                    using (var oleDbDataReader = oleDbCommand.ExecuteReader())
                    {
                        while (oleDbDataReader != null && oleDbDataReader.Read())
                        {
                            var str1 = oleDbDataReader.GetValue(0).ToString();
                            var str2 = oleDbDataReader.GetValue(1).ToString();
                            DataBase += string.Format("{0} {1}\r\n", str1, str2);
                        }
                    }
                }
            }
        }

    }
}
