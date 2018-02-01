using System.Windows;
using System.Collections.Generic;
namespace InlineFormParser.Model
{
	public interface IPopupManager : IFloatingDisplayProvider
	{
		string OpenPopupName
		{
			get;
		}

		IInputControl OpenPopupInputControl
		{
			get;
		}

		object OpenPopupCallerData
		{
			get;
		}

		bool IsPopupAvailable(string name);

		void OpenPopup(string name, IInputControl inputCtrl, IDictionary<string, object> parameterMap, PopupCallback callback, object callerData);

		void OpenPopupKeyboard(PopupKeyboardTypes type, IInputControl editCtrl, IDictionary<string, object> parameterMap, PopupCallback callback, object callerData);

		void OpenListSelectorPopup(IInputControl relatedCtrl, object[] items, int selectedItemIndex, IDictionary<string, object> parameterMap, PopupCallback callback, object callerData);

		void OpenTooltipPopup(IInputControl relatedCtrl, string tooltipText, bool observeBounds, PopupCallback callback, object callerData);

		void OpenTooltipPopup(Rect releatedArea, string tooltipText, PopupCallback callback, object callerData);
	}
}