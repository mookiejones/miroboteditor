using System;
using System.Windows;

namespace InlineFormParser.Model
{
	public interface IInputControl : IEquatable<IInputControl>
	{
		Rect Bounds
		{
			get;
		}

		bool HasFocus
		{
			get;
		}

		bool IsEnabledAndVisible
		{
			get;
		}

		bool IsSuitableEditControl
		{
			get;
		}

		string Text
		{
			get;
			set;
		}

		event EventHandler BoundsChanged;

		void Focus();

		void SelectAllText();

		PopupKeyboardTypes DetectKeyboardType();
	}
}