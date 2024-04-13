using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab2
{
	public partial class Form1 : Form
	{
		Label errorMessageLabel;
		Graphics g;
		SolidBrush brush;
		Pen pen;
		Pen linePen;

		int pointFatness = 4;

		List<Point> points;
		int requiredNumberOfPoints = 3;
		int splineOrder = 2;

		public Form1()
		{
			InitializeComponent();
			errorMessageLabel = label6;
            errorMessageLabel.Text = "";

			g = pictureBox1.CreateGraphics();
			brush = new SolidBrush(Color.Black);
			pen = new Pen(Color.Blue);
			linePen = new Pen(Color.Cyan);

			points = new List<Point>();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (EnoughCoordsEntered())
			{
				errorMessageLabel.Text = "";
                g.Clear(Color.White);
				DrawLines();
				DrawPoints();
				DrawSpline();
			} else
			{
				errorMessageLabel.Text = "Please enter at least 3 points";
			}
		}

        private void button2_Click(object sender, EventArgs e)
        {
            errorMessageLabel.Text = "";
			points.Clear();
			g.Clear(Color.White);
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
			points.Add(new Point(e.X, e.Y));
            DrawPoint(e.X, e.Y, pointFatness);
        }

		private void DrawPoints()
		{

			for (int i = 0; i < points.Count; i++)
			{
				DrawPoint(points[i].X, points[i].Y, pointFatness);
			}

		}
		
		private void DrawLines()
		{
			for (int i = 0; i < points.Count - 1; i++)
			{
				DrawLine(points[i], points[i + 1]);
            }
        }

		private void DrawLine(Point p1, Point p2)
		{
            g.DrawLine(linePen, p1, p2);
		}

		private void DrawPoint(int x, int y, int fatness)
		{
            g.FillRectangle(brush, x - fatness / 2, y - fatness / 2, fatness, fatness);
		}

		private void DrawSpline()
		{
			int numberOfPointsDrawn = 200;
			Point[] resultPoints = new Point[numberOfPointsDrawn];

			float t = 0f;
			float step = 1f / numberOfPointsDrawn;

			for (int i = 0; i < numberOfPointsDrawn; i++)
			{
				resultPoints[i] = Bezier(points, t);
				t += step;
			}

			for (int i = 0; i < numberOfPointsDrawn - 1; i++)
			{
				g.DrawLine(pen, resultPoints[i].X, resultPoints[i].Y, resultPoints[i + 1].X, resultPoints[i + 1].Y);
			}

			int iResult = resultPoints.Length - 1;
			int iPoints = points.Count - 1;
            g.DrawLine(pen, resultPoints[iResult].X, resultPoints[iResult].Y, points[iPoints].X, points[iPoints].Y);
		}

		private Point Bezier(List<Point> P, float t)
		{
			if (P.Count == 1) return P[0];

			List<Point> newP = new List<Point>();
			for (int i = 0; i < P.Count - 1; i++)
			{
                Point point = new Point();
				point.X = (int)((1 - t) * P[i].X + t * P[i + 1].X);
				point.Y = (int)((1 - t) * P[i].Y + t * P[i + 1].Y);
				newP.Add(point);
			}
			return Bezier(newP, t);
		}

		private bool EnoughCoordsEntered()
		{
			return points.Count() >= requiredNumberOfPoints;
		}

    }
}

