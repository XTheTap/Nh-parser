using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace Nh_parser
{
    internal class Program
    {
        public static void Main()
        {
            Console.WriteLine("Input link or numbers:");
            String numbr = Regex.Match(Console.ReadLine(),@"\d+").ToString();
            Console.WriteLine("Go to: " + numbr);

            try
            {
                using (WebClient wc = new WebClient())
                {
                    wc.Headers.Add("Root-pc", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");;
                    
                    string line = wc.DownloadString("https://nhentai.net/g/" + numbr);
                    string name = Regex.Match(line, @"<span class=.before.>(.+?)</span>").Groups[1].ToString() +
                                  Regex.Match(line, @"<span class=.pretty.>(.+?)</span>").Groups[1] +
                                  Regex.Match(line, @"<span class=.after.>(.+?)</span>").Groups[1];
                    
                    uint pageValue = Convert.ToUInt32(Regex.Match(line, @"<span class=.name.>(\d+)</span>")
                        .Groups[1].ToString());
                    
                    uint downloadNum = Convert.ToUInt32(Regex.Match(line, @"<meta itemprop=.image. content=.+\/galleries\/(\d+)")
                        .Groups[1].ToString());
                    
                    Console.WriteLine("Name: " + name + " with pages value: " + pageValue);
                    Console.WriteLine("Are you sure that you want download it? [Y/n]: ");
                    bool ans = ParsAns(Console.ReadLine());
                        
                    if (!ans) return;

                    try { Directory.CreateDirectory(name); }
                    catch { Console.WriteLine("Cant create directory"); throw; }
                    
                    for (int i = 1; i < pageValue + 1; i++)
                        wc.DownloadFile("https://i.nhentai.net/galleries/" + downloadNum + "/" + i + ".jpg", name + "/" + i + ".jpg");

                    Console.WriteLine("Done");
                }
            } catch (Exception e) { Console.WriteLine("That shit dosent download, maybe deal with your internet, exception: \n" + e); }

            Console.WriteLine("Press any key to clese this shit");
            Console.ReadKey();
        }

        static public bool ParsAns(string pars)
        {
            switch (pars.ToLower())
            {
                case "y": return true;
                case "n": return false;
                default: return false;
            }
        }
    }
}