using GalaSoft.MvvmLight;
using miRobotEditor.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace miRobotEditor.ViewModel
{
    public class ToolsViewModel:ViewModelBase
    {

        public ToolsViewModel()
        {
            Tools = new ObservableCollection<ToolViewModel>();
            Tools.Add(ObjectBrowser);
            Tools.Add(MessageView);
            Tools.Add(Notes);
            Tools.Add(LocalVariables);
            Tools.Add(Functions);
            Tools.Add(AngleConverter);
        }
        

        private readonly LocalVariablesViewModel _localVariables = null;
        public LocalVariablesViewModel LocalVariables { get { return _localVariables ?? new LocalVariablesViewModel(); } }



        public ObservableCollection<ToolViewModel> Tools{get;set;}



        #region · Fields·
        private readonly AngleConvertorViewModel _angleConverter = null;
        private readonly FunctionViewModel _functions = null;
        private readonly MessageViewModel _messageView = null;
        private readonly NotesViewModel _notes = null;
        //Object Browser
        private readonly ObjectBrowserViewModel _objectBrowser = null;

        #endregion

        #region · Properties ·


        public ObjectBrowserViewModel ObjectBrowser { get { return _objectBrowser ?? new ObjectBrowserViewModel(); } }


        public NotesViewModel Notes { get { return _notes ?? new NotesViewModel(); } }


        public MessageViewModel MessageView
        {
            get { return _messageView ?? new MessageViewModel(); }
        }


        public FunctionViewModel Functions { get { return _functions ?? new FunctionViewModel(); } }



        public AngleConvertorViewModel AngleConverter { get { return _angleConverter ?? new AngleConvertorViewModel(); } }

        #endregion

        #region · Commands ·
        #endregion

        #region · Private Methods ·
        #endregion


    }
}
