using System;
using GalaSoft.MvvmLight;

namespace miRobotEditor.Options
{
    internal class OptionPanel : ViewModelBase, IOptionPanel
    {
        public virtual object Owner { get; set; }

        public virtual object Control
        {
            get { return this; }
        }


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