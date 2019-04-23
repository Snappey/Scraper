using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Crawler.Interfaces;
using Crawler.Structures;
using OpenQA.Selenium;
using Scraper.Structures;

namespace Crawler.Sites
{
    // https://secure.rezserver.com/hotels/results
    // e.g. query: https://secure.rezserver.com/hotels/results/?check_in=04%2F17%2F2019&check_out=04%2F18%2F2019&rooms=1&adults=2&city_id=800013148&page=1&currency=GBP
    class Travel : ISite, IScrapable
    {

        public Travel(Scraper.Scraper scraper)
        {
            this.Scraper = scraper;

            this.Site = new Site(new Uri("https://secure.rezserver.com/"));
        }

        public List<Hotel> GetData()
        {
            Scraper.Run(Site);

            return new List<Hotel>();
        }

        public Scraper.Scraper Scraper { get; set; }
        public Site Site { get; set; }
        public void RegisterPages(RegisterArgs args)
        {
            if (Site != null)
            {

                UriBuilder uriBuilder = new UriBuilder(Site.URL);

                var param = HttpUtility.ParseQueryString(String.Empty);
                param["rooms"] = "1";
                param["adults"] = "2";
                param["city_id"] = "800013148"; // Travel.com "Londons" City ID
                param["currency"] = "GBP";
                param["check_in"] = "22/04/2019";
                param["check_out"] = "23/04/2018";
                param["page"] = "1";

                for (int i = 1; i < 10; i++)
                {
                    param["page"] = i.ToString();
                    uriBuilder.Path = "hotels/results/";
                    uriBuilder.Query = param.ToString();
                    var layout = Site.AddPage(uriBuilder.Uri.PathAndQuery.Substring(1), By.Id("rs_main_css"));

                    layout.AddNode(new NodeRequest { Property = "Name", XPath = "//div/article/div[2]/div[1]/div[2]/a" });
                }


                Scraper.AddSite(Site);
            }
            else
            {
                throw new Exception("Site was not initialised in class with a valid url");
            }
        }
    }
}
