using System;
using miRobotEditor.Classes;
using miRobotEditor.Core;
using miRobotEditor.Enums;
using miRobotEditor.GUI.AngleConverter;

namespace miRobotEditor.ViewModel
{
    public class AngleConvertorViewModel : ToolViewModel
    {
        public const string ToolContentId = "AngleConverterTool";


        public static CartesianEnum CartesianType { get; set; }

        #region InputItems

        /// <summary>
        /// The <see cref="InputItems" /> property's name.
        /// </summary>
        private const string InputItemsPropertyName = "InputItems";

        private ValueBoxViewModel _inputItems = new ValueBoxViewModel();

        /// <summary>
        /// Sets and gets the InputItems property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ValueBoxViewModel InputItems
        {
            get
            {
                return _inputItems;
            }

            set
            {
                if (_inputItems == value)
                {
                    return;
                }

                RaisePropertyChanging(InputItemsPropertyName);
                _inputItems = value;
                RaisePropertyChanged(InputItemsPropertyName);
            }
        }
        #endregion

        #region OutputItems
        /// <summary>
        /// The <see cref="OutputItems" /> property's name.
        /// </summary>
        public const string OutputItemsPropertyName = "OutputItems";

        private ValueBoxViewModel _outputItems = new ValueBoxViewModel{IsReadOnly=true};

        /// <summary>
        /// Sets and gets the OutputItems property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ValueBoxViewModel OutputItems
        {
            get
            {
                return _outputItems;
            }

            set
            {
                if (_outputItems == value)
                {
                    return;
                }

                RaisePropertyChanging(OutputItemsPropertyName);
                _outputItems = value;
                RaisePropertyChanged(OutputItemsPropertyName);
            }
        }
        #endregion
    

        // Properties
        protected double EPSILON
        {
            get { return double.Epsilon; }
            set { throw new NotImplementedException(); }
        }

        public string Error
        {
            get { return null; }
        }


        #region Matrix
        /// <summary>
        /// The <see cref="Matrix" /> property's name.
        /// </summary>
        public const string MatrixPropertyName = "Matrix";

        private string _matrix = String.Empty;

        /// <summary>
        /// Sets and gets the Matrix property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Matrix
        {
            get
            {
                return _matrix;
            }

            set
            {
                if (_matrix == value)
                {
                    return;
                }

                RaisePropertyChanging(MatrixPropertyName);
                _matrix = value;
                RaisePropertyChanged(MatrixPropertyName);
            }
        }
        #endregion
       

        #region Constructor

        public AngleConvertorViewModel() : base("Angle Converter")
        {
            InputItems.ItemsChanged += (s, e) => Convert();
            OutputItems.ItemsChanged += (s, e) => Convert();

            ContentId = ToolContentId;
            DefaultPane = DefaultToolPane.Right;
        }

        #endregion

        #region Fields

        private bool _isConverting;
        private RotationMatrix3D _rotationMatrix = RotationMatrix3D.Identity();

        #endregion

        public void Convert()
        {
            if ((InputItems == null | OutputItems == null)) return;

            if (_isConverting) return;
            _isConverting = true;
            var result = new Vector3D();
            double scalar = 0.0;
            var rotationMatrix = new Quaternion();

            switch (InputItems.SelectedItem)
            {
                case CartesianEnum.ABB_Quaternion:
                    rotationMatrix = new Quaternion(InputItems.V1, InputItems.V2, InputItems.V3, InputItems.V4);
//TODO Come Back to this                    this.rotationMatrix = this.rotationMatrix;
                    break;

                case CartesianEnum.Roll_Pitch_Yaw:
                    _rotationMatrix = RotationMatrix3D.FromRPY(InputItems.V1, InputItems.V2, InputItems.V3);
                    break;

                case CartesianEnum.Axis_Angle:
                    _rotationMatrix =
                        RotationMatrix3D.RotateAroundVector(new Vector3D(InputItems.V1, InputItems.V2, InputItems.V3),
                            InputItems.V4);
                    break;

                case CartesianEnum.Kuka_ABC:
                    _rotationMatrix = RotationMatrix3D.FromABC(InputItems.V1, InputItems.V2, InputItems.V3);
                    break;

                case CartesianEnum.Euler_ZYZ:
                    _rotationMatrix = RotationMatrix3D.FromEulerZYZ(InputItems.V1, InputItems.V2, InputItems.V3);
                    break;
            }

            switch (OutputItems.SelectedItem)
            {
                case CartesianEnum.ABB_Quaternion:
                    rotationMatrix = (Quaternion) _rotationMatrix;
                    result = rotationMatrix.Vector;
                    scalar = rotationMatrix.Scalar;
                    break;

                case CartesianEnum.Roll_Pitch_Yaw:
                    result = _rotationMatrix.RPY;
                    break;

                case CartesianEnum.Axis_Angle:
                    result = _rotationMatrix.RotationAxis();
                    scalar = _rotationMatrix.RotationAngle();
                    break;

                case CartesianEnum.Kuka_ABC:
                    result = _rotationMatrix.ABC;
                    break;

                case CartesianEnum.Euler_ZYZ:
                    result = _rotationMatrix.EulerZYZ;
                    break;

                case CartesianEnum.Alpha_Beta_Gamma:
                    result = _rotationMatrix.ABG;
                    break;
            }
            string str = rotationMatrix.ToString("F3");
            if ((Matrix != null) && (Matrix != str))
                Matrix = str;

            WriteValues(result, 0.0, false);

            if (OutputItems.SelectedItem == CartesianEnum.ABB_Quaternion)
                WriteValues(result, scalar, true);
            if (OutputItems.SelectedItem == CartesianEnum.Axis_Angle)
                OutputItems.V4 = scalar;

            _isConverting = false;
        }


        private void WriteValues(Vector3D result, double scalar, bool isScalar)
        {
            switch (isScalar)
            {
                case false:
                    OutputItems.V1 = result.X;
                    OutputItems.V2 = result.Y;
                    OutputItems.V3 = result.Z;
                    break;

                case true:
                    OutputItems.V1 = scalar;
                    OutputItems.V2 = result.X;
                    OutputItems.V3 = result.Y;
                    OutputItems.V4 = result.Z;
                    break;
            }
        }

        // Nested Types
    }
}