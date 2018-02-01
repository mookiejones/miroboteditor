#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:8:38 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using InlineFormParser.Common.Tracing;

#endregion

namespace InlineFormParser.Model
{
	public  class KxrReader
	{
		public KxrReader(FileSource source)
		{
			this.source = source;
			if (elementMap == null)
			{
				elementMap = new Dictionary<string, ReaderState>();
				elementMap["resources"] = ReaderState.Resources;
				elementMap["module"] = ReaderState.Module;
				elementMap["uiText"] = ReaderState.UiTextElement;
				elementMap["message"] = ReaderState.MessageElement;
				elementMap["text"] = ReaderState.Text;
			}
		}

		public void ReadModule(string moduleName, TextEntryMap resultMap, out string shortcut)
		{
			this.moduleName = moduleName;
			this.resultMap = resultMap;
			inModule = false;
			moduleFound = false;
			moduleShortcut = null;
			currentEntry = null;
			defaultLanguage = null;
			currentLanguage = null;
			newEntries = 0;
			replacedEntries = 0;
			Stopwatch stopwatch = Stopwatch.StartNew();
			try
			{
				RawReadModule();
			}
			finally
			{
				stopwatch.Stop();
				trace.WriteLine(TraceEventType.Verbose,
					"KxrReader: Reading \"{0}\" took {1} [ms], new entries={2}, replaced entries={3}", source,
					stopwatch.ElapsedMilliseconds, newEntries, replacedEntries);
				if (!moduleFound)
					trace.WriteLine(TraceEventType.Error, "KxrReader: Module \"{0}\" was not found in \"{1}\"!", moduleName, source);
				shortcut = moduleShortcut;
				this.resultMap = null;
			}
		}

		private void RawReadModule()
		{
			using (Stream input = source.OpenReader())
			{
				using (XmlTextReader xmlTextReader = new XmlTextReader(input))
				{
					xmlTextReader.WhitespaceHandling = WhitespaceHandling.None;
					xmlTextReader.Normalization = true;
					LinkedList<ReaderState> linkedList = new LinkedList<ReaderState>();
					linkedList.AddLast(ReaderState.Initial);
					while (xmlTextReader.Read())
						switch (xmlTextReader.NodeType)
						{
							case XmlNodeType.Element:
							{
								ReaderState readerState = ReaderState.UnknownElement;
								if (elementMap.ContainsKey(xmlTextReader.Name)) readerState = elementMap[xmlTextReader.Name];
								linkedList.AddLast(readerState);
								ProcessBeginElement(xmlTextReader, readerState);
								if (xmlTextReader.IsEmptyElement)
								{
									ProcessEndElement(xmlTextReader, readerState);
									linkedList.RemoveLast();
								}

								break;
							}
							case XmlNodeType.Text:
								ProcessText(xmlTextReader, linkedList.Last.Value);
								break;
							case XmlNodeType.EndElement:
								ProcessEndElement(xmlTextReader, linkedList.Last.Value);
								linkedList.RemoveLast();
								break;
						}
					if (linkedList.Count != 1 || linkedList.Last.Value != 0)
						trace.WriteLine(TraceEventType.Error, "KxrReader: Problem with XML structure of \"{0}\"!", source);
				}
			}
		}

		private void ProcessBeginElement(XmlTextReader input, ReaderState state)
		{
			switch (state)
			{
				case ReaderState.Resources:
				{
					string attribute2 = input.GetAttribute("xml:lang");
					if (!string.IsNullOrEmpty(attribute2)) defaultLanguage = attribute2;
					break;
				}
				case ReaderState.Module:
					if (!inModule)
					{
						inModule = true;
						string attribute3 = input.GetAttribute("name");
						if (!string.IsNullOrEmpty(attribute3))
							if (string.Equals(attribute3, moduleName, StringComparison.InvariantCultureIgnoreCase))
							{
								moduleFound = true;
								moduleShortcut = input.GetAttribute("shortcut");
							}
							else
							{
								trace.WriteLine(TraceEventType.Warning, "KxrReader: Skipping unexpected module \"{0}\" in \"{1}\"!", attribute3,
									source);
							}
						else
							TraceErrorAttributeMissing(input, "name");
					}

					break;
				case ReaderState.MessageElement:
				case ReaderState.UiTextElement:
					if (inModule)
					{
						string attribute4 = input.GetAttribute("key");
						if (!string.IsNullOrEmpty(attribute4))
						{
							currentKey = attribute4;
							if (resultMap.ContainsTextEntry(attribute4))
							{
								currentEntry = resultMap.GetTextEntry(currentKey);
							}
							else
							{
								currentEntry = new TextEntry();
								resultMap.AddTextEntry(currentKey, currentEntry);
							}
						}
						else
						{
							TraceErrorAttributeMissing(input, "key");
						}

						if (state == ReaderState.MessageElement && currentEntry != null)
						{
							string attribute5 = input.GetAttribute("number");
							if (!string.IsNullOrEmpty(attribute5))
							{
								int num = default(int);
								if (int.TryParse(attribute5, out num) && num > 0)
									if (currentEntry.MessageNumber > 0 && currentEntry.MessageNumber != num)
										trace.WriteLine(TraceEventType.Error,
											"Message number mismatch for \"{0}\" in module \"{1}\" detected in {2}!", currentKey, moduleName,
											GetFileInfo(input));
									else
										currentEntry.MessageNumber = num;
								else
									trace.WriteLine(TraceEventType.Error, "Invalid message number \"{0}\" for \"{1}\" in module \"{2}\" ({3})!",
										attribute5, currentKey, moduleName, GetFileInfo(input));
							}

							string attribute6 = input.GetAttribute("type");
							if (!string.IsNullOrEmpty(attribute6))
							{
								MessageType messageType = Helpers.ParseEnum(attribute6, MessageType.Unknown);
								if (messageType != MessageType.Unknown)
									if (currentEntry.MessageType != MessageType.Unknown && currentEntry.MessageType != messageType)
										trace.WriteLine(TraceEventType.Error, "Message type mismatch for \"{0}\" in module \"{1}\" detected in {2}!",
											currentKey, moduleName, GetFileInfo(input));
									else
										currentEntry.MessageType = messageType;
								else
									trace.WriteLine(TraceEventType.Error, "Invalid message type \"{0}\" for \"{1}\" in module \"{2}\" ({3})!",
										attribute6, currentKey, moduleName, GetFileInfo(input));
							}
						}
					}

					break;
				case ReaderState.Text:
					if (currentEntry != null)
					{
						string attribute = input.GetAttribute("xml:lang");
						if (!string.IsNullOrEmpty(attribute)) currentLanguage = attribute;
					}

					break;
			}
		}

