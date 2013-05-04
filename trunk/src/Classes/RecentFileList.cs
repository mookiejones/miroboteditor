﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Xml;
using System.Diagnostics;
using System.Text;
using System.Reflection;

namespace miRobotEditor.Classes
{
    [Localizable(false)]
    public class RecentFileList : Separator
	{

        public static RecentFileList Instance { get; private set; }

		public interface IPersist
		{
			List<string> RecentFiles( int max );
			void InsertFile( string filepath, int max );
			void RemoveFile( string filepath, int max );
		}

	    private IPersist Persister { get; set; }

		public void UseRegistryPersister() { Persister = new RegistryPersister(); }
		public void UseRegistryPersister( string key ) { Persister = new RegistryPersister( key ); }

		public void UseXmlPersister() { Persister = new XmlPersister(); }
		public void UseXmlPersister( string filepath ) { Persister = new XmlPersister( filepath ); }
		public void UseXmlPersister( Stream stream ) { Persister = new XmlPersister( stream ); }

	    private int MaxNumberOfFiles { get; set; }
	    private int MaxPathLength { get; set; }
	    private MenuItem FileMenu { get; set; }

		/// <summary>
		/// Used in: String.Format( MenuItemFormat, index, filepath, displayPath );
		/// Default = "_{0}:  {2}"
		/// </summary>
		private string MenuItemFormatOneToNine { get; set; }

		/// <summary>
		/// Used in: String.Format( MenuItemFormat, index, filepath, displayPath );
		/// Default = "{0}:  {2}"
		/// </summary>
		public string MenuItemFormatTenPlus { get; set; }

		public delegate string GetMenuItemTextDelegate( int index, string filepath );
		public GetMenuItemTextDelegate GetMenuItemTextHandler { get; set; }

		public event EventHandler<MenuClickEventArgs> MenuClick;

		Separator _separator;
		List<RecentFile> _recentFiles;

		public RecentFileList()
		{
			Persister = new RegistryPersister();

			MaxNumberOfFiles = 9;
			MaxPathLength = 50;
			MenuItemFormatOneToNine = "_{0}:  {2}";
			MenuItemFormatTenPlus = "{0}:  {2}";

			Loaded += ( s, e ) => HookFileMenu();
		    Instance = this;
		}

		void HookFileMenu()
		{
		    var parent = Parent as MenuItem;

			if ( parent == null ) throw new ApplicationException( "Parent must be a MenuItem" );

			if (Equals(FileMenu, parent)) return;

			if ( FileMenu != null ) FileMenu.SubmenuOpened -= FileMenuSubmenuOpened;

			FileMenu = parent;
			FileMenu.SubmenuOpened += FileMenuSubmenuOpened;
		}

		public List<string> RecentFiles { get { return Persister.RecentFiles( MaxNumberOfFiles ); } }
		public void RemoveFile( string filepath ) { Persister.RemoveFile( filepath, MaxNumberOfFiles ); }
		public void InsertFile( string filepath ) { Persister.InsertFile( filepath, MaxNumberOfFiles ); }

		void FileMenuSubmenuOpened( object sender, RoutedEventArgs e )
		{
			SetMenuItems();
		}

		void SetMenuItems()
		{
			RemoveMenuItems();

			LoadRecentFiles();

			InsertMenuItems();
		}

		void RemoveMenuItems()
		{
			if ( _separator != null ) FileMenu.Items.Remove( _separator );

			if ( _recentFiles != null )
				foreach ( var r in _recentFiles )
					if ( r.MenuItem != null )
						FileMenu.Items.Remove( r.MenuItem );

			_separator = null;
			_recentFiles = null;
		}

		void InsertMenuItems()
		{
			if ( _recentFiles == null ) return;
			if ( _recentFiles.Count == 0 ) return;

			var iMenuItem = FileMenu.Items.IndexOf( this );
			foreach ( var r in _recentFiles )
			{
				var header = GetMenuItemText( r.Number + 1, r.Filepath, r.DisplayPath );
			    r.MenuItem = new MenuItem {Header = header};
//				r.MenuItem = new MenuItem { Header = header };
				r.MenuItem.Click += MenuItemClick;

				FileMenu.Items.Insert( ++iMenuItem, r.MenuItem );
			}

			_separator = new Separator();
			FileMenu.Items.Insert( ++iMenuItem, _separator );
		}

