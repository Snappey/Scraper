using System;
using System.Collections.Generic;
using System.Text;

namespace Crawler.Report.Models
{
    class HotelViewModel
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string HotelLink { get; set; }
        public string ProviderLink { get; set; }
        public DateTime Gathered { get; set; }
    }
}
