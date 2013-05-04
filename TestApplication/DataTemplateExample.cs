using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
namespace TestApplication
{
    public class DataTemplateExample:ViewModelBase
    {
        private CompletionItem _item = new CompletionItem();
        public CompletionItem Item { get { return _item; } set { _item = value; RaisePropertyChanged("Item"); } }

        public ObservableCollection<ICodeCompletion> Items { get; set; }

        public DataTemplateExample()
        {
            Items = new ObservableCollection<ICodeCompletion>();
            Items.Add(new CompletionItem());
        }
    }

    public class CompletionItem : ViewModelBase,ICodeCompletion
    {
        private string _description = "Description";
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value; RaisePropertyChanged("Description");
            }
        }
        private string _title = "Title";
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value; RaisePropertyChanged("Title");
            }
        }

        private System.Windows.Media.ImageSource _icon;
        public System.Windows.Media.ImageSource Icon
        {
            get
            {
                return _icon;
            }
            set
            {
                _icon = value;RaisePropertyChanged("Icon");
            }
        }
    }
}