		string GetMenuItemText( int index, string filepath, string displaypath )
		{
			var delegateGetMenuItemText = GetMenuItemTextHandler;
			if ( delegateGetMenuItemText != null ) return delegateGetMenuItemText( index, filepath );

			var format = ( index < 10 ? MenuItemFormatOneToNine : MenuItemFormatTenPlus );

			var shortPath = ShortenPathname( displaypath, MaxPathLength );

			return String.Format( format, index, filepath, shortPath );
		}

		// This method is taken from Joe Woodbury's article at: http://www.codeproject.com/KB/cs/mrutoolstripmenu.aspx

	    /// <summary>
	    /// Shortens a pathname for display purposes.
	    /// </summary>
	    /// <param labelName="pathname">The pathname to shorten.</param>
	    /// <param labelName="maxLength">The maximum number of characters to be displayed.</param>
	    /// <param name="pathname"> </param>
	    /// <param name="maxLength"> </param>
	    /// <remarks>Shortens a pathname by either removing consecutive components of a path
	    /// and/or by removing characters from the end of the filename and replacing
	    /// then with three elipses (...)
	    /// <para>In all cases, the root of the passed path will be preserved in it's entirety.</para>
	    /// <para>If a UNC path is used or the pathname and maxLength are particularly short,
	    /// the resulting path may be longer than maxLength.</para>
	    /// <para>This method expects fully resolved pathnames to be passed to it.
	    /// (Use Path.GetFullPath() to obtain this.)</para>
	    /// </remarks>
	    /// <returns></returns>
	    private static string ShortenPathname( string pathname, int maxLength )
		{
			if ( pathname.Length <= maxLength )
				return pathname;

			var root = Path.GetPathRoot( pathname );
			if ( root != null && root.Length > 3 )
				root += Path.DirectorySeparatorChar;

	        if (root != null)
	        {
	            var elements = pathname.Substring( root.Length ).Split( Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar );

	            var filenameIndex = elements.GetLength( 0 ) - 1;

	            if ( elements.GetLength( 0 ) == 1 ) // pathname is just a root and filename
	            {
	                if ( elements[ 0 ].Length > 5 ) // long enough to shorten
	                {
	                    // if path is a UNC path, root may be rather long
	                    if ( root.Length + 6 >= maxLength )
	                    {
	                        return root + elements[ 0 ].Substring( 0, 3 ) + "...";
	                    }
	                    return pathname.Substring( 0, maxLength - 3 ) + "...";
	                }
	            }
	            else if ( ( root.Length + 4 + elements[ filenameIndex ].Length ) > maxLength ) // pathname is just a root and filename
	            {
	                root += "...\\";

	                var len = elements[ filenameIndex ].Length;
	                if ( len < 6 )
	                    return root + elements[ filenameIndex ];

	                if ( ( root.Length + 6 ) >= maxLength )
	                {
	                    len = 3;
	                }
	                else
	                {
	                    len = maxLength - root.Length - 3;
	                }
	                return root + elements[ filenameIndex ].Substring( 0, len ) + "...";
	            }
	            else if ( elements.GetLength( 0 ) == 2 )
	            {
	                return root + "...\\" + elements[ 1 ];
	            }
	            else
	            {
	                var len = 0;
	                var begin = 0;

	                for ( var i = 0 ; i < filenameIndex ; i++ )
	                {
	                    if (elements[i].Length <= len) continue;
	                    begin = i;
	                    len = elements[ i ].Length;
	                }

	                var totalLength = pathname.Length - len + 3;
	                var end = begin + 1;

	                while ( totalLength > maxLength )
	                {
	                    if ( begin > 0 )
	                        totalLength -= elements[ --begin ].Length - 1;

	                    if ( totalLength <= maxLength )
	                        break;

	                    if ( end < filenameIndex )
	                        totalLength -= elements[ ++end ].Length - 1;

	                    if ( begin == 0 && end == filenameIndex )
	                        break;
	                }

	                // assemble final string

	                for ( var i = 0 ; i < begin ; i++ )
	                {
	                    root += elements[ i ] + '\\';
	                }

	                root += "...\\";

	                for ( var i = end ; i < filenameIndex ; i++ )
	                {
	                    root += elements[ i ] + '\\';
	                }

	                return root + elements[ filenameIndex ];
	            }
	        }
	        return pathname;
		}

