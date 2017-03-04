using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Dropbox.Api;
using System.IO;
using Dropbox.Api.Users;
using static System.Configuration.ConfigurationSettings;

namespace DropboxHelloWorld
{
    
    class Program
    {

        protected static string _downloadRootLocation { get; set; }
        protected static string _dropboxFolderPrefix { get; set; }

        protected static string _dropboxFolderPostfix { get; set; }

        protected static string _dropboxSharedUrl { get; set; }
     
        static void Main(string[] args)
        {
            GetConfigSettings();
            var task = Task.Run((Func<Task>)Program.Run);
            task.Wait();
        }

        private static void GetConfigSettings()
        {
            _downloadRootLocation = ConfigurationManager.AppSettings["DownloadRootLocation"];
            _dropboxFolderPrefix = ConfigurationManager.AppSettings["DropboxFolderPrefix"];
            _dropboxFolderPostfix = ConfigurationManager.AppSettings["DropboxFolderPostfix"];
            _dropboxSharedUrl = ConfigurationManager.AppSettings["DropboxSharedUrl"];
        }

        static async Task Run()
        {
            string datePart = DateTime.Now.ToString(_dropboxFolderPostfix);
            string zipPath = _downloadRootLocation + _dropboxFolderPrefix + datePart + ".zip";

            string url = _dropboxSharedUrl;

            WebClient client = new WebClient();
            client.DownloadFile(url, zipPath);
            client.Dispose();

            await Program.UnzipFiles(zipPath, datePart);


        }

        static async Task UnzipFiles(string zipPath, string datePart)
        {

            string extractPath = _downloadRootLocation + _dropboxFolderPrefix + datePart;

            if (!System.IO.Directory.Exists(extractPath))
                System.IO.Directory.CreateDirectory(extractPath);

            using (ZipArchive archive = ZipFile.OpenRead(zipPath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (entry.Length > 0)
                    {
                        if (entry.FullName.IndexOf('/') > 0)
                        {
                            CreateDirectoryRecursively(extractPath, entry.FullName);
                        }
                        if (!System.IO.File.Exists(extractPath + "\\" + entry.FullName.Replace("/", "\\")))
                        {
                            entry.ExtractToFile(extractPath + "\\" + entry.FullName.Replace("/", "\\"));
                        }
                    }
                }
            }
        }
        static void CreateDirectoryRecursively(string extractPath,string path)
        {
            string[] pathParts = path.Split('/');

            for (int i = 0; i < pathParts.Length; i++)
            {
               if (pathParts[i].Contains("."))
                    continue;
                if (i > 0)
                pathParts[i] = Path.Combine(pathParts[i - 1], pathParts[i]);

                if (!Directory.Exists(extractPath + "\\" + pathParts[i]))
                    Directory.CreateDirectory(extractPath + "\\" + pathParts[i]);
            }
        }

       

       

      
    }
}
