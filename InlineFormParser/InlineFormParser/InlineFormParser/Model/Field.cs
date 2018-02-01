#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:7:35 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;
using InlineFormParser.Common.LockHandling;
using InlineFormParser.Common.ResourceAccess;

#endregion

namespace InlineFormParser.Model
{
	public abstract class Field : ModelElement, IVisualGroup, IField
	{
		private Field[] fields;

		private InputState inputState;

		private string label;

		private string lockConditionResourceKey;

		private SimpleLockCondition simpleLockCondition;

		private string unit;

		private List<Field> visibleFields;

		[XmlAttribute("LockConditionResourceKey")]
		[Description(
			"Resource key for the lock popup text. The field cannot be locked as long as this attribute remains empty.")]
		public string LockConditionResourceKey
		{
			get => lockConditionResourceKey;
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					lockConditionResourceKey = value;
					UseLockMechanism = true;
				}
			}
		}

		[Description(
			"True if the lock mechanism is to be used, false otherwise. Remains false until a LockConditionResourceKey has been entered")]
		[XmlAttribute("UseLockMechanism")]
		public bool UseLockMechanism { get; set; }

		[XmlIgnore]
		public bool ShowLabel
		{
			get
			{
				if (!string.IsNullOrEmpty(label)) return !DisplayLabelInternally;
				return false;
			}
		}

		[XmlIgnore]
		public bool ShowUnit => !string.IsNullOrEmpty(unit);

		[XmlIgnore]
		public string DisplayedUnit
		{
			get
			{
				if (string.IsNullOrEmpty(unit)) return string.Empty;
				return $"[{unit}]";
			}
		}

		[XmlIgnore]
		public string AutomationId
		{
			get
			{
				if (string.IsNullOrEmpty(Key)) return ChildIndex.ToString(CultureInfo.InvariantCulture);
				return Key;
			}
		}

		[XmlIgnore]
		public SimpleLockCondition LockCondition
		{
			get
			{
				if (simpleLockCondition == null)
					if (RootInlineForm == null)
						simpleLockCondition = new SimpleLockCondition(x => EnableInput,
							new FrameworkResourceAccessor(RootParameterList.KxrModule), LockConditionResourceKey);
					else
						simpleLockCondition = new SimpleLockCondition(x => EnableInput,
							new FrameworkResourceAccessor(RootInlineForm.KxrModule), LockConditionResourceKey);
				return simpleLockCondition;
			}
		}

		[XmlIgnore]
		public Group ParentGroup => Parent as Group;

		[XmlIgnore]
		public bool IsInputValid => inputState == InputState.Valid;

		[XmlIgnore]
		public string TPString => $"{FieldId}:{TPValueString}";

		[XmlIgnore]
		public virtual bool DisregardValidation => false;

		[XmlIgnore]
		public virtual string FieldInfoText => FormatFieldInfo(null);

		[XmlIgnore]
		public virtual string InvalidInputMessageText
		{
			get
			{
				if (inputState != InputState.Valid)
				{
					IResourceAccessor resolver = Root.Resolver;
					if (string.IsNullOrEmpty(label)) return resolver.GetString(this, "MsgInvalidInputDefaultNoLabel");
					return resolver.GetString(this, "MsgInvalidInputDefault", label);
				}

				return string.Empty;
			}
		}

		[XmlIgnore]
		public virtual string RestoreCurrentValueHint => string.Empty;

		[XmlIgnore]
		public abstract bool IsModified { get; }

		[XmlIgnore]
		public virtual bool DisplayLabelInternally => false;

		[XmlIgnore]
		public virtual PopupKeyboardTypes KeyboardType
		{
			get
			{
				PopupKeyboardTypes popupKeyboardTypes = PopupKeyboardTypes.Default;
				if (Root is InlineFormBase) popupKeyboardTypes |= PopupKeyboardTypes.CommonOptionObserveBounds;
				return popupKeyboardTypes;
			}
		}

		[XmlIgnore]
		protected InputState CurrentInputState
		{
			get => inputState;
			set
			{
				if (value != inputState)
				{
					inputState = value;
					OnInputStateChanged();
				}
			}
		}

		[Description("The label of the field displayed left of the input")]
		[XmlAttribute("Label")]
		public string Label
		{
			get => label;
			set => label = value;
		}

		[Description("The unit of the field value displayed right of the input")]
		[XmlAttribute("Unit")]
		public string Unit
		{
			get => unit;
			set => unit = value;
		}

		[XmlAttribute("EnableInput")]
		[Description("True if input in the field is enabled")]
		[DefaultValue(true)]
		public bool EnableInput { get; set; } = true;

		[Description("The unique key of this field")]
		[XmlAttribute("Key")]
		public string Key { get; set; }

		[XmlIgnore]
		public string FieldId { get; private set; } = string.Empty;

		[XmlIgnore]
		public abstract string TPValueString { get; }

		[Description("True if the field is visible")]
		[XmlAttribute("Visible")]
		[DefaultValue(true)]
		public virtual bool IsVisible { get; set; } = true;

		[XmlIgnore]
		public bool HasFields => true;

		[XmlIgnore]
		public Field[] Fields
		{
			get
			{
				if (fields == null)
					fields = new Field[1]
					{
						this
					};
				return fields;
			}
		}

		[XmlIgnore]
		public List<Field> VisibleFields
		{
			get
			{
				if (visibleFields == null)
				{
					visibleFields = new List<Field>(1);
					if (IsVisible) visibleFields.Add(this);
				}

				return visibleFields;
			}
		}

		[XmlIgnore]
		public bool IsSelected
		{
			get
			{
				VisualGroup visualGroup = Parent as VisualGroup;
				if (visualGroup != null) return visualGroup.IsSelected;
				if (ChildIndex < 0) return false;
				InlineFormBase inlineFormBase = Parent as InlineFormBase;
				if (inlineFormBase != null) return inlineFormBase.CurrentFieldIndex == ChildIndex;
				return false;
			}
		}

		[XmlIgnore]
		public virtual bool CanBeCurrent
		{
			get
			{
				if (IsVisible) return EnableInput;
				return false;
			}
		}

		public void UpdateIsSelected()
		{
			FirePropertyChanged("IsSelected");
		}

		public event EventHandler Changed;

		public abstract void RestoreInitialValue();

		public virtual void RestoreCurrentValue()
		{
		}

		protected virtual void OnInputStateChanged()
		{
			FirePropertyChanged("IsInputValid", "InvalidInputMessageText");
		}

		public virtual void Select()
		{
			if (CanBeCurrent) RootInlineForm.CurrentFieldIndex = ChildIndex;
		}

		internal override void OnCreated(ModelElement parent, int childIndex)
		{
			base.OnCreated(parent, childIndex);
			if (parent is Group && string.IsNullOrEmpty(label))
				throw new InvalidOperationException(
					$"Label for field #{ChildIndex + 1} in group \"{ParentGroup.Title}\" is missing or empty.");
			NormalizeString(ref label);
			NormalizeString(ref unit);
			if (ChildIndex >= 0) BuildFieldId();
			inputState = InputState.Valid;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(base.ToString());
			Group parentGroup = ParentGroup;
			if (parentGroup != null)
			{
				stringBuilder.Append(' ');
				stringBuilder.Append(parentGroup);
			}

			return stringBuilder.ToString();
		}

		private void BuildFieldId()
		{
			FieldId = Parent is Group ? ParentGroup.GroupId + ":" : string.Empty;
			FieldIdType fieldIdentification = Root.FieldIdentification;
			switch (fieldIdentification)
			{
				case FieldIdType.Unknown:
					throw new ArgumentException("Unexpected field identification type \"Unknown\"!");
				case FieldIdType.Index:
					FieldId += (ChildIndex + 1).ToString(CultureInfo.InvariantCulture);
					break;
				case FieldIdType.Key:
					if (string.IsNullOrEmpty(Key))
						throw new ArgumentException($"Required attribute \"Key\" is missing in field \"{ToString()}\".");
					FieldId += Key;
					break;
				default:
					throw new ArgumentException($"Unknown field identification type \"{fieldIdentification}\"!");
			}
		}

		protected bool CheckFieldValueChanging(string newValueString)
		{
			try
			{
				if (Changed != null) Changed(this, EventArgs.Empty);
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.ToString());
			}

			return Root.FireFieldValueChanging(this, newValueString);
		}

		protected string FormatFieldInfo(string valueString)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(!string.IsNullOrEmpty(label) ? label : Root.Resolver.GetString(this, "CurrentValue"));
			stringBuilder.Append("=");
			if (valueString != null) stringBuilder.Append(valueString);
			if (!string.IsNullOrEmpty(unit))
			{
				stringBuilder.Append(' ');
				stringBuilder.Append(DisplayedUnit);
			}

			return stringBuilder.ToString();
		}

		protected enum InputState
		{
			Unknown,
			Valid,
			DoesNotMatchInputPattern,
			CantParse,
			OutOfRange,
			TooLong,
			RefusedByChangingEvent
		}
	}
}