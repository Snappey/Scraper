using System;
using System.Collections.Generic;
using System.Text;
using Crawler.Structures;
using Scraper.Structures;

namespace Crawler.Interfaces
{
    /// <summary>
    /// Interface for Sites, provides the common functions for interacting with the scraper library
    /// </summary>
    interface IScrapable
    {
        Scraper.Scraper Scraper { get; set; }
        void RegisterPages(RequestArgs args=null);
        List<Hotel> PostProcess(List<Hotel> hotels, RequestArgs args);
    }
}
