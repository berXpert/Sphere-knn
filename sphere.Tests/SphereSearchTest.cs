using Xunit;
using BerXpert.SphereKnn;
using System;
using System.Collections.Generic;

namespace sphere.Tests
{

    public class SphereSearchTest
    {
        private List<City> cities;
        private Tree<City> tree;

        public SphereSearchTest()
        {
            cities = new List<City>
            {
                new City("Boston",    42.358, -71.064),
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
        
        [Fact]
        public void Build_Tree_of_ints()
        {
            Tree<int> tree;
            var ints = new int[3];

            tree = new Tree<int>(ints, new Func<int, double[]?>(p => null));
            Assert.NotNull(tree);
        }

        [Fact]
        public void Check_Philadelphia()
        {
            var tree = new Tree<City>(cities, new Func<City, double[]>(c => Cartesian.FromSpherical(c.Latitude, c.Longitude)));

            var philadelphia = new City("Philadelphia", 39.95, -75.17);

            var closest = tree.Lookup(philadelphia, tree.Root, 4);

            Assert.Equal(4, closest.Count);
            Assert.Equal("New York", closest[0].Node.Data.Name);
            Assert.Equal("Troy", closest[1].Node.Data.Name);
            Assert.Equal("Boston", closest[2].Node.Data.Name);
            Assert.Equal("Miami", closest[3].Node.Data.Name);

            var p = Cartesian.FromSpherical(philadelphia.Latitude, philadelphia.Longitude);
            Console.WriteLine("{0} - ({1},{2},{3})", philadelphia.Name, p[0], p[1], p[2]);

            foreach (var item in closest)
            {
                var q = Cartesian.FromSpherical(item.Node.Data.Latitude, item.Node.Data.Longitude);
                Console.WriteLine("{0} : At: {1} | ({2},{3},{4})", item.Node.Data.Name, item.Distance, q[0], q[1], q[2]);
            }
        }

        [Fact]
        public void Check_Hawaii()
        {
            var hawaii = new City("Hawaii", 21.31, -157.8);

            var closest = tree.Lookup(hawaii, tree.Root, 2);

            Assert.Equal(2, closest.Count);
            Assert.Equal("Tokyo", closest[0].Node.Data.Name);
            Assert.Equal("Seoul", closest[1].Node.Data.Name);
        }  

        [Fact]
        public void Check_Berlin()
        {
            var berlin = new City("Berlin", 52.50, 13.40);

            var closest = tree.Lookup(berlin, tree.Root, 4);

            Assert.Equal(4, closest.Count);
            Assert.Equal("Vienna", closest[0].Node.Data.Name);
            Assert.Equal("Paris", closest[1].Node.Data.Name);
            Assert.Equal("London", closest[2].Node.Data.Name);
            Assert.Equal("Rome", closest[3].Node.Data.Name);
        }      

        [Fact] 
        public void Check_Hartford_200km()
        {
            var hartford = new City("Hartford", 41.76, -72.67);

            var closest = tree.Lookup(hartford, tree.Root, 20, 200000.0);

            // only 3 cities are within 200km of Hartford
            Assert.Equal(3, closest.Count);
            Assert.Equal("Troy", closest[0].Node.Data.Name);
            Assert.Equal("Boston", closest[1].Node.Data.Name);
            Assert.Equal("New York", closest[2].Node.Data.Name);
        }  
    }
}