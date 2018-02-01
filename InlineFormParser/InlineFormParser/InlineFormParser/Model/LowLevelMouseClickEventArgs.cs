using System;
using System.Windows;
using System.Windows.Input;

namespace InlineFormParser.Model
{
	public class LowLevelMouseClickEventArgs : EventArgs
	{
		public MouseButton Button { get; }

		public MouseButtonState ButtonState { get; }

		public Point Position { get; }

		public LowLevelMouseClickEventArgs(MouseButton button, MouseButtonState state, Point position)
		{
			Button = button;
			ButtonState = state;
			Position = position;
		}
	}
}