using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Crawler.Report;
using Crawler.Structures;
using Scraper;
using Scraper.Structures;

namespace Crawler
{
    class App
    {
        private bool IsRunning = true;

        private ConsoleManager Display;
        private CommandManager Commands;
        private SiteManager Sites;
        private Storage Storage;
        private ProductMatching productMatching;

        public App()
        {
            Display = new ConsoleManager(170, 60); // Primary initialisation of each of the major classes
            Commands = new CommandManager();
            Sites = new SiteManager();
            Storage = new Storage("local.db");
            productMatching = new ProductMatching(Storage);

            Sites.Register(); // Gets all Site Providers
            Sites.GetSites().ForEach((site) => {Display.Attach(site);}); // Setup display for sites
        }

        public void Start()
        {
            Program.App.Log($"[{DateTime.Now.ToShortTimeString()}] Type 'help' to see a list of all commands and arguments availible!");
            Loop();
        }

        public void Log(string log)
        {
            Display.Log(log, LogType.Information); // Outputs logs to the console output section of the UI
        }

        public Dictionary<string, Command> GetCommands()
        {
            return Commands.GetCommands(); // Returns a list of all commands registered in the CommandManager
        }

        public void GetSiteData(Uri site, RequestArgs args) // Primary function used to iterate over each of the registered classes and retrieve their data
        {
            var isite = Sites.GetSiteInterface(site);

            if (isite != null)
            {
                isite.GetData(args).ForEach((hotel) => { Storage.AddHotel(hotel); });

                Sites.FlushData(); // Cleans up the data structure and releases some stored memory after its finished processing
                Log($"[{DateTime.Now.ToShortTimeString()}] [CMD] {site.Host} has finished!");
            }
            else
            {
                Log($"[{DateTime.Now.ToShortTimeString()}] [CMD] Failed to find site of " + site.Host);
            }
        }

        public void GetAllSiteData(RequestArgs args)
        {
            var hotelsList = Sites.GetAllData(args); // Returns a list of hotels from the providers after they've ran

            foreach (List<Hotel> hotels in hotelsList)
            {
                hotels.ForEach((hotel) => {Storage.AddHotel(hotel);}); // iterates over the results and adds them to the database
            }

            Log($"[{DateTime.Now.ToShortTimeString()}] [CMD] All Sites have finished!");
            Sites.FlushData();
        }

        public List<Site> GetSites()
        {
            return Sites.GetSites(); // Returns a list of all sites registered in the site manager
        }

        public void GenerateReport(RequestArgs args) // Function that is used to generate the HTML report
        {
            Program.App.Log($"[{DateTime.Now.ToShortTimeString()}] [CMD] Creating report..");
            Program.App.Log($"[{DateTime.Now.ToShortTimeString()}] [CMD] Gathering data, querying database..");

            productMatching.Start(args); // Starts the product matching process with the user defined filter

            Program.App.Log($"[{DateTime.Now.ToShortTimeString()}] [CMD] Results gathered, report being created");

            Creator reportCreator = new Creator();
            Task<string> reportLocation = reportCreator.Create(args, productMatching.GetResult()); // Passes the results from product matching to the report generation
            productMatching.Reset();          

            while (reportLocation.IsCompleted == false)
            {
                Thread.Sleep(10); // Waits for the async method to be completed before continuing
            }
            Program.App.Log($"[{DateTime.Now.ToShortTimeString()}] [CMD] Report created opening.. (${reportLocation.Result})");

            var p = new Process();
            p.StartInfo = new ProcessStartInfo(@reportLocation.Result)
            {
                UseShellExecute = true
            };
            p.Start(); // Opens the report created, uses the primary web browsers associated on the users machine

        }

        public void DropDatabase()
        {
            Storage.Clear(); // Clears the database and recreates it
        }

        public void StatsDatabase()
        {
            Storage.GetStats(); // Returns row count for each of databases tables
        }

        public void ClearDatabase(Uri site)
        {
            Storage.Clear(site); // Clears all data associated with the passed site from the database
        }

        public void QueryDatabase(RequestArgs args, string site) // helper function which is used to query the databse and return results from the database
        {
            var hotels = Storage.GetHotelsFullRequest(args, site);

            Program.App.Log($"Hotels matching City: '{args.City}' Name: '{args.Name}' Site: '{site}'");
            foreach (Hotel hotel in hotels)
            {
                Program.App.Log($" -> {hotel.City} - {hotel.Name}, {hotel.Address} ({hotel.ScrapeURL})");
            }
            Program.App.Log($"Found {hotels.Count} matches!");
        }

        private void Loop() // Primary command loop that checks for user input and parses it.
        {
            while (IsRunning)
            {
                var text = Console.ReadLine();
                if (text != String.Empty)
                {
                   Commands.CommandInput(text);
                } 
                Display.ResetCursor();
            }
        }
    }
}
