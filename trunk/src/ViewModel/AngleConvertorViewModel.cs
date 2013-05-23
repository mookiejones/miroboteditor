using miRobotEditor.ViewModel;
using System.Windows;
using System.Windows.Controls;
using miRobotEditor.GUI.AngleConverter.Robot;
using System;
using System.Globalization;
using ISTUK.MathLibrary;
using System.Windows.Data;
using System.ComponentModel;
namespace miRobotEditor.ViewModel
{

    public class AngleConvertorViewModel:ToolViewModel
    {

        public const string ToolContentId = "AngleConverterTool";

        public static CartesianEnum CartesianType { get; set; }
        private ValueBoxViewModel _inputItems = new ValueBoxViewModel();
        public ValueBoxViewModel InputItems { get { return _inputItems; } set { _inputItems = value; RaisePropertyChanged("InputItems");  } }
        private ValueBoxViewModel _outputItems =new ValueBoxViewModel{IsReadOnly=true};
        public ValueBoxViewModel OutputItems { get { return _outputItems; } set { _outputItems = value; RaisePropertyChanged("OutputItems");  } }

        #region Constructor
        public AngleConvertorViewModel():base("Angle Converter")
        {
            InputItems.ItemsChanged+=(s,e) => {Convert();};
            OutputItems.ItemsChanged += (s, e) => { Convert(); };

            ContentId = ToolContentId;
            DefaultPane = DefaultToolPane.Right;
        }
        #endregion

        #region Fields
        private string _matrix = string.Empty;  
            private bool IsConverting=false;
            private RotationMatrix3D rotationMatrix = RotationMatrix3D.Identity();
        #endregion

          
    public void Convert()
    {
        if ((InputItems == null | OutputItems == null)) return;

        if (!this.IsConverting)
        {
            this.IsConverting = true;
            Vector3D result = new Vector3D();
            double scalar = 0.0;
            Quaternion rotationMatrix = new Quaternion();

            switch (InputItems.SelectedItem)
            {
                case CartesianEnum.ABB_Quaternion:
                    rotationMatrix = new Quaternion(InputItems.V1, InputItems.V2, InputItems.V3, InputItems.V4);
//TODO Come Back to this                    this.rotationMatrix = this.rotationMatrix;
                    break;

                case CartesianEnum.Roll_Pitch_Yaw:
                    this.rotationMatrix = RotationMatrix3D.FromRPY(InputItems.V1, InputItems.V2, InputItems.V3);
                    break;

                case CartesianEnum.Axis_Angle:
                    this.rotationMatrix = RotationMatrix3D.RotateAroundVector(new Vector3D(InputItems.V1, InputItems.V2, InputItems.V3), InputItems.V4);
                    break;

                case CartesianEnum.Kuka_ABC:
                    this.rotationMatrix = RotationMatrix3D.FromABC(InputItems.V1, InputItems.V2, InputItems.V3);
                    break;

                case CartesianEnum.Euler_ZYZ:
                    this.rotationMatrix = RotationMatrix3D.FromEulerZYZ(InputItems.V1, InputItems.V2, InputItems.V3);
                    break;
            }

            switch (OutputItems.SelectedItem)
            {
                case CartesianEnum.ABB_Quaternion:
                    rotationMatrix = (Quaternion) this.rotationMatrix;
                    result = rotationMatrix.Vector;
                    scalar = rotationMatrix.Scalar;
                    break;

                case CartesianEnum.Roll_Pitch_Yaw:
                    result = this.rotationMatrix.RPY;
                    break;

                case CartesianEnum.Axis_Angle:
                    result = this.rotationMatrix.RotationAxis();
                    scalar = this.rotationMatrix.RotationAngle();
                    break;

                case CartesianEnum.Kuka_ABC:
                    result = this.rotationMatrix.ABC;
                    break;

                case CartesianEnum.Euler_ZYZ:
                    result = this.rotationMatrix.EulerZYZ;
                    break;

                case CartesianEnum.Alpha_Beta_Gamma:
                    result = this.rotationMatrix.ABG;
                    break;
            }
            string str = rotationMatrix.ToString("F3");
            if ((this.Matrix != null) && (this.Matrix != str))
                this.Matrix = str;

            this.WriteValues(result, 0.0, false);           

            if (OutputItems.SelectedItem == CartesianEnum.ABB_Quaternion)
                    this.WriteValues(result, scalar, true);
                if (OutputItems.SelectedItem==CartesianEnum.Axis_Angle)
                    OutputItems.V4 = scalar;

            this.IsConverting = false;
        }
    }


