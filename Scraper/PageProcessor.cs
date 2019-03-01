using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scraper.Structures;
using HtmlAgilityPack;

namespace Scraper
{
    internal class PageProcessor
    {
        public List<NodeResult> Next(RawPage rawPage, Site site)
        {

            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(rawPage.Content);

            List<NodeRequest> layouts = site.Pages[rawPage.URL.LocalPath.Remove(0,1)].Nodes;
            List<NodeResult> htmlNodes = new List<NodeResult>();

            Console.WriteLine("|> Processing, " + rawPage.URL);
            foreach(NodeRequest request in layouts)
            {
                Console.WriteLine("| > '" + request.Property + "' - {XPath: " + request.XPath + "}");

                var nodes = html.DocumentNode.SelectNodes(request.XPath);

                if (nodes != null)
                {
                    int i = 1;
                    foreach(HtmlNode node in nodes)
                    {
                        Console.WriteLine("|  ---> #" + i++ + ": '" + node.InnerText + "'");
                    }

                    NodeResult result = new NodeResult();
                    result.Property = request.Property;
                    result.Nodes = nodes.ToList();
                    result.Site = site;
                    result.Page = rawPage.URL.AbsolutePath;

                    htmlNodes.Add(result);
                } 
            }
            Console.Write("|" + string.Concat(Enumerable.Repeat("-", Console.BufferWidth - 1)));
            return htmlNodes;
        }
    }
}
