#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:7:52 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

#endregion

namespace InlineFormParser.Model
{
	public abstract class NumericFieldBase<T> : ValueFieldBase<T>, ICanIncDec where T : IEquatable<T>, IComparable<T>
	{
		protected NumericFieldBase(T defaultStep)
		{
			Step = defaultStep;
		}


		[Description("The minimal field value (inclusive)")]
		[XmlAttribute("Min")]
		public T Minimum { get; set; }


		[XmlAttribute("Max")]
		[Description("The maximal field value (inclusive)")]
		public T Maximum { get; set; }


		[Description("The step for Inc/Dec buttons")]
		[XmlAttribute("Step")]
		public T Step { get; set; }


		[DefaultValue(true)]
		[XmlAttribute("Slider")]
		[Description("True if the value shall have a graphical visualisation")]
		public bool HasSlider { get; set; }

		[XmlIgnore]
		public bool ShowSlider => HasSlider && IsInputValid;

		[XmlIgnore]
		public override PopupKeyboardTypes KeyboardType => (base.KeyboardType & ~PopupKeyboardTypes.MaskType) |
		                                                   PopupKeyboardTypes.Numeric |
		                                                   PopupKeyboardTypes.NumericOptionUpDownEnabled;

		[XmlIgnore]
		public override string InvalidInputMessageText
		{
			get
			{
				switch (CurrentInputState)
				{
					case InputState.CantParse:
						if (string.IsNullOrEmpty(Label)) return Root.Resolver.GetString(this, "MsgInvalidInputCantParseNumericNoLabel");
						return Root.Resolver.GetString(this, "MsgInvalidInputCantParseNumeric", Label);
					case InputState.OutOfRange:
						if (string.IsNullOrEmpty(Label))
							return Root.Resolver.GetString(this, "MsgInvalidInputOutOfRangeNoLabel", FormatValue(Minimum),
								FormatValue(Maximum));
						return Root.Resolver.GetString(this, "MsgInvalidInputOutOfRange", Label, FormatValue(Minimum),
							FormatValue(Maximum));
					default:
						return base.InvalidInputMessageText;
				}
			}
		}

		public override string FieldInfoText
		{
			get
			{
				var stringBuilder = new StringBuilder(base.FieldInfoText);
				if (stringBuilder.Length > 0) stringBuilder.AppendLine();
				stringBuilder.Append(Root.Resolver.GetString(this, "FrmValueRange", FormatValue(Minimum), FormatValue(Maximum)));
				if (ShowUnit)
				{
					stringBuilder.Append(' ');
					stringBuilder.Append(DisplayedUnit);
				}

				return stringBuilder.ToString();
			}
		}

		[XmlIgnore]
		public bool CanIncrement
		{
			get
			{
				if (EnableInput && IsInputValid) return CurrentValue.CompareTo(Maximum) < 0;
				return false;
			}
		}

		[XmlIgnore]
		public bool CanDecrement
		{
			get
			{
				if (EnableInput && IsInputValid) return CurrentValue.CompareTo(Minimum) > 0;
				return false;
			}
		}

		public abstract void Increment();

		public abstract void Decrement();

		protected override InputState ValidateInput(T value)
		{
			if (value.CompareTo(Minimum) >= 0 && value.CompareTo(Maximum) <= 0) return base.ValidateInput(value);
			return InputState.OutOfRange;
		}

		protected override void OnCurrentValueChanged()
		{
			base.OnCurrentValueChanged();
			FirePropertyChanged("CanIncrement", "CanDecrement");
		}

		protected override void OnInputStateChanged()
		{
			base.OnInputStateChanged();
			FirePropertyChanged("CanIncrement", "CanDecrement", "ShowSlider");
		}

		internal override void OnCreated(ModelElement parent, int childIndex)
		{
			if (Minimum.CompareTo(Maximum) > 0) throw new ArgumentOutOfRangeException("Min/Max");
			base.OnCreated(parent, childIndex);
		}
	}
}