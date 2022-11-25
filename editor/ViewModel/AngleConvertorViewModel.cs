using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using GalaSoft.MvvmLight;
using RobotEditor.Classes;
using RobotEditor.Controls.AngleConverter;
using RobotEditor.Controls.AngleConverter.Classes;
using RobotEditor.Enums;

namespace RobotEditor.ViewModel
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
            get { return _inputItems; }
            set
            {
                _inputItems = value;
                RaisePropertyChanged("InputItems");
            }
        }

        public ValueBoxViewModel OutputItems
        {
            get { return _outputItems; }
            set
            {
                _outputItems = value;
                RaisePropertyChanged("OutputItems");
            }
        }

        private double EPSILON
        {
            get { return 4.94065645841247E-324; }
            set { throw new NotImplementedException(); }
        }

        public string Error
        {
            get { return null; }
        }

        public string Matrix
        {
            get { return _matrix; }
            set
            {
                _matrix = value;
                RaisePropertyChanged("Matrix");
            }
        }

        public void Convert()
        {
            if (!(InputItems == null | OutputItems == null))
            {
                if (!_isConverting)
                {
                    _isConverting = true;
                    var result = new Vector3D();
                    var num = 0.0;
                    var quaternion = new Quaternion();
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
                            quaternion = (Quaternion) _rotationMatrix;
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
                    var text = quaternion.ToString();

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
        private readonly CartesianItems _selectionitems = new CartesianItems();
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
            get { return _v1; }
            set
            {
                _v1 = value;
                RaisePropertyChanged("V1");
                RaiseItemsChanged();
            }
        }

        public double V2
        {
            get { return _v2; }
            set
            {
                _v2 = value;
                RaisePropertyChanged("V2");
                RaiseItemsChanged();
            }
        }

        public double V3
        {
            get { return _v3; }
            set
            {
                _v3 = value;
                RaisePropertyChanged("V3");
                RaiseItemsChanged();
            }
        }

        public double V4
        {
            get { return _v4; }
            set
            {
                _v4 = value;
                RaisePropertyChanged("V4");
                RaiseItemsChanged();
            }
        }

        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set
            {
                _isReadOnly = value;
                RaisePropertyChanged("IsReadOnly");
            }
        }

        public string Header
        {
            get { return _header; }
            set
            {
                _header = value;
                RaisePropertyChanged("Header");
            }
        }

        public Visibility BoxVisibility
        {
            get { return _boxVisibility; }
            set
            {
                _boxVisibility = value;
                RaisePropertyChanged("BoxVisibility");
            }
        }

        public CartesianItems SelectionItems
        {
            get { return _selectionitems; }
        }

        public CartesianEnum SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                CheckVisibility();
                RaisePropertyChanged("SelectedItem");
                RaiseItemsChanged();
            }
        }

        public event ItemsChangedEventHandler ItemsChanged;

        private void RaiseItemsChanged()
        {
            if (ItemsChanged != null)
            {
                ItemsChanged(this, (ItemsChangedEventArgs) new EventArgs());
            }
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