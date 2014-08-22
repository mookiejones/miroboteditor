using System;

namespace miRobotEditor.GUI.AngleConverter
{
    public interface IGeometricElement3D : IFormattable
    {
        TransformationMatrix3D Position { get; }
    }
}