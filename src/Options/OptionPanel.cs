using System;
using miRobotEditor.ViewModel;
namespace miRobotEditor.Options
{
    class OptionPanel : ViewModelBase, IOptionPanel
    {
        public virtual object Owner { get; set; }
        public virtual object Control { get { return this; } }



        public void LoadOptions()
        {
            throw new NotImplementedException();
        }

        public bool SaveOptions()
        {
            throw new NotImplementedException();
        }
    }
}