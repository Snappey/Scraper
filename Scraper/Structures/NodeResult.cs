using System;
using System.Collections.Generic;
using System.Text;
using HtmlAgilityPack;

namespace Scraper.Structures
{
    public struct NodeResult
    {
        public List<HtmlNode> Nodes;
        public Site Site;
        public string Property;
        public string Page;
    }
}
