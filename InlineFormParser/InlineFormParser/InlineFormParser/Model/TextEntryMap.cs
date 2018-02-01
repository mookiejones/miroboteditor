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
using System.Linq;

#endregion

namespace InlineFormParser.Model
{
	public class TextEntryMap
	{
		private readonly Dictionary<string, TextEntry> textsByKey =
			new Dictionary<string, TextEntry>(StringComparer.InvariantCultureIgnoreCase);

		private Dictionary<string, int> languageIndicesByName;

		public TextEntryMap(string[] languageFilter)
		{
			LanguageFilter = languageFilter;
		}

		internal string[] LanguageFilter { get; }

		internal int NumEntries => textsByKey.Count;

		internal IEnumerable<string> Keys => textsByKey.Keys;

		internal int GetLanguageFilterIndex(string langCode)
		{
			if (languageIndicesByName == null)
			{
				languageIndicesByName = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
				for (var i = 0; i < LanguageFilter.Length; i++)
				{
					var key = LanguageFilter[i];
					languageIndicesByName[key] = i;
				}
			}

			if (!languageIndicesByName.ContainsKey(langCode)) return -1;
			return languageIndicesByName[langCode];
		}

		internal bool ContainsTextEntry(string key)
		{
			return textsByKey.ContainsKey(key);
		}

		internal TextEntry GetTextEntry(string key)
		{
			return textsByKey[key];
		}

		internal string GetLanguageOfEntry(TextEntry entry)
		{
			return LanguageFilter[entry.LanguageCodeIndex];
		}

		internal void AddTextEntry(string key, TextEntry entry)
		{
			textsByKey[key] = entry;
		}

		internal void RemoveTextEntry(string key)
		{
			textsByKey.Remove(key);
		}

		internal void Clear()
		{
			textsByKey.Clear();
		}

		internal string[] GetContainedLanguages()
		{
			var hashSet = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
			foreach (var value in textsByKey.Values)
			{
				var languageCodeIndex = value.LanguageCodeIndex;
				hashSet.Add(LanguageFilter[languageCodeIndex]);
			}

			return hashSet.ToArray();
		}

		internal string GetMessageKeyByNumber(int messageNumber)
		{
			foreach (var item in textsByKey)
				if (item.Value.MessageNumber == messageNumber)
					return item.Key;
			return null;
		}
	}
}