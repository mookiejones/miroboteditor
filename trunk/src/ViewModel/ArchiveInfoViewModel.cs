using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.IO;
using miRobotEditor.ViewModel;
using System.Windows.Forms;
using System.Windows.Input;
using miRobotEditor.Commands;
using System.Collections.ObjectModel;
using Ionic.Zip;
using System.Diagnostics;
using System.Windows.Data;
using System.Globalization;
using System.Windows;
namespace miRobotEditor.ViewModel
{
    public class ArchiveInfoViewModel:ToolViewModel
    {
        #region Constructor
        public ArchiveInfoViewModel():base("ArchiveInfoViewModel",300)
        {
            
            Root = new ObservableCollection<DirectoryInfo>();
            this.Inputs = new List<Item>(4096);
            this.Outputs = new List<Item>(4096);
            this.AnIn = new List<Item>(32);
            this.AnOut = new List<Item>(32);
            this.Timer = new List<Item>(20);
            this.Counter = new List<Item>(20);
            this.Flags = new List<Item>(999);
            this.CycFlags = new List<Item>(256);
            RaisePropertyChanged("DigInVisibility");
            RaisePropertyChanged("DigOutVisibility");
            RaisePropertyChanged("AnInVisibility");
            RaisePropertyChanged("AnOutVisibility");

        }
        #endregion

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
        public Visibility FlagVisibility { get { return _flagVisibility; } set { _flagVisibility = value; RaisePropertyChanged("FlagVisibility"); } }

        private Visibility _timerVisibility = Visibility.Collapsed;
        public Visibility TimerVisibility { get { return _timerVisibility; } set { _timerVisibility = value; RaisePropertyChanged("TimerVisibility"); } }

        private Visibility _cyclicFlagVisibility = Visibility.Collapsed;
        public Visibility CyclicFlagVisibility { get { return _cyclicFlagVisibility; } set { _cyclicFlagVisibility = value; RaisePropertyChanged("CyclicFlagVisibility"); } }

        private Visibility _counterVisibility = Visibility.Collapsed;
        public Visibility CounterVisibility { get { return _counterVisibility; } set { _counterVisibility = value; RaisePropertyChanged("CounterVisibility"); } }

        private ProgressBarViewModel _progress = new ProgressBarViewModel();

        private InfoFile _info = new InfoFile();
        public InfoFile Info { get { return _info; } set { _info = value; RaisePropertyChanged("Info"); } }


        public string DirectoryPath { get; set; }
        private string _archivePath = " ";
        public string ArchivePath { get { return _archivePath; } set { _archivePath = value; RaisePropertyChanged("ArchivePath"); } }


        private string _filecount = string.Empty;
        public string FileCount { get { return _filecount; } set { _filecount = value; RaisePropertyChanged("FileCount"); } }

        public ZipFile ArchiveZip { get; set; }
        private string _buffersize = string.Empty;

        public string BufferSize { get { return _buffersize; } set { _buffersize = value; RaisePropertyChanged("BufferSize"); } }

        string _dbFile = string.Empty;
        public string DataBaseFile { get; set; }
        private bool isKRC2 = true;

        private string _database = string.Empty;
        public string DataBase { get { return _database; } set { _database = value; RaisePropertyChanged("DataBase"); } }

        public string InfoFile { get; set; }

        public ObservableCollection<DirectoryInfo> Root { get; set; }

        private static string StartupPath
        {
            get
            {
                return System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            }
        }

        private DirectoryInfo _rootpath = null;
        public DirectoryInfo RootPath { get { return _rootpath; } set { _rootpath = value; RaisePropertyChanged("RootPath"); } }
        private string _languageText = String.Empty;
        public string LanguageText { get { return _languageText; } set { _languageText = value; RaisePropertyChanged("LanguageText"); } }
        private string _databaseText = String.Empty;
        public string DatabaseText { get { return _databaseText; } set { _databaseText = value; RaisePropertyChanged("DatabaseText"); } }


        public ObservableCollection<FileModel> Files { get; set; }
        public List<Item> Inputs { get; set; }
        public List<Item> Outputs { get; set; }
        public List<Item> AnIn { get; set; }
        public List<Item> AnOut { get; set; }
        public List<Item> Timer { get; set; }
        public List<Item> Flags { get; set; }
        public List<Item> CycFlags { get; set; }
        public List<Item> Counter { get; set; }
        #endregion

