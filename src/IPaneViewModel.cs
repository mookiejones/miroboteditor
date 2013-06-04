namespace miRobotEditor
{
    public interface IPaneViewModel
    {
        string Title{get;set;}
        System.Windows.Media.ImageSource IconSource{get;  set;}
        string ContentId{get;set;}
        bool IsSelected {get;set;}
        bool IsActive{get;set;}    	
    }
}