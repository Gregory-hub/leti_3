using MathNet.Numerics.LinearAlgebra;
using System;
using System.Drawing;

namespace lab5
{
    internal class Point3D
    {
        public Vector<double> Coords { get; set; }

        public Point3D()
        {
            Coords = Vector<double>.Build.Dense(3);
        }

        public Point3D(Vector<double> coords)
        {
            coords.CopyTo(Coords);
        }

        public Point3D(double x, double y, double z)
        {
            Coords = Vector<double>.Build.DenseOfArray(new double[] { x, y, z });
        }

        public Point3D(int x, int y, int z)
        {
            Coords = Vector<double>.Build.DenseOfArray(new double[] { x, y, z });
        }

        public double this[int i]
        {
            get { return Coords[i]; }
            set { Coords[i] = value; }
        }

        public static Point3D operator +(Point3D point1, Point3D point2)
        {
            return new Point3D(point1.Coords + point2.Coords);
        }

        public static Point3D operator -(Point3D point1, Point3D point2)
        {
            return new Point3D(point1.Coords - point2.Coords);
        }

        public Point ToPoint()
        {
            Point point = new Point();
            point.X = (int)Coords[0];
            point.Y = (int)Coords[1];

            return point;
        }

        public void Draw(Graphics g, Brush brush, int fatness)
        {
            Point point = ToPoint();
            g.FillRectangle(brush, point.X - fatness / 2, point.Y - fatness / 2, fatness, fatness);
        }

        public void RotateByMatrix(Point3D center, Matrix<double> matrix)
        {
            Coords -= center.Coords;
            Coords = matrix * Coords;
            Coords += center.Coords;
        }
    }
}

