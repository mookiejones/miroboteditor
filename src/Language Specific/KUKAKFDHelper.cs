/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 4/17/2013
 * Time: 12:29 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using miRobotEditor.Core;
using MessageViewModel = miRobotEditor.Core.MessageViewModel;

namespace miRobotEditor.Language_Specific
{
	/// <summary>
	/// Description of KUKAKFDHelper.
	/// </summary>
	public class KUKAKFDHelper:ViewModelBase
	{
#pragma warning disable 169
		List<String> _ilf = new List<String>();
		private string _tqTriggerId = string.Empty;

#pragma warning disable 649
		String[,] _sParam;
#pragma warning restore 649
#pragma warning disable 649
		String[,] _sVar;
#pragma warning restore 649
		
// ReSharper disable UnusedMember.Local
		int ParamCount()
		{
			var num2 = 1;
			do
			{
				if (String.IsNullOrEmpty(_sParam[num2,0]))
				    return num2;
				    
				    num2++;
			}
			while (num2 <=255);
			return 0;
		}
		
		string ParamGetByIndex(int sIndex, int iField = 0)
		{
			return _sParam[sIndex,iField];
		}
		
		private string ParamGet(string sPar, int iField=5)
		{
			var num = 1;
			do
			{
				if (_sParam[num,0] == sPar.ToLower())
					return _sParam[num,iField];
				num++;
			}
			while (num<=255);
			return String.Empty;
		}
		
		public bool IsNumber (ref string value)
		{
			const bool flag = false;
			var str = value;
			const string str2 = "0123456789";
			
			try
			{
				if (str.Length == 0)
					return false;
				
				if (str.Substring(0,1) == "-")
					str = str.Substring(2);
				for (var i = str.Length -1;i >=0;i+= -1)
					if (str2.IndexOf(str.Substring(i)) == -1)
					    return false;
			}
			catch (Exception ex)
			{
                MessageViewModel.AddError("KUKAKFDHELPER",ex);

			}
			return flag;
		}

		private int VarCount
		{
			get
			{
				for (var i = _sVar.GetUpperBound(0);i>=0;i+= -1)
					if (!String.IsNullOrEmpty(_sVar[i,0]))
						return i;
				
				return -1;
			}
		}

		

 

		
		List<Label> _labelCollection = new List<Label>();
		Panel _ilfPanel;
		Label _ilfCommand = new Label();

	    static string StartupPath
		{
			get{
				var s=				System.Reflection.Assembly.GetExecutingAssembly().Location;
				return s;
			}
		}
		