    private void WriteValues(Vector3D Result, double scalar, bool isScalar)
    {
        switch (isScalar)
        {
            case false:
                OutputItems.V1 = Result.X;
                OutputItems.V2 = Result.Y;
                OutputItems.V3 = Result.Z;
                break;

            case true:
                OutputItems.V1 = scalar;
                OutputItems.V2 = Result.X;
                OutputItems.V3 = Result.Y;
                OutputItems.V4 = Result.Z;
                break;
        }
    }

    // Properties
    protected double EPSILON
    {
        get
        {
            return double.Epsilon;
        }
        set
        {
            throw new NotImplementedException();
        }
    }

    public string Error
    {
        get
        {
            return null;
        }
    }

    public string Matrix {get{return _matrix; }set { _matrix = value;  RaisePropertyChanged("Matrix");  }  }

    // Nested Types
   
}
    public enum CartesianEnum
    {
        ABB_Quaternion,
        Roll_Pitch_Yaw,
        Axis_Angle,
        Kuka_ABC,
        Euler_ZYZ,
        Alpha_Beta_Gamma
    }
    public class CartesianItems : System.Collections.Generic.List<CartesianTypes>
    {
        // Methods
        public CartesianItems()
        {
            CartesianTypes item = new CartesianTypes
            {
                ValueCartesianEnum = CartesianEnum.ABB_Quaternion,
                ValueCartesianString = "ABB Quaternion"
            };
            base.Add(item);
            CartesianTypes types2 = new CartesianTypes
            {
                ValueCartesianEnum = CartesianEnum.Roll_Pitch_Yaw,
                ValueCartesianString = "Roll-Pitch-Yaw"
            };
            base.Add(types2);
            CartesianTypes types3 = new CartesianTypes
            {
                ValueCartesianEnum = CartesianEnum.Axis_Angle,
                ValueCartesianString = "Axis Angle"
            };
            base.Add(types3);
            CartesianTypes types4 = new CartesianTypes
            {
                ValueCartesianEnum = CartesianEnum.Kuka_ABC,
                ValueCartesianString = "Kuka ABC"
            };
            base.Add(types4);
            CartesianTypes types5 = new CartesianTypes
            {
                ValueCartesianEnum = CartesianEnum.Euler_ZYZ,
                ValueCartesianString = "Euler ZYZ"
            };
            base.Add(types5);
            CartesianTypes types6 = new CartesianTypes
            {
                ValueCartesianEnum = CartesianEnum.Alpha_Beta_Gamma,
                ValueCartesianString = "Alpha-Beta-Gamma"
            };
            base.Add(types6);
        }
    }
    public class CartesianTypes
    {
        // Properties
        public CartesianEnum ValueCartesianEnum { get; set; }

        public string ValueCartesianString { get; set; }
    }       


    public delegate void ItemsChangedEventHandler(object sender,EventArgs e);
    public class ValueBoxViewModel :ViewModelBase
    {
        public event ItemsChangedEventHandler ItemsChanged;

        #region Properties
        private double _v1 = 0.0F;
        private double _v2 = 0.0F;
        private double _v3 = 0.0F;
        private double _v4 = 0.0F;
        public double V1 { get { return _v1; } set { _v1 = value; RaisePropertyChanged("V1"); RaiseItemsChanged(); } }
        public double V2 { get { return _v2; } set { _v2 = value; RaisePropertyChanged("V2"); RaiseItemsChanged(); } }
        public double V3 { get { return _v3; } set { _v3 = value; RaisePropertyChanged("V3"); RaiseItemsChanged(); } }
        public double V4 { get { return _v4; } set { _v4 = value; RaisePropertyChanged("V4"); RaiseItemsChanged(); } }

