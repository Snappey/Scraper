using System;
using System.Collections.Generic;
using System.Text;
using Crawler.Structures;

namespace Crawler
{
    class ProductMatching
    {
        private Storage connection;
        private List<Hotel> preMatchHotels = new List<Hotel>();
        private List<List<Hotel>> postMatchHotels = new List<List<Hotel>>(); // List of a list of hotels that are the same

        public ProductMatching(Storage connection)
        {
            this.connection = connection;
        }

        public void Start()
        {
            preMatchHotels = connection.GetHotels();

            Loop();
        }

        private void Loop()
        {
            foreach (Hotel originalHotel in preMatchHotels)
            {
                foreach (Hotel compareHotel in preMatchHotels)
                {
                    int val = Compare(originalHotel, compareHotel);

                    // Determine a threshold that says they are the same hotel
                    // if val > threshold add both hotels to the list and remove one from the prematch list
                }
            }
            // ISSUE, we only add hotels that are similar to the postMatchHotel list, we are missing all hotels that we only have one entry of or are not similar to other hotels
            // Possibly always add hotels to the list, if a hotel is similar add the similar one as well and remove that from the list keeping the original still in for further comparisons

            // Once each hotel has been compared against each other and the postMatchHotel list is full
            // Loop through the postMatchHotels and collate the data from each of the fields to potentially have a more complete dataset
        }

        private int Compare(Hotel originalHotel, Hotel compareHotel)
        {
            int similarity = 0;

            // Loop through each of the hotels fields
            // Check how similar the hotel is to each other (How likely are they to be the same hotel)
            // Pending on the field add a weight to the similarity (0.0 <-> 1.0)
            // 1 = Same Hotel
            // 0 = No the same

            return similarity;
        }

        private Hotel Collate(List<Hotel> hotels)
        {
            // Loop through each of the hotels and collate the fields together if there is missing data

            return new Hotel();
        }
    }
}
