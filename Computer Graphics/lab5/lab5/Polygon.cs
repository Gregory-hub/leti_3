using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace lab5
{
    internal class Polygon
    {
        public int[] PointIndices { get; set; }

        public Polygon(int[] pointIndices)
        {
            if (pointIndices.Length != 3) throw new ArgumentException("Polygon must have 3 vertices");

            PointIndices = new int[3];
            for (int i = 0; i < pointIndices.Length; i++)
            {
                PointIndices[i] = pointIndices[i];
            }
        }

        public void Fill(Graphics g, Brush brush, List<Point3D> pointArray)
        {
            Point[] points = new Point[3];
            for (int i = 0; i < PointIndices.Length; i++)
            {
                points[i] = pointArray[PointIndices[i]].ToPoint();
            }
            g.FillPolygon(brush, points);
        }
    }
}

