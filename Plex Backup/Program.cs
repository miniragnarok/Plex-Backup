using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plex_Backup
{
    class Program
    {
        public static string registryPath = ConfigurationManager.AppSettings["PlexRegistryPath"];
        public static string pathToExportTo = ConfigurationManager.AppSettings["PathToExportTo"];
        public static string appDataDirectoryToExportFrom = string.Format(@"{0}\{1}",
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            ConfigurationManager.AppSettings["AppDataDirectoryToExportFrom"]);
        public static string registryBackupName = ConfigurationManager.AppSettings["RegistryBackupName"];
        public static string appDataBackupName = ConfigurationManager.AppSettings["AppDataBackupName"];
        public static int numberOfBackupsToKeep = int.Parse(ConfigurationManager.AppSettings["NumberOfBackupsToKeep"]);

        static void Main(string[] args)
        {
            var todaysDate = DateTime.Today.ToString("yyyy-MM-dd");

            ExportRegistrySettings(todaysDate);
            string backupFolderName = CreateBackupFolder(todaysDate);
            CopyBackupFiles(backupFolderName);
            DeleteOldFiles();
            DeleteOldFolders();

            Console.WriteLine("Program complete");
        }

        private static void ExportRegistrySettings(string todaysDate)
        {
            Console.WriteLine("Exporting registry directory");
            var newFileName = string.Format("{0} {1}.reg", registryBackupName, todaysDate);
            RegistryService.ExportDirectory(registryPath, pathToExportTo, newFileName);
        }

        private static string CreateBackupFolder(string todaysDate)
        {
            Console.WriteLine("Creating backup folder");
            var backupFolderName = string.Format(@"{0} {1}", appDataBackupName, todaysDate);
            Directory.CreateDirectory(backupFolderName);
            return backupFolderName;
        }

        private static void CopyBackupFiles(string backupFolderName)
        {
            Console.WriteLine("Copying backup files");
            CopyDirectory.Copy(appDataDirectoryToExportFrom, string.Format(@"{0}\{1}", pathToExportTo, backupFolderName));
        }

        private static void DeleteOldFiles()
        {
            Console.WriteLine("Deleting old backup files");
            var registryBackupFiles = new DirectoryInfo(pathToExportTo)
                .EnumerateFiles()
                .Where(m => m.Name.StartsWith(registryBackupName))
                .OrderByDescending(f => f.CreationTime)
                .Skip(numberOfBackupsToKeep)
                .ToList();
            registryBackupFiles.ForEach(f => f.Delete());
        }

        private static void DeleteOldFolders()
        {
            Console.WriteLine("Deleting old backup folders");
            var appDataDirectories = new DirectoryInfo(pathToExportTo)
                .EnumerateDirectories()
                .Where(m => m.Name.StartsWith(appDataBackupName))
                .OrderByDescending(d => d.CreationTime)
                .Skip(numberOfBackupsToKeep)
                .ToList();
            appDataDirectories.ForEach(d => d.Delete(true));
        }
    }
}
