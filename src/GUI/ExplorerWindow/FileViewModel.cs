using System;
using System.Drawing;

namespace miRobotEditor.GUI.ExplorerWindow
{
	public class FileViewModel : ItemViewModel
	{

	
		public FileViewModel( string fullPath, string name )
		{
			FullPath = fullPath;
			Name = name;

		    try
		    {
		        var i = Icon.ExtractAssociatedIcon(fullPath);
		        ImageName = i.ToBitmap();
		       //   ImageName = "../../Resources/files.png";
		    }
		    catch (Exception)
		    {
                ImageName = "../../Resources/files.png";
		    }
		}
	}

}
