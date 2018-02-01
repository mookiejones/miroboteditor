#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:8:32 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Threading;
using InlineFormParser.Common;
using InlineFormParser.Common.Attributes;
using InlineFormParser.Common.Tracing;
using InlineFormParser.Misc;
using InlineFormParser.Zip;

#endregion

namespace InlineFormParser.Model
{
	public class RuntimeClient : IDisposable
	{
		public enum ResolveMessageCodeResult
		{
			Unknown,
			Success,
			CodeIsToShort,
			ModuleShortcutNotKnown,
			CantParseMessageNumber,
			MessageNumberOutOfRange,
			MessageNotFoundInModule
		}

		private const string zipExtension = "kxz";

		private static readonly object CreationLock = new object();

		private static string defaultFileExtension = "kxr";

		private static RuntimeClient instance;

		private readonly object cacheLock = new object();

		private readonly Dispatcher dispatcher;

		private readonly HashSet<string> launchedWarnings = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		private readonly List<ILoadedModule> loadedModules = new List<ILoadedModule>();

		private readonly Dictionary<string, string> moduleNamesByShortcut =
			new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		private readonly Dictionary<string, LoadedModule> modulesByName =
			new Dictionary<string, LoadedModule>(StringComparer.OrdinalIgnoreCase);

		private readonly PrettyTraceSource trace = TraceSourceFactory.GetSource(PredefinedTraceSource.Resources);

		private CultureInfo currentLanguage;

		private bool observesCultureSwitch;

		private Dictionary<string, HashSet<ZipFile>> zipFiles;

		public RuntimeClient(string[] searchDirectories, [CanBeNull] CultureInfo initialLanguage)
		{
			dispatcher = Dispatcher.CurrentDispatcher;
			ArrayHelpers.CheckStringArray(searchDirectories, "searchDirectories", ArrayHelpers.CheckStringArrayOptions.CheckAll);
			SearchDirectories = searchDirectories;
			FileExtension = defaultFileExtension;
			if (initialLanguage == null)
			{
				observesCultureSwitch = true;
				currentLanguage = CultureSwitch.CurrentCulture;
				CultureSwitch.AddCultureChangedListener(CultureSwitchListenerPriority.Primary, OnCultureChanged);
			}
			else
			{
				CheckLanguage(initialLanguage);
				currentLanguage = initialLanguage;
			}

			trace.WriteLine(TraceEventType.Information,
				"RuntimeClient created: Language=\"{0}\", ObservesCultureSwitch={1}, SearchDirectories=\"{2}\", FileExtension=\"{3}\"",
				CurrentLanguageCode, observesCultureSwitch, ArrayHelpers.ArrayToString(searchDirectories), FileExtension);
			OpenZipFiles();
		}

		public static string DefaultFileExtension
		{
			get => defaultFileExtension;
			set
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentNullException(nameof(value), "value should not be null or empty");
				defaultFileExtension = value;
			}
		}

		public static RuntimeClient Instance
		{
			get
			{
				lock (CreationLock)
				{
					return instance ??
					       (instance = new RuntimeClient(ApplicationEnvironment.Current.KxrDirectories, null));
				}
			}
		}

		public CultureInfo CurrentLanguage
		{
			get => currentLanguage;
			set
			{
				if (observesCultureSwitch)
					throw new InvalidOperationException("Current language can not be set because culture switch is used!");
				InternalSetLanguage(value);
			}
		}

