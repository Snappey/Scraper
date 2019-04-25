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
    class HoteldotCom : ISite, IScrapable
    {
        public HoteldotCom(Scraper.Scraper Scraper)
        {
            this.Scraper = Scraper;

            this.Site = new Site(new Uri("https://uk.hotels.com/"));
            this.Site.OutputType = PipelineOutput.Object;
        }

        public List<Hotel> GetData(RequestArgs args)
        {
            RegisterPages(args);

            Scraper.Run(Site);

            var rawResult = Scraper.GetRawResult()[Site];

            List<Hotel> hotels = Hotel.Map(rawResult, args);

            hotels = PostProcess(hotels);

            Scraper.GetRawResult()[Site].Clear(); // Cleanup Memory footprint

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

        private string ConvertDate(DateTime date)
        {
            string[] dateStrings = new string[3];
            var oldDate = date.ToShortDateString().Split('/');

            dateStrings[0] = oldDate[2];
            dateStrings[1] = oldDate[1];
            dateStrings[2] = oldDate[0];

            return dateStrings[0] + "-" + dateStrings[1] + "-" + dateStrings[2];
        }

        public Scraper.Scraper Scraper { get; set; }
        public Site Site { get; set; }
        public void RegisterPages(RequestArgs args = null)
        {
            //https://uk.hotels.com/search.do?
            //q-destination=London,%20England,%20United%20Kingdom&
            //q-check-in=2019-04-26&
            //q-check-out=2019-04-27&
            //q-rooms=1&
            //q-room-0-adults=1&
            //q-room-0-children=0&
            //sort-order=DISTANCE_FROM_LANDMARK&

            UriBuilder uriBuilder = new UriBuilder(Site.URL);

            var param = HttpUtility.ParseQueryString(String.Empty);
            param["q-destination"] = args.City;
            param["q-check-in"] = ConvertDate(args.CheckIn);
            param["q-check-out"] = ConvertDate(args.CheckOut);
            param["q-rooms"] = args.Rooms;
            param["q-room-0-adults"] = args.People;
            param["q-room-0-children"] = "0";
            param["sort-order"] = "DISTANCE_FROM_LANDMARK";

            uriBuilder.Path = "search.do";
            uriBuilder.Query = param.ToString();

            string js = "\r\nsetTimeout(function(){\r\n    dio.widget.InfiniteScroll.instances[0].requestContent();\r\n    setTimeout(function() {\r\n        dio.widget.InfiniteScroll.instances[0].requestContent();\r\n        setTimeout(function() {\r\n            dio.widget.InfiniteScroll.instances[0].requestContent();\r\n            setTimeout(function() {\r\n                dio.widget.InfiniteScroll.instances[0].requestContent();\r\n                setTimeout(function() {\r\n                    dio.widget.InfiniteScroll.instances[0].requestContent();\r\n                    setTimeout(function() {\r\n                        dio.widget.InfiniteScroll.instances[0].requestContent();\r\n                        setTimeout(function() {\r\n                            dio.widget.InfiniteScroll.instances[0].requestContent();\r\n                            setTimeout(function() {\r\n                                dio.widget.InfiniteScroll.instances[0].requestContent();\r\n                                setTimeout(function() {\r\n                                    dio.widget.InfiniteScroll.instances[0].requestContent();\r\n                                    setTimeout(function() {\r\n                                        dio.widget.InfiniteScroll.instances[0].requestContent();\r\n                                        setTimeout(function() {\r\n                                            dio.widget.InfiniteScroll.instances[0].requestContent();\r\n                                            setTimeout(function() {\r\n                                                dio.widget.InfiniteScroll.instances[0].requestContent();\r\n                                                setTimeout(function() {\r\n                                                    dio.widget.InfiniteScroll.instances[0].requestContent();\r\n                                                    setTimeout(function() {\r\n                                                        dio.widget.InfiniteScroll.instances[0].requestContent();\r\n                                                        setTimeout(function() {\r\n                                                            dio.widget.InfiniteScroll.instances[0].requestContent();\r\n                                                            setTimeout(function() {\r\n                                                                dio.widget.InfiniteScroll.instances[0].requestContent();\r\n                                                            }, 750);\r\n                                                        }, 750);\r\n                                                    }, 750);\r\n                                                }, 750);\r\n                                            }, 750);\r\n                                        }, 750);\r\n                                    }, 750);\r\n                                }, 750);\r\n                            }, 750);\r\n                        }, 750);\r\n                    }, 750);\r\n                }, 750);\r\n            }, 750);\r\n        }, 750);\r\n    }, 750);\r\n}, 750);";

            var layout = Site.AddPage(uriBuilder.Uri.PathAndQuery.Substring(1), By.ClassName("hotel"),
                js, "//*[@id='listings']/ol/li/article/section", 30000);

            layout.AddNode(new NodeRequest
            {
                Property = "Name",
                XPath = "//div/h3/a"
            });

            layout.AddNode(new NodeRequest
            {
                Property = "HotelURL",
                XPath = "//div/h3/a",
                Attribute = "href"
            });

            layout.AddNode(new NodeRequest
            {
                Property = "Address",
                XPath = "//div/address/span"
            });

            layout.AddNode(new NodeRequest
            {
                Property = "City",
                XPath = "//div/div/div[1]/div[1]/a"
            });

            layout.AddNode(new NodeRequest
            {
                Property = "PriceL",
                XPath = "//aside/div[1]/a/strong"
            });

            layout.AddNode(new NodeRequest
            {
                Property = "Extras",
                XPath = "//div/h3/a",
                Attribute = "href",
                Recursive = true,
                RecursiveXPath = "//*[@id='overview - section - 4']/ul[1]"
            });

            Scraper.AddSite(Site);
        }
    }
}
// https://uk.hotels.com/search.do?resolved-location=CITY%3A549499%3AUNKNOWN%3AUNKNOWN&destination-id=549499&q-destination=London,%20England,%20United%20Kingdom&q-check-in=2019-04-26&q-check-out=2019-04-27&q-rooms=1&q-room-0-adults=1&q-room-0-children=0&sort-order=DISTANCE_FROM_LANDMARK&sort-lid=549499
