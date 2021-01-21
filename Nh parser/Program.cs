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
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Input link or numbers:");
                //Taking the numbers of manga
                String numbr = Regex.Match(Console.ReadLine(), @"\d+").ToString();
                Console.WriteLine("Going to: " + numbr);

                using (WebClient wc = new WebClient())
                {
                    //Set name of parser
                    wc.Headers.Add("NhParser", "Mozilla/4.0 (compatible; MSIE 6.0; Windows 10; .NET CLR 1.0.3705;)");
                    
                    string line = "";

                    try
                    {
                        //Writing all our page to 
                        line = wc.DownloadString("https://nhentai.net/g/" + numbr);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("That thing dosen't wanna to go by the link, " +
                                          "maybe deal with your internet or your numbers, lol. Exception: \n" +
                                          e.Message + "\nPress any key to continue");
                        Console.ReadKey();
                        continue;
                    }

                    //Making one name form author, name of manga and last
                    string name = Regex.Match(line, @"<span class=.before.>(.+?)</span>").Groups[1].ToString() +
                                  Regex.Match(line, @"<span class=.pretty.>(.+?)</span>").Groups[1] +
                                  Regex.Match(line, @"<span class=.after.>(.+?)</span>").Groups[1];

                    //Replacing special symbols, becouse 
                    name = Regex.Replace(name, "[\\|?/*:\"<>]", " "); 

                    //Parsing page value
                    uint pageValue = Convert.ToUInt32(Regex.Match(line, @"<span class=.name.>(\d+)</span>")
                        .Groups[1].ToString());

                    Console.WriteLine("Name: " + name + " with pages value: " + pageValue);

                    Console.WriteLine("Are you sure that you want download it? [Y/n]: ");
                    if (Console.ReadLine().ToLower() != "y") continue;

                    //Creating directory in download folder 
                    try
                    {
                        Directory.CreateDirectory(Environment.GetEnvironmentVariable("USERPROFILE") + "/Manga/" + name);
                    }
                    catch
                    {
                        Console.WriteLine("Cant create directory \nPress any key to continue");
                        Console.ReadKey();
                        continue;
                    }

                    //Downloading all pages in loop
                    try
                    {
                        foreach (Match i in Regex.Matches(line,
                        //parsing info about id manga in site database, page number and image format
                            @"img\ssrc=.https:\/\/t.nhentai.net\/galleries\/(\d+)\/(\d+)t(.\w+)."))
                        {//downloading parsed file
                            wc.DownloadFile(
                                new Uri("https://i.nhentai.net/galleries/" + i.Groups[1] + "/" + i.Groups[2] +
                                        i.Groups[3]),
                                Environment.GetEnvironmentVariable("USERPROFILE") + "/Manga/"
                                + name + "/" + i.Groups[2] + i.Groups[3]);
                            Console.WriteLine("Page: " + i.Groups[2] + " ...Done");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("That thing dosen't download, maybe deal with your internet, exception: \n" +
                                          e.Message + "\nPress any key to continue");
                        Console.ReadKey();
                        continue;
                    }

                    Console.Clear();
                    Console.WriteLine("All Done, Are you want to continue [Y/n]");
                    if (Console.ReadLine().ToLower() != "y") break;
                }
            }
        }
    }
}