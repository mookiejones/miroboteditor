using System;

namespace InlineFormParser.Model
{
	public class DisplayClosedItselfEventArgs : EventArgs
	{
		public LogicalDisplay Display { get; }

		public DisplayClosedItselfEventArgs(LogicalDisplay display)
		{
			Display = display ?? throw new ArgumentNullException(nameof(display));
		}
	}
}