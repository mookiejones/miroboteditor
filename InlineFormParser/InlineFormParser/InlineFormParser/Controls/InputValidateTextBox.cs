using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace InlineFormParser.Controls
{
	
public class InputValidateTextBox : TextBox
{
	[Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
	public enum PredefinedInputPatternEnum
	{
		None,
		TextOnlyLowerCase,
		TextOnlyUpperCase,
		TextOnly,
		Numeric,
		Numeric2Decimals,
		NumericInt,
		NumericUInt
	}

	private long textChangedCount;

	private string oldText = string.Empty;

	private static IDictionary<PredefinedInputPatternEnum, string> predefinedPatterns = new Dictionary<PredefinedInputPatternEnum, string>
	{
		{
			PredefinedInputPatternEnum.TextOnlyLowerCase,
			"^(\\s*[a-z]*)*$"
		},
		{
			PredefinedInputPatternEnum.TextOnlyUpperCase,
			"^(\\s*[A-Z]*)*$"
		},
		{
			PredefinedInputPatternEnum.TextOnly,
			"^(\\s*[a-zA-Z]*)*$"
		},
		{
			PredefinedInputPatternEnum.Numeric,
			"^-?[0-9]*\\.?[0-9]*$"
		},
		{
			PredefinedInputPatternEnum.Numeric2Decimals,
			"^-?[0-9]*\\.?[0-9]{0,2}?$"
		},
		{
			PredefinedInputPatternEnum.NumericInt,
			"^(-?[0-9]*)$"
		},
		{
			PredefinedInputPatternEnum.NumericUInt,
			"^([0-9]*)$"
		}
	};

	public static DependencyProperty InputPatternProperty = DependencyProperty.Register("InputPattern", typeof(string), typeof(InputValidateTextBox));

	public static DependencyProperty PredefinedInputPatternProperty = DependencyProperty.Register("PredefinedInputPattern", typeof(PredefinedInputPatternEnum), typeof(InputValidateTextBox));

	public string InputPattern
	{
		get
		{
			return (string)base.GetValue(InputValidateTextBox.InputPatternProperty);
		}
		set
		{
			base.SetValue(InputValidateTextBox.InputPatternProperty, value);
		}
	}

	public PredefinedInputPatternEnum PredefinedInputPattern
	{
		get
		{
			return (PredefinedInputPatternEnum)base.GetValue(InputValidateTextBox.PredefinedInputPatternProperty);
		}
		set
		{
			base.SetValue(InputValidateTextBox.PredefinedInputPatternProperty, value);
		}
	}

	protected virtual bool ValidateInput(string pattern)
	{
		return Regex.IsMatch(base.Text, pattern, RegexOptions.Compiled | RegexOptions.CultureInvariant);
	}

	protected override void OnTextChanged(TextChangedEventArgs e)
	{
		base.OnTextChanged(e);
		try
		{
			long num;
			this.textChangedCount = (num = this.textChangedCount) + 1;
			if (num == 0)
			{
				if (string.IsNullOrEmpty(base.Text))
				{
					this.oldText = string.Empty;
				}
				else
				{
					string text = (this.PredefinedInputPattern != 0) ? InputValidateTextBox.predefinedPatterns[this.PredefinedInputPattern] : this.InputPattern;
					if (!string.IsNullOrEmpty(text))
					{
						if (this.ValidateInput(text))
						{
							this.oldText = base.Text;
						}
						else
						{
							int caretIndex = base.CaretIndex;
							base.Text = this.oldText;
							base.CaretIndex = Math.Max(caretIndex - 1, 0);
						}
					}
				}
			}
		}
		finally
		{
			this.textChangedCount -= 1L;
		}
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		Key key = e.Key;
		if (key == Key.Return)
		{
			BindingExpression bindingExpression = base.GetBindingExpression(TextBox.TextProperty);
			if (bindingExpression != null)
			{
				bindingExpression.UpdateSource();
			}
			e.Handled = true;
		}
		else
		{
			base.OnKeyDown(e);
		}
	}
}
}
