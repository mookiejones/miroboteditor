using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace miRobotEditor.ViewModel
{
    public class FTPAccountWindowViewModel:ToolViewModel
    {
        #region Properties
        ObservableCollection<FTPAccount> _accounts = new ObservableCollection<FTPAccount>();
        ReadOnlyObservableCollection<FTPAccount> _readonlyAccounts = null;
        public ReadOnlyObservableCollection<FTPAccount> Accounts{get{return _readonlyAccounts?? new ReadOnlyObservableCollection<FTPAccount>(_accounts);}}
        #endregion

        public FTPAccountWindowViewModel():base("FTP Accounts")
        {
        }

        //Add Command

        //Remove Command

        //Connect Command

    }

    public class FTPAccount
    {
    }
}
