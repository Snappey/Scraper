using System;
using System.Collections.Generic;
using Scraper;
using Scraper.Structures;

namespace Crawler
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            
            Console.SetWindowSize(Console.LargestWindowWidth / 2, Console.LargestWindowHeight);
            Console.SetWindowPosition(0, 0);

            var scraper = new Scraper.Scraper();

            Site dotnetperls = new Site(new Uri("https://www.dotnetperls.com"));
            PageLayout indexLayout = dotnetperls.AddPage("");
            PageLayout asyncLayout = dotnetperls.AddPage("async");

            indexLayout.AddNode(new NodeRequest{Property = "Pages", XPath = "//body//a//b"});
            indexLayout.AddNode(new NodeRequest{Property = "Description", XPath = "//body//a//div" });
            asyncLayout.AddNode(new NodeRequest{Property = "Title", XPath = "//body//a"});

            Console.WriteLine(dotnetperls.ToString());
            scraper.AddSite(dotnetperls);

            scraper.Run();

            Console.ReadKey();
        }
    }
}
