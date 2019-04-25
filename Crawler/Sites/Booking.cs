﻿using System;
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
    class Booking : ISite, IScrapable
    {
        public Booking(Scraper.Scraper Scraper)
        {
            this.Scraper = Scraper;

            this.Site = new Site(new Uri("https://www.booking.com/"));
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

        private string[] ConvertDate(DateTime date)
        {
            string[] dateStrings = new string[3];

            dateStrings[0] = date.Day.ToString();
            dateStrings[1] = date.Month.ToString();
            dateStrings[2] = date.Year.ToString();

            return dateStrings;
        }

        public Scraper.Scraper Scraper { get; set; }
        public Site Site { get; set; }
        public void RegisterPages(RequestArgs args = null)
        {
            //https://www.booking.com/searchresults.en-gb.html?
            //checkin_month=4&
            //checkin_monthday=26&
            //checkin_year=2019&
            //checkout_month=4&
            //checkout_monthday=27&
            //checkout_year=2019&
            //class_interval=1&
            //group_adults=1&
            //group_children=0&
            //no_rooms=1&
            //order=distance_from_search&
            //room1=A&
            //ss=Central%20London%2C%20London%2C%20Greater%20London%2C%20United%20Kingdom&
            //rows=50& (Amount of Rows on page)
            //offset=60 (Offset for the page)

            UriBuilder uriBuilder = new UriBuilder(Site.URL);

            var param = HttpUtility.ParseQueryString(String.Empty);

            string[] checkin = ConvertDate(args.CheckIn);
            string[] checkout = ConvertDate(args.CheckOut);
            param["checkin_month"] = checkin[1];
            param["checkin_monthday"] = checkin[0];
            param["checkin_year"] = checkin[2];
            param["checkout_month"] = checkout[1];
            param["checkout_monthday"] = checkout[0];
            param["checkout_year"] = checkout[2];

            param["group_adults"] = "1";
            param["group_children"] = "0";
            param["no_rooms"] = "1";
            param["map"] = "0";
            param["ss"] = "London";
            param["rows"] = "50";
            param["order"] = "distance_from_search";


            for (int i = 0; i < 50 * 5; i = i + 50)
            {
                uriBuilder.Path = "searchresults.en-gb.html";
                param["offset"] = i.ToString();

                uriBuilder.Query = param.ToString();

                var layout = Site.AddPage(uriBuilder.Uri.PathAndQuery.Substring(1), By.Id("hotellist_inner"), xPathFilter: "//*[@id='hotellist_inner']/div/div[2]");

                layout.AddNode(new NodeRequest
                {
                    Property = "Name",
                    XPath = "//div[1]/div[1]/h3/a/span[1]",
                });

                layout.AddNode(new NodeRequest
                {
                    Property = "HotelURL",
                    XPath = "//div[1]/div[1]/h3/a",
                    Attribute = "href",
                });

                layout.AddNode(new NodeRequest
                {
                    Property = "Address",
                    XPath = "//div[1]/div[1]/h3/a",
                    Attribute = "href",
                    Recursive = true,
                    RecursiveXPath = "//*[@id='showMap2']/span[2]",
                });

                layout.AddNode(new NodeRequest
                {
                    Property = "City",
                    XPath = "//div[1]/div[1]/div/a"
                });

                layout.AddNode(new NodeRequest
                {
                    Property = "PriceL",
                    XPath = "//div[2]/div/div/div[1]/div/div[2]/div/strong/b"
                });

                layout.AddNode(new NodeRequest
                {
                    Property = "Extras",
                    XPath = "//div[1]/div[1]/h3/a",
                    Attribute = "href",
                    Recursive = true,
                    RecursiveXPath = "//*[@id='hp_facilities_box']/div[4]/div[11]/ul"
                });
            }
           

            Scraper.AddSite(Site);
        }
    }
}