using MathNet.Numerics.LinearAlgebra;

namespace lab5
{
    internal class Rotation
    {
        public Vector<double> Angles { get; set; }

        public Rotation()
        {
            Angles = Vector<double>.Build.Dense(3);
        }

        public Rotation(Vector<double> angles)
        {
            Angles = angles;
        }

        public Rotation(double x, double y, double z)
        {
            Angles = Vector<double>.Build.DenseOfArray(new double[] { x, y, z });
        }

        public Rotation(int x, int y, int z)
        {
            Angles = Vector<double>.Build.DenseOfArray(new double[] { x, y, z });
        }

        public double this[int i]
        {
            get { return Angles[i]; }
            set { Angles[i] = value; }
        }
    }
}

