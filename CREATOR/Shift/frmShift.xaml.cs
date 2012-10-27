using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace CREATOR.Shift
{
    /// <summary>
    /// Interaction logic for frmShift.xaml
    /// </summary>
    public partial class frmShift : Window, INotifyPropertyChanged
    {
        private string _txtFilter;

        public string txtFilter
        {
            get { return _txtFilter; }
            set
            {
                _txtFilter = value;
                OnPropertyChanged("txtFilter");
            }
        }
        [DefaultValue("X 0.0, Y 0.0, Z 0.0, A 0.0, B 0.0, C 0.0")]
        public string NewBase { get; set; }
        [DefaultValue("X 0.0, Y 0.0, Z 0.0, A 0.0, B 0.0, C 0.0")]
        public string OldBase { get; set; }

                [DefaultValue("X 0.0, Y 0.0, Z 0.0, A 0.0, B 0.0, C 0.0")]
        public string OldTool { get; set; }
                [DefaultValue("X 0.0, Y 0.0, Z 0.0, A 0.0, B 0.0, C 0.0")]
                public string NewTool { get; set; }
        public Frames Tools { get; set; }
        public Frames Bases { get; set; }
        public Filters Filters { get; set; }

        public frmShift()
        {
            InitializeComponent();

            DataContext = this;
            this.Activate();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Execute(object sender, RoutedEventArgs e)
        {
            int iEx = 0;
            float num = 0f;
            FrameConverter.Frame frame;
            try
            {
                if (this.TransformType.SelectedIndex == 0)
                {
                    frame.X = Convert.ToDouble(TransformPosition.X);
                    frame.Y = Convert.ToDouble(TransformPosition.Y);
                    frame.Z = Convert.ToDouble(TransformPosition.Z);
                    frame.A = Convert.ToDouble(TransformPosition.A);
                    frame.B = Convert.ToDouble(TransformPosition.B);
                    frame.C = Convert.ToDouble(TransformPosition.C);

                    num = Convert.ToSingle(External.Text);
                    if (num != 0f)
                    {
                        iEx = checked(this.cmbExternal.SelectedIndex + 1);
                    }
                }
                else
                {
                    if (this.TransformType.SelectedIndex == 1)
                    {
                        frame = FrameConverter.Doppelpunkt(FrameConverter.Str2Frame(OldBase),
                                                           FrameConverter.GetInverseFrame(
                                                               FrameConverter.Str2Frame(NewBase)));
                    }
                    else
                    {
                        if (this.TransformType.SelectedIndex == 2)
                        {
                            frame = FrameConverter.Doppelpunkt(FrameConverter.Str2Frame(oldtool),
                                                               GetInverseFrame(FrameConverter.Str2Frame(newtool)));
                        }
                    }
                }
                var text = string.Format("Translation: total {0}mm, X {1}, Y {2}, Z {3}",String.Format(GetVectorTrans(frame), "0.###"), String.Format(frame.X, "0.0##"),String.Format(frame.Y, "0.0##"), String.Format(frame.Z, "0.0##"));
                var text2 = String.Format("Rotation: A {0}, B {1}, C {2}", String.Format(frame.A, "0.0##"),String.Format(frame.B, "0.0##"), String.Format(frame.C, "0.0##"), "");
                var result= MessageBox.Show(String.Format("{0}\n{1}\n\n Its not possible to undo this action!\\n\\nContinue?", text, text2),"", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes)
                {
                    return;
                }
            }
            catch (Exception except)
            {
               // ProjectData.SetProjectError(except);
                Exception ex = except;
              //  MyProject.Forms.FormMain.SendStatusMsg(ex.Message, "!", 0, 0);
               // ProjectData.ClearProjectError();
                return;
            }
            try
            {
                this.Progress.Minimum = 0;
                this.Progress.Value = 0;
                this.Progress.Maximum = this.ListView1.CheckedItems.Count;
                this.Progress.Visibility = Visibility.Visible;
                try
                {
                    IEnumerator enumerator = this.ListView1.CheckedItems.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        ListViewItem listViewItem = (ListViewItem) enumerator.Current;
//                        this.Progress.PerformStep();
                        //                       this.Progress.Update();
                        if (String.Compare(listViewItem.Text, "", false) != 0)
                        {                            
                            var frame2 = FrameConverter.Str2Frame(listViewItem.ToString());
                        }
                        Frame fIn;
                        if (this.TransformType.SelectedIndex == 0)
                        {
                            fIn = this.Base.IsChecked == true ? FrameConverter.Doppelpunkt(frame, frame2) : FrameConverter.Doppelpunkt(frame2, frame);
                        }
                        else
                        {
                            if (this.TransformType.SelectedIndex == 1)
                            {
                                fIn = FrameConverter.Doppelpunkt(frame, frame2);
                            }
                            else
                            {
                                if (this.TransformType.SelectedIndex == 2)
                                {
                                    fIn = FrameConverter.Doppelpunkt(frame2, frame);
                                }
                            }
                        }
                        listViewItem.SubItems[4].Text = FrameConverter.SetFrame2String(listViewItem.SubItems[4].Text, fIn,
                                                                                iEx, num);
                        if (this.Progress.Value < this.Progress.Maximum)
                        {
                            MyProject.Forms.FormMain.VarSet(listViewItem.Text, listViewItem.SubItems[4].Text,
                                                            listViewItem.SubItems[5].Text, "", false);
                        }
                        else
                        {
                            MyProject.Forms.FormMain.VarSet(listViewItem.Text, listViewItem.SubItems[4].Text,
                                                            listViewItem.SubItems[5].Text, "", true);
                        }
                        this.ListView1.Refresh();
                    }
                }

                finally
                {
                    IEnumerator enumerator = null;
                    if (enumerator is IDisposable)
                    {
                        (enumerator as IDisposable).Dispose();
                    }
                }
                this.Progress.Value = this.Progress.Maximum;


            }
            catch (Exception expr_549)
            {
//                ProjectData.SetProjectError(expr_549);
                MessageBox.Show("Error while transforming positions", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                this.Progress.Visibility = Visibility.Collapsed;
//                ProjectData.ClearProjectError();
            }
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    public class Filters : ObservableCollection<string>
    {
        public Filters()
        {
            Add("Actual SRC");
            Add("Actual Dat");
            Add("Actual Selection");
            Add("actual SRC");
            Add("All SRC's");
            Add("All Dat's");
        }
    }

    public class Frames : ObservableCollection<String>
    {
        //TODO Need to eventually parse the information from the $Config.Dat File into this
        public Frames()
        {
            Add("All");
            Add("1");
            Add("2");
            Add("3");
            Add("4");
            Add("5");
            Add("6");
            Add("7");
            Add("8");
            Add("9");
            Add("10");
            Add("11");
            Add("12");
            Add("13");
            Add("14");
            Add("15");
            Add("16");
        }
    }

    public static class FrameConverter
    {
        public static string[] Split2(string sString, string sSep = " ")
        {
            string[] array = System.Text.RegularExpressions.Regex.Split(sString, sSep);
            int num = 0;
            string[] array2 = new string[]
	{
		sString
	};
            string[] array3 = array;
            checked
            {
                for (int i = 0; i < array3.Length; i++)
                {
                    string text = array3[i];
                    text = text.Trim();
                    if (String.Compare(text,"",false)!=0)
                    {
                        if (num > 0)
                        {
                       //     array2 = (string[])CopyArray((Array)array2, new string[num + 1]);
                        }
                        array2[num] = text;
                        num++;
                    }
                }
                return array2;
            }
        }
        public struct Frame
        {
            public double X;
            public double Y;
            public double Z;
            public double A;
            public double B;
            public double C;
        }
        public static string SetFrame2String(string sStr, Frame fIn, int iEx = 0, float sExVal = 0f)
        {
            string text1 = "X 0.0";
            string text2 = "Y 0.0";
            string text3 = "Z 0.0";
            string text4 = "A 0.0";
            string text5 = "B 0.0";
            string text6 = "C 0.0";
            sStr = sStr.Replace("}", "").Replace("{", "").Replace(",", " ");
            string[] array = FrameConverter.Split2(sStr, " ");
            int arg_76_0 = 0;
            checked
            {
                int num = array.Length - 1;
                string text7=string.Empty;
                string text8=string.Empty;
                string text9=string.Empty;
                string text10=string.Empty;
                string text11=string.Empty;
                string text12=string.Empty;
                string text13=string.Empty;
                string text14=string.Empty;
                for (int i = arg_76_0; i <= num; i++)
                {
                    array[i].Trim();
                    if (System.String.CompareOrdinal(array[i].ToLower(), "x") == 0)
                        text1 = "X " + ((float) fIn.X).ToString(CultureInfo.InvariantCulture).Replace(sKomma, ".");
                    if (System.String.CompareOrdinal(array[i].ToLower(), "y") == 0)
                        text2 = "Y " + ((float) fIn.X).ToString(CultureInfo.InvariantCulture).Replace(sKomma, ".");
                    if (System.String.CompareOrdinal(array[i].ToLower(), "z") == 0)
                        text3 = "Z " + ((float) fIn.X).ToString(CultureInfo.InvariantCulture).Replace(sKomma, ".");
                    if (System.String.CompareOrdinal(array[i].ToLower(), "a") == 0)
                        text4 = "A " + ((float) fIn.X).ToString(CultureInfo.InvariantCulture).Replace(sKomma, ".");
                    if (System.String.CompareOrdinal(array[i].ToLower(), "b") == 0)
                        text5 = "B " + ((float) fIn.X).ToString(CultureInfo.InvariantCulture).Replace(sKomma, ".");
                    if (System.String.CompareOrdinal(array[i].ToLower(), "c") == 0)
                        text6 = "C " + ((float) fIn.X).ToString(CultureInfo.InvariantCulture).Replace(sKomma, ".");
                    if (System.String.CompareOrdinal(array[i].ToLower(), "s") == 0)
                        text7 = "S " + array[i + 1];
                    if (System.String.CompareOrdinal(array[i].ToLower(), "t") == 0)
                        text8 = "T " + array[i + 1];
                    if (System.String.CompareOrdinal(array[i].ToLower(), "e1") == 0)
                    {
                        if (iEx == 1)
                        {
                            float num2 = Convert.ToSingle(array[i + 1].Replace(".", sKomma));
                            array[i + 1] = Convert.ToString(unchecked(num2 + sExVal));
                        }
                        text9 = "E1 " + array[i + 1].ToString().Replace(sKomma, ".");
                    }
                    if (System.String.CompareOrdinal(array[i].ToLower(), "e2") == 0)
                    {
                        if (iEx == 2)
                        {
                            float num2 = Convert.ToSingle(array[i + 1].Replace(".", sKomma));
                            array[i + 1] = Convert.ToString(unchecked(num2 + sExVal));
                        }
                        text9 = "E2 " + array[i + 1].ToString().Replace(sKomma, ".");
                    }
                    if (System.String.CompareOrdinal(array[i].ToLower(), "e3") == 0)
                    {
                        if (iEx == 3)
                        {
                            float num2 = Convert.ToSingle(array[i + 1].Replace(".", sKomma));
                            array[i + 1] = Convert.ToString(unchecked(num2 + sExVal));
                        }
                        text9 = "E3 " + array[i + 1].ToString().Replace(sKomma, ".");
                    }
                    if (System.String.CompareOrdinal(array[i].ToLower(), "e4") == 0)
                    {
                        if (iEx == 4)
                        {
                            float num2 = Convert.ToSingle(array[i + 1].Replace(".", sKomma));
                            array[i + 1] = Convert.ToString(unchecked(num2 + sExVal));
                        }
                        text9 = "E4 " + array[i + 1].ToString().Replace(sKomma, ".");
                    }
                    if (System.String.CompareOrdinal(array[i].ToLower(), "e5") == 0)
                    {
                        if (iEx == 5)
                        {
                            float num2 = Convert.ToSingle(array[i + 1].Replace(".", sKomma));
                            array[i + 1] = Convert.ToString(unchecked(num2 + sExVal));
                        }
                        text9 = "E5 " + array[i + 1].ToString().Replace(sKomma, ".");
                    }
                    if (System.String.CompareOrdinal(array[i].ToLower(), "e6") == 0)
                    {
                        if (iEx == 6)
                        {
                            float num2 = Convert.ToSingle(array[i + 1].Replace(".", sKomma));
                            array[i + 1] = Convert.ToString(unchecked(num2 + sExVal));
                        }
                        text9 = "E6 " + array[i + 1].ToString().Replace(sKomma, ".");
                    }




                 
                }
                string str = string.Format("{{{0},{1},{2},{3},{4},{5},{6}", text1, text2, text3, text4, text5, text6);

                if (!String.IsNullOrEmpty(text7)) str += "," + text7;
                if (!String.IsNullOrEmpty(text8)) str += "," + text8;
                if (!String.IsNullOrEmpty(text9)) str += "," + text9;
                if (!String.IsNullOrEmpty(text10)) str += "," + text10;
                if (!String.IsNullOrEmpty(text11)) str += "," + text11;
                if (!String.IsNullOrEmpty(text12)) str += "," + text12;
                if (!String.IsNullOrEmpty(text13)) str += "," + text13;
                if (!String.IsNullOrEmpty(text14)) str += "," + text14;
                    
                return str + "}";
            }
        }
        public static Frame Doppelpunkt(Frame f1, Frame f2)
        {
            var mat = new double[3,3];
            var mat2 = new double[3,3];
            var mat3 = new double[3,3];
            var array = new double[3,3];
            var mat4 = new double[3,3];
            var mat5 = new double[3,3];
            var mat6 = new double[3,3];
            var mat7 = new double[3,3];
            var m = new double[3,3];
            var array2 = new double[4,4];
            var array3 = new double[4,4];
            var array4 = new double[4,4];
            var array5 = new double[1,4];
            var array6 = new double[1,4];
            var array7 = new double[1,4];
            var array8 = new double[4,1];
            var array9 = new double[4,4];
            var array10 = new double[4,4];
            var array11 = new double[4,4];
            mat = EulerAToMatrix(f1.A);
            mat2 = EulerBToMatrix(f1.B);
            mat3 = EulerCToMatrix(f1.C);
            array = Multiply(Multiply(mat, mat2), mat3);
            mat4 = EulerAToMatrix(f2.A);
            mat5 = EulerBToMatrix(f2.B);
            mat6 = EulerCToMatrix(f2.C);
            mat7 = Multiply(Multiply(mat4, mat5), mat6);
            m = Multiply(array, mat7);
            Frame result;
            result.A = MatrixToEulerA(m);
            result.B = MatrixToEulerB(m);
            result.C = MatrixToEulerC(m);
            int num = 0;
            checked
            {
                do
                {
                    int num2 = 0;
                    do
                    {
                        array3[num, num2] = array[num, num2];
                        num2++;
                    } while (num2 <= 2);
                    num++;
                } while (num <= 2);
                array3[3, 3] = 1.0;
                array2[0, 0] = 1.0;
                array2[1, 1] = 1.0;
                array2[2, 2] = 1.0;
                array2[3, 3] = 1.0;
                array2[0, 3] = f1.X;
                array2[1, 3] = f1.Y;
                array2[2, 3] = f1.Z;
                array4 = Multiply(array2, array3);
                num = 0;
                do
                {
                    array5[0, num] = array4[0, num];
                    array6[0, num] = array4[1, num];
                    array7[0, num] = array4[2, num];
                    num++;
                } while (num <= 3);
                array8[0, 0] = f2.X;
                array8[1, 0] = f2.Y;
                array8[2, 0] = f2.Z;
                array8[3, 0] = 1.0;
                array9 = Multiply(array5, array8);
                array10 = Multiply(array6, array8);
                array11 = Multiply(array7, array8);
                result.X = array9[0, 0];
                result.Y = array10[0, 0];
                result.Z = array11[0, 0];
                return result;
            }
        }

        private static double RadToEuler(double Rad)
        {
            Rad = Rad*180.0/3.1415926535897931;
            return Rad;
        }

        private static double MatrixToEulerC(double[,] m)
        {
            double num = 0.0;
            double num2 = Math.Atan(m[2, 1]/m[2, 2]);
            if (m[2, 2] < 0.0)
            {
                num = 180.0;
            }
            num2 = RadToEuler(num2) + num;
            if (num2 > 180.0)
            {
                num2 -= 360.0;
            }
            if (num2 < -180.0)
            {
                num2 += 360.0;
            }
            return num2;
        }

        private static double MatrixToEulerB(double[,] m)
        {
            double num = Math.Asin(-m[2, 0]);
            num = RadToEuler(num);
            if (num > 180.0)
            {
                num -= 360.0;
            }
            if (num < -180.0)
            {
                num += 360.0;
            }
            return num;
        }

        private static double MatrixToEulerA(double[,] m)
        {
            double num = 0.0;
            double num2 = Math.Atan(m[1, 0]/m[0, 0]);
            if (m[0, 0] < 0.0)
            {
                num = 180.0;
            }
            num2 = RadToEuler(num2) + num;
            if (num2 > 180.0)
            {
                num2 -= 360.0;
            }
            if (num2 < -180.0)
            {
                num2 += 360.0;
            }
            return num2;
        }

        private static double[,] EulerAToMatrix(double A)
        {
            var array = new double[3,3];
            A = EulerToRad(A);
            array[0, 0] = Math.Cos(A);
            array[0, 1] = -Math.Sin(A);
            array[1, 0] = Math.Sin(A);
            array[1, 1] = Math.Cos(A);
            array[2, 2] = 1.0;
            return array;
        }

        private static double[,] EulerBToMatrix(double B)
        {
            var array = new double[3,3];
            B = EulerToRad(B);
            array[0, 0] = Math.Cos(B);
            array[0, 2] = Math.Sin(B);
            array[2, 0] = -Math.Sin(B);
            array[2, 2] = Math.Cos(B);
            array[1, 1] = 1.0;
            return array;
        }

        private static double[,] EulerCToMatrix(double C)
        {
            var array = new double[3,3];
            C = EulerToRad(C);
            array[1, 1] = Math.Cos(C);
            array[1, 2] = -Math.Sin(C);
            array[2, 1] = Math.Sin(C);
            array[2, 2] = Math.Cos(C);
            array[0, 0] = 1.0;
            return array;
        }

        private static double EulerToRad(double Euler)
        {
            Euler = Euler*3.1415926535897931/180.0;
            return Euler;
        }

        public static double[,] Multiply(double[,] Mat1, double[,] Mat2)
        {
            checked
            {
                int num;
                double[,] array2;
                int num10;
                try
                {
                    
                    num = 2;
                    double num2 = 0.0;
                    int num3;
                    int num4;
                    Find_R_C(Mat1, ref num3, ref num4);
                    int num5;
                    int num6;
                    Find_R_C(Mat2, ref num5, ref num6);
                    if (num4 == num5)
                    {
                        double[,] array = new double[num3 + 1,num6 + 1];
                        int arg_3F_0 = 0;
                        int num7 = num3;
                        for (int i = arg_3F_0; i <= num7; i++)
                        {
                            int arg_4B_0 = 0;
                            int num8 = num6;
                            for (int j = arg_4B_0; j <= num8; j++)
                            {
                                int arg_56_0 = 0;
                                int num9 = num4;
                                for (int k = arg_56_0; k <= num9; k++)
                                {
                                    unchecked
                                    {
                                        num2 += Mat1[i, k]*Mat2[k, j];
                                    }
                                }
                                array[i, j] = num2;
                                num2 = 0.0;
                            }
                        }
                        array2 = array;
                        goto IL_170;
                    }
                    throw new ShiftException(5009, null,"Dimensions of the two matrices not suitable for multiplication !", null,null);
                    IL_D4:
                    if (Information.Err().Number == 5009)
                    {
                        throw new ShiftException(5009, null,"Dimensions of the two matrices not suitable for multiplication !", null,null);

                        
                        goto IL_170;
                    }

                    throw new ShiftException(5022,null,"One OrElse both of the matrices are null, this operation cannot be done !!",null, null);

                    goto IL_170;
                    num10 = -1;
                //    @switch(ICSharpCode.Decompiler.ILAst.ILLabel[], num);
                    IL_13F:
                    goto IL_165;
                }
                catch(Exception ex)
                {
                    
                }
                object arg_144_0;
                endfilter(arg_144_0 is Exception & num > 0 & num10 == 0);
                IL_165:
                throw ProjectData.CreateProjectError(-2146828237);
                IL_170:
                double[,] arg_17E_0 = array2;
                if (num10 != 0)
                {
                    ProjectData.ClearProjectError();
                }
                return arg_17E_0;
            }
        }

        private static void Find_R_C(double[,] Mat, ref int Row, ref int Col)
        {
            Row = Mat.GetUpperBound(0);
            Col = Mat.GetUpperBound(1);
        }

        public static string sKomma = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;

        public static Frame Str2Frame(string sStr)
        {
              
            checked
            {
                var result = new Frame();

                try
                {
                    sStr = sStr.Replace("}", "").Replace("{", "").Replace(",", " ");
                    string[] array = null;
                    int arg_4D_0 = 0;
                    int num = array.Length - 1;
                    double x = 0.0f;
                    double y = 0.0f;
                    double z = 0.0f;
                    double a = 0.0f;
                    double b = 0.0f;
                    double c = 0.0f;
                    for (int i = arg_4D_0; i <= num; i++)
                    {
                        array[i].Trim();
                        if (System.String.Compare(array[i], "x", System.StringComparison.OrdinalIgnoreCase) == 0)
                            result.X = Convert.ToDouble(array[i + 1].Replace(".", sKomma));
                        if (System.String.Compare(array[i], "y", System.StringComparison.OrdinalIgnoreCase) == 0)
                            result.Y = Convert.ToDouble(array[i + 1].Replace(".", sKomma));
                        if (System.String.Compare(array[i], "z", System.StringComparison.OrdinalIgnoreCase) == 0)
                            result.Z = Convert.ToDouble(array[i + 1].Replace(".", sKomma));
                        if (System.String.Compare(array[i], "a", System.StringComparison.OrdinalIgnoreCase) == 0)
                            result.A = Convert.ToDouble(array[i + 1].Replace(".", sKomma));
                        if (System.String.Compare(array[i], "b", System.StringComparison.OrdinalIgnoreCase) == 0)
                            result.B = Convert.ToDouble(array[i + 1].Replace(".", sKomma));
                        if (System.String.Compare(array[i], "c", System.StringComparison.OrdinalIgnoreCase) == 0)
                            result.C = Convert.ToDouble(array[i + 1].Replace(".", sKomma));
                    }
                }
                catch (Exception expr_20C)
                {
                }
               
                    return result;
            }

        }
        // OrangeEdit.MatLib
        public static double Det(double[,] Mat)
{
	checked
	{
		int num;
		double num5;
		int num13;
		try
		{
			num = 2;
			int num2 = 0;
			int num3 = 0;
			Find_R_C(Mat, ref num2, ref num3);
			if (num2 == num3)
			{
				int num4 = num2;
				num5 = 1.0;
				var array = (double[,])Mat.Clone();
				int arg_36_0 = 0;
				int num6 = num4;
				for (int i = arg_36_0; i <= num6; i++)
				{
					if (array[i, i] == 0.0)
					{
						int j = i;
						while (j < num4 && array[i, j] == 0.0)
						{
							j++;
						}
						if (array[i, j] == 0.0)
						{
							num5 = 0.0;
							break;
						}
						int arg_B1_0 = i;
						int num7 = num4;
						for (int k = arg_B1_0; k <= num7; k++)
						{
							double num8 = array[k, j];
							array[k, j] = array[k, i];
							array[k, i] = num8;
						}
						num5 = unchecked(-num5);
					}
					double num9 = array[i, i];
					unchecked
					{
						num5 *= num9;
					}
					if (i < num4)
					{
						int num10 = i + 1;
						int arg_120_0 = num10;
						int num11 = num4;
						for (int k = arg_120_0; k <= num11; k++)
						{
							int arg_12C_0 = num10;
							int num12 = num4;
							for (int j = arg_12C_0; j <= num12; j++)
							{
								unchecked
								{
									array[k, j] -= array[k, i] * (array[i, j] / num9);
								}
							}
						}
					}
				}
				goto IL_248;
			}
//			Information.Err().Raise(Conversions.ToInteger("5011"), null, "Matrix should be a square matrix !", null, null);
			IL_1AC:
            //if (Information.Err().Number == 5011)
            //{
            //    Information.Err().Raise(Conversions.ToInteger("5011"), null, "Matrix should be a square matrix !", null, null);
            //    goto IL_248;
            //}
            //Information.Err().Raise(Conversions.ToInteger("5022"), null, "In order to do this operation values must be assigned to the matrix !!", null, null);
			goto IL_248;
			num13 = -1;
			IL_217:
			goto IL_23D;
		}
        catch
        {
        }
	    object arg_21C_0;
		//endfilter(arg_21C_0 is Exception & num > 0 & num13 == 0);
		IL_23D:
		throw ProjectData.CreateProjectError(-2146828237);
		IL_248:
		double arg_256_0 = num5;
		if (num13 != 0)
		{
			//ProjectData.ClearProjectError();
		}
		return arg_256_0;
	}
}

        public static double[,] Inv(double[,] Mat)
{
	checked
	{
		int num;
		double[,] array3;
		int num15;
		try
		{
			num = 2;
			int num2;
			int num3;
			Find_R_C(Mat, ref num2, ref num3);
			if (num2 == num3)
			{
				if (Det(Mat) != 0.0)
				{
					int num4 = num2;
					double[,] array = (double[,])Mat.Clone();
					double[,] array2 = new double[num4 + 1, num4 + 1];
					int arg_54_0 = 0;
					int num5 = num4;
					for (int i = arg_54_0; i <= num5; i++)
					{
						int arg_5F_0 = 0;
						int num6 = num4;
						for (int j = arg_5F_0; j <= num6; j++)
						{
							array2[j, i] = 0.0;
						}
						array2[i, i] = 1.0;
					}
					int arg_AA_0 = 0;
					int num7 = num4;
					for (int k = arg_AA_0; k <= num7; k++)
					{
						if (Math.Abs(array[k, k]) < 1E-10)
						{
							int arg_D6_0 = k + 1;
							int num8 = num4;
							for (int l = arg_D6_0; l <= num8; l++)
							{
								if (l != k && Math.Abs(array[k, l]) > 1E-10)
								{
									int arg_108_0 = 0;
									int num9 = num4;
									for (int m = arg_108_0; m <= num9; m++)
									{
										unchecked
										{
											array[m, k] += array[m, l];
											array2[m, k] += array2[m, l];
										}
									}
									break;
								}
							}
						}
						double num10 = 1.0 / array[k, k];
						int arg_19C_0 = 0;
						int num11 = num4;
						for (int l = arg_19C_0; l <= num11; l++)
						{
							unchecked
							{
								array[l, k] = num10 * array[l, k];
								array2[l, k] = num10 * array2[l, k];
							}
						}
						int arg_1E5_0 = 0;
						int num12 = num4;
						for (int l = arg_1E5_0; l <= num12; l++)
						{
							if (l != k)
							{
								double num13 = array[k, l];
								int arg_206_0 = 0;
								int num14 = num4;
								for (int m = arg_206_0; m <= num14; m++)
								{
									unchecked
									{
										array[m, l] -= num13 * array[m, k];
										array2[m, l] -= num13 * array2[m, k];
									}
								}
							}
						}
					}
					array3 = array2;
					goto IL_37B;
				}
				Information.Err().Raise(Conversions.ToInteger("5012"), null, "Determinent equals zero, inverse can't be found !", null, null);
			}
			Information.Err().Raise(Conversions.ToInteger("5014"), null, "Matrix should be a square matrix !", null, null);
			IL_2C9:
			if (Information.Err().Number == 5012)
			{
				Information.Err().Raise(Conversions.ToInteger("5012"), null, "Determinent equals zero, inverse can't be found !", null, null);
				goto IL_37B;
			}
			if (Information.Err().Number == 5014)
			{
				Information.Err().Raise(Conversions.ToInteger("5014"), null, "Matrix should be a square matrix !", null, null);
			}
			goto IL_37B;
			num15 = -1;
//			@switch(ICSharpCode.Decompiler.ILAst.ILLabel[], num);
			IL_348:
			goto IL_370;
		}
		object arg_34D_0;
		endfilter(arg_34D_0 is Exception & num > 0 & num15 == 0);
		IL_370:
		throw ProjectData.CreateProjectError(-2146828237);
		IL_37B:
		double[,] arg_389_0 = array3;
		if (num15 != 0)
		{
			ProjectData.ClearProjectError();
		}
		return arg_389_0;
	}
}
        public static Frame GetInverseFrame(Frame f1)
        {
            return Matrix2Frame(Inv(Frame2Matrix(f1)));
        }
        public static Frame Matrix2Frame(double[,] m)
        {
            Frame result;
            result.A = MatrixToEulerA(m);
            result.B = MatrixToEulerB(m);
            result.C = MatrixToEulerC(m);
            result.X = m[0, 3];
            result.Y = m[1, 3];
            result.Z = m[2, 3];
            return result;
        }
        public static double[,] Frame2Matrix(Frame f1)
        {
            double[,] mat = new double[3, 3];
            double[,] mat2 = new double[3, 3];
            double[,] mat3 = new double[3, 3];
            double[,] array = new double[3, 3];
            double[,] array2 = new double[3, 3];
            double[,] array3 = new double[3, 3];
            double[,] array4 = new double[3, 3];
            double[,] array5 = new double[3, 3];
            double[,] array6 = new double[3, 3];
            double[,] array7 = new double[4, 4];
            double[,] array8 = new double[4, 4];
            double[,] array9 = new double[4, 4];
            mat = EulerAToMatrix(f1.A);
            mat2 = EulerBToMatrix(f1.B);
            mat3 = EulerCToMatrix(f1.C);
            array = Multiply(Multiply(mat, mat2), mat3);
            int num = 0;
            checked
            {
                do
                {
                    int num2 = 0;
                    do
                    {
                        array8[num, num2] = array[num, num2];
                        num2++;
                    }
                    while (num2 <= 2);
                    num++;
                }
                while (num <= 2);
                array8[3, 3] = 1.0;
                array8[0, 3] = f1.X;
                array8[1, 3] = f1.Y;
                array8[2, 3] = f1.Z;
                return array8;
            }
        }

    }

    public class ShiftException:Exception
    {
        private int _number;
        public ShiftException (int number,object idk,string message,object idk2,object idk3)
        {
        }
    }

}
