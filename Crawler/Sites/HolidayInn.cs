﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Crawler.Interfaces;
using Crawler.Structures;
using Scraper;
using Scraper.Structures;

namespace Crawler.Sites
{
    class HolidayInn : ISite, IScrapable
    {

        public HolidayInn(Scraper.Scraper Scraper)
        {
            this.Scraper = Scraper;

            this.Site = new Site(new Uri("https://www.holidayinn.com/"));
            this.Site.OutputType = PipelineOutput.Object;
        }

        public List<Hotel> GetData()
        {
            Scraper.Run(Site);

            var test = Scraper.GetResult(Site);

            return null;
        }

        public Scraper.Scraper Scraper { get; set; }
        public Site Site { get; set; }

        public void RegisterPages()
        {
            if (Site != null)
            {
                UriBuilder uriBuilder = new UriBuilder(Site.URL);

                var param = HttpUtility.ParseQueryString(String.Empty);
                param["qRms"] = "1";
                param["qAdlt"] = "2";
                param["qDest"] = "London, United Kingdom"; // Travel.com "Londons" City ID
                param["qRad"] = "30"; // Radius to search
                param["qRdU"] = "mi"; // Distance Units
                //param["currency"] = "GBP";
                param["qCiD"] = "22"; // Check in Date
                param["qCiMy"] = "32019"; // Check in Month/Year (Month - 1)
                param["qCoD"] = "23"; // Check out Date
                param["qCoMy"] = "32019"; // Check out Money/Year (Month - 1)
                param["qPage"] = "1";

                uriBuilder.Path = "hotels/gb/en/find-hotels/hotel/list";
                uriBuilder.Query = param.ToString();

                var js =
                    "angular.element(document.evaluate('//*[@id=\"applicationWrapper\"]/div[2]/div/div/div[9]/div[2]', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue).scope().hotelList.bottomInView();";

                var layout = Site.AddPage(uriBuilder.Uri.PathAndQuery.Substring(1), "topOfPage",
                    String.Concat(Enumerable.Repeat(js, 20)), 850);

                layout.AddNode(new NodeRequest
                {
                    Property = "Name",
                    XPath = "//div/hotel-row/div[1]/div/div/hotel-details/div/div/div[1]/div/div/span[1]/a"
                });
                layout.AddNode(new NodeRequest
                {
                    Property = "Location",
                    XPath = "//div/hotel-row/div[1]/div/div/hotel-details/div/div/div[1]/div/div/span[2]/a"
                });
                layout.AddNode(new NodeRequest
                {
                    Property = "Address",
                    XPath =
                        "//div/hotel-row/div[1]/div/div/hotel-details/div/div/div[2]/div[1]/hotel-address/div/div/span[1]"
                });
                layout.AddNode(new NodeRequest
                {
                    Property = "Postcode",
                    XPath =
                        "//div/hotel-row/div[1]/div/div/hotel-details/div/div/div[2]/div[1]/hotel-address/div/div/span[1]"
                });
                layout.AddNode(new NodeRequest
                {
                    Property = "Country",
                    XPath =
                        "//div/hotel-row/div[1]/div/div/hotel-details/div/div/div[2]/div[1]/hotel-address/div/div/span[3]/span[2]"
                });

                Scraper.AddSite(Site);
            }
        }
    }
}