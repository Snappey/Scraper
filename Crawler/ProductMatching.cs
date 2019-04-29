using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Crawler.Structures;
using System.Text.RegularExpressions;

namespace Crawler
{
    class ProductMatching
    {
        private Storage connection;
        private List<Hotel> preMatchHotels = new List<Hotel>();
        private List<Hotel> postMatchHotels = new List<Hotel>(); // List of a list of hotels that are the same
        private Dictionary<Hotel, bool> checkHotel = new Dictionary<Hotel, bool>();

        public ProductMatching(Storage connection)
        {
            this.connection = connection;
        }

        public void Start(RequestArgs args)
        {
            if (args.Name != String.Empty)
            {
                preMatchHotels = connection.GetHotelsByName(args);
            }
            else
            {
                preMatchHotels = connection.GetHotelsByCity(args);
            }
            
            Loop();
            AddSingleHotels();
        }

        public List<Hotel> GetResult()
        {
            return postMatchHotels;
        }

        public void Reset()
        {
            preMatchHotels.Clear();
            postMatchHotels.Clear();
            checkHotel.Clear();
        }

        private void AddSingleHotels()
        {
            foreach (KeyValuePair<Hotel, bool> hotelChecked in checkHotel)
            {
                if (hotelChecked.Value == false && postMatchHotels.Contains(hotelChecked.Key) == false)
                {
                    postMatchHotels.Add(hotelChecked.Key);
                }
            }
        }

        private void Loop()
        {
            foreach (Hotel originalHotel in preMatchHotels)
            {
                foreach (Hotel compareHotel in preMatchHotels)
                {
                    if (originalHotel.Equals(compareHotel) || originalHotel.ScrapeURL.Equals(compareHotel.ScrapeURL)) { continue; } // Skip comparing the same hotels
                    double similarity = Compare(originalHotel, compareHotel);

                    if (similarity > .6)
                    {
                        if (checkHotel.ContainsKey(originalHotel) == false)
                        {
                            checkHotel.Add(originalHotel, true);
                            if (checkHotel.ContainsKey(compareHotel) == false)
                            {
                                checkHotel.Add(compareHotel, true);

                                if (postMatchHotels.Contains(originalHotel))
                                {
                                    var idx = postMatchHotels.IndexOf(originalHotel);
                                    Collate(postMatchHotels[idx], compareHotel);
                                }
                                else
                                {
                                    Collate(originalHotel, compareHotel);
                                    postMatchHotels.Add(originalHotel);
                                }
                            }                        
                        }
                        //Console.WriteLine($" Compare: {compareHotel.ScrapeURL} | Original: {originalHotel.ScrapeURL} | {compareHotel.Name} : {originalHotel.Name} - {similarity}");
                        else
                        {
                            Collate(originalHotel, compareHotel);
                        }
                    }
                }

                if (checkHotel.ContainsKey(originalHotel) == false)
                {
                    checkHotel.Add(originalHotel, false);
                }
            }
        }

        private double Compare(Hotel originalHotel, Hotel compareHotel)
        {
            double similarity = 0;
      
            { // NameSection
                var originalHotelName = originalHotel.Name;
                var compareHotelName = compareHotel.Name;

                if (originalHotelName == compareHotelName)
                {
                    similarity += .5;
                }
                else
                {
                    var originalHotelNameSplit = originalHotelName.Split(' ');
                    int matches = 0;

                    foreach (string originalComponent in originalHotelNameSplit)
                    {
                        if (originalComponent.Equals("Hotel")) { continue; } // The component 'hotel' isnt very unique
                        if (compareHotelName.Contains(originalComponent))
                        {
                            matches++;
                        }
                    }

                    similarity += .5 * (matches / originalHotelNameSplit.Length); // calculate threshold based on how many components match

                    //int distance = LevenshteinDistance(originalHotelName, compareHotelName);

                    //similarity += (.5 - (0.05 * distance));

                    //Console.WriteLine($"{originalHotelName} : {compareHotelName} = {distance} ");
                }
            }

           
            { // CitySection
                var originalCity = originalHotel.City;
                var compareCity = compareHotel.City;

                if (originalCity == compareCity)
                {
                    similarity += .1;
                }
            }

            { // AddressSection
                // ^([A-Za-z][A-Ha-hK-Yk-y]?[0-9][A-Za-z0-9]? ?[0-9][A-Za-z]{2}|[Gg][Ii][Rr] ?0[Aa]{2})$
                // Provided by UK Government for validating postcodes
                Regex postcodeRegex = new Regex("([A-Za-z][A-Ha-hK-Yk-y]?[0-9][A-Za-z0-9]? ?[0-9][A-Za-z]{2}|[Gg][Ii][Rr] ?0[Aa]{2})");

                var originalAddress = originalHotel.Address;
                var compareAddress = compareHotel.Address;

                if (originalAddress == compareAddress)
                {
                    similarity += .4;
                }
                else
                {
                    var originalSplit = originalAddress.Split(' ');
                    int matches = 0;

                    foreach (string component in originalSplit)
                    {
                        if (component.Contains(component))
                        {
                            matches++;
                        }
                    }

                    similarity += .4 * (matches / originalSplit.Length);
                }

                var originalPostcode = postcodeRegex.Match(originalAddress);
                if (originalPostcode.Success)
                {
                    originalHotel.Postcode = originalPostcode.Value;
                }

                var comparePostcode = postcodeRegex.Match(compareAddress);
                if (comparePostcode.Success)
                {
                    compareHotel.Postcode = comparePostcode.Value;
                }
            }

            { // PostcodeSection
                var originalPostcode = originalHotel.Postcode;
                var comparePostcode = compareHotel.Postcode;

                if (originalPostcode == comparePostcode)
                {
                    similarity += .2;
                }
            }


            // Loop through each of the hotels fields
            // Check how similar the hotel is to each other (How likely are they to be the same hotel)
            // Pending on the field add a weight to the similarity (0.0 <-> 1.0)
            // 1 = Same Hotel
            // 0 = No the same

            return similarity;
        }

        private void Collate(Hotel originalHotel, Hotel collateHotel)
        {
            foreach (HotelReservation hotelReservation in collateHotel.ReservationData.GetAllReservations())
            {
                originalHotel.ReservationData.AddDate(hotelReservation);
            }

            
        }

        private int LevenshteinDistance(string original, string compare)
        {
            int originalLength = original.Length;
            int compareLength = compare.Length;
            int[,] matrix = new int[originalLength + 1, compareLength + 1];

            if (originalLength == 0)
            {
                return compareLength;
            }
            if (compareLength == 0)
            {
                return originalLength;
            }

            for (int i = 0; i <= originalLength; matrix[i, 0] = i++)
            {
            }

            for (int j = 0; j <= compareLength; matrix[0, j] = j++)
            {
            }

            for (int i = 1; i <= originalLength; i++)
            {
                for (int j = 1; j <= compareLength; j++)
                {
                    int cost = (compare[j - 1] == original[i - 1]) ? 0 : 1;
                    matrix[i, j] = Math.Min(
                        Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                        matrix[i - 1, j - 1] + cost);
                }
            }
            return matrix[originalLength, compareLength];
        }
    }
}