		void LoadRecentFiles()
		{
			_recentFiles = LoadRecentFilesCore();
		}

		List<RecentFile> LoadRecentFilesCore()
		{
			var list = RecentFiles;

			var files = new List<RecentFile>( list.Count );

			var i = 0;
		    files.AddRange(list.Select(filepath => new RecentFile(i++, filepath)));

		    return files;
		}

		private class RecentFile
		{
			public readonly int Number;
			public readonly string Filepath = "";
//			public MenuItem MenuItem;
		    public MenuItem MenuItem;
			public string DisplayPath
			{
				get
				{
                    //TODO I think i want to show the filename for this
                    var dir = Path.GetDirectoryName(Filepath);
                    string f = ViewModel.GlobalOptions.Instance.Options.FileOptions.ShowFullName?Path.GetFileName(Filepath):Path.GetFileNameWithoutExtension(Filepath);
// ReSharper disable AssignNullToNotNullAttribute
					return Path.Combine(dir, f );
// ReSharper restore AssignNullToNotNullAttribute
				}
			}

			public RecentFile( int number, string filepath )
			{
				Number = number;
				Filepath = filepath;
			}
		}

		public class MenuClickEventArgs : EventArgs
		{
			public string Filepath { get; private set; }

			public MenuClickEventArgs( string filepath )
			{
				Filepath = filepath;
			}
		}

		void MenuItemClick( object sender, EventArgs e )
		{
//			var menuItem = sender as MenuItem;
            var menuItem = sender as MenuItem;

			OnMenuClick( menuItem );
		}

		protected virtual void OnMenuClick( MenuItem menuItem )
		{
			var filepath = GetFilepath( menuItem );

            
			if ( String.IsNullOrEmpty( filepath ) ) return;

            //Check if directory exists and protect against network freezing
            if (!Global.DoesDirectoryExist(filepath))
            {
                PromptForDelete(filepath);
                return;
            }

			var dMenuClick = MenuClick;

            if (File.Exists(filepath))
                Workspace.Instance.OpenFile(filepath);
            else
                PromptForDelete(filepath);
//			if ( dMenuClick != null ) dMenuClick( menuItem, new MenuClickEventArgs( filepath ) );
		}
        
        void PromptForDelete(string filepath)
        {
            var result =
                MessageBox.Show( String.Format("{0} is not accessible. Would you like to remove from the recent file list?", filepath),
                    "File Doesnt Exist", MessageBoxButton.YesNo, MessageBoxImage.Error);
            if (result.Equals(MessageBoxResult.Yes))
                RemoveFile(filepath);
        }

		string GetFilepath( MenuItem menuItem )
		{
		    foreach (var r in _recentFiles.Where(r => r.MenuItem.Equals( menuItem )))
		        return r.Filepath;

		    return String.Empty;
		}

        //-----------------------------------------------------------------------------------------

		static class ApplicationAttributes
		{
			static readonly Assembly Assembly;

// ReSharper disable InconsistentNaming
			static readonly AssemblyTitleAttribute _Title;
			static readonly AssemblyCompanyAttribute _Company;
			static readonly AssemblyCopyrightAttribute _Copyright;
			static readonly AssemblyProductAttribute _Product;
            static readonly Version _Version;
// ReSharper restore InconsistentNaming

			public static string CompanyName { get; private set; }
			public static string ProductName { get; private set; }

// ReSharper disable UnusedAutoPropertyAccessor.Local
            private static string Copyright { get; set; }
            private static string Title { get; set; }			
		    private static string Version { get; set; }
// ReSharper restore UnusedAutoPropertyAccessor.Local

			static ApplicationAttributes()
			{
				try
				{
					Title = String.Empty;
					CompanyName = String.Empty;
					Copyright = String.Empty;
					ProductName = String.Empty;
					Version = String.Empty;

					Assembly = Assembly.GetEntryAssembly();

					if ( Assembly != null )
					{
						var attributes = Assembly.GetCustomAttributes( false );

						foreach ( var attribute in attributes )
						{
							var type = attribute.GetType();

							if ( type == typeof( AssemblyTitleAttribute ) ) _Title = ( AssemblyTitleAttribute ) attribute;
							if ( type == typeof( AssemblyCompanyAttribute ) ) _Company = ( AssemblyCompanyAttribute ) attribute;
							if ( type == typeof( AssemblyCopyrightAttribute ) ) _Copyright = ( AssemblyCopyrightAttribute ) attribute;
							if ( type == typeof( AssemblyProductAttribute ) ) _Product = ( AssemblyProductAttribute ) attribute;
						}

						_Version = Assembly.GetName().Version;
					}

					if ( _Title != null ) Title = _Title.Title;
					if ( _Company != null ) CompanyName = _Company.Company;
					if ( _Copyright != null ) Copyright = _Copyright.Copyright;
					if ( _Product != null ) ProductName = _Product.Product;
					if ( _Version != null ) Version = _Version.ToString();
				}
				catch (Exception ex)
				{
				    ViewModel.MessageViewModel.AddError(ex);
				}
			}
		}

