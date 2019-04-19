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

        public PageLayout(Uri url, string path, string searchElement = "body")
        {
            URL = url;
            Path = path;
            SearchElement = searchElement;
        }

        public void AddNode(NodeRequest request)
        {
            Nodes.Add(request);
        }
    }
}
