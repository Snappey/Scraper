using System;
using System.Collections.Generic;
using System.Text;
using Crawler.Attributes;
using Crawler.Structures;
using Scraper.Structures;

namespace Crawler.Commands
{
    [CommandName("run")]
    [CommandDescription("Start the data gathering process")]
    [Attributes.CommandArgument("s", "site", "Site to start gathering")]
    [Attributes.CommandArgument("all", "all", "Run all sites registered")]
    [Attributes.CommandArgument("ci", "checkin", "Check-In Date DD/MM/YYYY (default = Next Day)")]
    [Attributes.CommandArgument("co", "checkout", "Check-Out Date DD/MM/YYYY (default = Day After Check In)")]
    [Attributes.CommandArgument("city", "city", "City to base the search in (default = London)")]
    [Attributes.CommandArgument("p", "people", "Amount of adults (default = 1)")]
    [Attributes.CommandArgument("rm", "rooms", "Number of rooms (default = 1)")]
    class Run : Command
    {
        public override void Invoke(CommandArguments commandArgument)
        {
            RequestArgs args = new RequestArgs
            {
                CheckIn = DateTime.Now.AddDays(1),
                CheckOut = DateTime.Now.AddDays(2),
                City = "London",
                People = "1",
                Rooms = "1",
            };

            Uri uri = new Uri("https://www.google.co.uk");
            bool AllSites = false;

            foreach (var argument in commandArgument.Arguments)
            {
                if (argument.Key == "-s")
                {
                    try
                    {
                        uri = new Uri(argument.Value);
                    }
                    catch
                    {
                        Program.App.Log($"[{DateTime.Now.ToShortTimeString()}] [CMD] Parameter -s failed expected URI format!");
                    }
                }

                if (argument.Key == "-ci")
                {
                    args.CheckIn = Convert.ToDateTime(argument.Value);
                }

                if (argument.Key == "-co")
                {
                    args.CheckOut = Convert.ToDateTime(argument.Value);
                }

                if (argument.Key == "-city")
                {
                    args.City = argument.Value;
                }

                if (argument.Key == "-p")
                {
                    args.People = argument.Value;
                }

                if (argument.Key == "-rm")
                {
                    args.People = argument.Value;
                }

                if (argument.Key == "-all")
                {
                    AllSites = true;
                }
            }

            if (AllSites)
            {
                Program.App.GetAllSiteData(args);
            }
            else
            {
                if (uri.Host == "www.google.co.uk")
                {
                    Program.App.Log($"[{DateTime.Now.ToShortTimeString()}] [CMD] -s is a required parameter of 'start'");
                }
                else
                {
                    Program.App.GetSiteData(uri, args);
                }
            }
        }
    }
}
