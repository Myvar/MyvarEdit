using System;

namespace mEdit.Core.OpenGL
{
    public class Matrix4f
    {


        public static Matrix4f Inverce(Matrix4f i)
        {
            var mat4 = new OpenTK.Matrix4(
                    new OpenTK.Vector4(i[0, 0], i[0, 1], i[0, 2], i[0, 3]),
                    new OpenTK.Vector4(i[1, 0], i[1, 1], i[1, 2], i[1, 3]),
                    new OpenTK.Vector4(i[2, 0], i[2, 1], i[2, 2], i[2, 3]),
                    new OpenTK.Vector4(i[3, 0], i[3, 1], i[3, 2], i[3, 3])
                );
            mat4.Invert();
            var re = new Matrix4f();

            re[0, 0] = mat4.Column0[0];
            re[0, 1] = mat4.Column0[1];
            re[0, 2] = mat4.Column0[2];
            re[0, 3] = mat4.Column0[3];

            re[1, 0] = mat4.Column1[0];
            re[1, 1] = mat4.Column1[1];
            re[1, 2] = mat4.Column1[2];
            re[1, 3] = mat4.Column1[3];

            re[2, 0] = mat4.Column1[0];
            re[2, 1] = mat4.Column1[1];
            re[2, 2] = mat4.Column1[2];
            re[2, 3] = mat4.Column1[3];

            re[3, 0] = mat4.Column1[0];
            re[3, 1] = mat4.Column1[1];
            re[3, 2] = mat4.Column1[2];
            re[3, 3] = mat4.Column1[3];

            return re;
        }

        public static Vector4f transform(Matrix4f left, Vector4f right, Vector4f dest)
        {
            if (dest == null)
                dest = new Vector4f(0, 0, 0, 0);

            float x = left[0, 0] * right.X + left[1, 0] * right.Y + left[2, 0] * right.Z + left[3, 0] * right.W;
            float y = left[0, 1] * right.X + left[1, 1] * right.Y + left[2, 1] * right.Z + left[3, 1] * right.W;
            float z = left[0, 2] * right.X + left[1, 2] * right.Y + left[2, 2] * right.Z + left[3, 2] * right.W;
            float w = left[0, 3] * right.X + left[1, 3] * right.Y + left[2, 3] * right.Z + left[3, 3] * right.W;

            dest.X = x;
            dest.Y = y;
            dest.Z = z;
            dest.W = w;

            return dest;
        }

        public OpenTK.Matrix4 ToTKMattrix()
        {
            return new OpenTK.Matrix4(
                    new OpenTK.Vector4(this[0, 0], this[0, 1], this[0, 2], this[0, 3]),
                    new OpenTK.Vector4(this[1, 0], this[1, 1], this[1, 2], this[1, 3]),
                    new OpenTK.Vector4(this[2, 0], this[2, 1], this[2, 2], this[2, 3]),
                    new OpenTK.Vector4(this[3, 0], this[3, 1], this[3, 2], this[3, 3])
                );
        }

        public float[][] m;

        public Matrix4f()
        {
            m = new float[4][];
            for (int i = 0; i < m.Length; i++)
            {
                m[i] = new float[4];
            }
        }

        public Matrix4f InitIdentity()
        {
            m[0][0] = 1; m[0][1] = 0; m[0][2] = 0; m[0][3] = 0;
            m[1][0] = 0; m[1][1] = 1; m[1][2] = 0; m[1][3] = 0;
            m[2][0] = 0; m[2][1] = 0; m[2][2] = 1; m[2][3] = 0;
            m[3][0] = 0; m[3][1] = 0; m[3][2] = 0; m[3][3] = 1;

            return this;
        }
        public Matrix4f InitCamera(Vector3f forward, Vector3f up)
        {
            Vector3f f = forward;
            f.Normalize();

            Vector3f r = up;
            r.Normalize();
            r = r.Cross(f);

            Vector3f u = f.Cross(r);

            m[0][0] = r.X; m[0][1] = r.Y; m[0][2] = r.Z; m[0][3] = 0;
            m[1][0] = u.X; m[1][1] = u.Y; m[1][2] = u.Z; m[1][3] = 0;
            m[2][0] = f.X; m[2][1] = f.Y; m[2][2] = f.Z; m[2][3] = 0;
            m[3][0] = 0; m[3][1] = 0; m[3][2] = 0; m[3][3] = 1;

            return this;
        }

        public Matrix4f InitProjection(float aFov, float aWidth, float aHeight, float Znear, float ZFar)
        {
            float ar = aWidth / aHeight;
            float tanHalfFOV = (float)Math.Tan(OpenTK.MathHelper.DegreesToRadians(aFov / 2));
            float zRange = Znear - ZFar;

            m[0][0] = 1.0f / (tanHalfFOV * ar); m[0][1] = 0; m[0][2] = 0; m[0][3] = 0;
            m[1][0] = 0; m[1][1] = 1.0f / tanHalfFOV; m[1][2] = 0; m[1][3] = 0;
            m[2][0] = 0; m[2][1] = 0; m[2][2] = (-Znear - ZFar) / zRange; m[2][3] = 2 * ZFar * Znear / zRange;
            m[3][0] = 0; m[3][1] = 0; m[3][2] = 1; m[3][3] = 0;

            return this;
        }

