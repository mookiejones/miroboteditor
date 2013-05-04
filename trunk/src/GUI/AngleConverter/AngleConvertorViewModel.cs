using miRobotEditor.ViewModel;
using System.Windows;
using miRobotEditor.GUI.AngleConverter.Robot;
using System;
using System.Globalization;
using ISTUK.MathLibrary;
namespace miRobotEditor.GUI
{
    public class AngleConvertorViewModel:ViewModelBase
    {
        public static CartesianEnum CartesianType { get; set; }
       
 
    // Fields
    private Visibility _in4Visible = Visibility.Visible;
    private CartesianItems _inputconvention = new CartesianItems();
    private CartesianEnum _inputItem;
    private string _inWidth;
    private string _matrix = string.Empty;
    private Visibility _out4Visible = Visibility.Visible;
    private CartesianItems _outputconvention = new CartesianItems();
    private CartesianEnum _outputItem;
    private string _outWidth;
    private double in1;
    private double in2;
    private double in3;
    private double in4;
    private bool IsConverting;
    private double out1;
    private double out2;
    private double out3;
    private double out4;
    private RotationMatrix3D rotationMatrix = RotationMatrix3D.Identity();

    // Methods
    public void Convert()
    {
        if (!this.IsConverting)
        {
            this.IsConverting = true;
            Vector3D result = new Vector3D();
            double scalar = 0.0;
            Quaternion rotationMatrix = new Quaternion();
            int num2 = 3;
            switch (this.InputItem)
            {
                case CartesianEnum.ABB_Quaternion:
                    rotationMatrix = new Quaternion(this.In2, this.In3, this.In4, this.In1);
//TODO Come Back to this                    this.rotationMatrix = this.rotationMatrix;
                    num2 = 4;
                    break;

                case CartesianEnum.Roll_Pitch_Yaw:
                    this.rotationMatrix = RotationMatrix3D.FromRPY(this.In1, this.In2, this.In3);
                    break;

                case CartesianEnum.Axis_Angle:
                    this.rotationMatrix = RotationMatrix3D.RotateAroundVector(new Vector3D(this.In1, this.In2, this.In3), this.In4);
                    num2 = 4;
                    break;

                case CartesianEnum.Kuka_ABC:
                    this.rotationMatrix = RotationMatrix3D.FromABC(this.In1, this.In2, this.In3);
                    break;

                case CartesianEnum.Euler_ZYZ:
                    this.rotationMatrix = RotationMatrix3D.FromEulerZYZ(this.In1, this.In2, this.In3);
                    break;
            }
            this.In4Visible = num2.Equals(3) ? Visibility.Hidden : Visibility.Visible;
            this.InWidth = num2.Equals(3) ? "33*" : "25*";
            num2 = 3;
            switch (this.OutputItem)
            {
                case CartesianEnum.ABB_Quaternion:
                    rotationMatrix = (Quaternion) this.rotationMatrix;
                    result = rotationMatrix.Vector;
                    scalar = rotationMatrix.Scalar;
                    num2 = 4;
                    break;

                case CartesianEnum.Roll_Pitch_Yaw:
                    result = this.rotationMatrix.RPY;
                    break;

                case CartesianEnum.Axis_Angle:
                    result = this.rotationMatrix.RotationAxis();
                    scalar = this.rotationMatrix.RotationAngle();
                    num2 = 4;
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
            {
                this.Matrix = str;
            }
            this.WriteValues(result, 0.0, false);
            this.Out4Visible = num2.Equals(3) ? Visibility.Collapsed : Visibility.Visible;
            this.OutWidth = num2.Equals(3) ? "33*" : "25*";
            if (num2 == 4)
            {
                if (this.OutputItem == CartesianEnum.ABB_Quaternion)
                {
                    this.WriteValues(result, scalar, true);
                }
                else
                {
                    this.Out4 = scalar;
                }
            }
            this.IsConverting = false;
        }
    }

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

    public void NotifyPropertyChanged(string propertyName)
    {
        RaisePropertyChanged(propertyName);
        this.Convert();
    }



    private void WriteValues(Vector3D Result, double scalar, bool isScalar)
    {
        switch (isScalar)
        {
            case false:
                this.Out1 = Result.X;
                this.Out2 = Result.Y;
                this.Out3 = Result.Z;
                this.Out4Visible = Visibility.Hidden;
                break;

            case true:
                this.Out1 = scalar;
                this.Out2 = Result.X;
                this.Out3 = Result.Y;
                this.Out4 = Result.Z;
                this.Out4Visible = Visibility.Visible;
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

    public double In1
    {
        get
        {
            return this.in1;
        }
        set
        {
            this.in1 = value;
            this.NotifyPropertyChanged("In1");
        }
    }

    public double In2
    {
        get
        {
            return this.in2;
        }
        set
        {
            if (Math.Abs((double) (this.in2 - value)) > this.EPSILON)
            {
                this.in2 = value;
                this.NotifyPropertyChanged("In2");
            }
        }
    }

    public double In3
    {
        get
        {
            return this.in3;
        }
        set
        {
            if (Math.Abs((double) (this.in3 - value)) > this.EPSILON)
            {
                this.RaisePropertyChanging("In3");
                this.in3 = value;
                this.RaisePropertyChanged("In3");
            }
        }
    }

    public double In4
    {
        get
        {
            return this.in4;
        }
        set
        {
            if (Math.Abs((double) (this.in4 - value)) > this.EPSILON)
            {
                this.in4 = value;
                this.NotifyPropertyChanged("In4");
            }
        }
    }

    public Visibility In4Visible
    {
        get
        {
            return this._in4Visible;
        }
        set
        {
            if (this._in4Visible != value)
            {
                this._in4Visible = value;
                this.NotifyPropertyChanged("In4Visible");
            }
        }
    }

    public CartesianItems InputConvention
    {
        get
        {
            return this._inputconvention;
        }
       private set
        {
            this._inputconvention = value;
            this.RaisePropertyChanging("InputConvention");
        }
    }

    public CartesianEnum InputItem
    {
        get
        {
            return this._inputItem;
        }
        set
        {
            this._inputItem = value;
            this.NotifyPropertyChanged("InputItem");
        }
    }

    public string InWidth
    {
        get
        {
            return this._inWidth;
        }
        set
        {
            this._inWidth = value;
            this.NotifyPropertyChanged("InWidth");
        }
    }

    public string this[string columnName]
    {
        get
        {
            bool flag = false;
            switch (columnName)
            {
                case "In1":
                    flag = this.IsDouble(this.In1.ToString(CultureInfo.InvariantCulture));
                    break;

                case "In2":
                    flag = this.IsDouble(this.In2.ToString(CultureInfo.InvariantCulture));
                    break;

                case "In3":
                    flag = this.IsDouble(this.In3.ToString(CultureInfo.InvariantCulture));
                    break;

                case "In4":
                    flag = this.IsDouble(this.In4.ToString(CultureInfo.InvariantCulture));
                    break;

                case "Out1":
                    flag = this.IsDouble(this.Out1.ToString(CultureInfo.InvariantCulture));
                    break;

                case "Out2":
                    flag = this.IsDouble(this.Out2.ToString(CultureInfo.InvariantCulture));
                    break;

                case "Out3":
                    flag = this.IsDouble(this.Out3.ToString(CultureInfo.InvariantCulture));
                    break;

                case "Out4":
                    flag = this.IsDouble(this.Out4.ToString(CultureInfo.InvariantCulture));
                    break;

                case "Matrix":
                    flag = true;
                    break;
            }
            if (!flag)
            {
                return string.Format("Value of {0} is not Numeric.", columnName);
            }
            return null;
        }
    }

    public string Matrix {get{return _matrix; }set { _matrix = value;  RaisePropertyChanged("Matrix");  }  }

    public double Out1{get{return out1;}set{if (this.IsDoubleValid(this.out1, value)){out1 = value;RaisePropertyChanging("Out1");}}}
    public double Out2{get{return out2;}set{if (this.IsDoubleValid(this.out2, value)){out2 = value;RaisePropertyChanging("Out2");}}}
    public double Out3{get{return out3;}set{if (this.IsDoubleValid(this.out3, value)){out3 = value;RaisePropertyChanging("Out3");}}}
    public double Out4{get{return out4;}set{if (this.IsDoubleValid(this.out4, value)){out4 = value;RaisePropertyChanging("Out4");}}}

    public Visibility Out4Visible
    {
        get
        {
            return this._out4Visible;
        }
        set
        {
            if (this._out4Visible != value)
            {
                this._out4Visible = value;
                this.NotifyPropertyChanged("Out4Visible");
            }
        }
    }

    public CartesianItems OutputConvention
    {
        get
        {
            return this._outputconvention;
        }
       private set
        {
            this._outputconvention = value;
            this.RaisePropertyChanging("OutputConvention");
        }
    }

    public CartesianEnum OutputItem
    {
        get
        {
            return this._outputItem;
        }
        set
        {
            this._outputItem = value;
            this.NotifyPropertyChanged("OutputItem");
        }
    }

    public string OutWidth
    {
        get
        {
            return this._outWidth;
        }
        set
        {
            this._outWidth = value;
            this.NotifyPropertyChanged("OutWidth");
        }
    }

    // Nested Types
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
            CartesianTypes item = new CartesianTypes {
                ValueCartesianEnum = CartesianEnum.ABB_Quaternion,
                ValueCartesianString = "ABB Quaternion"
            };
            base.Add(item);
            CartesianTypes types2 = new CartesianTypes {
                ValueCartesianEnum = CartesianEnum.Roll_Pitch_Yaw,
                ValueCartesianString = "Roll-Pitch-Yaw"
            };
            base.Add(types2);
            CartesianTypes types3 = new CartesianTypes {
                ValueCartesianEnum = CartesianEnum.Axis_Angle,
                ValueCartesianString = "Axis Angle"
            };
            base.Add(types3);
            CartesianTypes types4 = new CartesianTypes {
                ValueCartesianEnum = CartesianEnum.Kuka_ABC,
                ValueCartesianString = "Kuka ABC"
            };
            base.Add(types4);
            CartesianTypes types5 = new CartesianTypes {
                ValueCartesianEnum = CartesianEnum.Euler_ZYZ,
                ValueCartesianString = "Euler ZYZ"
            };
            base.Add(types5);
            CartesianTypes types6 = new CartesianTypes {
                ValueCartesianEnum = CartesianEnum.Alpha_Beta_Gamma,
                ValueCartesianString = "Alpha-Beta-Gamma"
            };
            base.Add(types6);
        }
    }

    public class CartesianTypes
    {
        // Properties
        public AngleConvertorViewModel.CartesianEnum ValueCartesianEnum { get; set; }

        public string ValueCartesianString { get; set; }
    }       
}

 
 
	
        

    
    
    public enum CartesianEnum
    {
        AbbQuaternion, AxisAngle
    }
}
