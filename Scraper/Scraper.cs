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

        private Queue<Site> sites = new Queue<Site>();

        public Scraper()
        {
            downloadManager = new Downloader();
            pageProcessor = new PageProcessor();
            outputPipeline = new Pipeline();
        }

        public void Run()
        {
            if (sites.Count == 0)
            {
                Console.WriteLine("Download Manager queue is empty!");
                return;
            }

            Queue<RawPage> rawPages = new Queue<RawPage>();
            List<NodeResult> results = new List<NodeResult>();

            while (sites.Count > 0)
            {
                Site site = sites.Dequeue();
                foreach(PageLayout page in site.Pages.Values)
                {
                    RawPage rawPage = downloadManager.Next(new Uri(page.URL + page.Path));

                    rawPages.Enqueue(rawPage);
                }

                Console.Write("|" + string.Concat(Enumerable.Repeat("-", Console.BufferWidth - 1)));

                while (rawPages.Count > 0)
                {
                    RawPage rawPage = rawPages.Dequeue();

                    results = pageProcessor.Next(rawPage, site);

                    outputPipeline.Output(results);
                }

                
            }

        }

        public void AddSite(Site site)
        {
            sites.Enqueue(site);
        }
    }
}
