#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:8:16 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System.ComponentModel;
using System.Windows.Controls;
using System.Xml.Serialization;

#endregion

namespace InlineFormParser.Model
{
	public class ParamListField : NameField, IParameterListField, IField
	{
		[Description("The handle of the ParamList that is requested for this field")]
		[XmlAttribute("ParamListHandle")]
		public string ParamListHandle { get; set; }

		[XmlIgnore]
		public Control NewParametersList { get; set; }

		public override bool DisregardValidation => !EnableInput;

		public string Handle => Handle;

		public void RequestParameterList()
		{
			if (NewParametersList != null)
			{
				var inlineFormBase = Root as InlineFormBase;
				if (inlineFormBase != null)
					if (inlineFormBase.ParametersList == null)
						inlineFormBase.ParametersList = NewParametersList;
					else
						inlineFormBase.ParametersList = null;
			}
			else
			{
				var inlineFormBase2 = (InlineFormBase) Root;
				inlineFormBase2.FireRequestParameterList(this);
			}
		}

		internal override void OnCreated(ModelElement parent, int childIndex)
		{
			base.OnCreated(parent, childIndex);
		}
	}
}