using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Dropbox.Api;

namespace DropboxHelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            var task = Task.Run((Func<Task>)Program.Run);
            task.Wait();
        }

        static async Task Run()
        {

            string url = "https://www.dropbox.com/sh/vs5dybevdxvkqk8/AAAkwhfKdC-Kv2GA79zfYP4_a?dl=1";

            WebClient client = new WebClient();
            client.DownloadFile(url, "sitecore-hackathon-wooli-mammoth.zip");
            client.Dispose();
            //using (var dbx = new DropboxClient("KRuKf9o33aAAAAAAAABeOKgNY98ZDz_VQ9uX0d6Gnsyg0B5aysV-qBWItpY52Jb4"))
            //{
            //    var full = await dbx.Users.GetCurrentAccountAsync();
            //    Console.WriteLine("{0} - {1}", full.Name.DisplayName, full.Email);
            //    var list = await dbx.Files.ListFolderAsync("/SBD");

                

            //    // show folders then files
            //    foreach (var item in list.Entries.Where(i => i.IsFolder))
            //    {
            //        Console.WriteLine("D  {0}/", item.Name);

            //    }
            //    ////  var folder = await dbx.Files.ListFolderAsync("home//SBD");
            //    //foreach (var item in list.Entries.Where(i => i.IsFile))
            //    //{
            //    //    Console.WriteLine("F{0,8} {1}", item.AsFile.Size, item.Name);
            //    //    using (var response = await dbx.Files.DownloadAsync("/SBD/" +  item.Name))
            //    //    {
            //    //        Console.WriteLine("Downloaded {0} Rev {1}", response.Response.Name, response.Response.Rev);
            //    //        Console.WriteLine("------------------------------");
            //    //        //Console.WriteLine(await response.GetContentAsStringAsync());
            //    //        //Console.WriteLine("------------------------------");
            //    //        var file = await response.GetContentAsByteArrayAsync();
            //    //        System.IO.File.WriteAllBytes(item.Name,file);

                        
            //    //    }
            //    //}

            //    string url = "https://www.dropbox.com/sh/vs5dybevdxvkqk8/AAAkwhfKdC-Kv2GA79zfYP4_a?dl=1";

            //    WebClient client = new WebClient();
            //    client.DownloadFile(url,"my-file.zip");
            //    client.Dispose();



            //}
        }

      
    }
}
