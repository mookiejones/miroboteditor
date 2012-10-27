using System;
using System.Collections.Generic;
using System.Globalization;
using ISTUK.MathLibrary;
using System.ComponentModel;
using System.Windows;

namespace miRobotEditor.GUI.AngleConverter
{
    public class AngleConvertor : INotifyPropertyChanged,INotifyPropertyChanging, IDataErrorInfo
    {
        #region Private Members
            private string _matrix = String.Empty;

            /// <summary>
            /// Has Conversion Process Allready Started?
            /// </summary>
            private bool IsConverting; 
          
            private double in1, in2, in3, in4, out1, out2, out3, out4;
            private string _inWidth, _outWidth;
            private CartesianItems _inputconvention = new CartesianItems();
            private CartesianItems _outputconvention = new CartesianItems();
            private Visibility _out4Visible = Visibility.Visible;
            private Visibility _in4Visible= Visibility.Visible;
            private CartesianEnum _inputItem, _outputItem;
            private RotationMatrix3D rotationMatrix = RotationMatrix3D.Identity();

            public Visibility Out4Visible
            {
                get { return _out4Visible; }
                set
                {
                    if (_out4Visible != value)
                    {
                        _out4Visible = value;
                        NotifyPropertyChanged("Out4Visible");
                    }
                }
            }
            public Visibility In4Visible
            {
                get { return _in4Visible; }
                set
                {
                    if (_in4Visible != value)
                    {
                        _in4Visible = value;
                        NotifyPropertyChanged("In4Visible");
                    }
                }
            }
        #endregion


        /// <summary>
        /// Tests Double Value
        /// <remarks> This is created because if 
        /// a = NaN
        /// b= NaN
        /// a.Equals(b) will return a false
        /// </remarks>
        /// </summary>
        /// <param name="_old"></param>
        /// <param name="_new"></param>
        /// <returns></returns>
        private bool IsDoubleValid(double _old, double _new)
        {
            if (!_old.Equals(_new))
                {
                    if (double.IsNaN(_old) & double.IsNaN(_new))
                        return false;
                    return true;
                }
            return false;
        }

        #region Public  Dependency Properties

        public CartesianEnum InputItem { get { return _inputItem; } set { _inputItem = value; NotifyPropertyChanged("InputItem"); } }
        public CartesianEnum OutputItem { get { return _outputItem; } set { _outputItem = value; NotifyPropertyChanged("OutputItem"); } }

        public string InWidth
        {
            get
            {
                return _inWidth;
            }
            set
            {
                _inWidth = value;              
                NotifyPropertyChanged("InWidth");              
            }
        }

        public string OutWidth
        {
            get { return _outWidth; }
            set
            {
                _outWidth = value;
                NotifyPropertyChanged("OutWidth");
            }
        }

        public double In1
        {
            get { return in1; }
            set
            {               
                    in1 = value;
                    NotifyPropertyChanged("In1");              
            }
        }
        public double In2
        {
            get { return in2; }
            set
            {
                if (Math.Abs(in2 - value) > EPSILON)
                {
                    in2 = value;
                    NotifyPropertyChanged("In2");
                }
            }
        }
        public double In3
        {
            get { return in3; }
            set
            {
                if (Math.Abs(in3 - value) > EPSILON)
                {
                    
                    NotifyPropertyChanging("In3");
                    in3 = value;
                    NotifyPropertyChanged("In3");
                }
            }
        }

        protected double EPSILON
        {
            get { return Double.Epsilon; }
            set { throw new NotImplementedException(); }
        }

        public double In4
        {
            get { return in4; }
            set
            {
                if (Math.Abs(in4 - value) > EPSILON)
                {
                    in4 = value;
                    NotifyPropertyChanged("In4");
                }
            }
        }
        public double Out1
        {
            get { return out1; }
            set
            {
                if (IsDoubleValid(out1,value))
                {
                    out1 = value;
                    NotifyPropertyChanged("Out1");
                }
            }
        }

