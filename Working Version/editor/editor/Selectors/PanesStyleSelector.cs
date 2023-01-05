using System.Windows;
using System.Windows.Controls;
using miRobotEditor.Interfaces;
using miRobotEditor.ViewModel;

namespace miRobotEditor.Selectors
{
    public sealed class PanesStyleSelector : StyleSelector
    {
        public Style ToolStyle { private get; set; }
        public Style FileStyle { private get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            Style result = item is ToolViewModel ? ToolStyle : item is IEditorDocument ? FileStyle : base.SelectStyle(item, container);
            return result;
        }
    }
}