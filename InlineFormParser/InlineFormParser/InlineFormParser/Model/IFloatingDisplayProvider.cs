using System;

namespace InlineFormParser.Model
{
	
	public interface IFloatingDisplayProvider
	{
		bool IsAnyPopupOpen
		{
			get;
		}

		IFloatingDisplay OpenPopupDisplay
		{
			get;
		}

		bool IsMousePositionInPopup
		{
			get;
		}

		event EventHandler<LowLevelMouseClickEventArgs> ContentAreaClicked;

		void CloseAnyPopup();
	}
}