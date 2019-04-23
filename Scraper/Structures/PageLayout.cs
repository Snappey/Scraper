using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;

namespace Scraper.Structures
{
    public class PageLayout
    {
        public List<NodeRequest> Nodes = new List<NodeRequest>();
        public string Path;
        public Uri URL;
        public By SearchElement;
        public string XPathFilter;
        public string JSExecution;
        public int PageDelay;

        public PageLayout(Uri url, string path, By searchElement, string jsExec = "", string xPathFilter = "", int pageDelay = 0)
        {
            URL = url;
            Path = path;
            SearchElement = searchElement;
            JSExecution = jsExec;
            XPathFilter = xPathFilter;
            PageDelay = pageDelay;
        }

        public void AddNode(NodeRequest request)
        {
            Nodes.Add(request);
        }
    }
}
