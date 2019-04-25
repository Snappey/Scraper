using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using HtmlAgilityPack;
using Scraper.Structures;

namespace Crawler.Structures
{
    class Hotel
    {
        public string Name;
        public string City;
        public string Address;
        public string Postcode;
        public string Phonenumber;
        public string Country;
        public string ScrapeURL;
        public string HotelURL;
        public string AmtPeople;
        public string AmtRooms;
        public DateTime DateGathered;
        public HotelReservations ReservationData;
        public string Extras;

        /// <summary>
        /// Maps a list of NodeResults potentially more than one hotel and returns a list of hotel objects mapping NodeResult properties to Hotel properties
        /// </summary>
        /// <param name="nodeResults">
        /// List of results from scraping a website
        /// </param>
        /// <param name="args">
        /// Register Args, stores checkin/out data used previously for registering pages with scraper
        /// </param>
        public static List<Hotel> Map(Dictionary<string, List<List<NodeResult>>> nodeResults, RequestArgs args)
        {
            List<Dictionary<string, string>> mappingList = new List<Dictionary<string, string>>();

            /*foreach(List<List<NodeResult>> nodes in nodeResults.Values)
            {
                foreach (List<NodeResult> nodeList in nodes)
                {

                    foreach (NodeResult node in nodeList)
                    {
                       
                        /*if (mappingList.Count <= i) { mappingList.Add(new Dictionary<string, string>()); }

                        

                        HtmlNode first = new HtmlNode(HtmlNodeType.Element, new HtmlDocument(), 0);
                        first.SetAttributeValue("text", "");
                        foreach (HtmlNode htmlNode in node[i].Nodes)
                        {
                            first = htmlNode;
                            break;
                        }

                        Console.WriteLine($"idx: {i}; text: {first.InnerText}; key: {node.Key}");
                        if (res.Attribute != null && first.Attributes[res.Attribute] != null)
                        {
                            mappingList[i].Add(node.Key, first.Attributes[res.Attribute].Value);
                        }
                        else
                        {
                            mappingList[i].Add(node.Key, first.InnerText);
                        }

                      
                    }
                }

            }*/

            List<Hotel> hotels = new List<Hotel>();
            List<FieldInfo> fields = typeof(Hotel).GetFields().ToList();
            Dictionary<string, FieldInfo> fieldNames = new Dictionary<string, FieldInfo>();
            fields.ForEach((field) => { fieldNames.Add(field.Name, field); });

            foreach (KeyValuePair<string, List<List<NodeResult>>> nodeList in nodeResults) // Iterate each page
            {
                foreach (List<NodeResult> rawHotel in nodeList.Value) // Iterate each raw entry (List of NodeResults which should have all the properties for a Hotel)
                {
                    Hotel hotel = new Hotel();
                    hotel.ReservationData = new HotelReservations();
                    HotelReservation reservation = new HotelReservation();


                    foreach (NodeResult result in rawHotel)
                    {
                        if (fieldNames.ContainsKey(result.Property))
                        {
                            if (result.Attribute != null)
                            {
                                var node = result.Nodes.FirstOrDefault();
                                if (node?.Attributes[result.Attribute] != null)
                                {
                                    fieldNames[result.Property].SetValue(hotel, node.Attributes[result.Attribute].Value);
                                }
                                else
                                {
                                    fieldNames[result.Property].SetValue(hotel, result.Nodes.First().InnerText.Trim(new char[] {'\r','\n','\t', ' '}));
                                }
                            }
                            else
                            {
                                fieldNames[result.Property].SetValue(hotel, result.Nodes.First().InnerText.Trim(new char[]{'\r','\n', '\t', ' '}));
                            }  
                        }
                    }

                    reservation.Price = ParseProperty<string>("PriceL", rawHotel);
                    reservation.Currency = ParseProperty<string>("Currency", rawHotel);
                    reservation.CheckIn = args.CheckIn;
                    reservation.CheckOut = args.CheckOut;

                    hotel.ReservationData.AddDate(reservation);
                    hotel.DateGathered = DateTime.Now;
                    hotels.Add(hotel);

                    //rawHotel.Clear();
                }
            }

            return hotels;
        }

        private static T ParseProperty<T>(string key,  List<NodeResult> nodes)
        {
            foreach (NodeResult node in nodes)
            {
                if (node.Property == key)
                {
                    try
                    {
                        return (T)Convert.ChangeType(node.Nodes.First().InnerText.Trim(new char[] {'\r','\n','\t', ' '}), typeof(T));
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
            return default(T);
        }

        public override string ToString()
        {
            return $"{Name} - {City} : {Address}";
        }
    }
}
