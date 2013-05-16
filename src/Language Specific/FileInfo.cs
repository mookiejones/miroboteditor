/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 11/16/2012
 * Time: 07:37
 * 
 */

using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using System.IO;
using miRobotEditor.Classes;
using miRobotEditor.ViewModel;
namespace miRobotEditor.Pads
{
  /// <summary>
    /// Wrapper for fileinfo for kuka robot
    /// </summary>
    public class FileInfo :ViewModelBase
    {
        private readonly System.IO.FileInfo _fi;
      private readonly FileType _filetype = FileType.NONE;
        private string _comment = string.Empty;
        public string Comment
        {
            get { return _comment; }
            set { _comment = value; RaisePropertyChanged("Comment"); }
        }
        public string DirectoryName
        {
            get { return _fi.DirectoryName; }
        }
        
        public DirectoryInfo Directory
        {
            get { return _fi.Directory; }
        }
       
       

        /// <summary>
        /// Gets or Sets a value that determines if the current file is read only.
        /// </summary>
        /// <returns>true if the current file is read only; otherwise, false</returns>
        public bool IsReadOnly
        {
            get { return _fi.IsReadOnly; }
            set { _fi.IsReadOnly = value; RaisePropertyChanged("IsReadOnly");} 
        }
       
     
       /// <summary>
       ///  Gets the size, in bytes, of the current file.
       /// </summary>
       /// <returns>The size of the current file in bytes.</returns>
       /// <exception cref="IOException"></exception>
        public long Length
        {
            get { return _fi.Length; }
        }
        
        /// <summary>
        /// Wrapper for Length
        /// </summary>
        public string Size
        {
        	get{return Length.ToString(CultureInfo.InvariantCulture);}
        }
        
       
        /// <summary>
        /// Gets the name of the file
        /// </summary>
        ///<returns>The name of the file.</returns>
        public string Name
        {
            get { return _fi.Name; }
        }

        
        /// <summary>
        /// Creates a System.IO.StreamWriter that appends text to the file represented
        /// by this instance of teh System.IO.FileInfo
        /// </summary>
        /// <returns>A New StreamWriter</returns>
        public StreamWriter AppendText()
        {
            return _fi.AppendText();
        }
        //
        // Summary:
        //     Copies an existing file to a new file, disallowing the overwriting of an
        //     existing file.
        //
        // Parameters:
        //   destFileName:
        //     The name of the new file to copy to.
        //
        // Returns:
        //     A new file with a fully qualified path.
        //
        // Exceptions:
        //   System.ArgumentException:
        //     destFileName is empty, contains only white spaces, or contains invalid characters.
        //
        //   System.IO.IOException:
        //     An error occurs, or the destination file already exists.
        //
        //   System.Security.SecurityException:
        //     The caller does not have the required permission.
        //
        //   System.ArgumentNullException:
        //     destFileName is null.
        //
        //   System.UnauthorizedAccessException:
        //     A directory path is passed in, or the file is being moved to a different
        //     drive.
        //
        //   System.IO.DirectoryNotFoundException:
        //     The directory specified in destFileName does not exist.
        //
        //   System.IO.PathTooLongException:
        //     The specified path, file name, or both exceed the system-defined maximum
        //     length. For example, on Windows-based platforms, paths must be less than
        //     248 characters, and file names must be less than 260 characters.
        //
        //   System.NotSupportedException:
        //     destFileName contains a colon (:) within the string but does not specify
        //     the volume.
        public System.IO.FileInfo CopyTo(string destFileName)
        {
            return _fi.CopyTo(destFileName);
        }
        
        
        
        
        
        /// <summary>
        /// Copies an existing file to a new file, allowing the overwriting of an existing file.
        /// </summary>
        /// <param name="destFileName">The name of the new file to copy to.</param>
        /// <param name="overwrite">true to allow an existing file to be overwritten; otherwise, false.</param>
        /// <returns> A new file, or an overwrite of an existing file if overwrite is true. If
        ///  the file exists and overwrite is false, an System.IO.IOException is thrown.</returns>
        public System.IO.FileInfo CopyTo(string destFileName, bool overwrite)
        {
            return _fi.CopyTo(destFileName, overwrite);
        }
        
        
        /// <summary>
        /// Creates a file
        /// </summary>
        /// <returns>A new file</returns>
        public FileStream Create()
        {
            return _fi.Create();
        }

        
        /// <summary>
        /// Creates a System.IO.StreamWriter that writes a new text file.
        /// </summary>
        /// <returns>A new StreamWriter</returns>
        public StreamWriter CreateText()
        {
            return _fi.CreateText();
        }
        
       
        
