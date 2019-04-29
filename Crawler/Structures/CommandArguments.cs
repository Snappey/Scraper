using System;
using System.Collections.Generic;
using System.Text;

namespace Crawler.Structures
{
    /// <summary>
    /// Data structure that stores command arguments to be passed into an invoked command
    /// </summary>
    class CommandArguments
    {
        public Dictionary<string, string> Arguments = new Dictionary<string, string>();

        public void Add(string flag, string parameter)
        {
            Arguments.Add(flag, parameter);
        }
    }
}
