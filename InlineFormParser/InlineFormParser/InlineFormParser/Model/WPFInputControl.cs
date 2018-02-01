using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Markup;

namespace InlineFormParser.Model
{
	
public class WPFInputControl : IInputControl, IEquatable<IInputControl>
{
	private Rect knownBounds;

	public Rect Bounds
	{
		get
		{
			try
			{
				Point location = Control.PointToScreen(default(Point));
				return new Rect(location, Control.RenderSize);
			}
			catch (InvalidOperationException)
			{
				return default(Rect);
			}
		}
	}

	public bool HasFocus => Control.IsMouseOver;

	public bool IsEnabledAndVisible
	{
		get
		{
			if (Control.IsEnabled)
			{
				return Control.IsVisible;
			}
			return false;
		}
	}

	public bool IsSuitableEditControl
	{
		get
		{
			if (!(Control is TextBoxBase) && !(Control is ListBoxItem) && !(Control is PasswordBox) && !(Control is ButtonBase) && !(Control is RangeBase) && !(Control is Selector) && !(Control is HeaderedContentControl)  )
			{
				return false;
			}
			return IsEnabledAndVisible;
		}
	}

	public string Text
	{
		get
		{
			if (IsSuitableEditControl)
			{
				TextBox textBox = Control as TextBox;
				if (textBox != null)
				{
					return textBox.Text;
				}
				PasswordBox passwordBox = Control as PasswordBox;
				if (passwordBox != null)
				{
					return passwordBox.Password;
				}
			}
			return null;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("Text");
			}
			if (IsSuitableEditControl)
			{
				TextBox textBox = Control as TextBox;
				if (textBox != null)
				{
					textBox.Text = value;
				}
				PasswordBox passwordBox = Control as PasswordBox;
				if (passwordBox != null)
				{
					passwordBox.Password = value;
				}
			}
		}
	}

	public UIElement Control { get; }

	public event EventHandler BoundsChanged;

	public WPFInputControl(UIElement control)
	{
		Control = control ?? throw new ArgumentNullException(nameof(control));
		Control.LayoutUpdated += OnControlLayoutUpdated;
		knownBounds = Bounds;
	}

	public void Focus()
	{
		if (!Control.IsFocused)
		{
			Control.Focus();
		}
	}

	public void SelectAllText()
	{
		if (IsSuitableEditControl)
		{
			TextBoxBase textBoxBase = Control as TextBoxBase;
			if (textBoxBase != null)
			{
				textBoxBase.SelectAll();
			}
			else
			{
				PasswordBox passwordBox = Control as PasswordBox;
				if (passwordBox != null)
				{
					passwordBox.SelectAll();
				}
			}
		}
	}

	public PopupKeyboardTypes DetectKeyboardType()
	{
		if (!(Control is TextBoxBase) && !(Control is PasswordBox))
		{
			return PopupKeyboardTypes.NoKeyboard;
		}
		FrameworkElement frameworkElement = Control as FrameworkElement;
		PopupKeyboardTypes popupKeyboardTypes = PopupKeyboardTypes.Default;
		if (frameworkElement != null)
		{
			string value = frameworkElement.Tag as string;
			if (!string.IsNullOrEmpty(value))
			{
				try
				{
					popupKeyboardTypes = (PopupKeyboardTypes)Enum.Parse(typeof(PopupKeyboardTypes), value);
				}
				catch (ArgumentException)
				{
				}
			}
		}
		if (!IsElementValidatable(frameworkElement))
		{
			return popupKeyboardTypes;
		}
		return popupKeyboardTypes |= PopupKeyboardTypes.CommonOptionObserveBounds;
	}

	private bool IsElementValidatable(FrameworkElement element)
	{
		try
		{
			DependencyProperty dependencyProperty = null;
			ContentPropertyAttribute contentPropertyAttribute = TypeDescriptor.GetAttributes(element.GetType()).OfType<ContentPropertyAttribute>().FirstOrDefault();
			if (contentPropertyAttribute != null && !string.IsNullOrEmpty(contentPropertyAttribute.Name))
			{
				goto IL_0049;
			}
//			if (element is PasswordBox && PasswordBoxAssistant.GetAttached(element as PasswordBox))
//			{
//				dependencyProperty = PasswordBoxAssistant.PasswordProperty;
//				goto IL_0049;
//			}
			return false;
			IL_0049:
			if (dependencyProperty == null)
			{
				dependencyProperty = DependencyPropertyDescriptor.FromName(contentPropertyAttribute.Name, element.GetType(), element.GetType()).DependencyProperty;
			}
			return BindingOperations.GetBinding(element, dependencyProperty) != null && BindingOperations.GetBinding(element, dependencyProperty).ValidatesOnDataErrors;
		}
		catch
		{
			return false;
		}
	}

	public bool Equals(IInputControl other)
	{
		WPFInputControl wPFInputControl = other as WPFInputControl;
		if (wPFInputControl != null)
		{
			return Control == wPFInputControl.Control;
		}
		return false;
	}

	private void OnControlLayoutUpdated(object sender, EventArgs e)
	{
		if (BoundsChanged != null)
		{
			Rect bounds = Bounds;
			if (!knownBounds.Equals(bounds))
			{
				knownBounds = bounds;
				BoundsChanged(this, EventArgs.Empty);
			}
		}
	}
}

}