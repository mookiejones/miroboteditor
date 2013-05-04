using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
namespace miRobotEditor.Templates
{
    public class VariableSelector : DataTemplateSelector
    {
        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            return base.SelectTemplate(item, container);
        }
    }
    
}
