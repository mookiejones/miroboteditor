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
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using InlineFormParser.Common.ResourceAccess;
using InlineFormParser.Common.Tracing;

#endregion

namespace InlineFormParser.Model
{
	public abstract class RootElementBase : ModelElement
	{
		private const string schemaRscPath = "KukaRoboter.Common.KfdXml.Model.etc.KfdXml.xsd";

		[XmlAttribute("FieldIds")]
		[Description("The way how fields shall be identified in TP-strings")]
		public FieldIdType FieldIdentification { get; set; } = FieldIdType.Index;

		[XmlAttribute("DefaultListItemIds")]
		[Description("The default way how list items shall be identified in TP-strings")]
		public ListItemIdType DefaultListItemIdentification { get; set; } = ListItemIdType.Index;

		[XmlIgnore]
		public IResourceAccessor Resolver { get; set; }

		[XmlIgnore]
		public bool IsAnyFieldModified
		{
			get { return FindFirstVisibleField(field => field.IsModified) != null; }
		}

		public abstract IEnumerable<Field> AllFields { get; }

		internal static PrettyTraceSource Trace => TraceSourceFactory.GetSource("KfdXml");

		public event EventHandler<FieldValueChangingEventArgs> FieldValueChanging;

		public Field FindFirstInvalidField()
		{
			return FindFirstVisibleField(field => !field.IsInputValid);
		}

		public string GetTPString(bool modifiedOnly)
		{
			StringBuilder stringBuilder = null;
			foreach (var allField in AllFields)
				if (!modifiedOnly || allField.IsModified)
				{
					if (stringBuilder == null)
						stringBuilder = new StringBuilder("%P ");
					else
						stringBuilder.Append(", ");
					stringBuilder.Append(allField.TPString);
				}

			if (stringBuilder == null) return string.Empty;
			return stringBuilder.ToString();
		}

		public static T CreateFromXmlFile<T>(string fileName, IResourceAccessor resolver) where T : RootElementBase
		{
			if (string.IsNullOrEmpty(fileName)) throw new ArgumentNullException(nameof(fileName));
			using (var xmlInput = new StreamReader(fileName))
			{
				return InternalCreateFromXml<T>(xmlInput, resolver);
			}
		}

		protected void CheckUniqueFieldIds()
		{
			var hashSet = new HashSet<string>();
			foreach (var allField in AllFields)
			{
				if (allField.ChildIndex >= 0 && hashSet.Contains(allField.FieldId))
					throw new ArgumentException($"Duplicate field id \"{allField.FieldId}\"!");
				hashSet.Add(allField.FieldId);
			}
		}

		protected Field FindFirstVisibleField(FieldPredicate predicate)
		{
			foreach (var allField in AllFields)
				if (allField.IsVisible && predicate(allField))
					return allField;
			return null;
		}

		protected static T CreateFromXmlString<T>(string xmlDescription, IResourceAccessor resolver) where T : RootElementBase
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (string.IsNullOrEmpty(xmlDescription)) throw new ArgumentNullException(nameof(xmlDescription));
			Trace.WriteLine(TraceEventType.Verbose, "CreateFromXmlString:{0}{1}", Environment.NewLine, xmlDescription);
			using (var xmlInput = new StringReader(xmlDescription))
			{
				return InternalCreateFromXml<T>(xmlInput, resolver);
			}
		}

		internal bool FireFieldValueChanging(Field field, string newValueString)
		{
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			var result = true;
			if (FieldValueChanging != null)
			{
				var fieldValueChangingEventArgs = new FieldValueChangingEventArgs(field, newValueString);
				var invocationList = FieldValueChanging.GetInvocationList();
				var array = invocationList;
				foreach (var @delegate in array)
				{
					@delegate.DynamicInvoke(this, fieldValueChangingEventArgs);
					if (fieldValueChangingEventArgs.Cancel)
					{
						result = false;
						Trace.WriteLine(TraceEventType.Verbose, "FieldValueChanging canceled for FieldIndex={0} NewValue=\"{1}\"",
							fieldValueChangingEventArgs.FieldIndex, fieldValueChangingEventArgs.NewValue);
					}
				}
			}

			return result;
		}

		private static T InternalCreateFromXml<T>(TextReader xmlInput, IResourceAccessor resolver) where T : RootElementBase
		{
			if (resolver == null) throw new ArgumentNullException(nameof(resolver));

			throw new NotImplementedException("Need to get resource");
			using (var input =
				typeof(RootElementBase).Assembly.GetManifestResourceStream("KukaRoboter.Common.KfdXml.Model.etc.KfdXml.xsd"))
			{
				using (var schemaDocument = XmlReader.Create(input))
				{
					var xmlReaderSettings = new XmlReaderSettings();
					xmlReaderSettings.Schemas.Add(null, schemaDocument);
					xmlReaderSettings.ValidationType = ValidationType.Schema;
					using (var xmlReader = XmlReader.Create(xmlInput, xmlReaderSettings))
					{
						var xmlSerializer = new XmlSerializer(typeof(T));
						var result = (T) xmlSerializer.Deserialize(xmlReader);
						result.Resolver = resolver;
						result.OnCreated(null, 0);
						return result;
					}
				}
			}
		}

		protected delegate bool FieldPredicate(Field field);
	}
}