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

        public PageLayout(Uri url, string path)
        {
            URL = url;
            Path = path;
        }

        public void AddNode(NodeRequest request)
        {
            Nodes.Add(request);
        }
    }
}
