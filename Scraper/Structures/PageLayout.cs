using System;
using System.Collections.Generic;
using System.Text;

namespace Scraper.Structures
{
    public class PageLayout
    {
        public List<string> Nodes = new List<string>();
        public string Path;
        public Uri URL;

        public PageLayout(Uri url, string path)
        {
            URL = url;
            Path = path;
        }

        public void AddNode(string xpath)
        {
            Nodes.Add(xpath);
        }
    }
}
