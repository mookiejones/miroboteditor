using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using InlineFormParser.Misc;

namespace InlineFormParser.Model
{
	public sealed class TemplatedAdorner : Adorner
	{
		private Control child;

		public FrameworkElement ReferenceElement { get; set; }

		protected override int VisualChildrenCount
		{
			get
			{
				if (child == null)
				{
					return 0;
				}
				return 1;
			}
		}

		public TemplatedAdorner(UIElement adornedElement, object dataContext, ControlTemplate adornerTemplate)
			: base(adornedElement)
		{
			Control control = new Control
			{
				Template = adornerTemplate
			};
			control.DataContext = dataContext;
			child = control;
			AddVisualChild(child);
		}

		public void ClearChild()
		{
			RemoveVisualChild(child);
			child = null;
		}

		public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
		{
			if (ReferenceElement == null)
			{
				return transform;
			}
			GeneralTransformGroup generalTransformGroup = new GeneralTransformGroup();
			generalTransformGroup.Children.Add(transform);
			GeneralTransform generalTransform = TransformToDescendant(ReferenceElement);
			if (generalTransform != null)
			{
				generalTransformGroup.Children.Add(generalTransform);
			}
			return generalTransformGroup;
		}

		protected override Visual GetVisualChild(int index)
		{
			if (child != null && index == 0)
			{
				return child;
			}
			throw new ArgumentOutOfRangeException(nameof(index));
		}

		protected override Size MeasureOverride(Size constraint)
		{
			if (ReferenceElement != null && AdornedElement != null && AdornedElement.IsMeasureValid && !DoubleUtil.AreClose(ReferenceElement.DesiredSize, AdornedElement.DesiredSize))
			{
				ReferenceElement.InvalidateMeasure();
			}
			child.Measure(constraint);
			return child.DesiredSize;
		}

		protected override Size ArrangeOverride(Size size)
		{
			Size size2 = base.ArrangeOverride(size);
			if (child != null)
			{
				child.Arrange(new Rect(default(Point), size2));
			}
			return size2;
		}
	}
}