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
        public DateTime DateGathered;
        public HotelReservations ReservationData;
        public List<string> Extras;

        /// <summary>
        /// Maps a list of NodeResults potentially more than one hotel and returns a list of hotel objects mapping NodeResult properties to Hotel properties
        /// </summary>
        /// <param name="nodeResults">
        /// List of results from scraping a website
        /// </param>
        /// <param name="args">
        /// Register Args, stores checkin/out data used previously for registering pages with scraper
        /// </param>
        public static List<Hotel> Map(Dictionary<string, Dictionary<string, List<NodeResult>>> nodeResults, RegisterArgs args)
        {
            List<Dictionary<string, string>> mappingList = new List<Dictionary<string, string>>();

            foreach(Dictionary<string, List<NodeResult>> node in nodeResults.Values)
            {
                foreach (KeyValuePair<string, List<NodeResult>> keyValue in node)
                {
                    for (var i = 0; i < keyValue.Value.Count; i++) // TODO: This wont work anymore, we need to iterate and fill up based on the property
                    {
                        if (mappingList.Count <= i) { mappingList.Add(new Dictionary<string, string>()); }

                        HtmlNode first = new HtmlNode(HtmlNodeType.Element, new HtmlDocument(), 0);
                        first.SetAttributeValue("text", "");
                        foreach (HtmlNode htmlNode in keyValue.Value[i].Nodes)
                        {
                            first = htmlNode;
                            break;
                        }

                        mappingList[i].Add(keyValue.Key, first.InnerText); // Flatten NodeResult based on index into corresponding dictionarie
                    }
                }

            }     

            List<Hotel> hotels = new List<Hotel>();
            List<FieldInfo> fields = typeof(Hotel).GetFields().ToList();
            Dictionary<string, FieldInfo> fieldNames = new Dictionary<string, FieldInfo>();

            fields.ForEach((field) => { fieldNames.Add(field.Name, field); });

            foreach (Dictionary<string, string> node in mappingList)
            {
                Hotel hotel = new Hotel();
                hotel.ReservationData = new HotelReservations();
                HotelReservation reservation = new HotelReservation();

                foreach (FieldInfo field in fields)
                {
                    if (node.ContainsKey(field.Name))
                    {
                        field.SetValue(hotel, node[field.Name]);
                    }
                }

                reservation.Price = ParseProperty<float>("PriceL", node);
                reservation.Currency = ParseProperty<string>("Currency", node);
                reservation.CheckIn = args.CheckIn;
                reservation.CheckOut = args.CheckOut;

                hotel.ReservationData.AddDate(reservation);
                hotel.DateGathered = DateTime.Now;
                hotels.Add(hotel);
            }
            return hotels;
        }

        private static T ParseProperty<T>(string key,  Dictionary<string, string> node)
        {
            if (node.TryGetValue(key, out var val))
            {
                try
                {
                    return (T)Convert.ChangeType(val, typeof(T));
                }
                catch
                {
                    // ignored
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
