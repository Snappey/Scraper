using System;
using System.Collections.Generic;
using System.Text;
using Crawler.Attributes;
using Crawler.Structures;

namespace Crawler.Commands
{
    [CommandName("query")]
    [Attributes.CommandArgument("city", "city", "The city you want to search in")]
    [Attributes.CommandArgument("name", "name", "The name of the hotel you want to search for")]
    [Attributes.CommandArgument("site", "site", "Site you want to filter down to")]
    [CommandDescription("Query the database and return the found dataset")]
    class Query : Command
    {
        public override void Invoke(CommandArguments commandArgument)
        {
            RequestArgs args = new RequestArgs
            {
                City = "",
                Name = "",
            };
            string site = "";

            foreach (var argument in commandArgument.Arguments)
            {
                if (argument.Key == "-city")
                {
                    args.City = argument.Value;
                }

                if (argument.Key == "-name")
                {
                    args.Name = argument.Value;
                }

                if (argument.Key == "-site")
                {
                    site = argument.Value;
                }
            }
            Program.App.QueryDatabase(args, site);
        }
    }
}
