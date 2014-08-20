namespace miRobotEditor.GUI.AngleConverter
{
    using System;

    public interface IGeometricElement3D : IFormattable
    {
        TransformationMatrix3D Position { get; }
    }
}

