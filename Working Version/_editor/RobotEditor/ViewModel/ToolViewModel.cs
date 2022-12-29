using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotEditor.ViewModel
{
    public class ToolViewModel : PaneViewModel
    {
        public DefaultToolPane DefaultPane = DefaultToolPane.None;
        private bool _isVisible = true;

        protected ToolViewModel(string name)
        {
            Name = name;
            Title = name;
        }

        public int Height { get; set; }
        public int Width { get; set; }
        public string Name { get; private set; }

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    RaisePropertyChanged("IsVisible");
                }
            }
        }
    }
    public enum DefaultToolPane
    {
        Left,
        Right,
        Bottom,
        None
    }
}
