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
                for (int x = xBorders[0]; x < xBorders[1]; x++)
                {
                    points.Add(new Point(x, y));
                }
            }

            return points;
        }

        private int[] GetHorizontalLineXBorders(Point maxYCorner, Point middleYCorner, Point minYCorner, int y)
        {
            int[] borders = new int[2];

            double[] lineCoefficients;
            double k, m;
            if (maxYCorner.X == minYCorner.X)
            {
                borders[1] = maxYCorner.X;
            }
            else
            {
                lineCoefficients = GetLineEquation(maxYCorner, minYCorner);
                k = lineCoefficients[0];
                m = lineCoefficients[1];
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
                    lineCoefficients = GetLineEquation(maxYCorner, middleYCorner);
                    k = lineCoefficients[0];
                    m = lineCoefficients[1];
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
                    lineCoefficients = GetLineEquation(middleYCorner, minYCorner);
                    k = lineCoefficients[0];
                    m = lineCoefficients[1];
                    borders[0] = (int)Math.Round((y - m) / k);
                }
            }

            return borders.OrderBy(x => x).ToArray();
        }

        public bool CornersArrangedClockwise(List<Point3D> pointArray)
        {
            Point[] points = GetCorners(pointArray).Select(p => p.ToPoint()).ToArray();
            if (points[0].X == points[1].X)
            {
                if (points[0].Y > points[1].Y)
                {
                    return points[2].X < points[0].X;
                }
                else
                {
                    return points[2].X > points[0].X;
                }
            }

            double[] lineCoefficients = GetLineEquation(points[0], points[1]);
            double k = lineCoefficients[0];
            double m = lineCoefficients[1];

            if (points[0].X < points[1].X)
            {
                return points[2].Y < k * points[2].X + m;
            }
            else
            {
                return points[2].Y > k * points[2].X + m;
            }
        }

        private double[] GetLineEquation(Point point1, Point point2)
        {
            // y = kx + m
            if (point1.X == point2.X) throw new ArgumentException("Method does not work for line equations like x = c");
            double k = (double)(point1.Y - point2.Y) / (point1.X - point2.X);
            double m = point2.Y - k * point2.X;
            return new double[] { k, m };
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
    }
}

