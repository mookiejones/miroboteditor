using System;

namespace InlineFormParser.Model
{
	[Flags]
	public enum MessageBoxButtons
	{
		None = 0,
		Ok = 1,
		Yes = 2,
		No = 4,
		Cancel = 8
	}
}