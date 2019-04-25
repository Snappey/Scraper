using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Crawler.Attributes;
using Crawler.Structures;

namespace Crawler.Commands
{
    
    [CommandName("stop")]
    [CommandDescription("Exit the program")]
    class Stop : Command
    {
        public override void Invoke(CommandArguments args)
        {
            Environment.Exit(0);
        }
    }
}
