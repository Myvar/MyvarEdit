namespace MyvarEdit.Rendering
{
    public class Size
    {
        public Size(float width, float height)
        {
            Width = width;
            Height = height;
        }

        public Size(float z)
        {
            Width = z;
            Height = z;
        }

        public float Width { get; set; }
        public float Height { get; set; }
    }
}