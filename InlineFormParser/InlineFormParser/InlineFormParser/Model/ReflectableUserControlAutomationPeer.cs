using System;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
namespace InlineFormParser.Model
{
	
	public class ReflectableUserControlAutomationPeer : UserControlAutomationPeer, IValueProvider
	{
		private string propertyPath;

		private PropertyPathReflector reflector = new PropertyPathReflector(false);

		public bool IsReadOnly => false;

		public string Value
		{
			get
			{
				if (propertyPath == null)
				{
					return string.Empty;
				}
				return GetPropertyValue();
			}
		}

		public ReflectableUserControlAutomationPeer(ReflectableUserControl control)
			: base(control)
		{
		}

		public void SetValue(string value)
		{
			propertyPath = value;
		}

		protected override string GetClassNameCore()
		{
			return typeof(ReflectableUserControl).Name;
		}

		protected override string GetLocalizedControlTypeCore()
		{
			return typeof(ReflectableUserControl).Name;
		}

		protected override AutomationControlType GetAutomationControlTypeCore()
		{
			return AutomationControlType.Custom;
		}

		public override object GetPattern(PatternInterface patternInterface)
		{
			if (patternInterface == PatternInterface.Value)
			{
				return this;
			}
			return base.GetPattern(patternInterface);
		}

		private string GetPropertyValue()
		{
			try
			{
				object obj = reflector.ReflectPropertyPath(Owner, propertyPath);
				return obj?.ToString() ?? "null";
			}
			catch (ArgumentException ex)
			{
				return $"#ERROR: {ex.Message} ({ex.GetType().Name})";
			}
		}
	}
}