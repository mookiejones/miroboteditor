namespace miRobotEditor.ViewModel
{
    public class GlobalOptions
    {
        private static GlobalOptions _instance = new GlobalOptions();
        public static GlobalOptions Instance {get{return _instance??new GlobalOptions();}set{_instance=value;}}
			
		
        private GlobalOptionsViewModel _options	= new GlobalOptionsViewModel();
        public GlobalOptionsViewModel Options{get{return _options;}set{_options=value;}}
    }
}