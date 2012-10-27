using System;
using System.Collections;
using System.Reflection;
using System.Text;
namespace DMC_Robot_Editor.Globals.Logging.Formatter
{
    public class XmlLogFormatter : LogFormatter
    {
        public override string Format(LogEntry log)
        {
            var stringBuilder = new StringBuilder();
            Format(log, stringBuilder);
            return stringBuilder.ToString();
        }
        private void Format(object obj, StringBuilder result)
        {
            if (obj == null)
            {
                return;
            }
            result.Append(CreateOpenElement(CrateRootName(obj)));
            if (Type.GetTypeCode(obj.GetType()) == TypeCode.Object)
            {
                PropertyInfo[] properties = obj.GetType().GetProperties();
                int i = 0;
                while (i < properties.Length)
                {
                    PropertyInfo propertyInfo = properties[i];
                    result.Append(CreateOpenElement(propertyInfo.Name));
                    if (!typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType) || Type.GetTypeCode(propertyInfo.PropertyType) != TypeCode.Object)
                    {
                        goto IL_C5;
                    }
                    var enumerable = (IEnumerable)propertyInfo.GetValue(obj, null);
                    if (enumerable != null)
                    {
                        IEnumerator enumerator = enumerable.GetEnumerator();
                        try
                        {
                            while (enumerator.MoveNext())
                            {
                                object current = enumerator.Current;
                                Format(current, result);
                            }
                            goto IL_D4;
                        }
                        finally
                        {
                            var disposable = enumerator as IDisposable;
                            if (disposable != null)
                            {
                                disposable.Dispose();
                            }
                        }
                    }
                IL_D4:
                    result.Append(CreateCloseElement(propertyInfo.Name));
                    i++;
                    continue;
                IL_C5:
                    result.Append(ConvertToString(propertyInfo, obj));
                    goto IL_D4;
                }
            }
            else
            {
                result.Append(obj.ToString());
            }
            result.Append(CreateCloseElement(CrateRootName(obj)));
        }
        private string CrateRootName(object obj)
        {
            return obj.GetType().Name;
        }
        private string CreateOpenElement(string name)
        {
            return string.Format(Resources.Culture, "<{0}>", new object[]
			{
				name
			});
        }
        private string CreateCloseElement(string name)
        {
            return string.Format(Resources.Culture, "</{0}>", new object[]
			{
				name
			});
        }
        private string ConvertToString(PropertyInfo propertyInfo, object obj)
        {
            object value = propertyInfo.GetValue(obj, null);
            if (value == null)
            {
                return "";
            }
            return value.ToString();
        }
    }
}
