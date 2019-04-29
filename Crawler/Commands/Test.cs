using System;
using System.Collections.Generic;
using System.Text;
using Crawler.Attributes;
using Crawler.Structures;
using OpenQA.Selenium;
using Scraper;
using Scraper.Structures;

namespace Crawler.Commands
{
    /// <summary>
    /// Test command used for debugging
    /// </summary>
    [CommandName("test")]
    [CommandDescription("Test method for testing")]
    class Test : Command
    {
        public override void Invoke(CommandArguments commandArgument)
        {
            Console.WriteLine("Hello World!");

            Console.SetWindowSize(Console.LargestWindowWidth / 2, Console.LargestWindowHeight);
            Console.SetWindowPosition(0, 0);

            var scraper = new Scraper.Scraper();

            var ihg = new Site(new Uri("https://www.ihg.com/"));
            var london = ihg.AddPage("hotels/gb/en/find-hotels/hotel/list?qDest=London,%20United%20Kingdom&qCiMy=32019&qCiD=16&qCoMy=32019&qCoD=17&qAdlt=1&qChld=0&qRms=1&qRtP=6CBARC&qAkamaiCC=GB&qSrt=sDD&qBrs=re.ic.in.vn.cp.vx.hi.ex.rs.cv.sb.cw.ma.ul.ki.va&srb_u=0&qRad=30&qRdU=mi", By.TagName("body"));

            london.AddNode(new NodeRequest { Property = "Name", XPath = "//div/hotel-row/div[1]/div/div/hotel-details/div/div/div[1]/div/div/span[1]/a" });
            london.AddNode(new NodeRequest { Property = "Location", XPath = "//div/hotel-row/div[1]/div/div/hotel-details/div/div/div[1]/div/div/span[2]/a" });
            london.AddNode(new NodeRequest { Property = "Address", XPath = "//div/hotel-row/div[1]/div/div/hotel-details/div/div/div[2]/div[1]/hotel-address/div/div/span[1]" });
            london.AddNode(new NodeRequest { Property = "Postcode", XPath = "//div/hotel-row/div[1]/div/div/hotel-details/div/div/div[2]/div[1]/hotel-address/div/div/span[1]" });
            london.AddNode(new NodeRequest { Property = "Country", XPath = "//div/hotel-row/div[1]/div/div/hotel-details/div/div/div[2]/div[1]/hotel-address/div/div/span[3]/span[2]" });

            Site dotnetperls = new Site(new Uri("https://www.dotnetperls.com"));
            dotnetperls.OutputType = PipelineOutput.Object;
            //PageLayout indexLayout = dotnetperls.AddPage("", By.TagName("body"));
            PageLayout asyncLayout = dotnetperls.AddPage("async", By.TagName("body"));

            //indexLayout.AddNode(new NodeRequest { Property = "Pages", XPath = "//body//a//b" });
            //indexLayout.AddNode(new NodeRequest { Property = "Description", XPath = "//body//a//div" });
            asyncLayout.AddNode(new NodeRequest
            {
                Property = "Title", XPath = "//body//a"
            });


            Console.WriteLine(ihg.ToString());
            scraper.AddSite(ihg);
            scraper.AddSite(dotnetperls);

            //scraper.RunAll();
            scraper.Run(dotnetperls);

            //Storage storage = new Storage("local.db");

            Console.ReadKey();
        }
    }
}
