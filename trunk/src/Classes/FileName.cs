using System;
using System.Text;

namespace miRobotEditor.Classes
{
    /// <summary>
    /// Represents a directory path or filename.
    /// The equality operator is overloaded to compare for path equality (case insensitive, normalizing paths with '..\')
    /// </summary>
    public sealed class FileName : IEquatable<FileName>
    {
        readonly string _normalizedFileName;

        public FileName(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException("fileName");
            if (fileName.Length == 0)
                throw new ArgumentException("The empty string is not a valid FileName");
            _normalizedFileName = NormalizePath(fileName);
        }
        /// <summary>
        /// Gets the normalized version of fileName.
        /// Slashes are replaced with backslashes, backreferences "." and ".." are 'evaluated'.
        /// </summary>
        private static string NormalizePath(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return fileName;

            int i;

            var isWeb = false;
            for (i = 0; i < fileName.Length; i++)
            {
                if (fileName[i] == '/' || fileName[i] == '\\')
                    break;
                if (fileName[i] != ':') continue;
                if (i > 1)
                    isWeb = true;
                break;
            }

            char outputSeparator = isWeb ? '/' : System.IO.Path.DirectorySeparatorChar;

            var result = new StringBuilder();
            if (isWeb == false && fileName.StartsWith(@"\\") || fileName.StartsWith("//"))
            {
                i = 2;
                result.Append(outputSeparator);
            }
            else
            {
                i = 0;
            }
            int segmentStartPos = i;
            for (; i <= fileName.Length; i++)
            {
                if (i == fileName.Length || fileName[i] == '/' || fileName[i] == '\\')
                {
                    int segmentLength = i - segmentStartPos;
                    switch (segmentLength)
                    {
                        case 0:
                            // ignore empty segment (if not in web mode)
                            // On unix, don't ignore empty segment if i==0
                            if (isWeb || (i == 0 && Environment.OSVersion.Platform == PlatformID.Unix))
                            {
                                result.Append(outputSeparator);
                            }
                            break;
                        case 1:
                            // ignore /./ segment, but append other one-letter segments
                            if (fileName[segmentStartPos] != '.')
                            {
                                if (result.Length > 0) result.Append(outputSeparator);
                                result.Append(fileName[segmentStartPos]);
                            }
                            break;
                        case 2:
                            if (fileName[segmentStartPos] == '.' && fileName[segmentStartPos + 1] == '.')
                            {
                                // remove previous segment
                                int j;
                                for (j = result.Length - 1; j >= 0 && result[j] != outputSeparator; j--)
                                {
                                }
                                if (j > 0)
                                {
                                    result.Length = j;
                                }
                                break;
                            }
                            // append normal segment
                            goto default;
                        default:
                            if (result.Length > 0) result.Append(outputSeparator);
                            result.Append(fileName, segmentStartPos, segmentLength);
                            break;
                    }
                    segmentStartPos = i + 1; // remember start position for next segment
                }
            }
            if (isWeb == false)
            {
                if (result.Length > 0 && result[result.Length - 1] == outputSeparator)
                {
                    result.Length -= 1;
                }
                if (result.Length == 2 && result[1] == ':')
                {
                    result.Append(outputSeparator);
                }
            }
            return result.ToString();
        }
        /// <summary>
        /// Creates a FileName instance from the string.
        /// It is valid to pass null or an empty string to this method (in that case, a null reference will be returned).
        /// </summary>
        public static FileName Create(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return null;
            return new FileName(fileName);
        }

        public static implicit operator string(FileName fileName)
        {
            if (fileName != null)
                return fileName._normalizedFileName;
            return null;
        }

        public override string ToString()
        {
            return _normalizedFileName;
        }

        #region Equals and GetHashCode implementation
        public override bool Equals(object obj)
        {
            return Equals(obj as FileName);
        }

        public bool Equals(FileName other)
        {
            if (other != null)
                return string.Equals(_normalizedFileName, other._normalizedFileName, StringComparison.OrdinalIgnoreCase);
            return false;
        }

        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(_normalizedFileName);
        }

        public static bool operator ==(FileName left, FileName right)
        {
            if (ReferenceEquals(left, right))
                return true;
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;
            return left.Equals(right);
        }

        public static bool operator !=(FileName left, FileName right)
        {
            return !(left == right);
        }

        [ObsoleteAttribute("Warning: comparing FileName with string results in case-sensitive comparison")]
        public static bool operator ==(FileName left, string right)
        {
            return (string)left == right;
        }

        [ObsoleteAttribute("Warning: comparing FileName with string results in case-sensitive comparison")]
        public static bool operator !=(FileName left, string right)
        {
            return (string)left != right;
        }

        [ObsoleteAttribute("Warning: comparing FileName with string results in case-sensitive comparison")]
        public static bool operator ==(string left, FileName right)
        {
            return left == (string)right;
        }

        [ObsoleteAttribute("Warning: comparing FileName with string results in case-sensitive comparison")]
        public static bool operator !=(string left, FileName right)
        {
            return left != (string)right;
        }
        #endregion
    }
}
