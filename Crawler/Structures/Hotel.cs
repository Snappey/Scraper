using System;
using System.Collections.Generic;
using System.Text;

namespace Crawler.Structures
{
    class Hotel
    {
        public string Name;
        public string City;
        public string Address;
        public string Postcode;
        public string Phonenumber;
        public DateTime DateGathered;
        public HotelReservations ReservationData;
        public List<string> Extras;
    }
}
