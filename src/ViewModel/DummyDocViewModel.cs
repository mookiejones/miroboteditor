/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 04/09/2013
 * Time: 14:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using miRobotEditor.Classes;
using miRobotEditor.Languages;
using miRobotEditor.GUI;
using System.IO;
using System.Text;
using System.Windows.Input;
using miRobotEditor.Commands;
using System.Collections.ObjectModel;
namespace miRobotEditor.ViewModel
{


  //  public delegate void UpdateFunctionEventHandler(object sender, FunctionEventArgs e);
	/// <summary>
	/// Description of DummyDocViewModel.
	/// </summary>
	public class DummyDocViewModel:ViewModelBase
	{
		#region Properties
		
		
		/// <summary>
        /// The default name for untitled files.
        /// </summary>
        public static readonly string UntitledFileName = "Untitled.txt";
        
		private string _filename;
		public string Filename {get{ return _filename;}set{_filename=value;RaisePropertyChanged("Filename");}}
		
		
		private object _host = null;
		public object Host{get{return _host;}set{_host = value;RaisePropertyChanged("Host");}}
		
		
		   /// <summary>
        /// The title of the document.
        /// </summary>
        public string Title
        {
            get
            {
                var title = new StringBuilder();
                title.Append(this.TextBox.Filename);

                if (this.IsModified)
                {
                    title.Append("*");
                }

                return title.ToString();
            }
        }
        
        
		
		
		private Controls.ExtendedGridSplitter _grid = new Controls.ExtendedGridSplitter();
		public Controls.ExtendedGridSplitter Grid{get{return _grid;}set{_grid=value;RaisePropertyChanged("Grid");}}
		
		
		
		private static DummyDocViewModel _instance = new DummyDocViewModel();
		public static DummyDocViewModel Instance{get{return _instance?? new DummyDocViewModel();}set{_instance=value;}}
		
		private Editor _data = new Editor();		
		public Editor Data{get{return _data;}set{_data = value;RaisePropertyChanged("Data");}}

		private Editor _source = new Editor();
		public Editor Source{get{return _source;}set{_source = value;RaisePropertyChanged("Source");}}
		
	 	private Editor _textbox = new Editor();
		public Editor TextBox{get{return _textbox;}set{_textbox = value;RaisePropertyChanged("TextBox");}}
		
		private AbstractLanguageClass _fileLanguage = new LanguageBase() ;
		public AbstractLanguageClass FileLanguage{get{return _fileLanguage;}set{_fileLanguage=value;RaisePropertyChanged("FileLanguage");}}
		
		

		 #endregion
	    #region Public Events

     //   public event UpdateFunctionEventHandler TextUpdated;

        #endregion
		 #region Commands

        private  RelayCommand _gridIsLoadedCommand;

        public   ICommand GridIsLoadedCommand
        {
            get { return _gridIsLoadedCommand ?? (_gridIsLoadedCommand = new RelayCommand(param => GridIsLoaded(param), param => true)); }
        }
        
        
        
        
        private  RelayCommand _visibleChangedCommand;

        public  ICommand VisibleChangedCommand
        {
            get { return _visibleChangedCommand ?? (_visibleChangedCommand = new RelayCommand(param => VisibleChanged(param), param => true)); }
        }

		 #endregion
		
		
		#region Constructor
		public DummyDocViewModel()
		{
			Instance = this;
		}
		
		
		public bool IsModified{get;set;}
		#endregion
		
		public bool IsDocumentModified
		{
			get{
				return Source.IsModified | Data.IsModified|TextBox.IsModified;
			}
		}

		private bool _isVisible = false;
		public bool IsVisible {get{return _isVisible;}set{_isVisible=value;RaisePropertyChanged("IsVisible");}}
		
		
		 public void VisibleChanged(object sender)
        {
           // if (MainWindow.Instance.IsClosing)
           //     return;
            if (IsVisible)
            Instance = this;
            if (!(sender is Editor)) return;
            TextBox = sender as Editor;
            FileLanguage=AbstractLanguageClass.GetRobotType(Filename);
        }
		 
		public static void UpdateVisualText(object param)
		{
			(param as Editor).UpdateVisualText();
		}
		 
        /// <summary>
        /// Select Text from variable offset
        /// </summary>
        /// <remarks>
        /// Selects appropriate editor that text resides in.
        /// </remarks>
        /// <param name="variable"></param>
        public void SelectText(IVariable var)
        {
        	if (var.Name == null) throw new ArgumentNullException("text");
            
        	//TODO Need to find out if this will work from Global Variables. Only Tested so far for Local Variable Window
        	
        	// Does Textbox have Variables
        	if (TextBox.Variables==null)
        		SwitchTextBox();
        	
        	
            // Is Offset of textbox greater than desired value?
            var enoughlines = TextBox.Text.Length >= var.Offset;
            if (enoughlines)            
	            TextBox.SelectText(var);        	
            else
            {
            	TextBox = Data;
            	enoughlines = TextBox.Text.Length >= var.Offset;
            	if (enoughlines)            
	            TextBox.SelectText(var); 
            }
        }
		
        private void SwitchTextBox()
        {
        	TextBox=TextBox.Name=="source"?Data:Source;
        }        
		 
        private void GridIsLoaded(object param)
        {
            Grid.IsAnimated = false;
            Grid.Collapse();
            Grid.IsCollapsed = true;
            Grid.IsAnimated = true;
        }
	}
}
