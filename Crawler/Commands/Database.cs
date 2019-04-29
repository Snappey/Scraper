using System;
using System.Collections.Generic;
using System.Text;
using Crawler.Attributes;
using Crawler.Structures;

namespace Crawler.Commands
{
    [CommandName("database")]
    [Attributes.CommandArgument("drop", "drop", "clear all the data from the database")]
    [Attributes.CommandArgument("stats", "statistics", "returns statistics from the database")]
    [Attributes.CommandArgument("clear", "clear", "Clear all data from the supplied site in the database")]
    [CommandDescription("interface with the database")]
    class Database : Command
    {
        public override void Invoke(CommandArguments commandArgument)
        {
            if (commandArgument.Arguments.ContainsKey("-drop"))
            {
                Program.App.DropDatabase();
            }
            else
            {
                if (commandArgument.Arguments.ContainsKey("-stats"))
                {
                    Program.App.StatsDatabase();
                }
                else
                {
                    if (commandArgument.Arguments.ContainsKey("-clear"))
                    {
                        Uri uri = new Uri("https://www.google.com");
                        try
                        {
                            uri = new Uri(commandArgument.Arguments["-clear"]);                     
                        }
                        catch
                        {
                            Program.App.Log($"[{DateTime.Now.ToShortTimeString()}] [CMD] Parameter -clear failed expected URI format!");
                        }
                        Program.App.ClearDatabase(uri);
                    }
                }
            }
        }
    }
}
