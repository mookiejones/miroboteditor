/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 4/12/2013
 * Time: 11:48 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using miRobotEditor.ViewModel;
namespace miRobotEditor.Dialogs
{
	/// <summary>
	/// Description of KFDDialogViewModel.
	/// </summary>
	public class KFDDialogViewModel:ViewModelBase
	{
		string _button1Text = "button1";
		string _button2Text = "button2";
		string _button3Text = "button3";
		string _button4Text = "button4";
		string _button5Text = "button5";
		string _button6Text = "button6";
		string _button7Text = "button7";
		
		public string Button1Text{get{return _button1Text;}set{_button1Text=value;RaisePropertyChanged("Button1Text");}}
		public string Button2Text{get{return _button2Text;}set{_button2Text=value;RaisePropertyChanged("Button2Text");}}
		public string Button3Text{get{return _button3Text;}set{_button3Text=value;RaisePropertyChanged("Button3Text");}}
		public string Button4Text{get{return _button4Text;}set{_button4Text=value;RaisePropertyChanged("Button4Text");}}
		public string Button5Text{get{return _button5Text;}set{_button5Text=value;RaisePropertyChanged("Button5Text");}}
		public string Button6Text{get{return _button6Text;}set{_button6Text=value;RaisePropertyChanged("Button6Text");}}
		public string Button7Text{get{return _button7Text;}set{_button7Text=value;RaisePropertyChanged("Button7Text");}}
		
		private bool _b1Visible = true,_b2Visible = true,_b3Visible = true,_b4Visible = true,_b5Visible = true,_b6Visible = true,_b7Visible = true;
		
		
		public bool Button1Visible{get{return _b1Visible;}set{_b1Visible=value;RaisePropertyChanged("Button1Visible");}}
		public bool Button2Visible{get{return _b2Visible;}set{_b2Visible=value;RaisePropertyChanged("Button2Visible");}}
		public bool Button3Visible{get{return _b3Visible;}set{_b3Visible=value;RaisePropertyChanged("Button3Visible");}}
		public bool Button4Visible{get{return _b4Visible;}set{_b4Visible=value;RaisePropertyChanged("Button4Visible");}}
		public bool Button5Visible{get{return _b5Visible;}set{_b5Visible=value;RaisePropertyChanged("Button5Visible");}}
		public bool Button6Visible{get{return _b6Visible;}set{_b6Visible=value;RaisePropertyChanged("Button6Visible");}}
		public bool Button7Visible{get{return _b7Visible;}set{_b7Visible=value;RaisePropertyChanged("Button7Visible");}}
		
		private int _width = 592;
		public int Width{get {return _width;}set{_width=value;RaisePropertyChanged("Width");}}
		
		
		private int _answer;
		public int Answer{get{return _answer;}set{_answer=value;RaisePropertyChanged("Answer");}}
		
		
		public KFDDialogViewModel()
		{
			
			Button7Visible = !String.IsNullOrEmpty(Button7Text);			
			if (!Button7Visible)Width=-81;

			Button6Visible = !String.IsNullOrEmpty(Button6Text);
			if (!Button6Visible)Width=-81;
			
			Button5Visible = !String.IsNullOrEmpty(Button5Text);			
			if (!Button5Visible)Width=-81;

			Button4Visible = !String.IsNullOrEmpty(Button4Text);			
			if (!Button4Visible)Width=-81;

			Button3Visible = !String.IsNullOrEmpty(Button3Text);			
			if (!Button3Visible)Width=-81;

			Button2Visible = !String.IsNullOrEmpty(Button2Text);			
			if (!Button2Visible)Width=-81;
			
			Button1Visible = !String.IsNullOrEmpty(Button1Text);			
			if (!Button1Visible)Width=-81;
		}
		
		
	}
}
