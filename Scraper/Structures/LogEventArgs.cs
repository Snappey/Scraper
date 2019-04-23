using System;
using System.Collections.Generic;
using System.Text;

namespace Scraper.Structures
{
    public class LogEventArgs : EventArgs
    {
        public string Log;
        public LogType Type;
    }
}
