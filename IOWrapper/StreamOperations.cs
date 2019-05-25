using System;
using System.IO;
using System.Text;

namespace IOWrapper
{
    public class StreamOperations
    {
        private static readonly int mBufferSize = 1024;

        public static void SaveStreamToFile(Stream streamToReadFrom, FilePath filePath)
        {
            using (Stream fileStream = new FileStream(filePath.FileFullPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                int bytesRead = -1;
                byte[] buffer = new byte[mBufferSize];
                streamToReadFrom.Seek(0, SeekOrigin.Begin);
                while ((bytesRead = streamToReadFrom.Read(buffer, 0, buffer.Length)) > 0)
                {
                    fileStream.Write(buffer, 0, bytesRead);
                }
            }

            Console.WriteLine($"{filePath.FileFullPath} created and stream was written");
        }

        public static void WriteTextToNewFile(string textToWrite, FilePath filePath)
        {
            using (Stream fileStream = new FileStream(filePath.FileFullPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                byte[] buffer = new UTF8Encoding(true).GetBytes(textToWrite);
                fileStream.Write(buffer, 0, buffer.Length);
            }

            Console.WriteLine($"{filePath.FileFullPath} created and text was written");
        }

        public static void WriteAllBytes(byte[] bytesToWrite, FilePath filePath)
        {
            using (Stream fileStream = new FileStream(filePath.FileFullPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                fileStream.Write(bytesToWrite, 0, bytesToWrite.Length);
            }

            Console.WriteLine($"{filePath.FileFullPath} created and byte[] was written");
        }

        public static bool IsStreamContnainsString(string stringToSearch, FilePath filePath, out long offset)
        {
            bool isStringFound = false;
            offset = -1; // TODO.

            using (StreamReader streamReader = new StreamReader(filePath.FileFullPath))
            {
                int bytesRead = -1;
                char[] buffer = new char[mBufferSize];
                while ((bytesRead = streamReader.ReadBlock(buffer, 0, buffer.Length)) > 0)
                {
                    string textBlock = new string(buffer, 0, bytesRead);
                    string textLower = textBlock.ToLower();
                    if (textLower.Contains(stringToSearch.ToLower()))
                    {
                        isStringFound = true;
                        break;
                    }
                }
            }

            return isStringFound;
        }

        public static void SetDataInSpecificPosition(string filename, int position, byte[] data)
        {
            using (Stream stream = File.Open(filename, FileMode.Open))
            {
                stream.Position = position;
                stream.Write(data, 0, data.Length);
            }
        }
    }
}