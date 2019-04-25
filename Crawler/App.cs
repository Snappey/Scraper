using System;
using System.Collections.Generic;
using System.Text;
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
            Commands = new CommandManager();
            Sites = new SiteManager();
            Storage = new Storage("local.db");
            Display = new ConsoleManager(170, 60);
            productMatching = new ProductMatching(Storage);

            //productMatching.Start();

            Sites.Register(); // Gets all Site Providers
            Sites.GetSites().ForEach((site) => {Display.Attach(site);}); // Setup display for sites

            /*var hotelsList = Sites.GetAllData(); // Returns a list of hotels from the providers after they've ran

            foreach (List<Hotel> hotels in hotelsList)
            {
                hotels.ForEach((hotel) => {Storage.AddHotel(hotel);}); // TODO: Run hotel data through a product matching class, work out if we  already store that hotel first then assign it an ID and store it
            }
            
            Sites.FlushData();*/

           

            //Creator creator = new Creator("Report");

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
                hotels.ForEach((hotel) => {Storage.AddHotel(hotel);}); // TODO: Run hotel data through a product matching class, work out if we  already store that hotel first then assign it an ID and store it
            }

            Log($"[{DateTime.Now.ToShortTimeString()}] [CMD] All Sites have finished!");
            Sites.FlushData();
        }

        public List<Site> GetSites()
        {
            return Sites.GetSites();
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
