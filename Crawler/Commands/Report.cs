using System;
using System.Collections.Generic;
using System.Text;
using Crawler.Attributes;
using Crawler.Structures;

namespace Crawler.Commands
{
    [CommandName("report")]
    [CommandDescription("Generates an HTML report of hotels with the given arguments")]
    [Attributes.CommandArgument("city", "city", "City to generate the report on")]
    [Attributes.CommandArgument("ci", "checkin", "Check-In Date DD/MM/YYYY (default = Next Day)")]
    [Attributes.CommandArgument("co", "checkout", "Check-Out Date DD/MM/YYYY (default = Day After Check In)")]
    [Attributes.CommandArgument("p", "people", "Amount of adults (default = 1)")]
    [Attributes.CommandArgument("rm", "rooms", "Number of rooms (default = 1)")]
    class Report : Command
    {
        public override void Invoke(CommandArguments commandArgument)
        {
            RequestArgs args = new RequestArgs
            {
                CheckIn = DateTime.Now.AddDays(1),
                CheckOut = DateTime.Now.AddDays(2),
                City = "",
                People = "1",
                Rooms = "1",
            };

            foreach (var argument in commandArgument.Arguments)
            {
                if (argument.Key == "city")
                {
                    args.City = argument.Value;
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
            }

            Program.App.GenerateReport(args);
        }
    }
}
