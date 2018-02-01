using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace InlineFormParser.Model
{
	public class AdornedElementPlaceholderExt : AdornedElementPlaceholder 
	{
		private Adorner adorner;

		public new UIElement AdornedElement
		{
			get
			{
				UIElement adornedElement = base.AdornedElement;
				if (adornedElement == null)
				{
					Adorner adorner = Adorner;
					if (adorner != null)
					{
						return adorner.AdornedElement;
					}
					return null;
				}
				return adornedElement;
			}
		}

		private Adorner Adorner
		{
			get
			{
				if (adorner == null)
				{
					FrameworkElement frameworkElement = TemplatedParent as FrameworkElement;
					if (frameworkElement != null)
					{
						adorner = (VisualTreeHelper.GetParent(frameworkElement) as Adorner);
						TemplatedAdorner templatedAdorner = adorner as TemplatedAdorner;
						if (templatedAdorner != null && templatedAdorner.ReferenceElement == null)
						{
							templatedAdorner.ReferenceElement = this;
						}
					}
				}
				return adorner;
			}
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);
		}

		protected override Size MeasureOverride(Size constraint)
		{
			if (TemplatedParent == null)
			{
				throw new InvalidOperationException("AdornedElementPlaceHolderExt must be within a template");
			}
			if (AdornedElement == null)
			{
				return new Size(0.0, 0.0);
			}
			Size renderSize = AdornedElement.RenderSize;
			UIElement child = Child;
			if (child != null)
			{
				child.Measure(renderSize);
			}
			return renderSize;
		}

		#region Implementation of IAddChild

		public void AddChild(object value)
		{

			throw new NotImplementedException();
		}

		public void AddText(string text)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}