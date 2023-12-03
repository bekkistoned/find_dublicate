using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Program
{
    static void Main()
    {
        Console.Write("Enter the path to the folder: ");
        string folderPath = Console.ReadLine();

        if (Directory.Exists(folderPath))
        {
            Dictionary<string, List<string>> filesBySize = GetFilesBySize(folderPath);

            foreach (var filesGroup in filesBySize.Values.Where(group => group.Count > 1))
            {
                Console.WriteLine("Duplicates found by size:");
                foreach (var filePath in filesGroup)
                {
                    Console.WriteLine(filePath);
                }

                Console.WriteLine();
            }

            Dictionary<string, List<string>> filesByHash = GetFilesByHash(filesBySize);

            foreach (var filesGroup in filesByHash.Values.Where(group => group.Count > 1))
            {
                Console.WriteLine("Exact duplicates found:");
                foreach (var filePath in filesGroup)
                {
                    Console.WriteLine(filePath);
                }

                Console.WriteLine();
            }
        }
        else
        {
            Console.WriteLine("The specified folder does not exist.");
        }

        Console.WriteLine("Press Enter to exit.");
        Console.ReadLine();
    }

    static Dictionary<string, List<string>> GetFilesBySize(string folderPath)
    {
        var filesBySize = new Dictionary<string, List<string>>();

        foreach (string filePath in Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories))
        {
            long fileSize = new FileInfo(filePath).Length;

            if (!filesBySize.TryGetValue(fileSize.ToString(), out var files))
            {
                files = new List<string>();
                filesBySize[fileSize.ToString()] = files;
            }

            files.Add(filePath);
        }

        return filesBySize;
    }

    static Dictionary<string, List<string>> GetFilesByHash(Dictionary<string, List<string>> filesBySize)
    {
        var filesByHash = new Dictionary<string, List<string>>();

        foreach (var filesGroup in filesBySize.Values)
        {
            foreach (var filePath in filesGroup)
            {
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    using (var md5 = System.Security.Cryptography.MD5.Create())
                    {
                        var hash = md5.ComputeHash(stream);
                        var hashString = BitConverter.ToString(hash).Replace("-", "").ToLower();

                        if (!filesByHash.TryGetValue(hashString, out var files))
                        {
                            files = new List<string>();
                            filesByHash[hashString] = files;
                        }

                        files.Add(filePath);
                    }
                }
            }
        }

        return filesByHash;
    }
}
