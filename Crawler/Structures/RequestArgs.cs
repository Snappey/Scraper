using System;
using System.Collections.Generic;
using System.Text;

namespace Crawler.Structures
{
    /// <summary>
    /// Primarly used as filter arguments and storing user input for requesting and filtering data
    /// </summary>
    public class RequestArgs
    {
        public DateTime CheckIn;
        public DateTime CheckOut;
        public string City;
        public string People;
        public string Rooms;
        public string Name;
    }
}
