using System;
using System.Collections.Generic;
using System.Text;
using Crawler.Attributes;
using Crawler.Structures;

namespace Crawler.Commands
{
    /// <summary>
    /// Report command, command that allows the user to generate the HTML reports with filters to decide what hotels should be compared
    /// </summary>
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
                Name = "",
            };

            foreach (var argument in commandArgument.Arguments)
            {
                if (argument.Key == "city")
                {
                    args.City = argument.Value;
                }

                if (argument.Key == "-ci")
                {
                    try
                    {
                        args.CheckIn = Convert.ToDateTime(argument.Value);
                        args.CheckOut = args.CheckIn.AddDays(1);
                    }
                    catch
                    {
                        Program.App.Log("Failed to convert -ci argument to valid date");
                    }                
                }

                if (argument.Key == "-co")
                {
                    args.CheckOut = Convert.ToDateTime(argument.Value);
                }

                if (argument.Key == "-city")
                {
                    args.City = argument.Value;
                }

                if (argument.Key == "-name")
                {
                    args.Name = argument.Value;
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

            if (args.Name == String.Empty && args.City == String.Empty)
            {
                Program.App.Log($"[{DateTime.Now.ToShortTimeString()}] No argument passed for city or name..");
            }
            else
            {
                Program.App.GenerateReport(args);
            }       
        }
    }
}
