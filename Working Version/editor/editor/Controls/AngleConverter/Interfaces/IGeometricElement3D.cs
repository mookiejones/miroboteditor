using miRobotEditor.Controls.AngleConverter.Classes;
using System;

namespace miRobotEditor.Controls.AngleConverter.Interfaces
{
    public interface IGeometricElement3D : IFormattable
    {
        TransformationMatrix3D Position { get; }
    }
}