        public double Out2
        {
            get { return out2; }
            set
            {
                if (IsDoubleValid(out2 ,value))
                {
                    out2 = value;
                    NotifyPropertyChanged("Out2");
                }

              }
        }
        public double Out3
        {
            get { return out3; }
            set
            {
                if (IsDoubleValid(out3,value))
                {
                    out3 = value;
                    NotifyPropertyChanged("Out3");
                }
            }
        }
        public double Out4
        {
            get { return out4; }
            set
            {
                if (IsDoubleValid(out4 ,value))
                {
                    out4 = value;
                    NotifyPropertyChanged("Out4");
                }
            }
        }
        public CartesianItems InputConvention { get { return _inputconvention; } set { _inputconvention = value; NotifyPropertyChanging("InputConvention"); } }
        public CartesianItems OutputConvention { get { return _outputconvention; } set { _outputconvention = value; NotifyPropertyChanging("OutputConvention"); } }

        public string Matrix
        {
            get { return _matrix; }
            set
            {
                _matrix = value;
                NotifyPropertyChanging("Matrix");
            }
        }

        #endregion

        public void Convert()
        {
            // Return if conversion process is started
            // Prevents multiple triggers
            if (IsConverting) return;

            // Start Conversion Process
            IsConverting = true;

            var aBC = new Vector3D();
            var scalar = 0.0;
            var _rotationMatrix = new Quaternion();

            int nValuesNeeded = 3;
           
                switch (InputItem)
                {
                    case CartesianEnum.Euler_ZYZ:
                        rotationMatrix = RotationMatrix3D.FromEulerZYZ(In1, In2, In3);
                        break;
                    case CartesianEnum.Roll_Pitch_Yaw:
                        rotationMatrix = RotationMatrix3D.FromRPY(In1, In2, In3);
                        break;
                    case CartesianEnum.ABB_Quaternion:
                        _rotationMatrix = new Quaternion(In2, In3, In4, In1);
                        rotationMatrix = rotationMatrix as RotationMatrix3D;
                        nValuesNeeded = 4;
                        break;
                    case CartesianEnum.Axis_Angle:
                        rotationMatrix = RotationMatrix3D.RotateAroundVector(new Vector3D(In1, In2, In3), In4);
                        nValuesNeeded = 4;
                        break;
                    case CartesianEnum.Kuka_ABC:
                        rotationMatrix = RotationMatrix3D.FromABC(In1, In2, In3);
                        break;
                }

                In4Visible = (nValuesNeeded.Equals(3)) ? Visibility.Hidden : Visibility.Visible;
                InWidth = (nValuesNeeded.Equals(3)) ? "33*" : "25*";
                nValuesNeeded = 3;
            switch (OutputItem)
                {
                    case CartesianEnum.Kuka_ABC: aBC = rotationMatrix.ABC; break;
                    case CartesianEnum.Euler_ZYZ: aBC = rotationMatrix.EulerZYZ; break;
                    case CartesianEnum.Roll_Pitch_Yaw: aBC = rotationMatrix.RPY; break;
                    case CartesianEnum.Alpha_Beta_Gamma: aBC = rotationMatrix.ABG; break;
                    case CartesianEnum.ABB_Quaternion:
                        _rotationMatrix = (Quaternion)rotationMatrix;
                        aBC = _rotationMatrix.Vector;
                        scalar = _rotationMatrix.Scalar;
                        nValuesNeeded = 4;
                        break;

                    case CartesianEnum.Axis_Angle:
                        aBC = rotationMatrix.RotationAxis();
                        scalar = rotationMatrix.RotationAngle();
                        nValuesNeeded = 4;
                        break;
                }

                string _result = _rotationMatrix.ToString("F3");
                if (Matrix != null && Matrix != _result)
                    Matrix = _result;
                WriteValues(aBC,0.0,false);

                Out4Visible = (nValuesNeeded.Equals(3)) ? Visibility.Collapsed : Visibility.Visible;
                OutWidth = (nValuesNeeded.Equals(3)) ? "33*" : "25*";

                if (nValuesNeeded == 4)
                {
                    switch (OutputItem)
                    {
                        case CartesianEnum.ABB_Quaternion:
                            WriteValues(aBC, scalar, true);
                            break;
                        default:
                            Out4 = scalar;
                            break;
                    }
                }
                //Finished Conversion Process
                IsConverting = false;
            }
        
