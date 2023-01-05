using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;

namespace miRobotEditor.ViewModel
{
    public abstract class ViewModelBase : ObservableRecipient
    {

        private static bool? _isInDesignMode;

        /// <summary>
        /// Gets a value indicating whether the control is in design mode
        /// (running under Blend or Visual Studio).
        /// </summary>
        public bool IsInDesignMode => IsInDesignModeStatic;

        /// <summary>
        /// Gets a value indicating whether the control is in design mode
        /// (running in Blend or Visual Studio).
        /// </summary>
        public static bool IsInDesignModeStatic
        {
            get
            {

                if (!_isInDesignMode.HasValue)
                {
                    DependencyProperty prop = DesignerProperties.IsInDesignModeProperty;
                    _isInDesignMode = (bool)DependencyPropertyDescriptor.FromProperty(prop, typeof(FrameworkElement)).Metadata.DefaultValue;
                }
                return _isInDesignMode.Value;
            }
        }

        /// <summary>
        /// Tell bound controls (via WPF binding) to refresh their display.
        /// 
        /// Sample call: this.NotifyPropertyChanged(() => this.IsSelected);
        /// where 'this' is derived from <seealso cref="BaseViewModel"/>
        /// and IsSelected is a property.
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="property"></param>
        public void NotifyPropertyChanged<TProperty>(Expression<Func<TProperty>> property)
        {
            LambdaExpression lambda = property;
            MemberExpression memberExpression = lambda.Body is UnaryExpression unaryExpression ? (MemberExpression)unaryExpression.Operand : (MemberExpression)lambda.Body;
            OnPropertyChanged(memberExpression.Member.Name);
        }
    }
}
