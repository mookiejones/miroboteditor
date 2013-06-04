using System.Windows;
using System.Windows.Controls;
using miRobotEditor.ViewModel;

namespace miRobotEditor
{
    class PanesTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FileViewTemplate
        {
            get;
            set;
        }

        public DataTemplate FileStatsViewTemplate
        {
            get;
            set;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is IDocument)
                return FileViewTemplate;
            //
            //   if (item is FileStatsViewModel)
            //       return FileStatsViewTemplate;

            return base.SelectTemplate(item, container);
        }
    }
}