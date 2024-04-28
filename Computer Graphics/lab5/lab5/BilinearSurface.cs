using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab5
{
    internal class BilinearSurface
    {
        public List<Polygon> Polygons { get; }
        public List<Point3D> PointArray { get; }
        public List<Point3D> Corners { get; }
        public Rotation Rotation { get; }

        public Brush pointBrush = Brushes.Black;
        public Brush cornerBrush = Brushes.Black;
        public Brush surfaceBrush = Brushes.Cyan;

        public int pointFatness = 1;
        public int cornerFatness = 4;

        public BilinearSurface()
        {
            Polygons = new List<Polygon>();
            PointArray = new List<Point3D>();
            Corners = new List<Point3D>();
            Rotation = new Rotation();
        }

        public void Fill(Graphics g, bool drawCorners = false, bool drawPoints = false)
        {
            foreach (Polygon polygon in Polygons)
            {
                polygon.Fill(g, surfaceBrush, PointArray);
                if (drawCorners)
                {
                    foreach (Point3D corner in Corners)
                    {
                        corner.Draw(g, cornerBrush, cornerFatness);
                    }
                }
                if (drawPoints)
                {
                    foreach (int pointIndex in polygon.PointIndices)
                    {
                        PointArray[pointIndex].Draw(g, pointBrush, pointFatness);
                    }
                }
            }
        }

        public void DrawCornerPoints(Graphics g)
        {
            foreach (Point3D corner in Corners)
            {
                corner.Draw(g, cornerBrush, cornerFatness);
            }
        }

        public void Calculate(int gridDensity)
        {
            Polygons.Clear();
            PointArray.Clear();
            double du = 1.0 / (gridDensity / 1 - 1);
            double dw = 1.0 / (gridDensity / 1 - 1);

            for (int i = 0; i < gridDensity; i++)
            {
                double u = i * du;
                for (int j = 0; j < gridDensity; j++)
                {
                    double w = j * dw;
                    Point3D point = CalculatePointOnSurface(u, w);
                    PointArray.Add(point);
                }
            }

            Polygon polygon;
            for (int i = 0; i < gridDensity - 1; i++)
            {
                for (int j = 0; j < gridDensity - 1; j++)
                {
                    polygon = new Polygon(new int[] { gridDensity * i + j, gridDensity * i + j + 1, gridDensity * (i + 1) + j });
                    Polygons.Add(polygon);
                    polygon = new Polygon(new int[] { gridDensity * i + j + 1, gridDensity * (i + 1) + (j + 1), gridDensity * (i + 1) + j });
                    Polygons.Add(polygon);
                }
            }
        }

        private Point3D CalculatePointOnSurface(double u, double w)
        {
            Point3D point = new Point3D();
            if (Corners.Count != 4)
            {
                return point;
            }

            for (int i = 0; i < 3; i++)
            {
                point[i] = Corners[0][i] * (1 - u) * (1 - w) + Corners[1][i] * (1 - u) * w + Corners[2][i] * u * (1 - w) + Corners[3][i] * u * w;
            }

            return point;
        }

        public void Rotate(Point3D center, Vector<double> rotation)
        {
            Matrix<double> matrixX = GetXRotationMatrix(rotation);
            Matrix<double> matrixY = GetYRotationMatrix(rotation);

            foreach (Point3D point in PointArray)
            {
                point.RotateByMatrix(center, matrixX);
                point.RotateByMatrix(center, matrixY);
            }

            foreach (Point3D corner in Corners)
            {
                corner.RotateByMatrix(center, matrixX);
                corner.RotateByMatrix(center, matrixY);
            }

            Rotation.Matrix = matrixY * matrixX * Rotation.Matrix;
        }

        public void ResetRotation(Point3D center)
        {
            foreach (Point3D point in PointArray)
            {
                point.RotateByMatrix(center, Rotation.Matrix.Inverse());
            }

            foreach (Point3D corner in Corners)
            {
                corner.RotateByMatrix(center, Rotation.Matrix.Inverse());
            }

            Rotation.Clear();
        }

        private Matrix<double> GetXRotationMatrix(Vector<double> rotation)
        {
            double cos = Math.Cos(rotation[0]);
            double sin = Math.Sin(rotation[0]);

            double[,] rotX = {
                { 1, 0, 0},
                { 0, cos, -sin},
                { 0, sin, cos}
            };

            return Matrix<double>.Build.DenseOfArray(rotX);
        }

        private Matrix<double> GetYRotationMatrix(Vector<double> rotation)
        {
            double cos = Math.Cos(rotation[1]);
            double sin = Math.Sin(rotation[1]);

            double[,] rotY = {
                { cos, 0, sin},
                { 0, 1, 0},
                { -sin, 0, cos}
            };

            return Matrix<double>.Build.DenseOfArray(rotY);
        }

    }
}

