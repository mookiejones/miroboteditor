/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 11/16/2012
 * Time: 07:18
 * 
 */
using System;
using System.Data.OleDb;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using miRobotEditor.Commands;
using miRobotEditor.Classes;
using System.Windows.Input;
namespace miRobotEditor.Pads
{
	public delegate void StatusUpdateEventHandler(object sender, StatusEventArgs e);
	
	/// <summary>
	/// Description of ArchiveInfoHelper.
	/// </summary>
	public class ArchiveInfoHelper:ViewModelBase 
	{		
		
		private static ArchiveInfoHelper _instance;
		public static ArchiveInfoHelper Instance
		{
			get{ if (_instance==null) _instance= new ArchiveInfoHelper(); return _instance;}
			set{ _instance=value; }
		}
		
		public event StatusUpdateEventHandler UpdateStatus;
		
		#region Properties For Form
		private string _archivename = string.Empty;
		private string _archiveconfigtype = string.Empty;
		private string _archivediskno = string.Empty;
		private string _archiveid = string.Empty;
		private string _archivedate = string.Empty;
		private string _robotname = string.Empty;
		private string _robotserial = string.Empty;
		private string _kssversion = string.Empty;
		private ObservableCollection<TechPack> _techpacks = new ObservableCollection<ArchiveInfoHelper.TechPack>();
		public string ArchiveName{get{return _archivename;}set{_archivename=value;OnPropertyChanged("ArchiveName");}}
		public string ArchiveConfigType{get{return _archiveconfigtype;}set{_archiveconfigtype=value;OnPropertyChanged("ArchiveConfigType");}}
		public string ArchiveDiskNo{get{return _archivediskno;}set{_archivediskno=value;OnPropertyChanged("ArchiveDiskNo");}}
		public string ArchiveID{get{return _archiveid;}set{_archiveid=value;OnPropertyChanged("ArchiveID");}}
		public string ArchiveDate{get{return _archivedate;}set{_archivedate=value;OnPropertyChanged("ArchiveDate");}}
		
		private ObservableCollection<FileInfo> _filecollection = new ObservableCollection<FileInfo>();
		public ObservableCollection<FileInfo> FileCollection
		{
			get{return _filecollection;}set{_filecollection = value;OnPropertyChanged("FileCollection");}
		}
		
		private ObservableCollection<IOPoint> _digitalin = new ObservableCollection<IOPoint>();
		private ObservableCollection<IOPoint> _digitalout = new ObservableCollection<IOPoint>();
		private ObservableCollection<IOPoint> _analogin = new ObservableCollection<IOPoint>();
		private ObservableCollection<IOPoint> _analogout = new ObservableCollection<IOPoint>();
		
		public ObservableCollection<IOPoint> DigitalIn{get{return _digitalin;}set{_digitalin=value;OnPropertyChanged("DigitalIn");}}
		public ObservableCollection<IOPoint> DigitalOut{get{return _digitalout;}set{_digitalout=value;OnPropertyChanged("DigitalOut");}}
		public ObservableCollection<IOPoint> AnalogIn{get{return _analogin;}set{_analogin=value;OnPropertyChanged("AnalogIn");}}
		public ObservableCollection<IOPoint> AnalogOut{get{return _analogout;}set{_analogout=value;OnPropertyChanged("AnalogOut");}}

			
		
		private string filename = string.Empty;
			

		public string RobotName{get{return _robotname;}set{_robotname=value;OnPropertyChanged("RobotName");}}
		public string RobotSerial{get{return _robotserial;}set{_robotserial=value;OnPropertyChanged("RobotSerial");}}
		
		public string KSSVersion{get{return _kssversion;}set{_kssversion=value;OnPropertyChanged("KSSVersion");}}
		
		public ObservableCollection<TechPack> TechPacks{get{return _techpacks;}set{_techpacks=value;OnPropertyChanged("TechPacks");}}
		
