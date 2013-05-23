using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// <summary>
    /// Provides access to objects containing OptionBindings, such as OptionPanels.
    /// </summary>
    public interface IOptionPanel
    {
        /// <summary>
        /// Gets/sets the owner (the context object used when building the option panels
        /// from the addin-tree). This is null for IDE options or the IProject instance for project options.
        /// </summary>
        object Owner { get; set; }

        object Control
        {
            get;
        }

        void LoadOptions();
        bool SaveOptions();
    }
}