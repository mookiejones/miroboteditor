using System;
using System.Windows;
namespace InlineFormParser.Model
{
	public class FloatingDisplayParameters
	{
		public PositionMode PositionMode { get; private set; }

		public FloatingDisplayAlignment Alignment { get; private set; }

		public Point FixedPosition { get; private set; }

		public UIElement CloseToControl { get; private set; }

		public Rect CloseToArea { get; private set; }

		public bool ScreenCoordinates { get; private set; }

		public DisplaySource CloseToDisplay { get; private set; }

		public string CloseToViewName { get; private set; }

		public string CloseToViewRegionId { get; private set; }

		public FloatingDisplayParameters()
		{
			SetPositionAligned(FloatingDisplayAlignment.HCenter);
		}

		public void SetPositionAligned(FloatingDisplayAlignment alignment)
		{
			if ((alignment & FloatingDisplayAlignment.HorizontalMask) == FloatingDisplayAlignment.HorizontalMask)
			{
				throw new ArgumentException("Invalid horizontal alignment");
			}
			if ((alignment & FloatingDisplayAlignment.VerticalMask) == FloatingDisplayAlignment.VerticalMask)
			{
				throw new ArgumentException("Invalid vertical alignment");
			}
			PositionMode = PositionMode.Aligned;
			Alignment = alignment;
		}

		public void SetPositionFixed(Point pos, bool screenCoordinates)
		{
			PositionMode = PositionMode.Fixed;
			FixedPosition = pos;
			ScreenCoordinates = screenCoordinates;
		}

		public void SetPositionCloseToControl(UIElement control)
		{
			PositionMode = PositionMode.CloseToControl;
			CloseToControl = control ?? throw new ArgumentNullException(nameof(control));
		}

		public void SetPositionCloseToArea(Rect area, bool screenCoordinates)
		{
			if (!(area.Width <= 0.0) && !(area.Height <= 0.0))
			{
				PositionMode = PositionMode.CloseToArea;
				CloseToArea = area;
				ScreenCoordinates = screenCoordinates;
				return;
			}
			throw new ArgumentException("area");
		}

		public void SetPositionCloseToDisplay(DisplaySource display)
		{
			if (display == (DisplaySource)null)
			{
				throw new ArgumentNullException(nameof(display));
			}
			PositionMode = PositionMode.CloseToDisplay;
			CloseToDisplay = display;
		}

		public void SetPositionCloseToViewRegion(string viewName, string regionId)
		{
			if (string.IsNullOrEmpty(viewName))
			{
				throw new ArgumentNullException(nameof(viewName));
			}
			if (string.IsNullOrEmpty(regionId))
			{
				throw new ArgumentNullException(nameof(regionId));
			}
			PositionMode = PositionMode.CloseToViewRegion;
			CloseToViewName = viewName;
			CloseToViewRegionId = regionId;
		}
	}
}