using System;

namespace InlineFormParser.Model
{
	public class DisplayEventArgs : EventArgs
	{
		public DisplaySource DisplaySource { get; }

		public DisplayEventArgs(DisplaySource displaySource)
		{
			if (displaySource == (DisplaySource)null)
			{
				throw new ArgumentNullException("display");
			}
			DisplaySource = displaySource;
		}
	}
}