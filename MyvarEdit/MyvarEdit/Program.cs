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
        static float Bezier(float p0, float p1, float p2, float t) // Parameter 0 <= t <= 1
        {
            //B(T) = P1 + (1 - t)^2 * (P0 - P1) + t^2 (P2 - P1)

            return p1 + MathF.Pow(1f - t, 2) * (p0 - p1) + MathF.Pow(t, 2) * (p2 - p1);
        }

        static void Main(string[] args)
        {
            var ttf = new TrueTypeFontFile();
            ttf.Load("./Fonts/Karla-Regular.ttf");


            var gl = ttf.Glyfs[(byte) '~'];

            Console.WriteLine($"[xMin: {gl.Xmin}, xMax: {gl.Xmax}]");
            Console.WriteLine($"[yMin: {gl.Ymin}, yMax: {gl.Ymax}]");

            var w = gl.Xmax + Math.Abs(gl.Xmin);
            var h = gl.Ymax + Math.Abs(gl.Ymin);

            // var w = 1024;
            // var h = 1024;

            var scale = 1f;
            var paddingPx = 15;

            using (var img = new Bitmap(w + paddingPx, h + paddingPx * 2))
            using (var g = Graphics.FromImage(img))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;

                g.Clear(Color.Black);
                g.TranslateTransform(Math.Abs(gl.Xmin) / 2f, img.Height - (Math.Abs(gl.Ymin) / 2f));
                //   g.TranslateTransform(paddingPx, (img.Height - paddingPx));
                //g.TranslateTransform(img.Width / 2f, img.Height / 2f);
                g.ScaleTransform(scale, -scale);

                var sp = gl.Points[0];
                var lp = gl.Points.Last();

                int cnt = 0;

                var points = new List<PointF>();
                var contours = new List<List<PointF>>();


                for (var i = 1; i < gl.Points.Count; i++)
                {
                    var point = gl.Points[i];
                    var beforepoint = gl.Points[i - 1];
                    Console.WriteLine($"[X: {point.X}, Y: {point.Y}]");

                    if (gl.ContourEnds.Contains((ushort) i))
                    {
                        points.Add(new PointF(point.X, point.Y));

                        if (contours.Count == 0) points.Add(new PointF(gl.Points[0].X, gl.Points[0].Y));
                        contours.Add(points);
                        points = new List<PointF>();
                    }
                    else
                    {
                        if (!gl.Curves[i])
                        {
                            var res = 5f;

                            for (int j = 0; j <= res; j++)
                            {
                                var t = j / res;
                                points.Add(new PointF(
                                    Bezier(gl.Points[i - 1].X, gl.Points[i].Cx, gl.Points[i + 1].X, t),
                                    Bezier(gl.Points[i - 1].Y, gl.Points[i].Cy, gl.Points[i + 1].Y, t)));
                            }
                        }
                        else
                        {
                            points.Add(new PointF(point.X, point.Y));
                        }
                    }
                }

                for (var i = 0; i < contours.Count; i++)
                {
                    var contour = contours[i];
                    g.DrawPolygon(i % 2 == 0 ? Pens.Gold : Pens.GreenYellow, contour.ToArray());

                    foreach (var f in contour)
                    {
                        g.FillEllipse(Brushes.Cyan, f.X - 5, f.Y - 5, 10, 10);
                    }

                    g.FillEllipse(Brushes.Red, contour[0].X - 5, contour[0].Y - 5, 10, 10);
                    g.FillEllipse(Brushes.Magenta, contour.Last().X - 5, contour.Last().Y - 5, 10, 10);
                }


                img.Save("lol.png");
            }
        }
    }
}