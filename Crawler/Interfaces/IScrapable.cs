using System;
using System.Collections.Generic;
using System.Text;
using Scraper.Structures;

namespace Crawler.Interfaces
{
    interface IScrapable
    {
        Scraper.Scraper Scraper { get; set; }
        Site Site { get; set; }
        void RegisterPages();
    }
}