        public Matrix4f InitOrthographic(float left, float right, float bottom, float top, float near, float far)
        {
            float width = right - left;
            float height = top - bottom;
            float depth = far - near;

            m[0][0] = 2 / width; m[0][1] = 0; m[0][2] = 0; m[0][3] = -(right + left) / width;
            m[1][0] = 0; m[1][1] = 2 / height; m[1][2] = 0; m[1][3] = -(top + bottom) / height;
            m[2][0] = 0; m[2][1] = 0; m[2][2] = -2 / depth; m[2][3] = -(far + near) / depth;
            m[3][0] = 0; m[3][1] = 0; m[3][2] = 0; m[3][3] = 1;

            return this;
        }

        public Matrix4f InitTranslation(float x, float y, float z)
        {
            m[0][0] = 1; m[0][1] = 0; m[0][2] = 0; m[0][3] = x;
            m[1][0] = 0; m[1][1] = 1; m[1][2] = 0; m[1][3] = y;
            m[2][0] = 0; m[2][1] = 0; m[2][2] = 1; m[2][3] = z;
            m[3][0] = 0; m[3][1] = 0; m[3][2] = 0; m[3][3] = 1;

            return this;
        }

        public Matrix4f InitRotation(float x, float y, float z)
        {
            Matrix4f rx = new Matrix4f();
            Matrix4f ry = new Matrix4f();
            Matrix4f rz = new Matrix4f();

            x = (float)OpenTK.MathHelper.DegreesToRadians(x);
            y = (float)OpenTK.MathHelper.DegreesToRadians(y);
            z = (float)OpenTK.MathHelper.DegreesToRadians(z);

            rz.m[0][0] = (float)Math.Cos(z); rz.m[0][1] = -((float)Math.Sin(z)); rz.m[0][2] = 0; rz.m[0][3] = 0;
            rz.m[1][0] = (float)Math.Sin(z); rz.m[1][1] = (float)Math.Cos(z); rz.m[1][2] = 0; rz.m[1][3] = 0;
            rz.m[2][0] = 0; rz.m[2][1] = 0; rz.m[2][2] = 1; rz.m[2][3] = 0;
            rz.m[3][0] = 0; rz.m[3][1] = 0; rz.m[3][2] = 0; rz.m[3][3] = 1;

            rx.m[0][0] = 1; rx.m[0][1] = 0; rx.m[0][2] = 0; rx.m[0][3] = 0;
            rx.m[1][0] = 0; rx.m[1][1] = (float)Math.Cos(x); rx.m[1][2] = -((float)Math.Sin(x)); rx.m[1][3] = 0;
            rx.m[2][0] = 0; rx.m[2][1] = (float)Math.Sin(x); rx.m[2][2] = (float)Math.Cos(x); rx.m[2][3] = 0;
            rx.m[3][0] = 0; rx.m[3][1] = 0; rx.m[3][2] = 0; rx.m[3][3] = 1;

            ry.m[0][0] = (float)Math.Cos(y); ry.m[0][1] = 0; ry.m[0][2] = -((float)Math.Sin(y)); ry.m[0][3] = 0;
            ry.m[1][0] = 0; ry.m[1][1] = 1; ry.m[1][2] = 0; ry.m[1][3] = 0;
            ry.m[2][0] = (float)Math.Sin(y); ry.m[2][1] = 0; ry.m[2][2] = (float)Math.Cos(y); ry.m[2][3] = 0;
            ry.m[3][0] = 0; ry.m[3][1] = 0; ry.m[3][2] = 0; ry.m[3][3] = 1;

            m = rz.Mul(ry.Mul(rx)).m;
            return this;
        }

        public Matrix4f InitScale(float x, float y, float z)
        {
            m[0][0] = x; m[0][1] = 0; m[0][2] = 0; m[0][3] = 0;
            m[1][0] = 0; m[1][1] = y; m[1][2] = 0; m[1][3] = 0;
            m[2][0] = 0; m[2][1] = 0; m[2][2] = z; m[2][3] = 0;
            m[3][0] = 0; m[3][1] = 0; m[3][2] = 0; m[3][3] = 1;

            return this;
        }

        public Matrix4f Mul(Matrix4f r)
        {
            Matrix4f re = new Matrix4f();

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    re[i, j] =
                     m[i][0] * r[0, j] +
                     m[i][1] * r[1, j] +
                     m[i][2] * r[2, j] +
                     m[i][3] * r[3, j];
                }
            }

            return re;
        }

        public float this[int x, int y]
        {
            get
            {
                return m[x][y];
            }
            set
            {
                m[x][y] = value;
            }
        }

        public static Matrix4f operator *(Matrix4f c1, Matrix4f c2)
        {
            return c1.Mul(c2);
        }
    }
}