using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using System.ComponentModel;
namespace FTPBrowser
{
    //
    // The FTPFolder class allows you to store as well as quickly
    // access a folder's name and path.
    //
    public partial class FTPAccountWindow
	{
    	#region Members    	
    	
    	
    	
    	private FTPAccount _selectedvalue;
     	
    	private string SelectedPath = string.Empty;
    	
		private string language = "kuka";
		
        //Variables for storing server related information
        private const string FTPHEADER = "ftp://";

        public string startingPath;
		private const string FOLDERIMAGES = "pack://application:,,,/folderopen.gif";
        private const string ICONIMAGE = "pack://application:,,,/folder.gif";
        private const string FILEIMAGE = "pack://application:,,,/file.gif";
        //Our FtpWebRequest object
        FtpWebRequest request;
		#endregion
		
		public FTPAccountWindow()
		{
			this.InitializeComponent();					
		}
		
        //
        //Various dictionaries for creating mappings between folders, treeitems, and dockpanels
        //
        private Dictionary<string, TreeViewItem> folderNameToTreeItem = new Dictionary<string, TreeViewItem>();
        private Dictionary<string, TreeViewItem> seekAheadList = new Dictionary<string, TreeViewItem>();
        private Dictionary<TreeViewItem, string> treeItemToFolderName = new Dictionary<TreeViewItem, string>();
        private Dictionary<string, string> namePathDictionary = new Dictionary<string, string>();
        private Dictionary<DockPanel, string> dockPanelContent = new Dictionary<DockPanel, string>();

        //
        // Returns a folder name and strips out any folders with an extension (aka files)
        //
        private string ParseLine(string line, string path)
        {
            string[] parsedLine = line.Split('/');
            string temp;
            if (parsedLine.Length > 1)
            {
                temp = path + "/" + parsedLine[1];
                if (parsedLine[1].Contains("."))
                {
                    return "ERROR";
                }
            }
            else
            {
                temp = path + "/" + parsedLine[0];
                if (parsedLine[0].Contains("."))
                {
                    return "ERROR";
                }
            }
            return temp;
        }

        //
        // Initializes the application
        //
        private void SetupFTP()
        {
           
            // Is URI Path Valid
            if (!IsURIValid()) return;
            request = (FtpWebRequest)WebRequest.Create(startingPath);
            
            SetupTree();
        }
        
        bool IsURIValid()
        {
        	var ftp = FTPAccountViewModel.Instance.SelectedItem.Server;
        	if (string.IsNullOrEmpty(ftp))return false;
        	
        	// Does text start with ftp:// ?
        	var loc = ftp.IndexOf(FTPHEADER);
			
        	//TODO Automate this better
        	if(language=="kuka")
        	{
        		if (string.IsNullOrEmpty(FTPAccountViewModel.Instance.SelectedItem.Username))
        			FTPAccountViewModel.Instance.SelectedItem.Username="administrator";
        		if (string.IsNullOrEmpty(FTPAccountViewModel.Instance.SelectedItem.Password))
        			FTPAccountViewModel.Instance.SelectedItem.Password="kukarobxpe2";
        	}
        	if (loc == -1) // ftp string not in line
        		startingPath = FTPHEADER + ftp;
        	return true;
        }

        

        //
        // Responsible for initially drawing the tree
        //
        private void SetupTree()
        {
            var tv = new TreeViewItem();

            var dock = new DockPanel();
            dock.MouseUp += new MouseButtonEventHandler(TreeViewMouseDown);

         
            dock.Children.Add(GetImage(true,FOLDERIMAGES,ICONIMAGE));
            dock.Children.Add(new TextBlock{Text = startingPath});

            dockPanelContent.Add(dock, startingPath);

            tv.Header = dock;

            treeFolderBrowser.Items.Add(tv);

            folderNameToTreeItem.Add(startingPath, tv);
            treeItemToFolderName.Add(tv, startingPath);

            var folder = new FTPFolder(){Path=startingPath,Name=FTPFolder.SafeFolderName(startingPath)};
            
            DrawChildTreeItems(tv, folder, true);
        }

        
        
        //
        // Returns an image used by the TreeViewItem to designate a visited Node
        //
        
        private Image GetImage(bool expanded,string expimg,string notexpimg)
        {
            Image imageContent = new Image();
            string imagePath;
            imagePath = expanded?expimg:notexpimg;
            
            imageContent.Source = new BitmapImage(new Uri(imagePath));
            return imageContent;
        }
		
