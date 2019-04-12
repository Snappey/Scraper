using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading;
using Scraper.Structures;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;

namespace Scraper
{
    internal class Downloader
    {
        private ChromeDriver chrome;

        public Downloader()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("headless");
            options.AddArgument("--log-level=3");

            chrome = new ChromeDriver(Environment.CurrentDirectory, options); // Chromedriver is copied across from the working directory to the output dir
        }

        public RawPage Next(Uri uri)
        {
            chrome.Navigate().GoToUrl(uri.AbsoluteUri);
            string content = chrome.PageSource;

            RawPage page = new RawPage
            {
                Content = content,
                Time = DateTime.Now,
                URL = uri
            };

            Console.WriteLine("| > " + uri + " - Downloaded");
            return page;
        }
    }

}
