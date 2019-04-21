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

        private delegate void DownloadedHandler(object sender, EventArgs e);

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
                Console.WriteLine("Site is not registered in Scraper List!");
                return;
            }

            Queue<RawPage> rawPages = new Queue<RawPage>();
            List<NodeResult> results = new List<NodeResult>();

            foreach (PageLayout page in site.Pages.Values)
            {
                RawPage rawPage = downloadManager.Next(new Uri(page.URL + page.Path), page.SearchElement, page.JSExecution, page.PageDelay);
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
                PageCompleted.Invoke(results, EventArgs.Empty);
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
                    RawPage rawPage = downloadManager.Next(new Uri(page.URL + page.Path), page.SearchElement, page.JSExecution, page.PageDelay);
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

        public List<NodeResult> GetResult(Site site, string page="", string property="")
        {
            var data = outputPipeline.Data;
            List<NodeResult> result = new List<NodeResult>();


            if (data.ContainsKey(site))
            {
                if (page == "")
                {
                    foreach (KeyValuePair<string, Dictionary<string, NodeResult>> keyValue in data[site])
                    {
                        foreach (NodeResult nodeResult in keyValue.Value.Values)
                        {
                            result.Add(nodeResult);
                        }
                    }
                }
                else
                {
                    if (data[site].ContainsKey(page))
                    {
                        if (property == "")
                        {
                            foreach (KeyValuePair<string, NodeResult> keyValue in data[site][page])
                            {
                                result.Add(keyValue.Value);
                            }
                        }
                        else
                        {
                            if (data[site][page].ContainsKey(property))
                            {
                                result.Add(data[site][page][property]);
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}
