using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Crawler.Attributes;
using Crawler.Structures;

namespace Crawler.Commands
{
    
    [CommandName("stop")]
    [CommandAlias("shutdown"), CommandAlias("exit")]
    [CommandDescription("Exit the program")]
    [Attributes.CommandArgument("f", "force", "force shutdown")]
    class Stop : Command
    {
        public override void Invoke(CommandArguments args)
        {
            Environment.Exit(0);
        }
    }
}
