﻿using System;
using System.IO;
using System.Threading;

namespace miRobotEditor.Utilities
{
    public static class FileExtended
    {

        public static FileInfo GetFileInfo(this FileInfo fi, string path)
        {
            return null;
        }

        public static bool AreEqual(string path1, string path2)
        {
            string fullName = new FileInfo(path1).FullName;
            string fullName2 = new FileInfo(path2).FullName;
            return fullName.Equals(fullName2, StringComparison.InvariantCultureIgnoreCase);
        }

        public static string CopyIfExisting(string sourcePath, string targetPath)
        {
            if (!File.Exists(sourcePath))
            {
                throw new ArgumentException("File must exist.", "sourcePath");
            }
            string text;
            if (Directory.Exists(targetPath))
            {
                text = targetPath;
                targetPath = Path.Combine(targetPath, GetName(sourcePath));
            }
            else
            {
                text = Path.GetDirectoryName(targetPath);
                if (text == null)
                {
                    throw new InvalidOperationException("Target path should not be null.");
                }
            }
            _ = Directory.CreateDirectory(text);
            File.Copy(sourcePath, targetPath, true);
            return targetPath;
        }

        public static void CopyIfExisting(string sourceDirectory, string pattern, string targetDirectory)
        {
            string[] files = Directory.GetFiles(sourceDirectory, pattern);
            for (int i = 0; i < files.Length; i++)
            {
                string sourcePath = files[i];
                _ = CopyIfExisting(sourcePath, targetDirectory);
            }
        }

        public static void DeleteIfExisting(string path)
        {
            DeleteIfExisting(path, true);
        }

        public static void DeleteIfExisting(string path, bool force)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            if (!File.Exists(path))
            {
                return;
            }
            if (force)
            {
                File.SetAttributes(path, FileAttributes.Normal);
            }
            File.Delete(path);
            for (int i = 0; i < 10; i++)
            {
                if (!File.Exists(path))
                {
                    return;
                }
                Thread.Sleep(20);
            }
        }

        public static void DeleteIfExisting(string directory, string pattern)
        {
            if (!Directory.Exists(directory))
            {
                return;
            }
            string[] files = Directory.GetFiles(directory, pattern);
            for (int i = 0; i < files.Length; i++)
            {
                string path = files[i];
                DeleteIfExisting(path);
            }
        }

        public static string GetName(string path)
        {
            string fileName = Path.GetFileName(path);
            return fileName ?? throw new InvalidOperationException("Could not acquire filename from " + path);
        }

        public static string GetNameWithoutExtension(string path)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
            return fileNameWithoutExtension ?? throw new InvalidOperationException("Could not acquire filename from " + path);
        }

        public static void MakeWriteable(string path)
        {
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                File.SetAttributes(path, FileAttributes.Normal);
            }
        }

        public static void Move(string sourcePath, string targetPath)
        {
            string directoryName = Path.GetDirectoryName(targetPath);
            if (directoryName == null)
            {
                return;
            }
            if (!Directory.Exists(directoryName))
            {
                _ = Directory.CreateDirectory(directoryName);
            }
            File.Move(sourcePath, targetPath);
        }
    }
}