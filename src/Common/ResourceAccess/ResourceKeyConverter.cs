using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows.Data;
namespace miRobotEditor.Common.ResourceAccess
{
    [Localizable(false)]
    public class ResourceKeyConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2)
            {
                return null;
            }
            var indexedResourceAccessor = values[0] as IIndexedResourceAccessor;
            var text = values[1] as string;
            if (indexedResourceAccessor == null || text == null)
            {
                return null;
            }
            var @string = indexedResourceAccessor.GetString(this, text, new object[0]);
            if (values.Length == 2)
            {
                return @string;
            }
            if (@string.Contains("{0}"))
            {
                var array = new object[values.Length - 2];
                Array.Copy(values, 2, array, 0, array.Length);
                return string.Format(CultureInfo.InvariantCulture, @string, array);
            }
            var stringBuilder = new StringBuilder(@string);
            for (var i = 2; i < values.Length; i++)
            {
                if (!string.IsNullOrEmpty(values[i] as string))
                {
                    stringBuilder.AppendLine();
                    stringBuilder.Append(indexedResourceAccessor.GetString(this, values[i].ToString(), new object[0]));
                }
            }
            return stringBuilder.ToString();
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
