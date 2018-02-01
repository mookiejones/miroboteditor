#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:7:34 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System.Xml.Serialization;
using InlineFormParser.ViewModel;

#endregion

namespace InlineFormParser.Model
{
	public abstract class ModelElement : PropertyChangedNotifier
	{
		[XmlIgnore]
		public ModelElement Parent { get; private set; }

		[XmlIgnore]
		public RootElementBase Root
		{
			get
			{
				ModelElement modelElement = this;
				while (modelElement.Parent != null) modelElement = modelElement.Parent;
				return (RootElementBase) modelElement;
			}
		}

		[XmlIgnore]
		public InlineFormBase RootInlineForm => Root as InlineFormBase;

		[XmlIgnore]
		public ParameterList RootParameterList => Root as ParameterList;

		[XmlIgnore]
		public int ChildIndex { get; private set; }

		internal virtual void OnCreated(ModelElement parent, int childIndex)
		{
			Parent = parent;
			ChildIndex = childIndex;
		}

		public override string ToString()
		{
			return $"{GetType().Name}#{ChildIndex + 1}";
		}

		protected void NormalizeString(ref string element)
		{
			element = element != null ? element.Trim() : string.Empty;
		}
	}
}