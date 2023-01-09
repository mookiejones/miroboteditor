using System;

namespace AngleConverterControl.Model
{
    public interface IGeometricElement3D : IFormattable
    {
        TransformationMatrix3D Position { get; }
    }
}
