/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 4/15/2013
 * Time: 1:13 PM
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using miRobotEditor.Classes;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using miRobotEditor.GUI;
using System.IO;
using System.ComponentModel;
using System.Windows.Input;
using miRobotEditor.Commands;
using AvalonDock.Layout;
using System.Linq;
using miRobotEditor.Properties;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
namespace miRobotEditor.ViewModel
{
	public class MainViewModel:ViewModelBase
	{

        private string _title = string.Empty;
        public string Title { get { return _title; } set { _title = value; RaisePropertyChanged("Title"); } }

        private DummyDoc _ActiveEditor = new DummyDoc();
        public DummyDoc ActiveEditor { get { return _ActiveEditor; } set { _ActiveEditor = value; RaisePropertyChanged("ActiveEditor"); } }
        		
		#region Commands
		
		#region OpenFileCommand

		RelayCommand _openCommand;
		public ICommand OpenCommand 
		{
			get
			{
				return _openCommand??(_openCommand= new RelayCommand((p) => OpenFile(p),(p) => CanOpen(p) ));
			}
		}
		
		bool CanOpen(object param)
		{
			return true;
		}
		
		void OpenFile(object param)
        {
            var fn = GetFileName();
            foreach (var f in fn)
                if (File.Exists(f))
                    OpenFile(f);
        }
		
		        

        [Localizable(false)]
        private IEnumerable<string> GetFileName()
        {
        

         	//Get Current Filename so i dont have to keep referencing ActiveEditor
            var fn = ActiveEditor.Filename;

           //Sets Current Directory to ActiveEditor
           var dir = Path.GetDirectoryName(fn);

            var ofd = new OpenFileDialog
                          {
                              DefaultExt=File.Exists(fn)?Path.GetExtension(fn):"*.*",
                              Filter = Properties.Resources.DefaultFilter,
                              Multiselect = true,
                              FilterIndex = Settings.Default.Filter,
                              InitialDirectory = dir != null && Directory.Exists(dir)?dir:String.Empty
                          };
            
           //ofd.Filter =  "All _files (*.*)|*.*|ABB _files (.mod,.prg,.cfg)|*.mod;*.prg;*.cfg|Kawasaki _files(*.as)|*.as|KUKA _files (.sub,.src,.dat,.kfd)|*.sub;*.src;*.dat;*.kfd|Fanuc _files (*.ls)|*.ls";


            // Display OpenFileDialog by calling ShowDialog method
            var result = ofd.ShowDialog();
            
            Settings.Default.Filter = ofd.FilterIndex;
           
            // Get the selected file name and display in a TextBox
            return result==true ? ofd.FileNames : new[]{String.Empty};
        }
        

        
		
         #endregion

        #region CloseFileCommand         
        /// <summary>
		/// Relay Command for Exiting Application
		/// </summary>
		private  RelayCommand _closeCommand;

        public  ICommand CloseCommand
        {
            get { return _closeCommand ?? (_closeCommand = new RelayCommand(param => Close(), param => true)); }
        }
        
        void Close()
        {
        	
        }
		#endregion
		
		#endregion
		
	}
}
