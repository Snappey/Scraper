﻿using System;
using System.Collections.Generic;
using System.Text;
using Crawler.Structures;

namespace Crawler.Report.Models
{
    /// <summary>
    /// ViewModel used in the HTML template, required for the RazerEngine to render the template and pass C# data through
    /// </summary>
    public class ReportViewModel
    {
        public string SearchLocation { get; set; }
        public string CheckIn { get; set; }
        public string CheckOut { get; set; }
        public string ProviderLink { get; set; }
        public string RoomAmount { get; set; }
        public string PersonAmount { get; set; }
        public DateTime Gathered { get; set; }
        public List<Hotel> Hotels { get; set; }
    }
}
