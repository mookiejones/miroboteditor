using miRobotEditor.Classes;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;

namespace miRobotEditor.Converters
{
    public class VariableToFunctionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var val = value as ReadOnlyObservableCollection<IVariable>;

            if (val.Count > 0 && val != null)
            {
                var list = new ObservableCollection<IVariable>();

                foreach (var i in val.Where(i => String.Equals(i.Type, "def", StringComparison.OrdinalIgnoreCase)))
                    list.Add(i);

                return new ReadOnlyObservableCollection<IVariable>(list);
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}