		public class TechPack:ViewModelBase
		{
			private string _name = string.Empty;
			private string _version = string.Empty;
			public string Name{get{return _name;}set{_name=value;OnPropertyChanged("Name");}}
			public string Version{get{return _version;}set{_version = value;OnPropertyChanged("Version");}}			
		}
		#endregion
		
		
		#region Commands
		public static RelayCommand _loadCommand;
		public static ICommand LoadCommand
		{
			get
			{
				if (_loadCommand==null)
					_loadCommand=new Commands.RelayCommand(param=>Instance.Load(),param=>true);
				
				return _loadCommand as ICommand;
			}
		}
		#endregion
		
		public class IOPoint:ViewModelBase
		{
			private string _name = string.Empty;
			private string _number = string.Empty;
			public string Name {get{return _name;}set{_name=value;OnPropertyChanged("Name");}}
			public string Number{get{return _number;} set{_number = value;OnPropertyChanged("Number");}}
		}
		
		//TODO Eventually just make this to one variable that becomes filtered
	
			private string _name = string.Empty;
			private string _number = string.Empty;
			public string Name {get{return _name;}set{_name=value;OnPropertyChanged("Name");}}
			public string Number{get{return _number;} set{_number = value;OnPropertyChanged("Number");}}
			
			#region Observable Properties
			
		#endregion
		
		#region Members
		
		public string IOFilename {get;set;}
	     	
		 private const string cmdText = "SELECT Items.KeyString, Messages.[String] FROM (Items INNER JOIN Messages ON Items.Key_id = Messages.Key_id)WHERE (Items.[Module] = 'IO')";
         private const string strAccessConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=";
			
		 System.Data.DataSet myDataSet = new System.Data.DataSet();
		#endregion
		
		
		public void GetIO()
		{
                if (File.Exists(IOFilename))
                {
                    string connectionString = String.Format("{0}{1};", "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=", IOFilename);
                    using (OleDbConnection c = new System.Data.OleDb.OleDbConnection(connectionString))
                    {

                        c.Open();
                        using (OleDbCommand command = new OleDbCommand(cmdText, c))
                        {
                            using (OleDbDataReader reader = command.ExecuteReader())
                            {

                                string temp = string.Empty;
                                while (reader.Read())
                                {
                                    temp = reader.GetValue(0).ToString();
                                    if (String.IsNullOrEmpty(reader.GetValue(1).ToString().Trim()))
                                        continue;
	
                                    var newpoint = new IOPoint();
                                    string name = reader.GetValue(1).ToString();

                                    newpoint.Name=name;
                                    switch (temp.Substring(0, temp.IndexOf("_")))
                                    {
                                        case "IN":
                                    		DigitalIn.Add(new IOPoint{Number=temp.Substring(3),Name=name});
                                            break;
                                        case "OUT":
                                            DigitalOut.Add(new IOPoint{Number=temp.Substring(4),Name=name});
                                            break;
                                        case "ANIN":
                                            AnalogIn.Add(new IOPoint{Number=temp.Substring(5),Name=name});
                                            break;
                                        case "ANOUT":
                                            AnalogOut.Add(new IOPoint{Number=temp.Substring(6),Name=name});
                                            break;
                                        default:
                                            break;
                                    }
                                    
                                    

                                }
/*
                                if (dgANIN.Rows.Count == 1)
                                    tabControl1.TabPages.RemoveByKey("tpANIN");
                                if (dgANOUT.Rows.Count == 1)
                                    tabControl1.TabPages.RemoveByKey("tpANOUT");
                                if (dgInputs.Rows.Count == 1)
                                    tabControl1.TabPages.RemoveByKey("tpIN");
                                if (dgOutputs.Rows.Count == 1)
                                    tabControl1.TabPages.RemoveByKey("tpOUT");
                                    */

                            }
                        }
                    }

                }
		}
				
		
		void ResetSignals()
		{
			DigitalIn= new ObservableCollection<IOPoint>();
			DigitalOut = new ObservableCollection<IOPoint>();
			AnalogOut= new ObservableCollection<IOPoint>();
			AnalogIn = new ObservableCollection<IOPoint>();
		}
		
		
		public void Load()
		{
			Instance=this;			
			filename = GetZipFileName();
			OpenZip();
			LoadIO();
			GetPrograms();
			WriteArchiveInfo();	
		}
		
