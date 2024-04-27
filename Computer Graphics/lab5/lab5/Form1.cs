using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using MathNet.Numerics.LinearAlgebra;

namespace lab5
{
    public partial class Form1 : Form
    {
        private Vector<double>[] corners = new Vector<double>[4];
        private int cornerIndex = 0;
        private Vector<double> center = Vector<double>.Build.DenseOfArray(new[] { 0d, 0d, 0d });
        private Vector<double>[,] surfacePoints;

        private bool rightMousePressed = false;
        private Point mouseDownPoint = new Point(0, 0);
        private Vector<double> rotation = Vector<double>.Build.Dense(2);
        private Vector<double> deltaRotation = Vector<double>.Build.Dense(2);

        private bool cursorHidden = false;
        private Point cursorFixPosition;

        private int usualPointSize = 2;
        private int cornerPointSize = 5;
        private double sensitivityX = 0.01;
        private double sensitivityY = 0.01;

        public int GridDensity
        {
            get { return int.TryParse(textBox1.Text, out int density) ? density : 0; }
        }

        public bool FreezeX
        {
            get { return checkBox1.Checked; }
        }

        public bool FreezeY
        {
            get { return checkBox2.Checked; }
        }

        public Form1()
        {
            InitializeComponent();
            textBox1.Text = "50";
            center[0] = pictureBox1.Width / 2;
            center[1] = pictureBox1.Height / 2;
        }

        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (corners.All(p => p != null) && GridDensity != 0)
            {
                label2.Text = "";
                surfacePoints = CalculateBilinearSurface(corners, GridDensity);
                DrawBilinearSurface(e.Graphics);
            }
            else
            {
                DrawCornerPoints(e.Graphics);
            }

            textBox2.Text = (rotation[0] * 180 / Math.PI).ToString();
            textBox3.Text = (rotation[1] * 180 / Math.PI).ToString();
            DisplayMessage();
        }

        private void PictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && cornerIndex < corners.Length)
            {
                Vector<double> point = Vector<double>.Build.DenseOfArray(new[] { e.Location.X, e.Location.Y, 0d });
                //RotatePointY(ref point, -rotation[1]);
                //RotatePointX(ref point, -rotation[0]);
                corners[cornerIndex] = point;
                cornerIndex++;

                pictureBox1.Refresh();
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (rightMousePressed)
            {
                if (!FreezeX)
                {
                    deltaRotation[0] = -(e.Y - mouseDownPoint.Y) * sensitivityX;
                    rotation[0] += deltaRotation[0];
                }

                if (!FreezeY)
                {
                    deltaRotation[1] = (e.X - mouseDownPoint.X) * sensitivityY;
                    rotation[1] += deltaRotation[1];
                }

                pictureBox1.Refresh();
                Cursor.Position = cursorFixPosition;
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                rightMousePressed = true;
                mouseDownPoint.X = e.X;
                mouseDownPoint.Y = e.Y;
                if (!cursorHidden)
                {
                    Cursor.Hide();
                    cursorHidden = true;
                    cursorFixPosition = Cursor.Position;
                }
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                rightMousePressed = false;
                if (cursorHidden)
                {
                    Cursor.Show();
                    cursorHidden = false;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            rotation.Clear();
            pictureBox1.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox1.Refresh();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void DisplayMessage()
        {
            if (GridDensity <= 1)
            {
                label2.ForeColor = Color.Red;
                label2.Text = "Invalid grid density";
            }
            else
            {
                label2.ForeColor = Color.Black;
                label2.Text = "Use LMB to Enter 4 corner points\nUse RMB to turn";
            }
        }

        private void DrawCornerPoints(Graphics g)
        {
            for (int i = 0; i < corners.Length; i++)
            {
                Vector<double> corner = corners[i];
                if (corner != null)
                {
                    Vector<double> point = corner;
                    //RotatePointX(ref point, rotation[0]);
                    //RotatePointY(ref point, rotation[1]);
                    DrawPoint(point, cornerPointSize, g);
                }
            }
        }

        private void DrawPoint(Vector<double> point, int fatness, Graphics g)
        {
            g.FillRectangle(Brushes.Black, (float)point[0] - fatness / 2, (float)point[1] - fatness / 2, fatness, fatness);
        }

        private void DrawLine(Vector<double> point1, Vector<double> point2, Graphics g)
        {
            if (GridDensity > 1)
            {
                g.DrawLine(Pens.Black, new Point((int)point1[0], (int)point1[1]), new Point((int)point2[0], (int)point2[1]));
            }
        }

        private Vector<double>[,] CalculateBilinearSurface(Vector<double>[] corners, int gridDensity)
        {
            Vector<double>[,] points = new Vector<double>[gridDensity, gridDensity];

            double du = 1.0 / (gridDensity / 1 - 1);
            double dw = 1.0 / (gridDensity / 1 - 1);

            for (int i = 0; i < gridDensity; i++)
            {
                double u = i * du;
                for (int j = 0; j < gridDensity; j++)
                {
                    double w = j * dw;
                    Vector<double> point = CalculatePointOnSurface(corners, u, w);
                    points[i, j] = point;
                }
            }

            return points;
        }

        private void DrawBilinearSurface(Graphics g)
        {
            for (int i = 0; i < surfacePoints.GetLength(0) - 1; i++)
            {
                for (int j = 0; j < surfacePoints.GetLength(1) - 1; j++)
                {
                    FillShape(g, surfacePoints[i, j], surfacePoints[i, j + 1], surfacePoints[i + 1, j], surfacePoints[i + 1, j + 1]);
                    g.FillRectangle(Brushes.Black, (int)surfacePoints[i, j][0] - usualPointSize / 2, (int)surfacePoints[i, j][1] - usualPointSize / 2, usualPointSize, usualPointSize);
                }
            }
        }

        private void FillShape(Graphics g, Vector<double> upLeft, Vector<double> upRight, Vector<double> downLeft, Vector<double> downRight)
        {
            Point[] points = new Point[4];
            points[0] = ConvertVectorToPoint(upLeft);
            points[1] = ConvertVectorToPoint(upRight);
            points[2] = ConvertVectorToPoint(downRight);
            points[3] = ConvertVectorToPoint(downLeft);
            g.FillPolygon(Brushes.Magenta, points);

            //for (int y = upLeft; i < )

            //while (u != downLeft)
        }

        //private void Draw

        private Point ConvertVectorToPoint(Vector<double> v)
        {
            Point point = new Point();
            point.X = (int)v[0];
            point.Y = (int)v[1];
            return point;
        }

        private Vector<double> CalculatePointOnSurface(Vector<double>[] corners, double u, double w)
        {
            Vector<double> point = Vector<double>.Build.Dense(3);
            if (corners.Any(p => p == null))
            {
                return point;
            }

            for (int i = 0; i < 3; i++)
            {
                point[i] = corners[0][i] * (1 - u) * (1 - w) + corners[1][i] * (1 - u) * w + corners[2][i] * u * (1 - w) + corners[3][i] * u * w;
            }

            //RotatePointX(ref point, rotation[0]);
            //RotatePointY(ref point, rotation[1]);

            return point;
        }

        private void ClearForm()
        {
            Array.Clear(corners, 0, corners.Length);
            cornerIndex = 0;
            rotation.Clear();
            pictureBox1.Refresh();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        }
    }
}
