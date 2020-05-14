namespace MyvarEdit.Rendering
{
    public class Matrix4f
    {
        public float[][] m;

        public Matrix4f()
        {
            m = new float[4][];
            for (int i = 0; i < m.Length; i++)
            {
                m[i] = new float[4];
            }
        }
        
        public Matrix4f InitTranslation(float x, float y, float z)
        {
            m[0][0] = 1; m[0][1] = 0; m[0][2] = 0; m[0][3] = x;
            m[1][0] = 0; m[1][1] = 1; m[1][2] = 0; m[1][3] = y;
            m[2][0] = 0; m[2][1] = 0; m[2][2] = 1; m[2][3] = z;
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
        
        public static Matrix4f operator *(Matrix4f c1, Matrix4f c2)
        {
            return c1.Mul(c2);
        }
        
        public Matrix4f InitScale(float x, float y, float z)
        {
            m[0][0] = x; m[0][1] = 0; m[0][2] = 0; m[0][3] = 0;
            m[1][0] = 0; m[1][1] = y; m[1][2] = 0; m[1][3] = 0;
            m[2][0] = 0; m[2][1] = 0; m[2][2] = z; m[2][3] = 0;
            m[3][0] = 0; m[3][1] = 0; m[3][2] = 0; m[3][3] = 1;

            return this;
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
        
        public Matrix4f InitIdentity()
        {
            m[0][0] = 1; m[0][1] = 0; m[0][2] = 0; m[0][3] = 0;
            m[1][0] = 0; m[1][1] = 1; m[1][2] = 0; m[1][3] = 0;
            m[2][0] = 0; m[2][1] = 0; m[2][2] = 1; m[2][3] = 0;
            m[3][0] = 0; m[3][1] = 0; m[3][2] = 0; m[3][3] = 1;

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
    }
}