		void LoadIO()
		{
			string IOPath="\\C\\KRC\\DATA\\kuka_con.mdb";
			var root = filename.Substring(0,filename.LastIndexOf("."));			
			IOFilename= root + "\\" + IOPath;		
			ResetSignals();
			GetIO();
		}
		
		public void RaiseUpdateStatus(string message, int percentage)
		{
			if (UpdateStatus!=null)
				UpdateStatus(this, new StatusEventArgs{Message=message,Percentage=percentage});
		}
		
		
		public ArchiveInfoHelper()
		{
			Instance=this;								
		}
		
		
		public static string GetZipFileName()
		{
			var ofd = new OpenFileDialog();
			ofd.DefaultExt=".zip";
			ofd.Multiselect=false;
			ofd.Filter="Kuka Archive (*.zip)|*.zip|All Files(*.*)|(*.*)";
			
			Nullable<bool> result = ofd.ShowDialog();
			
			return ofd.FileName;
				
		}
	
		public void LoadArchive(){}
	
		private void GetPrograms()
		{
			RaiseUpdateStatus("Files",0);
			
			var root = filename.Substring(0,filename.LastIndexOf("."));
			
			// Data structure to hold names of subfolders to be examined for files
			
			var dirs = new Stack<string>(20);
			
			if (!Directory.Exists(root))
				return;
				
			
			dirs.Push(root);
			
			while (dirs.Count>0)
			{
				
				var currentDir = dirs.Pop();
				string[] subDirs;
				RaiseUpdateStatus(currentDir,0);
				
				try
				{
					subDirs = Directory.GetDirectories(currentDir);
				}
				 // An UnauthorizedAccessException exception will be thrown if we do not have
                // discovery permission on a folder or file. It may or may not be acceptable 
                // to ignore the exception and continue enumerating the remaining files and 
                // folders. It is also possible (but unlikely) that a DirectoryNotFound exception 
                // will be raised. This will happen if currentDir has been deleted by
                // another application or thread after our call to Directory.Exists. The 
                // choice of which exceptions to catch depends entirely on the specific task 
                // you are intending to perform and also on how much you know with certainty 
                // about the systems on which this code will run.
                catch(UnauthorizedAccessException e)
                {
                	Console.WriteLine(e.Message);
                    continue;
                }
                catch (DirectoryNotFoundException e)
                {
                	 Console.WriteLine(e.Message);
                     continue;
                }
                
                string[] files = null;
                try
                {
                	files = Directory.GetFiles(currentDir);
                }
                catch (UnauthorizedAccessException e)
                {
                	Console.WriteLine(e.Message);
                	continue;
                }
                catch (DirectoryNotFoundException e)
                {
                	Console.WriteLine(e.Message);
                	continue;
                }
                
                int i = 0;
                // Perform the required action on each file here
                foreach(string file in files)
                {
                	try
                	{
                		RaiseUpdateStatus(file,i);
                		
                		System.IO.FileInfo fi = new System.IO.FileInfo(file);
                		FileCollection.Add(new FileInfo(file));
                		Console.WriteLine("{0}: {1}, {2}",fi.Name,fi.Length,fi.CreationTime);
                		i++;
                	}
                	catch (FileNotFoundException e)
                	{
                		// If File was deleted by a separate application or thread then continue.
                		Console.WriteLine(e.Message);
                		continue;
                	}
                }
                
                // Push the SubDirectories onto the stack for traversal.
                // This could also be done before handling the files.
                foreach(string str in subDirs)
                	dirs.Push(str);
			}
			
			ClearStatus();
			
		}
		
		private void ClearStatus()
		{
			RaiseUpdateStatus(string.Empty,-1);
		}
		
		private void OpenZip()
		{
			if (File.Exists(filename))
			{
				var zip = new Ionic.Zip.ZipFile();
				zip.ExtractProgress +=Zip_ExtractProgress;
				zip.ExtractAll(filename,Ionic.Zip.ExtractExistingFileAction.OverwriteSilently);
			}
		}
		
		
		
