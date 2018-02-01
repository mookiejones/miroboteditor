#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:8:34 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using InlineFormParser.Common.Tracing;

#endregion

namespace InlineFormParser.Model
{
	internal class LoadedModule : ILoadedModule
	{
		private readonly TextEntryMap entryMap;

		private readonly List<string> readFilePathes = new List<string>();

		internal LoadedModule(string name, string[] languageFilter)
		{
			Name = name;
			entryMap = new TextEntryMap(languageFilter);
			LoadingTime = DateTime.Now;
		}

		public string Name { get; }

		public string Shortcut { get; private set; }

		public int NumEntries => entryMap.NumEntries;

		public IEnumerable<IModuleElement> Entries
		{
			get
			{
				foreach (var key in entryMap.Keys) yield return new DiagModuleElement(this, key);
			}
		}

		public DateTime LoadingTime { get; }

		public IEnumerable<string> ReadFilePathes => readFilePathes;

		public string[] LanguageFilter => entryMap.LanguageFilter;

		public string[] GetContainedLanguages()
		{
			return entryMap.GetContainedLanguages();
		}

		internal bool IsFileRequired(string fileName)
		{
			if (!fileName.StartsWith(Name, StringComparison.InvariantCultureIgnoreCase)) return false;
			fileName = Path.GetFileNameWithoutExtension(fileName);
			if (fileName.Length > Name.Length)
			{
				var num = Name.Length;
				if (fileName[num] == '_')
				{
					num = fileName.IndexOf('.', num + 1);
					if (num < 0) return true;
				}

				if (fileName[num] == '.')
				{
					var langCode = fileName.Substring(num + 1);
					if (entryMap.GetLanguageFilterIndex(langCode) < 0) return false;
					goto IL_007d;
				}

				return false;
			}

			IL_007d:
			return true;
		}

		internal bool ContainsKey(string key)
		{
			return entryMap.ContainsTextEntry(key);
		}

		internal TextEntry GetTextEntry(string key)
		{
			return entryMap.GetTextEntry(key);
		}

		internal string GetLanguageOfEntry(TextEntry entry)
		{
			return entryMap.GetLanguageOfEntry(entry);
		}

		internal string GetString(string key)
		{
			return entryMap.GetTextEntry(key).Text;
		}

		internal bool TryGetMessageByNumber(int number, out string key)
		{
			key = entryMap.GetMessageKeyByNumber(number);
			return key != null;
		}

		internal void AddContent(FileSource source)
		{
			var kxrReader = new KxrReader(source);
			var text = default(string);
			kxrReader.ReadModule(Name, entryMap, out text);
			readFilePathes.Add(source.ToString());
			if (text != null)
				if (Shortcut != null && !string.Equals(Shortcut, text))
					TraceSourceFactory.GetSource(PredefinedTraceSource.Resources).WriteLine(TraceEventType.Error,
						"Module shortcut \"{0}\" differs from \"{1}\" for module \"{2}\" in {3}!", text, Shortcut, Name, source);
				else
					Shortcut = text;
		}
	}
}