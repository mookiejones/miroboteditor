#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:7:23 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;
using InlineFormParser.Common.ResourceAccess;

#endregion

namespace InlineFormParser.Model
{
	[XmlRoot("InlineForm")]
	public class InlineForm : InlineFormBase
	{
		private List<Field> allFields;

		private int currentFieldIndex = -1;

		private int defaultFieldIndex;

		private IVisualGroup[] groups;

		private string title;

		private List<IVisualGroup> visibleGroups;

		[Description("The ILF title")]
		[XmlAttribute("Title")]
		public override string Title
		{
			get => title;
			set => title = value;
		}

		[XmlAttribute("KxrModule")]
		[Description("The module to get the resources from")]
		public override string KxrModule { get; set; }

		[Description("The default field id")]
		[XmlAttribute("DefField")]
		public string DefaultFieldId { get; set; }

		[XmlElement(ElementName = "Float", Type = typeof(FloatField))]
		[XmlElement(ElementName = "Free", Type = typeof(FreeField))]
		[XmlElement(ElementName = "Static", Type = typeof(StaticField))]
		[XmlElement(ElementName = "Number", Type = typeof(NumberField))]
		[XmlElement(ElementName = "Bool", Type = typeof(BoolField))]
		[XmlElement(ElementName = "List", Type = typeof(ListField))]
		[XmlElement(ElementName = "VisualGroup", Type = typeof(VisualGroup))]
		[XmlElement(ElementName = "Name", Type = typeof(NameField))]
		[XmlElement(ElementName = "ParamList", Type = typeof(ParamListField))]
		public ModelElement[] InternalGroups { get; set; }

		[XmlIgnore]
		public IVisualGroup[] Groups
		{
			get
			{
				if (groups == null)
				{
					int num = InternalGroups != null ? InternalGroups.Length : 0;
					groups = new IVisualGroup[num];
					if (InternalGroups != null)
						for (int i = 0; i < InternalGroups.Length; i++)
							groups[i] = (IVisualGroup) InternalGroups[i];
				}

				return groups;
			}
		}

		[XmlIgnore]
		public List<IVisualGroup> VisibleGroups
		{
			get
			{
				if (visibleGroups == null)
				{
					visibleGroups = new List<IVisualGroup>();
					if (!string.IsNullOrEmpty(title))
					{
						TitleField titleField = new TitleField(title);
						titleField.OnCreated(this, -1);
						visibleGroups.Add(titleField);
					}

					IVisualGroup[] array = Groups;
					foreach (IVisualGroup visualGroup in array)
						if (visualGroup.IsVisible)
							visibleGroups.Add(visualGroup);
				}

				return visibleGroups;
			}
		}

		[XmlIgnore]
		public override int CurrentFieldIndex
		{
			get => currentFieldIndex;
			set
			{
				if (value != currentFieldIndex)
				{
					CheckValidCurrentFieldIndex(value);
					int num = currentFieldIndex;
					currentFieldIndex = value;
					FirePropertyChanged("CurrentFieldIndex");
					foreach (IVisualGroup visibleGroup in VisibleGroups) visibleGroup.UpdateIsSelected();
					Field prevField = num >= 0 ? Fields[num] : null;
					Field newField = Fields[currentFieldIndex];
					FireCurrentFieldIndexChanged(prevField, newField);
				}
			}
		}

		[XmlIgnore]
		public override int DefaultFieldIndex => defaultFieldIndex;

		[XmlIgnore]
		public List<Field> Fields
		{
			get
			{
				if (allFields == null)
				{
					allFields = new List<Field>();
					IVisualGroup[] array = Groups;
					foreach (IVisualGroup visualGroup in array)
					{
						Field[] fields = visualGroup.Fields;
						foreach (Field item in fields) allFields.Add(item);
					}
				}

				return allFields;
			}
		}

		public override IEnumerable<Field> AllFields => Fields;

