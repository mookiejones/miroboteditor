/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 11/15/2012
 * Time: 12:38
 * 
 */
using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Input;
namespace FTPBrowser
{
	/// <summary>
	/// Description of FTPAccountViewModel.
	/// </summary>
	public class FTPAccountViewModel:ViewModelBase,IDisposable
	{
		#region Members
		/// <summary>
		/// Filename to save to
		/// </summary>
		private const string FTPACCOUNTSFILENAME ="ftpaccounts.xml";
		private static FTPAccountViewModel _instance;
		public static FTPAccountViewModel Instance{get{return _instance;}set{_instance=value;}}
		
		private ObservableCollection<FTPAccount>_accounts;
		public ObservableCollection<FTPAccount> Accounts
		{
			get{return _accounts;}
			set{_accounts=value;
				OnPropertyChanged("Accounts");}
		}
		
		

		private int _currentindex = 0;
		
		public int CurrentIndex
		{
			get{return _currentindex;}
			set
			{
				_currentindex=value;
				OnPropertyChanged("CurrentIndex");
			}
		}
		private FTPAccount _selecteditem ;
		
		public FTPAccount SelectedItem
		{
			get
			{
				if (_selecteditem==null)
					_selecteditem=Accounts[CurrentIndex];
				return _selecteditem;
			}
			set
			{
				_selecteditem=value;
				if (SelectedItem!=null)
				IsSelectedEqual=SelectedItem == Accounts[CurrentIndex];
				OnPropertyChanged("SelectedItem");
			}
		}
		#endregion
		
		#region Commands
		RelayCommand _addCommand;
		public ICommand AddCommand
		{
			get{
					if (_addCommand==null)
					{
						_addCommand = new RelayCommand(param=>this.AddAccount(),param=> Accounts!=null);
					}
					return _addCommand;
				}
		}
		RelayCommand _removeCommand;
		public ICommand RemoveCommand
		{
			get{
					if (_removeCommand==null)
					{
						_removeCommand = new RelayCommand(param=>this.RemoveAccount(),param=> SelectedItem!=null);
					}
					return _removeCommand;
				}
		}
		RelayCommand _copyCommand;
		public ICommand CopyCommand
		{
			get{
					if (_copyCommand==null)
					{
						_copyCommand = new RelayCommand(param=>this.CopyAccount(),param=> Accounts!=null);
					}
					return _copyCommand;
				}
		}
		RelayCommand _updateCommand;
		public ICommand UpdateCommand
		{
			get{
					if (_updateCommand==null)
					{
						_updateCommand = new RelayCommand(param=>this.UpdateAccount(),param=> Accounts!=null);
					}
					return _updateCommand;
				}
		}
		#endregion
		
		public void RemoveAccount()
		{
//			Accounts.Remove(SelectedItem);
			Accounts.RemoveAt(CurrentIndex);
	//		SelectedItem=Accounts[CurrentIndex];
		}
		public void CopyAccount()
		{
			
		}
		public void AddAccount()
		{
			Accounts.Add(new FTPAccount(Accounts.Count));
		}
		
		/// <summary>
		/// Updates Current Item
		/// </summary>
		public void UpdateAccount()
		{
			if (SelectedItem!=null)
				Accounts[CurrentIndex] = SelectedItem;
		}
		
		public FTPAccountViewModel()
		{			
			DeserializeAccounts();
			Instance=this;
		}
		
		private bool _isselectedEqual;
		public bool IsSelectedEqual
		{
			get{return _isselectedEqual;}
			set
			{
				_isselectedEqual=value;
				OnPropertyChanged("IsSelectedEqual");
			}
		}
		
		//XML Serialization
		public void SerializeAccounts()
		{
			var serial = new XmlSerializer(typeof(ObservableCollection<FTPAccount>));
			using (var writer = new StreamWriter(FTPACCOUNTSFILENAME))
				serial.Serialize(writer,Accounts);
		}
		
		void DeserializeAccounts()
		{
			if (!File.Exists(FTPACCOUNTSFILENAME)) return;
			
			
			// Construct an instance of the XmlSerializer with the type
			// of object that is being deserialized.
			XmlSerializer serial = new XmlSerializer(typeof(ObservableCollection<FTPAccount>));
			
				//To Read the Stream, Create a FileStream
				using (FileStream fs = new FileStream(FTPACCOUNTSFILENAME,FileMode.Open))
				{
				
					// Call the Deserialize method and cast to the object type.
					Accounts = (ObservableCollection<FTPAccount>) serial.Deserialize(fs);
				}
			
		}
		
		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}
