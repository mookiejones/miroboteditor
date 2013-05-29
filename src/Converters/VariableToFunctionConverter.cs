using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using miRobotEditor.Classes;
namespace miRobotEditor.Converters
{
    public class VariableToFunctionConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ReadOnlyObservableCollection<IVariable>)
            {
                var prev = value as ReadOnlyObservableCollection<IVariable>;

                
              // If Count is zero, hide the function window
                if (prev.Count == 0)
                {
                    return new ObservableCollection<IVariable>();
                }
                var list = new ObservableCollection<IVariable>();
               

                foreach (var i in prev.Where(i => i.Type == "def"))
                    list.Add(i);

                var result = new ReadOnlyObservableCollection<IVariable>(list);


                return result;
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
