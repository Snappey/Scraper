using System;
using System.Collections.Generic;
using System.Text;
using Scraper.Structures;

namespace Scraper
{
    class LogStream
    {
        public Queue<Log> Stream = new Queue<Log>();
        public void Log(string log, Site site, LogType type = LogType.Information)
        {
            Stream.Enqueue(new Log
            {
                Type = type,
                Site = site,
                Contents = log
            });
        }

        public void Flush()
        {
            Stream.Clear();
        }
    }
}
