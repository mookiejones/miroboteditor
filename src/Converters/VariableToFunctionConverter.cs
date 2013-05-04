using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Data;
using miRobotEditor.Classes;
namespace miRobotEditor.Converters
{
    public class VariableToFunctionConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ObservableCollection<IVariable>)
            {
                var prev = value as ObservableCollection<IVariable>;
                if (prev.Count == 0) return new ObservableCollection<IVariable>();
                var list = new ObservableCollection<IVariable>();

                foreach (var i in prev)
                {
                    if (i.Type == "def")
                        list.Add(i);
                }
                return list;
            }


            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class FunctionVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
           


            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