		public string CurrentLanguageCode
		{
			get => currentLanguage.Name;
			set
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentNullException(nameof(value), "value should not be null or empty");
				CurrentLanguage = CultureInfo.GetCultureInfo(value);
			}
		}

		public int CurrentLanguageID
		{
			get => currentLanguage.LCID;
			set => CurrentLanguage = CultureInfo.GetCultureInfo(value);
		}

		public string FileExtension { get; }

		public IEnumerable<ILoadedModule> LoadedModules
		{
			get
			{
				lock (cacheLock)
				{
					return loadedModules;
				}
			}
		}

		public string[] SearchDirectories { get; }

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public event EventHandler FailedRequestsChanged;

		public event EventHandler LoadedModulesChanged;

		~RuntimeClient()
		{
			Dispose(false);
		}

		public static void Exit()
		{
			lock (CreationLock)
			{
				if (instance != null)
				{
					instance.Dispose();
					instance = null;
				}
			}
		}

		public void ClearKxrCache()
		{
			lock (cacheLock)
			{
				modulesByName.Clear();
				moduleNamesByShortcut.Clear();
				if (loadedModules.Count > 0)
				{
					loadedModules.Clear();
					FireLoadedModulesChanged();
				}

				if (launchedWarnings.Count > 0)
				{
					launchedWarnings.Clear();
					FireFailedRequestsChanged();
				}
			}
		}

		public string[] GetAvailableModuleNames()
		{
			lock (cacheLock)
			{
				HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
				char[] anyOf = new char[2]
				{
					'_',
					'.'
				};
				string searchPattern = string.Format(CultureInfo.InvariantCulture, "*.{0}", FileExtension);
				string[] array = SearchDirectories;
				foreach (string directory in array)
				{
					IEnumerable<string> matchingFilesInDirectoryAndSubDirectories =
						GetMatchingFilesInDirectoryAndSubDirectories(directory, searchPattern);
					foreach (string item3 in matchingFilesInDirectoryAndSubDirectories)
					{
						string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(item3);
						int num = fileNameWithoutExtension.IndexOfAny(anyOf);
						string item = num < 0 ? fileNameWithoutExtension : fileNameWithoutExtension.Substring(0, num);
						hashSet.Add(item);
					}
				}

				foreach (HashSet<ZipFile> value in zipFiles.Values)
				foreach (ZipFile item4 in value)
				foreach (ZipEntry entry in item4.Entries)
				{
					string fileNameWithoutExtension2 = Path.GetFileNameWithoutExtension(entry.FileName);
					int num2 = fileNameWithoutExtension2.IndexOfAny(anyOf);
					string item2 = num2 < 0 ? fileNameWithoutExtension2 : fileNameWithoutExtension2.Substring(0, num2);
					hashSet.Add(item2);
				}

				return hashSet.ToArray();
			}
		}

		public IList<string> GetKeys([NotNull] string moduleName, [NotNull] string identifier)
		{
			if (moduleName == null) throw new ArgumentNullException(nameof(moduleName));
			if (identifier == null) throw new ArgumentNullException(nameof(identifier));
			List<string> result = new List<string>();
			lock (cacheLock)
			{
				LoadedModule loadedModule = InternalGetModule(moduleName);
				IEnumerable<IModuleElement> source = from moduleElement in loadedModule.Entries
					where moduleElement.Key.StartsWith(identifier, StringComparison.OrdinalIgnoreCase)
					select moduleElement;
				foreach (IModuleElement item in from entry in source
					where !result.Contains(entry.Key)
					select entry)
					result.Add(item.Key);
			}

			return result;
		}

		public string[] GetFailedRequests()
		{
			List<string> list = new List<string>();
			lock (cacheLock)
			{
				list.AddRange(launchedWarnings);
			}

			list.Sort();
			return list.ToArray();
		}

		public ILoadedModule GetModule(string moduleName)
		{
			if (string.IsNullOrEmpty(moduleName)) throw new ArgumentNullException(nameof(moduleName));
			lock (cacheLock)
			{
				return InternalGetModule(moduleName);
			}
		}

		public bool IsDefined(string moduleName, string key)
		{
			if (key == null) throw new ArgumentNullException(nameof(key));
			if (!string.IsNullOrEmpty(key) && key.IndexOf('%') != 0)
			{
				if (string.IsNullOrEmpty(moduleName)) throw new ArgumentNullException(nameof(moduleName));
				lock (cacheLock)
				{
					LoadedModule loadedModule = InternalGetModule(moduleName);
					return loadedModule.ContainsKey(key);
				}
			}

			return true;
		}

		public bool TryGetMessageNumber(string moduleName, string key, out int messageNumber)
		{
			if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
			if (string.IsNullOrEmpty(moduleName)) throw new ArgumentNullException(nameof(moduleName));
			lock (cacheLock)
			{
				LoadedModule loadedModule = InternalGetModule(moduleName);
				if (loadedModule.ContainsKey(key))
				{
					TextEntry textEntry = loadedModule.GetTextEntry(key);
					messageNumber = textEntry.MessageNumber;
					return messageNumber > 0;
				}
			}

			messageNumber = 0;
			return false;
		}

		public bool TryGetMessageType(string moduleName, string key, out MessageType messageType)
		{
			if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
			if (string.IsNullOrEmpty(moduleName)) throw new ArgumentNullException(nameof(moduleName));
			lock (cacheLock)
			{
				LoadedModule loadedModule = InternalGetModule(moduleName);
				if (loadedModule.ContainsKey(key))
				{
					TextEntry textEntry = loadedModule.GetTextEntry(key);
					messageType = textEntry.MessageType;
					return messageType != MessageType.Unknown;
				}
			}

			messageType = MessageType.Unknown;
			return false;
		}

		public bool TryGetModuleNameByShortcut(string shortcut, out string moduleName)
		{
			if (string.IsNullOrEmpty(shortcut)) throw new ArgumentNullException(nameof(shortcut));
			lock (cacheLock)
			{
				if (moduleNamesByShortcut.ContainsKey(shortcut))
				{
					moduleName = moduleNamesByShortcut[shortcut];
					return true;
				}
			}

			moduleName = null;
			return false;
		}

		public bool TryGetModuleShortcut(string moduleName, out string shortcut)
		{
			if (string.IsNullOrEmpty(moduleName)) throw new ArgumentNullException(nameof(moduleName));
			lock (cacheLock)
			{
				LoadedModule loadedModule = InternalGetModule(moduleName);
				shortcut = loadedModule.Shortcut;
				return !string.IsNullOrEmpty(loadedModule.Shortcut);
			}
		}

		public bool TryGetString(string moduleName, string key, [CanBeNull] out string result)
		{
			return TryGetString(moduleName, key, out result, true);
		}

		public bool TryGetString(string moduleName, string key, [CanBeNull] out string result, bool logIfNotFound)
		{
			if (key == null) throw new ArgumentNullException(nameof(key));
			if (string.IsNullOrEmpty(key))
			{
				result = string.Empty;
				return true;
			}

			if (key.IndexOf('%') == 0)
			{
				result = key.Substring(1);
				return true;
			}

			if (string.IsNullOrEmpty(moduleName)) throw new ArgumentNullException(nameof(moduleName));
			lock (cacheLock)
			{
				LoadedModule loadedModule = InternalGetModule(moduleName);
				if (loadedModule.ContainsKey(key))
				{
					result = loadedModule.GetString(key);
					return true;
				}

				Helpers.TryReplaceParameters(key, out result);
				if (logIfNotFound)
				{
					string item = MakeCombinedKey(moduleName, key);
					if (!launchedWarnings.Contains(item))
					{
						trace.WriteLine(TraceEventType.Warning,
							"XmlResources: No entry found for key \"{0}\" in module \"{1}\" in language \"{2}\"!", key, moduleName,
							currentLanguage.Name);
						launchedWarnings.Add(item);
						FireFailedRequestsChanged();
					}
				}
			}

			return false;
		}

		public bool TryGetString(string moduleName, string key, string[] parameters, [CanBeNull] bool[] lookupParameters,
			[CanBeNull] out string result)
		{
			return TryGetString(moduleName, key, parameters, lookupParameters, out result, true);
		}

		public bool TryGetString(string moduleName, string key, string[] parameters, [CanBeNull] bool[] lookupParameters,
			[CanBeNull] out string result, bool logIfNotFound)
		{
			ArrayHelpers.CheckStringArray(parameters, "parameters", ArrayHelpers.CheckStringArrayOptions.ElementsNotNull);
			if (lookupParameters != null && lookupParameters.Length != parameters.Length)
				throw new ArgumentException("\"lookupParameters\" must have the same size as \"parameters\"");
			bool result2 = true;
			string[] array;
			if (lookupParameters != null)
			{
				array = new string[parameters.Length];
				for (int i = 0; i < parameters.Length; i++)
					if (lookupParameters[i])
					{
						string text = parameters[i];
						int num = text.IndexOf('#');
						string moduleName2 = num >= 0 ? text.Substring(0, num) : moduleName;
						string key2 = num >= 0 ? text.Substring(num + 1) : text;
						if (!TryGetString(moduleName2, key2, out array[i], logIfNotFound)) result2 = false;
					}
					else
					{
						array[i] = parameters[i];
					}
			}
			else
			{
				array = parameters;
			}

			string input = default(string);
			if (TryGetString(moduleName, key, out input, logIfNotFound))
			{
				Helpers.TryReplaceParameters(input, out result);
				try
				{
					input = result = string.Format(CultureInfo.InvariantCulture, result, array);
					return result2;
				}
				catch (Exception)
				{
					return result2;
				}
			}

			Helpers.TryReplaceParameters(key, out result);
			try
			{
				input = result = string.Format(CultureInfo.InvariantCulture, result, array);
			}
			catch
			{
			}

			return false;
		}

		public ResolveMessageCodeResult TryResolveMessageCode(string code, out string moduleName, out string key)
		{
			if (code == null) throw new ArgumentNullException(nameof(code));
			moduleName = null;
			key = null;
			if (code.Length < 4) return ResolveMessageCodeResult.CodeIsToShort;
			string shortcut = code.Substring(0, 3);
			int num = default(int);
			if (!int.TryParse(code.Substring(3), out num)) return ResolveMessageCodeResult.CantParseMessageNumber;
			if (num <= 0) return ResolveMessageCodeResult.MessageNumberOutOfRange;
			string text = default(string);
			if (!TryGetModuleNameByShortcut(shortcut, out text)) return ResolveMessageCodeResult.ModuleShortcutNotKnown;
			lock (cacheLock)
			{
				LoadedModule loadedModule = InternalGetModule(text);
				if (!loadedModule.TryGetMessageByNumber(num, out key)) return ResolveMessageCodeResult.MessageNotFoundInModule;
			}

			moduleName = text;
			return ResolveMessageCodeResult.Success;
		}

		protected virtual void Dispose(bool disposing)
		{
			lock (cacheLock)
			{
				if (observesCultureSwitch)
				{
					CultureSwitch.RemoveCultureChangedListener(CultureSwitchListenerPriority.Primary, OnCultureChanged);
					observesCultureSwitch = false;
				}

				LoadedModulesChanged = null;
				ClearKxrCache();
				ClearZipFiles();
			}
		}

		private static IEnumerable<string> GetMatchingFilesInDirectoryAndSubDirectories(string directory,
			string searchPattern)
		{
			List<string> list = new List<string>();
			if (!Directory.Exists(directory)) return list;
			list.AddRange(Directory.GetFiles(directory, searchPattern, SearchOption.TopDirectoryOnly));
			string[] directories = Directory.GetDirectories(directory);
			string[] array = directories;
			foreach (string text in array)
			{
				list.AddRange(Directory.GetFiles(text, searchPattern, SearchOption.TopDirectoryOnly));
				string path = Path.Combine(text, defaultFileExtension);
				if (Directory.Exists(path)) list.AddRange(Directory.GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly));
			}

			return list;
		}

		private static void CheckLanguage(CultureInfo language)
		{
			if (language == null) throw new ArgumentNullException(nameof(language));
			if (!string.IsNullOrEmpty(language.Name)) return;
			throw new ArgumentException("Language is not suitable! (neutral?)");
		}

		private static string MakeCombinedKey(string moduleName, string key)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}#{1}", moduleName, key);
		}

		private void ClearZipFiles()
		{
			if (zipFiles != null)
			{
				foreach (HashSet<ZipFile> value in zipFiles.Values)
				foreach (ZipFile item in value)
					item.Dispose();
				zipFiles = null;
			}
		}

		private LoadedModule CreateModule(string moduleName)
		{
			if (!Helpers.IsValidModuleName(moduleName))
				trace.WriteLine(TraceEventType.Warning, "XmlResources: Module name \"{0}\" is not suitable!", moduleName);
			trace.WriteLine(TraceEventType.Start, "XmlResources: Loading module \"{0}\" in language \"{1}\" ...", moduleName,
				CurrentLanguageCode);
			int num = 0;
			int num2 = 0;
			bool flag = false;
			Stopwatch stopwatch = Stopwatch.StartNew();
			string[] languageFilter = FallbackStrategy.GetLanguageFilter(CurrentLanguageCode);
			LoadedModule loadedModule = new LoadedModule(moduleName, languageFilter);
			try
			{
				Dictionary<string, FileSource> dictionary = new Dictionary<string, FileSource>(StringComparer.OrdinalIgnoreCase);
				string searchPattern = string.Format(CultureInfo.InvariantCulture, "{0}*.{1}", moduleName, FileExtension);
				string[] array = SearchDirectories;
				foreach (string text in array)
				{
					IEnumerable<string> matchingFilesInDirectoryAndSubDirectories =
						GetMatchingFilesInDirectoryAndSubDirectories(text, searchPattern);
					foreach (string item in matchingFilesInDirectoryAndSubDirectories)
					{
						string fileName = Path.GetFileName(item);
						if (loadedModule.IsFileRequired(fileName))
						{
							FileSource fileSource = new FileSystemSource(item);
							if (dictionary.ContainsKey(fileName))
								WarnEclipsedSource(fileSource, dictionary[fileName]);
							else
								dictionary[fileName] = fileSource;
						}
					}

					if (zipFiles.ContainsKey(text))
						foreach (ZipFile item2 in zipFiles[text])
						{
							string zipPath = Path.Combine(text, Path.GetFileName(item2.Name));
							foreach (ZipEntry entry in item2.Entries)
							{
								string fileName2 = entry.FileName;
								if (loadedModule.IsFileRequired(fileName2))
								{
									FileSource fileSource2 = new ZipSource(zipPath, entry);
									if (dictionary.ContainsKey(fileName2))
										WarnEclipsedSource(fileSource2, dictionary[fileName2]);
									else
										dictionary[fileName2] = fileSource2;
								}
							}
						}
				}

				if (trace.Switch.ShouldTrace(TraceEventType.Verbose))
				{
					string[] array2 = dictionary.Keys.ToArray();
					trace.WriteLine(TraceEventType.Verbose,
						"XmlResources: Module=\"{0}\" Language=\"{1}\" Filter=\"{2}\" Sources=\"{3}\"", moduleName, CurrentLanguageCode,
						ArrayHelpers.ArrayToString(languageFilter), ArrayHelpers.ArrayToString(array2));
					trace.WriteLine(TraceEventType.Verbose, "Finding sources took {0} [ms].", stopwatch.ElapsedMilliseconds);
				}

				foreach (FileSource value in dictionary.Values)
					try
					{
						loadedModule.AddContent(value);
						num++;
					}
					catch (Exception exception)
					{
						trace.WriteException(exception, false, "XmlResources: Error reading {0}!", value);
					}

				num2 = loadedModule.NumEntries;
				flag = true;
				return loadedModule;
			}
			catch (Exception exception2)
			{
				trace.WriteException(exception2, true, "Error creating module \"{0}\"!", moduleName);
				return loadedModule;
			}
			finally
			{
				stopwatch.Stop();
				trace.WriteLine(TraceEventType.Stop,
					"XmlResources: Loading module \"{0}\" in language \"{1}\" took {2} [ms], (success={3}, files={4}, entries={5})",
					moduleName, CurrentLanguageCode, stopwatch.ElapsedMilliseconds, flag, num, num2);
				if (num == 0)
					trace.WriteLine(TraceEventType.Warning, "XmlResources: No files found for module \"{0}\" in language \"{1}\"!",
						moduleName, CurrentLanguageCode);
				else if (num2 == 0)
					trace.WriteLine(TraceEventType.Warning, "XmlResources: No entries found for module \"{0}\" in language \"{1}\"!",
						moduleName, CurrentLanguageCode);
			}
		}

		private void FireFailedRequestsChanged()
		{
			if (FailedRequestsChanged != null)
				if (dispatcher.CheckAccess())
					FailedRequestsChanged(this, EventArgs.Empty);
				else
					dispatcher.BeginInvoke(new EventHandler(FailedRequestsChanged.Invoke), this, EventArgs.Empty);
		}

		private void FireLoadedModulesChanged()
		{
			if (LoadedModulesChanged != null)
				if (dispatcher.CheckAccess())
					LoadedModulesChanged(this, EventArgs.Empty);
				else
					dispatcher.BeginInvoke(new EventHandler(LoadedModulesChanged.Invoke), this, EventArgs.Empty);
		}

		private LoadedModule InternalGetModule(string moduleName)
		{
			if (modulesByName.ContainsKey(moduleName)) return modulesByName[moduleName];
			LoadedModule loadedModule = CreateModule(moduleName);
			modulesByName[moduleName] = loadedModule;
			if (!string.IsNullOrEmpty(loadedModule.Shortcut)) moduleNamesByShortcut[loadedModule.Shortcut] = moduleName;
			loadedModules.Add(loadedModule);
			FireLoadedModulesChanged();
			return loadedModule;
		}

		private void InternalSetLanguage(CultureInfo newLanguage)
		{
			CheckLanguage(newLanguage);
			if (!Helpers.AreCodesEqual(CurrentLanguageCode, newLanguage.Name))
				lock (cacheLock)
				{
					currentLanguage = newLanguage;
					ClearKxrCache();
				}
		}

		private void OnCultureChanged(object sender, CultureChangedEventArgs e)
		{
			InternalSetLanguage(e.NewCulture);
		}

		private void OpenZipFiles()
		{
			lock (cacheLock)
			{
				zipFiles = new Dictionary<string, HashSet<ZipFile>>(StringComparer.OrdinalIgnoreCase);
				Stopwatch stopwatch = Stopwatch.StartNew();
				try
				{
					trace.WriteLine(TraceEventType.Start, "Scanning for zip-files...");
					string searchPattern = string.Format(CultureInfo.InvariantCulture, "*.{0}", "kxz");
					string[] array = SearchDirectories;
					foreach (string text in array)
					{
						IEnumerable<string> matchingFilesInDirectoryAndSubDirectories =
							GetMatchingFilesInDirectoryAndSubDirectories(text, searchPattern);
						if (matchingFilesInDirectoryAndSubDirectories.Any())
						{
							HashSet<ZipFile> hashSet = new HashSet<ZipFile>();
							zipFiles[text] = hashSet;
							foreach (string item in matchingFilesInDirectoryAndSubDirectories)
								try
								{
									hashSet.Add(ZipFile.Read(item));
									trace.WriteLine(TraceEventType.Verbose, "Zip-file \"{0}\" opened.", item);
								}
								catch (Exception exception)
								{
									trace.WriteException(exception, false, "Could not open zip-file \"{0}\"!", item);
								}
						}
					}
				}
				finally
				{
					stopwatch.Stop();
					trace.WriteLine(TraceEventType.Stop, "{0} zip-file(s) scanned and opened in {1} [ms].", zipFiles.Count,
						stopwatch.ElapsedMilliseconds);
				}
			}
		}

		private void WarnEclipsedSource(FileSource eclipsed, FileSource eclipsedBy)
		{
			trace.WriteLine(TraceEventType.Warning, "\"{0}\" is eclipsed by \"{1}\" and is skipped!", eclipsed, eclipsedBy);
		}
	}
}