#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:7:48 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System.Collections.Generic;
using System.Xml.Serialization;

#endregion

namespace InlineFormParser.Model
{
	public class VisualGroup : ModelElement, IVisualGroup
	{
		private int firstFieldIndex;
		private List<Field> visibleFields;

		public VisualGroup()
		{
		}

		public VisualGroup(Field[] fields)
		{
			Fields = fields;
		}

		[XmlElement(ElementName = "Bool", Type = typeof(BoolField))]
		[XmlElement(ElementName = "Number", Type = typeof(NumberField))]
		[XmlElement(ElementName = "Float", Type = typeof(FloatField))]
		[XmlElement(ElementName = "Name", Type = typeof(NameField))]
		[XmlElement(ElementName = "List", Type = typeof(ListField))]
		[XmlElement(ElementName = "Free", Type = typeof(FreeField))]
		[XmlElement(ElementName = "Static", Type = typeof(StaticField))]
		[XmlElement(ElementName = "ParamList", Type = typeof(ParamListField))]
		public Field[] Fields { get; set; }

		[XmlIgnore]
		public bool HasFields => Fields?.Length > 0;

		[XmlIgnore]
		public List<Field> VisibleFields
		{
			get
			{
				if (visibleFields == null)
				{
					visibleFields = new List<Field>();
					if (Fields != null)
					{
						Field[] array = Fields;
						foreach (Field field in array)
							if (field.IsVisible)
								visibleFields.Add(field);
					}
				}

				return visibleFields;
			}
		}

		[XmlIgnore]
		public bool IsVisible => VisibleFields.Count > 0;

		[XmlIgnore]
		public bool CanBeCurrent
		{
			get
			{
				foreach (Field visibleField in VisibleFields)
					if (visibleField.CanBeCurrent)
						return true;
				return false;
			}
		}

		[XmlIgnore]
		public bool IsSelected
		{
			get
			{
				if (Fields == null) return false;
				int currentFieldIndex = ((InlineFormBase) Parent).CurrentFieldIndex;
				if (currentFieldIndex >= firstFieldIndex) return currentFieldIndex < firstFieldIndex + Fields.Length;
				return false;
			}
		}

		public void UpdateIsSelected()
		{
			FirePropertyChanged("IsSelected");
		}

		internal override void OnCreated(ModelElement parent, int childIndex)
		{
			base.OnCreated(parent, -1);
			firstFieldIndex = childIndex;
			if (Fields != null)
				for (int i = 0; i < Fields.Length; i++)
					Fields[i].OnCreated(this, firstFieldIndex + i);
		}

		internal Field GetSelectableField(Field startField)
		{
			if (startField.CanBeCurrent) return startField;
			int num = startField.ChildIndex - firstFieldIndex;
			int num2 = num + 1;
			while (num2 < Fields.Length)
			{
				Field field = Fields[num2++];
				if (field.CanBeCurrent) return field;
			}

			num2 = num - 1;
			while (num2 >= 0)
			{
				Field field2 = Fields[num2--];
				if (field2.CanBeCurrent) return field2;
			}

			return null;
		}
	}
}