using System;
using System.Collections.Generic;
using System.Text;

namespace Crawler.Structures
{
    class CommandArguments
    {
        public Dictionary<string, string> Arguments = new Dictionary<string, string>();

        public void Add(string flag, string parameter)
        {
            Arguments.Add(flag, parameter);
        }
    }
}
