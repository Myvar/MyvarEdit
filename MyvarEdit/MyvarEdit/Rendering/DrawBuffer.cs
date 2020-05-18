using System;
using System.Collections.Generic;
using System.IO;
using MyvarEdit.TrueType;
using OpenToolkit.Graphics.OpenGL;
using OpenToolkit.Windowing.Desktop;

namespace MyvarEdit.Rendering
{
    public static class DrawBuffer
    {
        #region Overloads

        public static void DrawRect(Color c, float x, float y, float w, float h) => DrawRect(c, new Rect(x, y, w, h));


        public static void DrawString(Color c, string s, float size, float x, float y) =>
            DrawString(c, s, size, new Point(x, y));

        #endregion

        private static Shader _lineShader;
        private static Shader _rectShader;
        private static Matrix4f _orthMat;
        public static Size ScreenSize;
        private static Mesh _quad;
        private static TrueTypeFontFile _ttf = new TrueTypeFontFile();

        public static void Init(int width, int height)
        {
            _ttf.Load("./Fonts/Hack-Regular.ttf");
            _lineShader = new Shader(File.ReadAllText("./Shaders/line.glsl"));
            _rectShader = new Shader(File.ReadAllText("./Shaders/rect.glsl"));
            _quad = new Mesh();
            _quad.Load(
                new Vertex[]
                {
                    new Vertex(new Point(0, 0)),
                    new Vertex(new Point(1, 0)),
                    new Vertex(new Point(1, 1)),
                    new Vertex(new Point(0, 1)),
                }, new List<uint>()
                {
                    0, 1, 2,
                    2, 3, 0
                });
            Resize(width, height);
        }

        public static void Resize(int width, int height)
        {
            var orth = new Matrix4f().InitIdentity().InitOrthographic(0, width, height, 0, -1, 1);

            _orthMat = orth;

            ScreenSize = new Size(width, height);
        }

        public static void Flush()
        {
        }


        public static void DrawRect(Color c, Rect r)
        {
            _rectShader.Apply();

            var trans = new Matrix4f().InitIdentity().InitTranslation(r.Location.X, r.Location.Y, 0);
            var scale = new Matrix4f().InitIdentity().InitScale(r.Size.Width, r.Size.Height, 0);

            _rectShader.SetUniform("mvp", _orthMat * trans * scale);
            _rectShader.SetUniform("uColor", c);
            _quad.Draw();
        }

        public static Size MesureString(string s, float size)
        {
            var lineHeight = 1.0f;
            var spacesInTab = 4;

            var xOff = 0f;
            var yOff = 0f;

            foreach (var c in s)
            {
                var gl = _ttf.Glyfs[(byte) c];
                var mesh = GlyfCache.GetGlyfMesh(gl);
                //we need to scale things down
                var maxWidth = _ttf.Header.Xmax + 0.0000000001f;
                var maxHeight = _ttf.Header.Ymax + 0.0000000001f;

                if (char.IsWhiteSpace(c))
                {
                    if (c == '\n')
                    {
                        yOff += (size - (size * (_ttf.HorizontalHeaderTable.lineGap / maxHeight)) * lineHeight);
                        xOff = 0;
                    }
                    else if (c == '\t')
                    {
                        xOff += size * spacesInTab;
                    }
                    else
                    {
                        xOff += size;
                    }

                    continue;
                }

                xOff += size * (_ttf.longHorMetrics[0].advanceWidth / maxWidth);
            }

            return new Size(xOff, yOff);
        }

        public static void DrawString(Color clr, string s, float size, Point p)
        {
            var lineHeight = 1.0f;
            var spacesInTab = 4;

            var xOff = 0f;
            var yOff = 0f;

            foreach (var c in s)
            {
                var gl = _ttf.Glyfs[(byte) c];
                var mesh = GlyfCache.GetGlyfMesh(gl);
                //we need to scale things down
                var maxWidth = _ttf.Header.Xmax + 0.0000000001f;
                var maxHeight = _ttf.Header.Ymax + 0.0000000001f;

                if (char.IsWhiteSpace(c))
                {
                    if (c == '\n')
                    {
                        yOff += (size - (size * (_ttf.HorizontalHeaderTable.lineGap / maxHeight)) * lineHeight);
                        xOff = 0;
                    }
                    else if (c == '\t')
                    {
                        xOff += size * spacesInTab;
                    }
                    else
                    {
                        xOff += size;
                    }

                    continue;
                }


                var scaleFactorX = 1f / maxWidth;
                var scaleFactorY = 1f / maxHeight;


                var scaleX = size * scaleFactorX;
                var scaleY = size * scaleFactorY;

                var trans = new Matrix4f().InitIdentity().InitTranslation(xOff + p.X, yOff + p.Y + (size), 0);
                var scale = new Matrix4f().InitIdentity().InitScale(scaleX, -scaleY, 0);


                _rectShader.Apply();
                _rectShader.SetUniform("mvp", _orthMat * trans * scale);
                _rectShader.SetUniform("uColor", clr);


                xOff += size * (_ttf.longHorMetrics[0].advanceWidth / maxWidth);

                mesh.Draw();
            }
        }
    }
}