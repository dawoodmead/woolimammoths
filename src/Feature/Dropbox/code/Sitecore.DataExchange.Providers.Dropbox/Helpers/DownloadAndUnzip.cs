using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;

namespace Sitecore.DataExchange.Providers.Dropbox.Helpers
{

    public static class DownloadAndUnzipHelper
    {

        public static void Run(string dropboxurl)
        {
            try
            {
                //download location for the dropbox URL
                var downloadLocation = "\\temp\\dropbox\\";
                
                //extracting zip file location for the dropbox
                var uploadLocation = "\\upload\\dropboxextract\\";
                
                //unique date time format
                var datePart = DateTime.Now.ToString("yyyyMMMMddhhmmss");
                
                //full zip path
                var zipPath = AssemblyDirectory + downloadLocation + datePart + ".zip";
                
                //full extraction path
                var extractPath = AssemblyDirectory + uploadLocation + datePart;
                
                //fix url link for any dl=0
                var url = FixLink(dropboxurl);
                
                //create the download location
                if (!Directory.Exists(AssemblyDirectory + downloadLocation))
                {
                    Directory.CreateDirectory(AssemblyDirectory + downloadLocation);
                }
                //if there is no extension in the url, download it as a  zip
                if (!HasUrlExtensions(dropboxurl))
                {
                    var client = new WebClient();
                    client.DownloadFile(url, zipPath);
                    client.Dispose();
                    UnzipFiles(zipPath, extractPath);
                }
                //otherwise download it with the extension
                else
                {
                    var client = new WebClient();
                    var extension = GetUrlExtension(url).Replace("?dl=1", string.Empty);
                    client.DownloadFile(url, extractPath  +  extension);
                    client.Dispose();
                }

            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error(ex.Message,ex,typeof(DownloadAndUnzipHelper));

            }
            
        }

        public static string FixLink(string url)
        {
            var uri = new Uri(url);
            return uri.GetLeftPart(UriPartial.Path) + "?dl=1";
        }

        private static void UnzipFiles(string zipPath, string extractPath)
        {
            try
            {
                if (!Directory.Exists(extractPath))
                {
                    Directory.CreateDirectory(extractPath);
                }
                using (var archive = ZipFile.OpenRead(zipPath))
                {
                    foreach (var entry in archive.Entries)
                    {
                        if (entry.Length > 0)
                        {
                            //create directories recursively
                            if (entry.FullName.IndexOf('/') > 0)
                            {
                                CreateDirectoryRecursively(extractPath, entry.FullName);
                            }
                            //extract file
                            if (!System.IO.File.Exists(extractPath + "\\" + entry.FullName.Replace("/", "\\")))
                            {
                                entry.ExtractToFile(extractPath + "\\" + entry.FullName.Replace("/", "\\"));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error(ex.Message, ex, typeof(DownloadAndUnzipHelper));
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
                {
                    Directory.CreateDirectory(extractPath + "\\" + pathParts[i]);
                }
            }
        }
        public static string AssemblyDirectory
        {
            get
            {
                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path)?.Replace("\\bin",string.Empty);
            }
        }
        private static bool HasUrlExtensions(string url)
        {
            Uri uri = new Uri(url);
            return Path.HasExtension(uri.AbsoluteUri);
        }
        private static string GetUrlExtension(string url)
        {
            return Path.GetExtension(url);
        }
    }
}
