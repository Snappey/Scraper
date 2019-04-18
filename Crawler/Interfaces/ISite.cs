using System;
using System.Collections.Generic;
using System.Text;
using Crawler.Structures;

namespace Crawler.Interfaces
{
    interface ISite
    {
        List<Hotel> GetData();
    }
}
