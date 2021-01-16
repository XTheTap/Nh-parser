using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace Nh_parser
{
    internal class Program
    {
        public static void Main()
        {
            Console.WriteLine("Input link or numbers:");
            String numbr = Regex.Match(Console.ReadLine(),@"\d+").ToString();
            Console.WriteLine("Go to: " + numbr);

           
            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add("2XTheTap", "Mozilla/4.0 (compatible; MSIE 6.0; Windows 10; .NET CLR 1.0.3705;)");;
                
                string line = wc.DownloadString("https://nhentai.net/g/" + numbr);
                string name = Regex.Match(line, @"<span class=.before.>(.+?)</span>").Groups[1].ToString() +
                              Regex.Match(line, @"<span class=.pretty.>(.+?)</span>").Groups[1] +
                              Regex.Match(line, @"<span class=.after.>(.+?)</span>").Groups[1];

                name = Regex.Replace(name, "[\\|?/*:\"<>]", " ");
                
                uint pageValue = Convert.ToUInt32(Regex.Match(line, @"<span class=.name.>(\d+)</span>")
                    .Groups[1].ToString());

                var tempRg = Regex.Match(line, @"<meta itemprop=.image. content=.+\/galleries\/(\d+)/\w+(.\w+)");
                
                uint downloadId = Convert.ToUInt32(tempRg
                    .Groups[1].ToString());
                
                string picForm = tempRg
                    .Groups[2].ToString();
                
                Console.WriteLine("Name: " + name + " with pages value: " + pageValue);
                Console.WriteLine("Are you sure that you want download it? [Y/n]: ");
                bool ans = Console.ReadLine().ToLower() == "y";
                    
                if (!ans) return;

                try { Directory.CreateDirectory("e:/Личное/Личное блять/Оно того не стоит/Manga/"
                                                + name); }
                catch { Console.WriteLine("Cant create directory"); throw; }
                
                try {
                    for (uint i = 1; i < pageValue + 1; i++)
                    {
                        wc.DownloadFile(
                            new Uri("https://i.nhentai.net/galleries/" + downloadId + "/" + i + picForm),
                            "e:/Личное/Личное блять/Оно того не стоит/Manga/"
                            + name + "/" + i + picForm);
                        Console.WriteLine("Picture: " + i + " Done");
                    }
                } catch (Exception e) { Console.WriteLine("That shit dosent download, maybe deal with your internet, exception: \n" + e.Message); }
                
                Console.WriteLine("Done");
            }
        }
    }
}