using System;
using System.Collections.Generic;
using System.Text;
using Crawler.Structures;
using Scraper.Structures;

namespace Crawler.Interfaces
{
    /// <summary>
    /// Interface which provides the access to getting the resulting data from the given site
    /// </summary>
    interface ISite
    {
        Site Site { get; set; }
        List<Hotel> GetData(RequestArgs args);
    }
}