        /// <summary>
        ///   Decrypts a file that was encrypted by the current account using the System.IO.FileInfo.Encrypt() method.
        /// </summary>
        [ComVisible(false)]
        public void Decrypt()
        {
            _fi.Decrypt();
        }
        
    
        /// <summary>
        /// Permanently deletes a file.
        /// </summary>
        [SecuritySafeCritical]
        public  void Delete()
        {
            _fi.Delete();
        }
                
        /// <summary>
        ///  Encrypts a file so that only the account used to encrypt the file can decrypt it.
        /// </summary>
        [ComVisible(false)]
        public void Encrypt()
        {
            _fi.Encrypt();
        }
        
       
        /// <summary>
        /// Gets a System.Security.AccessControl.FileSecurity object that encapsulates
        /// the access control list (ACL) entries for the file described by the current
        /// System.IO.FileInfo object.</summary>
        /// <returns>A System.Security.AccessControl.FileSecurity object that encapsulates the
        /// access control rules for the current file.</returns>
        public FileSecurity GetAccessControl()
        {
            return _fi.GetAccessControl();
        }
        
      
        
        /// <summary>
        /// Gets a System.Security.AccessControl.FileSecurity object that encapsulates
		/// the specified type of access control list (ACL) entries for the file described
		/// by the current System.IO.FileInfo object.
        /// </summary>
        /// <param name="includeSections">One of the System.Security.AccessControl.AccessControlSections values that specifies which group of access control entries to retrieve. </param>
        /// <returns>A System.Security.AccessControl.FileSecurity object that encapsulates the access control rules for the current file.</returns>
        public FileSecurity GetAccessControl(AccessControlSections includeSections)
        {
            return _fi.GetAccessControl(includeSections);
        }
        
        
        /// <summary>
        /// Moves a specified file to a new location, providing the option to specify a new file name.
        /// </summary>
        /// <param name="destFileName">The path to move the file to, which can specify a different file name.</param>
        [SecuritySafeCritical]
        public void MoveTo(string destFileName)
        {
            _fi.MoveTo(destFileName);
        }
        
