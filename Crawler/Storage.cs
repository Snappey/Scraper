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
                Address TEXT,
                Postcode TEXT,
                Phonenumber TEXT,
                Gathered DATETIME,
                Extras TEXT
            )".Replace("{tbl}", hotelTable);
            
            new SQLiteCommand(createtable, connection).ExecuteNonQuery();

            string reservationtable = @"CREATE TABLE IF NOT EXISTS `{tbl}`(
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
