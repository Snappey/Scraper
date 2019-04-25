using System;
using System.Collections.Generic;
using System.Text;
using Crawler.Structures;
using Scraper.Structures;

namespace Crawler.Interfaces
{
    interface ISite
    {
        Site Site { get; set; }
        List<Hotel> GetData(RequestArgs args);
    }
}
