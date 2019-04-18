using System;
using System.Collections.Generic;
using System.Text;

namespace Crawler.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    class CommandDescription : Attribute
    {
        private string desc;

        public CommandDescription(string desc)
        {
            this.desc = desc;
        }

        public virtual string Description
        {
            get { return desc; }
        }
    }
}
