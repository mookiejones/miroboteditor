using System.Collections.ObjectModel;

namespace miRobotEditor.ViewModel
{
    public class FTPAccountWindowViewModel:ToolViewModel
    {
        #region Properties

        readonly ObservableCollection<FTPAccount> _accounts = new ObservableCollection<FTPAccount>();
        readonly ReadOnlyObservableCollection<FTPAccount> _readonlyAccounts = null;
        public ReadOnlyObservableCollection<FTPAccount> Accounts{get{return _readonlyAccounts?? new ReadOnlyObservableCollection<FTPAccount>(_accounts);}}
        #endregion

        public FTPAccountWindowViewModel():base("FTP Accounts")
        {
        }

        //Add Command

        //Remove Command

        //Connect Command

    }
}