		//-----------------------------------------------------------------------------------------

	    [Localizable(false)]
	    private class RegistryPersister : IPersist
		{
	        private string RegistryKey { get; set; }

			public RegistryPersister()
			{
				RegistryKey =
					"Software\\" +
					ApplicationAttributes.CompanyName + "\\" +
					ApplicationAttributes.ProductName + "\\" +
					"RecentFileList";
			}

			public RegistryPersister( string key )
			{
				RegistryKey = key;
			}

			string Key( int i ) { return i.ToString( "00" ); }

			public List<string> RecentFiles( int max )
			{
				var k = Registry.CurrentUser.OpenSubKey( RegistryKey ) ?? Registry.CurrentUser.CreateSubKey( RegistryKey );

			    var list = new List<string>( max );

				for ( var i = 0 ; i < max ; i++ )
				{
				    if (k == null) continue;
				    var filename = ( string ) k.GetValue( Key( i ) );

				    if ( String.IsNullOrEmpty( filename ) ) break;

				    list.Add( filename );
				}

				return list;
			}

			public void InsertFile( string filepath, int max )
			{
				var k = Registry.CurrentUser.OpenSubKey( RegistryKey );
				if ( k == null ) Registry.CurrentUser.CreateSubKey( RegistryKey );
				k = Registry.CurrentUser.OpenSubKey( RegistryKey, true );

				RemoveFile( filepath, max );

				for ( var i = max - 2 ; i >= 0 ; i-- )
				{
					var sThis = Key( i );
					var sNext = Key( i + 1 );

				    if (k != null)
				    {
				        var oThis = k.GetValue( sThis );
				        if ( oThis == null ) continue;

				        k.SetValue( sNext, oThis );
				    }
				}

			    if (k != null) k.SetValue( Key( 0 ), filepath );
			}

			public void RemoveFile( string filepath, int max )
			{
				var k = Registry.CurrentUser.OpenSubKey( RegistryKey );
				if ( k == null ) return;

				for ( var i = 0 ; i < max ; i++ )
				{
again:
					var s = ( string ) k.GetValue( Key( i ) );
					if ( s != null && s.Equals( filepath, StringComparison.CurrentCultureIgnoreCase ) )
					{
						RemoveFile( i, max );
						goto again;
					}
				}
			}

			void RemoveFile( int index, int max )
			{
				var k = Registry.CurrentUser.OpenSubKey( RegistryKey, true );
				if ( k == null ) return;

				k.DeleteValue( Key( index ), false );

				for ( var i = index ; i < max - 1 ; i++ )
				{
					var sThis = Key( i );
					var sNext = Key( i + 1 );

					var oNext = k.GetValue( sNext );
					if ( oNext == null ) break;

					k.SetValue( sThis, oNext );
					k.DeleteValue( sNext );
				}
			}
		}

		//-----------------------------------------------------------------------------------------

		private class XmlPersister : IPersist
		{
		    private string Filepath { get; set; }
		    private Stream Stream { get; set; }

			public XmlPersister()
			{
				Filepath =
					Path.Combine(
						Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData ),
						ApplicationAttributes.CompanyName + "\\" +
						ApplicationAttributes.ProductName + "\\" +
						"RecentFileList.xml" );
			}

			public XmlPersister( string filepath )
			{
				Filepath = filepath;
			}

			public XmlPersister( Stream stream )
			{
				Stream = stream;
			}

			public List<string> RecentFiles( int max )
			{
				return Load( max );
			}

			public void InsertFile( string filepath, int max )
			{
				Update( filepath, true, max );
			}

			public void RemoveFile( string filepath, int max )
			{
				Update( filepath, false, max );
			}

