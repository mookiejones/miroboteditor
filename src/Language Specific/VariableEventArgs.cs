using System;

namespace miRobotEditor.Language_Specific
{
    public class VariableEventArgs:EventArgs
{ 
    public VariableEventArgs()
    {}
    public VariableEventArgs(string name)
    {_name = name;}
    private string _name = string.Empty;   
    public string Name
    {
        get{return _name;}
        set{_name = value;}
    }
        private readonly KUKAVariableViewerViewModel.Variable _variable;
        public KUKAVariableViewerViewModel.Variable Variable => _variable;

        public VariableEventArgs(KUKAVariableViewerViewModel.Variable variable)
        {
            _variable = variable;
        }
    }    
}

