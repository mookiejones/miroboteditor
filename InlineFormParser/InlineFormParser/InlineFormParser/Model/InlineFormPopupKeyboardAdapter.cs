using System;
using System.Windows;
using System.Windows.Controls;

namespace InlineFormParser.Model
{
	
	public class InlineFormPopupKeyboardAdapter : FieldPopupKeyboardAdapter
	{
		private InlineFormWrapper wrapper;

		protected override bool IsDetectionAllowed
		{
			get
			{
				bool flag = base.IsDetectionAllowed;
				if (this.wrapper != null)
				{
					flag &= this.wrapper.IsParentEnabledAndVisible;
				}
				return flag;
			}
		}

		public InlineFormPopupKeyboardAdapter(Control control, IServiceProvider serviceProvider)
			: base(control, serviceProvider)
		{
		}

		internal InlineFormPopupKeyboardAdapter(InlineFormWrapper wrapper)
			: base(wrapper.Content, wrapper.Owner.Site)
		{
			this.wrapper = wrapper;
		}

		protected override bool OnPopupOpening(IInputControl editControl)
		{
			FrameworkElement frameworkElement = (FrameworkElement)((WPFInputControl)editControl).Control;
			Field field = frameworkElement.DataContext as Field;
			if (field != null)
			{
				field.Select();
			}
			return base.OnPopupOpening(editControl);
		}
	}

}