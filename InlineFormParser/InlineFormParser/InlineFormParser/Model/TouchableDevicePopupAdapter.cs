using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace InlineFormParser.Model
{
	public class TouchableDevicePopupAdapter : DefaultPopupAdapter
	{
		public TouchableDevicePopupAdapter(Control control, IServiceProvider serviceProvider)
			: base(control, serviceProvider)
		{
		}

		protected override IInputControl DetectEditControl(Point screenPosition)
		{
			var inputControl = base.DetectEditControl(screenPosition);
			if (inputControl != null)
			{
				return inputControl;
			}
			var point = Control.PointFromScreen(screenPosition);
			var inputElement = Control.InputHitTest(point);
			var dependencyObject = inputElement as DependencyObject;
			while (dependencyObject != null && dependencyObject != Control)
			{
				var buttonBase = dependencyObject as ButtonBase;
				if (buttonBase != null && buttonBase.IsVisible && buttonBase.IsEnabled)
				{
					return new WPFInputControl(buttonBase);
				}
				var listBoxItem = dependencyObject as ListBoxItem;
				if (listBoxItem != null && listBoxItem.IsVisible && listBoxItem.IsEnabled)
				{
					return new WPFInputControl(listBoxItem);
				}
//				NumericUpDown numericUpDown = dependencyObject as NumericUpDown;
//				if (numericUpDown != null && numericUpDown.IsVisible && numericUpDown.IsEnabled)
//				{
//					return new WPFInputControl(numericUpDown);
//				}
				var slider = dependencyObject as Slider;
				if (slider != null && slider.IsVisible && slider.IsEnabled)
				{
					return new WPFInputControl(slider);
				}
				var comboBox = dependencyObject as ComboBox;
				if (comboBox != null && comboBox.IsVisible && comboBox.IsEnabled)
				{
					return new WPFInputControl(comboBox);
				}
				var listBox = dependencyObject as ListBox;
				if (listBox != null && listBox.IsVisible && listBox.IsEnabled)
				{
					return new WPFInputControl(listBox);
				}
				var tabControl = dependencyObject as TabControl;
				if (tabControl != null && tabControl.IsVisible && tabControl.IsEnabled)
				{
					return new WPFInputControl(tabControl);
				}
				var tabItem = dependencyObject as TabItem;
				if (tabItem != null && tabItem.IsVisible && tabItem.IsEnabled)
				{
					return new WPFInputControl(tabItem);
				}
				dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
			}
			return null;
		}

		protected override void OpenPopupKeyboard(IInputControl editControl, PopupCallback callback, object callerData)
		{
			var wPFInputControl = editControl as WPFInputControl;
			if (LockManager.GetIsLocked(wPFInputControl.Control))
			{
				PopupManager.OpenPopup("LockInfoPopup", editControl, null, OnPopupEvent, KeyboardToken);
			}
			else
			{
				var parameterMap = new Dictionary<string, object>();
				PopupManager.OpenPopupKeyboard(wPFInputControl.DetectKeyboardType(), editControl, parameterMap, OnPopupEvent, KeyboardToken);
			}
		}
	}
}