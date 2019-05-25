using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IOWrapper
{
    public class FilesOperations
    {
        private static readonly string DuplicatedPrefix = "Duplicated";

        public static void CompareSizeOnDisk(FilePath beforeFilePath, FilePath afterFilePath)
        {
            long firstFileSizeInBytes = new FileInfo(beforeFilePath.FileFullPath).Length;
            long secondFileSizeInBytes = new FileInfo(afterFilePath.FileFullPath).Length;
            float sizeChange = GetSizeChangeInPercentage(firstFileSizeInBytes, secondFileSizeInBytes);

            string sizeComparisonReport = $"Before: {firstFileSizeInBytes} bytes.   After: {secondFileSizeInBytes} bytes. Change in size on disk: {sizeChange}%";
            Console.WriteLine(sizeComparisonReport);
        }

        private static float GetSizeChangeInPercentage(long orgSize, long modifiedSize)
        {
            float sizeDifference = modifiedSize - orgSize;
            return (sizeDifference / (float)orgSize) * 100;
        }

        public static void CopyFiles(string sourcePath, string destPath)
        {
            try
            {
                foreach (string fileName in Directory.GetFiles(sourcePath))
                {
                    CopyFile(fileName, destPath);
                }
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void DeleteFile(string fileName)
        {
            try
            {
                File.Delete(fileName);
                Console.WriteLine($"{fileName} was deleted");
            }
            catch (IOException e)
            {
                Console.WriteLine($"Could not delete {fileName}", e);
            }
        }

        public static void DeleteFilesFromGivenDirectory(string outDirectoryPath)
        {
            foreach (string filePath in Directory.GetFiles(outDirectoryPath))
            {
                DeleteFile(filePath);
            }

            foreach (string directoryName in Directory.GetDirectories(outDirectoryPath))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directoryName);
                directoryInfo.Delete(true); // True for recursive delete.
                Console.WriteLine($"{directoryName} was deleted recursively.");
            }
        }

        /// <summary>
        /// Copy given file to a directory without changing it's name.
        /// Returns the full name of the destination file.
        /// </summary>
        /// <param name="sourceFilePathStr"></param>
        /// <param name="destDirectoryPath"></param>
        /// <returns></returns>
        public static FilePath CopyFile(string sourceFilePathStr, string destDirectoryPath)
        {
            FilePath destFilePath = null;
            try
            {
                FileInfo fileInfo = new FileInfo(sourceFilePathStr);
                if (fileInfo.Attributes != FileAttributes.ReadOnly)
                {
                    using (Stream fileStreamSource = new FileStream(sourceFilePathStr, FileMode.Open))
                    {
                        FilePath sourceFilePath = FilePath.CreateFilePath(sourceFilePathStr);
                        destFilePath = FilePath.CreateFilePath(destDirectoryPath, sourceFilePath.FileName);
                        StreamOperations.SaveStreamToFile(fileStreamSource, destFilePath);
                        Console.WriteLine($"{sourceFilePath.FileName} copied");
                    }
                }
                else
                {
                    Console.WriteLine("File is Read-Only, unable to copy");
                }
            }
            catch (IOException e)
            {
                Console.WriteLine($"Copying {sourceFilePathStr} failed!!!", e);
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e);
            }

            return destFilePath;
        }

        /// <summary>
        /// Duplicating a file on the same directory. Should get new name for the file.
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public static FilePath DuplicateFile(FilePath originalFilePath)
        {
            return DuplicateFile(originalFilePath, 1);
        }

        /// <summary>
        /// Duplicating a file on the same directory. Should get new name for the file.
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public static FilePath DuplicateFile(FilePath originalFilePath, int timesToDuplicate)
        {
            FilePath destFilePath = null;
            try
            {
                using (Stream fileStreamSource = new FileStream(originalFilePath.FileFullPath, FileMode.Open))
                {
                    for (int i = 0; i < timesToDuplicate; ++i)
                    {
                        destFilePath = FilePath.CreateFilePathWithPrefix(originalFilePath.DirectoryPath, $"{DuplicatedPrefix}_{i}", originalFilePath.FileName);
                        StreamOperations.SaveStreamToFile(fileStreamSource, destFilePath);
                        Console.WriteLine($"{originalFilePath.FileName} duplicated to {destFilePath.FileName}");
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine($"Copying {originalFilePath} failed!!!", e);
            }

            return destFilePath;
        }

        public static FilePath CreateDirectory(string directoryPath, string directoryName)
        {
            return CreateDirectory(Path.Combine(directoryPath, directoryName));
        }

        /// <summary>
        /// Returns the FilePath of given <param name="dirFullPath"/>.
        /// </summary>
        public static FilePath CreateDirectory(string dirFullPath)
        {
            FilePath directoryFullPath = FilePath.CreateFilePath(dirFullPath);
            try
            {
                if (!Directory.Exists(directoryFullPath.FileFullPath))
                {
                    Directory.CreateDirectory(directoryFullPath.FileFullPath);
                    Console.WriteLine($"New directory {directoryFullPath.FileFullPath} created");
                }
                else
                {
                    Console.WriteLine($"Directory {directoryFullPath.FileFullPath} is already exists");
                }
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
                directoryFullPath = null;
            }

            return directoryFullPath;
        }

        /// <summary>
        /// Takes all the files which in <paramref name="exampleDirectory"/>, copies them from <paramref name="sourceDirectory"/>
        /// to <paramref name="sourceDirectory"/>. That Method is not recursive.
        /// </summary>
        public static void CopyFilesFromSpecificDirectory(FilePath exampleDirectory, FilePath sourceDirectory, FilePath destDirectory)
        {
            List<FilePath> filePaths = exampleDirectory.GetFiles();
            List<string> fileNames = filePaths.Select(filePath => filePath.FileName).ToList();

            Console.WriteLine($"Destination directory (directory to copy to...): {destDirectory.FileFullPath}");
            foreach (FilePath filePath in sourceDirectory.GetFiles())
            {
                if (fileNames.Contains(filePath.FileName))
                {
                    CopyFile(filePath.FileFullPath, destDirectory.FileFullPath);
                }
            }
        }

        public static void ChangeFilesNameDeleteToEnd(FilePath directoryPath, string subString)
        {
            Console.WriteLine($"Renaming files in {directoryPath.FileFullPath}. Deleteing from {subString}");

            foreach (FilePath filePath in directoryPath.GetFiles())
            {
                int indexToDeleteFrom = filePath.FileName.IndexOf(subString);
                if (indexToDeleteFrom == -1)
                    continue;

                StringBuilder stringBuilder = new StringBuilder(filePath.FileName);
                int sizeToDelete = filePath.FileName.Length - indexToDeleteFrom;
                stringBuilder.Remove(indexToDeleteFrom, sizeToDelete);
                File.Move(filePath.FileFullPath, Path.Combine(filePath.DirectoryPath, stringBuilder.ToString()));
            }

            Console.WriteLine("Finished renaming");
        }

        public static void AddPrefixToManyFiles(FilePath directoryPath, string prefix)
        {
            foreach (FilePath filePath in directoryPath.GetFiles())
            {
                FilePath newFilePath = filePath.CreateNewFilePathWithPrefix(prefix);
                File.Move(filePath.FileFullPath, newFilePath.FileFullPath);
            }
        }

        public static void AddSuffixToManyFiles(FilePath directoryPath, string suffix)
        {
            foreach (FilePath filePath in directoryPath.GetFiles())
            {
                string fileNameWithSuffix = $"{filePath.NameWithoutExtension}_{suffix}{filePath.Extension}";
                string newFilePath = Path.Combine(filePath.DirectoryPath, fileNameWithSuffix);
                File.Move(filePath.FileFullPath, newFilePath);
            }
        }
    }
}