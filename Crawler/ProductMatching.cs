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
            preMatchHotels = connection.GetHotels(args);

            Loop();
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

        private void Loop()
        {
            foreach (Hotel originalHotel in preMatchHotels)
            {
                foreach (Hotel compareHotel in preMatchHotels)
                {
                    if (originalHotel.Equals(compareHotel) || originalHotel.ScrapeURL.Equals(compareHotel.ScrapeURL)) { continue; } // Skip comparing the same hotels
                    double similarity = Compare(originalHotel, compareHotel);

                    if (similarity > .55 )
                    {
                        if (checkHotel.ContainsKey(originalHotel) == false)
                        {
                            checkHotel.Add(originalHotel, true);
                            postMatchHotels.Add(Collate(originalHotel, compareHotel));
                        }
                        //Console.WriteLine($" Compare: {compareHotel.ScrapeURL} | Original: {originalHotel.ScrapeURL} | {compareHotel.Name} : {originalHotel.Name} - {similarity}");
                        else
                        {
                            Collate(originalHotel, compareHotel);
                        }
                    }

                    // Determine a threshold that says they are the same hotel
                    // if val > threshold add both hotels to the list and remove one from the prematch list
                }
            }
            // ISSUE, we only add hotels that are similar to the postMatchHotel list, we are missing all hotels that we only have one entry of or are not similar to other hotels
            // Possibly always add hotels to the list, if a hotel is similar add the similar one as well and remove that from the list keeping the original still in for further comparisons

            // Once each hotel has been compared against each other and the postMatchHotel list is full
            // Loop through the postMatchHotels and collate the data from each of the fields to potentially have a more complete dataset
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
                    /*var originalHotelNameSplit = originalHotelName.Split(' ');
                    var compareHotelNameSplit = compareHotelName.Split(' ');
                    int matches = 0;

                    foreach (string originalComponent in originalHotelNameSplit)
                    {
                        if (originalComponent.Equals("Hotel")) { continue; } // The component 'hotel' isnt very unique
                        if (compareHotelName.Contains(originalComponent))
                        {
                            matches++;
                        }
                    }

                    if (matches > 0)
                    {
                        similarity += Math.Max((0.12) * matches, .4); // calculate threshold based on how many components match
                    }*/

                    int distance = LevenshteinDistance(originalHotelName, compareHotelName);

                    similarity += (.4 - (0.05 * distance));

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
                    similarity += .2;
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

        private Hotel Collate(Hotel originalHotel, Hotel collateHotel)
        {
            foreach (HotelReservation hotelReservation in collateHotel.ReservationData.GetAllReservations())
            {
                originalHotel.ReservationData.AddDate(hotelReservation);
            }
            
            return originalHotel;
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
