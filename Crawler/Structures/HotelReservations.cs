using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.Text;

namespace Crawler.Structures
{
    /// <summary>
    /// Stores Checkin/Out Dates vs Price for a given Hotel, stored on the Hotel.cs DataStructure
    /// </summary>
    class HotelReservations
    {
        private List<HotelReservation> reservations = new List<HotelReservation>();

        public bool AddDate(DateTime checkIn, DateTime checkOut, float price, string currency)
        {
            int idx = this.Contains(checkIn, checkOut);
            if (idx >= 0)
            {
                // Found a duplicate reservation entry
                // TODO: Consider whether keeping duplicate reservations
                reservations.RemoveAt(idx);
            }

            reservations.Add(new HotelReservation
            {
                CheckIn = checkIn,
                CheckOut = checkOut,
                Currency = currency,
                Price = price,
            });
            return true;
        }

        public bool AddDate(HotelReservation reservation)
        {
            int idx = this.Contains(reservation.CheckIn, reservation.CheckOut);
            if (idx >= 0)
            {
                reservations.RemoveAt(idx);
            }

            reservations.Add(reservation);
            return true;
        }

        public int Contains(DateTime checkIn, DateTime checkOut)
        {
            int idx = -1;
            for (var i = 0; i < reservations.Count; i++)
            {
                HotelReservation reservation = reservations[i];
                if (reservation.CheckIn.ToShortDateString() == checkIn.ToShortDateString() &&
                    reservation.CheckOut.ToShortDateString() == checkOut.ToShortDateString())
                {
                    idx = i;
                    break;
                }
            }
            return idx;
        }

        // Can be either CheckIn or CheckOut
        public HotelReservation GetReservation(DateTime checkDateTime)
        {
            HotelReservation searchReservation = null;

            foreach (HotelReservation reservation in reservations)
            {
                if (reservation.CheckIn.ToShortDateString() == checkDateTime.ToShortDateString() ||
                    reservation.CheckOut.ToShortDateString() == checkDateTime.ToShortDateString())
                {
                    searchReservation = reservation;
                    break;
                }
            }

            return searchReservation;
        }

        public HotelReservation GetReservation(DateTime checkIn, DateTime checkOut)
        {
            HotelReservation searchReservation = null;

            foreach (HotelReservation reservation in reservations)
            {
                if (reservation.CheckIn.ToShortDateString() == checkIn.ToShortDateString() &&
                    reservation.CheckOut.ToShortDateString() == checkOut.ToShortDateString())
                {
                    searchReservation = reservation;
                    break;
                }
            }
            return searchReservation;
        }

        public HotelReservation GetReservation(int day, int month, int year)
        {
            HotelReservation searchReservation = null;

            foreach (HotelReservation reservation in reservations)
            {
                if ((reservation.CheckIn.Day == day && reservation.CheckIn.Month  == month && reservation.CheckIn.Year == year) ||
                    (reservation.CheckOut.Day == day && reservation.CheckOut.Month == month && reservation.CheckOut.Year == year ))
                {
                    searchReservation = reservation;
                }
            }

            return searchReservation;
        }

        public List<HotelReservation> GetAllReservations()
        {
            return reservations;
        }
    }

    class HotelReservation
    {
        public DateTime CheckIn;
        public DateTime CheckOut;
        public float Price;
        public string Currency;
    }
}
