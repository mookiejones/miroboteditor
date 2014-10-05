using System;
using miRobotEditor.Controls.AngleConverter.Classes;

namespace miRobotEditor.Controls.AngleConverter.Interfaces
{
    public interface IGeometricElement3D : IFormattable
    {
        TransformationMatrix3D Position { get; }
    }
}