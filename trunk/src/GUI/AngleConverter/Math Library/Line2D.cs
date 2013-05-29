using System.ComponentModel;

namespace ISTUK.MathLibrary
{
    using System;

    public class Line2D : IFormattable
    {
        private readonly Vector2D _direction;
        private readonly Vector2D _origin;

        public Line2D()
        {
            _origin = new Vector2D();
            _direction = new Vector2D();
        }

        public Line2D(Vector2D origin, Vector2D direction)
        {
            _origin = origin;
            _direction = direction;
        }

        [Localizable(false)]
        public override string ToString()
        {
            return string.Format("Line: Origin={0}, Direction={1}", _origin, _direction);
        }

        [Localizable(false)]
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return string.Format("Line: Origin={0}, Direction={1}", _origin.ToString(format, formatProvider), _direction.ToString(format, formatProvider));
        }
    }
}

