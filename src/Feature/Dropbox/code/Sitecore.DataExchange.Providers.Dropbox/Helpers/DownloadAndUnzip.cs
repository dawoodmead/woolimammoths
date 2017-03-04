using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;

namespace Sitecore.DataExchange.Providers.Dropbox.Helpers
{

    public static class DownloadAndUnzipHelper
    {

        public static string Run(string dropboxurl)
        {
            var datePart = DateTime.Now.ToString("yyyyMMMMddhhmmss");
            var zipPath = AssemblyDirectory + "\\upload\\dropbox\\" + datePart + ".zip";
            var extractPath = AssemblyDirectory + "\\upload\\dropboxextract\\" + datePart;

            var url = FixLink(dropboxurl);

            var client = new WebClient();
            client.DownloadFile(url, zipPath);
            client.Dispose();

            UnzipFiles(zipPath, extractPath);

            return extractPath;
        }

        public static string FixLink(string url)
        {
            var uri = new Uri(url);
            return uri.GetLeftPart(UriPartial.Path) + "?dl=1";
        }

        private static void UnzipFiles(string zipPath, string extractPath)
        {


            if (!Directory.Exists(extractPath))
                Directory.CreateDirectory(extractPath);

            using (var archive = ZipFile.OpenRead(zipPath))
            {
                foreach (var entry in archive.Entries)
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

        private static void CreateDirectoryRecursively(string extractPath, string path)
        {
            var pathParts = path.Split('/');

            for (var i = 0; i < pathParts.Length; i++)
            {
                if (pathParts[i].Contains("."))
                    continue;
                if (i > 0)
                    pathParts[i] = Path.Combine(pathParts[i - 1], pathParts[i]);

                if (!Directory.Exists(extractPath + "\\" + pathParts[i]))
                    Directory.CreateDirectory(extractPath + "\\" + pathParts[i]);
            }
        }

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path).Replace("\\bin",string.Empty);
            }
        }
    }
}
