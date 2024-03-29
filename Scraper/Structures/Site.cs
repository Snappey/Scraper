﻿using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;

namespace Scraper.Structures
{
    public class Site
    {
        public Uri URL;
        public Dictionary<string, PageLayout> Pages = new Dictionary<string, PageLayout>();
        private LogStream logStream;

        public SiteStatus Status = SiteStatus.Pending;
        public DateTime SiteStart = DateTime.MinValue;
        public DateTime SiteFinished = DateTime.MinValue;

        public event EventHandler LogReceived = delegate { };

        #region Config Options
        public bool IncludeIndex;
        public bool ScrapeLinks;
        public bool ScrapeImages;
        public PipelineOutput OutputType;
        #endregion

        public Site(Uri uri, bool incidx = false, bool scplnks = false, bool scpimgs = false, PipelineOutput outputtype = PipelineOutput.Plaintext)
        {
            URL = uri;

            IncludeIndex = incidx;
            ScrapeLinks = scplnks;
            ScrapeImages = scplnks;
            OutputType = outputtype;

            logStream = new LogStream();
        }

        public PageLayout AddPage(string page, By searchElement, string jsExec = "", string xPathFilter = "", int pageDelay = 0)
        {
            PageLayout pageLayout = new PageLayout(URL, page, searchElement, jsExec, xPathFilter, pageDelay);
            Pages.Add(page, pageLayout);
            Log("Registered page: " + page);
            return pageLayout;
        }

        public void Log(string log, LogType type = LogType.Information)
        {
            logStream.Log(log, this, type);

            LogReceived.Invoke(this, new LogEventArgs
            {
                Log = $"[{DateTime.Now.ToShortTimeString()}] {URL.Host}: {log}",
                Type = type,
            });
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("| Site Layout");
            stringBuilder.AppendLine("| Base: " + URL.AbsoluteUri);
            foreach(PageLayout page in Pages.Values)
            {
                stringBuilder.AppendLine("| -> /" + page.Path);
            }

            return stringBuilder.ToString();
        }
    }
}