		public static InlineForm CreateFromXmlString(string xmlDescription, IResourceAccessor resolver)
		{
			return CreateFromXmlString<InlineForm>(xmlDescription, resolver);
		}

		public static InlineForm CreateFromXmlFile(string fileName, IResourceAccessor resolver)
		{
			return CreateFromXmlFile<InlineForm>(fileName, resolver);
		}

		public void AddField(Field field)
		{
			AddGroup(new Field[1]
			{
				field
			});
		}

		public void AddGroup(Field[] fields)
		{
			if (InternalGroups == null) InternalGroups = new ModelElement[0];
			VisualGroup item = new VisualGroup(fields);
			List<ModelElement> list = new List<ModelElement>(InternalGroups);
			list.Add(item);
			InternalGroups = list.ToArray();
		}

		public void EndInit()
		{
			allFields = null;
			visibleGroups = null;
			groups = null;
			OnCreated(null, 0);
		}

		internal override void OnCreated(ModelElement parent, int childIndex)
		{
			base.OnCreated(parent, childIndex);
			NormalizeString(ref title);
			List<ModelElement> list = null;
			for (int i = 0; i < InternalGroups.Length; i++)
			{
				IVisualGroup visualGroup = (IVisualGroup) InternalGroups[i];
				if (list == null)
				{
					if (!visualGroup.HasFields)
					{
						list = new List<ModelElement>();
						for (int j = 0; j < i; j++) list.Add(InternalGroups[j]);
					}
				}
				else if (visualGroup.HasFields)
				{
					list.Add(InternalGroups[i]);
				}
			}

			if (list != null)
			{
				Trace.WriteLine(TraceEventType.Verbose, "{0} empty VisualGroup(s) removed from InlineForm",
					InternalGroups.Length - list.Count);
				if (list.Count == 0) throw new InvalidOperationException("InlineForm has no fields!");
				InternalGroups = list.ToArray();
			}

			int num = 0;
			for (int k = 0; k < InternalGroups.Length; k++)
			{
				ModelElement modelElement = InternalGroups[k];
				modelElement.OnCreated(this, num);
				num += ((IVisualGroup) modelElement).Fields.Length;
			}

			CheckUniqueFieldIds();
			if (VisibleGroups.Count == 0) Trace.WriteLine(TraceEventType.Warning, "InlineForm has no visible fields!");
			Field field = FindFieldById(DefaultFieldId);
			defaultFieldIndex = field != null ? field.ChildIndex : 0;
			currentFieldIndex = defaultFieldIndex;
			bool flag = false;
			do
			{
				if (Fields[currentFieldIndex].CanBeCurrent)
					flag = true;
				else
					currentFieldIndex = (currentFieldIndex + 1) % Fields.Count;
			} while (!flag && currentFieldIndex != defaultFieldIndex);

			if (!flag)
			{
				Trace.WriteLine(TraceEventType.Warning,
					"InlineForm has no field that is suitable to be marked as \"current field\"!");
				currentFieldIndex = -1;
			}
			else if (currentFieldIndex != defaultFieldIndex)
			{
				Trace.WriteLine(TraceEventType.Warning, "Default field {0} in InlineForm is not suitable, current field is {1}",
					defaultFieldIndex + 1, currentFieldIndex + 1);
			}
		}

		public override string FindIdByKey(string key)
		{
			foreach (var field in Fields)
				if (field.Key == key)
					return field.FieldId;
			throw new InvalidOperationException("A field with the given key (" + key + ") was not found.");
		}

		private Field FindFieldById(string id)
		{
			if (!string.IsNullOrEmpty(id))
				foreach (Field field in Fields)
					if (field.FieldId == id)
						return field;
			return null;
		}

		private void CheckValidCurrentFieldIndex(int idx)
		{
			if (idx >= 0 && idx < Fields.Count)
			{
				Field field = Fields[idx];
				if (field.CanBeCurrent) return;
				throw new InvalidOperationException("Field is not suitable to be the current field");
			}

			throw new IndexOutOfRangeException("CurrentFieldIndex");
		}
	}
}