using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sphere.Tests
{
    public record City(string Name, double Latitude, double Longitude)
    {
        public override string ToString()
        {
            return Name;
        }
    }
}