        private void AddItems(TreeViewItem parentTree,FTPFolder folder,bool expand,bool isfolder,List<string> items)
        {
        	 List<TreeViewItem> treeViewItemList = new List<TreeViewItem>();
        	foreach (string t in items)
            {
                if (folderNameToTreeItem.ContainsKey(t))
                {
                    continue;
                }
                else
                {
                    var tv = new TreeViewItem();
                    var newFolder = new FTPFolder{Path=t,Name=FTPFolder.SafeFolderName(t)};

                    var dock = new DockPanel();
                    dock.MouseUp += new MouseButtonEventHandler(TreeViewMouseDown);

                    var textContent = new TextBlock{Text = newFolder.Name};
                    
                    if (isfolder)
                    	dock.Children.Add(GetImage(false,FOLDERIMAGES,ICONIMAGE));
                    else                   	
                    	dock.Children.Add(GetImage(false,FILEIMAGE,FILEIMAGE));
                    dock.Children.Add(textContent);
                    dockPanelContent.Add(dock, newFolder.Path);

                    treeViewItemList.Add(tv);


                    tv.Header = dock;
                    parentTree.Items.Add(tv);

                    folderNameToTreeItem.Add(t, tv);
                    treeItemToFolderName.Add(tv, t);

                    parentTree.IsExpanded = expand;

                }
            }
        }
        
        //
        // Given a parent TreeViewItem, FTPFolder, and an expand flag, draws the children folders
        // in your directory structure
        //
        private void DrawChildTreeItems(TreeViewItem parentTree, FTPFolder folder, bool expand)
        {
            List<string> Folders = GetDirectories(folder.Path);
            List<string> Files = GetFiles(folder.Path);
            List<TreeViewItem> treeViewItemList = new List<TreeViewItem>();
            AddItems(parentTree,folder,expand,true,Folders);
            AddItems(parentTree,folder,expand,false,Files);
        }
		
        
        //
        // Our event handler for reacting to mousedown events
        //
        private void TreeViewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var clickedDock = (DockPanel)sender;
            clickedDock.Children.RemoveAt(0);
            clickedDock.Children.Insert(0,GetImage(true,FOLDERIMAGES,ICONIMAGE));
            var clickedText = dockPanelContent[clickedDock];

            var clickedFolder = new FTPFolder{Path=clickedText,Name=FTPFolder.SafeFolderName(clickedText)};
            DrawChildTreeItems(folderNameToTreeItem[clickedText], clickedFolder, true);
        }
        
        
        private string GetFTPFilePath(string path)
        {
        	return "\\\\" + path.Substring(FTPHEADER.Length);
        }
        
        
        private List<string> GetFiles(string path)
        {
        	request = (FtpWebRequest)WebRequest.Create(path);
            request.Credentials = new NetworkCredential(FTPAccountViewModel.Instance.SelectedItem.Username, FTPAccountViewModel.Instance.SelectedItem.Password);
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
           
            var files = new List<string>();
            string[] list = null;
            using  (FtpWebResponse  response = (FtpWebResponse)request.GetResponse())
            using (var reader = new StreamReader(response.GetResponseStream()))
                {
					  list = reader.ReadToEnd().Split(new string[] { "\r\n"},StringSplitOptions.RemoveEmptyEntries);
            }
            
            foreach(string line in list)
            {
            	if (!line.Substring(24,5).Equals("<dir>", StringComparison.InvariantCultureIgnoreCase))
            		files.Add(line.Substring(39));
            }
            
	             
            return files;
        }
        
        //
        // Connects to the server and returns a list of directories located at the
        // given path.
        //
        private List<string> GetDirectories(string path)
        {
        	// Is Selection a file?
        	
            request = (FtpWebRequest)WebRequest.Create(path);
            request.Credentials = new NetworkCredential(FTPAccountViewModel.Instance.SelectedItem.Username, FTPAccountViewModel.Instance.SelectedItem.Password);
            request.Method = WebRequestMethods.Ftp.ListDirectory;

            var dir = new List<string>();

            try
            {
                var response = (FtpWebResponse)request.GetResponse();
                using (var reader = new StreamReader(response.GetResponseStream()))
                {

	                var line = reader.ReadLine();
	                while (line != null)
	                {
	                    string newPath = ParseLine(line, path);
	
	                    if (newPath != "ERROR")	                   
	                        dir.Add(newPath);

	                    line = reader.ReadLine();
	                }
                }
            }
            catch (WebException e)
            {
            	Console.WriteLine(e.ToString());
            }
            return dir;
        }

        private void ConnectClick(object sender, RoutedEventArgs e)
        {
        	
            SetupFTP();
        }       
		void ShowAccounts(object sender, RoutedEventArgs e)
		{
			var cf = new ConnectionForm();
			cf.DataContext=this.DataContext;
			cf.ShowDialog();
		}

		void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			FTPAccountViewModel.Instance.SerializeAccounts();
		}
	}
}