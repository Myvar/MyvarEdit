using System.Collections.Generic;
using System.Drawing;

namespace MyvarEdit.TrueType
{
    public static class Ext
    {
        public static PointF[] ToPointArray(this List<GlyfPoint> x)
        {
            var re = new List<PointF>();

            foreach (var glyfPoint in x)
            {
                re.Add(new PointF(glyfPoint.X, glyfPoint.Y));
            }

            return re.ToArray();
        }
    }
}