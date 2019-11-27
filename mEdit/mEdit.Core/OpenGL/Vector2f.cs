using System;

namespace mEdit.Core.OpenGL
{
    public class Vector2f
    {
        public float X, Y;

        public float Length()
        {
            return (float)Math.Sqrt(X * X + Y * Y);
        }

        public float Dot(Vector2f r)
        {
            return X * r.X + Y * r.Y;
        }

        public Vector2f Normalize()
        {
            var len = Length();

            X /= len;
            Y /= len;

            return this;
        }

        public Vector2f Rotate(float angle)
        {
            float rad = OpenTK.MathHelper.DegreesToRadians(angle);

            double cos = Math.Cos(rad);
            double sin = Math.Sin(rad);

            return new Vector2f((float)(X * cos - Y * sin), (float)(X * sin + Y * cos));
        }

        public Vector2f Add(Vector2f r)
        {
            return new Vector2f(X + r.X, Y + r.Y);
        }

        public Vector2f Add(float r)
        {
            return new Vector2f(X + r, Y + r);
        }

        public Vector2f Sub(Vector2f r)
        {
            return new Vector2f(X - r.X, Y - r.Y);
        }

        public Vector2f Sub(float r)
        {
            return new Vector2f(X - r, Y - r);
        }

        public Vector2f Mul(Vector2f r)
        {
            return new Vector2f(X * r.X, Y * r.Y);
        }

        public Vector2f Mul(float r)
        {
            return new Vector2f(X * r, Y * r);
        }

        public Vector2f Dev(Vector2f r)
        {
            return new Vector2f(X / r.X, Y / r.Y);
        }

        public Vector2f Dev(float r)
        {
            return new Vector2f(X / r, Y / r);
        }

        public static Vector2f operator +(Vector2f c1, Vector2f c2)
        {
            return c1.Add(c2);
        }

        public static Vector2f operator -(Vector2f c1, Vector2f c2)
        {
            return c1.Sub(c2);
        }

        public static Vector2f operator *(Vector2f c1, Vector2f c2)
        {
            return c1.Mul(c2);
        }

        public static Vector2f operator /(Vector2f c1, Vector2f c2)
        {
            return c1.Dev(c2);
        }

        
        public Vector2f(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Vector2f(float all)
        {
            X = all;
            Y = all;
        }
    }
}