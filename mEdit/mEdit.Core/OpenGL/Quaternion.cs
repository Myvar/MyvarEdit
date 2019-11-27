using System;

namespace mEdit.Core.OpenGL
{
    public class Quaternion
    {
        public float X, Y, Z, W;

        public Quaternion(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public float Length()
        {
            return (float)Math.Sqrt(X * X + Y * Y + Z * Z + W * W);
        }

        public Quaternion Normalize()
        {
            var len = Length();

            X /= len;
            Y /= len;
            Z /= len;
            W /= len;

            return this;
        }

        public Quaternion Conjugate()
        {
            return new Quaternion(-X, -Y, -Z, W);
        }

        public Quaternion Mul(Quaternion r)
        {
            float w = W * r.W - X * r.X - Y * r.Y - Z * r.Z;
            float x = X * r.W + W * r.X + Y * r.Z - Z * r.Y;
            float y = Y * r.W + W * r.Y + Z * r.X - X * r.Z;
            float z = Z * r.W + W * r.Z + X * r.Y - Y * r.X;

            return new Quaternion(x, y, z, w);
        }

        public Quaternion Mul(Vector3f r)
        {
            float w = -X * r.X - Y * r.Y - Z * r.Z;
            float x = W * r.X + Y * r.Z - Z * r.Y;
            float y = W * r.Y + Z * r.X - X * r.Z;
            float z = W * r.Z + X * r.Y - Y * r.X;

            return new Quaternion(x, y, z, w);
        }

         public static Quaternion operator *(Quaternion c1, Quaternion c2)
        {
            return c1.Mul(c2);
        }
    }
}