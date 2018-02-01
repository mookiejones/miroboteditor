using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace InlineFormParser.Model
{
	public class FieldPopupKeyboardAdapter : TouchableDevicePopupAdapter
	{
		private IViewManager viewManager;

		public FieldPopupKeyboardAdapter(Control control, IServiceProvider serviceProvider)
			: base(control, serviceProvider)
		{
			viewManager = (serviceProvider.GetService(typeof(IViewManager)) as IViewManager);
			if (viewManager != null)
			{
				return;
			}
			throw new ServiceNotFoundException("IViewManager");
		}

		public void ProcessTextBoxKey(TextBox textBox, KeyEventArgs e)
		{
			if (textBox == null)
			{
				throw new ArgumentNullException(nameof(textBox));
			}
			if (e == null)
			{
				throw new ArgumentNullException(nameof(e));
			}
			if (!e.Handled && e.IsDown)
			{
				Field field = textBox.DataContext as Field;
				ICanIncDec canIncDec = textBox.DataContext as ICanIncDec;
				switch (e.Key)
				{
					case Key.Space:
						e.Handled = true;
						break;
					case Key.Up:
						if (canIncDec != null && canIncDec.CanIncrement)
						{
							canIncDec.Increment();
						}
						e.Handled = true;
						break;
					case Key.Down:
						if (canIncDec != null && canIncDec.CanDecrement)
						{
							canIncDec.Decrement();
						}
						e.Handled = true;
						break;
					case Key.Return:
						textBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
						e.Handled = true;
						if (field != null)
						{
							SetPopupCloseOnKeyMode(field);
						}
						break;
					case Key.Escape:
						if (field != null)
						{
							PopupManager.CloseAnyPopup();
							field.RestoreCurrentValue();
							e.Handled = true;
						}
						break;
				}
			}
		}

		protected override void OpenPopupKeyboard(IInputControl editControl, PopupCallback callback, object callerData)
		{
			FrameworkElement frameworkElement = (FrameworkElement)((WPFInputControl)editControl).Control;
			Field field = frameworkElement.DataContext as Field;
			if (LockManager.GetIsLocked(frameworkElement))
			{
				PopupManager.OpenPopup("LockInfoPopup", editControl, null, OnPopupEvent, KeyboardToken);
			}
			else
			{
				if (!(((WPFInputControl)editControl).Control is TextBoxBase) && !(((WPFInputControl)editControl).Control is PasswordBox))
				{
					return;
				}
				if (field != null)
				{
					Dictionary<string, object> dictionary = new Dictionary<string, object>();
					dictionary["InfoLine"] = new FieldInfoControl(field);
					PopupManager.OpenPopupKeyboard(field.KeyboardType, editControl, dictionary, callback, callerData);
				}
				else
				{
					base.OpenPopupKeyboard(editControl, callback, callerData);
				}
			}
		}

		private void SetPopupCloseOnKeyMode(Field field)
		{
			string openPopupName = PopupManager.OpenPopupName;
			if (!string.IsNullOrEmpty(openPopupName))
			{
				IFloatingDisplay floatingDisplay = viewManager.GetOpenDisplay(openPopupName) as IFloatingDisplay;
				if (floatingDisplay != null)
				{
					if (field.IsInputValid)
					{
						floatingDisplay.CloseOnKeyMode |= ClosingKeys.Return;
					}
					else
					{
						floatingDisplay.CloseOnKeyMode &= ~ClosingKeys.Return;
					}
				}
			}
		}
	}
}