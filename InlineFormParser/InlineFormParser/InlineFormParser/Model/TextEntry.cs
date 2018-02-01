#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:8:35 AM
// Modified:2018:02:01:9:48 AM:

#endregion

namespace InlineFormParser.Model
{
	public class TextEntry
	{
		private string text;

		internal TextEntry()
		{
		}

		internal bool IsEmpty => LanguageCodeIndex < 0;

		internal string Text => text;

		internal int LanguageCodeIndex { get; private set; } = -1;

		internal int MessageNumber { get; set; }

		internal MessageType MessageType { get; set; } = MessageType.Unknown;

		internal bool SetText(string text, int langCodeIndex)
		{
			LanguageCodeIndex = langCodeIndex;
			var input = Helpers.NormalizeWhitespaces(text);
			return Helpers.TryReplaceParameters(input, out this.text);
		}
	}
}