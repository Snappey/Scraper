using Scraper.Structures;
using System;
using System.Collections.Generic;

namespace Scraper
{
    public class Scraper
    {
        private Downloader downloadManager;
        private PageProcessor pageProcessor;

        private Queue<Site> sites = new Queue<Site>();

        public Scraper()
        {
            downloadManager = new Downloader();
            pageProcessor = new PageProcessor();
        }

        public void Start()
        {
            if (sites.Count == 0)
            {
                Console.WriteLine("Download Manager queue is empty!");
                return;
            }

            Queue<RawPage> rawPages = new Queue<RawPage>();

            while (sites.Count > 0)
            {
                Site site = sites.Dequeue();
                foreach(PageLayout page in site.Pages.Values)
                {
                    RawPage rawPage = downloadManager.Next(new Uri(page.URL + page.Path));
                    rawPage.Site = site;

                    rawPages.Enqueue(rawPage);
                }

                while(rawPages.Count > 0)
                {
                    RawPage rawPage = rawPages.Dequeue();

                    pageProcessor.Next(rawPage, site);
                }

            }

        }

        public void AddSite(Site site)
        {
            sites.Enqueue(site);
        }
    }
}
