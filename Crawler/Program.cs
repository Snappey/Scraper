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

            Scraper.Scraper scraper = new Scraper.Scraper();

            Site dotnetperls = new Site(new Uri("https://www.dotnetperls.com"));
            var indexLayout = dotnetperls.AddPage("/");
            var asyncLayout = dotnetperls.AddPage("async");

            asyncLayout.AddNode("//*[@id=\"u\"]/div/p[3]/text()");


            Console.WriteLine(dotnetperls.ToString());
            scraper.AddSite(dotnetperls);

            scraper.Start();

            Console.ReadKey();
        }
    }
}
