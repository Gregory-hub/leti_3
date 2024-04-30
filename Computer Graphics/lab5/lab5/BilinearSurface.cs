using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        public Brush surfaceFrontBrush = Brushes.Cyan;
        public Brush surfaceBackBrush = Brushes.Cyan;
        public Pen polygonBorderPen = Pens.Black;

        public int pointFatness = 1;
        public int cornerFatness = 4;

        private enum PixelType { Point, SurfaceFront, SurfaceBack, Background }

        public BilinearSurface()
        {
            Polygons = new List<Polygon>();
            PointArray = new List<Point3D>();
            Corners = new List<Point3D>();
            Rotation = new Rotation();
        }

        public void Fill(Graphics g, PictureBox pictureBox)
        {
            double[,] zBuffer = new double[pictureBox.Height, pictureBox.Width];
            PixelType[,] frameBuffer = new PixelType[pictureBox.Height, pictureBox.Width];
            FillBuffers(ref zBuffer, ref frameBuffer, pictureBox);
            DrawPixels(g, frameBuffer);
        }

        private void FillBuffers(ref double[,] zBuffer, ref PixelType[,] frameBuffer, PictureBox pictureBox)
        {
            for (int y = 0; y < zBuffer.GetLength(0); y++)
            {
                for (int x = 0; x < zBuffer.GetLength(1); x++)
                {
                    zBuffer[y, x] = double.NegativeInfinity;
                    frameBuffer[y, x] = PixelType.Background;
                }
            }

            foreach (Polygon polygon in Polygons)
            {
                List<Point> points = polygon.Get2DPointsInsidePolygon(PointArray);
                Point[] corners = polygon.GetCorners(PointArray).Select(c => c.ToPoint()).ToArray();
                foreach (Point point in points)
                {
                    if (!(point.X < 0 || point.X >= pictureBox.Width || point.Y < 0 || point.Y >= pictureBox.Height))
                    {
                        double z = polygon.GetZValue(PointArray, point);
                        if (z > zBuffer[point.Y, point.X])
                        {
                            zBuffer[point.Y, point.X] = z;

                            if (polygon.CornersArrangedClockwise(PointArray))
                            {
                                frameBuffer[point.Y, point.X] = PixelType.SurfaceFront;
                            }
                            else
                            {
                                frameBuffer[point.Y, point.X] = PixelType.SurfaceBack;
                            }

                            foreach (Point corner in corners)
                            {
                                if (corner.X == point.X && corner.Y == point.Y)
                                {
                                    frameBuffer[point.Y, point.X] = PixelType.Point;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void DrawPixels(Graphics g, PixelType[,] frameBuffer)
        {
            for (int y = 0; y < frameBuffer.GetLength(0); y++)
            {
                for (int x = 0; x < frameBuffer.GetLength(1); x++)
                {
                    if (frameBuffer[y, x] == PixelType.Point)
                    {
                        g.FillRectangle(pointBrush, x, y, 1, 1);
                    }
                    else if (frameBuffer[y, x] == PixelType.SurfaceFront)
                    {
                        g.FillRectangle(surfaceFrontBrush, x, y, 1, 1);
                    }
                    else if (frameBuffer[y, x] == PixelType.SurfaceBack)
                    {
                        g.FillRectangle(surfaceBackBrush, x, y, 1, 1);
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

