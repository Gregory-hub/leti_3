using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using MathNet.Numerics.LinearAlgebra;

namespace lab5
{
    public partial class Form1 : Form
    {
        private Point3D center = new Point3D(0, 0, 0);
        private BilinearSurface surface = new BilinearSurface();

        private bool rightMousePressed = false;
        private Point mouseDownPoint = new Point(0, 0);
        private Vector<double> deltaRotation = Vector<double>.Build.Dense(2);

        private bool cursorHidden = false;
        private Point cursorFixPosition;

        private int usualPointSize = 2;
        private int cornerPointSize = 5;
        private double sensitivityX = 0.01;
        private double sensitivityY = 0.01;

        private Brush surfaceFrontBrush = Brushes.Aqua;
        private Brush cornerBrush = Brushes.Black;
        private Brush pointBrush = Brushes.Red;

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

            surface.cornerBrush = cornerBrush;
            surface.pointBrush = pointBrush;
            surface.surfaceBrush = surfaceFrontBrush;
            surface.pointFatness = usualPointSize;
            surface.cornerFatness = cornerPointSize;
        }

        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (surface.Corners.Count == 4 && GridDensity != 0)
            {
                label2.Text = "";
                surface.Calculate(GridDensity);
                surface.Fill(e.Graphics, true, true);
            }
            else
            {
                surface.DrawCornerPoints(e.Graphics);
            }

            Vector<double> angles = surface.Rotation.GetAngles();
            textBox2.Text = (angles[0] * 180 / Math.PI).ToString();
            textBox3.Text = (angles[1] * 180 / Math.PI).ToString();
            textBox4.Text = (angles[2] * 180 / Math.PI).ToString();
            DisplayMessage();
        }

        private void PictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && surface.Corners.Count != 4)
            {
                Point3D point = new Point3D(e.Location.X, e.Location.Y, 0d);
                surface.Corners.Add(point);

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
                }

                if (!FreezeY)
                {
                    deltaRotation[1] = (e.X - mouseDownPoint.X) * sensitivityY;
                }

                surface.Rotate(center, deltaRotation);
                deltaRotation.Clear();

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
            surface.ResetRotation(center);
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

        private void ClearForm()
        {
            surface.Polygons.Clear();
            surface.PointArray.Clear();
            surface.Corners.Clear();
            surface.Rotation.Clear();
            pictureBox1.Refresh();
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

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        }
    }
}