        //
        // Summary:
        //     Opens a file in the specified mode.
        //
        // Parameters:
        //   mode:
        //     A System.IO.FileMode constant specifying the mode (for example, Open or Append)
        //     in which to open the file.
        //
        // Returns:
        //     A file opened in the specified mode, with read/write access and unshared.
        //
        // Exceptions:
        //   System.IO.FileNotFoundException:
        //     The file is not found.
        //
        //   System.UnauthorizedAccessException:
        //     The file is read-only or is a directory.
        //
        //   System.IO.DirectoryNotFoundException:
        //     The specified path is invalid, such as being on an unmapped drive.
        //
        //   System.IO.IOException:
        //     The file is already open.
        public FileStream Open(FileMode mode)
        {
            return _fi.Open(mode);
        }
        //
        // Summary:
        //     Opens a file in the specified mode with read, write, or read/write access.
        //
        // Parameters:
        //   mode:
        //     A System.IO.FileMode constant specifying the mode (for example, Open or Append)
        //     in which to open the file.
        //
        //   access:
        //     A System.IO.FileAccess constant specifying whether to open the file with
        //     Read, Write, or ReadWrite file access.
        //
        // Returns:
        //     A System.IO.FileStream object opened in the specified mode and access, and
        //     unshared.
        //
        // Exceptions:
        //   System.Security.SecurityException:
        //     The caller does not have the required permission.
        //
        //   System.ArgumentException:
        //     path is empty or contains only white spaces.
        //
        //   System.IO.FileNotFoundException:
        //     The file is not found.
        //
        //   System.ArgumentNullException:
        //     One or more arguments is null.
        //
        //   System.UnauthorizedAccessException:
        //     path is read-only or is a directory.
        //
        //   System.IO.DirectoryNotFoundException:
        //     The specified path is invalid, such as being on an unmapped drive.
        //
        //   System.IO.IOException:
        //     The file is already open.
        public FileStream Open(FileMode mode, FileAccess access)
        {
            return _fi.Open(mode, access);
        }
        //
        // Summary:
        //     Opens a file in the specified mode with read, write, or read/write access
        //     and the specified sharing option.
        //
        // Parameters:
        //   mode:
        //     A System.IO.FileMode constant specifying the mode (for example, Open or Append)
        //     in which to open the file.
        //
        //   access:
        //     A System.IO.FileAccess constant specifying whether to open the file with
        //     Read, Write, or ReadWrite file access.
        //
        //   share:
        //     A System.IO.FileShare constant specifying the type of access other FileStream
        //     objects have to this file.
        //
        // Returns:
        //     A System.IO.FileStream object opened with the specified mode, access, and
        //     sharing options.
        //
        // Exceptions:
        //   System.Security.SecurityException:
        //     The caller does not have the required permission.
        //
        //   System.ArgumentException:
        //     path is empty or contains only white spaces.
        //
        //   System.IO.FileNotFoundException:
        //     The file is not found.
        //
        //   System.ArgumentNullException:
        //     One or more arguments is null.
        //
        //   System.UnauthorizedAccessException:
        //     path is read-only or is a directory.
        //
        //   System.IO.DirectoryNotFoundException:
        //     The specified path is invalid, such as being on an unmapped drive.
        //
        //   System.IO.IOException:
        //     The file is already open.
        public FileStream Open(FileMode mode, FileAccess access, FileShare share)
        {
            return Open(mode, access, share);
        }
        //
        // Summary:
        //     Creates a read-only System.IO.FileStream.
        //
        // Returns:
        //     A new read-only System.IO.FileStream object.
        //
        // Exceptions:
        //   System.UnauthorizedAccessException:
        //     path is read-only or is a directory.
        //
        //   System.IO.DirectoryNotFoundException:
        //     The specified path is invalid, such as being on an unmapped drive.
        //
        //   System.IO.IOException:
        //     The file is already open.
        public FileStream OpenRead()
        {
            return _fi.OpenRead();
        }
        //
        // Summary:
        //     Creates a System.IO.StreamReader with UTF8 encoding that reads from an existing
        //     text file.
        //
        // Returns:
        //     A new StreamReader with UTF8 encoding.
        //
        // Exceptions:
        //   System.Security.SecurityException:
        //     The caller does not have the required permission.
        //
        //   System.IO.FileNotFoundException:
        //     The file is not found.
        //
        //   System.UnauthorizedAccessException:
        //     path is read-only or is a directory.
        //
        //   System.IO.DirectoryNotFoundException:
        //     The specified path is invalid, such as being on an unmapped drive.
        [SecuritySafeCritical]
        public StreamReader OpenText()
        {
            return _fi.OpenText();
        }
        //
        // Summary:
        //     Creates a write-only System.IO.FileStream.
        //
        // Returns:
        //     A write-only unshared System.IO.FileStream object for a new or existing file.
        //
        // Exceptions:
        //   System.UnauthorizedAccessException:
        //     The path specified when creating an instance of the System.IO.FileInfo object
        //     is read-only or is a directory.
        //
        //   System.IO.DirectoryNotFoundException:
        //     The path specified when creating an instance of the System.IO.FileInfo object
        //     is invalid, such as being on an unmapped drive.
        public FileStream OpenWrite()
        {
            return _fi.OpenWrite();
        }
        //
        // Summary:
        //     Replaces the contents of a specified file with the file described by the
        //     current System.IO.FileInfo object, deleting the original file, and creating
        //     a backup of the replaced file.
        //
        // Parameters:
        //   destinationFileName:
        //     The name of a file to replace with the current file.
        //
        //   destinationBackupFileName:
        //     The name of a file with which to create a backup of the file described by
        //     the destFileName parameter.
        //
        // Returns:
        //     A System.IO.FileInfo object that encapsulates information about the file
        //     described by the destFileName parameter.
        //
        // Exceptions:
        //   System.ArgumentException:
        //     The path described by the destFileName parameter was not of a legal form.-or-The
        //     path described by the destBackupFileName parameter was not of a legal form.
        //
        //   System.ArgumentNullException:
        //     The destFileName parameter is null.
        //
        //   System.IO.FileNotFoundException:
        //     The file described by the current System.IO.FileInfo object could not be
        //     found.-or-The file described by the destinationFileName parameter could not
        //     be found.
        //
        //   System.PlatformNotSupportedException:
        //     The current operating system is not Microsoft Windows NT or later.
        [ComVisible(false)]
        public System.IO.FileInfo Replace(string destinationFileName, string destinationBackupFileName)
        {
            return _fi.Replace(destinationFileName, destinationBackupFileName);
        }
        //
        // Summary:
        //     Replaces the contents of a specified file with the file described by the
        //     current System.IO.FileInfo object, deleting the original file, and creating
        //     a backup of the replaced file. Also specifies whether to ignore merge errors.
        //
        // Parameters:
        //   destinationFileName:
        //     The name of a file to replace with the current file.
        //
        //   destinationBackupFileName:
        //     The name of a file with which to create a backup of the file described by
        //     the destFileName parameter.
        //
        //   ignoreMetadataErrors:
        //     true to ignore merge errors (such as attributes and ACLs) from the replaced
        //     file to the replacement file; otherwise false.
        //
        // Returns:
        //     A System.IO.FileInfo object that encapsulates information about the file
        //     described by the destFileName parameter.
        //
        // Exceptions:
        //   System.ArgumentException:
        //     The path described by the destFileName parameter was not of a legal form.-or-The
        //     path described by the destBackupFileName parameter was not of a legal form.
        //
        //   System.ArgumentNullException:
        //     The destFileName parameter is null.
        //
        //   System.IO.FileNotFoundException:
        //     The file described by the current System.IO.FileInfo object could not be
        //     found.-or-The file described by the destinationFileName parameter could not
        //     be found.
        //
        //   System.PlatformNotSupportedException:
        //     The current operating system is not Microsoft Windows NT or later.
        [ComVisible(false)]
        public System.IO.FileInfo Replace(string destinationFileName, string destinationBackupFileName, bool ignoreMetadataErrors)
        {
            return _fi.Replace(destinationFileName, destinationBackupFileName, ignoreMetadataErrors);
        }

        
        //
        // Summary:
        //     Applies access control list (ACL) entries described by a System.Security.AccessControl.FileSecurity
        //     object to the file described by the current System.IO.FileInfo object.
        //
        // Parameters:
        //   fileSecurity:
        //     A System.Security.AccessControl.FileSecurity object that describes an access
        //     control list (ACL) entry to apply to the current file.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     The fileSecurity parameter is null.
        //
        //   System.SystemException:
        //     The file could not be found or modified.
        //
        //   System.UnauthorizedAccessException:
        //     The current process does not have access to open the file.
        //
        //   System.PlatformNotSupportedException:
        //     The current operating system is not Microsoft Windows 2000 or later.
        public void SetAccessControl(FileSecurity fileSecurity)
        {
            _fi.SetAccessControl(fileSecurity);
        }


