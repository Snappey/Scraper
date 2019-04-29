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
                    new SQLiteParameter("@gathered", hotel.DateGathered),
                    new SQLiteParameter("@extras", hotel.Extras),
                    new SQLiteParameter("@scrapeurl", hotel.ScrapeURL),
                    new SQLiteParameter("@hotelurl", hotel.HotelURL), 
                };
                command.Parameters.AddRange(parameters);

                command.ExecuteNonQuery();
            }

            foreach (HotelReservation hotelReservation in hotel.ReservationData.GetAllReservations())
            {
                using (SQLiteCommand command = new SQLiteCommand($"INSERT INTO `hotels_reservations` VALUES (@scrapeurl, @name, @city, @checkin, @checkout, @price, @currency, @rooms, @people)", connection))
                {
                    //{hotel.Name}, {hotel.City}, {hotelReservation.CheckIn}, {hotelReservation.CheckOut}, {hotelReservation.Price}, {hotelReservation.Currency}
                    SQLiteParameter[] parameters =
                    {
                        new SQLiteParameter("scrapeurl", hotel.ScrapeURL), 
                        new SQLiteParameter("name", hotel.Name),
                        new SQLiteParameter("city", hotel.City),
                        new SQLiteParameter("checkin", hotelReservation.CheckIn),
                        new SQLiteParameter("checkout", hotelReservation.CheckOut),
                        new SQLiteParameter("price", hotelReservation.Price),
                        new SQLiteParameter("currency", hotelReservation.Currency), 
                        new SQLiteParameter("rooms", hotel.AmtRooms),
                        new SQLiteParameter("people", hotel.AmtPeople), 
                    };
                    command.Parameters.AddRange(parameters);

                    command.ExecuteNonQuery();
                }
            }
            connection.Close();
        }

        public List<Hotel> GetHotels()
        {
            List<Hotel> hotels = new List<Hotel>();
            connection.Open();
            using (SQLiteCommand command = new SQLiteCommand($"SELECT * FROM `hotels`", connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Hotel hotel = new Hotel();

                            hotel.Name = reader["name"] != null ? Convert.ToString(reader["name"]) : String.Empty;
                            hotel.Address = reader["address"] != null ? Convert.ToString(reader["address"]) : String.Empty;
                            hotel.City = reader["city"] != null ? Convert.ToString(reader["city"]) : String.Empty;
                            hotel.Postcode = reader["postcode"] != null ? Convert.ToString(reader["postcode"]) : String.Empty;
                            hotel.Phonenumber = reader["phonenumber"] != null ? Convert.ToString(reader["phonenumber"]) : String.Empty;
                            hotel.Extras = reader["extras"] != null ? Convert.ToString(reader["extras"]) : String.Empty;
                            hotel.ScrapeURL = reader["search_url"] != null ? Convert.ToString(reader["search_url"]) : String.Empty;
                            hotel.HotelURL = reader["hotel_url"] != null ? Convert.ToString(reader["hotel_url"]) : String.Empty;
                            //hotel.DateGathered = Convert.ToDateTime(reader["gathered"]);
                            hotel.ReservationData = new HotelReservations();

                            if (hotel.ScrapeURL != null)
                            {
                                using (SQLiteCommand reserveCommand =
                                    new SQLiteCommand("", connection))
                                {
                                    reserveCommand.CommandText = "SELECT * FROM `hotels_reservations` WHERE search_url=@site AND city=@city AND name=@name";
                                    reserveCommand.Parameters.Add(new SQLiteParameter("site", hotel.ScrapeURL));
                                    reserveCommand.Parameters.Add(new SQLiteParameter("city", hotel.City));
                                    reserveCommand.Parameters.Add(new SQLiteParameter("name", hotel.Name));

                                    using (SQLiteDataReader reserveReader = reserveCommand.ExecuteReader())
                                    {
                                        if (reserveReader.HasRows)
                                        {
                                            while (reserveReader.Read())
                                            {
                                                HotelReservation reservation = new HotelReservation();

                                                reservation.CheckOut = Convert.ToDateTime(reserveReader["check_out"]);
                                                reservation.CheckIn = Convert.ToDateTime(reserveReader["check_in"]);
                                                reservation.Price = reserveReader["price"] != null ? Convert.ToString(reserveReader["price"]) : String.Empty;
                                                reservation.Currency = reserveReader["currency"] != null ? Convert.ToString(reserveReader["currency"]) : String.Empty;
                                                reservation.Site = reserveReader["search_url"] != null ? Convert.ToString(reserveReader["search_url"]) : String.Empty;

                                                hotel.AmtPeople = reserveReader["people"] != null ? Convert.ToString(reserveReader["people"]) : String.Empty;
                                                hotel.AmtRooms = reserveReader["rooms"] != null ? Convert.ToString(reserveReader["rooms"]) : String.Empty;

     
                                                hotel.ReservationData.AddDate(reservation);
                                            }
                                        }
                                    }
                                }
                            }
                            hotels.Add(hotel);
                        }
                    }
                }
            }
            connection.Close();
            return hotels;
        }

        public List<Hotel> GetHotels(RequestArgs args)
        {
            List<Hotel> hotels = new List<Hotel>();
            connection.Open();
            using (SQLiteCommand command = new SQLiteCommand($"SELECT * FROM `hotels` WHERE city LIKE '%{args.City}%'", connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Hotel hotel = new Hotel();

                            hotel.Name = reader["name"] != null ? Convert.ToString(reader["name"]) : String.Empty;
                            hotel.Address = reader["address"] != null ? Convert.ToString(reader["address"]) : String.Empty;
                            hotel.City = reader["city"] != null ? Convert.ToString(reader["city"]) : String.Empty;
                            hotel.Postcode = reader["postcode"] != null ? Convert.ToString(reader["postcode"]) : String.Empty;
                            hotel.Phonenumber = reader["phonenumber"] != null ? Convert.ToString(reader["phonenumber"]) : String.Empty;
                            hotel.Extras = reader["extras"] != null ? Convert.ToString(reader["extras"]) : String.Empty;
                            hotel.ScrapeURL = reader["search_url"] != null ? Convert.ToString(reader["search_url"]) : String.Empty;
                            hotel.HotelURL = reader["hotel_url"] != null ? Convert.ToString(reader["hotel_url"]) : String.Empty;
                            hotel.DateGathered = Convert.ToDateTime(reader["gathered"]);
                            hotel.ReservationData = new HotelReservations();

                            if (hotel.ScrapeURL != null)
                            {
                                using (SQLiteCommand reserveCommand =
                                    new SQLiteCommand("", connection))
                                {
                                    reserveCommand.CommandText = "SELECT * FROM `hotels_reservations` WHERE search_url=@site AND city=@city AND name=@name AND rooms=@rooms AND people=@people";
                                    reserveCommand.Parameters.Add(new SQLiteParameter("site", hotel.ScrapeURL));
                                    reserveCommand.Parameters.Add(new SQLiteParameter("city", hotel.City));
                                    reserveCommand.Parameters.Add(new SQLiteParameter("name", hotel.Name));
                                    reserveCommand.Parameters.Add(new SQLiteParameter("rooms", args.Rooms));
                                    reserveCommand.Parameters.Add(new SQLiteParameter("people", args.People));

                                    using (SQLiteDataReader reserveReader = reserveCommand.ExecuteReader())
                                    {
                                        if (reserveReader.HasRows)
                                        {
                                            while (reserveReader.Read())
                                            {
                                                HotelReservation reservation = new HotelReservation();

                                                reservation.CheckOut = Convert.ToDateTime(reserveReader["check_out"]);
                                                reservation.CheckIn = Convert.ToDateTime(reserveReader["check_in"]);
                                                reservation.Price = reserveReader["price"] != null ? Convert.ToString(reserveReader["price"]) : String.Empty;
                                                reservation.Currency = reserveReader["currency"] != null ? Convert.ToString(reserveReader["currency"]) : String.Empty;
                                                reservation.Site = reserveReader["search_url"] != null ? Convert.ToString(reserveReader["search_url"]) : String.Empty;

                                                hotel.AmtPeople = reserveReader["people"] != null ? Convert.ToString(reserveReader["people"]) : String.Empty;
                                                hotel.AmtRooms = reserveReader["rooms"] != null ? Convert.ToString(reserveReader["rooms"]) : String.Empty;

                                                if (reservation.CheckIn.ToShortDateString() ==
                                                    args.CheckIn.ToShortDateString())
                                                {
                                                    if (reservation.CheckOut.ToShortDateString() ==
                                                        args.CheckOut.ToShortDateString())
                                                    {
                                                        hotel.ReservationData.AddDate(reservation);
                                                    }                                                
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            hotels.Add(hotel);
                        }
                    }
                }
            }
            connection.Close();
            return hotels;
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
                search_url TEXT,
                name TEXT,
                city TEXT,
                check_in DATETIME,
                check_out DATETIME,
                price TEXT,
                currency TEXT,
                rooms TEXT,
                people TEXT
            )".Replace("{tbl}", reservationTable);

            new SQLiteCommand(reservationtable, connection).ExecuteNonQuery();

            connection.Close();
        }
    }
}
