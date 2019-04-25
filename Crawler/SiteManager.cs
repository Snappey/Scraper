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
            IHG ihg = new IHG(scraper);
            Sites.Add(ihg);

            HolidayInn holidayInn = new HolidayInn(scraper);
            Sites.Add(holidayInn);
            
            CrownePlaza crownePlaza = new CrownePlaza(scraper);
            Sites.Add(crownePlaza);

            HoteldotCom hoteldotCom = new HoteldotCom(scraper);
            Sites.Add(hoteldotCom);

            Booking booking = new Booking(scraper);
            Sites.Add(booking);

            Expedia expedia = new Expedia(scraper);
            Sites.Add(expedia);
        }

        private void RegisterScrapingSites()
        {
            foreach (ISite site in Sites)
            {
                var scrapable = site as IScrapable;
                scrapable?.RegisterPages();
            }
        }

        public List<Hotel> GetData(ISite site, RequestArgs args)
        {
            foreach (ISite isite in Sites)
            {
                if (isite.Equals(site))
                {
                    return isite.GetData(args);
                }
            }
            return new List<Hotel>();
        }

        public List<List<Hotel>> GetAllData(RequestArgs args)
        {
            List<List<Hotel>> data = new List<List<Hotel>>();
            foreach (ISite site in Sites)
            {
                data.Add(site.GetData(args));
            }
            return data;
        }

        public List<Site> GetSites()
        {
            List<Site> res = new List<Site>();

            foreach (ISite site in Sites)
            {
                res.Add(site.Site);
            }
            return res;
        }

        public Site GetSite(Uri url)
        {
            foreach (ISite site in Sites)
            {
                if (site.Site.URL == url)
                {
                    return site.Site;
                }
            }
            return null;
        }

        public ISite GetSiteInterface(Uri url)
        {
            foreach (ISite site in Sites)
            {
                if (site.Site.URL == url)
                {
                    return site;
                }
            }
            return null;
        }

        public void FlushData()
        {
            scraper.FlushData();
        }
    }
}
