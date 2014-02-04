using System.Windows;
using System.Windows.Controls;

namespace miRobotEditor.Classes
{
    public class MenuTemplateSelector : DataTemplateSelector
    {
        public DataTemplate KUKATemplate { get; set; }

        public DataTemplate FanucTemplate { get; set; }

        public DataTemplate NachiTemplate { get; set; }

        public DataTemplate ABBTemplate { get; set; }

        public DataTemplate KawasakiTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var element = container as FrameworkElement;
            if (WorkspaceViewModel.Instance.ActiveEditor.FileLanguage is Languages.KUKA)
                return KUKATemplate;
            if (WorkspaceViewModel.Instance.ActiveEditor.FileLanguage is Languages.ABB)
                return ABBTemplate;
            if (WorkspaceViewModel.Instance.ActiveEditor.FileLanguage is Languages.Fanuc)
                return FanucTemplate;
            if (WorkspaceViewModel.Instance.ActiveEditor.FileLanguage is Languages.Kawasaki)
                return KawasakiTemplate;
            return null;
        }
    }
}