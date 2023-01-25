using System;
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

            var contentPresenter = item as ContentPresenter;
            switch (contentPresenter.Content)
            {
                case ObjectBrowserViewModel _:
                    return ObjectBrowserTemplate;
                case KukaViewModel _:
                    return KukaTemplate;
                case NotesViewModel _:
                    return NotesTemplate;
                case FunctionViewModel _:
                    return FunctionTemplate;
                case DocumentViewModel _:
                    return DocumentTemplate;
                case MessageViewModel _:
                    return MessageTemplate;
                case AngleConvertorViewModel _:
                    return AngleConverterTemplate;
                default:
                    Console.WriteLine();
                    break;
                    //return item is FunctionViewModel
                    //    ? FunctionTemplate
                    //    : item is DocumentViewModel
                    //    ? DocumentTemplate
                    //    : item is MessageViewModel
                    //    ? MessageTemplate
                    //    : item is AngleConvertorViewModel ? AngleConverterTemplate : base.SelectTemplate(item, container);
            }
            return base.SelectTemplate(item, container);
        }
    }
}