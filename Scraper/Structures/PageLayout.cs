using System;
using System.Collections.Generic;
using System.Text;

namespace Scraper.Structures
{
    public class PageLayout
    {
        public List<NodeRequest> Nodes = new List<NodeRequest>();
        public string Path;
        public Uri URL;
        public string SearchElement;
        public string JSExecution;
        public int PageDelay;

        public PageLayout(Uri url, string path, string searchElement = "body", string jsExec = "", int pageDelay = 0)
        {
            URL = url;
            Path = path;
            SearchElement = searchElement;
            JSExecution = jsExec;
            PageDelay = pageDelay;
        }

        public void AddNode(NodeRequest request)
        {
            Nodes.Add(request);
        }
    }
}
