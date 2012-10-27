using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
namespace miRobotEditor.Classes.Messages
{
    public abstract class NotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void FirePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public virtual void FirePropertyChanged<TProperty>(Expression<Func<TProperty>> property)
        {
            FirePropertyChanged(GetMemberInfo(property).Name);
        }

        private static MemberInfo GetMemberInfo(LambdaExpression expression)
        {
            MemberExpression memberExpression;
            if (expression.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)expression.Body;
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else
            {
                memberExpression = (MemberExpression)expression.Body;
            }
            return memberExpression.Member;
        }
    }
}
