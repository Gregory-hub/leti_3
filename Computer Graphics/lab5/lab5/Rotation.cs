using MathNet.Numerics.LinearAlgebra;
using System;

namespace lab5
{
    internal class Rotation
    {
        public Matrix<double> Matrix { get; set; }

        public Rotation()
        {
            Matrix = Matrix<double>.Build.DenseIdentity(3, 3);  // identity matrix (zero rotation)
        }

        public Vector<double> GetAngles()
        {
            Vector<double> angles = Vector<double>.Build.Dense(3);
            if (Math.Abs(Matrix[2, 0]) != 1)
            {
                angles[1] = Math.Asin(-Matrix[2, 0]);
                double cosY = Math.Cos(angles[1]);
                angles[0] = Math.Atan2(Matrix[2, 1] / cosY, Matrix[2, 2] / cosY);
                angles[2] = Math.Atan2(Matrix[1, 0] / cosY, Matrix[0, 0] / cosY);
            }
            else
            {
                angles[2] = 0;
                if (Matrix[2, 0] == -1)
                {
                    angles[1] = Math.PI;
                    angles[0] = angles[2] + Math.Atan2(Matrix[0, 1], Matrix[0, 2]);
                }
                else
                {
                    angles[1] = -Math.PI;
                    angles[0] = -angles[2] + Math.Atan2(-Matrix[0, 1], -Matrix[0, 2]);
                }
            }
            return angles;
        }

        public static Rotation operator -(Rotation rotation)
        {
            Rotation newRotation = new Rotation();
            newRotation.Matrix = rotation.Matrix.Inverse();
            return newRotation;
        }

        public void Clear()
        {
            Matrix = Matrix<double>.Build.DenseIdentity(3, 3);
        }
    }
}