			void Update( string filepath, bool insert, int max )
			{
				var old = Load( max );

				var list = new List<string>( old.Count + 1 );

				if ( insert ) list.Add( filepath );

				CopyExcluding( old, filepath, list, max );

				Save( list );
			}

			void CopyExcluding( IEnumerable<string> source, string exclude, List<string> target, int max )
			{
			    foreach (var s in from s in source where !String.IsNullOrEmpty( s ) where !s.Equals( exclude, StringComparison.OrdinalIgnoreCase ) where target.Count < max select s)
			        target.Add( s );
			}

		    class SmartStream : IDisposable
			{
			    readonly bool _isStreamOwned = true;
				Stream _stream;

				public Stream Stream { get { return _stream; } }

				public static implicit operator Stream( SmartStream me ) { return me.Stream; }

				public SmartStream( string filepath, FileMode mode )
				{
					_isStreamOwned = true;

				    var dir = Path.GetDirectoryName(filepath);
// ReSharper disable AssignNullToNotNullAttribute
					Directory.CreateDirectory(dir);
// ReSharper restore AssignNullToNotNullAttribute

					_stream = File.Open( filepath, mode );
				}

				public SmartStream( Stream stream )
				{
					_isStreamOwned = false;
					_stream = stream;
				}

				public void Dispose()
				{
					if ( _isStreamOwned && _stream != null ) _stream.Dispose();

					_stream = null;
				}
			}

			SmartStream OpenStream( FileMode mode )
			{
			    if ( !String.IsNullOrEmpty( Filepath ) )
				{
					return new SmartStream( Filepath, mode );
				}
			    return new SmartStream( Stream );
			}

		    List<string> Load( int max )
			{
				var list = new List<string>( max );

				using ( var ms = new MemoryStream() )
				{
					using ( var ss = OpenStream( FileMode.OpenOrCreate ) )
					{
						if ( ss.Stream.Length == 0 ) return list;

						ss.Stream.Position = 0;

						var buffer = new byte[ 1 << 20 ];
						for ( ; ; )
						{
							var bytes = ss.Stream.Read( buffer, 0, buffer.Length );
							if ( bytes == 0 ) break;
							ms.Write( buffer, 0, bytes );
						}

						ms.Position = 0;
					}

					XmlTextReader x = null;

					try
					{
						x = new XmlTextReader( ms );

						while ( x.Read() )
						{
							switch ( x.NodeType )
							{
								case XmlNodeType.XmlDeclaration:
								case XmlNodeType.Whitespace:
									break;

								case XmlNodeType.Element:
									switch ( x.Name )
									{
										case "RecentFiles": break;

										case "RecentFile":
											if ( list.Count < max ) list.Add( x.GetAttribute( 0 ) );
											break;

										default: Debug.Assert( false ); break;
									}
									break;

								case XmlNodeType.EndElement:
									switch ( x.Name )
									{
										case "RecentFiles": return list;
										default: Debug.Assert( false ); break;
									}
									break;

								default:
									Debug.Assert( false );
									break;
							}
						}
					}
					finally
					{
						if ( x != null ) x.Close();
					}
				}
				return list;
			}

			void Save( IEnumerable<string> list )
			{
				using ( var ms = new MemoryStream() )
				{
					XmlTextWriter x = null;

					try
					{
						x = new XmlTextWriter( ms, Encoding.UTF8 ) {Formatting = Formatting.Indented};

					    x.WriteStartDocument();

						x.WriteStartElement( "RecentFiles" );

						foreach ( var filepath in list )
						{
							x.WriteStartElement( "RecentFile" );
							x.WriteAttributeString( "Filepath", filepath );
							x.WriteEndElement();
						}

						x.WriteEndElement();

						x.WriteEndDocument();

						x.Flush();

						using ( var ss = OpenStream( FileMode.Create ) )
						{
							ss.Stream.SetLength( 0 );

							ms.Position = 0;

							var buffer = new byte[ 1 << 20 ];
							for ( ; ; )
							{
								var bytes = ms.Read( buffer, 0, buffer.Length );
								if ( bytes == 0 ) break;
								ss.Stream.Write( buffer, 0, bytes );
							}
						}
					}
					finally
					{
						if ( x != null ) x.Close();
					}
				}
			}
		}

		//-----------------------------------------------------------------------------------------

	}
}