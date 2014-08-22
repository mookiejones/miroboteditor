/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 9/22/2012
 * Time: 12:39 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace miRobotEditor.Interfaces
{
    /// <summary>
    ///     Description of ImageElementGenerator.
    /// </summary>
    public class ImageElementGenerator : VisualLineElementGenerator
    {
        private static readonly Regex ImageRegex =
            new Regex(@"<img src=""([\.\/\w\d]+)""/?>",
                RegexOptions.IgnoreCase);

        private readonly string _basePath;

        public ImageElementGenerator(string basePath)
        {
            if (basePath == null)
                throw new ArgumentNullException("basePath");
            _basePath = basePath;
        }

        /// Gets the first offset >= startOffset where the generator wants to construct
        /// an element.
        /// Return -1 to signal no interest.
        private Match FindMatch(int startOffset)
        {
            // fetch the end offset of the VisualLine being generated
            int endOffset = CurrentContext.VisualLine.LastDocumentLine.EndOffset;
            TextDocument document = CurrentContext.Document;
            string relevantText = document.GetText(startOffset, endOffset - startOffset);
            return ImageRegex.Match(relevantText);
        }

        public override int GetFirstInterestedOffset(int startOffset)
        {
            Match m = FindMatch(startOffset);
            return m.Success ? (startOffset + m.Index) : -1;
        }

        /// Constructs an element at the specified offset.
        /// May return null if no element should be constructed.
        public override VisualLineElement ConstructElement(int offset)
        {
            Match m = FindMatch(offset);
            // check whether there's a match exactly at offset
            if (m.Success && m.Index == 0)
            {
                BitmapImage bitmap = LoadBitmap(m.Groups[1].Value);
                if (bitmap != null)
                {
                    var image = new Image {Source = bitmap, Width = bitmap.PixelWidth, Height = bitmap.PixelHeight};
                    // Pass the length of the match to the 'documentLength' parameter
                    // of InlineObjectElement.
                    return new InlineObjectElement(m.Length, image);
                }
            }
            return null;
        }

        private BitmapImage LoadBitmap(string fileName)
        {
            // TODO: add some kind of cache to avoid reloading the image whenever the
            // VisualLine is reconstructed
            try
            {
                string fullFileName = Path.Combine(_basePath, fileName);
                if (File.Exists(fullFileName))
                {
                    var bitmap = new BitmapImage(new Uri(fullFileName));
                    bitmap.Freeze();
                    return bitmap;
                }
            }
            catch (ArgumentException)
            {
                // invalid filename syntax
            }
            catch (IOException)
            {
                // other IO error
            }
            return null;
        }
    }
}