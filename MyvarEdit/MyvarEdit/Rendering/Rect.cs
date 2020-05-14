namespace MyvarEdit.Rendering
{
    public class Rect
    {
        public Rect(Point location, Size size)
        {
            Location = location;
            Size = size;
        }

        public Rect(float x, float y, float w, float h)
        {
            Location = new Point(x, y);
            Size = new Size(w, h);
        }

        public Point Location { get; set; }
        public Size Size { get; set; }
    }
}