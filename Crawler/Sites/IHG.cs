using System;
using System.Collections.Generic;
using System.Text;
using Crawler.Interfaces;
using Crawler.Structures;

namespace Crawler.Sites
{
    class IHG : ISite
    {

        private string apikey = "tq5jhkg9mrc652ms9232yvs8";

        public IHG()
        {

        }

        public List<Hotel> GetData()
        {
            throw new NotImplementedException();
        }
    }
}
