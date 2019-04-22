using System;
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
            RegisterArgs args = new RegisterArgs
            {
                CheckIn = DateTime.Now.AddDays(1),
                CheckOut = DateTime.Now.AddDays(2),
            };

            RegisterPages(args);

            Scraper.Run(Site);

            var results = Scraper.GetResult(Site);

            List<Hotel> hotels = Hotel.Map(results, args); // TODO: Pass arguments from the scraper to the map results (Used for check in/out dates)

            return hotels;
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

        public void RegisterPages(RegisterArgs args)
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

                var layout = Site.AddPage(uriBuilder.Uri.PathAndQuery.Substring(1), "topOfPage",
                    String.Concat(Enumerable.Repeat(js, 2)), 850);

                layout.AddNode(new NodeRequest
                {
                    Property = "Name",
                    XPath = "//div/hotel-row/div[1]/div/div/hotel-details/div/div/div[1]/div/div/span[1]/a"
                });
                layout.AddNode(new NodeRequest
                {
                    Property = "City",
                    XPath = "//div/hotel-row/div[1]/div/div/hotel-details/div/div/div[1]/div/div/span[2]/a"
                });
                layout.AddNode(new NodeRequest
                {
                    Property = "Address",
                    XPath = "//div/hotel-row/div[1]/div/div/hotel-details/div/div/div[2]/div[1]/hotel-address/div/div/span[1]"
                });
                layout.AddNode(new NodeRequest
                {
                    Property = "Postcode",
                    XPath = "//div/hotel-row/div[1]/div/div/hotel-details/div/div/div[2]/div[1]/hotel-address/div/div/span[3]/span[1]"
                });
                layout.AddNode(new NodeRequest
                {
                    Property = "Country",
                    XPath = "//div/hotel-row/div[1]/div/div/hotel-details/div/div/div[2]/div[1]/hotel-address/div/div/span[3]/span[2]"
                });
                layout.AddNode(new NodeRequest
                {
                    Property = "Phonenumber",
                    XPath = "//div/hotel-row/div[1]/div/div/hotel-details/div/div/div[2]/div[1]/hotel-phone-number/div[2]/div/div/a"
                });
                layout.AddNode(new NodeRequest
                {
                    Property = "PriceL",
                    XPath = "//div/hotel-row/div[1]/div/div/div[3]/div[1]/div/div/span[1]"
                });
                layout.AddNode(new NodeRequest
                {
                    Property = "PriceS",
                    XPath = "//div/hotel-row/div[1]/div/div/div[3]/div[1]/div/div/span[2]"
                });
                layout.AddNode(new NodeRequest
                {
                    Property = "Currency",
                    XPath = "//div/hotel-row/div[1]/div/div/div[3]/div[1]/div/span[2]"
                });

                Scraper.AddSite(Site);
            }
        }
    }
}
