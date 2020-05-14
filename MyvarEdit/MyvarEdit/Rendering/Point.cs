namespace MyvarEdit.Rendering
{
    public class Point
    {
        public Point(float x, float y)
        {
            X = x;
            Y = y;
        }
        
        public Point(float z)
        {
            X = z;
            Y = z;
        }

        public float X { get; set; }
        public float Y { get; set; }
    }
}