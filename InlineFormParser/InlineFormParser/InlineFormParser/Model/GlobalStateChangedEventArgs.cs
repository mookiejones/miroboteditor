using System;

namespace InlineFormParser.Model
{
	public class GlobalStateChangedEventArgs : EventArgs
	{
		public IGlobalState State { get; }

		public GlobalStateChangedEventArgs(IGlobalState state)
		{
			State = state ?? throw new ArgumentNullException(nameof(state));
		}
	}
}