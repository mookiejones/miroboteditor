using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;

namespace InlineFormParser.Model
{
	public class FieldInfoControl : UserControlBase, IComponentConnector
	{
		private bool _contentLoaded;

		public FieldInfoControl(Field field)
		{
			if (field == null)
			{
				throw new ArgumentNullException(nameof(field));
			}
			DataContext = field;
			InitializeComponent();
		}

		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void InitializeComponent()
		{
			if (!_contentLoaded)
			{
				_contentLoaded = true;
				Uri resourceLocator = new Uri("/KukaRoboter.Common.KfdXml;component/base/fieldinfocontrol.xaml", UriKind.Relative);
				Application.LoadComponent(this, resourceLocator);
			}
		}

		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerNonUserCode]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			_contentLoaded = true;
		}
	}
}