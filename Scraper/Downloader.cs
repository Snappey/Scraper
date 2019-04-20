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
using OpenQA.Selenium.Support.UI;
//using SeleniumExtras.WaitHelpers;

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
            chrome.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        }

        public RawPage Next(Uri uri, string elementid, string jsexec, int pagedelay)
        {
            chrome.Navigate().GoToUrl(uri.AbsoluteUri);

            try
            {
                var wait = new WebDriverWait(chrome, TimeSpan.FromSeconds(10));
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id(elementid))); // Captcha ID: recaptcha-anchor
            }
            catch(WebDriverTimeoutException)
            {
                Console.WriteLine($"No such element was found: {uri.PathAndQuery} with ID '{elementid}'");
            }

            if (jsexec != String.Empty)
            {
                chrome.ExecuteScript(jsexec);
            }

            Thread.Sleep(pagedelay);

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
