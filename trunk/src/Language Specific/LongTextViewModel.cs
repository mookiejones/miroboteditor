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
namespace miRobotEditor.Language_Specific
{
    class LongTextViewModel:ViewModelBase
    {

        #region Commands

        private ProgressBarViewModel _progress = new ProgressBarViewModel();

        private string _languageText = String.Empty;
        public string LanguageText { get { return _languageText; } set { _languageText = value; RaisePropertyChanged("LanguageText"); } }


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

        private RelayCommand _getLanguageTextCommand;
        public ICommand GetLanguageTextCommand
        {
            get { return _getLanguageTextCommand ?? (_getLanguageTextCommand = new RelayCommand(param => this.GetAllLangtextFromDatabase(), param => true)); }
        }

        public ObservableCollection<Signal> Inputs { get; set; }
        public ObservableCollection<Signal> Outputs { get; set; }
        public ObservableCollection<Signal> AnIn { get; set; }
        public ObservableCollection<Signal> AnOut { get; set; }
        public ObservableCollection<Signal> Timer { get; set; }
        public ObservableCollection<Signal> Flags { get; set; }
        public ObservableCollection<Signal> CycFlags { get; set; }
        public ObservableCollection<Signal> Counter { get; set; }
        #endregion
        string _dbFile = string.Empty;
        private bool isKRC2 =true;


        private string _database = string.Empty;
        public string DataBase { get { return _database; } set { _database = value; RaisePropertyChanged("DataBase"); } }



        void Load()
        {
         //  language.SetLanguageFormLongtext(language.sLangAct);
         // 
         //
         //  DataBase = MyProject.Forms.FormMain.LangtextGetPath();
         //  this.tInitGrids.Tick += delegate(object a0, EventArgs a1)
         //  {
         //      this.InitGrids();
         //  };
         //  this.tInitGrids.Interval = 300;
         //  this.tInitGrids.Start();
         //  this.tstSearch.Text = language.s("Search", "", "", "", "");
        }

        private void GetAllLangtextFromDatabase()
        {

            var ofd = new OpenFileDialog { Title = "Select Database", Filter = "Kuka connection(kuka_con.mdb(kuka_con.mdb)",Multiselect=false };
            ofd.ShowDialog();
            LanguageText = string.Empty;

            string path = ofd.FileName;
            if (File.Exists(path))
            {
                string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";";
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
                                LanguageText+= string.Format("{0}{1}\r\n", oleDbDataReader.GetValue(0).ToString(), oleDbDataReader.GetValue(1).ToString());
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

            Application.DoEvents();
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

        private void Import()
        {
            var openFileDialog = new OpenFileDialog();
            var openFileDialog2 = openFileDialog;

            openFileDialog2.Filter = "Text file (*.txt)|*.txt|csv file (*.csv)|*.csv";
            openFileDialog2.Multiselect = false;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (Path.GetExtension(openFileDialog.FileName).ToLower() == ".csv")
                    ImportFile(openFileDialog.FileName, true);
                else
                    ImportFile(openFileDialog.FileName, false);
            }
        }

        string[] LangText;

        private void Export(string filename,bool iscsv)
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
                            text = string.Format("{0}IN{1}{2}{3}{4}\r\n",pre,post,i,text2,LangText[i]);

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
                            text = string.Format("{0}{1}{2}{3}{4}\r\n",isKRC2?"TimerText":"$Timer[", post, i, text2, LangText[i]);

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

            this.Inputs = new ObservableCollection<Signal>();
            this.Outputs = new ObservableCollection<Signal>();
            this.AnIn = new ObservableCollection<Signal>();
            this.AnOut = new ObservableCollection<Signal>();
            this.Timer = new ObservableCollection<Signal>();
            this.Counter = new ObservableCollection<Signal>();
            this.Flags = new ObservableCollection<Signal>();
            this.CycFlags = new ObservableCollection<Signal>();

//            MyProject.Forms.FormMain.LangtextFillBuffer(this.sFileDb);
            int num = 1;
            checked
            {
                for (int i = 1;i<4096;i++)
                {
                    Inputs.Add(new Signal(String.Format("$IN[{0}]", i), string.Empty));
                    Outputs.Add(new Signal(String.Format("$OUT[{0}]", i), string.Empty));

                    _progress.Value++;

                    num++;
                }

                for (var i = 1; i < 32; i++)
                {
                    AnIn.Add(new Signal(String.Format("$ANIN[{0}]",i),string.Empty));
                    AnOut.Add(new Signal(String.Format("$ANOUT[{0}]",i),string.Empty));
                    _progress.Value++;
                }

                for (var i = 1; i < 20; i++)
                {
                    Timer.Add(new Signal(String.Format("$TIMER[{0}]",i),string.Empty));
                    Counter.Add(new Signal(String.Format("$COUNT_I[{0}]",i),string.Empty));
                    _progress.Value++;
                }
                for (var i = 1; i < 999; i++)
                {
                    Flags.Add(new Signal(String.Format("$FLAG[{0}]", i), string.Empty));                  
                    _progress.Value++;
                }
                for (var i = 1; i < 256; i++)
                {
                    Flags.Add(new Signal(String.Format("$CYCFKAG[{0}]", i), string.Empty));
                    _progress.Value++;
                }
               
                this._progress.IsVisible = false;
            }
        }
    }
    public class Signal : ViewModelBase
    {

        public Signal(string type, string description)
        {
            Type = type;
            Description = description;
        }
        private string _type = string.Empty;
        public string Type { get { return _type; } set { _type = value; RaisePropertyChanged("Type"); } }

        private string _description = string.Empty;
        public string Description { get { return _description; } set { _description = value; RaisePropertyChanged("Description"); } }
    }
}
