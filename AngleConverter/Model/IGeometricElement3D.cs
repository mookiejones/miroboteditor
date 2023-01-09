using System;

namespace AngleConverter.Model
{
    public interface IGeometricElement3D : IFormattable
    {
        TransformationMatrix3D Position { get; }
    }
}
