using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
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
        public static List<Hotel> Map(List<NodeResult> nodeResults)
        {
            List<Dictionary<string, string>> mappingList = new List<Dictionary<string, string>>();

            foreach(NodeResult node in nodeResults)
            {
                for (var i = 0; i < node.Nodes.Count; i++)
                {
                    if(mappingList.Count <= i) { mappingList.Add(new Dictionary<string, string>()); }
                    mappingList[i].Add(node.Property, node.Nodes[i].InnerText); // Flatten NodeResult based on index into corresponding dictionarie
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

                    if (field.Name == "PriceL")
                    {
                        float price = 0;
                        bool successParse = float.TryParse(node[field.Name], out price);

                        if (successParse)
                        {
                            reservation.Price = price;
                        }
                    }
                }            
                hotel.DateGathered = DateTime.Now;
                hotels.Add(hotel);
            }

            return hotels;
        }


        public override string ToString()
        {
            return $"{this.Name} - {this.City} : {this.Address}";
        }
    }
}
