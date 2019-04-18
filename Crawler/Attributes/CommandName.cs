using System;
using System.Collections.Generic;
using System.Text;

namespace Crawler.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    class CommandName : Attribute
    {
        private string name;

        public CommandName(string name)
        {
            this.name = name;
        }

        public virtual string Name
        {
            get { return this.name; }
        }
    }
}
