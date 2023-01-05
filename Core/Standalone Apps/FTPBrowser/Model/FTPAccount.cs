using CommunityToolkit.Mvvm.ComponentModel;

namespace FTPBrowser.Model
{
    public partial class FTPAccount:ObservableObject
    {
        
        public FTPAccount(int number){
            CreateFTPAccount();
			AccountName = $"New Account {number}" + number.ToString();
        }


        public FTPAccount()=>CreateFTPAccount();

        private void CreateFTPAccount(){

        }

        private string _accountname = "New Account";
        private string _server = "10.20.5.197";
        private string _protocol = "FTP";
        private string _port = "21";
        private string _username = "administrator";
        private string _password = "kukarobxpe2";
        private bool _savepassword = true;
        private bool _nopassword = false;



        public string AccountName { get { return _accountname; } set { _accountname = value; OnPropertyChanged("AccountName"); } }
        public string Server { get { return _server; } set { _server = value; OnPropertyChanged("Server"); } }
        public string Protocol { get { return _protocol; } set { _protocol = value; OnPropertyChanged("Protocol"); } }
        public string Port { get { return _port; } set { _port = value; OnPropertyChanged("Port"); } }
        public string Username { get { return _username; } set { _username = value; OnPropertyChanged("Username"); } }
        public string Password { get { return _password; } set { _password = value; OnPropertyChanged("Password"); } }
        public bool SavePassword { get { return _savepassword; } set { _savepassword = value; OnPropertyChanged("SavePassword"); } }
        public bool NoPassword { get { return _nopassword; } set { _nopassword = value; OnPropertyChanged("NoPassword"); } }
    }
}