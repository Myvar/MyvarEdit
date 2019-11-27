namespace mEdit.Core.OpenGL
{
    public class Transform
    {
        public Vector3f Translation;
        public Vector3f Rotation;
        public Vector3f Scale;

        public float RotX
        {
            get
            {
                return Rotation.X;
            }
            set
            {
                Rotation.X = value;
            }
        }

        public float RotY
        {
            get
            {
                return Rotation.Y;
            }
            set
            {
                Rotation.Y = value;
            }
        }

        public float RotZ
        {
            get
            {
                return Rotation.Z;
            }
            set
            {
                Rotation.Z = value;
            }
        }

        public float X
        {
            get
            {
                return Translation.X;
            }
            set
            {
                Translation.X = value;
            }
        }

        public float Y
        {
            get
            {
                return Translation.Y;
            }
            set
            {
                Translation.Y = value;
            }
        }

        public float Z
        {
            get
            {
                return Translation.Z;
            }
            set
            {
                Translation.Z = value;
            }
        }

        public Transform()
        {
            Translation = new Vector3f(0, 0, 0);
            Rotation = new Vector3f(0, 0, 0);
            Scale = new Vector3f(1, 1, 1);
        }

        public Matrix4f GetTranformation()
        {
            Matrix4f trans = new Matrix4f().InitTranslation(Translation.X, Translation.Y, Translation.Z);
            Matrix4f rot = new Matrix4f().InitRotation(Rotation.X, Rotation.Y, Rotation.Z);
            Matrix4f scal = new Matrix4f().InitScale(Scale.X, Scale.Y, Scale.Z);

            return trans.Mul(rot.Mul(scal));
        }
    }
}