		void Zip_ExtractProgress(object sender, Ionic.Zip.ExtractProgressEventArgs e)
		{
			
            
            
		}
		
		void WriteArchiveInfo()
		{
			var path = filename.Substring(0, filename.LastIndexOf("."));
            var iniFilePath = String.Format("{0}\\am.ini", path);
            
            var ini = new IniFile(iniFilePath);
            
            if (!System.IO.File.Exists(iniFilePath))
                return;
           
            ArchiveName= ini.ReadValue("Archive","Name");
            ArchiveConfigType= ini.ReadValue("Archive","Config");
            ArchiveDiskNo= ini.ReadValue("Archive","DiskNo");
            ArchiveID= ini.ReadValue("Archive","ID");
            ArchiveDate=ini.ReadValue("Archive","Date");
            RobotName= ini.ReadValue("Roboter","RobName");
            RobotSerial= ini.ReadValue("Roboter","IRSerialNr");
            KSSVersion= ini.ReadValue("Version","Version");
            
            var name = ini.ReadValue("TechPacks","TechPacks");
            
        	GetTechs(name,ini);
            
		}
		
		void GetTechs(string techstring,IniFile ini)
		{			
			TechPacks = new ObservableCollection<ArchiveInfoHelper.TechPack>();
			
			var sb = new System.Text.StringBuilder();
			string[] Split = techstring.Split('|');
			
			for (int i = 0;i<Split.Length;i++)
			{
				var entry = new TechPack();
				entry.Name = Split[i];
				entry.Version = ini.ReadValue("TechPacks",entry.Name);
				
				TechPacks.Add(entry);
			}			
		}
	
	
	
	internal class IniFile
	{
        private string filename = String.Empty;

        public IniFile()
        {
        }

        public IniFile(string filename)
        {
            this.filename = filename;
        }


		
		[DllImport("kernel32")]
		private static extern long WritePrivateProfileString(string section,string key,string val,string filePath);
		[DllImport("kernel32")]
		private static extern int GetPrivateProfileString(string section,string key,string def,StringBuilder retVal,int size,string filePath);
		
		/// <summary>
		/// Returns Length
		/// </summary>
		/// <param name="section">Section from INIFile</param>
		/// <param name="Path"></param>
		/// <returns></returns>
		public  int Length(int section,string Path)
		{
			StringBuilder field = new StringBuilder(255);
			StringBuilder Value = new StringBuilder(255);
			for (int i = 1; i <= 256; i++) 
			{
				int b = GetPrivateProfileString("Section" + section.ToString(),"FieldName" + i,"",field,255,Path);
				int c = GetPrivateProfileString("Section" + section.ToString(),"Value" + i,"",Value,255,Path);
				if ((! (field.ToString().Length > 0)) | (!(Value.ToString().Length > 0))){return i - 1;}
			}
			return 0;

		}

        public void WriteValue(string Section, string Key, string Value)
        {
            WriteValue(Section, Key, Value, filename);
        }


		/// <summary>
		/// Write Data to the INI File
		/// </summary>
		/// <param name="Section">Section name</param>
		/// <param name="Key">Key Name</param>
		/// <param name="Value">Value Name</param>
		/// <param name="Path"></param>
		public static void WriteValue(string Section,string Key,string Value, string Path)
		{
			WritePrivateProfileString(Section,Key,Value,Path);
		}

        public string ReadValue(string Section, string Key)
        {
            return ReadValue(Section, Key, filename);
        }
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="Section"></param>
		/// <param name="Key"></param>
		/// <param name="Path"></param>
		/// <returns></returns>
		public static string ReadValue(string Section,string Key, string Path)
		{
			StringBuilder temp = new StringBuilder(255);
			int i = GetPrivateProfileString(Section,Key,"",temp,255,Path);
			return temp.ToString();

		}
	}
	}
	public class StatusEventArgs:EventArgs
	{
		public int Percentage{get;set;}
		public string Message{get;set;}
	}
	
}
