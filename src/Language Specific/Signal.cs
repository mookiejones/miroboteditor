namespace miRobotEditor.Language_Specific
{
    public class Signal : ViewModel.ViewModelBase
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
