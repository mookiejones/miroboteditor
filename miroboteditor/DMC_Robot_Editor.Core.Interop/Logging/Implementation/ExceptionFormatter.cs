using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Reflection;
using System.Security;
using System.Security.Principal;
using System.Text;
namespace DMC_Robot_Editor.Globals.Logging.Implementation
{
    public class ExceptionFormatter
    {
        private const string LineSeparator = "--------------------------------------";
        private const string BlockSeparator = "======================================";
        public static string Header = Resources.ExceptionFormatterHeader;
        private readonly NameValueCollection _additionalInfo;
        private static readonly string nl = Environment.NewLine;
        public ExceptionFormatter()
            : this(new NameValueCollection())
        {
        }
        public ExceptionFormatter(NameValueCollection additionalInfo)
        {
            _additionalInfo = additionalInfo;
        }
        public string GetMessage(Exception exception)
        {
            var stringBuilder = new StringBuilder();
            CollectAdditionalInfo();
            string value = _additionalInfo.Get(Header);
            if (!string.IsNullOrEmpty(value))
            {
                stringBuilder.AppendFormat("{0}" + nl, _additionalInfo.Get(Header));
            }
            stringBuilder.AppendFormat("{0}:" + nl + "{1}", Resources.ExceptionSummary, LineSeparator);
            foreach (string text in _additionalInfo)
            {
                if (text != Header)
                    stringBuilder.AppendFormat(nl + "--> {0}", _additionalInfo.Get(text));
            }
            int num = 1;
            if (exception != null)
            {
                var ex = exception;
                do
                {
                    stringBuilder.AppendFormat(string.Concat(new string[]{nl, nl, "{0}",nl, "{1}" }), Resources.ExceptionDetails, LineSeparator);
                    stringBuilder.AppendFormat(nl + "{0}: {1}", Resources.ExceptionType, ex.GetType().FullName);
                    ReflectException(ex, stringBuilder);
                    if (ex.StackTrace != null)
                    {
                        stringBuilder.AppendFormat(string.Concat(new string[] {nl,nl,"{0}",nl,"{1}"}), Resources.ExceptionStackTraceDetails, LineSeparator);
                        stringBuilder.AppendFormat(nl + "{0}", ex.StackTrace);
                    }
                    ex = ex.InnerException;
                    num++;
                }
                while (ex != null);
            }
            string text2 = stringBuilder.ToString();

            return string.Concat(new string[] {nl,BlockSeparator,nl,"   ",text2.Replace(nl, nl + "   "),nl,BlockSeparator});
        }
        private static void ReflectException(Exception currException, StringBuilder strEventInfo)
        {
            PropertyInfo[] properties = currException.GetType().GetProperties();
            PropertyInfo[] array = properties;
            foreach (PropertyInfo propertyInfo in array)
            {
                if (propertyInfo.Name != "InnerException" && propertyInfo.Name != "StackTrace")
                {
                    object obj;
                    try
                    {
                        obj = propertyInfo.GetValue(currException, null);
                    }
                    catch (TargetInvocationException)
                    {
                        obj = Resources.PropertyAccessFailed;
                    }
                    if (obj == null)
                    {
                        strEventInfo.AppendFormat(nl + "{0}: NULL", propertyInfo.Name);
                    }
                    else
                    {
                        ProcessAdditionalInfo(propertyInfo, obj, strEventInfo);
                    }
                }
            }
        }
        private static void ProcessAdditionalInfo(PropertyInfo propinfo, object propValue, StringBuilder stringBuilder)
        {
            if (propinfo.Name == "AdditionalInformation")
            {
                if (propValue != null)
                {
                    var nameValueCollection = (NameValueCollection)propValue;
                    if (nameValueCollection.Count > 0)
                    {
                        stringBuilder.AppendFormat(nl + "AdditionalInformation:", new object[0]);
                        for (var i = 0; i < nameValueCollection.Count; i++)
                            stringBuilder.AppendFormat(nl + "{0}: {1}", nameValueCollection.GetKey(i),nameValueCollection[i]);
                    }
                }
            }
            else
            {
                stringBuilder.AppendFormat(nl + "{0}: {1}", propinfo.Name, propValue);
            }
        }
        private void CollectAdditionalInfo()
        {
            if (_additionalInfo["MachineName:"] != null)
            {
                return;
            }
            _additionalInfo.Add("MachineName:", "MachineName: " + GetMachineName());
            _additionalInfo.Add("TimeStamp:", "TimeStamp: " + DateTime.UtcNow.ToString(CultureInfo.CurrentCulture));
            _additionalInfo.Add("FullName:", "FullName: " + Assembly.GetExecutingAssembly().FullName);
            _additionalInfo.Add("AppDomainName:", "AppDomainName: " + AppDomain.CurrentDomain.FriendlyName);
            _additionalInfo.Add("WindowsIdentity:", "WindowsIdentity: " + GetWindowsIdentity());
        }
        private static string GetWindowsIdentity()
        {
            string result;
            try
            {
                result = WindowsIdentity.GetCurrent().Name;
            }
            catch (SecurityException)
            {
                result = "Permission Denied";
            }
            return result;
        }
        private static string GetMachineName()
        {
            string result;
            try
            {
                result = Environment.MachineName;
            }
            catch (SecurityException)
            {
                result = "Permission Denied";
            }
            return result;
        }
    }
}