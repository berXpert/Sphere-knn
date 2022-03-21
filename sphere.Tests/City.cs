using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sphere.Tests
{
    public class City
    {
        public string Name { get; set; }
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public City(string n, double lat, double lon)
        {
            Name = n;
            Latitude = lat;
            Longitude = lon;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
