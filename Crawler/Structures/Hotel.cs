﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using HtmlAgilityPack;
using Scraper.Structures;

namespace Crawler.Structures
{
    /// <summary>
    /// Main data structure used for storing structured data gathered from various websites
    /// </summary>
    public class Hotel
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
                        if (fieldNames.ContainsKey(result.Property)) // Map each results property to the classes field
                        {
                            if (result.Attribute != null)
                            {
                                var node = result.Nodes.FirstOrDefault(); // maps attributes for example href to field values
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

                    Program.App.Log($"[{DateTime.Now.ToShortTimeString()}] {hotel.ScrapeURL}: Found, {hotel.Name}, {hotel.Address}");

                    reservation.Price = ParseProperty<string>("PriceL", rawHotel);
                    reservation.Currency = ParseProperty<string>("Currency", rawHotel);
                    reservation.CheckIn = args.CheckIn;
                    reservation.CheckOut = args.CheckOut; // Reservation initialisation and data passing

                    hotel.ReservationData.AddDate(reservation);
                    hotel.DateGathered = DateTime.Now;
                    hotels.Add(hotel);
                }
            }
            return hotels;
        }

        /// <summary>
        /// Generic function that allows for a property to be converted into any specifc type
        /// </summary>
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
