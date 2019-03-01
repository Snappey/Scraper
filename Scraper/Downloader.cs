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

            chrome = new ChromeDriver(@"C:\Users\Jon\Source\Repos\Scraper\Scraper\bin\Debug\netcoreapp2.0", options);
        }

        public RawPage Next(Uri uri)
        {
            chrome.Navigate().GoToUrl(uri.AbsoluteUri);
            string content = chrome.PageSource;

            RawPage page = new RawPage();
            page.Content = content;
            page.Time = DateTime.Now;
            page.URL = uri;

            Console.WriteLine("| > " + uri + " - Downloaded");
            return page;
        }
    }

}
