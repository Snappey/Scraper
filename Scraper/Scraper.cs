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

        private List<Site> sites = new List<Site>(); // TODO: Run should iterate through sites instead of a list of pages, store rawpages in the site structure rather than the scraper

        private delegate void DownloadedHandler(object sender, EventArgs e);

        public event EventHandler PageDownloaded = delegate { };
        public event EventHandler PageProcessed = delegate { };

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
                Console.WriteLine("Site is not registered in Scraper List!");
                return;
            }

            Queue<RawPage> rawPages = new Queue<RawPage>();
            List<NodeResult> results = new List<NodeResult>();

            foreach (PageLayout page in site.Pages.Values)
            {
                RawPage rawPage = downloadManager.Next(new Uri(page.URL + page.Path));
                PageDownloaded.Invoke(rawPage, EventArgs.Empty);

                rawPages.Enqueue(rawPage);
            }

            Console.WriteLine("|" + string.Concat(Enumerable.Repeat("-", Console.BufferWidth - 1)));

            while (rawPages.Count > 0)
            {
                RawPage rawPage = rawPages.Dequeue();

                results = pageProcessor.Next(rawPage, site);
                PageProcessed.Invoke(results, EventArgs.Empty);

                outputPipeline.Output(results, site, rawPage.URL.LocalPath);
            }
        }

        public void RunAll()
        {
            if (sites.Count == 0)
            {
                Console.WriteLine("Download Manager queue is empty!");
                return;
            }

            Queue<RawPage> rawPages = new Queue<RawPage>();
            List<NodeResult> results = new List<NodeResult>();

            foreach(Site site in sites)
            {
                foreach (PageLayout page in site.Pages.Values)
                {
                    RawPage rawPage = downloadManager.Next(new Uri(page.URL + page.Path));
                    PageDownloaded.Invoke(rawPage, EventArgs.Empty);

                    rawPages.Enqueue(rawPage);
                }

                Console.WriteLine("|" + string.Concat(Enumerable.Repeat("-", Console.BufferWidth - 1)));

                while (rawPages.Count > 0)
                {
                    RawPage rawPage = rawPages.Dequeue();

                    results = pageProcessor.Next(rawPage, site);
                    PageProcessed.Invoke(results, EventArgs.Empty);

                    outputPipeline.Output(results, site, rawPage.URL.LocalPath);
                }
            }
        }

        public void AddSite(Site site)
        {
            sites.Add(site);
        }
    }
}
