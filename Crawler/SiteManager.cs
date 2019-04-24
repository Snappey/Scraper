using System;
using System.Collections.Generic;
using System.Text;
using Crawler.Interfaces;
using Crawler.Sites;
using Crawler.Structures;
using Scraper;
using Scraper.Structures;

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
            //Travel travel = new Travel(scraper);
            //Sites.Add(travel);

            //IHG ihg = new IHG(scraper);
            //Sites.Add(ihg);

            HolidayInn holidayInn = new HolidayInn(scraper);
            Sites.Add(holidayInn);

            //RegisterScrapingSites();
        }

        private void RegisterScrapingSites()
        {
            foreach (ISite site in Sites)
            {
                var scrapable = site as IScrapable;
                scrapable?.RegisterPages();
            }
        }

        public List<Hotel> GetData(ISite site)
        {
            foreach (ISite isite in Sites)
            {
                if (isite.Equals(site))
                {
                    return isite.GetData();
                }
            }
            return new List<Hotel>();
        }

        public List<List<Hotel>> GetAllData()
        {
            List<List<Hotel>> data = new List<List<Hotel>>();
            foreach (ISite site in Sites)
            {
                data.Add(site.GetData());
            }
            return data;
        }

        public List<Site> GetSites()
        {
            List<Site> res = new List<Site>();

            foreach (ISite site in Sites)
            {
                if ((IScrapable) site != null)
                {
                    res.Add((site as IScrapable).Site);
                }
            }
            return res;
        }

        public void FlushData()
        {
            scraper.FlushData();
        }
    }
}
