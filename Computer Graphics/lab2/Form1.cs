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
	public struct Vector2
	{
		public int x, y;

		public Vector2(int x, int y)
		{
			this.x = x;
			this.y = y;
		}
	}

	public partial class Form1 : Form
	{
		Label errorMessageLabel;
		Graphics g;
		SolidBrush brush;
		Pen pen;

		Vector2[] points;
		int numberOfPoints = 4;
		int splineOrder = 2;

		int[] knots;

		public Form1()
		{
			InitializeComponent();
			errorMessageLabel = label6;

			g = pictureBox1.CreateGraphics();
			brush = new SolidBrush(Color.Black);
			pen = new Pen(Color.Blue);

			points = new Vector2[numberOfPoints];
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (AllCoordsEntered())
			{
                g.Clear(Color.White);
				DrawPoints();
				DrawSpline();
			}
		}

		private void DrawSpline()
		{
			int numberOfPointsDrawn = 200;
			Vector2[] resultPoints = new Vector2[numberOfPointsDrawn];

			float t = 0f;
			float step = 1f / numberOfPointsDrawn;

			for (int i = 0; i < numberOfPointsDrawn; i++)
			{
				resultPoints[i] = Bezier(points, t);
				t += step;
			}

			for (int i = 0; i < numberOfPointsDrawn - 1; i++)
			{
				g.DrawLine(pen, resultPoints[i].x, resultPoints[i].y, resultPoints[i + 1].x, resultPoints[i + 1].y);
			}

			int iResult = resultPoints.Length - 1;
			int iPoints = points.Length - 1;
            g.DrawLine(pen, resultPoints[iResult].x, resultPoints[iResult].y, points[iPoints].x, points[iPoints].y);
		}

		private Vector2 Bezier(Vector2[] P, float t)
		{
			if (P.Length == 1) return P[0];

			Vector2[] newP = new Vector2[P.Length - 1];
			for (int i = 0; i < newP.Length; i++)
			{
				newP[i] = new Vector2();
				newP[i].x = (int)((1 - t) * P[i].x + t * P[i + 1].x);
				newP[i].y = (int)((1 - t) * P[i].y + t * P[i + 1].y);
			}
			return Bezier(newP, t);
		}

		//private Vector2 BSpline(int t)
		//{
		//	float x = 0, y = 0;
		//	for (int i = 0; i < points.Length; i++)
		//	{
		//		FVector2 j = J(i, splineOrder, t);
		//		x += points[i].x * j.x;
		//		y += points[i].y * j.y;
		//	}
		//	return new Vector2((int)x, (int)y);
		//}

		//private FVector2 J(int i, int k, int t)
		//{
		//	if (k == 1)
		//	{
		//		return new FVector2();
		//	}
		//}

		private void DrawPoints()
		{

			for (int i = 0; i < points.Length; i++)
			{
				g.FillRectangle(brush, points[i].x - 2, points[i].y - 2, 4, 4);
			}

		}

		private void ProcessCoordinateInput(TextBox textBox, int pointIndex, char coord)
		{
			// puts coordinates from textboxes into the points array
			if (coord != 'x' && coord != 'y') throw new Exception("Invalid coord (must be 'x' or 'y')");

			if (textBox.Text.Length > 0)
			{
				try
				{
					if (coord == 'x')
					{
						points[pointIndex].x = Convert.ToInt32(textBox.Text);
					}
					else if (coord == 'y')
					{
						points[pointIndex].y = Convert.ToInt32(textBox.Text);
					}
					errorMessageLabel.Text = "";
				}
				catch (Exception ex) when (ex is System.FormatException || ex is System.OverflowException)
				{
					errorMessageLabel.Text = ex.Message;
				}
			}
			else
			{
				errorMessageLabel.Text = "";
			}
		}

		private bool AllCoordsEntered()
		{
			return
				textBox1.Text != "" &&
				textBox2.Text != "" &&
				textBox3.Text != "" &&
				textBox4.Text != "" &&
				textBox5.Text != "" &&
				textBox6.Text != "";
		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			ProcessCoordinateInput(textBox1, 0, 'x');
		}

		private void textBox2_TextChanged(object sender, EventArgs e)
		{
			ProcessCoordinateInput(textBox2, 0, 'y');
		}

		private void textBox4_TextChanged(object sender, EventArgs e)
		{
			ProcessCoordinateInput(textBox4, 1, 'x');
		}

		private void textBox3_TextChanged(object sender, EventArgs e)
		{
			ProcessCoordinateInput(textBox3, 1, 'y');
		}

		private void textBox6_TextChanged(object sender, EventArgs e)
		{
			ProcessCoordinateInput(textBox6, 2, 'x');
		}

		private void textBox5_TextChanged(object sender, EventArgs e)
		{
			ProcessCoordinateInput(textBox5, 2, 'y');
		}

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
			ProcessCoordinateInput(textBox7, 3, 'x');
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
			ProcessCoordinateInput(textBox8, 3, 'y');
        }
    }
}

