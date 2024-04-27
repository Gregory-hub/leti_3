using System;

namespace lab5
{
    internal class Polygon
    {
        public Point3D[] points;
        public int[] PointIndices { get; set; }

        public Polygon(Point3D[] pointsArray, int[] pointIndices)
        {
            if (pointIndices.Length != 3) throw new ArgumentException("Polygon must have 3 vertices");
            foreach (int index in pointIndices)
            {
                if (index >= pointsArray.Length) throw new ArgumentException("Index out of pointsArray bounds");
            }

            points = pointsArray;
            PointIndices = pointIndices;
        }
    }
}
