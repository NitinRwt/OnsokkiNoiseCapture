using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;


namespace OnsokkiNoiseCaptureDemo
{
    class Utils
    {

        public static string GetLatestFile(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                throw new DirectoryNotFoundException($"The directory '{directoryPath}' does not exist.");
            }

            var latestFile = new DirectoryInfo(directoryPath)
                                .GetFiles()
                                .OrderByDescending(f => f.CreationTime)
                                .FirstOrDefault();

            return latestFile?.FullName; // Returns full file path or null if no files exist
        }


        public static bool IsFileLocked(string filePath)
        {
            try
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    // File is not locked
                    return false;
                }
            }
            catch (IOException)
            {
                // File is locked by another process
                return true;
            }
        }



        public static void DeleteAllFiles(string directoryPath)
        {
            try
            {
                if (Directory.Exists(directoryPath))
                {
                    string[] files = Directory.GetFiles(directoryPath);
                    foreach (string file in files)
                    {
                        File.Delete(file);
                        Console.WriteLine($"Deleted: {file}");
                    }
                    Console.WriteLine("All files deleted successfully.");
                }
                else
                {
                    Console.WriteLine("Directory does not exist.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

    }
}