        #region Commands

        private RelayCommand _openCommand;
        public ICommand OpenCommand
        {
            get { return _openCommand ?? (_openCommand = new RelayCommand(param => Open(), param => true)); }
        }
        private RelayCommand _importCommand;
        public ICommand ImportCommand
        {
            get { return _importCommand ?? (_importCommand = new RelayCommand(param => Import(), param => true)); }
        }

        private RelayCommand _loadCommand;
        public ICommand LoadCommand
        {
            get { return _loadCommand ?? (_loadCommand = new RelayCommand(param => this.GetSignals(), param => true)); }
        }


        #endregion

        //TODO need to make this into allowing either a folder or a zip file.

        void Import()
        {
            var ofd = new OpenFileDialog { Title = "Select Archive", Filter = "KUKA Archive (*.zip)|*.zip", Multiselect = false };
            ofd.ShowDialog();

            Root.Clear();
            if (File.Exists(ofd.FileName))
            {
                ArchivePath = ofd.FileName;
                ArchiveZip = new ZipFile(ofd.FileName);
                RaisePropertyChanged("ArchiveZip");
                GetAllLangtextFromDatabase();
                UnpackZip();
                GetFiles(DirectoryPath);
                GetAMInfo();
                GetSignals();
            }
        }

        void ReadZip()
        {
            foreach (ZipEntry z in ArchiveZip.EntriesSorted)
                if (z.IsDirectory)
                    Console.WriteLine("Yes");


            foreach (ZipEntry e in ArchiveZip.Entries)
                if (e.IsDirectory)
                    Console.WriteLine("Yes");


            RaisePropertyChanged("Root");

        }
      
