using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IOWrapper
{
    [Serializable]
    public class FilePath
    {
        public string DirectoryPath { get; private set; }
        public string FileName { get; private set; }

        public static FilePath CreateFilePathWithPrefix(string directoryPath, string prefix, string nameWithExtension)
        {
            return new FilePath(directoryPath.TrimEnd(), prefix, nameWithExtension);
        }

        public static FilePath CreateFilePath(string fullPath)
        {
            return new FilePath(fullPath);
        }

        public static FilePath CreateFilePath(string path, string name)
        {
            return new FilePath(path, name);
        }

        public string NameWithoutExtension
        {
            get
            {
                return Path.GetFileNameWithoutExtension(FileName);
            }
        }

        public string FileFullPath
        {
            get
            {
                return Path.Combine(DirectoryPath, FileName);
            }
        }

        public string Extension
        {
            get
            {
                return Path.GetExtension(FileName).ToLower();
            }
        }

        public static List<FilePath> GetFiles(string directoryPath)
        {
            List<FilePath> filePaths = new List<FilePath>();
            FileAttributes fileAttributes = File.GetAttributes(directoryPath);

            if (fileAttributes.HasFlag(FileAttributes.Directory))
            {
                string[] files = Directory.GetFiles(directoryPath);
                for (int i = 0; i < files.Length; ++i)
                {
                    filePaths.Add(CreateFilePath(files[i]));
                }
            }
            else
            {
                Console.WriteLine($"{directoryPath} is not a directory therefore does not have files");
            }

            return filePaths;
        }

        public List<FilePath> GetFiles()
        {
            return GetFiles(FileFullPath);
        }

        public static List<FilePath> GetDirectories(string directoryPath)
        {
            List<FilePath> directoriesPaths = new List<FilePath>();
            FileAttributes fileAttributes = File.GetAttributes(directoryPath);

            if (fileAttributes.HasFlag(FileAttributes.Directory))
            {
                string[] files = Directory.GetDirectories(directoryPath);
                for (int i = 0; i < files.Length; ++i)
                {
                    directoriesPaths.Add(CreateFilePath(files[i]));
                }
            }
            else
            {
                Console.WriteLine($"{directoryPath} is not a directory therefore does not contain another directories");
            }

            return directoriesPaths;
        }

        public List<FilePath> GetDirectories()
        {
            return GetDirectories(FileFullPath);
        }

        public FilePath CreateNewFilePathWithPrefix(string outputDirectory, string prefix)
        {
            return CreateFilePathWithPrefix(outputDirectory, prefix, FileName);
        }

        public FilePath CreateNewFilePathWithPrefix(string prefix)
        {
            return CreateFilePathWithPrefix(DirectoryPath, prefix, FileName);
        }

        private FilePath(string directoryPath, string prefix, string nameWithExtension)
        {
            DirectoryPath = directoryPath;
            FileName = $"{prefix}_{Path.GetFileName(nameWithExtension)}";
        }

        private FilePath(string path, string name)
        {
            DirectoryPath = path;
            try
            {
                FileName = Path.GetFileName(name);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine($"Invalid Characters in {name}", e);
                if (CheckForInvalidCharacters(name))
                {
                    name = GetNameWithoutInvalidCharacters(name);
                }
            }
        }

        private FilePath(string fullPath)
        {
            DirectoryPath = Path.GetDirectoryName(fullPath);
            FileName = Path.GetFileName(fullPath);
        }

        private bool CheckForInvalidCharacters(string name)
        {
            bool isContainInvalidCharacters = false;

            foreach (char ch in name)
            {
                if (Path.GetInvalidPathChars().Contains(ch))
                {
                    isContainInvalidCharacters = true;
                    break;
                }
            }

            return isContainInvalidCharacters;
        }

        private string GetNameWithoutInvalidCharacters(string name)
        {
            string invalidCharacters = new string(Path.GetInvalidPathChars()) + new string(Path.GetInvalidFileNameChars());
            foreach (char ch in invalidCharacters)
            {
                name = name.Replace(ch.ToString(), "");
            }

            return name;
        }

        public static implicit operator FilePath(string path)
        {
            return new FilePath(path);
        }
    }
}