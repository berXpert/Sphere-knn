#region License
// Copyright (c) 2015 Bernardino Perea
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using BerXpert.SphereKnn;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BerXpert.SphereKnn.UnitTest
{
    [TestClass]
    public class SphereSearchTest
    {
        private List<City> cities;
        private SphereSearch<City> citySearch;

        [TestInitialize]
        public void Given_Some_Cities()
        {
            cities = new List<City>
            {
                new  City("Boston",    42.358, -71.064),
                new City("Troy",      42.732, -73.693),
                 new City("New York",  40.664, -73.939),
                 new City("Miami",     25.788, -80.224),
                 new City("London",    51.507,  -0.128),
                 new City("Paris",     48.857,   2.351),
                 new City("Vienna",    48.208,  16.373),
                 new City("Rome",      41.900,  12.500),
                 new City("Beijing",   39.914, 116.392),
                 new City("Hong Kong", 22.278, 114.159),
                 new City("Seoul",     37.567, 126.978),
                 new City("Tokyo",     35.690, 139.692)
            };

            citySearch = new SphereSearch<City>(cities, new Func<City, double[]>(c => Cartesian.FromSpherical(c.Latitude, c.Longitude)));

        }

        [TestMethod]
        public void Check_Hawaii()
        {
            var hawaii = new City("Hawaii", 21.31, -157.8);

            var closest = citySearch.Lookup(hawaii, 2).ToList();

            Assert.IsTrue(closest.Count == 2);
            Assert.IsTrue(closest[0].Name == "Tokyo");
            Assert.IsTrue(closest[1].Name == "Seoul");
        }

        [TestMethod]
        public void Check_Berlin()
        {
            var berlin = new City("Berlin", 52.50, 13.40);

            var closest = citySearch.Lookup(berlin, 4).ToList();

            Assert.IsTrue(closest.Count == 4);
            Assert.IsTrue(closest[0].Name == "Vienna");
            Assert.IsTrue(closest[1].Name == "Paris");
            Assert.IsTrue(closest[2].Name == "London");
            Assert.IsTrue(closest[3].Name == "Rome");
        }

        [TestMethod]
        public void Check_Hartford_200km()
        {
            var hartford = new City("Hartford", 41.76, -72.67);

            var closest = citySearch.Lookup(hartford, 20, 200000.0).ToList();

            // only 3 cities are within 200km of Hartford
            Assert.AreEqual(3, closest.Count);
            Assert.IsTrue(closest[0].Name == "Troy");
            Assert.IsTrue(closest[1].Name == "Boston");
            Assert.IsTrue(closest[2].Name == "New York");
        }

        [TestMethod]
        public void Check_Hartford_Measured_200km()
        {
            var hartford = new City("Hartford", 41.76, -72.67);

            var closest = citySearch.LookupDistance(hartford, 20, 200000.0).ToList();

            // only 3 cities are within 200km of Hartford
            Assert.AreEqual(3, closest.Count);
            Assert.IsTrue(closest[0].Item1.Name == "Troy");
            Assert.IsTrue(closest[1].Item1.Name == "Boston");
            Assert.IsTrue(closest[2].Item1.Name == "New York");

        }
    }
}
