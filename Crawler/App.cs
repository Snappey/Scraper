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
            Display = new ConsoleManager(170, 60);
            Commands = new CommandManager();
            Sites = new SiteManager();
            Storage = new Storage("local.db");
            productMatching = new ProductMatching(Storage);

            Sites.Register(); // Gets all Site Providers // TODO: Booking.com Price property is not working properly
            Sites.GetSites().ForEach((site) => {Display.Attach(site);}); // Setup display for sites
        }

        public void Start()
        {
            Program.App.Log($"[{DateTime.Now.ToShortTimeString()}] Type 'help' to see a list of all commands and arguments availible!");
            Loop();
        }

        public void Log(string log)
        {
            Display.Log(log, LogType.Information);
        }

        public Dictionary<string, Command> GetCommands()
        {
            return Commands.GetCommands();
        }

        public void GetSiteData(Uri site, RequestArgs args)
        {
            var isite = Sites.GetSiteInterface(site);

            if (isite != null)
            {
                isite.GetData(args).ForEach((hotel) => { Storage.AddHotel(hotel); });

                Sites.FlushData();
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
                hotels.ForEach((hotel) => {Storage.AddHotel(hotel);});
            }

            Log($"[{DateTime.Now.ToShortTimeString()}] [CMD] All Sites have finished!");
            Sites.FlushData();
        }

        public List<Site> GetSites()
        {
            return Sites.GetSites();
        }

        public void GenerateReport(RequestArgs args)
        {
            Program.App.Log($"[{DateTime.Now.ToShortTimeString()}] [CMD] Creating report..");
            Program.App.Log($"[{DateTime.Now.ToShortTimeString()}] [CMD] Gathering data, querying database..");
            productMatching.Start(args);

            Program.App.Log($"[{DateTime.Now.ToShortTimeString()}] [CMD] Results gathered, report being created");

            Creator reportCreator = new Creator();
            Task<string> reportLocation = reportCreator.Create(args, productMatching.GetResult());
            productMatching.Reset();          

            while (reportLocation.IsCompleted == false)
            {
                Thread.Sleep(10);
            }
            Program.App.Log($"[{DateTime.Now.ToShortTimeString()}] [CMD] Report created opening.. (${reportLocation.Result})");

            var p = new Process();
            p.StartInfo = new ProcessStartInfo(@reportLocation.Result)
            {
                UseShellExecute = true
            };
            p.Start();

        }

        public void DropDatabase()
        {
            Storage.Clear();
        }

        public void StatsDatabase()
        {
            Storage.GetStats();
        }

        public void ClearDatabase(Uri site)
        {
            Storage.Clear(site);
        }

        public void QueryDatabase(RequestArgs args, string site)
        {
            var hotels = Storage.GetHotelsFullRequest(args, site);

            Program.App.Log($"Hotels matching City: '{args.City}' Name: '{args.Name}' Site: '{site}'");
            foreach (Hotel hotel in hotels)
            {
                Program.App.Log($" -> {hotel.City} - {hotel.Name}, {hotel.Address} ({hotel.ScrapeURL})");
            }
            Program.App.Log($"Found {hotels.Count} matches!");
        }

        private void Loop()
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
