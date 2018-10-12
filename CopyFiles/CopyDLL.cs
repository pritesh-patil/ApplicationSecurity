using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Diagnostics;
using Approva.QA.Common;

namespace CopyFiles
{
    public class CopyDLL
    {
        public static void _CopyFromLocation(string _sourcePath, string _targetPath, string _fileName)
        {
            try
            {
                string sourceFile = System.IO.Path.Combine(_sourcePath, _fileName);
                string destFile = System.IO.Path.Combine(_targetPath, _fileName);

                // To copy a folder's contents to a new location:
                // Create a new target folder, if necessary.

                //if (!System.IO.Directory.Exists(targetPath))
                //{
                //    System.IO.Directory.CreateDirectory(targetPath);
                //}

                if (System.IO.File.Exists(destFile))
                {
                    _backupFiles(_targetPath, _fileName);
                }
                // To copy a file to another location and 
                // overwrite the destination file if it already exists.
                System.IO.File.Copy(sourceFile, destFile, true);

                // To copy all the files in one directory to another directory.
                // Get the files in the source folder. (To recursively iterate through
                // all subfolders under the current directory, see
                // "How to: Iterate Through a Directory Tree.")
                // Note: Check for target path was performed previously
                //       in this code example.
                //if (System.IO.Directory.Exists(sourcePath))
                //{
                //    string[] files = System.IO.Directory.GetFiles(sourcePath);

                //    // Copy the files and overwrite destination files if they already exist.
                //    foreach (string s in files)
                //    {
                //        // Use static Path methods to extract only the file name from the path.
                //        fileName = System.IO.Path.GetFileName(s);
                //        destFile = System.IO.Path.Combine(targetPath, fileName);
                //        System.IO.File.Copy(s, destFile, true);
                //    }
                //}
                //else
                //{
                //    Console.WriteLine("Source path does not exist!");
                //}

            }
            catch (Exception ex)
            {
                Logger.WriteMessage("Failed while copying "+ _fileName);
                Logger.WriteError(ex);
            }
        }

        public static bool _backupFiles(string _sourcePath, string _fileName)
        {
            bool success = false;
            // Create a new target folder, if necessary.
            string _targetPath = AppDomain.CurrentDomain.BaseDirectory + "backUp";
            try
            {
                if (!System.IO.Directory.Exists(_targetPath))
                {
                    System.IO.Directory.CreateDirectory(_targetPath);
                }
                string sourceFile = System.IO.Path.Combine(_sourcePath, _fileName);
                string destFile = System.IO.Path.Combine(_targetPath, _fileName);

                System.IO.File.Copy(sourceFile, destFile, true);
                success = true;
            }
            catch (Exception ex)
            {
                Logger.WriteError("Error in backing up files");
                Logger.WriteError(ex.Message);
                success = false;
            }
            return success;
        }
    }
}
