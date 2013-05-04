using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Text.RegularExpressions;
namespace CSVE6POS
{
    public class E6Reader:ViewModelBase
    {

        public bool ShouldRound = true;

        private string Filename { get; set; }
        string text;
        public  Regex XYZRegex { get { return new Regex(@"^[DECL ]*[GLOBAL ]*(POS|E6POS|E6AXIS|FRAME) ([\w\d_\$]+)=?\{?([^}}]*)?\}?", Ro); } }
        private const RegexOptions Ro = (int)RegexOptions.IgnoreCase + RegexOptions.Multiline;

        private ObservableCollection<E6Position> _positions = new ObservableCollection<E6Position>();
        public ObservableCollection<E6Position> Positions { get { return _positions; } set { _positions = value; RaisePropertyChanged("Positions"); } }

        public void GetPositions()
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "KUKA dat file (*.dat)|*.dat|All Files (*.*)|*.*";
            ofd.Title = "Select Dat file to load";

            if (!ofd.ShowDialog().GetValueOrDefault())
                return;
            
            Filename = ofd.FileName;

            text = File.ReadAllText(Filename);
                    GetMatches(text);

        }
        string AddFDat(string name)
        {
            var p = name.ToLower();
            var l = File.ReadAllLines(Filename);           
                foreach (var ll in l)
                {
                    var m = ll.ToLower();
                    if (m.Contains("ldat")|m.Contains("fdat"))
                    if ( m.ToLower().Contains("p" + p) | m.ToLower().Contains("l" + p))
                    {
                        var apo = m;

                        var start = m.Substring(m.IndexOf("APO_DIST") + 8).Trim();

//                        "DECL PDAT PDrop_Tray={VEL 100.0,ACC 100.0,APO_DIST 100.0}"
                        Console.WriteLine(m);
                        var idx = 0;
                        foreach (Char c in start)
                        {
                            switch (c.ToString())
                            {
                                case "0":
                                case "1":
                                case "2":
                                case "3":
                                case "4":
                                case "5":
                                case "6":
                                case "7":
                                case "8":
                                case "9":
                                case "-":
                                case ".":
                                    break;
                                default:
                                    continue;
                            }

                            idx++;



                            return start.Substring(0, idx);
                        }

                    }
                }
                return String.Empty;
        }

        private void GetMatches(string text)
        {

            var m = XYZRegex.Match(text);
            while (m.Success)
            {
                var _e6 = new E6Position();

                _e6.Name = m.Groups[2].ToString();
                _e6.APO=AddFDat(_e6.Name);
                var t = m.Groups[3].ToString();

                

                t = t.Substring(t.IndexOf("X")+1);
                _e6.X = t.Substring(0, t.IndexOf(","));
                t = t.Substring(t.IndexOf("Y") + 1);
                _e6.Y = t.Substring(0, t.IndexOf(","));
                t = t.Substring(t.IndexOf("Z") + 1);
                _e6.Z = t.Substring(0, t.IndexOf(","));
                t = t.Substring(t.IndexOf("A") + 1);
                _e6.A = t.Substring(0, t.IndexOf(","));
                t = t.Substring(t.IndexOf("B") + 1);
                _e6.B = t.Substring(0, t.IndexOf(","));
                t = t.Substring(t.IndexOf("C") + 1);
                _e6.C = t.Substring(0, t.IndexOf(","));


                if (ShouldRound)
                {
                    _e6.X = Round(_e6.X);
                    _e6.Y = Round(_e6.Y);
                    _e6.Z = Round(_e6.Z);
                    _e6.A = Round(_e6.A);
                    _e6.B = Round(_e6.B);
                    _e6.C = Round(_e6.C);
                }
                Positions.Add(_e6);
                m = m.NextMatch();
            }
            
            

        }

        string Round(string value)
        {
            var d = Convert.ToDouble(value);
            var r = Math.Round(d);

           return Convert.ToString(r);
        }
       

    }
}
