/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 3/28/2013
 * Time: 3:12 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
namespace FanucTools
{
	/// <summary>
	/// Description of FanucDiagnostics.
	/// </summary>
	public class FanucDiagnostics:ViewModelBase
	{
		
		        private const string REPTCD = "REPTCD";
		        private ObservableCollection<IFanucDiagnosticItem> _items = new ObservableCollection<IFanucDiagnosticItem>();
		        public ObservableCollection<IFanucDiagnosticItem> Items {get{return _items;}set{_items=value;RaisePropertyChanged("Items");}}
		        
		         static string GetFile(string filename)
        {
            string result = String.Empty;
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = String.Format("Select {0}.dg File", filename);
                ofd.Filter = String.Format("{0}({0}.dg)|{0}.dg", filename);
                ofd.Multiselect = false;

                if (ofd.ShowDialog() == DialogResult.OK)
                    if (!File.Exists(ofd.FileName))
                        return string.Empty;


                using (var sr = new StreamReader(ofd.FileName))
                {
                    result = sr.ReadToEnd();
                }
            }

            return result;
		         }
		         

       
		         

        public static ObservableCollection<REPTCDItem> GetREPTCD()
        {
            var file = GetFile(REPTCD);
            if (String.IsNullOrEmpty((file)))
                return new ObservableCollection<REPTCDItem>();


            
             var s=System.Text.RegularExpressions.Regex.Split(file, "PX");

            Regex regex = new Regex(@"([a-zA-Z\s0-9_]*):\s*([~a-zA-Z0-9_-]*)");

            var items = new ObservableCollection<REPTCDItem>(); 


            for (int i = 0; i < s.Length; i++)
            {
                if (String.IsNullOrEmpty(s[i])) continue;
                var item = new REPTCDItem();

                item.Number =  i.ToString("PX00");
                string exp = s[i].Substring(s[i].IndexOf(":")+1);
                var m = regex.Match(exp);

                while (m.Success)
                {
                    string prop = m.Groups[1].Value.Substring(m.Groups[1].Value.IndexOf("\n")+1).Trim();
                    var val = m.Groups[2].Value;

                    switch (prop)
                    {
                        case "task_name":
                            item.Task_Name = val;
                            break;
                        case "rout_name":
                            item.Rout_Name = val;
                            break;
                        case "cur_line":
                            item.Cur_Line = val;
                            break;
                        case "task_pri":
                            item.Task_Priority = val;
                            break;
                        case "invisible":
                            item.Invisible = val.ToUpper()=="YES";
                            break;
                        case "system task":
                            item.System_Task = val.ToUpper() == "YES";
                            break;
                        case "open_id":
                            item.Open_Id = val;
                            break;
                        case "ept_index":
                            item.Ept_Index = val;
                            break;
                        case "task_st":
                            item.Task_st = val;
                            break;
                        case "tsk_hld_cnd":
                            item.Task_Hold_Condition = val;
                            break;
                        case "strt_hld_cnd":
                            item.Strt_Hold_Cond = val;
                            break;
                        case "curln_stat":
                            item.Cur_Ln_Stat = val;
                            break;
                        case "pre_executing":
                            item.Pre_Executing = val;
                            break;
                        case "start_pre":
                            item.Start_Pre = val;
                            break;
                        case "done_pre":
                            item.Done_Pre = val;
                            break;
                        case "pre_open_id":
                            item.Pre_Open_Id = val;
                            break;
                        case "pre_ept_index":
                            item.Pre_Ept_Index = val;
                            break;
                        case "pre_line":
                            item.Pre_Line = val;
                            break;
                        case "to_rel_mctl":
                            item.To_Rel_Mctl = val;
                            break;
                        case "to_unlock":
                            item.To_Unlock = val;
                            break;
                        case "hld_grp_msk":
                            item.Hold_Group_Mask = val;
                            break;
                        case "num_out_exe_mmr":
                            item.Num_Out_Exe_Mmr = val;
                            break;
                        case "num_out_mmr":
                            item.Num_Out_MMR = val;
                            break;
                        case "num_out_amr":
                            item.Num_Out_AMR = val;
                            break;
                        case "num_out_nowait_amr":
                            item.Num_Out_Nowait_Amr = val;
                            break;
                        case "master_rob_num":
                            item.Master_Rob_Num = val;
                            break;
                        case "my_own_rob_num":
                            item.My_Own_Rob_Num = val;
                            break;
                        default:
                            Console.WriteLine(val);
                            break;
                    }


                    m = m.NextMatch();
                }
                items.Add(item);
            }

            return items;

        }

		        
	}
}
