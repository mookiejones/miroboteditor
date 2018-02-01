#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:7:50 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

#endregion

namespace InlineFormParser.Model
{
	public abstract class ValueFieldBase<T> : Field, IValueField<T> where T : IEquatable<T>
	{
		private T currentValue;
		protected T initialValue;

		private string invalidInput;

		private Regex regExInputPattern;

		protected ValueFieldBase()
		{
		}

		protected ValueFieldBase(T initialValue)
		{
			this.initialValue = initialValue;
		}

		[XmlAttribute("Value")]
		[Description("The initial field value")]
		public T InitialValue
		{
			get => initialValue;
			set => initialValue = value;
		}

		[XmlIgnore]
		public string ShownValue
		{
			get
			{
				if (!IsInputValid) return invalidInput;
				return FormatValue(CurrentValue);
			}
			set
			{
				var val = default(T);
				if (!DisregardValidation && !MatchesInputPattern(value))
					SetInputState(InputState.DoesNotMatchInputPattern, value);
				else if (!TryParseInput(value, out val))
					SetInputState(InputState.CantParse, value);
				else
					CurrentValue = val;
			}
		}

		[XmlIgnore]
		public virtual string InputPattern => "^.*$";

		public sealed override bool IsModified => !currentValue.Equals(initialValue);

		public sealed override string TPValueString => FormatValue(CurrentValue);

		[XmlIgnore]
		public override string FieldInfoText => FormatFieldInfo(FormatValue(CurrentValue));

		public override string RestoreCurrentValueHint =>
			Root.Resolver.GetString(this, "FrmRestoreValueHint", FormatValue(CurrentValue), DisplayedUnit);

		[XmlIgnore]
		public T CurrentValue
		{
			get => currentValue;
			set
			{
				var inputState = ValidateInput(value);
				if (inputState != InputState.Valid)
				{
					SetInputState(inputState, FormatValue(value));
				}
				else
				{
					var flag = !value.Equals(currentValue);
					var text = FormatValue(value);
					if (!flag || CheckFieldValueChanging(text))
					{
						SetInputState(InputState.Valid, string.Empty);
						if (flag)
						{
							currentValue = value;
							OnCurrentValueChanged();
						}
					}
					else
					{
						SetInputState(InputState.RefusedByChangingEvent, text);
					}
				}
			}
		}

		protected abstract string FormatValue(T value);

		protected abstract bool TryParseInput(string input, out T value);

		protected virtual InputState ValidateInput(T value)
		{
			return InputState.Valid;
		}

		protected virtual void OnCurrentValueChanged()
		{
			FirePropertyChanged("CurrentValue", "ShownValue", "IsModified", "RestoreCurrentValueHint");
		}

		protected override void OnInputStateChanged()
		{
			base.OnInputStateChanged();
			FirePropertyChanged("ShownValue");
		}

		public override void RestoreInitialValue()
		{
			CurrentValue = InitialValue;
		}

		public override void RestoreCurrentValue()
		{
			SetInputState(InputState.Valid, string.Empty);
			OnCurrentValueChanged();
		}

		internal override void OnCreated(ModelElement parent, int childIndex)
		{
			base.OnCreated(parent, childIndex);
			currentValue = initialValue;
			RestoreInitialValue();
		}

		private bool MatchesInputPattern(string input)
		{
			if (regExInputPattern == null) regExInputPattern = new Regex(InputPattern);
			return regExInputPattern.IsMatch(input);
		}

		private void SetInputState(InputState state, string input)
		{
			invalidInput = input;
			CurrentInputState = state;
		}
	}
}