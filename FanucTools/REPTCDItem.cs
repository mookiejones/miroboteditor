using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MookiesEditor.Core;

namespace FanucTools
{
    public class REPTCDItem:ViewModelBase ,IFanucDiagnosticItem
    {
        private string number = String.Empty;
        public string Number{get { return number; }set { number = value;RaisePropertyChanged("Number"); }}
        private string taskname = String.Empty;
        public string Task_Name{get { return taskname; }set { taskname = value;RaisePropertyChanged("TaskName"); }}

        public string Task_Priority{get { return task_pri; }set { task_pri = value;RaisePropertyChanged("Task_Priority"); }}
        private string task_pri = String.Empty;
        public string Task_Hold_Condition{get { return tsk_hld_cnd; }set { tsk_hld_cnd = value;RaisePropertyChanged("Task_Hold_Condition"); }}
        private string tsk_hld_cnd = String.Empty;
        public string Hold_For{get { return hold_for; }set { tsk_hld_cnd = value;RaisePropertyChanged("Hold_For"); }}
        private string hold_for = String.Empty;

        public string Pre_Executing{get { return pre_executing; }set { pre_executing = value;RaisePropertyChanged("Pre_Executing"); }}
        private string pre_executing = String.Empty;

        public string Pre_Open_Id{get { return pre_open_id; }set { pre_open_id = value;RaisePropertyChanged("Pre_Open_Id"); }}
        private string pre_open_id = String.Empty;
        
        public string To_Rel_Mctl{get { return to_rel_mctl; }set { to_rel_mctl = value;RaisePropertyChanged("To_Rel_Mctl"); }}
        private string to_rel_mctl = String.Empty;

        public string Num_Out_Exe_Mmr{get { return num_out_exe_mmr; }set { num_out_exe_mmr = value;RaisePropertyChanged("Num_Out_Exe_Mmr"); }}
        private string num_out_exe_mmr = String.Empty;

        public string Num_Out_Nowait_Amr{get { return num_out_nowait_amr; }set { num_out_nowait_amr = value;RaisePropertyChanged("Num_Out_Nowait_Amr"); }}
        private string num_out_nowait_amr = String.Empty;

        public string Rout_Name{get { return rout_name; }set { rout_name = value;RaisePropertyChanged("Rout_Name"); }}
        private string rout_name = String.Empty;

        public string Cur_Line{get { return cur_line; }set { cur_line = value;RaisePropertyChanged("Cur_Line"); }}
        private string cur_line = String.Empty;

        public bool Invisible{get { return invisible; }set { invisible = value;RaisePropertyChanged("Invisible"); }}
        private bool invisible = false;
        public bool System_Task{get { return system_task; }set { system_task = value;RaisePropertyChanged("System_Task"); }}
        private bool system_task=false;
        public string Open_Id{get { return open_id; }set { open_id = value;RaisePropertyChanged("Open_Id"); }}
        private string open_id = String.Empty;

                public string Ept_Index{get { return ept_index; }set { ept_index = value;RaisePropertyChanged("Ept_Index"); }}

        private string ept_index = string.Empty;
        public string Task_st { get { return task_st; } set { task_st = value; RaisePropertyChanged("Task_st"); } }
        private String task_st=string.Empty;

        public string Strt_Hold_Cond { get { return strt_hld_cnd; } set { strt_hld_cnd = value; RaisePropertyChanged("Strt_Hold_Cond"); } }
        private string strt_hld_cnd = String.Empty;

        public string Cur_Ln_Stat { get { return curln_stat; } set { curln_stat = value; RaisePropertyChanged("Cur_Ln_Stat"); } }
        private string curln_stat = String.Empty;
        public string Num_Out_MMR { get { return num_out_mmr; } set { num_out_mmr = value; RaisePropertyChanged("Num_Out_MMR"); } }
        private string num_out_mmr = String.Empty;
        public string Num_Out_AMR { get { return num_out_amr; } set { num_out_amr = value; RaisePropertyChanged("Num_Out_AMR"); } }
        private string num_out_amr = String.Empty;

        public string Start_Pre { get { return start_pre; } set { start_pre = value; RaisePropertyChanged("Start_Pre"); } }
        private string start_pre = String.Empty;
        public string Done_Pre { get { return done_pre; } set { done_pre = value; RaisePropertyChanged("Done_Pre"); } }
        private string done_pre = String.Empty;

        public string Pre_Ept_Index { get { return pre_ept_index; } set { pre_ept_index = value; RaisePropertyChanged("Pre_Ept_Index"); } }
        private string pre_ept_index = String.Empty;

        public string Pre_Line { get { return pre_line; } set { pre_line = value; RaisePropertyChanged("Pre_Line"); } }
        private string pre_line = String.Empty;

        public string To_Unlock { get { return to_unlock; } set { to_unlock = value; RaisePropertyChanged("To_Unlock"); } }
        private string to_unlock = String.Empty;

        public string Hold_Group_Mask { get { return hld_grp_msk; } set { hld_grp_msk = value; RaisePropertyChanged("Hold_Group_Mask"); } }
        private string hld_grp_msk = String.Empty;

        public string Master_Rob_Num { get { return master_rob_num; } set { master_rob_num = value; RaisePropertyChanged("Master_Rob_Num"); } }
        private string master_rob_num = String.Empty;

        public string My_Own_Rob_Num { get { return my_own_rob_num; } set { my_own_rob_num = value;RaisePropertyChanged("My_Own_Rob_Num"); } }
        private string my_own_rob_num = String.Empty;

    }
}
