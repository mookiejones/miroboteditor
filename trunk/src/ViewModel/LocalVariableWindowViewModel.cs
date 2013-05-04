/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 4/5/2013
 * Time: 9:11 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using miRobotEditor.Classes;
using System.Collections.Generic;
namespace miRobotEditor.ViewModel
{
	/// <summary>
	/// Description of LocalVariableWindowViewModel.
	/// </summary>
	public class LocalVariableWindowViewModel:AbstractPaneViewModel
	{
		
		private List<IVariable> _variables = new List<IVariable>();
		public List<IVariable> Variables{get{return _variables;}set{_variables=value;RaisePropertyChanged("Variables");}}
		
		
		public static LocalVariableWindowViewModel Instance{get;set;}
		 /// <summary>
        /// View-model for main window.
        /// </summary>
        private MainViewModel ParentViewModel{get;set;}
		
        public LocalVariableWindowViewModel()
        {
        	Instance = this;
        }
        
        
    //    	public LocalVariableWindowViewModel(MainViewModel parentviewmodel) 
    //	{
    //		   if (parentviewmodel == null)
    //        {
    //            throw new ArgumentNullException("mainWindowViewModel");
    //        }
	//
    //        this.ParentViewModel = parentviewmodel;
    //        this.ParentViewModel.ActiveDocumentChanged += new EventHandler<EventArgs>(ParentViewModel_ActiveDocumentChanged);
    //        Instance=this;
    //
    //	}
    	
    	 /// <summary>
        /// Event raised when the active document in the main window's view model has changed.
        /// </summary>
        private void ParentViewModel_ActiveDocumentChanged(object sender, EventArgs e)
        {         
 	        RaisePropertyChanged("Variables");
        }

	}
}
