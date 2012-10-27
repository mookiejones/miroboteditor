namespace ISTUK.MathLibrary
{
    using System;

    public class Circle2D : IFormattable
    {
        private Point2D _origin;
        private double _radius;

        public Circle2D()
        {
            _origin = new Point2D();
            _radius = 0.0;
        }

        public Circle2D(Point2D origin, double radius)
        {
            _origin = origin;
            _radius = radius;
        }

        public override string ToString()
        {
            return ToString("F2", null);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return string.Format("Circle: Centre {0}, Radius {1}", _origin.ToString(format, formatProvider), _radius.ToString(format, formatProvider));
        }
    }
}

