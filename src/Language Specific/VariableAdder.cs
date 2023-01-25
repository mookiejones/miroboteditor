using System;

namespace miRobotEditor.Language_Specific
{
    public class VariableAdder:ViewModel.ViewModelBase
    {

         public event AddVariableEventHandler AddVariableClick;
        private string _text;
        public string Text{get { return _text; }set { _text = value;RaisePropertyChanged("Text"); }}

        private int _interval;
        public int Interval{get { return _interval; }set { _interval = value;RaisePropertyChanged("Interval"); }}



        void AddVariable(object param)
        {
            if ((!String.IsNullOrEmpty(Text))&&(AddVariableClick!=null))
                AddVariableClick(this,new VariableEventArgs(new KUKAVariableViewerViewModel.Variable(Interval,Text)));
        }

        public void Clear()
        {
            
        }
    }
}

