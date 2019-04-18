using System;
using System.Collections.Generic;
using System.Text;
using Crawler.Interfaces;
using Crawler.Structures;
using Scraper.Structures;

namespace Crawler.Sites
{
    // https://secure.rezserver.com/hotels/results
    // e.g. query: https://secure.rezserver.com/hotels/results/?check_in=04%2F17%2F2019&check_out=04%2F18%2F2019&rooms=1&adults=2&city_id=800013148&page=1&currency=GBP
    class Travel : ISite, IScrapable
    {
        private Scraper.Scraper scraper;
        private List<Hotel> Data;

        public Travel(Scraper.Scraper scraper)
        {
            this.scraper = scraper;

            this.Site = new Site(new Uri("https://secure.rezserver.com/"));
        }

        public List<Hotel> GetData()
        {
            scraper.Run();

            return Data;
        }

        public Site Site { get; set; }
        public void RegisterPages()
        {
            if (Site != null)
            {
                var layout = Site.AddPage(
                    "hotels/results/?check_in=04%2F17%2F2019&check_out=04%2F18%2F2019&rooms=1&adults=2&city_id=800013148&page=1&currency=GBP");

                layout.AddNode(new NodeRequest{Property = "Name", XPath = "//div/article/div[2]/div[1]/div[2]/a" });

                scraper.AddSite(Site);
            }
            else
            {
                throw new Exception("Site was not initialised in class with a valid url");
            }
        }
    }
}
