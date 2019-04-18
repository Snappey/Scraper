using System;
using System.Collections.Generic;
using System.Text;

namespace Crawler.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    class CommandAlias : Attribute
    {
        private string alias;

        public CommandAlias(string alias)
        {
            this.alias = alias;
        }

        public virtual string Alias
        {
            get { return alias; }
        }
    }
}
