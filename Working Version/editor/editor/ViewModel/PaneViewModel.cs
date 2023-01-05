using System.Windows.Media;


namespace miRobotEditor.ViewModel
{
    public class PaneViewModel : ViewModelBase
    {
        private string _contentId;
        private readonly bool _isActive;
        private bool _isSelected;
        private string _title;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);

        }

        public ImageSource IconSource { get; set; }

        public string ContentId
        {
            get => _contentId;
            set
            => SetProperty(ref _contentId, value);
        }

        public bool IsSelected
        {
            get => _isSelected;

            set => SetProperty(ref _isSelected, value);
        }

    }
}