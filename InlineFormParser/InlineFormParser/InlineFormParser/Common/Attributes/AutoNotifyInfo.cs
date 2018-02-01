#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:7:28 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.Reflection;

#endregion

namespace InlineFormParser.Common.Attributes
{
	public static class AutoNotifyInfo
	{
		private static readonly Dictionary<Type, AutoNotifyPropertyDictionary> autoNotifyInfos =
			new Dictionary<Type, AutoNotifyPropertyDictionary>();

		public static void Clear()
		{
			autoNotifyInfos.Clear();
		}

		internal static void CheckAddAutoNotifyInfo(Type senderType)
		{
			if (!autoNotifyInfos.ContainsKey(senderType))
			{
				autoNotifyInfos[senderType] = null;
				var array = (UsesAutoNotifyAttribute[]) senderType.GetCustomAttributes(typeof(UsesAutoNotifyAttribute), true);
				if (array != null && array.Length != 0)
				{
					var autoNotifyPropertyDictionary = new AutoNotifyPropertyDictionary(senderType);
					var properties = senderType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
					var dictionary = new Dictionary<string, PropertyInfo>();
					var array2 = properties;
					foreach (var propertyInfo in array2) dictionary[propertyInfo.Name] = propertyInfo;
					var array3 = properties;
					foreach (var propertyInfo2 in array3)
					{
						var array4 =
							(PropertyDependsOnAttribute[]) propertyInfo2.GetCustomAttributes(typeof(PropertyDependsOnAttribute), true);
						if (array4 != null && array4.Length > 0)
						{
							var name = propertyInfo2.Name;
							var array5 = array4;
							foreach (var propertyDependsOnAttribute in array5)
							{
								var dependsOnPropertyNames = propertyDependsOnAttribute.DependsOnPropertyNames;
								foreach (var text in dependsOnPropertyNames)
								{
									if (!dictionary.ContainsKey(text))
										throw new ArgumentException(
											$"The property \"{text}\" referred by the PropertyDependsOnAttribute of property \"{name}\" in class \"{senderType.FullName}\" is not known.");
									if (autoNotifyPropertyDictionary.CheckCyclicDependency(text, name))
										throw new ArgumentException(
											$"Cyclic dependency detected by adding the dependency of property \"{name}\" on property \"{text}\" in class \"{senderType.FullName}\"!");
									DependsOnPropertyCollection dependsOnPropertyCollection = null;
									dependsOnPropertyCollection = autoNotifyPropertyDictionary.ContainsKey(text)
										? autoNotifyPropertyDictionary[text]
										: (autoNotifyPropertyDictionary[text] = new DependsOnPropertyCollection(text));
									if (dependsOnPropertyCollection.Contains(name))
										throw new ArgumentException(
											$"The dependency of property \"{name}\" on property \"{text}\" in class \"{senderType.FullName}\" is defined more than once.");
									dependsOnPropertyCollection.Add(name);
								}
							}
						}
					}

					if (autoNotifyPropertyDictionary.Count > 0) autoNotifyInfos[senderType] = autoNotifyPropertyDictionary;
				}
			}
		}

		internal static bool TypeUsesAutoNotify(Type senderType)
		{
			if (autoNotifyInfos.ContainsKey(senderType)) return autoNotifyInfos[senderType] != null;
			return false;
		}

		internal static AutoNotifyPropertyDictionary GetAutoNotifyPropertyDictionary(Type senderType)
		{
			if (autoNotifyInfos.ContainsKey(senderType)) return autoNotifyInfos[senderType];
			return null;
		}
	}
}