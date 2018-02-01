#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:9:43 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using InlineFormParser.Common.ResourceAccess;

#endregion

namespace InlineFormParser.Model
{
	[XmlRoot("ParameterList")]
	public class ParameterList : RootElementBase, IParameterList
	{
		private List<Field> allFields;

		private int selectedGroupIndex = -1;

		public ParameterList()
		{
			GroupIdentification = GroupIdType.Index;
		}

		public override IEnumerable<Field> AllFields => allFields;

		[XmlIgnore]
		public int DefaultGroupIndex { get; private set; }

		[XmlAttribute("GroupIds")]
		[Description("The way how groups shall be identified in TP-strings")]
		public GroupIdType GroupIdentification { get; set; }

		[Description("The groups of this ParamList")]
		[XmlElement("Group")]
		public Group[] Groups { get; set; }

		[XmlIgnore]
		public string Handle { get; private set; }

		[XmlAttribute("KxrModule")]
		[Description("The module to get the resources from")]
		public string KxrModule { get; set; }

		public int SelectedGroup
		{
			get
			{
				if (selectedGroupIndex == -1)
				{
					Group group = FindGroupById(DefaultGroupId);
					if (group == null) return 0;
					return group.ChildIndex;
				}

				return selectedGroupIndex;
			}
			set => selectedGroupIndex = value;
		}

		[Description("The index of the default group to be shown")]
		[XmlAttribute("DefGroup")]
		public string DefaultGroupId { get; set; }

		IGroup[] IParameterList.Groups => Groups.Cast<IGroup>().ToArray();

		public static ParameterList CreateFromXmlFile(string fileName, string handle, IResourceAccessor resolver)
		{
			if (handle == null) throw new ArgumentNullException(nameof(handle));
			ParameterList parameterList = CreateFromXmlFile<ParameterList>(fileName, resolver);
			parameterList.Handle = handle;
			return parameterList;
		}

		public static ParameterList CreateFromXmlString(string xmlDescription, string handle, IResourceAccessor resolver)
		{
			if (handle == null) throw new ArgumentNullException(nameof(handle));
			ParameterList parameterList = CreateFromXmlString<ParameterList>(xmlDescription, resolver);
			parameterList.Handle = handle;
			return parameterList;
		}

		public IGroup AddGroup()
		{
			throw new NotImplementedException();
		}

		internal override void OnCreated(ModelElement parent, int childIndex)
		{
			base.OnCreated(parent, childIndex);
			for (int i = 0; i < Groups.Length; i++) Groups[i].OnCreated(this, i);
			allFields = Groups.SelectMany(g => g.Fields).ToList();
			CheckUniqueFieldIds();
			Group group = FindGroupById(DefaultGroupId);
			DefaultGroupIndex = group != null ? group.ChildIndex : 0;
		}

		private Group FindGroupById(string id)
		{
			if (!string.IsNullOrEmpty(id)) return Groups.FirstOrDefault(g => g.GroupId == id);
			return null;
		}
	}
}