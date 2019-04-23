using System;
using System.Collections.Generic;
using System.Text;

namespace Scraper.Structures
{
    class DownloadResult
    {
        public List<RawPage> Results;
        public DownloadStatus Status;
        public List<string> Logs;
    }

    [Flags]
    enum DownloadStatus
    {
        Success,
        Failed,
        ErrorOccurred
    }
}
