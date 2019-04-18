using System;
using System.Collections.Generic;
using System.Text;

namespace Crawler.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    class CommandArgument : Attribute
    {
        private string prefix;
        private string fullfix;
        private string description;

        public CommandArgument(string prefix, string fullfix, string description)
        {
            this.prefix = prefix;
            this.fullfix = fullfix;
            this.description = description;
        }

        public string Prefix
        {
            get { return prefix; }
        }

        public string Fullfix
        {
            get { return fullfix; }
        }

        public string Description
        {
            get { return description; }
        }
    }
}
