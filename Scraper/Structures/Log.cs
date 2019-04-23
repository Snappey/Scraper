using System;
using System.Collections.Generic;
using System.Text;

namespace Scraper.Structures
{
    public class Log
    {
        public string Contents;
        public Site Site;
        public LogType Type;
    }

    public enum LogType
    {
        Downloader,
        Processing,
        Information,
        Output
    }
}
