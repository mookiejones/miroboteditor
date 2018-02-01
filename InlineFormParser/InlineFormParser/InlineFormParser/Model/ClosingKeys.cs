using System;

namespace InlineFormParser.Model
{
	[Flags]
	public enum ClosingKeys
	{
		None = 0,
		Escape = 1,
		Return = 2,
		Tab = 4,
		Default = 7
	}
}