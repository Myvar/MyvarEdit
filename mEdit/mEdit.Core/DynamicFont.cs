using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using LibTessDotNet;
using mEdit.Core.OpenGL;
using TrueTypeSharp;

namespace mEdit.Core
{
    public class DynamicFont
    {
        private Dictionary<char, Model> _dictionary = new Dictionary<char, Model>();

        private TrueTypeFont _font;

        public DynamicFont(Stream s)
        {
            using (var mem = new MemoryStream())
            {
                s.CopyTo(mem);
                _font = new TrueTypeFont(mem.ToArray(), 0);
            }
        }

        public Model this[char c]
        {
            get
            {
                if (!_dictionary.ContainsKey(c))
                {
                    LoadGlyph(c);
                }

                return _dictionary[c];
            }
        }

        float Bezier(float p0, float p1, float p2, float t) // Parameter 0 <= t <= 1
        {
            //B(T) = P1 + (1 - t)^2 * (P0 - P1) + t^2 (P2 - P1)

            return p1 + MathF.Pow(1f - t, 2) * (p0 - p1) + MathF.Pow(t, 2) * (p2 - p1);
        }

        float Normalize(float value, float min, float max)
        {
            return (value - min) / (max - min);
        }


        private void LoadGlyph(char ch)
        {
            var vertexes = new List<Vertex>();
            var indecies = new List<uint>();
            // Render some characters...

            int x0, y0, x1, y1;
            var shape = _font.GetCodepointShape(ch);
            _font.GetCodepointBox(ch, out x0, out y0, out x1, out y1);

            var tess = new Tess();


            var c = 0;
            var last = 0;


            var vecs = new List<Vec3>();

            for (var i = 1; i < shape.Length; i++)
            {
                var it = shape[i - 1];
                var it2 = shape[i];
                if (it.Type == GlyphVertexType.Move)
                {
                    {
                        var contour = new ContourVertex[vecs.Count];

                        for (var index = 0; index < vecs.Count; index++)
                        {
                            contour[index].Position = vecs[index];
                            contour[index].Data = Color.Black;
                        }

                        tess.AddContour(contour, ContourOrientation.Clockwise);
                        vecs.Clear();
                    }
                }

                var res = it2.Type == GlyphVertexType.Curve ? 100f : 1f;

                for (int j = 0; j <= res; j++)
                {
                    var t = j / res;
                    vecs.Add(new Vec3()
                    {
                        X = Bezier(it.X, it2.CX, it2.X, t),
                        Y = Bezier(it.Y, it2.CY, it2.Y, t),
                        Z = 0
                    });
                }
            }

            {
                var contour = new ContourVertex[vecs.Count];

                for (var index = 0; index < vecs.Count; index++)
                {
                    contour[index].Position = vecs[index];
                    contour[index].Data = Color.Black;
                }

                tess.AddContour(contour, ContourOrientation.Clockwise);
                vecs.Clear();
            }


            tess.Tessellate(WindingRule.EvenOdd, ElementType.Polygons, 3);


            int numTriangles = tess.ElementCount;
            for (int i = 0; i < numTriangles; i++)
            {
                var v0 = tess.Vertices[tess.Elements[i * 3]].Position;
                var v1 = tess.Vertices[tess.Elements[i * 3 + 1]].Position;
                var v2 = tess.Vertices[tess.Elements[i * 3 + 2]].Position;

                vertexes.Add(new Vertex(new Vector3f(
                    Normalize(v0.X, x0, x1), Normalize(v0.Y, y0, y1), 0)));
                indecies.Add((uint) (vertexes.Count - 1));
                vertexes.Add(new Vertex(new Vector3f(
                    Normalize(v1.X, x0, x1), Normalize(v1.Y, y0, y1), 0)));
                indecies.Add((uint) (vertexes.Count - 1));
                   vertexes.Add(new Vertex(new Vector3f(
                    Normalize(v2.X, x0, x1), Normalize(v2.Y, y0, y1), 0)));
                indecies.Add((uint) (vertexes.Count - 1));
            }


            var m = new Model();
            m.Load(vertexes.ToArray(), new List<List<uint>> {indecies});


            _dictionary.Add(ch, m);
        }
    }
}