using System.Collections.Generic;
using System.Drawing;

namespace MyvarEdit.TrueType
{
    public class GlyfPoint
    {

        public float X, Y, Cx, Cy;

        public GlyfPoint(float x, float y)
        {
            X = x;
            Y = y;
            Cx = x;
            Cy = y;
        }
    }
    
    public class Glyf
    {
        public short NumberOfContours { get; set; }
        public short Xmin { get; set; }
        public short Ymin { get; set; }
        public short Xmax { get; set; }
        public short Ymax { get; set; }
        
        public List<ushort> ContourEnds { get; set; } = new List<ushort>();
        public List<GlyfPoint> Points { get; set; } = new List<GlyfPoint>();
        public List<bool> Curves { get; set; } = new List<bool>();
    }
}