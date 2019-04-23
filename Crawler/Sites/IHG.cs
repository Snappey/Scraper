using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Crawler.Interfaces;
using Crawler.Structures;
using OpenQA.Selenium;
using Scraper;
using Scraper.Structures;

namespace Crawler.Sites
{
    class IHG : ISite, IScrapable
    {
        // https://www.ihg.com/
        // e.g. query: https://www.ihg.com/hotels/gb/en/find-hotels/hotel/list?qDest=London,%20United%20Kingdom&
        // qCiMy=32019
        // qCiD=21
        // qCoMy=32019
        // qCoD=22
        // qAdlt=2
        // qChld=0
        // qRms=1
        // qRtP=6CBARC
        // qAkamaiCC=GB
        // qSrt=sDD
        // qBrs=re.ic.in.vn.cp.vx.hi.ex.rs.cv.sb.cw.ma.ul.ki.va
        // srb_u=0
        // qRad=30
        // qRdU=mi

        public IHG(Scraper.Scraper scraper)
        {
            this.Scraper = scraper;

            this.Site = new Site(new Uri("https://www.ihg.com/"));
            this.Site.OutputType = PipelineOutput.Object;
        }

        public List<Hotel> GetData()
        {
            RequestArgs args = new RequestArgs
            {
                CheckIn = DateTime.Now.AddDays(1),
                CheckOut = DateTime.Now.AddDays(2),
            };

            RegisterPages(args);

            Scraper.Run(Site);

            List<Hotel> hotels = Hotel.Map(Scraper.GetRawResult()[Site], args); // TODO: pass base site url for mapping

            return hotels; // TODO: dont return null return data
        }

        private string[] ConvertDate(DateTime date)
        {
            string[] dateStrings = new string[2];

            dateStrings[0] = date.Day.ToString();
            dateStrings[1] = (date.Month - 1).ToString() + (date.Year).ToString();

            return dateStrings;
        }

        public Scraper.Scraper Scraper { get; set; }
        public Site Site { get; set; }
        public void RegisterPages(RequestArgs args)
        {
            if (Site != null)
            {
                UriBuilder uriBuilder = new UriBuilder(Site.URL);

                var param = HttpUtility.ParseQueryString(String.Empty);
                param["qRms"] = "1";
                param["qAdlt"] = "2";
                param["qDest"] = "London, United Kingdom"; // Travel.com "Londons" City ID
                param["qRad"] = "300"; // Radius to search
                param["qRdU"] = "mi"; // Distance Units
                //param["currency"] = "GBP";

                string[] checkin = new string[2];
                string[] checkout = new string[2];
                if (args != null)
                {
                    checkin = ConvertDate(args.CheckIn);
                    checkout = ConvertDate(args.CheckOut);
                }

                param["qCiD"] = checkin[0]; // Check in Date
                param["qCiMy"] = checkin[1]; // Check in Month/Year (Month - 1)
                param["qCoD"] = checkout[0]; // Check out Date
                param["qCoMy"] = checkout[1]; // Check out Money/Year (Month - 1)

                uriBuilder.Path = "hotels/gb/en/find-hotels/hotel/list";
                uriBuilder.Query = param.ToString();


                var js =
                    "angular.element(document.evaluate('//*[@id=\"applicationWrapper\"]/div[2]/div/div/div[9]/div[2]', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue).scope().hotelList.bottomInView();";
                
                var layout = Site.AddPage(uriBuilder.Uri.PathAndQuery.Substring(1), By.Id("topOfPage"), String.Concat(Enumerable.Repeat(js, 60)), "//div/hotel-row", 850);

                layout.AddNode(new NodeRequest
                {
                    Property = "Name",
                    XPath = "//div[1]/div/div/hotel-details/div/div/div[1]/div/div/span[1]/a"
                });
                layout.AddNode(new NodeRequest
                {
                    Property = "HotelURL",
                    XPath = "//div[1]/div/div/hotel-details/div/div/div[1]/div/div/span[1]/a",
                    Attribute = "href",
                });

                layout.AddNode(new NodeRequest
                {
                    Property = "City",
                    XPath = "//div[1]/div/div/hotel-details/div/div/div[1]/div/div/span[2]/a"
                });
                layout.AddNode(new NodeRequest
                {
                    Property = "Address",
                    XPath = "//div[1]/div/div/hotel-details/div/div/div[2]/div[1]/hotel-address/div/div/span[1]"
                });
                layout.AddNode(new NodeRequest
                {
                    Property = "Postcode",
                    XPath = "//div[1]/div/div/hotel-details/div/div/div[2]/div[1]/hotel-address/div/div/span[3]/span[1]"
                });
                layout.AddNode(new NodeRequest
                {
                    Property = "Country",
                    XPath = "//div[1]/div/div/hotel-details/div/div/div[2]/div[1]/hotel-address/div/div/span[3]/span[2]"
                });
                layout.AddNode(new NodeRequest
                {
                    Property = "Phonenumber",
                    XPath = "//div[1]/div/div/hotel-details/div/div/div[2]/div[1]/hotel-phone-number/div[2]/div/div/a"
                });
                layout.AddNode(new NodeRequest
                {
                    Property = "PriceL",
                    XPath = "//div[1]/div/div/div[3]/div[1]/div/div/span[1]"
                });
                layout.AddNode(new NodeRequest
                {
                    Property = "PriceS",
                    XPath = "//div[1]/div/div/div[3]/div[1]/div/div/span[2]"
                });
                layout.AddNode(new NodeRequest
                {
                    Property = "Available",
                    XPath = "//div[1]/div/div/div[3]/div/div/div[1]"
                });
                layout.AddNode(new NodeRequest
                {
                    Property = "Currency",
                    XPath = "//div[1]/div/div/div[3]/div[1]/div/span[2]"
                });

                Scraper.AddSite(Site);
            }
        }
    }
}
