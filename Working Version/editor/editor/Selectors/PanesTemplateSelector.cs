using System.Windows;
using System.Windows.Controls;
using miRobotEditor.ViewModel;

namespace miRobotEditor.Selectors
{
    public sealed class PanesTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FunctionTemplate { private get; set; }
        public DataTemplate ObjectBrowserTemplate { private get; set; }
        public DataTemplate LocalVariablesTemplate { private get; set; }
        public DataTemplate NotesTemplate { private get; set; }
        public DataTemplate KukaTemplate { private get; set; }
        public DataTemplate DocumentTemplate { private get; set; }

        public DataTemplate AngleConverterTemplate { private get; set; }
        public DataTemplate MessageTemplate { private get; set; }

        public override DataTemplate SelectTemplate(object item,
            DependencyObject container)
        {
            if (item is ObjectBrowserViewModel)
            {
                return ObjectBrowserTemplate;
            }

            if (item is KukaViewModel)
            {
                return KukaTemplate;
            }

            if (item is NotesViewModel)
            {
                return NotesTemplate;
            }

            return item is FunctionViewModel
                ? FunctionTemplate
                : item is DocumentViewModel
                ? DocumentTemplate
                : item is MessageViewModel
                ? MessageTemplate
                : item is AngleConvertorViewModel ? AngleConverterTemplate : base.SelectTemplate(item, container);
        }
    }
}