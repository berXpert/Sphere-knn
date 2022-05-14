using System;
using System.Collections.Generic;
using System.Linq;

namespace BerXpert.SphereKnn
{
    public class SphereSearch<T>
    {
        private Tree<T> kd;
        public SphereSearch(IList<T> points, Func<T, Point3d> getPosition)
        {
            kd = new Tree<T>(points, getPosition);
        }

        /// <summary>
        /// Return a list of n near-neighbors to the search element
        /// </summary>
        /// <param name="search"></param>
        /// <param name="n"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public IEnumerable<T> Lookup(T search, int n, double max = double.MaxValue)
        {
            if (kd == null)
            {
                return null;
            }
            var q = from s in kd.Lookup(search, kd.Root, n, max)
                    select s.Node.Data;

            return q;
        }

        /// <summary>
        /// Return a list of n points and its distance to the search element
        /// </summary>
        /// <param name="search"></param>
        /// <param name="n"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public IEnumerable<Tuple<T, double>> LookupDistance(T search, int n, double max = double.MaxValue)
        {
            if (kd == null)
            {
                return null;
            }
            var q = from s in kd.Lookup(search, kd.Root, n, max)
                    select new Tuple<T, double>(s.Node.Data, s.Distance);

            return q;
        }
    }
}
