using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using Scraper.Structures;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
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
            options.AddArgument("headless");
            options.AddArgument("--log-level=3");
            options.AddArgument("--user-agent=Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3112.50 Safari/537.36");

            ChromeDriverService service = ChromeDriverService.CreateDefaultService(Environment.CurrentDirectory);
            service.SuppressInitialDiagnosticInformation = true;

            chrome = new ChromeDriver(service, options); // Chromedriver is copied across from the working directory to the output dir

            chrome.Manage().Window.Position = new Point(0, 2000);
            chrome.Manage().Window.Size = new Size(1920, 1080);
            chrome.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        }

        public DownloadResult Next(Uri uri, By elementid, string jsexec, string xpathfilter, int pagedelay)
        {
            DownloadResult result = new DownloadResult();

            chrome.Navigate().GoToUrl(uri.AbsoluteUri);
 
            // TODO: This needs refactoring to account for multiple issues, error catching, request polling and source timeout
            try
            {
                var wait = new WebDriverWait(chrome, TimeSpan.FromSeconds(10));
                wait.Until(ExpectedConditions.ElementIsVisible(elementid)); // Captcha ID: recaptcha-anchor
            }
            catch(WebDriverTimeoutException)
            {
                result.Logs.Add($"No such element was found: {uri.PathAndQuery} with ID '{elementid}'");
                result.Status &= DownloadStatus.ErrorOccurred;
            }

            if (jsexec != String.Empty)
            {
                try
                {
                    chrome.ExecuteScript(jsexec);
                }
                catch(Exception e)
                {
                    result.Logs.Add($"Javascript Error: {e}");
                    result.Status &= DownloadStatus.ErrorOccurred;
                }
            }

            new WebDriverWait(chrome, TimeSpan.FromSeconds(75)).Until(
                d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));

            Thread.Sleep(pagedelay);

            string content;
            try
            {
                content = chrome.PageSource;
            }
            catch
            {
                result.Logs.Add($"Failed to get source of {chrome.Url}");
                result.Status = DownloadStatus.Failed;
                return result;
            }
            
            result.Logs.Add(uri + " - Downloaded");
            result.Status &= DownloadStatus.Success;

            if (xpathfilter == String.Empty)
            {
                RawPage page = new RawPage
                {
                    Content = content,
                    Time = DateTime.Now,
                    URL = uri
                };

                result.Results =  new List<RawPage>{page};       
            }
            else
            {
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(content);
                var nodes = document.DocumentNode.SelectNodes(xpathfilter);
                List<RawPage> pages = new List<RawPage>();

                foreach (HtmlNode node in nodes)
                {
                    pages.Add(new RawPage
                    {
                        Content = node.InnerHtml,
                        Time = DateTime.Now,
                        URL = uri,
                    });
                }
                
                result.Results = pages;
            }

            return result;
        }

        public RawPage DownloadPage(Uri url)
        {
            chrome.Navigate().GoToUrl(url.AbsoluteUri);

            new WebDriverWait(chrome, TimeSpan.FromSeconds(10)).Until( // Wait till page is fully loaded
                d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));

            return new RawPage
            {
                Content = chrome.PageSource,
                Time = DateTime.Now,
                URL = url,
            };
        }
    }
}
