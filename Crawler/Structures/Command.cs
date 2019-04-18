using System;
using System.Collections.Generic;
using System.Text;
using Crawler.Structures;

namespace Crawler
{
    class Command
    {
        public string Name;
        public string Description;
        public List<string> Aliases = new List<string>();
        public List<CommandArgument> Arguments = new List<CommandArgument>();

        public virtual void Invoke(CommandArguments commandArgument)
        {

        }
    }
}
