using System;
using System.Collections.Generic;
using System.Text;
using Scraper.Structures;

namespace Crawler.Interfaces
{
    interface IScrapable
    {
        Site Site { get; set; }
        void RegisterPages();
    }
}
