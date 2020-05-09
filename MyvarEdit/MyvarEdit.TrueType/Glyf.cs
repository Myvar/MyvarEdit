using System.Collections.Generic;
using System.Drawing;
using MyvarEdit.TrueType.Internals;

namespace MyvarEdit.TrueType
{
    public class ComponentGlyph
    {
        public ComponentFlags Flags;
        public ushort GlyphIndex;
        public int Argument1;
        public int Argument2;
        public int DestPointIndex;
        public int SrcPointIndex;

        public int A = 1, B, C, D = 1, E, F;
    }
    public class ComponentTriangle
    {
        public GlyfPoint A, B, C;
    }


    public class GlyfPoint
    {
        public float X, Y;
        public bool isMidpoint;
        public bool IsOnCurve;

        public GlyfPoint(float x, float y)
        {
            X = x;
            Y = y;
        }
    }


    public class Glyf
    {
        public short NumberOfContours { get; set; }
        public short Xmin { get; set; }
        public short Ymin { get; set; }
        public short Xmax { get; set; }
        public short Ymax { get; set; }

        public List<List<GlyfPoint>> Shapes { get; set; } = new List<List<GlyfPoint>>();

        public List<ushort> ContourEnds { get; set; } = new List<ushort>();
        public List<GlyfPoint> Points { get; set; } = new List<GlyfPoint>();
        public List<ComponentTriangle> Triangles { get; set; } = new List<ComponentTriangle>();
        public List<bool> Curves { get; set; } = new List<bool>();
        public List<ComponentGlyph> Components { get; set; } = new List<ComponentGlyph>();
    }
}