		private void ProcessEndElement(XmlTextReader input, ReaderState state)
		{
			switch (state)
			{
				case ReaderState.Resources:
					defaultLanguage = null;
					break;
				case ReaderState.Module:
					inModule = false;
					break;
				case ReaderState.MessageElement:
				case ReaderState.UiTextElement:
					if (currentEntry != null && currentEntry.IsEmpty)
					{
						resultMap.RemoveTextEntry(currentKey);
						trace.WriteLine(TraceEventType.Warning, "Empty element \"{0}\" removed in {1}!", currentKey, GetFileInfo(input));
					}

					currentEntry = null;
					currentKey = null;
					break;
				case ReaderState.Text:
					currentLanguage = null;
					break;
			}
		}

		private void ProcessText(XmlTextReader input, ReaderState state)
		{
			if (state == ReaderState.Text && currentEntry != null)
			{
				string text = currentLanguage;
				if (text == null) text = defaultLanguage;
				if (text != null)
				{
					int languageFilterIndex = resultMap.GetLanguageFilterIndex(text);
					if (languageFilterIndex >= 0)
					{
						bool flag = true;
						if (currentEntry.IsEmpty)
						{
							flag = currentEntry.SetText(input.Value, languageFilterIndex);
							newEntries++;
						}
						else if (languageFilterIndex < currentEntry.LanguageCodeIndex)
						{
							flag = currentEntry.SetText(input.Value, languageFilterIndex);
							replacedEntries++;
						}

						if (!flag)
							trace.WriteLine(TraceEventType.Warning,
								"KxrReader: Error in parameter conversion of text \"{0}\"! (Key=\"{1}\", {2})", input.Value, currentKey,
								GetFileInfo(input));
					}
				}
				else
				{
					trace.WriteLine(TraceEventType.Error, "Cannot get language for text element! ({0})", GetFileInfo(input));
				}
			}
		}

		private void TraceErrorAttributeMissing(XmlTextReader input, string attrName)
		{
			trace.WriteLine(TraceEventType.Error, "KxrReader: Attribute \"{0}\" is missing in element \"{1}\"! ({2})", attrName,
				input.Name, GetFileInfo(input));
		}

		private string GetFileInfo(XmlTextReader input)
		{
			return $"File=\"{source}\" Line={input.LineNumber} Column={input.LinePosition}";
		}

		private enum ReaderState
		{
			Initial,
			Resources,
			Module,
			MessageElement,
			UiTextElement,
			Text,
			UnknownElement
		}


		#region · Fields ·

		private const string xmlAttributeKey = "key";

		private const string xmlAttributeName = "name";

		private const string xmlAttributeShortcut = "shortcut";

		private const string xmlAttributeNumber = "number";

		private const string xmlAttributeType = "type";

		private const string xmlAttributeLang = "xml:lang";

		private readonly PrettyTraceSource trace = TraceSourceFactory.GetSource(PredefinedTraceSource.Resources);

		private readonly FileSource source;

		private static Dictionary<string, ReaderState> elementMap;

		private string moduleName;

		private TextEntryMap resultMap;

		private bool inModule;

		private bool moduleFound;

		private string moduleShortcut;

		private string currentKey;

		private TextEntry currentEntry;

		private string defaultLanguage;

		private string currentLanguage;

		private int newEntries;

		private int replacedEntries;

		#endregion
	}
}