using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab1_compgr
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        double x_point, y_point;
        double x_circle, y_circle;
        bool isFirstClick = true;

        private void Form1_Load(object sender, EventArgs e)
        {
            label1.Text = "Точка";
            label2.Text = "x";
            label3.Text = "y";
            label4.Text = "Окружность";
            label5.Text = "y";
            label6.Text = "x";
            label7.Text = "Радиус - ";
            pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label4.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label5.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label6.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label7.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            textBox2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            textBox3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            textBox4.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            textBox5.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;

        }

        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            ((PictureBox)sender).Invalidate();
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            Graphics g = pictureBox1.CreateGraphics();
            if (isFirstClick)
            {
                ClearCoords(g);
                isFirstClick = false;

                x_point = e.X;
                y_point = e.Y;

                textBox1.Text = x_point.ToString();
                textBox2.Text = y_point.ToString();

                SolidBrush redBrush = new SolidBrush(Color.Red);
                DrawFatPoint(g, (int)x_point, (int)y_point);
            }
            else
            {
                isFirstClick = true;

                x_circle = e.X;
                y_circle = e.Y;

                textBox4.Text = x_circle.ToString();
                textBox3.Text = y_circle.ToString();

                DrawFatPoint(g, (int)x_circle, (int)y_circle);
            }

        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            //int width = ((PictureBox)sender).Width;
            //int height = ((PictureBox)sender).Height;

            //g.DrawLine(Pens.Black, 0, height / 2, width, height / 2); //x
            //g.DrawLine(Pens.Black, width / 2, 0, width / 2, height); //y

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Graphics g = pictureBox1.CreateGraphics();

            if (!FormFilled())
            {                
                ClearCoords(g);
                return;
            }
            
            int radius = Convert.ToInt32(textBox5.Text);

            Pen p = new Pen(Color.Red);

            g.DrawEllipse(p, (int)x_circle - radius, (int)y_circle - radius, radius * 2, radius * 2);

            double k = (y_circle - y_point) / (x_circle - x_point);
            double m = -(k * x_point - y_point);
            double hypotenuse = Math.Sqrt(Math.Pow(x_circle - x_point, 2) + Math.Pow(y_circle - y_point, 2));
            double cos_alpha = radius / hypotenuse;
            double angle_alpha = Math.Acos(cos_alpha);// * (180 / Math.PI);
            double sin_alpha = Math.Sin(angle_alpha);

            // x^2 + k^2*x^2 - 2x(xb) + 2kmx - 2kx(yb) + (xb)^2 + m^2 + (yb)^2 - 2m(yb)
            // (1+k^2)x^2   +  (-2(xb)+2km-2k(yb))x   +   (xb)^2 + m^2 + (yb)^2 - 2m(yb) - r^2
            double a = 1 + Math.Pow(k, 2);
            double b = -2 * x_circle + (2 * k * m) - (2 * k * y_circle);
            double c = Math.Pow(x_circle, 2) + Math.Pow(m, 2) + Math.Pow(y_circle, 2) - (2 * m * y_circle) - Math.Pow(radius, 2);
            double D = Math.Pow(b, 2) - 4 * a * c;

            double x_1 = (-b + Math.Sqrt(D)) / (2 * a);
            double x_2 = (-b - Math.Sqrt(D)) / (2 * a);

            double y_1 = k * x_1 + m;
            double y_2 = k * x_2 + m;

            double dist_1 = Math.Sqrt(Math.Pow(x_1 - x_point, 2) + Math.Pow(y_1 - y_point, 2));
            double dist_2 = Math.Sqrt(Math.Pow(x_2 - x_point, 2) + Math.Pow(y_2 - y_point, 2));

            double x_between_point, y_between_point;
            if (dist_1 < dist_2)
            {
                x_between_point = x_1;
                y_between_point = y_1;
            }
            else
            {
                x_between_point = x_2;
                y_between_point = y_2;
            }

            // move to center, rotate around center and move back
            double x_centralized = x_between_point - x_circle;
            double y_centralized = y_between_point - y_circle;
            double x_tangent_1 = x_centralized * cos_alpha - y_centralized * sin_alpha;
            double y_tangent_1 = x_centralized * sin_alpha + y_centralized * cos_alpha;

            double x_tangent_2 = x_centralized * cos_alpha - y_centralized * -sin_alpha;
            double y_tangent_2 = x_centralized * -sin_alpha + y_centralized * cos_alpha;

            x_tangent_1 += x_circle;
            y_tangent_1 += y_circle;

            x_tangent_2 += x_circle;
            y_tangent_2 += y_circle;

            DrawFatPoint(g, (int)x_tangent_1, (int)y_tangent_1);
            DrawFatPoint(g, (int)x_tangent_2, (int)y_tangent_2);
            g.DrawLine(p, new Point((int)x_point, (int)y_point), new Point((int)x_tangent_1, (int)y_tangent_1));
            g.DrawLine(p, new Point((int)x_point, (int)y_point), new Point((int)x_tangent_2, (int)y_tangent_2));
        }

        private bool FormFilled()
        {
            return textBox1.Text != "" &&
                textBox2.Text != "" &&
                textBox3.Text != "" &&
                textBox4.Text != "" &&
                textBox5.Text != "";
        }

        private void ClearCoords(Graphics g)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            g.Clear(Color.White);
            isFirstClick = true;
        }

        private void DrawFatPoint(Graphics g, int x, int y)
        {
            SolidBrush redBrush = new SolidBrush(Color.Red);
            int fatness = 6;
            g.FillRectangle(redBrush, x - fatness / 2, y - fatness / 2, fatness, fatness);
        }
    }
}
