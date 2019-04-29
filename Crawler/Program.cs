using System;
using System.Collections.Generic;
using Scraper;
using Scraper.Structures;

namespace Crawler
{
    static class Program
    {
        public static App App;

        /// <summary>
        /// Main Entry point of the application
        /// </summary>
        static void Main(string[] args)
        {
            App = new App();
            App.Start();
        }
    }
}