        bool CheckPathExists(string path)
        {
            // if archive path exists, allow to delete or rename
            if (!Directory.Exists(path)) return false;

            var msgResult = System.Windows.Forms.MessageBox.Show(String.Format("The path of {0} \r\n allready exists. Do you want to Delete the path?", path), "Archive Exists", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button3);

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
       
        void UnpackZip()
        {
            
          
           DirectoryPath= Path.Combine(StartupPath,Path.GetFileNameWithoutExtension(ArchivePath));
           var exists= CheckPathExists(DirectoryPath);
           if (exists) return;

                //  This call to ExtractAll() assumes:
                //   - none of the entries are password-protected.
                //   - want to extract all entries to current working directory
                //   - none of the files in the zip already exist in the directory;
                //     if they do, the method will throw.
           ArchiveZip.ExtractAll(DirectoryPath);

           Root.Add(new DirectoryInfo(DirectoryPath));
        }
     
        void GetDirectories()
        {

//            Root = new ObservableCollection<DirectoryRecord>{  new DirectoryRecord { Info = new DirectoryInfo(folder)
           
        }

        void GetAMInfo()
        {
            if (File.Exists(InfoFile))
            {
                var file = File.ReadAllLines(InfoFile);
                foreach (var f in file)
                {
                    var sp = f.Split('=');
                    if (sp.Length > 0)
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
            }
        }

        void GetFlags()
        {
            string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DataBaseFile + ";";
            string cmdText = "SELECT Items.KeyString, Messages.[String] FROM (Items INNER JOIN Messages ON Items.Key_id = Messages.Key_id)WHERE (Items.[Module] = 'FLAG')";
            using (var oldDbConnection = new OleDbConnection(connectionString))
            {
                oldDbConnection.Open();
                using (var oleDbCommand = new OleDbCommand(cmdText, oldDbConnection))
                {
                    using (var oleDbDataReader = oleDbCommand.ExecuteReader())
                    {
                        string temp = string.Empty;
                        while (oleDbDataReader.Read())
                        {
                            temp = oleDbDataReader.GetValue(0).ToString();
                            var sig = new Item(String.Format("$FLAG[{0}]", temp.Substring(8)), oleDbDataReader.GetValue(1).ToString());
                            Flags.Add(sig);
                        }
                    }
                }
            }
            FlagVisibility = Flags.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            RaisePropertyChanged("FlagVisibility");

        }

        List<Item> GetValues(string cmd, int index)
        {
            List<Item> Result = new List<Item>();
            string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DataBaseFile + ";";
            string cmdText = String.Format("SELECT Items.KeyString, Messages.[String] FROM (Items INNER JOIN Messages ON Items.Key_id = Messages.Key_id)WHERE (Items.[Module] = '{0}')",cmd);
            using (var oldDbConnection = new OleDbConnection(connectionString))
            {
                oldDbConnection.Open();
                using (var oleDbCommand = new OleDbCommand(cmdText, oldDbConnection))
                {
                    using (var oleDbDataReader = oleDbCommand.ExecuteReader())
                    {
                        string temp = string.Empty;
                        while (oleDbDataReader.Read())
                        {
                            temp = oleDbDataReader.GetValue(0).ToString();
                            var sig = new Item(String.Format("${1}[{0}]", temp.Substring(index),cmd), oleDbDataReader.GetValue(1).ToString());
                            Result.Add(sig);
                        }
                    }
                }
            }
            return Result;
        }

        void GetTimers()
        {
            string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DataBaseFile + ";";
            string cmdText = "SELECT Items.KeyString, Messages.[String] FROM (Items INNER JOIN Messages ON Items.Key_id = Messages.Key_id)WHERE (Items.[Module] = 'TIMER')";
            using (var oldDbConnection = new OleDbConnection(connectionString))
            {
                oldDbConnection.Open();
                using (var oleDbCommand = new OleDbCommand(cmdText, oldDbConnection))
                {
                    using (var oleDbDataReader = oleDbCommand.ExecuteReader())
                    {
                        string temp = string.Empty;
                        while (oleDbDataReader.Read())
                        {
                            temp = oleDbDataReader.GetValue(0).ToString();
                            var sig = new Item(String.Format("$TIMER[{0}]", temp.Substring(9)), oleDbDataReader.GetValue(1).ToString());
                            Timer.Add(sig);
                        }
                    }
                }
            }
            TimerVisibility = Timer.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            RaisePropertyChanged("TimerVisibility");
        }

        void GetSignalsFromDataBase()
        {
            var ofd = new OpenFileDialog { Title = "Select Database", Filter = "KUKA Connection Files (kuka_con.mdb)|kuka_con.mdb|All files (*.*)|*.*", Multiselect = false };
            ofd.ShowDialog();
            LanguageText = string.Empty;

            DataBaseFile = ofd.FileName;
            GetSignals();
        }
    
        void GetSignals()
        {
            if (File.Exists(DataBaseFile))
            {
                string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DataBaseFile + ";";
                string cmdText = "SELECT Items.KeyString, Messages.[String] FROM (Items INNER JOIN Messages ON Items.Key_id = Messages.Key_id)WHERE (Items.[Module] = 'IO')";
                using (var oldDbConnection = new OleDbConnection(connectionString))
                {
                    oldDbConnection.Open();
                    using (var oleDbCommand = new OleDbCommand(cmdText, oldDbConnection))
                    {
                        using (var oleDbDataReader = oleDbCommand.ExecuteReader())
                        {
                            string temp = string.Empty;
                            while (oleDbDataReader.Read())
                            {
                                temp = oleDbDataReader.GetValue(0).ToString();
                                Item sig;

                                switch (temp.Substring(0, temp.IndexOf("_")))
                                {
                                    case "IN":
                                        sig = new Item(String.Format("$IN[{0}]", temp.Substring(3)), oleDbDataReader.GetValue(1).ToString());

                                        Inputs.Add(sig);
                                        LanguageText += sig.ToString() + "\r\n";
                                        break;
                                    case "OUT":
                                        sig = new Item(String.Format("$OUT[{0}]", temp.Substring(4)), oleDbDataReader.GetValue(1).ToString());
                                        Outputs.Add(sig);
                                        LanguageText += sig.ToString() + "\r\n";
                                        break;
                                    case "ANIN":
                                        sig = new Item(String.Format("$ANIN[{0}]", temp.Substring(5)), oleDbDataReader.GetValue(1).ToString());
                                        this.AnIn.Add(sig);
                                        LanguageText += sig.ToString() + "\r\n";
                                        break;
                                    case "ANOUT":
                                        sig = new Item(String.Format("$ANOUT[{0}]", temp.Substring(6)), oleDbDataReader.GetValue(1).ToString());
                                        this.AnOut.Add(sig);
                                        LanguageText += sig.ToString() + "\r\n";
                                        break;
                                    default:
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

            if (File.Exists(DataBaseFile))
            {
                string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DataBaseFile + ";";
                string cmdText = "SELECT i.keystring, m.string FROM ITEMS i, messages m where i.key_id=m.key_id and m.language_id=99";
                using (var oldDbConnection = new OleDbConnection(connectionString))
                {
                    oldDbConnection.Open();
                    using (var oleDbCommand = new OleDbCommand(cmdText, oldDbConnection))
                    {
                        using (var oleDbDataReader = oleDbCommand.ExecuteReader())
                        {
                            while (oleDbDataReader.Read())
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

        private void ImportFile(string sFile, bool bCsv)
        {
            if (!File.Exists(sFile))
                return;


            // MyProject.Forms.FormMain.LangtextOpen(this.sFileDb);
            var result = System.Windows.Forms.MessageBox.Show("Delete existing long texts?", "Import File", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                //DeleteAllLanguageText
            }



            string[] array = File.ReadAllLines(sFile);
            string text = " ";
            if (bCsv)
                text = ";";


            var progress = new ProgressBarViewModel { Maximum = array.Length, Value = 0, IsVisible = true };

            System.Windows.Forms.Application.DoEvents();
            string[] array2 = array;
            checked
            {
                for (int i = 0; i < array2.Length; i++)
                {
                    string text2 = array2[i];
                    if (text2.Contains(text))
                    {
                        string[] array3 = text2.Split(text.ToCharArray(), 2);

                        if (array3[0].StartsWith("$IN[", StringComparison.CurrentCultureIgnoreCase))
                            array3[0] = "IN_" + array3[0].Substring(4, array3[0].Length - 5);
                        else
                            if (array3[0].StartsWith("$OUT[", StringComparison.CurrentCultureIgnoreCase))
                                array3[0] = "OUT_" + array3[0].Substring(5, array3[0].Length - 6);
                            else
                                if (array3[0].StartsWith("$TIMER[", StringComparison.CurrentCultureIgnoreCase))
                                    array3[0] = "TimerText" + array3[0].Substring(7, array3[0].Length - 8);
                                else
                                    if (array3[0].StartsWith("$COUNT_I[", StringComparison.CurrentCultureIgnoreCase))
                                        array3[0] = "CounterText" + array3[0].Substring(9, array3[0].Length - 10);
                                    else
                                        if (array3[0].StartsWith("$FLAG[", StringComparison.CurrentCultureIgnoreCase))
                                            array3[0] = "FlagText" + array3[0].Substring(6, array3[0].Length - 7);
                                        else
                                            if (array3[0].StartsWith("$CYC_FLAG[", StringComparison.CurrentCultureIgnoreCase))
                                                array3[0] = "NoticeText" + array3[0].Substring(10, array3[0].Length - 11);
                                            else
                                                if (array3[0].StartsWith("$ANIN[", StringComparison.CurrentCultureIgnoreCase))
                                                    array3[0] = "ANIN_" + array3[0].Substring(6, array3[0].Length - 7);
                                                else
                                                    if (array3[0].StartsWith("$ANOUT[", StringComparison.CurrentCultureIgnoreCase))
                                                        array3[0] = "ANOUT_" + array3[0].Substring(6, array3[0].Length - 8);
                        //     MyProject.Forms.FormMain.LangtextWrite("IO", array3[0], array3[1]);


                    }
                    progress.Value++;
                    //   MyProject.Forms.FormMain.LangtextClose();

                }
                progress.IsVisible = false;
                this.InitGrids();
            }
        }

        void Open()
        {
            var openFileDialog = new OpenFileDialog();
            var openFileDialog2 = openFileDialog;

            openFileDialog2.Filter = "longtext (*.mdb)|*.mdb";


            openFileDialog2.Multiselect = false;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                _dbFile = openFileDialog.FileName;
                DataBase = _dbFile;
                this.InitGrids();
            }
        }
   
        string[] LangText;

        private void Export(string filename, bool iscsv)
        {
            string text = "";
            string text2 = " ";
            this._progress.Maximum = 9551;
            this._progress.Value = 0;
            this._progress.IsVisible = true;

            string pre = isKRC2 ? string.Empty : "$";
            string post = isKRC2 ? "_" : "]";
            if (iscsv)
                text2 = ";";

            checked
            {
                if (isKRC2)
                {
                    for (var i = 1; i < 4096; i++)
                    {
                        if (!String.IsNullOrEmpty(LangText[i]))
                            text = string.Format("{0}IN{1}{2}{3}{4}\r\n", pre, post, i, text2, LangText[i]);

                        _progress.Value++;
                    }

                    for (var i = 1; i < 4096; i++)
                    {
                        if (!String.IsNullOrEmpty(LangText[i]))
                            text = string.Format("{0}OUT{1}{2}{3}{4}\r\n", pre, post, i, text2, LangText[i]);

                        _progress.Value++;
                    }

                    for (var i = 1; i < 32; i++)
                    {
                        if (!String.IsNullOrEmpty(LangText[i]))
                            text = string.Format("{0}ANIN{1}{2}{3}{4}\r\n", pre, post, i, text2, LangText[i]);

                        _progress.Value++;
                    }

                    for (var i = 1; i < 32; i++)
                    {
                        if (!String.IsNullOrEmpty(LangText[i]))
                            text = string.Format("{0}ANOUT{1}{2}{3}{4}\r\n", pre, post, i, text2, LangText[i]);

                        _progress.Value++;
                    }
                    for (var i = 1; i < 20; i++)
                    {
                        if (!String.IsNullOrEmpty(LangText[i]))
                            text = string.Format("{0}{1}{2}{3}{4}\r\n", isKRC2 ? "TimerText" : "$Timer[", post, i, text2, LangText[i]);

                        _progress.Value++;
                    }

                    for (var i = 1; i < 20; i++)
                    {
                        if (!String.IsNullOrEmpty(LangText[i]))
                            text = string.Format("{0}{1}{2}{3}{4}\r\n", isKRC2 ? "CounterText" : "$Counter[", post, i, text2, LangText[i]);

                        _progress.Value++;
                    }

                    for (var i = 1; i < 999; i++)
                    {
                        if (!String.IsNullOrEmpty(LangText[i]))
                            text = string.Format("{0}{1}{2}{3}{4}\r\n", isKRC2 ? "FlagText" : "$FLAG[", post, i, text2, LangText[i]);

                        _progress.Value++;
                    }

                    for (var i = 1; i < 256; i++)
                    {
                        if (!String.IsNullOrEmpty(LangText[i]))
                            text = string.Format("{0}{1}{2}{3}{4}\r\n", isKRC2 ? "NoticeText" : "$CycFlag[", post, i, text2, LangText[i]);

                        _progress.Value++;
                    }

                }

                File.WriteAllText(filename, text);
                this._progress.IsVisible = false;
            }

        }

        private void InitGrids()
        {

            this._progress.Maximum = 5403;
            this._progress.Value = 0;
            this._progress.IsVisible = true;

            this.Inputs = new List<Item>(4096);
            this.Outputs = new List<Item>(4096);
            this.AnIn = new List<Item>(32);
            this.AnOut = new List<Item>(32);
            this.Timer = new List<Item>(20);
            this.Counter = new List<Item>(20);
            this.Flags = new List<Item>(999);
            this.CycFlags = new List<Item>(256);

            //            MyProject.Forms.FormMain.LangtextFillBuffer(this.sFileDb);
            int num = 1;
            checked
            {
                for (int i = 1; i < 4096; i++)
                {
                    Inputs.Add(new Item(String.Format("$IN[{0}]", i), string.Empty));
                    Outputs.Add(new Item(String.Format("$OUT[{0}]", i), string.Empty));

                    _progress.Value++;

                    num++;
                }

                for (var i = 1; i < 32; i++)
                {
                    AnIn.Add(new Item(String.Format("$ANIN[{0}]", i), string.Empty));
                    AnOut.Add(new Item(String.Format("$ANOUT[{0}]", i), string.Empty));
                    _progress.Value++;
                }

                for (var i = 1; i < 20; i++)
                {
                    Timer.Add(new Item(String.Format("$TIMER[{0}]", i), string.Empty));
                    Counter.Add(new Item(String.Format("$COUNT_I[{0}]", i), string.Empty));
                    _progress.Value++;
                }
                for (var i = 1; i < 999; i++)
                {
                    Flags.Add(new Item(String.Format("$FLAG[{0}]", i), string.Empty));
                    _progress.Value++;
                }
                for (var i = 1; i < 256; i++)
                {
                    Flags.Add(new Item(String.Format("$CYCFLAG[{0}]", i), string.Empty));
                    _progress.Value++;
                }

                this._progress.IsVisible = false;
            }
        }

        private void GetFiles(string dir)
        {

            // Does AM.INI Exist?
            if (File.Exists(dir + "\\am.ini"))
                InfoFile = dir + "\\am.ini";

            if (File.Exists(dir + "\\C\\KRC\\Data\\kuka_con.mdb"))
                DataBaseFile = dir + "\\C\\KRC\\Data\\kuka_con.mdb"; ;
            if ((File.Exists(InfoFile)) && (File.Exists(DataBaseFile)))
                return;

                foreach (var d in Directory.GetDirectories(dir))
                {
                    foreach (var f in Directory.GetFiles(d))
                    {
                        var file = Path.GetFileName(f).ToLower();
                        Console.WriteLine(file);
                        switch(file)
                        {
                            case "am.ini":
                                InfoFile = f;
                                break;
                            case "kuka_con.mdb":
                                DataBaseFile = f;
                                break;
                        }
                    }
                 
                    this.GetFiles(d);
                }
            
        }

    }

    public class InfoFile:ViewModelBase
    {
        private string _archivename = string.Empty;
        private string _archiveconfigtype = string.Empty;
        private string _archiveDiskNo = string.Empty;
        private string _archiveID = string.Empty;
        private string _archiveDate = string.Empty;
        private string _archiveRobotName = string.Empty;
        private string _archiveRobotSerial = string.Empty;
        private string _archiveKssVersion = string.Empty;

        public string ArchiveName { get { return _archivename; } set { _archivename = value; RaisePropertyChanged("ArchiveName"); } }
        public string ArchiveConfigType { get { return _archiveconfigtype; } set { _archiveconfigtype = value; RaisePropertyChanged("ArchiveConfigType"); } }
        public string ArchiveDiskNo { get { return _archiveDiskNo; } set { _archiveDiskNo = value; RaisePropertyChanged("ArchiveDiskNo"); } }
        public string ArchiveID { get { return _archiveID; } set { _archiveID = value; RaisePropertyChanged("ArchiveID"); } }
        public string ArchiveDate { get { return _archiveDate; } set { _archiveDate = value; RaisePropertyChanged("ArchiveDate"); } }
        public string RobotName { get { return _archiveRobotName; } set { _archiveRobotName = value; RaisePropertyChanged("RobotName"); } }
        public string RobotSerial { get { return _archiveRobotSerial; } set { _archiveRobotSerial = value; RaisePropertyChanged("RobotSerial"); } }
        public string KSSVersion { get { return _archiveKssVersion; } set { _archiveKssVersion = value; RaisePropertyChanged("KSSVersion"); } }

        
        
        public ObservableCollection<Technology> Technologies { get; set; }

    }

   

    public class Technology
    {
        public string Name { get; set; }
        public string Version { get; set; }
    }

      public class GetFileSystemInfosConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is DirectoryInfo)
                {
                    return ((DirectoryInfo)value).GetFileSystemInfos();
                }
            }
            catch { }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
      public class GetFileIconConverter : IValueConverter
      {
          public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
          {
              try
              {
                  switch (Path.GetExtension(value.ToString().ToLower()))
                  {
                          //TODO This must not be hardcoded
                      case ".src":
                          var ico =  miRobotEditor.Classes.Utilities.LoadBitmap(@"C:\Programming\ miroboteditor --username Charles.Heath.Berman@gmail.com\trunk\src\Resources\srcfile.png");
                          return ico;
                      case ".dat":
                          return miRobotEditor.Classes.Utilities.LoadBitmap(@"C:\Programming\ miroboteditor --username Charles.Heath.Berman@gmail.com\trunk\src\Resources\datfile.png");
                      case ".sub":
                          return miRobotEditor.Classes.Utilities.LoadBitmap(@"C:\Programming\ miroboteditor --username Charles.Heath.Berman@gmail.com\trunk\src\Resources\spsfile.png");
                      default:
                          break;
                  }
                

              }
              catch { }
              return null;
          }

          public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
          {
              throw new NotImplementedException();
          }

      }
 

    public class Item : ViewModelBase
    {

        public Item(string type, string description)
        {
            Type = type;
            Description = description;
        }
        private string _type = string.Empty;
        public string Type { get { return _type; } set { _type = value; RaisePropertyChanged("Type"); } }

        private string _description = string.Empty;
        public string Description { get { return _description; } set { _description = value; RaisePropertyChanged("Description"); } }

        public override string ToString()
        {
            return String.Format("{0};{1}", Type, Description);
        }
    }
}
