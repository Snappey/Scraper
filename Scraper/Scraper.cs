using Scraper.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace Scraper
{
    public class Scraper
    {
        private Downloader downloadManager;
        private PageProcessor pageProcessor;
        private Pipeline outputPipeline;

        private List<Site> sites = new List<Site>();

        public event EventHandler PageDownloaded = delegate { };
        public event EventHandler PageProcessed = delegate { };
        public event EventHandler PageCompleted = delegate { };

        public Scraper()
        {
            downloadManager = new Downloader();
            pageProcessor = new PageProcessor();
            outputPipeline = new Pipeline();
        }

        public void Run(Site site)
        {
            if (sites.Contains(site) == false)
            {
                site.Log("Site is not registered in Scraper List!");
                return;
            }

            Queue<RawPage> rawPages = new Queue<RawPage>(); // Setup for transfer of data between each of the classes
            List<NodeResult> results = new List<NodeResult>();

            site.Status = SiteStatus.Downloading; // Set site status
            site.SiteStart = DateTime.Now;

            foreach (PageLayout page in site.Pages.Values)
            {
                site.Log("Downloading " + site.URL + "...", LogType.Downloader);
                
                DownloadResult result = downloadManager.Next(new Uri(page.URL + page.Path), page.SearchElement, page.JSExecution, page.XPathFilter, page.PageDelay);
                // Download each page and store it,

                if (result.Status.HasFlag(DownloadStatus.ErrorOccurred)) // Error checking if any errors occured let the user know and log it
                {
                    site.Log("Error occurred in " + site.URL, LogType.Downloader);
                }

                if (result.Status.HasFlag(DownloadStatus.Failed))
                {
                    site.Log("Failed to download " + site.URL + " skipped..", LogType.Downloader);
                    continue;
                }

                result.Results.ForEach((rawPage) =>
                {
                    PageDownloaded.Invoke(rawPage, EventArgs.Empty); // Invoke the event for each page downloaded
                    rawPages.Enqueue(rawPage);
                });

                site.Log("Downloaded " + site.URL + "!", LogType.Downloader);

            }

            //Console.WriteLine("|" + string.Concat(Enumerable.Repeat("-", Console.BufferWidth - 1)));

            site.Status = SiteStatus.Processing;
            while (rawPages.Count > 0)
            {
                RawPage rawPage = rawPages.Dequeue(); // Loop back over the downloaded pages and process them

                results = pageProcessor.Next(rawPage, site, downloadManager);
                PageProcessed.Invoke(results, EventArgs.Empty);

                outputPipeline.Output(results, site, rawPage.URL.LocalPath); // Take the results from page processor and pass them to the pipeline for packaging
            }

            site.Status = SiteStatus.Finished;
            site.SiteFinished = DateTime.Now; // Stopwatch for the sites total running time
        }

        public void RunAll()
        {
            foreach(Site site in sites)
            {
               Run(site);
            }
        }

        public void AddSite(Site site)
        {
            sites.Add(site);
        }

        public Dictionary<Site, Dictionary<string, List<List<NodeResult>>>> GetRawResult()
        {
            return outputPipeline.Data;
        }

        public void FlushData()
        {
            outputPipeline.Data.Clear();
        }
    }
}
