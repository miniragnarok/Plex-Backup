using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Plex_Backup
{
    public class RegistryService
    {
        public static void ExportDirectory(string registryDirectory, string pathToExportTo, string newFileName)
        {
            var fileName = "regedit.exe";
            var newFileWithPath = string.Format(@"{0}\{1}", pathToExportTo, newFileName);
            var arguments = string.Format(@"/E ""{0}"" ""{1}""", newFileWithPath, registryDirectory);

            using (var process = new Process())
            {
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.Verb = "runas";
                process.StartInfo.FileName = fileName;
                process.StartInfo.Arguments = arguments;
                process.Start();
                process.WaitForExit();
            }
        }
    }
}
