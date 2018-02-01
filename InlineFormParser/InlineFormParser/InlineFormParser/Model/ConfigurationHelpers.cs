using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Xml.Serialization;

namespace InlineFormParser.Model
{
	public static class ConfigurationHelpers
	{
		private static string[] nameProperties = new string[8]
		{
			"Name",
			"SystemName",
			"OriginalName",
			"DisplayName",
			"ID",
			"Id",
			"Key",
			"ViewSystemName"
		};

		public static void CheckAccessProperty(object config, string propertyName)
		{
			CheckArguments(config, propertyName);
			var propertyInfo = GetPropertyInfo(config, propertyName);
			GetPropertyValue(config, propertyInfo);
		}

		public static void CheckPropertyNotNull(object config, string propertyName)
		{
			CheckArguments(config, propertyName);
			var propertyInfo = GetPropertyInfo(config, propertyName);
			if (GetPropertyValue(config, propertyInfo) == null)
			{
				InternalThrow(config, propertyInfo, "Value is missing.");
			}
		}

		public static void CheckStringProperty(object config, string propertyName)
		{
			CheckArguments(config, propertyName);
			var propertyInfo = GetPropertyInfo(config, propertyName);
			CheckPropertyType(propertyInfo, typeof(string));
			var value = GetPropertyValue(config, propertyInfo) as string;
			if (string.IsNullOrEmpty(value))
			{
				InternalThrow(config, propertyInfo, "Value is missing or empty.");
			}
		}

		public static void CheckOptionalStringProperty(object config, string propertyName)
		{
			CheckArguments(config, propertyName);
			var propertyInfo = GetPropertyInfo(config, propertyName);
			CheckPropertyType(propertyInfo, typeof(string));
			var text = GetPropertyValue(config, propertyInfo) as string;
			if (text != null && text == string.Empty)
			{
				InternalThrow(config, propertyInfo, "Value is empty.");
			}
		}

		public static void CheckArrayProperty(object config, string propertyName)
		{
			CheckArguments(config, propertyName);
			var propertyInfo = GetPropertyInfo(config, propertyName);
			if (!propertyInfo.PropertyType.IsArray)
			{
				throw new ArgumentException($"ConfigurationHelpers: \"{propertyInfo.Name}\" is not an array type!");
			}
			var array = (Array)GetPropertyValue(config, propertyInfo);
			if (array != null && array.Length != 0)
			{
				return;
			}
			InternalThrow(config, propertyInfo, "At least one element is required.");
		}

		public static void CheckPropertyMinMax(object config, string propertyName, IComparable min, IComparable max)
		{
			CheckArguments(config, propertyName);
			var propertyInfo = GetPropertyInfo(config, propertyName);
			var value = propertyInfo.GetValue(config, null);
			if (value == null)
			{
				InternalThrow(config, propertyInfo, "Value is missing.");
			}
			if (min != null && min.CompareTo(value) > 0)
			{
				InternalThrow(config, propertyInfo, $"Value {value} is less than the allowed minimum of {min}.");
			}
			if (max != null && max.CompareTo(value) < 0)
			{
				InternalThrow(config, propertyInfo, $"Value {value} is greater than the allowed maximum of {max}.");
			}
		}

		public static void CheckTypeNameProperty(object config, string propertyName, bool optional)
		{
			CheckArguments(config, propertyName);
			var propertyInfo = GetPropertyInfo(config, propertyName);
			CheckPropertyType(propertyInfo, typeof(string));
			var text = GetPropertyValue(config, propertyInfo) as string;
			if (text == null && optional)
			{
				return;
			}
			if (string.IsNullOrEmpty(text))
			{
				InternalThrow(config, propertyInfo, "Value is missing or empty.");
			}
			try
			{
				Type.GetType(text, true);
			}
			catch (Exception innerException)
			{
				InternalThrow(config, propertyInfo, "Not a valid type.", innerException);
			}
		}

		public static void ValidateChildConfigurationArray(object config, string propertyName, bool canBeNullOrEmpty)
		{
			var propertyInfo = GetPropertyInfo(config, propertyName);
			if (!typeof(ICollection).IsAssignableFrom(propertyInfo.PropertyType))
			{
				throw new ArgumentException($"ConfigurationHelpers: \"{propertyInfo.Name}\" is not an array!");
			}
			var collection = (ICollection)GetPropertyValue(config, propertyInfo);
			if (!canBeNullOrEmpty && (collection == null || collection.Count == 0))
			{
				InternalThrow(config, propertyInfo, "At least one element is required.");
			}
			if (collection != null)
			{
				foreach (var item in collection)
				{
					if (item == null)
					{
						InternalThrow(config, propertyInfo, "Invalid array entry.");
					}
					var adeComponentConfigurationListItem = item as AdeComponentConfigurationListItem;
					if (adeComponentConfigurationListItem != null)
					{
						adeComponentConfigurationListItem.Validate();
					}
				}
			}
		}

		public static void ThrowConfigurationError(object config, string propertyName, string message)
		{
			ThrowConfigurationError(config, propertyName, message, null);
		}

