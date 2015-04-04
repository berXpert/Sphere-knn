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

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BerXpert.SphereKnn;
using System.Collections.Generic;


namespace BerXpert.SphereKnn.UnitTest
{
    [TestClass]
    public class BasicTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            Tree<int> tree;
            var ints = new int[3];

            tree = new Tree<int>(ints, new Func<int, double[]>(p => null));

            Assert.IsNotNull(tree);
        }

        private List<City> cities;
        private Tree<City> tree;

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

            tree = new Tree<City>(cities, new Func<City, double[]>(c => Cartesian.FromSpherical(c.Latitude, c.Longitude)));

        }

        [TestMethod]
        public void Check_Philadelphia()
        {
            var tree = new Tree<City>(cities, new Func<City, double[]>(c => Cartesian.FromSpherical(c.Latitude, c.Longitude)));

            var philadelphia = new City("Philadelphia", 39.95, -75.17);

            var closest = tree.Lookup(philadelphia, tree.Root, 4);

            Assert.IsTrue(closest.Count == 4);
            Assert.IsTrue(closest[0].Node.Data.Name == "New York");
            Assert.IsTrue(closest[1].Node.Data.Name == "Troy");
            Assert.IsTrue(closest[2].Node.Data.Name == "Boston");
            Assert.IsTrue(closest[3].Node.Data.Name == "Miami");

            var p = Cartesian.FromSpherical(philadelphia.Latitude, philadelphia.Longitude);
            Console.WriteLine("{0} - ({1},{2},{3})", philadelphia.Name, p[0], p[1], p[2]);

            foreach (var item in closest)
            {
                var q = Cartesian.FromSpherical(item.Node.Data.Latitude, item.Node.Data.Longitude);
                Console.WriteLine("{0} : At: {1} | ({2},{3},{4})", item.Node.Data.Name, item.Distance, q[0], q[1], q[2]);
            }
        }

        [TestMethod]
        public void Check_Hawaii()
        {
            var hawaii = new City("Hawaii", 21.31, -157.8);

            var closest = tree.Lookup(hawaii, tree.Root, 2);

            Assert.IsTrue(closest.Count == 2);
            Assert.IsTrue(closest[0].Node.Data.Name == "Tokyo");
            Assert.IsTrue(closest[1].Node.Data.Name == "Seoul");
        }

        [TestMethod]
        public void Check_Berlin()
        {
            var berlin = new City("Berlin", 52.50, 13.40);

            var closest = tree.Lookup(berlin, tree.Root, 4);

            Assert.IsTrue(closest.Count == 4);
            Assert.IsTrue(closest[0].Node.Data.Name == "Vienna");
            Assert.IsTrue(closest[1].Node.Data.Name == "Paris");
            Assert.IsTrue(closest[2].Node.Data.Name == "London");
            Assert.IsTrue(closest[3].Node.Data.Name == "Rome");
        }

        [TestMethod]
        public void Check_Hartford_200km()
        {
            var hartford = new City("Hartford", 41.76, -72.67);

            var closest = tree.Lookup(hartford, tree.Root, 20, 200000.0);

            // only 3 cities are within 200km of Hartford
            Assert.AreEqual(3, closest.Count);
            Assert.IsTrue(closest[0].Node.Data.Name == "Troy");
            Assert.IsTrue(closest[1].Node.Data.Name == "Boston");
            Assert.IsTrue(closest[2].Node.Data.Name == "New York");
        }
    }
}