        private void WriteValues(Vector3D Result, double scalar, bool isScalar)
        {
            switch (isScalar)
            {
                case true:
                    Out1 = scalar;
                    Out2 = Result.X;
                    Out3 = Result.Y;
                    Out4 = Result.Z;
                    Out4Visible = Visibility.Visible;
                    break;
                case false:
                    Out1 = Result.X;
                    Out2 = Result.Y;
                    Out3 = Result.Z;
                    Out4Visible = Visibility.Hidden;
                    break;
            }
        }

        #region IDataErrorInfo Members

        public string Error
        {
            get { return null; }
        }
        private bool IsDouble(string Value)
        {
            double result;
            return double.TryParse(Value, out result);
        }
        public string this[string columnName]
        {
            get
            {
                bool valid = false;
                switch (columnName)
                {
                    case "In1": valid = IsDouble(Value: In1.ToString(provider: CultureInfo.InvariantCulture)); break;
                    case "In2": valid = IsDouble(Value: In2.ToString(CultureInfo.InvariantCulture)); break;
                    case "In3": valid = IsDouble(Value: In3.ToString(CultureInfo.InvariantCulture)); break;
                    case "In4": valid = IsDouble(Value: In4.ToString(CultureInfo.InvariantCulture)); break;
                    case "Out1": valid = IsDouble(Value: Out1.ToString(CultureInfo.InvariantCulture)); break;
                    case "Out2": valid = IsDouble(Value: Out2.ToString(CultureInfo.InvariantCulture)); break;
                    case "Out3": valid = IsDouble(Value: Out3.ToString(CultureInfo.InvariantCulture)); break;
                    case "Out4": valid = IsDouble(Value: Out4.ToString(CultureInfo.InvariantCulture)); break;
                    case "Matrix": valid = true; break;
                }

                if (!valid) return String.Format("Value of {0} is not Numeric.", columnName);

                return null;

            }


        #endregion
        }

        public class CartesianItems :List<CartesianTypes>
        {
            public CartesianItems()
            {
                Add(new CartesianTypes { ValueCartesianEnum = CartesianEnum.ABB_Quaternion, ValueCartesianString = "ABB Quaternion" });
                Add(new CartesianTypes { ValueCartesianEnum = CartesianEnum.Roll_Pitch_Yaw, ValueCartesianString = "Roll-Pitch-Yaw" });
                Add(new CartesianTypes { ValueCartesianEnum = CartesianEnum.Axis_Angle, ValueCartesianString = "Axis Angle" });
                Add(new CartesianTypes { ValueCartesianEnum = CartesianEnum.Kuka_ABC, ValueCartesianString = "Kuka ABC" });
                Add(new CartesianTypes { ValueCartesianEnum = CartesianEnum.Euler_ZYZ, ValueCartesianString = "Euler ZYZ" });
                Add(new CartesianTypes { ValueCartesianEnum = CartesianEnum.Alpha_Beta_Gamma, ValueCartesianString = "Alpha-Beta-Gamma"});

            }
        }
        public enum CartesianEnum
        {
            ABB_Quaternion = 0,
            Roll_Pitch_Yaw=1,            
            Axis_Angle=2,
            Kuka_ABC=3,
            Euler_ZYZ=4,
            Alpha_Beta_Gamma=5,
        }

        public class CartesianTypes
        {
            public CartesianEnum ValueCartesianEnum { get; set; }
            public string ValueCartesianString {get;set;}
        }

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;



        public void NotifyPropertyChanging(string propertyname)
        {

            OnPropertyChanging(new PropertyChangingEventArgs(propertyname));
        }


        protected virtual void OnPropertyChanging(PropertyChangingEventArgs e)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, e);
            }
        }

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            Convert();
        }
    }
}
     

