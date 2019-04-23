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

            List<NodeRequest> layouts = site.Pages[rawPage.URL.PathAndQuery.Remove(0,1)].Nodes;
            List<NodeResult> htmlNodes = new List<NodeResult>();

            site.Log("Processing, " + rawPage.URL);
            foreach(NodeRequest request in layouts)
            {
                site.Log(request.Property + " - {XPath: " + request.XPath + "}", LogType.Processing);

                var nodes = html.DocumentNode.SelectNodes(request.XPath);

                if (nodes != null)
                {
                    foreach(HtmlNode node in nodes)
                    {
                        site.Log(request.Property + ": " + node.InnerText, LogType.Processing);
                    }

                    NodeResult result = new NodeResult();
                    result.Property = request.Property;
                    result.Nodes = nodes.ToList();
                    result.Site = site;
                    result.Page = rawPage.URL.AbsolutePath;

                    if (request.Attribute != null)
                    {
                        result.Attribute = request.Attribute;
                    }

                    htmlNodes.Add(result);
                } 
            }
            //Console.WriteLine("|" + string.Concat(Enumerable.Repeat("-", Console.BufferWidth - 1)));
            return htmlNodes;
        }
    }
}
