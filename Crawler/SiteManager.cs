using System;
using System.Collections.Generic;
using System.Text;
using Crawler.Interfaces;
using Crawler.Sites;
using Scraper;

namespace Crawler
{
    class SiteManager
    {
        private List<ISite> Sites = new List<ISite>();
        private Scraper.Scraper scraper;

        public SiteManager()
        {
            scraper = new Scraper.Scraper();
        }

        public void Register()
        {
            Travel travel = new Travel(scraper);
            Sites.Add(travel);


            RegisterScrapingSites();
        }

        private void RegisterScrapingSites()
        {
            foreach (ISite site in Sites)
            {
                var scrapable = site as IScrapable;
                scrapable?.RegisterPages();
            }
        }

        public void GetData(ISite site)
        {
            // Just get data froma single site instead of all
        }

        public void GetAllData()
        {
            foreach (ISite site in Sites)
            {
                site.GetData();
            }
        }
    }
}
