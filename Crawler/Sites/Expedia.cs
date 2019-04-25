using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Crawler.Interfaces;
using Crawler.Structures;
using OpenQA.Selenium;
using Scraper;
using Scraper.Structures;

namespace Crawler.Sites
{
    class Expedia : ISite, IScrapable
    {

        public Expedia(Scraper.Scraper scraper)
        {
            this.Scraper = scraper;

            this.Site = new Site(new Uri("https://www.expedia.co.uk"));
            this.Site.OutputType = PipelineOutput.Object;
        }

        public List<Hotel> GetData(RequestArgs args)
        {

            RegisterPages(args);

            Scraper.Run(Site);

            var rawResult = Scraper.GetRawResult()[Site]; // TODO: Processing section, 

            List<Hotel> hotels = Hotel.Map(rawResult, args); // TODO: pass base site url for mapping

            hotels = PostProcess(hotels);

            Scraper.GetRawResult()[Site].Clear();

            return hotels;
        }

        private List<Hotel> PostProcess(List<Hotel> hotels)
        {
            List<Hotel> newHotels = new List<Hotel>();

            foreach (Hotel hotel in hotels)
            {
                hotel.ScrapeURL = Site.URL.Host;

                newHotels.Add(hotel);
            }

            return newHotels;
        }

        public Scraper.Scraper Scraper { get; set; }
        public Site Site { get; set; }
        public void RegisterPages(RequestArgs args = null)
        {
            //https://www.expedia.co.uk/Hotel-Search
            //?destination=London+(and+vicinity),+England,+United+Kingdom&s
            //startDate=24/04/2019&
            //endDate=25/04/2019&
            //adults=1&
            //regionId=178279&
            //lodging=hotel&
            //sort=distance

            UriBuilder uriBuilder = new UriBuilder(Site.URL);

            var param = HttpUtility.ParseQueryString(String.Empty);
            param["destination"] = args.City;
            param["startDate"] = args.CheckIn.ToShortDateString();
            param["endDate"] = args.CheckOut.ToShortDateString();
            param["adults"] = args.People;
            param["lodging"] = "hotel";
            param["sort"] = "distance";
            param["page"] = "1";

            for (int i = 1; i < 5; i++)
            {
                param["page"] = i.ToString();
                uriBuilder.Path = "Hotel-Search";
                uriBuilder.Query = param.ToString();

                var page = Site.AddPage(uriBuilder.Uri.PathAndQuery.Substring(1), By.Id("resultsContainer"), xPathFilter: "//article", pageDelay: 50);

                page.AddNode(new NodeRequest
                {
                    Property = "Name",
                    XPath = "//h3"
                });

                page.AddNode(new NodeRequest
                {
                    Property = "HotelURL",
                    XPath = "//div[2]/div/a",
                    Attribute = "href"
                });

                page.AddNode(new NodeRequest
                {
                    Property = "Address",
                    XPath = "//div[2]/div/a",
                    Attribute = "href",
                    Recursive = true,
                    RecursiveXPath = "//section/div[1]/div/a/span[1]"
                });

                page.AddNode(new NodeRequest
                {
                    Property = "City",
                    XPath = "//*[contains(concat(' ',normalize-space(@class),' '),' neighborhood secondary ')]" // Returns all elements with the given class id
                });

                page.AddNode(new NodeRequest
                {
                    Property = "Phonenumber",
                    XPath = "//div[2]/div/div[1]/div[2]/ul[1]/li[12]/span"
                });

                page.AddNode(new NodeRequest
                {
                    Property = "PriceL",
                    XPath = "//*[contains(concat(' ',normalize-space(@class),' '),' actualPrice ')]"
                });

                page.AddNode(new NodeRequest
                {
                    Property = "Extras",
                    XPath = "//div[2]/div/a",
                    Attribute = "href",
                    Recursive = true,
                    RecursiveXPath = "/html/body/div[3]/div[9]/section/div[15]/div[2]/div[1]/div"
                });

            }

            Scraper.AddSite(Site);
        }
    }
}
