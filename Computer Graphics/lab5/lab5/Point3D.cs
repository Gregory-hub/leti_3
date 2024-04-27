using MathNet.Numerics.LinearAlgebra;
using System;

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
            Coords = coords;
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

        public void RotateAround(Point3D center, Rotation rotation)
        {
            Matrix<double> matrixX = GetXRotationMatrix(rotation[0]);
            Matrix<double> matrixY = GetYRotationMatrix(rotation[1]);
            Rotate(center, matrixX);
            Rotate(center, matrixY);
        }

        public void RotateX(Point3D center, Rotation rotation)
        {
            Matrix<double> matrixX = GetYRotationMatrix(rotation[0]);
            Rotate(center, matrixX);
        }

        public void RotateY(Point3D center, Rotation rotation)
        {
            Matrix<double> matrixY = GetYRotationMatrix(rotation[1]);
            Rotate(center, matrixY);
        }

        private void Rotate(Point3D center, Matrix<double> matrix)
        {
            Coords -= center.Coords;
            Coords = matrix * Coords;
            Coords += center.Coords;
        }

        private Matrix<double> GetXRotationMatrix(double angle)
        {
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);

            double[,] rotX = {
                { 1, 0, 0},
                { 0, cos, -sin},
                { 0, sin, cos}
            };

            return Matrix<double>.Build.DenseOfArray(rotX);
        }

        private Matrix<double> GetYRotationMatrix(double angle)
        {
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);

            double[,] rotY = {
                { cos, 0, sin},
                { 0, 1, 0},
                { -sin, 0, cos}
            };

            return Matrix<double>.Build.DenseOfArray(rotY);
        }
    }
}

