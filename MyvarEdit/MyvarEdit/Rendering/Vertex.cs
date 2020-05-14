namespace MyvarEdit.Rendering
{
    public class Vertex
    {
        public static int Size = 2;

        public Vertex(Point position)
        {
            this.Position = position;
        }


        public Point Position { get; set; }
    }
}