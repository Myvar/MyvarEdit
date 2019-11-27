namespace mEdit.Core.OpenGL
{
    public class Vertex
    {
        public static int Size = 14;

        public Vertex(Vector3f position, Vector2f texCoord, Vector3f color, Vector3f normal, Vector3f tangent)
        {
            this.Position = position;
            this.TexCoord = texCoord;
            this.Color = color;
            this.Normal = normal;
            this.Tangent = tangent;

        }

        public Vertex(Vector3f position) : this(
            position,
            new Vector2f(0, 0),
            new Vector3f(0.5f, 0.5f, 0.5f),
            new Vector3f(0, 0, 0),
            new Vector3f(0, 0, 0))
        {

        }
        
        public Vertex(Vector3f position, Vector2f texCoord) : this(
            position,
            texCoord,
            new Vector3f(0.5f, 0.5f, 0.5f),
            new Vector3f(0, 0, 0),
            new Vector3f(0, 0, 0))
        {

        }

        public Vector3f Position { get; set; }
        public Vector2f TexCoord { get; set; }
        public Vector3f Color { get; set; }
        public Vector3f Normal { get; set; }
        public Vector3f Tangent { get; set; }

    }
}