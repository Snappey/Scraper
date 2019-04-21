using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SQLite;

namespace Crawler
{
    class Storage
    {
        private string database;
        private SQLiteConnection connection;
        private SQLiteCommandBuilder queryBuilder;

        public Storage(string database)
        {
            this.database = database;

            Init();

            Console.Write("Test");
        }

        private void Init()
        {
            connection = new SQLiteConnection();
            connection.ConnectionString = $"Data Source={database}";

            queryBuilder = new SQLiteCommandBuilder();

            try
            {
                connection.Open();
            }
            catch (Exception e)
            {

            }
            connection.Close();
        }

        private void CreateSchema()
        {
            string createtable = @"CREATE TABLE `hotels`(
                name TEXT,
                city TEXT,
                Address TEXT,
                Postcode TEXT,
                Phonenumber TEXT,
                Gathered DATETIME,
                Extras TEXT
            )";
            
            new SQLiteCommand(createtable).ExecuteNonQuery();

        }
    }
}
