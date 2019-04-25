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
        public List<NodeResult> Next(RawPage rawPage, Site site, Downloader downloader)
        {

            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(rawPage.Content);

            List<NodeRequest> layouts = site.Pages[rawPage.URL.PathAndQuery.Remove(0,1)].Nodes;
            List<NodeResult> htmlNodes = new List<NodeResult>();

            site.Log("Processing, " + layouts.Count + " Nodes");
            foreach(NodeRequest request in layouts)
            {
                //site.Log(request.Property + " - {XPath: " + request.XPath + "}", LogType.Processing);

                var nodes = html.DocumentNode.SelectNodes(request.XPath);

                if (nodes != null)
                {
                    /*foreach(HtmlNode node in nodes)
                    {
                        if (request.Property == "Name")
                        {
                            site.Log("Processing, " + node.InnerText);
                        }
                    }*/

                    NodeResult result = new NodeResult();
                    result.Property = request.Property;
                    result.Nodes = nodes.ToList();
                    result.Site = site;
                    result.Page = rawPage.URL.AbsolutePath;

                    if (request.Attribute != null)
                    {
                        result.Attribute = request.Attribute;
                    }


                    if (request.Recursive && request.Attribute == "href")
                    {
                        var node = result.Nodes.FirstOrDefault();
                        if (node?.Attributes[request.Attribute] != null)
                        {
                            var link = node.Attributes[request.Attribute].Value;
                            site.Log("Recursive request downloading page..", LogType.Downloader);

                            if (link.Substring(0,4) != "http") // Attach the host address if its not inside the href attribute
                            {
                                if (link.Substring(0,5) == "&#10;") // Specific workaround for bookings.com
                                {
                                    link = link.Substring(6);
                                }
                                link = "http://" + site.URL.Host + "/" + link;
                            }

                            var recursivePage = downloader.DownloadPage(new Uri(link));

                            var recursiveDoc = new HtmlDocument();
                            recursiveDoc.LoadHtml(recursivePage.Content);

                            var recursiveNodes = recursiveDoc.DocumentNode.SelectNodes(request.RecursiveXPath);

                            if (recursiveNodes != null) // Recursive XPath didnt find anything
                            {
                                htmlNodes.Add(new NodeResult
                                {
                                    Property = request.Property,
                                    Attribute = request.Attribute,
                                    Nodes = recursiveNodes.ToList(),
                                    Site = site,
                                    Page = rawPage.URL.AbsolutePath,
                                });
                            }
                        }
                    }
                    else
                    {
                        htmlNodes.Add(result);
                    }           
                } 
            }
            site.Log("Finished Processing!");
            //Console.WriteLine("|" + string.Concat(Enumerable.Repeat("-", Console.BufferWidth - 1)));
            return htmlNodes;
        }
    }
}
