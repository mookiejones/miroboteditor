#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:8:18 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Xml.Serialization;

#endregion

namespace InlineFormParser.Model
{
	public abstract class InlineFormBase : RootElementBase
	{
		private Control parametersList;

		[XmlIgnore]
		public abstract int CurrentFieldIndex { get; set; }

		[XmlAttribute("DefField")]
		[Description("The default field id")]
		public abstract int DefaultFieldIndex { get; }

		[XmlAttribute("Title")]
		[Description("The ILF title")]
		public abstract string Title { get; set; }

		[XmlAttribute("KxrModule")]
		[Description("The module to get the resources from")]
		public abstract string KxrModule { get; set; }

		[XmlIgnore]
		public Control ParametersList
		{
			get => parametersList;
			set
			{
				parametersList = value;
				FirePropertyChanged("ParametersList");
			}
		}

		public event EventHandler<CurrentFieldIndexChangedEventArgs> CurrentFieldIndexChanged;

		public event EventHandler<RequestParameterListEventArgs> RequestParameterList;

		public abstract string FindIdByKey(string key);

		internal void FireRequestParameterList(ParamListField field)
		{
			RequestParameterList?.Invoke(this, new RequestParameterListEventArgs(field));
		}

		internal void FireCurrentFieldIndexChanged(Field prevField, Field newField)
		{
			CurrentFieldIndexChanged?.Invoke(this, new CurrentFieldIndexChangedEventArgs(prevField, newField));
		}
	}
}