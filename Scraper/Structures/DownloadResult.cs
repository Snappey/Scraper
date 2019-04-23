using System;
using System.Collections.Generic;
using System.Text;

namespace Scraper.Structures
{
    class DownloadResult
    {
        public List<RawPage> Results = new List<RawPage>();
        public DownloadStatus Status;
        public List<string> Logs = new List<string>();
    }

    [Flags]
    enum DownloadStatus
    {
        Success,
        Failed,
        ErrorOccurred
    }
}