        private bool _isReadOnly = false;
        public bool IsReadOnly { get { return _isReadOnly; } set { _isReadOnly = value; RaisePropertyChanged("IsReadOnly"); } }

        private string _header = String.Empty;
        public string Header { get { return _header; } set { _header = value; RaisePropertyChanged("Header"); } }

        private Visibility _boxVisibility = Visibility.Visible;
        public Visibility BoxVisibility { get { return _boxVisibility; } set { _boxVisibility = value; RaisePropertyChanged("BoxVisibility"); } }





        private CartesianItems _selectionitems = new CartesianItems();
        public CartesianItems SelectionItems { get { return _selectionitems; } }


        private CartesianEnum _selectedItem = CartesianEnum.ABB_Quaternion;
        public CartesianEnum SelectedItem { get { return _selectedItem; } set { _selectedItem = value; CheckVisibility(); RaisePropertyChanged("SelectedItem"); RaiseItemsChanged(); } }
        #endregion

        void RaiseItemsChanged()
        {
            if (ItemsChanged != null)
                ItemsChanged(this, new EventArgs());
        }
        void CheckVisibility()
        {
            switch (_selectedItem)
            {
                case CartesianEnum.ABB_Quaternion:
                case CartesianEnum.Axis_Angle:
                    BoxVisibility = Visibility.Visible;
                    break;
                default:
                    BoxVisibility = Visibility.Collapsed;
                    break;
            }

        }
    }



    public sealed class DoubleValidationRule:ValidationRule
    {
        #region Constructor
        public DoubleValidationRule()
        {
        }
        #endregion

        private bool IsDouble(string Value)
        {
            double num;
            return double.TryParse(Value, out num);
        }

        private bool IsDoubleValid(double _old, double _new)
        {
            if (!_old.Equals(_new))
            {
                if (double.IsNaN(_old) & double.IsNaN(_new))
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            
        try
        {
            double val = 0.0F;
                val = double.Parse((String)value);
        }
        catch (Exception e)
        {
            return new ValidationResult(false, "Illegal characters or " + e.Message);
        }

            return new ValidationResult(true, null);
       
    }
    }

    public sealed class EnumtoInt32 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Int32)(CartesianEnum)value;
            // Do the conversion from bool to visibility
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (CartesianEnum)Enum.Parse(typeof(CartesianEnum), ((Int32)value).ToString(CultureInfo.InvariantCulture));
        }
    }



    public class AngleToolTipConverter : IValueConverter
    {
        string Title = string.Empty;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((CartesianEnum)value)
            {
                case CartesianEnum.ABB_Quaternion:
                    Title = "ABB Quaternion";
                    switch (parameter.ToString())
                    {
                        case "V1":
                            return Title + " Q1";
                        case "V2":
                            return Title + " Q2";
                        case "V3":
                            return Title + " Q3";
                        case "V4":
                            return Title + " Q4";
                    }
                    break;
                case CartesianEnum.Alpha_Beta_Gamma:
                    return "Alpha Beta Gamma";
                case CartesianEnum.Axis_Angle:
                    return "Axis Angle";
                case CartesianEnum.Euler_ZYZ:
                    return "Euler ZYZ";
                case CartesianEnum.Kuka_ABC:
                     Title = "Kuka ABC";
                    switch (parameter.ToString())
                    {
                        case "V1":
                            return Title + " A. Rotation around Z.";
                        case "V2":           
                            return Title + " B. Rotation around Y.";
                        case "V3":           
                            return Title + " C. Rotation around X.";
                    }
                    break;
                case CartesianEnum.Roll_Pitch_Yaw:
                     Title = "Roll Pitch Yaw";
                    switch (parameter.ToString())
                    {
                        case "V1":
                            return Title + " R. Rotation around X.";
                        case "V2":           
                            return Title + " P. Rotation around Y.";
                        case "V3":           
                            return Title + " Y. Rotation around Z.";
                    }
                    break;
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
