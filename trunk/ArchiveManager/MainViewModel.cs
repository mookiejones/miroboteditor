using System.Collections.ObjectModel;
namespace ArchiveManager
{
    class MainViewModel:ViewModelBase
    {


        public ObservableCollection<ArchiveItem> Archives { get; set; }
        private ArchiveItem _selectedItem = new ArchiveItem();
        public ArchiveItem SelectedItem { get { return _selectedItem; } set { _selectedItem = value; RaisePropertyChanged("SelectedItem"); } }
        public MainViewModel()
        {
            Archives = new ObservableCollection<ArchiveItem>();

            Archives.Add(new ArchiveItem{Description="Description", Directory="C:\\", Title ="Title"});
            
        }


    }
}
