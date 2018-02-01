using System;
using System.Diagnostics.CodeAnalysis;

namespace InlineFormParser.Model
{
	[SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
	[Flags]
	public enum CommandStates
	{
		Nothing = 0,
		Enabled = 1,
		Checked = 2
	}
}