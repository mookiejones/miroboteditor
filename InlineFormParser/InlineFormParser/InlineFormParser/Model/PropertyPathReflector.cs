using System;
using System.Reflection;
using System.Text;

namespace InlineFormParser.Model
{
	public class PropertyPathReflector
	{
		private BindingFlags bindingFlags;

		public PropertyPathReflector(bool allowNonPublicProperties)
		{
			bindingFlags = (BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.GetProperty);
			if (allowNonPublicProperties)
			{
				bindingFlags |= BindingFlags.NonPublic;
			}
		}

		public PropertyPathReflector()
			: this(false)
		{
		}

		public object ReflectPropertyPath(object instance, string path)
		{
			if (instance == null)
			{
				throw new ArgumentNullException(nameof(instance));
			}
			if (path == null)
			{
				throw new ArgumentNullException(nameof(path));
			}
			object obj = instance;
			string[] array = path.Split(new char[2]
			{
				'.',
				'['
			}, StringSplitOptions.RemoveEmptyEntries);
			bool flag = false;
			string[] array2 = array;
			foreach (string text in array2)
			{
				string text2 = text.Trim();
				if (text2.Length > 0)
				{
					if (text2 == "%")
					{
						if (flag)
						{
							throw new ArgumentException("Type-operator '%' can only appear once in a path.");
						}
						obj = obj.GetType();
						flag = true;
					}
					else
					{
						if (text2 == "?")
						{
							obj = ListProperties(obj);
							break;
						}
						if (text2.EndsWith("]"))
						{
							string indexers = text2.Substring(0, text2.Length - 1);
							obj = ResolveIndexers(obj, indexers);
						}
						else
						{
							obj = ResolveProperty(obj, text2, flag);
						}
					}
					if (obj == null)
					{
						break;
					}
				}
			}
			return obj;
		}

		private object ResolveProperty(object current, string propertyName, bool typeResolving)
		{
			Type type = typeResolving ? ((Type)current) : current.GetType();
			PropertyInfo property = type.GetProperty(propertyName, bindingFlags);
			if (property == null)
			{
				throw new ArgumentException($"Unknown property \"{propertyName}\" for type \"{type.FullName}\"");
			}
			if (typeResolving)
			{
				return property.PropertyType;
			}
			try
			{
				return property.GetValue(current, null);
			}
			catch (TargetInvocationException innerException)
			{
				throw new ArgumentException($"Cannot access value of property \"{propertyName}\"!", innerException);
			}
		}

		private object ResolveIndexers(object instance, string indexers)
		{
			string[] array = indexers.Split(',');
			Type type = instance.GetType();
			PropertyInfo propertyInfo = null;
			try
			{
				propertyInfo = type.GetProperty("Item", bindingFlags);
			}
			catch (AmbiguousMatchException)
			{
				Type[] estimatedParametersTypes = GetEstimatedParametersTypes(array);
				propertyInfo = type.GetProperty("Item", bindingFlags, null, null, estimatedParametersTypes, null);
			}
			if (propertyInfo == null)
			{
				throw new ArgumentException(string.Format("Type \"{1}\" has no indexer!", type.FullName));
			}
			ParameterInfo[] indexParameters = propertyInfo.GetIndexParameters();
			if (indexParameters.Length != array.Length)
			{
				throw new ArgumentException(
					$"Type \"{type.FullName}\" requires {indexParameters.Length} indexers, but {array.Length} indexers are given!");
			}
			object[] array2 = new object[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i].Trim();
				Type parameterType = indexParameters[i].ParameterType;
				try
				{
					array2[i] = Convert.ChangeType(text, parameterType);
				}
				catch (InvalidCastException innerException)
				{
					throw new ArgumentException(
						$"Cannot parse indexer {i} of type \"{type.FullName}\": \"{text}\" cannot be converted to \"{parameterType.FullName}\"!", innerException);
				}
			}
			try
			{
				return propertyInfo.GetValue(instance, array2);
			}
			catch (TargetInvocationException innerException2)
			{
				throw new ArgumentException($"Cannot access indexer value of type \"{type.FullName}\"!", innerException2);
			}
		}

		private Type[] GetEstimatedParametersTypes(string[] tokens)
		{
			Type[] array = new Type[tokens.Length];
			for (int i = 0; i < tokens.Length; i++)
			{
				try
				{
					Convert.ChangeType(tokens[i].Trim(), typeof(int));
					array[i] = typeof(int);
				}
				catch (InvalidCastException)
				{
					array[i] = typeof(string);
				}
			}
			return array;
		}

		private string ListProperties(object instance)
		{
			Type type = instance.GetType();
			PropertyInfo[] properties = type.GetProperties(bindingFlags);
			StringBuilder stringBuilder = new StringBuilder();
			PropertyInfo[] array = properties;
			foreach (PropertyInfo propertyInfo in array)
			{
				stringBuilder.AppendFormat("{0} : {1};", propertyInfo.PropertyType.FullName, propertyInfo.Name);
				ParameterInfo[] indexParameters = propertyInfo.GetIndexParameters();
				if (indexParameters.Length > 0)
				{
					stringBuilder.Append('[');
					for (int j = 0; j < indexParameters.Length; j++)
					{
						if (j > 0)
						{
							stringBuilder.Append(", ");
						}
						stringBuilder.Append(indexParameters[j].ParameterType.Name);
					}
					stringBuilder.Append(']');
				}
				stringBuilder.AppendLine();
			}
			return stringBuilder.ToString();
		}
	}
}