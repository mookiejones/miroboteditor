using System.Windows.Controls;
using System.Windows;
using miRobotEditor.ViewModel;
namespace miRobotEditor.Classes
{
    class PanesStyleSelector : StyleSelector
    {
        public Style ToolStyle
        {
            get;
            set;
        }

        public Style FileStyle
        {
            get;
            set;
        }

        public Style MessageStyle { get; set; }
        public Style VariableStyle { get; set; }

        public override System.Windows.Style SelectStyle(object item, System.Windows.DependencyObject container)
        {
            if (item is MessageViewModel)
                return MessageStyle;

            if (item is IVariable)
                return VariableStyle;

            return base.SelectStyle(item, container);
        }
    }
}
