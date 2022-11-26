﻿#region License
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
using System.Collections.Generic;
using System.Linq;

namespace BerXpert.SphereKnn
{
    public class Tree<T>
    {
        public Node<T> Root { get; }

        private readonly Func<T, Point3d> GetPosition;

        private readonly List<Node<T>> NodeData;

        private Point3d Position(T item)
        {
            return this.GetPosition(item) ?? new(1, 1, 1);
        }

        /// <summary>
        /// Build a Tree
        /// </summary>
        /// <param name="data">List of elements used for search</param>
        /// <param name="getPosition">A function that provides a position in Cartesian form as an array [x,y,z] for the given data</param>
        public Tree(IList<T> data, Func<T, Point3d> getPosition)
        {
            GetPosition = getPosition ?? new Func<T, Point3d>(_ => new(1, 1, 1));

            NodeData = new List<Node<T>>();

            if (data != null)
            {
                foreach (var item in data)
                {
                    NodeData.Add(new Node<T>() { Data = item, Position = Position(item) });
                }
            }

            Root = BuildRectangle(NodeData, 0);
        }

        private Node<T> BuildRectangle(List<Node<T>> points, int depth)
        {
            //Console.WriteLine("depth: {0}", depth);
            if (points == null)
            {
                return null;
            }

            if (points.Count == 0)
            {
                return null;
            }

            if (points.Count == 1)
            {
                return new Node<T>() { Data = points[0].Data, Position = points[0].Position };
            }

            var axis = depth % 3; //Position(points[0].Data).Length; -> for true knn the mod is with "n"

            points = points.OrderBy(p => p.Position[axis]).ToList();
            //points = (from o in points
            //          orderby Position(o.Data)[axis] descending
            //          select o).ToList();

            var i = points.Count / 2;

            ++depth;

            return new Node<T>(points[i].Data,
                                axis,
                                points[i].Position[axis],
                                BuildRectangle(points.GetRange(0, i), depth),
                                BuildRectangle(points.GetRange(i, points.Count - i), depth)
                );
        }

        public List<NodeMeasured<T>> Lookup(T search, Node<T> node, int n)
        {
            return Lookup(search, node, n, 100000000);
        }

        private readonly double invEarthDiameter = 1.0 / 12742018.0; /* meters */

        /// <summary>
        /// Return the n elements closes to the search element for a given startingNode, withing max kilometers
        /// </summary>
        /// <param name="search"></param>
        /// <param name="startingNode"></param>
        /// <param name="n">Return N elements</param>
        /// <param name="max">Maxim distance to look for</param>
        /// <returns></returns>
        public List<NodeMeasured<T>> Lookup(T search, Node<T> node, int n, double max)
        {
            if (max > 0)
            {
                max = 2 * Math.Sin(max * invEarthDiameter);
                //max = 10000;
            }

            var result = new List<NodeMeasured<T>>();
            if (node == null || max <= 0 || n <= 0)
            {
                return result;
            }

            var stack = new Stack<object>();
            stack.Push(node);
            stack.Push(0.0);

            while (stack.Count > 0)
            {
                double dist = (double)stack.Pop();
                node = (Node<T>)stack.Pop();

                if (node == null)
                {
                    continue;
                }

                // If this subtree is further away than we care about, then skip it.
                if (dist > max)
                {
                    continue;
                }

                // If we've already found enough locations, and the furthest one is closer
                // than this subtree possibly could be, just skip the subtree. 
                if (result.Count == n && result[^1].Distance < dist * dist)
                {
                    continue;
                }

                //Iterate all the way down the tree, adding nodes that we need to remember
                //to visit later onto the stack.
                var searchPosition = Position(search);
                while (node != null)
                {
                    Node<T> m;

                    if (searchPosition[node.Axis] < node.Split)
                    {
                        stack.Push(node.Right);
                        stack.Push(node.Split - searchPosition[node.Axis]);
                        m = node.Left;
                    }
                    else
                    {
                        stack.Push(node.Left);
                        stack.Push(searchPosition[node.Axis] - node.Split);
                        m = node.Right;
                    }

                    if (m != null)
                    {
                        node = m;
                    }
                    else
                    {
                        break;
                    }
                }

                // Once hit a leaf node, insert into the list of candidates, making sure the list keep sorted
                dist = Distance(searchPosition, node.Position);

                if (dist <= max * max)
                {
                    var candidate = new NodeMeasured<T>(node, dist);
                    var index = result.BinarySearch(candidate, new CompareByDistance<T>());

                    if (index < 0)
                    {
                        result.Insert(~index, candidate);
                    }

                    if (result.Count > n)
                    {
                        result.Remove(result.Last());
                    }
                }
            }
            return result;
        }

        private static double Distance(Point3d a, Point3d b)
        {
            var i = 3;
            double d = 0;
            double k;

            while ((i--) > 0)
            {
                k = b[i] - a[i];
                d += k * k;
            }
            return d;
        }
    }
}