        //
        // Summary:
        //     Returns the path as a string.
        //
        // Returns:
        //     A string representing the path.
        public override string ToString()
        {
        	
            System.Windows.MessageBox.Show(_fi.ToString(), "fi (FileInfo).ToString()");
            return _fi.ToString();
        }


        public string Filetype
        {
            get { return _filetype.ToString(); }
        }

      public bool Visible { get; set; }

      public FileInfo(String path)
        {
            _fi = new System.IO.FileInfo(path);
            _filetype = GetFileType();
        }

        private FileType GetFileType()
        {
            //If File doesnt have an extension return as None
            if (String.IsNullOrEmpty(_fi.Extension)) return FileType.NONE;
            try
            {
                switch (_fi.Extension.Substring(1))
                {
                    case "src":
                        return FileType.SRC;
                    case "dat":
                        return FileType.DAT;
                    case "ini":
                        return FileType.INI;
                    case "kfd":
                        return FileType.KFD;                   
                    case "sps":
                        return FileType.SPS;
                    default:
                        return FileType.NONE;
                }
            }
            catch (Exception ex)
            {
                MessageViewModel.AddError("FileInfo",ex);
              }

            return FileType.NONE ;
        }

            enum FileType{SRC,DAT,INI,KFD,SPS,NONE};
    }
}
