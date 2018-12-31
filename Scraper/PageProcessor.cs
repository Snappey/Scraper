using System;
using System.Collections.Generic;
using System.Text;
using Scraper.Structures;
using HtmlAgilityPack;

namespace Scraper
{
    class PageProcessor
    {
        public void Next(RawPage rawPage, Site site)
        {

            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(rawPage.Content);

            List<string> layouts = site.Pages[rawPage.URL.LocalPath.Remove(0,1)].Nodes;

            Console.WriteLine("Scraping, " + rawPage.URL);
            foreach(string xpath in layouts)
            {
                Console.Write("| > " + xpath + " - ");

                var nodes = html.DocumentNode.SelectNodes(xpath);

                if (nodes != null)
                {
                    Console.Write("Found Matching Nodes\n\r");

                    int i = 1;
                    foreach(HtmlNode node in nodes)
                    {
                        Console.WriteLine("|     > #" + i++ + ": '" + node.InnerText + "'");
                    }
                }
                else
                {
                    Console.Write("Missing Nodes\n\r");
                }
            }      
            Console.WriteLine("");
        }
    }
}
