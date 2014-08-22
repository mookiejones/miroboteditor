using System;
using System.ComponentModel;

namespace miRobotEditor.GUI.AngleConverter
{
    [Localizable(false)]
    public sealed class Circle2D : IFormattable
    {
        private readonly Point2D _origin;
        private readonly double _radius;

        public Circle2D(Point2D origin, double radius)
        {
            _origin = origin;
            _radius = radius;
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return string.Format("Circle: Centre {0}, Radius {1}", _origin.ToString(format, formatProvider),
                _radius.ToString(format, formatProvider));
        }

        public override string ToString()
        {
            return ToString("F2", null);
        }
    }
}