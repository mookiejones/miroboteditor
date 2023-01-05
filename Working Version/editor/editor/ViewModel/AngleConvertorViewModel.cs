using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using miRobotEditor.Controls.AngleConverter;
using miRobotEditor.Controls.AngleConverter.Classes;
using miRobotEditor.Enums;
using miRobotEditor.Position;

namespace miRobotEditor.ViewModel
{
    public sealed class AngleConvertorViewModel : ToolViewModel
    {
        public const string ToolContentId = "AngleConverterTool";
        private ValueBoxViewModel _inputItems = new ValueBoxViewModel();

        private bool _isConverting;
        private string _matrix = string.Empty;

        private ValueBoxViewModel _outputItems = new ValueBoxViewModel
        {
            IsReadOnly = true
        };

        private RotationMatrix3D _rotationMatrix = RotationMatrix3D.Identity();

        public AngleConvertorViewModel()
            : base("Angle Converter")
        {
            InputItems.ItemsChanged += (s, e) => Convert();
            OutputItems.ItemsChanged += (s, e) => Convert();
            base.ContentId = "AngleConverterTool";
            DefaultPane = DefaultToolPane.Right;
        }

        public static CartesianEnum CartesianType { get; set; }

        public ValueBoxViewModel InputItems
        {
            get => _inputItems;
            set => SetProperty(ref _inputItems, value);
        }

        public ValueBoxViewModel OutputItems
        {
            get => _outputItems;
            set => SetProperty(ref _outputItems, value);
        }

        private double EPSILON
        {
            get => 4.94065645841247E-324;
            set => throw new NotImplementedException();
        }

        public string Error => null;

        public string Matrix
        {
            get => _matrix;
            set => SetProperty(ref _matrix, value);
        }

        public void Convert()
        {
            if (!(InputItems == null | OutputItems == null))
            {
                if (!_isConverting)
                {
                    _isConverting = true;
                    Vector3D result = new Vector3D();
                    double num = 0.0;
                    Quaternion quaternion = new Quaternion();
                    switch (InputItems.SelectedItem)
                    {
                        case CartesianEnum.ABB_Quaternion:
                            quaternion = new Quaternion(InputItems.V1, InputItems.V2, InputItems.V3, InputItems.V4);
                            break;
                        case CartesianEnum.Roll_Pitch_Yaw:
                            _rotationMatrix = RotationMatrix3D.FromRPY(InputItems.V1, InputItems.V2, InputItems.V3);
                            break;
                        case CartesianEnum.Axis_Angle:
                            _rotationMatrix =
                                RotationMatrix3D.RotateAroundVector(
                                    new Vector3D(InputItems.V1, InputItems.V2, InputItems.V3), InputItems.V4);
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
                            quaternion = (Quaternion)_rotationMatrix;
                            result = quaternion.Vector;
                            num = quaternion.Scalar;

                            break;
                        case CartesianEnum.Roll_Pitch_Yaw:
                            result = _rotationMatrix.RPY;
                            break;
                        case CartesianEnum.Axis_Angle:
                            result = _rotationMatrix.RotationAxis();
                            num = _rotationMatrix.RotationAngle();
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
                    string text = quaternion.ToString();

                    //                    var text = quaternion.ToString("F3");
                    if (Matrix != null && Matrix != text)
                    {
                        Matrix = text;
                    }
                    WriteValues(result, 0.0, false);
                    if (OutputItems.SelectedItem == CartesianEnum.ABB_Quaternion)
                    {
                        WriteValues(result, num, true);
                    }
                    if (OutputItems.SelectedItem == CartesianEnum.Axis_Angle)
                    {
                        OutputItems.V4 = num;
                    }
                    _isConverting = false;
                }
            }
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
    }

    public class ValueBoxViewModel : ViewModelBase
    {
        private Visibility _boxVisibility = Visibility.Visible;
        private string _header = string.Empty;
        private bool _isReadOnly;
        private CartesianEnum _selectedItem = CartesianEnum.ABB_Quaternion;
        private double _v1;
        private double _v2;
        private double _v3;
        private double _v4;

        public double V1
        {
            get => _v1;
            set
            {
                _ = SetProperty(ref _v1, value);
                RaiseItemsChanged();
            }
        }

        public double V2
        {
            get => _v2;
            set
            {
                _ = SetProperty(ref _v2, value);
                RaiseItemsChanged();
            }
        }

        public double V3
        {
            get => _v3;
            set
            {
                _ = SetProperty(ref _v3, value);

                RaiseItemsChanged();
            }
        }

        public double V4
        {
            get => _v4;
            set
            {
                _ = SetProperty(ref _v4, value);
                RaiseItemsChanged();
            }
        }

        public bool IsReadOnly
        {
            get => _isReadOnly;
            set => SetProperty(ref _isReadOnly, value);
        }

        public string Header
        {
            get => _header;
            set => SetProperty(ref _header, value);
        }

        public Visibility BoxVisibility
        {
            get => _boxVisibility;
            set => SetProperty(ref _boxVisibility, value);
        }

        public CartesianItems SelectionItems { get; } = new CartesianItems();

        public CartesianEnum SelectedItem
        {
            get => _selectedItem;
            set
            {


                _selectedItem = value;
                CheckVisibility();
                OnPropertyChanged(nameof(SelectedItem));
                RaiseItemsChanged();
            }
        }

        public event ItemsChangedEventHandler ItemsChanged;

        private void RaiseItemsChanged()
        {



            ItemsChanged?.Invoke(this, null);
        }

        private void CheckVisibility()
        {
            switch (_selectedItem)
            {
                case CartesianEnum.ABB_Quaternion:
                case CartesianEnum.Axis_Angle:
                    BoxVisibility = Visibility.Visible;
                    return;
            }
            BoxVisibility = Visibility.Collapsed;
        }
    }
}