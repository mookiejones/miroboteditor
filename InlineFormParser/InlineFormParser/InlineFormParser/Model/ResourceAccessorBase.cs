#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:8:24 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Resources;
using System.Windows.Threading;
using InlineFormParser.Common.ResourceAccess;
using InlineFormParser.Common.Tracing;
using InlineFormParser.ViewModel;

#endregion

namespace InlineFormParser.Model
{
	public abstract class ResourceAccessorBase : PropertyChangedNotifier, IIndexedResourceAccessor, IResourceAccessor,
		IDisposable
	{
		private IndexedAccess<object> objectAccess;

		private bool observeCultureChanges;

		private IndexedAccess<string> stringAccess;
		protected PrettyTraceSource trace = TraceSourceFactory.GetSource(PredefinedTraceSource.Resources);

		protected ResourceAccessorBase(bool observeCultureChanges)
		{
			this.observeCultureChanges = observeCultureChanges;
			if (observeCultureChanges) CultureSwitch.CultureChanged += OnCultureChanged;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public IndexedAccess<string> Strings
		{
			get
			{
				if (stringAccess == null) stringAccess = new IndexedAccess<string>(GetIndexedString);
				return stringAccess;
			}
		}

		public IndexedAccess<object> Objects
		{
			get
			{
				if (objectAccess == null) objectAccess = new IndexedAccess<object>(GetIndexedObject);
				return objectAccess;
			}
		}

		public string GetString(object requester, string key, params object[] args)
		{
			if (key == null) throw new ArgumentNullException(nameof(key));
			if (key == string.Empty) return string.Empty;
			if (key.IndexOf('%') == 0) return key.Substring(1);
			string text = GetString(key);
			if (args != null && args.Length > 0)
				try
				{
					text = string.Format(text, args);
					return text;
				}
				catch (FormatException exception)
				{
					trace.WriteException(exception, false,
						"ResourceAccessorBase.GetString: Error formatting \"{0}\" with {1} parameter(s).", text, args.Length);
					return text;
				}

			return text;
		}

		public object GetObject(object requester, string key)
		{
			if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
			return GetObject(key);
		}

		~ResourceAccessorBase()
		{
			Dispose(false);
		}

		public static object SearchInAssembly(string key, Assembly assembly)
		{
			if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
			if (assembly == null) throw new ArgumentNullException(nameof(assembly));
			string[] manifestResourceNames = assembly.GetManifestResourceNames();
			string[] array = manifestResourceNames;
			foreach (string text in array)
			{
				int num = text.LastIndexOf(".resources");
				if (num >= 0)
				{
					string baseName = text.Remove(num);
					ResourceManager resourceManager = new ResourceManager(baseName, assembly);
					object @object = resourceManager.GetObject(key);
					if (@object != null) return @object;
				}
			}

			return null;
		}

		protected abstract string GetString(string key);

		protected abstract object GetObject(string key);

		protected virtual void Dispose(bool disposing)
		{
			if (observeCultureChanges)
			{
				CultureSwitch.CultureChanged -= OnCultureChanged;
				observeCultureChanges = false;
			}
		}

		private void OnCultureChanged(object sender, CultureChangedEventArgs e)
		{
			NotifyCultureChanged();
		}

		protected static ResourceManager[] GetAssemblyResourceManagers(Assembly assembly)
		{
			List<ResourceManager> list = new List<ResourceManager>();
			string[] manifestResourceNames = assembly.GetManifestResourceNames();
			string[] array = manifestResourceNames;
			foreach (string text in array)
			{
				int num = text.LastIndexOf(".resources");
				if (num > 0)
				{
					string baseName = text.Remove(num);
					list.Add(new ResourceManager(baseName, assembly));
				}
			}

			return list.ToArray();
		}

		protected static string GetStringFromResourceManagers(ResourceManager[] managers, string key)
		{
			foreach (ResourceManager resourceManager in managers)
				try
				{
					string @string = resourceManager.GetString(key, CultureSwitch.CurrentCulture);
					if (@string != null) return @string;
				}
				catch (MissingManifestResourceException)
				{
				}

			return null;
		}

		protected static object GetObjectFromResourceManagers(ResourceManager[] managers, string key)
		{
			foreach (ResourceManager resourceManager in managers)
				try
				{
					object @object = resourceManager.GetObject(key, CultureSwitch.CurrentCulture);
					if (@object != null) return @object;
				}
				catch (MissingManifestResourceException)
				{
				}

			return null;
		}

		protected void WarnNotFound(string key)
		{
			trace.WriteLine(TraceEventType.Warning, "{0}: Resource key \"{1}\" cannot be found for language \"{2}\"!",
				GetType().Name, key, CultureSwitch.CurrentCulture.Name);
		}

		protected void NotifyCultureChanged()
		{
			Dispatcher.Invoke(DispatcherPriority.Send, new Action<string>(FirePropertyChanged), "Strings");
			Dispatcher.Invoke(DispatcherPriority.Send, new Action<string>(FirePropertyChanged), "Objects");
		}

		private string GetIndexedString(string key)
		{
			return GetString(this, key);
		}

		private object GetIndexedObject(string key)
		{
			return GetObject(this, key);
		}
	}
}