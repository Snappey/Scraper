using System;
using System.Collections.Generic;
using System.Text;
using Crawler.Structures;

namespace Crawler
{
    /// <summary>
    /// Base class used by all other commands, implements all fields.
    /// </summary>
    class Command
    {
        public string Name;
        public string Description;
        public List<string> Aliases = new List<string>();
        public List<CommandArgument> Arguments = new List<CommandArgument>();

        /// <summary>
        /// Provides a common function that all commands can implement that we can invoke generically
        /// </summary>
        /// <param name="commandArgument"></param>
        public virtual void Invoke(CommandArguments commandArgument)
        {

        }
    }
}
