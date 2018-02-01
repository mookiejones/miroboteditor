#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:8:23 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System;
using System.Reflection;
using System.Resources;

#endregion
using Component = InlineFormParser.Model.AdeComponent;
namespace InlineFormParser.Model
{
	public class FrameworkResourceAccessor : ComponentResourceAccessor
	{
		public FrameworkResourceAccessor(string defaultModuleName)
			: this(null, defaultModuleName, true)
		{
		}

		public FrameworkResourceAccessor(string defaultModuleName, bool observeCultureChanges)
			: this(null, defaultModuleName, observeCultureChanges)
		{
		}

		public FrameworkResourceAccessor(Component component, string defaultModuleName)
			: this(component, defaultModuleName, true)
		{
		}

		public FrameworkResourceAccessor(Component component, string defaultModuleName, bool observeCultureChanges)
			: base(component, observeCultureChanges)
		{
			DefaultModuleName = defaultModuleName;
		}

		private string DefaultModuleName { get; }

		protected override string GetString(string key)
		{
			if (key.IndexOf('@') < 0)
			{
				if (key.IndexOf('$') >= 0) return (string) ResolveAdornedKey(key, true);
				int num = key.IndexOf('#');
				if (num >= 0)
				{
					string moduleName = key.Substring(0, num);
					string key2 = key.Substring(num + 1);
					string result = default(string);
					RuntimeClient.Instance.TryGetString(moduleName, key2, out result, true);
					return result;
				}

				string result2 = default(string);
				if (!string.IsNullOrEmpty(DefaultModuleName) &&
				    RuntimeClient.Instance.TryGetString(DefaultModuleName, key, out result2, false)) return result2;
			}

			return base.GetString(key);
		}

		protected override object GetObject(string key)
		{
			if (key.IndexOf('@') < 0 && key.IndexOf('$') >= 0) return ResolveAdornedKey(key, false);
			return base.GetObject(key);
		}

		private object ResolveAdornedKey(string adornedKey, bool returnKeyIfNotFound)
		{
			int num = adornedKey.IndexOf('$');
			string text;
			if (num > 0 && num + 1 < adornedKey.Length)
			{
				text = adornedKey.Substring(0, num).Trim();
				string text2 = adornedKey.Substring(num + 1).Trim();
				int num2 = text2.IndexOf(',');
				if (num2 >= 0)
				{
					string text3 = text2.Substring(0, num2).Trim();
					string text4 = text2.Substring(num2 + 1).Trim();
					if (!string.IsNullOrEmpty(text4) && !string.IsNullOrEmpty(text3))
					{
						Assembly assembly = SafeLoadAssembly(text4, adornedKey);
						if (assembly != null)
						{
							ResourceManager resourceManager = new ResourceManager(text3, assembly);
							object @object = resourceManager.GetObject(text);
							if (@object != null) return @object;
						}

						goto IL_00df;
					}

					throw new ArgumentException($"FrameworkResourceAccessor: \"{adornedKey}\" is an invalid adorned key.");
				}

				Assembly assembly2 = SafeLoadAssembly(text2, adornedKey);
				if (assembly2 != null)
				{
					object obj = SearchInAssembly(text, assembly2);
					if (obj != null) return obj;
				}

				goto IL_00df;
			}

			throw new ArgumentException($"FrameworkResourceAccessor: \"{adornedKey}\" is an invalid adorned key.");
			IL_00df:
			WarnNotFound(adornedKey);
			if (!returnKeyIfNotFound) return null;
			return text;
		}

		private Assembly SafeLoadAssembly(string assemblyName, string adornedKey)
		{
			try
			{
				return Assembly.Load(assemblyName);
			}
			catch (Exception exception)
			{
				trace.WriteException(exception, false,
					"FrameworkResourceAccessor.SafeLoadAssembly: \"{0}\" could not be loaded to resolve adorned key \"{1}\"!",
					assemblyName, adornedKey);
			}

			return null;
		}
	}
}