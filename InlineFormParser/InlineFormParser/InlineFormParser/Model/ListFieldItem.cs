#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:8:12 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

#endregion

namespace InlineFormParser.Model
{
	public class ListFieldItem : ModelElement, IListFieldItem
	{
		private string displayString;

		[XmlIgnore]
		public string ItemId { get; private set; }

		[XmlIgnore]
		public ListField ParentListField => (ListField) Parent;

		[XmlAttribute("Disp")]
		[Description("The displayed textual representation of this item")]
		public string DisplayString
		{
			get => displayString;
			set => displayString = value;
		}

		[XmlAttribute("Key")]
		[Description("The unique key of this item")]
		public string Key { get; set; }

		internal override void OnCreated(ModelElement parent, int childIndex)
		{
			base.OnCreated(parent, childIndex);
			NormalizeString(ref displayString);
			BuildItemId();
		}

		private void BuildItemId()
		{
			ListItemIdType listItemIdType = ParentListField.ListItemIdentification;
			if (listItemIdType == ListItemIdType.Unknown) listItemIdType = Root.DefaultListItemIdentification;
			switch (listItemIdType)
			{
				case ListItemIdType.Unknown:
					throw new ArgumentException("Unexpected list item identification type \"Unknown\"!");
				case ListItemIdType.Index:
					ItemId = (ChildIndex + 1).ToString(CultureInfo.InvariantCulture);
					break;
				case ListItemIdType.Key:
					if (string.IsNullOrEmpty(Key)) throw new ArgumentException("Required attribute \"Key\" is missing in list item.");
					ItemId = Key;
					break;
				case ListItemIdType.Disp:
				case ListItemIdType.QuotedDisp:
					if (string.IsNullOrEmpty(displayString))
						throw new ArgumentException("Required attribute \"Disp\" is missing in list item.");
					ItemId = listItemIdType == ListItemIdType.Disp ? displayString : $"\"{displayString}\"";
					break;
				default:
					throw new ArgumentException($"Unknown list item identification type \"{listItemIdType}\"!");
			}
		}
	}
}