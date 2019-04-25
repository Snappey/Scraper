using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SQLite;
using Crawler.Structures;

namespace Crawler
{
    class Storage
    {
        private string database;
        private SQLiteConnection connection;

        private string hotelTable = "hotels";
        private string reservationTable = "hotels_reservations";

        public Storage(string database)
        {
            this.database = database;

            Init();
            CreateSchema();
        }

        public void AddHotel(Hotel hotel)
        {
            if (hotel.Name == null || hotel.City == null) { return; }
            connection.Open();
            using (SQLiteCommand command = new SQLiteCommand($"INSERT INTO `hotels` VALUES (@name, @city, @address, @postcode, @phonenumber, @gathered, @extras, @scrapeurl, @hotelurl)", connection))
            {
                //{hotel.Name}, {hotel.City}, {hotel.Address}, {hotel.Postcode}, {hotel.Phonenumber}, {hotel.DateGathered}, {hotel.Extras.ToString()}, {hotel.ScrapeURL}, {hotel.HotelURL}
                SQLiteParameter[] parameters = 
                {
                    new SQLiteParameter("@name", hotel.Name),
                    new SQLiteParameter("@city", hotel.City),
                    new SQLiteParameter("@address", hotel.Address),
                    new SQLiteParameter("@postcode", hotel.Postcode),
                    new SQLiteParameter("@phonenumber", hotel.Phonenumber), 
                    new SQLiteParameter("@gathered", hotel.DateGathered.ToShortDateString()),
                    new SQLiteParameter("@extras", hotel.Extras),
                    new SQLiteParameter("@scrapeurl", hotel.ScrapeURL),
                    new SQLiteParameter("@hotelurl", hotel.HotelURL), 
                };
                command.Parameters.AddRange(parameters);

                command.ExecuteNonQuery();
            }

            foreach (HotelReservation hotelReservation in hotel.ReservationData.GetAllReservations())
            {
                using (SQLiteCommand command = new SQLiteCommand($"INSERT INTO `hotels_reservations` VALUES (@site, @name, @city, @checkin, @checkout, @price, @currency)", connection))
                {
                    //{hotel.Name}, {hotel.City}, {hotelReservation.CheckIn}, {hotelReservation.CheckOut}, {hotelReservation.Price}, {hotelReservation.Currency}
                    SQLiteParameter[] parameters =
                    {
                        new SQLiteParameter("site", hotel.ScrapeURL), 
                        new SQLiteParameter("name", hotel.Name),
                        new SQLiteParameter("city", hotel.City),
                        new SQLiteParameter("checkin", hotelReservation.CheckIn.ToShortDateString()),
                        new SQLiteParameter("checkout", hotelReservation.CheckOut.ToShortDateString()),
                        new SQLiteParameter("price", hotelReservation.Price),
                        new SQLiteParameter("currency", hotelReservation.Currency), 
                    };
                    command.Parameters.AddRange(parameters);

                    command.ExecuteNonQuery();
                }
            }
            connection.Close();
        }

        public void HasHotel(Hotel hotel)
        {

        }

        private void Init()
        {
            connection = new SQLiteConnection();
            connection.ConnectionString = $"Data Source={database}";

            try
            {
                connection.Open();
            }
            catch
            {
                // Connection
            }
            connection.Close();
        }

        private void CreateSchema()
        {
            connection.Open();

            string createtable = @"CREATE TABLE IF NOT EXISTS `{tbl}`(
                name TEXT,
                city TEXT,
                address TEXT,
                postcode TEXT,
                phonenumber TEXT,
                gathered DATETIME,
                extras TEXT,
                search_url TEXT,
                hotel_url TEXT
            )".Replace("{tbl}", hotelTable);
            
            new SQLiteCommand(createtable, connection).ExecuteNonQuery();

            string reservationtable = @"CREATE TABLE IF NOT EXISTS `{tbl}`(
                site TEXT,
                name TEXT,
                city TEXT,
                check_in DATETIME,
                check_out DATETIME,
                price TEXT,
                currency TEXT
            )".Replace("{tbl}", reservationTable);

            new SQLiteCommand(reservationtable, connection).ExecuteNonQuery();

            connection.Close();
        }
    }
}
