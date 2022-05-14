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


namespace BerXpert.SphereKnn
{
    public class Node<T>
    {
        public T Data { get; private set; }
        public int Axis { get; set; }
        public double Split { get; set; }
        public Node<T> Left { get; set; }
        public Node<T> Right { get; set; }

        public Point3d Position { get; set; }
        public Node(T data, Point3d position)
        {
            this.Data = data;
            this.Position = position;
        }

        public Node(T data, int axis, double splitPoint, Node<T> left, Node<T> right)
        {
            this.Data = data;
            this.Axis = axis;
            this.Split = splitPoint;
            this.Left = left;
            this.Right = right;
        }
    }
}
