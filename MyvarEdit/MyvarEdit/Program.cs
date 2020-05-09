using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
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
            
            var gl = ttf.Glyfs[(byte) ';'];

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
                
                foreach (var shape in gl.Shapes)
                {
                    using (var p = new Pen(Color.White, 2))
                    {
                        var contour = shape.ToPointArray();
                        g.DrawPolygon(p, contour);
                    }
                }


                img.Save("lol.png");
            }
        }
    }
}