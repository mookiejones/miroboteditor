using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace KUKATools.TemplateSelectors
{
    public class AuthorizationTemplateSelector:DataTemplateSelector
    {
        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            return base.SelectTemplate(item, container);
        }
    }
}
