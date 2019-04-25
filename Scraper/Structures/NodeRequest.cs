using System;
using System.Collections.Generic;
using System.Text;

namespace Scraper.Structures
{
    public struct NodeRequest
    {
        public string XPath;
        public string Property;
        public string Attribute;
        public bool Recursive;
        public string RecursiveXPath;
    }
}
