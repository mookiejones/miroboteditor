/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 4/20/2013
 * Time: 9:31 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows;
using System.Windows.Controls;
using AvalonDock.Layout;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using miRobotEditor.ViewModel;

namespace miRobotEditor
{
	/// <summary>
	/// Description of PanesStyleSelector.
	/// </summary>
    class PanesStyleSelector : StyleSelector
    {
        public Style ToolStyle
        {
            get;
            set;
        }

        public Style FileStyle
        {
            get;
            set;
        }
        
        public override System.Windows.Style SelectStyle(object item, System.Windows.DependencyObject container)
        {
        	
            if (item is ToolViewModel)
                return ToolStyle;

            if (item is miRobotEditor.GUI.DummyDoc)
                return FileStyle;

            return base.SelectStyle(item, container);
        }
    }
      public class ToolViewModel : PaneViewModel
    {
        public ToolViewModel(string name)
        {
            Name = name;
            Title = name;
        }

        public string Name
        {
            get;
            private set;
        }


        #region IsVisible

        private bool _isVisible = true;
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    RaisePropertyChanged("IsVisible");
                }
            }
        }

        #endregion


    }
      public class PaneViewModel : ViewModelBase,IPaneViewModel
    {
         public PaneViewModel()
        { }


        #region Title

        private string _title = null;
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    RaisePropertyChanged("Title");
                }
            }
        }

        #endregion

        public System.Windows.Media.ImageSource IconSource
        {
            get;
            set;
        }

        #region ContentId

        private string _contentId = null;
        public string ContentId
        {
            get { return _contentId; }
            set
            {
                if (_contentId != value)
                {
                    _contentId = value;
                    RaisePropertyChanged("ContentId");
                }
            }
        }

        #endregion

        #region IsSelected

        private bool _isSelected = false;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    RaisePropertyChanged("IsSelected");
                }
            }
        }

        #endregion

        #region IsActive

        private bool _isActive = false;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    RaisePropertyChanged("IsActive");
                }
            }
        }

        #endregion


    }
    
    public interface IPaneViewModel
    {
    	 string Title{get;set;}
    	 System.Windows.Media.ImageSource IconSource{get;  set;}
    	 string ContentId{get;set;}
    	 bool IsSelected {get;set;}
    	 bool IsActive{get;set;}    	
    }
    
        class PanesTemplateSelector : DataTemplateSelector
    {
        public PanesTemplateSelector()
        {
        
        }


        public DataTemplate FileViewTemplate
        {
            get;
            set;
        }

        public DataTemplate FileStatsViewTemplate
        {
            get;
            set;
        }

        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            var itemAsLayoutContent = item as LayoutContent;

            
           if (item is GUI.DummyDoc)
                 return FileViewTemplate;
		//
        //   if (item is FileStatsViewModel)
        //       return FileStatsViewTemplate;

            return base.SelectTemplate(item, container);
        }
    }

}
