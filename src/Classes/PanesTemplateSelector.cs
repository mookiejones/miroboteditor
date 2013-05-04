using AvalonDock.Layout;
using System.Windows;
using System.Windows.Controls;
using miRobotEditor.ViewModel;
namespace miRobotEditor.Classes
{
    public  class PanesTemplateSelector : DataTemplateSelector
    {
        public PanesTemplateSelector()
        {
        
        }


        public DataTemplate MessageViewTemplate
        {
            get;
            set;
        }

        public DataTemplate FunctionViewTemplate
        {
            get;
            set;
        }

        public DataTemplate ObjectBrowserTemplate
        {
            get;
            set;
        }
        public DataTemplate ProjectExplorerTemplate
        {
            get;
            set;
        }


        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            var itemAsLayoutContent = item as LayoutContent;

            if (item is MessageViewModel)
                return MessageViewTemplate;

            if (item is IVariable)
                return FunctionViewTemplate;

            if (item is ObjectBrowserViewModel)
                 return ObjectBrowserTemplate;
        //    if (item is GUI.ProjectExplorerTemplate)
         //       return ProjectExplorerTemplate;
            return base.SelectTemplate(item, container);
        }
    }
}

