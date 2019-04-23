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
        Site Site { get; set; }
        void RegisterPages(RequestArgs args=null);
    }
}
