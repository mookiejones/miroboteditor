#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:9:42 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Xml.Serialization;

#endregion

namespace InlineFormParser.Model
{
	public class Group : ModelElement, IGroup
	{
		private List<Field> visibleFields;

		[XmlElement(ElementName = "Bool", Type = typeof(BoolField))]
		[XmlElement(ElementName = "Number", Type = typeof(NumberField))]
		[XmlElement(ElementName = "Free", Type = typeof(FreeField))]
		[XmlElement(ElementName = "Float", Type = typeof(FloatField))]
		[XmlElement(ElementName = "List", Type = typeof(ListField))]
		[XmlElement(ElementName = "Static", Type = typeof(StaticField))]
		public Field[] Fields { get; set; }

		[XmlIgnore]
		public string GroupId { get; private set; }

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

		[Description("The group title")]
		[XmlAttribute("Title")]
		public string Title { get; set; }

		[XmlAttribute("Key")]
		[Description("The unique key of this group")]
		public string Key { get; set; }

		IField[] IGroup.Fields => Fields;

		internal override void OnCreated(ModelElement parent, int childIndex)
		{
			base.OnCreated(parent, childIndex);
			BuildGroupId();
			for (int i = 0; i < Fields.Length; i++) Fields[i].OnCreated(this, i);
			if (VisibleFields.Count == 0)
				RootElementBase.Trace.WriteLine(TraceEventType.Warning, "ParameterList Group \"{0}\" has no visible fields!",
					Title);
		}

		public override string ToString()
		{
			return $"Group#{ChildIndex + 1}";
		}

		private void BuildGroupId()
		{
			GroupIdType groupIdentification = RootParameterList.GroupIdentification;
			switch (groupIdentification)
			{
				case GroupIdType.Unknown:
					throw new ArgumentException("Unexpected group identification type \"Unknown\"!");
				case GroupIdType.Index:
					GroupId = (ChildIndex + 1).ToString(CultureInfo.InvariantCulture);
					break;
				case GroupIdType.Key:
					if (string.IsNullOrEmpty(Key)) throw new ArgumentException("Required attribute \"Key\" is missing in group.");
					GroupId = Key;
					break;
				case GroupIdType.Title:
				case GroupIdType.QuotedTitle:
					if (string.IsNullOrEmpty(Title))
						throw new ArgumentException(
							$"Required attribute \"Title\" is missing in group \"{ToString()}\".");
					GroupId = groupIdentification == GroupIdType.Title ? Title : $"\"{Title}\"";
					break;
				default:
					throw new ArgumentException($"Unknown group identification type \"{groupIdentification}\"!");
			}
		}
	}
}