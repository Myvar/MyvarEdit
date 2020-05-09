using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using MyvarEdit.TrueType;

namespace MyvarEdit
{
    class Program
    {
        static void Main(string[] args)
        {
            var ttf = new TrueTypeFontFile();
            ttf.Load("./Fonts/Hack-Regular.ttf");

            var gl = ttf.Glyfs[(byte) '?'];

            var w = gl.Xmax + Math.Abs(gl.Xmin);
            var h = gl.Ymax + Math.Abs(gl.Ymin);

            var scale = 1f;
            var paddingPx = 15;

            using (var img = new Bitmap(w + paddingPx, h + paddingPx * 2))
            using (var g = Graphics.FromImage(img))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;

                g.Clear(Color.Black);
                g.TranslateTransform(Math.Abs(gl.Xmin) / 2f, img.Height - (Math.Abs(gl.Ymin) / 2f));
                g.ScaleTransform(scale, -scale);


                foreach (var triangle in gl.Triangles)
                {
                    using (var p = new Pen(Color.White, 2))
                    {
                        var contour = new PointF[]
                        {
                            new PointF(triangle.A.X, triangle.A.Y),
                            new PointF(triangle.B.X, triangle.B.Y),
                            new PointF(triangle.C.X, triangle.C.Y),
                        };
                        g.FillPolygon(Brushes.White, contour);
                        g.DrawPolygon(p, contour);
                    }
                }


                img.Save("lol.png");
            }
        }
    }
}