using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CommonServiceLocator;
using miRobotEditor.Interfaces;
using miRobotEditor.ViewModel;

namespace miRobotEditor.Controls
{
    /// <summary>
    ///     Interaction logic for VisualVariableControl.xaml
    /// </summary>
    public sealed partial class VisualVariableControl : UserControl
    {
        public VisualVariableControl()
        {
            InitializeComponent();
        }

        private void ToolTip_Opening(object sender, ToolTipEventArgs e) => throw new NotImplementedException();

        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var child = (DependencyObject) e.OriginalSource;
            var dataGridRow = TryFindParent<DataGridRow>(child);
            if (dataGridRow != null)
            {
                var dataGrid = sender as DataGrid;
                if (dataGrid == null || dataGrid.CurrentCell.IsValid)
                {
                    if (dataGrid != null)
                    {
                        var variable = dataGrid.CurrentCell.Item as IVariable;
                        var instance = ServiceLocator.Current.GetInstance<MainViewModel>();
                        if (variable != null && File.Exists(variable.Path))
                        {
                            instance.OpenFile(variable);
                        }
                    }
                    e.Handled = true;
                }
            }
        }

        public T TryFindParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parentObject = GetParentObject(child);
            T result;
            if (parentObject == null)
            {
                result = default(T);
            }
            else
            {
                var parent = parentObject as T;
                T resultObject;

                if ((resultObject = parent) == null)
                {
                    resultObject = TryFindParent<T>(parentObject);
                }
                result = resultObject;
            }
            return result;
        }

        public DependencyObject GetParentObject(DependencyObject child)
        {
            DependencyObject result;
            if (child == null)
            {
                result = null;
            }
            else
            {
                var contentElement = child as ContentElement;
                if (contentElement != null)
                {
                    var parent = ContentOperations.GetParent(contentElement);
                    if (parent != null)
                    {
                        result = parent;
                    }
                    else
                    {
                        var frameworkContentElement = contentElement as FrameworkContentElement;
                        result = ((frameworkContentElement != null) ? frameworkContentElement.Parent : null);
                    }
                }
                else
                {
                    var frameworkElement = child as FrameworkElement;
                    if (frameworkElement != null)
                    {
                        var parent = frameworkElement.Parent;
                        if (parent != null)
                        {
                            result = parent;
                            return result;
                        }
                    }
                    result = VisualTreeHelper.GetParent(child);
                }
            }
            return result;
        }
    }
}