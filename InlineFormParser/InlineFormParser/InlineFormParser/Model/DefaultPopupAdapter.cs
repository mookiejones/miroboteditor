using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace InlineFormParser.Model
{
	public class DefaultPopupAdapter : PopupAdapter
	{
		public DefaultPopupAdapter(Control control, IServiceProvider serviceProvider)
			: base(control, serviceProvider)
		{
		}

		protected override IInputControl DetectEditControl(Point screenPosition)
		{
			if (IsAnyDropDownListOpen(Control))
			{
				return null;
			}
			Point point = Control.PointFromScreen(screenPosition);
			IInputElement inputElement = Control.InputHitTest(point);
			DependencyObject dependencyObject = inputElement as DependencyObject;
			while (dependencyObject != null && dependencyObject != Control)
			{
				TextBoxBase textBoxBase = dependencyObject as TextBoxBase;
				if (textBoxBase != null && textBoxBase.IsVisible && textBoxBase.IsEnabled && !textBoxBase.IsReadOnly)
				{
					return new WPFInputControl(textBoxBase);
				}
				PasswordBox passwordBox = dependencyObject as PasswordBox;
				if (passwordBox != null && passwordBox.IsVisible && passwordBox.IsEnabled)
				{
					return new WPFInputControl(passwordBox);
				}
				dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
			}
			return null;
		}

		public override void OpenPopup(IInputControl editControl)
		{
			OpenPopupKeyboard(editControl, OnPopupEvent, KeyboardToken);
		}

		protected virtual void OpenPopupKeyboard(IInputControl editControl, PopupCallback callback, object callerData)
		{
			PopupManager.OpenPopupKeyboard(PopupKeyboardTypes.MaskType, editControl, null, callback, callerData);
		}
	}
}