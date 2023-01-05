using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FTPBrowser.Model;

namespace FTPBrowser.ViewModels
{
    public partial class FTPAccountViewModel:ObservableObject
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
		
        [RelayCommand(CanExecute=nameof(CanAddAccount))]
        private void AddAccount()=>Accounts.Add(new FTPAccount(Accounts.Count));

        private bool CanAddAccount()=>Accounts!=null;

        [RelayCommand(CanExecute=(nameof(CanRemoveAccount)))]
        private void RemoveAccount()=>Accounts.RemoveAt(CurrentIndex);
        private bool CanRemoveAccount() => SelectedItem!=null;


        private bool AccountsNotNull()=>Accounts!=null;

        [RelayCommand(CanExecute=nameof(AccountsNotNull))]
        private void CopyAccount(){

        }

/// <summary>
		/// Updates Current Item
		/// </summary>
          [RelayCommand(CanExecute=nameof(AccountsNotNull))]
        private void UpdateAccount(){
if (SelectedItem!=null)
				Accounts[CurrentIndex] = SelectedItem;
        }
		#region Commands
		 
	 
	 
	 
		#endregion
		
  
		
	 
		
		public FTPAccountViewModel()
		{			
			DeserializeAccounts();
			Instance=this;
		}
		
        [ObservableProperty]
        private bool isSelectedEqual;
		 
		
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