		private void LoadBasisFiles()
		{
			try
			{
				if (!Directory.Exists(StartupPath + @"\TPBasis"))
					MessageViewModel.Add("KFD","TPBasis path ",null,false);
				else
				{
				    foreach (var str in from str in Directory.GetFiles(StartupPath + @"\TPBasis\") let extension = Path.GetExtension(str) where extension != null && extension.ToLower() == ".kfd" select str)
				    {
				        LoadFile(str, "TPBasis");
				    }
				    MessageViewModel.Add("KFD", "TPBasis initialization complete", null, false);
				}
			}
			catch (Exception ex)
			{
                MessageViewModel.AddError("KUKAKFDHelper LoadBasisFiles",ex);
			}
		}
		

 private bool CheckPar(string sLine, string sParam)
{
    var flag = false;
    try
    {
    	
    	var start = sLine.IndexOf(sParam + " ");
        if (start == 0)
            return false;

        start += sParam.Length;
        var num2 = sLine.IndexOf("}", start, StringComparison.Ordinal);
        var num3 = sLine.IndexOf(":", start, StringComparison.Ordinal);
        var num4 = sLine.IndexOf(",", start, StringComparison.Ordinal);
        if ((((num3 < num2) && (num3 > 0)) || ((num4 < num2) && (num4 > 0))) || (num2 == 0))
// ReSharper disable RedundantAssignment
            num2 = num3;
// ReSharper restore RedundantAssignment

        flag = true;
    }
    catch (Exception ex)
    {
                MessageViewModel.AddError("KFDHelper.CheckPar",ex);
    }
    return flag;
 }
		private void CreateControls()
		{
			/*
		}
		    int index = 0;
		    try
		    {
		        string str;
		        if (ilf[0] == "TPBasis")
		            str = " ";
		        else if (this.ilf[0] == "TPUser")
		            str = ", ";
		        ILFCommand.Parent = ILFPanel;
		        LabelCollection[index] = new Label();
//		        this.cLabel[index] = new Label();
		        var label = this.LabelCollection[index];
		        label.Visibility = System.Windows.Visibility.Visible;
		        label.Parent = ILFCommand.Parent;
		        label.Parent.Controls.Add(this.LabelCollection[index]);
		        label.Font = this.ILFCommand.Font;
		        label.Name = "cLabel" + index.ToString();		        
		        label.Margin = this.ILFCommand.Margin;
		        label.AutoSize = true;
		        label.Width = 0;
		        label = null;
		        index = 1;
		        do
		        {
		            if (this.ParamGetByNo(index, 0) != "")
		            {
		                string sPar = this.ParamGetByNo(index, 0);
		                bool flag = Strings.StrComp(this.ParamGet(sPar, 14), "true", CompareMethod.Text) != 0;
		                if (Strings.StrComp(this.ParamGet(sPar, 2), "false", CompareMethod.Text) != 0)
		                {
		                    if ((!this.bIlfChangeCmd && this.bIlfChange) && (index <= this.sParamDefault.Length))
		                    {
		                        if ((this.sActIlf[0] == "TPUser") && (this.sIlfStyle.ToUpper() == "WYSIWYG"))
		                        {
		                            if ((Strings.InStr(this.sParamDefault[index - 1], this.ParamGet(sPar, 4), CompareMethod.Text) > 0) && (Strings.InStr(this.sParamDefault[index - 1], this.ParamGet(sPar, 6), CompareMethod.Text) > 0))
		                            {
		                                this.sParamDefault[index - 1] = Strings.Replace(this.sParamDefault[index - 1], this.ParamGet(sPar, 4), "", 1, -1, CompareMethod.Text);
		                                this.sParamDefault[index - 1] = Strings.Replace(this.sParamDefault[index - 1], this.ParamGet(sPar, 6), "", 1, -1, CompareMethod.Text);
		                                if (this.ParamGet(sPar, 3) == "LIST")
		                                {
		                                    this.ParamListDisp2Val(sPar, this.sParamDefault[index - 1]);
		                                }
		                                else
		                                {
		                                    this.ParamSet(sPar, this.sParamDefault[index - 1], 5, false);
		                                }
		                            }
		                        }
		                        else if ((this.sActIlf[0] == "TPUser") && (this.sIlfStyle.ToUpper() == "SUB"))
		                        {
		                            if (this.ParamGet(sPar, 3) == "LIST")
		                            {
		                                this.ParamListDisp2Val(sPar, this.sParamDefault[index - 1].Trim());
		                            }
		                            else
		                            {
		                                this.ParamSet(sPar, this.sParamDefault[index - 1].Trim(), 5, false);
		                            }
		                        }
		                        else if (this.sActIlf[0] == "TPBasis")
		                        {
		                            string str4;
		                            if (this.bSoc)
		                            {
		                                if (this.sParamDefault.Length > (index + 3))
		                                {
		                                    str4 = this.sParamDefault[index + 3];
		                                }
		                            }
		                            else if (this.sParamDefault.Length > (index + 2))
		                            {
		                                str4 = this.sParamDefault[index + 2];
		                            }
		                            str4 = Strings.Mid(str4, Strings.InStr(str4, ":", CompareMethod.Binary) + 1);
		                            this.ParamSet(sPar, str4, 5, false);
		                        }
		                    }
		                    this.cLabel[index] = new Label();
		                    Label label2 = this.cLabel[index];
		                    label2.Visible = true;
		                    label2.Font = this.LabelIlfCmd.Font;
		                    label2.Name = "cLabel" + Conversions.ToString(index);
		                    label2.Margin = this.LabelIlfCmd.Margin;
		                    label2.AutoSize = true;
		                    label2 = null;
		                    string str5 = "";
		                    switch (this.ParamGet(sPar, 3))
		                    {
		                        case "FREE":
		                        {
		                            this.cParamText[index] = new TextBox();
		                            TextBox box = this.cParamText[index];
		                            box.Visible = flag;
		                            box.Parent = this.LabelIlfCmd.Parent;
		                            box.Parent.Controls.Add(this.cParamText[index]);
		                            box.CausesValidation = true;
		                            box.Text = this.ParamGet(sPar, 5);
		                            box.Width = this.KfdGetParamWidth(box.Text);
		                            box.Font = this.LabelIlfCmd.Font;
		                            box.Name = sPar;
		                            box.Margin = this.txtIlfTmp.Margin;
		                            box.TabIndex = index;
		                            box.GotFocus += new EventHandler(this.cParamText_GotFocus);
		                            box.TextChanged += new EventHandler(this.cParamText_TextChanged);
		                            box.PreviewKeyDown += new PreviewKeyDownEventHandler(this.cParam_PreviewKeyDown);
		                            box = null;
		                            if (index == 1)
		                            {
		                                this.cParamText[1].Focus();
		                            }
		                            break;
		                        }
		                        case "STATIC":
		                            str5 = this.ParamGet(sPar, 5);
		                            break;
		
		                        case "NAME":
		                        {
		                            this.cParamText[index] = new TextBox();
		                            TextBox box2 = this.cParamText[index];
		                            box2.Visible = flag;
		                            box2.Parent = this.LabelIlfCmd.Parent;
		                            box2.Parent.Controls.Add(this.cParamText[index]);
		                            box2.CausesValidation = true;
		                            box2.Text = this.ParamGet(sPar, 5);
		                            box2.Width = this.KfdGetParamWidth(box2.Text);
		                            box2.Font = this.LabelIlfCmd.Font;
		                            box2.Name = sPar;
		                            box2.Margin = this.txtIlfTmp.Margin;
		                            box2.TabIndex = index;
		                            if ((this.sActIlf[0] == "TPBasis") && (((sPar.ToUpper() == "POINT1") || (sPar.ToUpper() == "POINT2")) || ((sPar.ToUpper() == "PARAMLISTPDAT") || (sPar.ToUpper() == "PARAMLISTLDAT"))))
		                            {
		                                box2.MaxLength = 23;
		                            }
		                            else
		                            {
		                                box2.MaxLength = 24;
		                            }
		                            box2.LostFocus += new EventHandler(this.cParamText_LostFocus);
		                            box2.GotFocus += new EventHandler(this.cParamText_GotFocus);
		                            box2.TextChanged += new EventHandler(this.cParamText_TextChanged);
		                            box2.PreviewKeyDown += new PreviewKeyDownEventHandler(this.cParam_PreviewKeyDown);
		                            box2 = null;
		                            break;
		                        }
		                        case "NUMBER":
		                        {
		                            this.cParamText[index] = new TextBox();
		                            TextBox box3 = this.cParamText[index];
		                            box3.Visible = flag;
		                            box3.Parent = this.LabelIlfCmd.Parent;
		                            box3.Parent.Controls.Add(this.cParamText[index]);
		                            box3.CausesValidation = true;
		                            if (this.ParamGet(sPar, 9) != "")
		                            {
		                                box3.Text = this.ParamGet(sPar, 9);
		                            }
		                            if (this.ParamGet(sPar, 5) != "")
		                            {
		                                box3.Text = this.ParamGet(sPar, 5);
		                            }
		                            this.ParamSet(sPar, box3.Text, 5, false);
		                            box3.Width = this.KfdGetParamWidth(box3.Text);
		                            box3.Font = this.LabelIlfCmd.Font;
		                            box3.Name = sPar;
		                            box3.Margin = this.txtIlfTmp.Margin;
		                            box3.TabIndex = index;
		                            box3.MaxLength = 24;
		                            box3.GotFocus += new EventHandler(this.cParamText_GotFocus);
		                            box3.TextChanged += new EventHandler(this.cParamText_TextChanged);
		                            box3.PreviewKeyDown += new PreviewKeyDownEventHandler(this.cParam_PreviewKeyDown);
		                            box3 = null;
		                            break;
		                        }
		                        case "REAL":
		                        {
		                            this.cParamText[index] = new TextBox();
		                            TextBox box4 = this.cParamText[index];
		                            box4.Visible = flag;
		                            box4.Parent = this.LabelIlfCmd.Parent;
		                            box4.Parent.Controls.Add(this.cParamText[index]);
		                            box4.CausesValidation = true;
		                            if (this.ParamGet(sPar, 9) != "")
		                            {
		                                box4.Text = this.ParamGet(sPar, 9);
		                            }
		                            if (this.ParamGet(sPar, 5) != "")
		                            {
		                                box4.Text = this.ParamGet(sPar, 5);
		                            }
		                            this.ParamSet(sPar, box4.Text, 5, false);
		                            box4.Width = this.KfdGetParamWidth(box4.Text);
		                            box4.Font = this.LabelIlfCmd.Font;
		                            box4.Name = sPar;
		                            box4.Margin = this.txtIlfTmp.Margin;
		                            box4.TabIndex = index;
		                            box4.MaxLength = 24;
		                            box4.GotFocus += new EventHandler(this.cParamText_GotFocus);
		                            box4.TextChanged += new EventHandler(this.cParamText_TextChanged);
		                            box4.PreviewKeyDown += new PreviewKeyDownEventHandler(this.cParam_PreviewKeyDown);
		                            box4 = null;
		                            break;
		                        }
		                        case "LIST":
		                        {
		                            this.cParamList[index] = new ComboBox();
		                            ComboBox box5 = this.cParamList[index];
		                            box5.Visible = flag;
		                            box5.Parent = this.LabelIlfCmd.Parent;
		                            box5.Parent.Controls.Add(this.cParamList[index]);
		                            box5.CausesValidation = true;
		                            box5.Width = 5;
		                            box5.DropDownStyle = ComboBoxStyle.DropDownList;
		                            box5.Font = this.LabelIlfCmd.Font;
		                            box5.Margin = this.ComboIlfCmd.Margin;
		                            box5.TabIndex = index;
		                            box5.Name = sPar;
		                            box5.GotFocus += new EventHandler(this.cParamList_GotFocus);
		                            box5.PreviewKeyDown += new PreviewKeyDownEventHandler(this.cParamList_PreviewKeyDown);
		                            box5.SelectedIndexChanged += new EventHandler(this.cParamList_TextChanged);
		                            box5 = null;
		                            this.ParamListFill(index);
		                            break;
		                        }
		                    }
		                    this.cLabel[index].Parent = this.LabelIlfCmd.Parent;
		                    this.cLabel[index].Parent.Controls.Add(this.cLabel[index]);
		                    if (flag)
		                    {
		                        int num3;
		                        if (index > 1)
		                        {
		                            num3 = index - 1;
		                            this.cLabel[num3].Text = this.cLabel[num3].Text + str;
		                        }
		                        num3 = index - 1;
		                        this.cLabel[num3].Text = this.cLabel[num3].Text + this.ParamGet(sPar, 4);
		                        this.cLabel[index].Text = this.ParamGet(sPar, 6) + str5;
		                    }
		                }
		            }
		            index++;
		        }
		        while (index <= 255);
		        this.btnIlfOk = new Button();
		        Button btnIlfOk = this.btnIlfOk;
		        btnIlfOk.Visible = true;
		        btnIlfOk.Parent = this.LabelIlfCmd.Parent;
		        btnIlfOk.Parent.Controls.Add(this.btnIlfOk);
		        btnIlfOk.Font = new Font("Microsoft Sans Serif", 8f, FontStyle.Regular);
		        btnIlfOk.Text = "OK";
		        btnIlfOk.Name = "BtnIlfOk";
		        btnIlfOk.Margin = this.btnIlfTmp1.Margin;
		        btnIlfOk.Width = 36;
		        btnIlfOk.Height = 24;
		        btnIlfOk.Click += new EventHandler(this._Lambda__7);
		        btnIlfOk = null;
		        this.btnIlfEsc = new Button();
		        Button btnIlfEsc = this.btnIlfEsc;
		        btnIlfEsc.Visible = true;
		        btnIlfEsc.Parent = this.LabelIlfCmd.Parent;
		        btnIlfEsc.Parent.Controls.Add(this.btnIlfEsc);
		        btnIlfEsc.Font = new Font("Microsoft Sans Serif", 8f, FontStyle.Regular);
		        btnIlfEsc.Text = "ESC";
		        btnIlfEsc.Name = "BtnIlfEsc";
		        btnIlfEsc.Margin = this.btnIlfTmp1.Margin;
		        btnIlfEsc.Width = 36;
		        btnIlfEsc.Height = 24;
		        btnIlfEsc.Click += new EventHandler(this._Lambda__8);
		        btnIlfEsc = null;
		    }
		    catch (Exception ex)
		    {
		    	MessageWindow.Add(ex);
		        this.SendErrorMsg();
		    }
		    */
		}
// ReSharper disable UnusedParameter.Local
private void KfdCreateIlf(string sTpTyp, string sFile, string sTp, string sCmd, string sCmdShow, string sTpName = "", string sTpVersion = "")
// ReSharper restore UnusedParameter.Local
{
	/*
}
    Font font = new Font(this.sFont, 20f, FontStyle.Regular, GraphicsUnit.Pixel, 0);
    try
    {
        string text;
        TreeListViewItem current;
        TreeListViewItem item2;
        this.bEditorNoChange[this.iSelected] = true;
        this.sActIlf[0] = sTpTyp;
        this.sActIlf[1] = sFile;
        this.sActIlf[2] = sTp;
        this.sActIlf[3] = sCmd;
        this.sActIlf[4] = sCmdShow;
        this.sActIlf[6] = sTpName;
        this.sActIlf[7] = sTpVersion;
        this.iSelStart = this.Editor[this.iSelected].GetFirstCharIndexOfCurrentLine();
        this.IlfPanel.Parent = this.Editor[this.iSelected];
        this.IlfPanel.Cursor = Cursors.Default;
        this.IlfPanel.Width = this.Editor[this.iSelected].Width;
        this.IlfPanel.Top = this.Editor[this.iSelected].GetPositionFromCharIndex(this.Editor[this.iSelected].GetFirstCharIndexOfCurrentLine()).Y;
        this.IlfPanel.BringToFront();
        this.IlfPanel.Visible = false;
        string str = "";
        string sFol = "";
        string sList = "";
        string sLine = "";
        this.sIlfOnAccept = "";
        this.sIlfOnOpen = "";
        bool flag = false;
        bool flag2 = false;
        this.ComboIlfCmd.Items.Clear();
        this.ParamResetUsed();
        this.FoldReset();
        this.PListReset();
        foreach (string str6 in File.ReadAllLines(sFile, Encoding.Default))
        {
            str6 = str6.Trim();
            if (Strings.InStr(str6, ";", CompareMethod.Binary) > 0)
            {
            }
            if (Strings.Len(str6) > 0)
            {
                if (Strings.Mid(str6, Strings.Len(str6), 1) == "_")
                {
                    sLine = sLine + str6.Trim(new char[] { '_' });
                    flag = true;
                    continue;
                }
                flag = false;
                sLine = sLine + str6;
                str6 = sLine;
                if ((Strings.Mid(str6, 1, 16).ToLower() == "decl inlineform ") && (sTp == str))
                {
                    if (this.KfdGetPar(str6, "CMDSHOW").Trim() == "")
                    {
                        this.ComboIlfCmd.Items.Add(Strings.Mid(str6, 16, Strings.InStr(str6, "=", CompareMethod.Text) - 16).Trim());
                    }
                    else
                    {
                        this.ComboIlfCmd.Items.Add(this.KfdGetPar(str6, "CMDSHOW").Trim());
                    }
                    flag2 = true;
                }
                if ((Strings.Mid(str6, 1, 11).ToUpper() == "DECL PARAM ") && ((sTp == str) || (str == "")))
                {
                    this.KfdGetParam(sLine);
                }
                else if ((Strings.Mid(str6, 1, 10).ToUpper() == "DECL FOLD ") && ((sTp == str) || (str == "")))
                {
                    sFol = Strings.Mid(str6, 11, Strings.InStr(str6, "[", CompareMethod.Binary) - 11);
                    this.FoldAdd(sFol);
                }
                else
                {
                    string str7;
                    if ((Strings.Mid(str6, 1, Strings.Len(sFol) + 1).ToUpper() == (sFol.ToUpper() + "[")) && (sFol != ""))
                    {
                        str7 = this.FoldGet(sFol, 2) + Strings.Mid(str6, Strings.InStr(str6, "=", CompareMethod.Binary) + 1).Trim().Trim(new char[] { '"' }) + "\r\n";
                        this.FoldSet(sFol, str7, 2);
                    }
                    else if ((Strings.Mid(str6, 1, 11).ToUpper() == "DECL PLIST ") && (sTp == str))
                    {
                        sList = Strings.Mid(str6, 12, Strings.InStr(str6, "[", CompareMethod.Binary) - 12);
                        this.PlistAdd(sList);
                        str7 = Strings.Mid(str6, Strings.InStr(str6, "->", CompareMethod.Binary) + 2).Trim();
                        this.PListSet(sList, str7, 1);
                    }
                    else if ((Strings.Mid(str6, 1, Strings.Len(sList) + 1).ToUpper() == (sList.ToUpper() + "[")) && (sList != ""))
                    {
                        int iField = 2;
                        do
                        {
                            if (this.PListGet(sList, iField) == "")
                            {
                                break;
                            }
                            iField++;
                        }
                        while (iField <= 255);
                        str7 = Strings.Mid(str6, Strings.InStr(str6, "=", CompareMethod.Binary) + 1).Trim();
                        this.PListSet(sList, str7, iField);
                    }
                    else if (Strings.Mid(str6, 1, 6).ToUpper() == "DEFTP ")
                    {
                        int num3 = Strings.InStr(str6, "=", CompareMethod.Binary);
                        this.bSoc = true;
                        if (num3 > 0)
                        {
                            str = Strings.Mid(str6, 6, num3 - 6).Trim();
                            if ((Strings.InStr(str6, "soc", CompareMethod.Text) > 0) && (this.KfdGetPar(str6, "SOC").ToUpper() == "FALSE"))
                            {
                                this.bSoc = false;
                            }
                        }
                        else
                        {
                            str = Strings.Mid(str6, 6).Trim();
                        }
                    }
                    else if (Strings.Mid(str6, 1, 5).ToUpper() == "ENDTP")
                    {
                        str = "";
                        if (flag2)
                        {
                            break;
                        }
                    }
                    else if ((Strings.Mid(Strings.Replace(str6, " ", "", 1, -1, CompareMethod.Binary), 1, 15 + Strings.Len(sCmd)).ToUpper() == ("DECLINLINEFORM" + sCmd.ToUpper() + "=")) && (sTp == str))
                    {
                        int num4 = 1;
                        do
                        {
                            text = this.KfdGetPar(str6, "PARAM[" + Conversions.ToString(num4) + "]");
                            if (Strings.Len(text) == 0)
                            {
                                break;
                            }
                            this.ParamSet(text, Conversions.ToString(num4), 1, false);
                            num4++;
                        }
                        while (num4 <= 255);
                        num4 = 1;
                        do
                        {
                            text = this.KfdGetPar(str6, "FOLD[" + Conversions.ToString(num4) + "]");
                            if (Strings.Len(text) == 0)
                            {
                                break;
                            }
                            this.FoldSet(text, Conversions.ToString(num4), 1);
                            num4++;
                        }
                        while (num4 <= 255);
                        this.sIlfOnAccept = this.KfdGetPar(str6, "ONACCEPT");
                        this.sIlfOnOpen = this.KfdGetPar(str6, "ONOPEN");
                        this.sIlfStyle = this.KfdGetPar(str6, "STYLE");
                        if (this.sIlfStyle == "")
                        {
                            this.sIlfStyle = "WYSIWYG";
                        }
                        this.sIlfSpline = this.KfdGetPar(str6, "SPLINE");
                        this.sIlfType = this.KfdGetPar(str6, "ILF_TYPE");
                    }
                }
                sLine = "";
            }
        }
        string str9 = this.sIlfStyle.ToUpper();
        switch (str9)
        {
            case "":
            case "WYSIWYG":
                sTp = sTp + ".";
                break;

            default:
                if (str9 == "SUB")
                {
                    sTp = sTp;
                }
                break;
        }
        text = sCmd;
        if (sTpTyp == "TPUser")
        {
            IEnumerator enumerator;
            try
            {
                enumerator = this.tlvUser.Items.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    IEnumerator enumerator2;
                    current = (TreeListViewItem) enumerator.Current;
                    try
                    {
                        enumerator2 = current.Items.GetEnumerator();
                        while (enumerator2.MoveNext())
                        {
                            item2 = (TreeListViewItem) enumerator2.Current;
                            if (((item2.SubItems[1].Text == sFile) && ((item2.SubItems[2].Text + ".") == sTp)) && ((item2.SubItems[3].Text == sCmd) && (item2.Text != (sTp + sCmd))))
                            {
                                text = item2.Text;
                                sTp = "";
                            }
                        }
                        continue;
                    }
                    finally
                    {
                        if (enumerator2 is IDisposable)
                        {
                            (enumerator2 as IDisposable).Dispose();
                        }
                    }
                }
            }
            finally
            {
                if (enumerator is IDisposable)
                {
                    (enumerator as IDisposable).Dispose();
                }
            }
        }
        else if (sTpTyp == "TPBasis")
        {
            IEnumerator enumerator3;
            try
            {
                enumerator3 = this.tlvBasis.Items.GetEnumerator();
                while (enumerator3.MoveNext())
                {
                    IEnumerator enumerator4;
                    current = (TreeListViewItem) enumerator3.Current;
                    try
                    {
                        enumerator4 = current.Items.GetEnumerator();
                        while (enumerator4.MoveNext())
                        {
                            item2 = (TreeListViewItem) enumerator4.Current;
                            if (((item2.SubItems[1].Text == sFile) && ((item2.SubItems[2].Text + ".") == sTp)) && ((item2.SubItems[3].Text == sCmd) && (item2.Text != (sTp + sCmd))))
                            {
                                text = item2.Text;
                                sTp = "";
                            }
                        }
                        continue;
                    }
                    finally
                    {
                        if (enumerator4 is IDisposable)
                        {
                            (enumerator4 as IDisposable).Dispose();
                        }
                    }
                }
            }
            finally
            {
                if (enumerator3 is IDisposable)
                {
                    (enumerator3 as IDisposable).Dispose();
                }
            }
        }
        if (this.bSoc)
        {
            this.LabelIlfCmd.Text = sTp;
            this.ComboIlfCmd.Text = sCmdShow;
            this.ComboIlfCmd.Width = this.KfdGetParamWidth(sCmdShow) + 17;
            this.ComboIlfCmd.Visible = true;
        }
        else
        {
            this.LabelIlfCmd.Text = sTp + sCmdShow;
            this.ComboIlfCmd.Visible = false;
        }
        this.KfdCreateControls();
        if (this.sIlfOnOpen != "")
        {
            this.KfdExecuteScript(this.sIlfOnOpen);
        }
        this.bIlf = true;
        this.KfdUpdateControls();
        this.IlfPanel.Visible = true;
        this.EditorSelectLine(0);
        if (this.ParamGetByNo(1, 0) != "")
        {
            string str10 = this.ParamGetByNo(1, 3);
            if (str10 == "LIST")
            {
                this.cParamList[1].Focus();
            }
            else if (str10 != "STATIC")
            {
                this.cParamText[1].Focus();
            }
        }
        this.bEditorNoChange[this.iSelected] = false;
    }
    catch (Exception ex)
    {
    	MessageWindow.Add(ex);
        this.SendErrorMsg();
        this.IlfCancel();
    }
    */
}

// ReSharper disable UnusedParameter.Local
		private void ExecuteScript(string sScript)
// ReSharper restore UnusedParameter.Local
		{
			/*
		}
		    int num;
		    string str;
		    bool flag = false;
		    string[] strArray = new string[8];
		    string[] strArray2 = new string[8];
		    try
		    {
		        string[] strArray5;
		        this.bStopKfd = false;
		        this.bAcceptInlineform = false;
		        this.bCancelInlineform = false;
		        num = 0;
		        str = "";
		        bool flag2 = false;
		        string str2 = "";
		        foreach (string str3 in File.ReadAllLines(this.sActIlf[1], Encoding.Default))
		        {
		            strArray5 = (string[]) Utils.CopyArray((Array) strArray5, new string[num + 1]);
		            strArray5[strArray5.Length - 1] = str3;
		            num++;
		        }
		        num = 0;
		        while (num != strArray5.Length)
		        {
		            str3 = strArray5[num];
		            str3 = str3.Trim();
		            num++;
		            if (this.bStopKfd)
		            {
		                return;
		            }
		            if (flag2)
		            {
		                if (flag)
		                {
		                    if ((str3 != "") && (str3.ToLower() == "endswitch"))
		                    {
		                        flag = false;
		                    }
		                }
		                else
		                {
		                    string str4;
		                    string str5;
		                    string str6;
		                    if (this.DebugKfd)
		                    {
		                        Interaction.MsgBox(str3, MsgBoxStyle.DefaultButton1, null);
		                    }
		                    if (Strings.Mid(str3, 1, 9).ToLower() == "endscript")
		                    {
		                        flag2 = false;
		                        return;
		                    }
		                    if (Strings.Mid(str3, 1, 7).ToLower() == "showvar")
		                    {
		                        if (!this.KfdScriptShowVar(this.KfdGetScriptPar(str3, "FULLPATH[]"), this.KfdGetScriptPar2(str3, "PARAM")))
		                        {
		                            this.SendStatusMsg("<" + str + "> " + language.s("Script aborted in Line %1 <%2>", Conversions.ToString(num), str3, "", ""), "!", 0, 0);
		                        }
		                        continue;
		                    }
		                    if (Strings.Mid(str3, 1, 6).ToLower() == "setvar")
		                    {
		                        if (!this.KfdScriptSetVar(this.KfdGetScriptPar(str3, "FULLPATH[]"), this.KfdGetScriptPar(str3, "VALUE[]")))
		                        {
		                            this.SendStatusMsg("<" + str + "> " + language.s("Script aborted in Line %1 <%2>", Conversions.ToString(num), str3, "", ""), "!", 0, 0);
		                        }
		                        continue;
		                    }
		                    if (Strings.Mid(str3, 1, 6).ToLower() == "redecl")
		                    {
		                        if (!this.KfdScriptReDecl(this.KfdGetScriptPar(str3, "PATH[]"), this.KfdGetScriptPar(str3, "DECLARATION[]")))
		                        {
		                            this.SendStatusMsg("<" + str + "> " + language.s("Script aborted in Line %1 <%2>", Conversions.ToString(num), str3, "", ""), "!", 0, 0);
		                        }
		                        continue;
		                    }
		                    if (Strings.Mid(str3, 1, 14).ToLower() == "switch dialog ")
		                    {
		                        string[] strArray6 = Strings.Split(str3.Replace("/\"", "<!!!>"), "\"", -1, CompareMethod.Binary);
		                        if (str3.Length > 0)
		                        {
		                            str5 = strArray6[1].Replace("<!!!>", "/\"") + "\"";
		                            int num3 = this.ParamCount();
		                            for (int i = 1; i <= num3; i++)
		                            {
		                                str4 = this.ParamGetByIndex(i, 5);
		                                str5 = Strings.Replace(Strings.Replace(Strings.Replace(str5, "%" + this.ParamGetByIndex(i, 0) + " ", str4 + "", 1, -1, CompareMethod.Text), "%" + this.ParamGetByIndex(i, 0) + "/", str4 + "", 1, -1, CompareMethod.Text), "%" + this.ParamGetByIndex(i, 0) + "\"", str4 + "\"", 1, -1, CompareMethod.Text);
		                            }
		                            str5 = str5.TrimEnd(new char[] { '"' });
		                        }
		                        continue;
		                    }
		                    if (Strings.Mid(str3, 1, 7).ToLower() == "switch ")
		                    {
		                        str6 = Strings.Replace(str3, "switch ", "", 1, 1, CompareMethod.Text).Trim().Trim(new char[] { '"' });
		                        if (str6.ToLower() == "ischange")
		                        {
		                            str2 = "false";
		                            if (this.bIlfChange)
		                            {
		                                str2 = "true";
		                            }
		                        }
		                        else if (str6.ToLower() == "doalways")
		                        {
		                            str2 = "false";
		                            if (this.bDoAlways)
		                            {
		                                str2 = "true";
		                            }
		                        }
		                        else if (this.ParamGet(str6, 5) != "")
		                        {
		                            str2 = this.ParamGet(str6, 5);
		                        }
		                        else if (str6.Contains("=="))
		                        {
		                            string str7 = this.ParamGet(str6.Substring(0, str6.IndexOf("=")), 5);
		                            string str8 = this.ParamGet(str6.Substring(str6.IndexOf("=") + 2), 5);
		                            if (str7 == str8)
		                            {
		                                str2 = "true";
		                            }
		                            else
		                            {
		                                str2 = "false";
		                            }
		                        }
		                        continue;
		                    }
		                    if ((Strings.Mid(str3, 1, 5).ToLower() == "case ") || ((Strings.Mid(str3, 1, 4).ToLower() == "case") && (Strings.Len(str3) == 4)))
		                    {
		                        if (str3.Contains(";"))
		                        {
		                            str3 = str3.Substring(0, str3.IndexOf(";"));
		                        }
		                        if (str3.Contains("\""))
		                        {
		                            str6 = this.KfdGetScriptPar(str3, "case");
		                        }
		                        else
		                        {
		                            string[] strArray7 = Strings.Split(str3.Trim(), " ", -1, CompareMethod.Binary);
		                            str6 = "";
		                            if (strArray7.Length > 1)
		                            {
		                                str6 = strArray7[1];
		                            }
		                        }
		                        if ((str6.ToLower() == str2.ToLower()) && (str5 == ""))
		                        {
		                            this.KfdExecuteScript(Strings.Mid(str3, Strings.InStr(str3, "do ", CompareMethod.Text) + 3).Trim());
		                            flag = true;
		                        }
		                        if (str6 == "")
		                        {
		                            str6 = " ";
		                        }
		                        if (str5 != "")
		                        {
		                            int index = 1;
		                            do
		                            {
		                                if (strArray[index] == "")
		                                {
		                                    strArray[index] = str6;
		                                    strArray2[index] = Strings.Mid(str3, Strings.InStr(str3, "do ", CompareMethod.Text) + 3).Trim();
		                                    continue;
		                                }
		                                index++;
		                            }
		                            while (index <= 7);
		                        }
		                    }
		                    else if (Strings.Mid(str3, 1, 4).ToLower() == "set ")
		                    {
		                        str6 = Strings.Replace(str3, "set ", "", 1, 1, CompareMethod.Text).Trim();
		                        string sPar = Strings.Mid(str6, 1, Strings.InStr(str6, "=", CompareMethod.Binary) - 1).Trim();
		                        if (sPar.ToLower() == this.sActIlf[3].ToLower())
		                        {
		                            int num6 = 1;
		                            do
		                            {
		                                this.sFold[num6, 1] = "";
		                                num6++;
		                            }
		                            while (num6 <= 255);
		                            num6 = 1;
		                            do
		                            {
		                                str4 = this.KfdGetPar(str3, "FOLD[" + Conversions.ToString(num6) + "]");
		                                if (Strings.Len(str4) != 0)
		                                {
		                                    this.FoldSet(str4, Conversions.ToString(num6), 1);
		                                    num6++;
		                                }
		                            }
		                            while (num6 <= 255);
		                        }
		                        else if (this.ParamGet(sPar, 1) != "")
		                        {
		                            this.ParamSet(sPar, this.KfdGetPar(str3, "DUMMY"), 14, true);
		                        }
		                    }
		                    else if (Strings.Mid(str3, 1, 8).ToLower() == "message ")
		                    {
		                        if (str3.Contains(";"))
		                        {
		                            str3 = str3.Substring(0, str3.IndexOf(";"));
		                        }
		                        string[] strArray8 = Strings.Split(str3.Replace("/\"", "<!!!>"), "\"", -1, CompareMethod.Binary);
		                        if (str3.Length > 0)
		                        {
		                            string expression = strArray8[1].Replace("<!!!>", "/\"") + "\"";
		                            int num7 = this.ParamCount();
		                            for (int j = 1; j <= num7; j++)
		                            {
		                                str4 = this.ParamGetByIndex(j, 5);
		                                expression = Strings.Replace(Strings.Replace(Strings.Replace(expression, "%" + this.ParamGetByIndex(j, 0) + " ", str4 + "", 1, -1, CompareMethod.Text), "%" + this.ParamGetByIndex(j, 0) + "\"", str4 + "\"", 1, -1, CompareMethod.Text), "%" + this.ParamGetByIndex(j, 0) + "/", str4 + "", 1, -1, CompareMethod.Text);
		                            }
		                            this.SendStatusMsg("<" + str + "> " + expression.TrimEnd(new char[] { '"' }), ".", 0, 0);
		                        }
		                    }
		                    else if (Strings.Mid(str3, 1, 9).ToLower() == "endswitch")
		                    {
		                        if (str5 != "")
		                        {
		                            this.SendStatusMsg("<" + str + "> " + str5, "?", 0, 0);
		                            MyProject.Forms.FormKfdDialog.Text = str;
		                            MyProject.Forms.FormKfdDialog.Label1.Text = str5;
		                            MyProject.Forms.FormKfdDialog.Button1.Text = strArray[1];
		                            MyProject.Forms.FormKfdDialog.Button2.Text = strArray[2];
		                            MyProject.Forms.FormKfdDialog.Button3.Text = strArray[3];
		                            MyProject.Forms.FormKfdDialog.Button4.Text = strArray[4];
		                            MyProject.Forms.FormKfdDialog.Button5.Text = strArray[5];
		                            MyProject.Forms.FormKfdDialog.Button6.Text = strArray[6];
		                            MyProject.Forms.FormKfdDialog.Button7.Text = strArray[7];
		                            MyProject.Forms.FormKfdDialog.ShowDialog();
		                            string str11 = strArray2[MyProject.Forms.FormKfdDialog.iAnswer];
		                            str5 = "";
		                            int num9 = 1;
		                            do
		                            {
		                                strArray[num9] = "";
		                                strArray2[num9] = "";
		                                num9++;
		                            }
		                            while (num9 <= 7);
		                            this.KfdExecuteScript(str11);
		                        }
		                    }
		                    else if (Strings.Mid(str3, 1, 3).ToLower() == "do ")
		                    {
		                        if (str3.Contains(";"))
		                        {
		                            str3 = str3.Substring(0, str3.IndexOf(";"));
		                        }
		                        str4 = Strings.Replace(str3, "do ", "", 1, 1, CompareMethod.Text).Trim();
		                        string str12 = str4.ToLower();
		                        switch (str12)
		                        {
		                            case "acceptinlineform":
		                            {
		                                this.bStopKfd = true;
		                                this.bAcceptInlineform = true;
		                                this.bCancelInlineform = false;
		                                continue;
		                            }
		                            case "cancelinlineform":
		                            {
		                                this.bStopKfd = true;
		                                this.IlfCancel();
		                                this.bCancelInlineform = true;
		                                continue;
		                            }
		                        }
		                        if (str12 != "nothing")
		                        {
		                            if (str12 == "end")
		                            {
		                                flag2 = false;
		                                return;
		                            }
		                            this.KfdExecuteScript(str4);
		                        }
		                    }
		                }
		            }
		            else if (Strings.Mid(str3, 1, 6).ToUpper() == "DEFTP ")
		            {
		                string str13 = Conversions.ToString(Strings.InStr(str3, "=", CompareMethod.Binary));
		                if (Conversions.ToDouble(str13) > 0.0)
		                {
		                    str = Strings.Mid(str3, 6, (int) Math.Round((double) (Conversions.ToDouble(str13) - 6.0))).Trim();
		                }
		                else
		                {
		                    str = Strings.Mid(str3, 6).Trim();
		                }
		            }
		            else if (Strings.Mid(str3, 1, 5).ToUpper() == "ENDTP")
		            {
		                str = "";
		            }
		            else if ((Strings.Mid(str3, 1).ToLower() == ("defscript " + sScript.ToLower())) && ((this.sActIlf[2] == str) || (str == "")))
		            {
		                flag2 = true;
		            }
		        }
		    }
		    catch (Exception exception1)
		    {
		        ProjectData.SetProjectError(exception1);
		        Exception exception = exception1;
		        this.SendStatusMsg("<" + str + "> " + language.s("Script aborted in Line %1 <%2>", Conversions.ToString(num), str3, "", ""), "!", 0, 0);
		        ProjectData.ClearProjectError();
		    }
		    */
		}

 
	private string GetPar(string sLine, string sParam)
	{
	    var str = "";

	    try
	    {
	        var flag = false;
	        var start = sLine.IndexOf(sParam + " ");
	        if (start == 0)
	            return str;

	        start+=sParam.Length;
	        sLine = sLine.Substring(start).Trim();
	        
	        var num2 = sLine.IndexOf(",");
	        var num3 = sLine.IndexOf("}");
	        var num4 = sLine.IndexOf("{");
	        
	        start = 1;
	        if (((num4 > 0) && (num4 < num3)) && ((num4 < num2) || (num2 == 0)))
	        {
	            start = num4;
	            flag = true;
	        }
	        if ((flag || ((num3 < num2) && (num3 > 0))) || (num2 == 0))
	            num2 = num3;
	
	        str = sLine.Substring(start,num2-start).Trim().Trim(new[]{'"'}).Replace("/;",";");
	    }
	    catch (Exception ex)
	    {
	    	MessageViewModel.AddError("KFDHelper.GetPar",ex);
	    }
	    return str;
	}

// ReSharper disable UnusedParameter.Local
 	private string GetParam(string sLine)
// ReSharper restore UnusedParameter.Local
 	{
 	    const string str = "";
 	    /*
 	}
    
    try
    {
        int start = Strings.InStr(12, sLine, "=", CompareMethod.Binary);
        string sPar = Strings.Replace(Strings.Replace(Strings.Mid(sLine, 1, start - 1), "decl", "", 1, 1, CompareMethod.Text), "param", "", 1, 1, CompareMethod.Text).Trim();
        start = Strings.InStr(Strings.InStr(sLine, "VALUE ", CompareMethod.Text), sLine, "{", CompareMethod.Binary) + 1;
        int num2 = Strings.InStr(start, sLine, ":", CompareMethod.Binary);
        this.ParamAdd(sPar);
        this.ParamSet(sPar, "0", 1, false);
        string str3 = Strings.Mid(sLine, start, num2 - start).Trim(new char[] { ' ' }).ToUpper();
        switch (str3)
        {
            case "FREE":
                this.ParamSet(sPar, "FREE", 3, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "VISIBLE"), 2, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "SHORTNAME[]"), 4, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "UNIT[]"), 6, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "DEFAULT[]"), 7, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "SHORTCUT[]"), 8, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "MIN"), 9, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "MAX"), 10, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "AUTOLIMIT"), 11, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "STEP"), 12, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "ENABLE"), 13, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "DUMMY"), 14, false);
                if (this.ParamGet(sPar, 5) == "")
                {
                    this.ParamSet(sPar, this.ParamGet(sPar, 7), 5, false);
                }
                return str;

            case "STATIC":
                this.ParamSet(sPar, "STATIC", 3, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "VISIBLE"), 2, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "SHORTNAME[]"), 4, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "UNIT[]"), 6, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "DEFAULT[]"), 7, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "SHORTCUT[]"), 8, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "MIN"), 9, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "MAX"), 10, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "AUTOLIMIT"), 11, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "STEP"), 12, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "ENABLE"), 13, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "DUMMY"), 14, false);
                if (this.ParamGet(sPar, 5) == "")
                {
                    this.ParamSet(sPar, this.ParamGet(sPar, 7), 5, false);
                }
                return str;

            case "NAME":
                this.ParamSet(sPar, "NAME", 3, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "VISIBLE"), 2, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "SHORTNAME[]"), 4, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "UNIT[]"), 6, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "DEFAULT[]"), 7, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "SHORTCUT[]"), 8, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "MIN"), 9, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "MAX"), 10, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "AUTOLIMIT"), 11, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "STEP"), 12, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "ENABLE"), 13, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "DUMMY"), 14, false);
                if (this.ParamGet(sPar, 5) == "")
                {
                    this.ParamSet(sPar, this.ParamGet(sPar, 7), 5, false);
                }
                return str;

            case "NUMBER":
                this.ParamSet(sPar, "NUMBER", 3, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "VISIBLE"), 2, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "SHORTNAME[]"), 4, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "UNIT[]"), 6, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "DEFAULT"), 7, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "SHORTCUT[]"), 8, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "MIN"), 9, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "MAX"), 10, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "AUTOLIMIT"), 11, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "STEP"), 12, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "ENABLE"), 13, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "DUMMY"), 14, false);
                if (this.ParamGet(sPar, 5) == "")
                {
                    this.ParamSet(sPar, this.ParamGet(sPar, 7), 5, false);
                }
                return str;

            case "REAL":
                this.ParamSet(sPar, "REAL", 3, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "VISIBLE"), 2, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "SHORTNAME[]"), 4, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "UNIT[]"), 6, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "DEFAULT"), 7, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "SHORTCUT[]"), 8, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "MIN"), 9, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "MAX"), 10, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "AUTOLIMIT"), 11, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "STEP"), 12, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "ENABLE"), 13, false);
                this.ParamSet(sPar, this.KfdGetPar(sLine, "DUMMY"), 14, false);
                if (this.ParamGet(sPar, 5) == "")
                {
                    this.ParamSet(sPar, this.ParamGet(sPar, 7), 5, false);
                }
                return str;
        }
        if (str3 != "LIST")
        {
            return str;
        }
        this.ParamSet(sPar, "LIST", 3, false);
        this.ParamSet(sPar, this.KfdGetPar(sLine, "VISIBLE"), 2, false);
        this.ParamSet(sPar, this.KfdGetPar(sLine, "SHORTNAME[]"), 4, false);
        this.ParamSet(sPar, this.KfdGetPar(sLine, "UNIT[]"), 6, false);
        this.ParamSet(sPar, this.KfdGetPar(sLine, "SHORTCUT[]"), 8, false);
        this.ParamSet(sPar, this.KfdGetPar(sLine, "ENABLE"), 13, false);
        this.ParamSet(sPar, this.KfdGetPar(sLine, "DUMMY"), 14, false);
        this.ParamSet(sPar, "0", 15, false);
        this.ParamSet(sPar, this.KfdGetPar(sLine, "ONCHANGE"), 7, false);
        int num3 = 1;
        do
        {
            string expression = this.KfdGetPar(sLine, "ITEM[" + Conversions.ToString(num3) + "]");
            if (Strings.Len(expression) <= 0)
            {
                return str;
            }
            expression = expression + "\"}";
            if ((num3 == 1) && (this.ParamGet(sPar, 5) == ""))
            {
                this.ParamSet(sPar, this.KfdGetPar(expression, "VALUE[]"), 5, false);
            }
            this.ParamSet(sPar, this.KfdGetPar(expression, "VALUE[]"), 14 + (num3 * 2), false);
            if (this.KfdCheckPar(expression, "DISP[]"))
            {
                this.ParamSet(sPar, this.KfdGetPar(expression, "DISP[]"), 15 + (num3 * 2), false);
            }
            else
            {
                this.ParamSet(sPar, this.KfdGetPar(expression, "VALUE[]"), 15 + (num3 * 2), false);
            }
            this.ParamSet(sPar, Conversions.ToString(num3), 15, false);
            num3++;
        }
        while (num3 <= 240);
    }
    catch (Exception exception1)
    {
        ProjectData.SetProjectError(exception1);
        Exception exception = exception1;
        this.SendErrorMsg();
        ProjectData.ClearProjectError();
    }
    */
    return str;
 	}


// ReSharper disable UnusedParameter.Local
	    private int GetParamWidth(string sText)
// ReSharper restore UnusedParameter.Local
	    {
	        const int num = -1;
	        /*
	    try
	    {
	        Graphics graphics = this.CreateGraphics();
	        num = (int) Math.Round((double) (graphics.MeasureString(sText, this.LabelIlfCmd.Font).Width + 3f));
	        graphics.Dispose();
	        if (num < 12)
	            num = 12;
	    }
	    catch (Exception ex)
	    {
	    	MessageWindow.Add(ex);
	        this.SendErrorMsg();
	    }
	    */
	    return num;
	    }


// ReSharper disable UnusedParameter.Local
	    private string GetScriptPar(string sLine, string sPar)
// ReSharper restore UnusedParameter.Local
	    {
	        const string str = "";
	        /*
	    try
	    {
	        int start = (Strings.InStr(sLine.Replace("(", " ").Replace(",", " "), " " + sPar + " ", CompareMethod.Text) + Strings.Len(sPar)) + 1;
	        if (start == 0)
	            return str;

	        start = Strings.InStr(start, sLine, "\"", CompareMethod.Binary) + 1;
	        int num2 = Strings.InStr(start, sLine.Replace("/\"", "/@"), "\"", CompareMethod.Binary);
	        if ((num2 - start) > 0)
	            str = Strings.Mid(sLine, start, num2 - start);
	    }
	    catch (Exception ex)
	    {
	    	MessageWindow.Add(ex);
	        this.SendErrorMsg();
	    }
	    */
	   	    return str;
	    }

// ReSharper disable UnusedParameter.Local
	    private string GetScriptPar2(string sLine, string sPar)
// ReSharper restore UnusedParameter.Local
	    {
	        const string str = "";
	        /*
	    try
	    {
	        int num4;
	        int start = (Strings.InStr(sLine.Replace("(", " ").Replace(",", " "), " " + sPar + " ", CompareMethod.Text) + Strings.Len(sPar)) + 2;
	        if (start == 0)
	            return str;
	        
	        sLine = Strings.Mid(sLine, start).Trim();
	        int num2 = Strings.InStr(sLine, ")", CompareMethod.Binary);
	        int num3 = Strings.InStr(sLine, ",", CompareMethod.Binary);
	        if ((num2 > 0) && ((num2 < num3) || (num3 == 0)))
	            num4 = num2;

	        if ((num3 > 0) && ((num3 < num2) || (num2 == 0)))
	            num4 = num3;
	        str = Strings.Mid(sLine, 1, num4 - 1).TrimEnd(new char[0]);
	        str = str.Trim(new char[] { '"' });
	    }
	    catch (Exception ex)
	    {
	    	MessageWindow.Add(ex);
	        this.SendErrorMsg();
	    }
	    */
	    return str;
	    }


// ReSharper disable UnusedParameter.Local
	    private string GetStrucVal(string sLine, string sVar)
// ReSharper restore UnusedParameter.Local
	    {
	        const string str = "";
	        /*
		   
		    try
		    {
		        int num2;
		        int start = (Strings.InStr(sLine.Replace("{", " ").Replace(",", " "), " " + sVar + " ", CompareMethod.Text) + Strings.Len(sVar)) + 2;
		        if (start == 0)
		            return str;

		        sLine = Strings.Mid(sLine, start).Trim();
		        if (Strings.Mid(sLine, 1, 1) == "{")
		        {
		            num2 = Strings.InStr(sLine, "}", CompareMethod.Binary);
		            return Strings.Mid(sLine, 1, num2);
		        }
		        int num3 = Strings.InStr(sLine, "}", CompareMethod.Binary);
		        int num4 = Strings.InStr(sLine, ",", CompareMethod.Binary);
		        if ((num3 > 0) && ((num3 < num4) || (num4 == 0)))
		            num2 = num3;

		        if ((num4 > 0) && ((num4 < num3) || (num3 == 0)))
		            num2 = num4;
		        str = Strings.Mid(sLine, 1, num2 - 1).TrimEnd(new char[0]);
		    }
		    catch (Exception ex)
		    {
		    	MessageWindow.Add(ex);
		        this.SendErrorMsg();
		    }
		    */
		    return str;
	    }


// ReSharper disable UnusedParameter.Local
	    private string GetVarFromDat(string sVarIn)
// ReSharper restore UnusedParameter.Local
		{
#pragma warning disable 168
		    var strArray = new string[1];
#pragma warning restore 168
		    const string str = "";
		    /*
		    try
		    {
		        int num3;
		        string str2;
		        int num6;
		        int editorDat = this.GetEditorDat(false);
		        if (editorDat < 0) return str;
		        
		        if (Strings.InStr(sVarIn, ".", CompareMethod.Binary) > 0)
		            strArray = Strings.Split(sVarIn, ".", -1, CompareMethod.Binary);
		        else
		            strArray[0] = sVarIn;

		        string[] strArray2 = this.Editor[editorDat].Text.Split(new char[] { '\n' });
		        int num2 = strArray2.Length - 1;
		        for (num3 = 0; num3 <= num2; num3++)
		        {
		            int length = Strings.InStr(strArray2[num3], ";", CompareMethod.Binary);
		            if (length == 0)
		                length = Strings.Len(strArray2[num3]);

		            str2 = Strings.Mid(strArray2[num3], 1, length);
		            if ((Strings.InStr(str2, " " + strArray[0] + "=", CompareMethod.Text) > 0) || (Strings.InStr(str2, " " + strArray[0] + " ", CompareMethod.Text) > 0))
		            {
		                if (strArray.Length == 1)
		                    return Strings.Mid(str2, Strings.InStr(str2, "=", CompareMethod.Binary) + 1).Trim();

		                str2 = Strings.Mid(str2, Strings.InStr(str2, "=", CompareMethod.Binary) + 1).Trim();
		                int num5 = strArray.Length - 1;
		                num6 = 1;
		                while (num6 <= num5)
		                {
		                    str2 = this.KfdGetStrucVal(str2, strArray[num6]);
		                    num6++;
		                }
		                return str2;
		            }
		        }
		        if (strArray[0].Contains("["))
		            strArray[0] = strArray[0].Substring(0, strArray[0].IndexOf("["));
		        
		        int num7 = this.VarCount();
		        for (num3 = 0; num3 <= num7; num3++)
		        {
		            if (this.sVar[num3, 4].ToUpper() == "TRUE")
		            {
		                string str3;
		                if (this.sVar[num3, 0].Contains("["))
		                    str3 = this.sVar[num3, 0].Substring(0, this.sVar[num3, 0].IndexOf("["));
		                else
		                    str3 = this.sVar[num3, 0];
		                if (str3.ToUpper() == strArray[0].ToUpper())
		                {
		                    if (strArray.Length == 1)
		                        return this.sVar[num3, 2];
	
		                    str2 = this.sVar[num3, 2];
		                    int num8 = strArray.Length - 1;
		                    for (num6 = 1; num6 <= num8; num6++)
		                        str2 = this.KfdGetStrucVal(str2, strArray[num6]);
		                    return str2;
		                }
		            }
		        }
		    }
		    catch (Exception ex)
		    {
		    	MessageWindow.Add(ex);
		        this.SendErrorMsg();
		    }
		    */
		    return str;
		}

 
// ReSharper disable UnusedParameter.Local
private static void LoadFile(string sFile, string sTyp)
// ReSharper restore UnusedParameter.Local
		{
			/*
		}
		    try
		    {
		        string[] strArray = File.ReadAllLines(sFile, Encoding.Default);
		        if (this.bKfdFirstColor)
		            this.bKfdFirstColor = false;
		        else
		            this.bKfdFirstColor = true;

		        string text = "";
		        string str2 = "";
		        TreeListViewItem item = new TreeListViewItem("", 0) {
		            Items = { SortOrder = SortOrder.None }
		        };
		        item.SubItems.Add(sFile);
		        string krlComment = Toolbox.GetKrlComment(sFile);
		        if (krlComment != "")
		            item.Text = language.s(krlComment, "", "", "", "");
		        else
		            item.Text = Path.GetFileNameWithoutExtension(sFile);

		        foreach (string str4 in strArray)
		        {
		            string str5;
		            str4 = str4.Trim();
		            if (Strings.InStr(str4, "_", CompareMethod.Binary) == Strings.Len(str4))
		            {
		                str5 = str5 + str4.Trim(new char[] { '_' });
		                continue;
		            }
		            str4 = str5 + str4;
		            if (Strings.Mid(str4, 1, 6).ToUpper() == "DEFTP ")
		            {
		                if (Strings.InStr(str4, "=", CompareMethod.Text) > 0)
		                    text = Strings.Mid(str4, 6, Strings.InStr(str4, "=", CompareMethod.Text) - 6).Trim(new char[] { ' ' });
		                else
		                    text = Strings.Mid(str4, 6).Trim(new char[] { ' ' });
		            }
		            else if (Strings.Mid(str4, 1, 16).ToLower() == "decl inlineform ")
		            {
		                str2 = Strings.Mid(str4, 16, Strings.InStr(str4, "=", CompareMethod.Text) - 16).Trim(new char[] { ' ' });
		                string expression = this.KfdGetPar(str4, "CMDSHOW");
		                string str7 = this.KfdGetPar(str4, "CMDALIAS");
		                string str8 = this.KfdGetPar(str4, "STYLE").ToUpper();
		                string str9 = this.KfdGetPar(str4, "TPNAME[]");
		                string str10 = this.KfdGetPar(str4, "TPVERSION[]");
		                string str11 = "";
		                if (Strings.Len(str7) > 0)
		                    str11 = str7;
		                else if (Strings.Len(expression) > 0)
		                    str11 = expression;

		                if (Strings.InStr(str4, "CMDSHOW ", CompareMethod.Text) == 0)
		                    expression = str2;

		                string str12 = str8;
		                switch (str12)
		                {
		                    case "":
		                    case "WYSIWYG":
		                        if (Strings.Len(str11) == 0)
		                        {
		                            str11 = text + "." + str2;
		                        }
		                        break;
		
		                    default:
		                        if ((str12 == "SUB") && (Strings.Len(str11) == 0))
		                        {
		                            str11 = text + str2;
		                        }
		                        break;
		                }
		                TreeListViewItem item2 = new TreeListViewItem(str11, 2);
		                item2.SubItems.Add(sFile);
		                item2.SubItems.Add(text);
		                item2.SubItems.Add(str2);
		                item2.SubItems.Add(expression);
		                item2.SubItems.Add(str7);
		                item2.SubItems.Add(str8);
		                item2.SubItems.Add(str9);
		                item2.SubItems.Add(str10);
		                item.Items.Add(item2);
		            }
		            str5 = "";
		        }
		        
		        if (sTyp == "TPBasis")
		            this.tlvBasis.Items.Add(item);
		        else
		            this.tlvUser.Items.Add(item);
		    }
		    catch (Exception ex)
		    {
		    	MessageWindow.Add(ex);
		        this.SendErrorMsg();
		    }
		    */
		}
		
// ReSharper disable UnusedParameter.Local
private bool ReDeclFromDat(string sName, string sVal)
// ReSharper restore UnusedParameter.Local
{
#pragma warning disable 168
    var strArray = new string[1];
#pragma warning restore 168
    const bool flag = false;
    /*
    try
    {
        bool flag2;
        string[] strArray2;
        int editorDat = this.GetEditorDat(false);
        
        if (editorDat < 0)
        {
        	MessageWindow.Add("No datalist, do redeclare Variables");
            return flag;
        }
        var editorFile = this.GetEditorFile(editorDat);

        if (Strings.InStr(sName, ".", CompareMethod.Binary) > 0)
            strArray = Strings.Split(sName, ".", -1, CompareMethod.Binary);
        else
            strArray[0] = sName;

        var upperBound = this.sVar.GetUpperBound(0);
        var index = 0;
        while (index <= upperBound)
        {
            if (((this.sVar[index, 0] != "") && (this.sVar[index, 0].ToUpper() == strArray[0].ToUpper())) && ((this.sVar[index, 3] == editorFile) && (this.sVar[index, 7] == "")))
            {
                flag2 = true;
                break;
            }
            index++;
        }
        
        if (flag2 && (this.sVar[index, 2] != ""))
        {
            this.Editor[editorDat].TextChanged -= new EventHandler(this.Editor_TextChanged);
            int startIndex = this.Editor[editorDat].Text.Substring(0, Conversions.ToInteger(this.sVar[index, 6])).LastIndexOf("\n") + 1;
            int count = this.Editor[editorDat].Text.IndexOf("\n", Conversions.ToInteger(this.sVar[index, 6])) - startIndex;
            this.Editor[editorDat].Text = this.Editor[editorDat].Text.Remove(startIndex, count).Insert(startIndex, sVal);
            this.VarLoadEditorDat(editorDat, editorFile, "");
            this.Editor[editorDat].TextChanged += new EventHandler(this.Editor_TextChanged);
            flag = true;
        }
        if (!flag)
        {
            strArray2 = this.Editor[editorDat].Text.Split(new char[] { '\n' });
            int num6 = strArray2.Length - 1;
            for (index = 0; index <= num6; index++)
            {
                int length = Strings.InStr(strArray2[index], ";", CompareMethod.Binary);
                if (length == 0)
                    length = Strings.Len(strArray2[index]);

                if (Strings.InStr(Strings.Mid(strArray2[index], 1, length).Trim(), "ENDDAT", CompareMethod.Text) == 1)
                {
                    strArray2 = (string[]) Utils.CopyArray((Array) strArray2, new string[strArray2.Length + 1]);
                    int num8 = index + 1;
                    for (int i = strArray2.Length - 1; i >= num8; i += -1)
                        strArray2[i] = strArray2[i - 1];
 
                    strArray2[index] = sVal;
                    flag = true;
                    break;
                }
            }
        }
        if (!flag)
        {
            MessageWindow.Add(String.Format("Variable {0} not found",sName));
            return flag;
        }
        MessageWindow.Add(String.Format("Variable {0} redeclared",sName);
        if (!flag2)
        {
            this.Editor[editorDat].TextChanged -= new EventHandler(this.Editor_TextChanged);
            this.EditorSetStringA(editorDat, strArray2, false);
            this.VarLoadEditorDat(editorDat, this.GetEditorFile(editorDat), "");
            this.Editor[editorDat].TextChanged += new EventHandler(this.Editor_TextChanged);
        }
    }
    catch (Exception ex)
    {
    	MessageWindow.Add(ex);
        this.SendErrorMsg();
    }
    */
    return flag;
}

 

// ReSharper disable UnusedParameter.Local
private bool ScriptReDecl(string sPar1, string sPar2)
// ReSharper restore UnusedParameter.Local
{
    const bool flag = false;
    /*
		    try
		    {
		        string str3;
		        sPar1 = sPar1.Replace("/", " /");
		        sPar2 = sPar2.Replace("/", " /");
		        int num = this.ParamCount();
		        for (int i = 1; i <= num; i++)
		        {
		            string str = this.ParamGetByIndex(i, 0);
		            string replacement = this.ParamGetByIndex(i, 5);
		            sPar1 = Strings.Replace(sPar1, "%" + str + " ", replacement, 1, -1, CompareMethod.Text);
		            sPar2 = Strings.Replace(sPar2, "%" + str + " ", replacement, 1, -1, CompareMethod.Text);
		        }
		        sPar1 = sPar1.Replace(" /", "");
		        sPar2 = sPar2.Replace(" /", "");
		        sPar1 = sPar1.Replace("/", "");
		        sPar2 = sPar2.Replace("/", "");
		        string[] strArray = Strings.Split(sPar2, " ", -1, CompareMethod.Binary);
		        int index = 1;
		        if (strArray.Length > 1)
		        {
		            if (strArray[index - 1].ToUpper() == "DECL")
		            {
		                index = 2;
		            }
		            str3 = strArray[index];
		            if (Strings.InStr(strArray[index], "=", CompareMethod.Binary) > 0)
		            {
		                str3 = Strings.Split(strArray[index], "=", -1, CompareMethod.Binary)[0];
		            }
		            else
		            {
		                str3 = strArray[index];
		            }
		        }
		        flag = this.ReDeclFromDat(str3, sPar2);
		    }
		    catch (Exception ex)
		    {
		    	MessageWindow.Add(ex);
		        this.SendErrorMsg();
		    }
		    */
		    return flag;
}

// ReSharper disable UnusedParameter.Local
	    private bool ScriptSetVar(string sPar1, string sPar2)
// ReSharper restore UnusedParameter.Local
	    {
	        const bool flag = false;
	        /*
    try
    {
        int num = this.ParamCount();
        for (int i = 1; i <= num; i++)
        {
            string str = this.ParamGetByIndex(i, 0);
            string replacement = this.ParamGetByIndex(i, 5);
            sPar1 = Strings.Replace(sPar1, "%" + str + " ", replacement, 1, -1, CompareMethod.Text);
            sPar2 = Strings.Replace(sPar2, "%" + str + " ", replacement, 1, -1, CompareMethod.Text);
        }
        if (sPar1 == "$TMP")
        {
            flag = true;
            this.scTmp = sPar2;
            return flag;
        }
        flag = this.SetVarFromDat(sPar1, sPar2, true);
    }
    catch (Exception ex)
    {
    	MessageWindow.Add(ex);
        this.SendErrorMsg();
    }
    */
    return flag;
	    }


// ReSharper disable UnusedParameter.Local
	    private bool ScriptShowVar(string sPar1, string sPar2)
// ReSharper restore UnusedParameter.Local
{
#pragma warning disable 168
 	var strArray = new List<string>(1);
#pragma warning restore 168
#pragma warning disable 168
    var expression = String.Empty;
#pragma warning restore 168
    const bool flag = false;
#pragma warning disable 168
    var scTmp = String.Empty;
#pragma warning restore 168
/*
    try
    {
        int num2;
        int num = this.ParamCount();
        for (num2 = 1; num2 <= num; num2++)
        {
            string str2 = this.ParamGetByIndex(num2, 0);
            scTmp = this.ParamGetByIndex(num2, 5);
            
            if (sPar1.IndexOf("%" + str2 + " ") > 0)
            {
//                sPar1 = Strings.Replace(sPar1, "%" + str2 + " ", scTmp, 1, -1, CompareMethod.Text);
//                sPar1 = sPar1.Replace("%"+str2+");
                break;
            }
        }
        
        switch (sPar1)
        {
        	case "$TMP":
        		scTmp = scTmp; break;
        	case "$STAMP":
        		scTmp = string.Format("{0} {1}:{2}",DateTime.Today.ToString(),DateTime.Now.Hour,DateTime.Now.Minute);break;
        	case "$TQ_ID":
        		scTmp = this.TQ_TriggerId; break;
        	case "$COUNTUP":
        		scTmp = ParamGet(sPar2,5);
        		for (num2 = scTmp.Length;num2>=1; num2 += -1)
        		{
        			if (!IsNumber(scTmp.Substring(num2,1)))
        			    break;
        			
        			    expression = scTmp.Substring(num2,1) + expression;
        		}
        		if (!String.IsNullOrEmpty(expression))
        			scTmp = scTmp.Substring(1,scTmp.Length-expression.Length) +  Convert.ToString(Convert.ToInt32(expression) + 1);
        		break;
        	default:
        		if (sPar1.Contains("\""))
        			sPar1=sPar1.Replace("\"","").Trim();
        		
        		scTmp = GetVarFromDat(sPar1);
        		
        		if (String.IsNullOrEmpty(scTmp))
        			if (sPar1.IndexOf(".") > 0)
        				strArray = sPar1.Split(".",-1);
        			else
        				strArray[0] = sPar1;

        		var num4 = this.VarCount -1;
        
                for (num2 = 0; num2 <= num4; num2++)
                {
                    if (this.sVar[num2, 0].ToUpper() == strArray[0].ToUpper())
                    {
                        if (strArray.Length == 1)
                            scTmp = this.sVar[num2, 2];
                        else
                        {
                            var num5 = strArray.Length - 1;
                            for (var i = 1; i <= num5; i++)
                                scTmp = this.GetStrucVal(this.sVar[num2, 2], strArray[i]);
                        }
                    }
                }
            }
        if (!String.IsNullOrEmpty(scTmp))
        {
            scTmp = scTmp.Replace("\"", "").Trim();
            this.ParamSet(sPar2, scTmp, 5, false);
        }
        else
        {
        	MessageWindow.Add(String.Format("Variable {0} not found",sPar1));
        }
        flag = true;
    }
    catch (Exception ex)
    {
    	MessageWindow.Add(ex);
        this.SendErrorMsg();
    }
    */
    return flag;
}
 

 public bool SetVarFromDat(string sVarIn, string sVal, bool bRefresh = true)
{
#pragma warning disable 168
    var strArray = new string[1];
#pragma warning restore 168
    const bool flag = false;
    /*
    try
    {
        int num3;
        string str;
        int num6;
        int editorDat = this.GetEditorDat(false);
        if (editorDat < 0)
        {
            return flag;
        }
        if (Strings.InStr(sVarIn, ".", CompareMethod.Binary) > 0)
        {
            strArray = Strings.Split(sVarIn, ".", -1, CompareMethod.Binary);
        }
        else
        {
            strArray[0] = sVarIn;
        }
        string[] sStringA = this.Editor[editorDat].Text.Split(new char[] { '\n' });
        int num2 = sStringA.Length - 1;
        for (num3 = 0; num3 <= num2; num3++)
        {
            int length = Strings.InStr(sStringA[num3], ";", CompareMethod.Binary);
            if (length == 0)
            {
                length = Strings.Len(sStringA[num3]);
            }
            str = Strings.Mid(sStringA[num3], 1, length);
            if ((Strings.InStr(str, " " + strArray[0] + "=", CompareMethod.Text) > 0) || (Strings.InStr(str, " " + strArray[0] + " ", CompareMethod.Text) > 0))
            {
                if (strArray.Length == 1)
                {
                    sStringA[num3] = Strings.Replace(sStringA[num3], Strings.Mid(str, Strings.InStr(str, "=", CompareMethod.Binary) + 1).Trim(), sVal, 1, -1, CompareMethod.Binary);
                }
                else
                {
                    str = Strings.Mid(str, Strings.InStr(str, "=", CompareMethod.Binary) + 1).Trim();
                    int num5 = strArray.Length - 1;
                    num6 = 1;
                    while (num6 <= num5)
                    {
                        sStringA[num3] = Strings.Replace(sStringA[num3], strArray[num6] + " " + this.KfdGetPar(str, strArray[num6]), strArray[num6] + " " + sVal, 1, -1, CompareMethod.Binary);
                        num6++;
                    }
                }
                flag = true;
                break;
            }
        }
        if (!flag)
        {
            if (strArray[0].Contains("["))
            {
                strArray[0] = strArray[0].Substring(0, strArray[0].IndexOf("["));
            }
            int num7 = this.VarCount();
            for (num3 = 0; num3 <= num7; num3++)
            {
                if (this.sVar[num3, 4].ToUpper() == "TRUE")
                {
                    string str3;
                    if (this.sVar[num3, 0].Contains("["))
                    {
                        str3 = this.sVar[num3, 0].Substring(0, this.sVar[num3, 0].IndexOf("["));
                    }
                    else
                    {
                        str3 = this.sVar[num3, 0];
                    }
                    if (str3.ToUpper() == strArray[0].ToUpper())
                    {
                        if (strArray.Length == 1)
                        {
                            this.sVar[num3, 2] = sVal;
                        }
                        else
                        {
                            str = this.sVar[num3, 2];
                            int num8 = strArray.Length - 1;
                            for (num6 = 1; num6 <= num8; num6++)
                            {
                                str = this.KfdGetPar(str, strArray[num6]);
                            }
                            this.sVar[num3, 2] = Strings.Replace(this.sVar[num3, 2], str, sVal, 1, -1, CompareMethod.Binary);
                        }
                        flag = true;
                        break;
                    }
                }
            }
        }
        if (!flag)
        {
            this.SendStatusMsg(language.s("Variable %1 not found", sVarIn, "", "", ""), "!", 0, 0);
            return flag;
        }
        this.EditorSetStringA(editorDat, sStringA, bRefresh);
    }
    catch (Exception ex)
    {
    	MessageWindow.Add(ex);
        this.SendErrorMsg();
    }
    */
// ReSharper disable ConditionIsAlwaysTrueOrFalse
    return flag;
// ReSharper restore ConditionIsAlwaysTrueOrFalse
}

 private void UpdateControls()
		{/*
		    try
		    {
		        int sNo = 1;
		        do
		        {
		            if (this.ParamGetByNo(sNo, 0) != "")
		            {
		                string sPar = this.ParamGetByNo(sNo, 0);
		                bool flag = Strings.StrComp(this.ParamGet(sPar, 14), "true", CompareMethod.Text) != 0;
		                if (this.ParamGet(sPar, 2) != "FALSE")
		                {
		                    string str3 = this.ParamGet(sPar, 3);
		                    if (str3 == "FREE")
		                    {
		                        this.cParamText[sNo].Text = this.ParamGet(sPar, 5);
		                        this.cParamText[sNo].Visible = flag;
		                    }
		                    else if (str3 != "STATIC")
		                    {
		                        if (str3 == "NAME")
		                        {
		                            this.cParamText[sNo].Text = this.ParamGet(sPar, 5);
		                            this.cParamText[sNo].Visible = flag;
		                        }
		                        else if (str3 == "NUMBER")
		                        {
		                            this.cParamText[sNo].Text = this.ParamGet(sPar, 5);
		                            this.cParamText[sNo].Visible = flag;
		                        }
		                        else if (str3 == "REAL")
		                        {
		                            this.cParamText[sNo].Text = this.ParamGet(sPar, 5);
		                            this.cParamText[sNo].Visible = flag;
		                        }
		                        else if (str3 == "LIST")
		                        {
		                            this.ParamListFill(sNo);
		                            this.cParamList[sNo].Visible = flag;
		                        }
		                    }
		                }
		            }
		            sNo++;
		        }
		        while (sNo <= 255);
		    }
		    catch (Exception ex)
		    {
		    	MessageWindow.Add(ex);
		        this.SendErrorMsg();
		    }
		    */
		}

 

 private void UpdateControlsSet()
{
 	/*
    try
    {
        string str;
        if (this.sActIlf[0] == "TPBasis")
        {
            str = " ";
        }
        else if (this.sActIlf[0] == "TPUser")
        {
            str = ", ";
        }
        this.IlfPanel.SuspendLayout();
        int sNo = 1;
        do
        {
            if (this.ParamGetByNo(sNo, 0) != "")
            {
                string sPar = this.ParamGetByNo(sNo, 0);
                bool flag = Strings.StrComp(this.ParamGet(sPar, 14), "true", CompareMethod.Text) != 0;
                if (this.ParamGet(sPar, 2) != "FALSE")
                {
                    string str4 = "";
                    switch (this.ParamGet(sPar, 3))
                    {
                        case "FREE":
                            this.cParamText[sNo].Visible = flag;
                            break;

                        case "STATIC":
                            str4 = this.ParamGet(sPar, 5);
                            break;

                        case "NAME":
                            this.cParamText[sNo].Visible = flag;
                            break;

                        case "NUMBER":
                            this.cParamText[sNo].Visible = flag;
                            break;

                        case "REAL":
                            this.cParamText[sNo].Visible = flag;
                            break;

                        case "LIST":
                            this.cParamList[sNo].Visible = flag;
                            break;
                    }
                    this.cLabel[sNo].Text = "";
                    if (flag)
                    {
                        int num2;
                        if (sNo > 1)
                        {
                            num2 = sNo - 1;
                            this.cLabel[num2].Text = this.cLabel[num2].Text + str;
                        }
                        else
                        {
                            this.cLabel[sNo - 1].Text = "";
                        }
                        num2 = sNo - 1;
                        this.cLabel[num2].Text = this.cLabel[num2].Text + this.ParamGet(sPar, 4);
                        this.cLabel[sNo].Text = this.ParamGet(sPar, 6) + str4;
                    }
                }
            }
            sNo++;
        }
        while (sNo <= 255);
        this.IlfPanel.ResumeLayout();
    }
    catch (Exception ex)
    {
    	MessageWindow.Add(ex);
        this.SendErrorMsg();
    }
    */
}

 private void LoadUserFiles()
{
    /*
    try
    {
        foreach (string str in Directory.GetFiles(this.sEnvPathTpUser))
            if (Path.GetExtension(str).ToLower() == ".kfd")
                this.LoadFile(str, "TPUser");

        MessageWindow.Add("TP Initialization complete");
    }
    catch (Exception ex)
    {
        MessageWindow.Add(ex);
        this.SendErrorMsg();
    }
    
}
*/
#pragma warning restore 169
    // ReSharper restore UnusedMember.Local

}
}
}

