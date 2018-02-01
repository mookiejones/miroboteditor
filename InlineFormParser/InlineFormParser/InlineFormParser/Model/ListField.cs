#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:8:11 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

#endregion

namespace InlineFormParser.Model
{
	public class ListField : Field, IListField, IField
	{
		private int currentSelectedIndex;

		private string[] displayedItems;
		private int initialSelectedIndex;

		[Description("The id of the initially selected item in the list")]
		[XmlAttribute("SelectedItem")]
		public string InitialSelectedId { get; set; }

		[XmlAttribute("ListItemIds")]
		[Description("The way how list items shall be identified in TP-strings")]
		public ListItemIdType ListItemIdentification { get; set; }

		[XmlElement("Item")]
		public ListFieldItem[] Items { get; set; }

		[XmlIgnore]
		public int CurrentSelectedIndex
		{
			get => currentSelectedIndex;
			set
			{
				bool flag = currentSelectedIndex != value;
				if (!flag || CheckFieldValueChanging(SelectedIndex2String(value)))
				{
					CurrentInputState = InputState.Valid;
					if (flag)
					{
						currentSelectedIndex = value;
						FirePropertyChanged("CurrentSelectedIndex", "ShownValue");
					}
				}
				else
				{
					CurrentInputState = InputState.RefusedByChangingEvent;
				}
			}
		}

		[XmlIgnore]
		public string ShownValue => DisplayedItems[currentSelectedIndex];

		[XmlIgnore]
		public string[] DisplayedItems
		{
			get
			{
				if (displayedItems == null)
				{
					displayedItems = new string[Items.Length];
					for (int i = 0; i < Items.Length; i++) displayedItems[i] = Items[i].DisplayString;
				}

				return displayedItems;
			}
		}

		[XmlIgnore]
		public override bool IsModified => currentSelectedIndex != initialSelectedIndex;

		public override string FieldInfoText => FormatFieldInfo(Items[currentSelectedIndex].DisplayString);

		[XmlIgnore]
		public override string TPValueString => SelectedIndex2String(currentSelectedIndex);

		public IListFieldItem SelectedItem => Items[currentSelectedIndex];

		IListFieldItem[] IListField.Items => Items;

		internal override void OnCreated(ModelElement parent, int childIndex)
		{
			base.OnCreated(parent, childIndex);
			for (int i = 0; i < Items.Length; i++) Items[i].OnCreated(this, i);
			CheckUniqueItemIds();
			ListFieldItem listFieldItem = FindItemById(InitialSelectedId);
			initialSelectedIndex = listFieldItem != null ? listFieldItem.ChildIndex : 0;
			currentSelectedIndex = initialSelectedIndex;
			RestoreInitialValue();
		}

		public override void RestoreInitialValue()
		{
			CurrentSelectedIndex = initialSelectedIndex;
		}

		private ListFieldItem FindItemById(string id)
		{
			if (!string.IsNullOrEmpty(id))
			{
				ListFieldItem[] array = Items;
				foreach (ListFieldItem listFieldItem in array)
					if (listFieldItem.ItemId == id)
						return listFieldItem;
			}

			return null;
		}

		private string SelectedIndex2String(int idx)
		{
			return Items[idx].ItemId;
		}

		private void CheckUniqueItemIds()
		{
			HashSet<string> hashSet = new HashSet<string>();
			ListFieldItem[] array = Items;
			int num = 0;
			ListFieldItem listFieldItem;
			while (true)
			{
				if (num < array.Length)
				{
					listFieldItem = array[num];
					if (!hashSet.Contains(listFieldItem.ItemId))
					{
						hashSet.Add(listFieldItem.ItemId);
						num++;
						continue;
					}

					break;
				}

				return;
			}

			throw new ArgumentException($"Duplicate list item id \"{listFieldItem.ItemId}\" in field \"{ToString()}\" !");
		}

		public IListFieldItem AddItem()
		{
			throw new NotImplementedException();
		}
	}
}