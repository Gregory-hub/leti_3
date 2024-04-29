using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
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

        public Point3D[] GetCorners(List<Point3D> pointArray)
        {
            Point3D[] points = new Point3D[3];
            points[0] = pointArray[PointIndices[0]];
            points[1] = pointArray[PointIndices[1]];
            points[2] = pointArray[PointIndices[2]];
            return points;
        }

        public double GetZValue(List<Point3D> pointArray, Point point)
        {
            double[] coefficients = GetPlaneEquation(pointArray);
            double z = -(coefficients[0] * point.X + coefficients[1] * point.Y + coefficients[3]) / coefficients[2];
            return z;
        }

        public List<Point> Get2DPointsInsidePolygon(List<Point3D> pointArray)
        {
            List<Point> points = new List<Point>();
            Point[] corners = GetCorners(pointArray).Select((p) => p.ToPoint()).ToArray();

            int maxYCornerIndex = 0;
            int middleYCornerIndex = 1;
            int minYCornerIndex = 2;
            for (int i = 0; i < corners.Length; i++)
            {
                // DANGER!!! hard to understand. Should work though
                if (corners[i].Y < corners[minYCornerIndex].Y)
                {
                    minYCornerIndex = i;
                }
                else if (corners[i].Y > corners[maxYCornerIndex].Y)
                {
                    maxYCornerIndex = i;
                }
                middleYCornerIndex = 3 - minYCornerIndex - maxYCornerIndex;
            }
            Point maxYCorner = corners[maxYCornerIndex];
            Point middleYCorner = corners[middleYCornerIndex];
            Point minYCorner = corners[minYCornerIndex];

            for (int y = maxYCorner.Y; y > minYCorner.Y; y--)
            {
                int[] xBorders = GetHorizontalLineXBorders(maxYCorner, middleYCorner, minYCorner, y);
                for (int x = xBorders[0]; x <= xBorders[1]; x++)
                {
                    points.Add(new Point(x, y));
                }
            }

            return points;
        }

        private int[] GetHorizontalLineXBorders(Point maxYCorner, Point middleYCorner, Point minYCorner, int y)
        {
            int[] borders = new int[2];

            double k;
            double m;
            if (maxYCorner.X == minYCorner.X)
            {
                borders[1] = maxYCorner.X;
            }
            else
            {
                k =  (double)(maxYCorner.Y - minYCorner.Y) / (maxYCorner.X - minYCorner.X);
                m = maxYCorner.Y - k * maxYCorner.X;
                borders[1] = (int)Math.Round((y - m) / k);
            }

            if (y > middleYCorner.Y)
            {
                if (maxYCorner.X == middleYCorner.X)
                {
                    borders[0] = maxYCorner.X;
                }
                else
                {
                    k = (double)(maxYCorner.Y - middleYCorner.Y) / (maxYCorner.X - middleYCorner.X);
                    m = maxYCorner.Y - k * maxYCorner.X;
                    borders[0] = (int)Math.Round((y - m) / k);
                }
            }
            else
            {
                if (minYCorner.X == middleYCorner.X)
                {
                    borders[0] = minYCorner.X;
                }
                else
                {
                    k =  (double)(middleYCorner.Y - minYCorner.Y) / (middleYCorner.X - minYCorner.X);
                    m = minYCorner.Y - k * minYCorner.X;
                    borders[0] = (int)Math.Round((y - m) / k);
                }
            }

            return borders.OrderBy(x => x).ToArray();
        }

        private double[] GetPlaneEquation(List<Point3D> pointArray)
        {
            double[] coefficients = new double[4];
            Point3D[] corners = GetCorners(pointArray);
            Vector<double> v1 = corners[1].Coords - corners[0].Coords;
            Vector<double> v2 = corners[2].Coords - corners[0].Coords;

            Vector<double> normal = Cross(v1, v2);

            coefficients[0] = normal[0];
            coefficients[1] = normal[1];
            coefficients[2] = normal[2];

            Point3D point = corners[0];
            coefficients[3] = -(point[0] * coefficients[0] + point[1] * coefficients[1] + point[2] * coefficients[2]);

            return coefficients;
        }

        private Vector<double> Cross(Vector<double> v1, Vector<double> v2)
        {
            Vector<double> result = Vector<double>.Build.Dense(3);
            result[0] = v1[1] * v2[2] - v1[2] * v2[1];
            result[1] = v1[2] * v2[0] - v1[0] * v2[2];
            result[2] = v1[0] * v2[1] - v1[1] * v2[0];
            return result;
        }

        //public bool PointIsInside(Point3D point, List<Point3D> pointArray)
        //{
        //    // Sdf for triangle in 3d
        //    // https://www.shadertoy.com/view/4sXXRN

        //    Point3D v1 = pointArray[PointIndices[0]];
        //    Point3D v2 = pointArray[PointIndices[1]];
        //    Point3D v3 = pointArray[PointIndices[2]];

        //    Point3D v21 = v2 - v1;
        //    Point3D p1 = point - v1;
        //    Point3D v32 = v3 - v2;
        //    Point3D p2 = point - v2;
        //    Point3D v13 = v1 - v3;
        //    Point3D p3 = point - v3;
        //    Point3D normal = Cross(v21, v13);

        //    Point3D vp21 = v21 * Clamp(v21 * p1 / (v21 * v21), 0.0, 1.0) - p1;
        //    Point3D vp32 = v32 * Clamp(v32 * p2 / (v32 * v32), 0.0, 1.0) - p2;
        //    Point3D vp13 = v13 * Clamp(v13 * p3 / (v13 * v13), 0.0, 1.0) - p3;
        //    double distance = Math.Sqrt((Math.Sign(Cross(v21, normal) * p1)) +
        //                  Math.Sign(Cross(v32, normal) * p2) +
        //                  Math.Sign(Cross(v13, normal) * p3) < 2.0
        //                  ?
        //                  Math.Min(Math.Min(
        //                  vp21 * vp21,
        //                  vp32 * vp32),
        //                  vp13 * vp13)
        //                  :
        //                  normal * p1 * normal * p1 / (normal * normal));

        //    return distance < 1;
        //}

        //private double Clamp(double value, double min, double max)
        //{
        //    return Math.Max(min, Math.Min(max, value));
        //}
    }
}

