using System;
using System.Globalization;

namespace mEdit.Core.OpenGL
{
    public class Vector3f
    {

        public float X, Y, Z;

        public Vector3f(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3f Rotate(float angle, Vector3f axis)
        {
            float sinHalfAngle = (float)Math.Sin(OpenTK.MathHelper.DegreesToRadians(angle / 2));
            float cosHalfAngle = (float)Math.Cos(OpenTK.MathHelper.DegreesToRadians(angle / 2));

            float rx = axis.X * sinHalfAngle;
            float ry = axis.Y * sinHalfAngle;
            float rz = axis.Z * sinHalfAngle;
            float rw = cosHalfAngle;

            var rotation = new Quaternion(rx, ry, rz, rw);
            var conjugate = rotation.Conjugate();

            var w = rotation.Mul(this).Mul(conjugate);

            X = w.X;
            Y = w.Y;
            Z = w.Z;

            return this;
        }

        public float DistanceTo(Vector3f b)
        {
            Vector3f vector = new Vector3f(X - b.X, Y - b.Y, Z - b.Z);
            return (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
        }

        public float Length()
        {
            return (float)Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public float Dot(Vector3f r)
        {
            return X * r.X + Y * r.Y + Z * r.Z;
        }

        public Vector3f Abs()
        {
            return new Vector3f(Math.Abs(X), Math.Abs(Y), Math.Abs(Z));
        }

        public Vector3f Cross(Vector3f r)
        {
            float x = Y * r.Z - Z * r.Y;
            float y = Z * r.X - X * r.Z;
            float z = X * r.Y - Y * r.X;

            return new Vector3f(x, y, z);
        }

        public Vector3f Normalize()
        {
            var len = Length();

            X /= len;
            Y /= len;
            Z /= len;

            return this;
        }

        public Vector3f Add(Vector3f r)
        {
            return new Vector3f(X + r.X, Y + r.Y, Z + r.Z);
        }

        public Vector3f Add(float r)
        {
            return new Vector3f(X + r, Y + r, Z + r);
        }

        public Vector3f Sub(Vector3f r)
        {
            return new Vector3f(X - r.X, Y - r.Y, Z - r.Z);
        }

        public Vector3f Sub(float r)
        {
            return new Vector3f(X - r, Y - r, Z - r);
        }

        public Vector3f Mul(Vector3f r)
        {
            return new Vector3f(X * r.X, Y * r.Y, Z * r.Z);
        }

        public Vector3f Mul(float r)
        {
            return new Vector3f(X * r, Y * r, Z * r);
        }

        public Vector3f Dev(Vector3f r)
        {
            return new Vector3f(X / r.X, Y / r.Y, Z / r.Z);
        }

        public Vector3f Dev(float r)
        {
            return new Vector3f(X / r, Y / r, Z / r);
        }

        public static Vector3f operator +(Vector3f c1, Vector3f c2)
        {
            return c1.Add(c2);
        }

        public static Vector3f operator -(Vector3f c1, Vector3f c2)
        {
            return c1.Sub(c2);
        }

        public static Vector3f operator *(Vector3f c1, Vector3f c2)
        {
            return c1.Mul(c2);
        }

        public static Vector3f operator /(Vector3f c1, Vector3f c2)
        {
            return c1.Dev(c2);
        }


        public static Vector3f ParseVector3f(string raw, char spliter)
        {
            raw = raw.Trim();

            float x = 0, y = 0, z = 0;

            var fmt = new NumberFormatInfo();
            fmt.NegativeSign = "-";
            fmt.NumberDecimalSeparator = ".";

            var segs = raw.Split(spliter);

            if (!string.IsNullOrEmpty(segs[0].Trim()))
            {
                x = float.Parse(segs[0].Trim(), fmt);
            }
            else
            {
                x = 0;
            }

            if (!string.IsNullOrEmpty(segs[1].Trim()))
            {
                y = float.Parse(segs[1].Trim(), fmt);
            }
            else
            {
                y = 0;
            }

            if (!string.IsNullOrEmpty(segs[2].Trim()))
            {
                z = float.Parse(segs[2].Trim(), fmt);
            }
            else
            {
                z = 0;
            }


            return new Vector3f(x, y, z);
        }
    }
}