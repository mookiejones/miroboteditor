using System.IO;

namespace miRobotEditor.Core
{
    /// <summary>
    /// This class defines the SharpDevelop display binding interface, it is a factory
    /// structure, which creates IViewContents.
    /// </summary>
    public interface IDisplayBinding
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        bool IsPreferredBindingForFile(string fileName);

        /// <remarks>
        /// This function determines, if this display binding is able to create
        /// an IViewContent for the file given by fileName.
        /// </remarks>
        /// <returns>
        /// true, if this display binding is able to create
        /// an IViewContent for the file given by fileName.
        /// false otherwise
        /// </returns>
        bool CanCreateContentForFile(string fileName);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileContent"></param>
        /// <param name="detectedMimeType"></param>
        /// <returns></returns>
        double AutoDetectFileContent(string fileName, Stream fileContent, string detectedMimeType);

        /// <remarks>
        /// Creates a new IViewContent object for the file fileName
        /// </remarks>
        /// <returns>
        /// A newly created IViewContent object.
        /// </returns>
        IViewContent CreateContentForFile(OpenedFile file);
    }
}