		public static void ThrowConfigurationError(object config, string propertyName, string message, Exception innerException)
		{
			CheckArguments(config, propertyName);
			if (string.IsNullOrEmpty(message))
			{
				throw new ArgumentNullException(nameof(message));
			}
			InternalThrow(config, GetPropertyInfo(config, propertyName), message, innerException);
		}

		public static T ParseFlags<T>(string value)
		{
			return (T)ParseFlags(typeof(T), value);
		}

		public static object ParseFlags(Type enumType, string value)
		{
			if (enumType == null)
			{
				throw new ArgumentNullException(nameof(enumType));
			}
			if (!enumType.IsEnum)
			{
				throw new ArgumentException("Enum type expected");
			}
			var num = 0;
			if (!string.IsNullOrEmpty(value))
			{
				var array = value.Split('|');
				var array2 = array;
				foreach (var text in array2)
				{
					var num2 = (int)Enum.Parse(enumType, text.Trim());
					num |= num2;
				}
			}
			return Enum.ToObject(enumType, num);
		}

		private static void InternalThrow(object config, PropertyInfo info, string message)
		{
			InternalThrow(config, info, message, null);
		}

		private static void InternalThrow(object config, PropertyInfo info, string message, Exception innerException)
		{
			var xmlName = GetXmlName(info);
			var reflectedName = GetReflectedName(config);
			var name = config.GetType().Name;
			var text = (reflectedName != null) ? $"{name} \"{reflectedName}\"" : name;
			var ex = new InvalidConfigFileException(
				$"Error in configuration file at {text}, {xmlName}:{Environment.NewLine}{message}", innerException);
			var stackTrace = new StackTrace(false);
			MethodBase methodBase = null;
			var num = 2;
			while (methodBase == null && num < stackTrace.FrameCount)
			{
				var frame = stackTrace.GetFrame(num);
				var method = frame.GetMethod();
				if (method.DeclaringType != typeof(ConfigurationHelpers))
				{
					methodBase = method;
				}
				num++;
			}
			if (methodBase != null)
			{
				ex.Source = $"Module={methodBase.Module.Name}, Type={methodBase.DeclaringType.FullName}, Method={methodBase.Name}";
			}
			throw ex;
		}

		private static string GetXmlName(PropertyInfo info)
		{
			string text = null;
			var customAttributes = info.GetCustomAttributes(false);
			var array = customAttributes;
			foreach (var obj in array)
			{
				var xmlAttributeAttribute = obj as XmlAttributeAttribute;
				if (xmlAttributeAttribute != null && !string.IsNullOrEmpty(xmlAttributeAttribute.AttributeName))
				{
					return $"Attribute \"{xmlAttributeAttribute.AttributeName}\"";
				}
				var xmlElementAttribute = obj as XmlElementAttribute;
				if (xmlElementAttribute != null)
				{
					text = ((text != null) ? (text + ", " + xmlElementAttribute.ElementName) : xmlElementAttribute.ElementName);
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				return $"Element \"{text}\"";
			}
			return $"Property \"{info.Name}\"";
		}

		private static string GetReflectedName(object config)
		{
			var type = config.GetType();
			var array = nameProperties;
			foreach (var name in array)
			{
				var property = type.GetProperty(name);
				object obj = null;
				if (property != null && !IsPropertyIndexed(property))
				{
					try
					{
						obj = property.GetValue(config, null);
						if (obj != null)
						{
							var text = obj.ToString();
							if (!string.IsNullOrEmpty(text))
							{
								return text;
							}
						}
					}
					catch (TargetInvocationException)
					{
					}
				}
			}
			return null;
		}

		private static bool IsPropertyIndexed(PropertyInfo property)
		{
			var indexParameters = property.GetIndexParameters();
			return indexParameters.Length > 0;
		}

		private static void CheckPropertyType(PropertyInfo info, Type type)
		{
			if (type.IsAssignableFrom(info.PropertyType))
			{
				return;
			}
			throw new ArgumentException($"ConfigurationHelpers: \"{info.Name}\" is not of expected type \"{type.Name}\"!");
		}

		private static object GetPropertyValue(object config, PropertyInfo info)
		{
			try
			{
				return info.GetValue(config, null);
			}
			catch (TargetInvocationException ex)
			{
				InternalThrow(config, info, "Value is missing or invalid.", ex.InnerException);
			}
			return null;
		}

		private static PropertyInfo GetPropertyInfo(object config, string propertyName)
		{
			var type = config.GetType();
			var property = type.GetProperty(propertyName);
			if (property == null)
			{
				throw new ArgumentException(
					$"ConfigurationHelpers: \"{propertyName}\" is not a public property of \"{type.FullName}\"!");
			}
			return property;
		}

		private static void CheckArguments(object config, string propertyName)
		{
			if (config == null)
			{
				throw new ArgumentNullException(nameof(config));
			}
			if (!string.IsNullOrEmpty(propertyName))
			{
				return;
			}
			throw new ArgumentNullException(nameof(propertyName));
		}
	}
}