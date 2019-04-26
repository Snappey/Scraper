using System;
using System.Collections.Generic;
using System.Text;
using Crawler.Structures;
using Scraper.Structures;

namespace Crawler.Interfaces
{
    interface IScrapable
    {
        Scraper.Scraper Scraper { get; set; }
        void RegisterPages(RequestArgs args=null);
        List<Hotel> PostProcess(List<Hotel> hotels, RequestArgs args);
    }
}
