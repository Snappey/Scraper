using System;
using System.Collections.Generic;
using System.Text;
using Crawler.Attributes;
using Crawler.Structures;

namespace Crawler.Commands
{
    [CommandName("list")]
    [CommandDescription("Lists all registered sites")]
    class List : Command
    {
        public override void Invoke(CommandArguments commandArgument)
        {
            var sites = Program.App.GetSites();

            Program.App.Log("Sites: ");
            foreach (var site in sites)
            {
                StringBuilder stringBuilder = new StringBuilder();

                stringBuilder.Append("> ");
                stringBuilder.Append(site.URL);
                
                Program.App.Log(stringBuilder.ToString());

                foreach (var page in site.Pages)
                {
                    Program.App.Log("  > " + page.Value.URL);
                    Program.App.Log("   > " + page.Value.Nodes.Count + " nodes");
                }
            }
        }
    }
}
