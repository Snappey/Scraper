using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading;
using Scraper.Structures;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;

namespace Scraper
{
    internal class Downloader
    {
        private ChromeDriver chrome;

        public Downloader()
        {
            ChromeOptions options = new ChromeOptions();
            //options.AddArgument("headless");
            options.AddArgument("--log-level=3");

            chrome = new ChromeDriver(Environment.CurrentDirectory, options); // Chromedriver is copied across from the working directory to the output dir

            chrome.Manage().Window.Maximize();
            chrome.ExecuteScript("window.scrollTo(0, 9000)");

        }

        public RawPage Next(Uri uri)
        {
            chrome.Navigate().GoToUrl(uri.AbsoluteUri);
            Thread.Sleep(15000); // TODO: Check if element exists before loading page and parsing https://stackoverflow.com/questions/43203243/how-to-get-webdriver-to-wait-for-page-to-load-c-selenium-project
            string content = chrome.PageSource;   // TODO: DO this by checking for an exisiting element, pass this through the function based on data stored in the site data structure

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
