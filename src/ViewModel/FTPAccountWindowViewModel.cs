using System.Collections.ObjectModel;
using miRobotEditor.Classes;
using miRobotEditor.Core;

namespace miRobotEditor.ViewModel
{
    public class FTPAccountWindowViewModel : ToolViewModel
    {
        #region Properties

        private readonly ObservableCollection<FTPAccount> _accounts = new ObservableCollection<FTPAccount>();
        private readonly ReadOnlyObservableCollection<FTPAccount> _readonlyAccounts = null;

        public ReadOnlyObservableCollection<FTPAccount> Accounts => _readonlyAccounts ?? new ReadOnlyObservableCollection<FTPAccount>(_accounts);

        #endregion

        public FTPAccountWindowViewModel() : base("FTP Accounts")
        {
        }

        //Add Command

        //Remove Command

        //Connect Command
    }
}