using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.Reflection;
using System.Security;
using System.Security.Principal;
using System.Threading;
namespace DMC_Robot_Editor.Exceptionhandling
{
    using Logging;
   public abstract class ExceptionFormatter
	{
		private static readonly ArrayList IgnoredProperties = new ArrayList(new string[]
		{
			"Source",
			"Message",
			"HelpLink",
			"InnerException",
			"StackTrace"
		});
		private NameValueCollection additionalInfo;
       public Exception Exception { get; private set; }
		public NameValueCollection AdditionalInfo
		{
			get
			{
				if (additionalInfo == null)
				{
					additionalInfo = new NameValueCollection
					{

						{
							"MachineName",GetMachineName()
						},

						{
							"TimeStamp",
							DateTime.UtcNow.ToString(CultureInfo.CurrentCulture)
						},

						{
							"FullName",
							Assembly.GetExecutingAssembly().FullName
						},

						{
							"AppDomainName",
							AppDomain.CurrentDomain.FriendlyName
						},

						{
							"ThreadIdentity",
							Thread.CurrentPrincipal.Identity.Name
						},

						{
							"WindowsIdentity",GetWindowsIdentity()
						}
					};
				}
				return additionalInfo;
			}
		}
		protected ExceptionFormatter(Exception exception)
		{
			if (exception == null)
			{
				throw new ArgumentNullException("exception");
			}

		    this.Exception = exception;
		}
		public virtual void Format()
		{
			WriteDescription();
			WriteDateTime(DateTime.UtcNow);
			WriteException(Exception, null);
		}
		protected virtual void WriteException(Exception exceptionToFormat, Exception outerException)
		{
			if (exceptionToFormat == null)
			{
				throw new ArgumentNullException("exceptionToFormat");
			}
			WriteExceptionType(exceptionToFormat.GetType());
			WriteMessage(exceptionToFormat.Message);
			WriteSource(exceptionToFormat.Source);
			WriteHelpLink(exceptionToFormat.HelpLink);
			WriteReflectionInfo(exceptionToFormat);
			WriteStackTrace(exceptionToFormat.StackTrace);
			if (outerException == null)
			{
				WriteAdditionalInfo(AdditionalInfo);
			}
			var innerException = exceptionToFormat.InnerException;
			if (innerException != null)
			{
				WriteException(innerException, exceptionToFormat);
			}
		}
		protected void WriteReflectionInfo(Exception exceptionToFormat)
		{
			if (exceptionToFormat == null)
			{
				throw new ArgumentNullException("exceptionToFormat");
			}
			Type type = exceptionToFormat.GetType();
			PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
			PropertyInfo[] array = properties;
			for (var i = 0; i < array.Length; i++)
			{
				PropertyInfo propertyInfo = array[i];
				if (propertyInfo.CanRead && IgnoredProperties.IndexOf(propertyInfo.Name) == -1)
				{
					object value;
					try
					{
						value = propertyInfo.GetValue(exceptionToFormat, null);
					}
					catch (TargetInvocationException)
					{
						value = Resources.PropertyAccessFailed;
					}
					WritePropertyInfo(propertyInfo, value);
				}
			}
			FieldInfo[] array2 = fields;
			for (var j = 0; j < array2.Length; j++)
			{
				FieldInfo fieldInfo = array2[j];
				object value;
				try
				{
					value = fieldInfo.GetValue(exceptionToFormat);
				}
				catch (TargetInvocationException)
				{
					value = Properties.FieldAccessFailed;
				}
				WriteFieldInfo(fieldInfo, value);
			}
		}
		protected abstract void WriteDescription();
		protected abstract void WriteDateTime(DateTime utcNow);
		protected abstract void WriteExceptionType(Type exceptionType);
		protected abstract void WriteMessage(string message);
		protected abstract void WriteSource(string source);
		protected abstract void WriteHelpLink(string helpLink);
		protected abstract void WriteStackTrace(string stackTrace);
		protected abstract void WritePropertyInfo(PropertyInfo propertyInfo, object value);
		protected abstract void WriteFieldInfo(FieldInfo fieldInfo, object value);
		protected abstract void WriteAdditionalInfo(NameValueCollection additionalInformation);
		private static string GetMachineName()
		{
			string result;
			try
			{
				result = Environment.MachineName;
			}
			catch (SecurityException)
			{
				result = Properties.PermissionDenied;
			}
			return result;
		}
		private static string GetWindowsIdentity()
		{
			string result = string.Empty;
			try
			{
			    var windowsIdentity = WindowsIdentity.GetCurrent();
			    if (windowsIdentity != null) result = windowsIdentity.Name;
			}
			catch (SecurityException)
			{
                result = Properties.PermissionDenied;
			}
			return result;
		}
	}
}
