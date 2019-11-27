using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using mEdit.Core.OpenGL;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace mEdit.Core
{
    public static class OpenTKHost
    {
        private static Shader _shader { get; set; }
        private static Matrix4f _projection { get; set; }
        private static Transform _tranform { get; set; }
        private static Model _quad { get; set; }
        private static DynamicFont _font { get; set; }
        private static DynamicFont _fontAwesome { get; set; }


        public static GameWindow Window { get; set; } = new GameWindow(800, 600, new GraphicsMode(
            new ColorFormat(8, 8, 8, 0),
            24, // Depth bits
            8, // Stencil bits
            16 // FSAA samples
        ), "Editor");

        public static void Host()
        {
            Window.RenderFrame += WindowOnRenderFrame;
            Window.Load += WindowOnLoad;
            Window.Resize += (sender, args) =>
            {
                _projection = GetOrth(Window);
                GL.Viewport(0, 0, Window.Width, Window.Height);
            };

            Window.Run();
        }


        public static void DrawString(DynamicFont f, string s, int xo, int yo, int size)
        {
            var color = Color.White;

            var x = (float) xo;
            var y = (float) yo;

            float xStart = x;

            var maxy = 0;

            foreach (var c in s)
            {
                (
                    Model mesh,
                    float advanceWidth,
                    float leftSideBearing,
                    float lineAscender,
                    float lineDescender,
                    float lineGap,
                    float x0,
                    float y0,
                    float x1,
                    float y1) = f[c, size];


                if (y1 > maxy) maxy = (int) y1;
            }

            for (var i = 0; i < s.Length; i++)
            {
                var c = s[i];


                (
                    Model mesh,
                    float advanceWidth,
                    float leftSideBearing,
                    float lineAscender,
                    float lineDescender,
                    float lineGap,
                    float x0,
                    float y0,
                    float x1,
                    float y1) = f[c, size];

                var scale = (float) size / (float) maxy;
                color = Color.White;
                if (i + 1 < s.Length && i > 1)
                {
                    var b = s[i - 1];
                    var a = s[i + 1];
                    var bb = s[i - 2];
                    
                    if(b == 'v' && c == 'a' && a == 'r') color = Color.Purple;
                    if(b == 'a' && c == 'r' && a == ' ') color = Color.Purple;
                    if( c == 'v' && a == 'a') color = Color.Purple;
                    
                    if (char.IsSymbol(c))
                    {
                        color = Color.Cyan;
                    }
                    if (char.IsNumber(c) && !char.IsLetter(b))
                    {
                        color = Color.Green;
                    }
                    // if (char.IsSymbol(c) && (char.IsSymbol(a)))
                    {
                        //@HACK ^ 1024 this only works for this one font on this one version
                        var replaceList = new Dictionary<(char, char), uint>()
                        {
                            {('=', '>'), 1594},
                            {('<', '='), 1624},
                            {('>', '='), 1601},
                            {(':', '='), 1076},
                            {(':', ':'), 1073},
                            {('>', '>'), 1603},
                            {('<', '<'), 1631},
                            {('-', '>'), 1059},
                            {('<', '-'), 1607},
                            {('!', '='), 1082},
                            {('/', '/'), 1108},
                            {('/', '='), 1105},
                            {('*', '*'), 1088},
                            {('*', '='), 1072},
                            {('|', '|'), 1577},
                            {('&', '&'), 1572},
                            {('=', '='), 1591},
                            {('+', '+'), 1586},
                        };
                        if (replaceList.ContainsKey((c, a)))
                        {
                            x += advanceWidth * scale;
                            continue;
                        }

                        if (replaceList.ContainsKey((b, c)))
                        {
                            (mesh,
                                advanceWidth,
                                leftSideBearing,
                                lineAscender,
                                lineDescender,
                                lineGap,
                                x0,
                                y0,
                                x1,
                                y1) = f.GetGlyph(replaceList[(b, c)], size);
                            color = Color.Cyan;
                            x += advanceWidth * scale;
                            // i++;
                        }
                    }
                }

              

               

                if (c == '\n')
                {
                    y += (int) ((maxy * 1.2f) * scale);
                    x = xStart;
                    continue;
                }


                if (Char.IsWhiteSpace(c))
                {
                    x += advanceWidth * scale;

                    continue;
                }

                void SetupDraw(float x, float y, float w, float h, Color c)
                {
                    _tranform.Translation = new Vector3f(x, OpenTKHost.Window.Height - y - h, 0);
                    _tranform.Scale = new Vector3f(w, h, 0);


                    _shader.SetUniform("Model", _tranform.GetTranformation());
                    _shader.SetUniform("Proj", _projection);
                    _shader.SetUniform("color", c);
                }

                SetupDraw(x + (leftSideBearing * scale), y + ((lineAscender + (maxy)) * scale), scale, scale,
                    color);

                x += advanceWidth * scale;


                _quad = mesh;
                _quad.Draw();
            }
        }

        private static void WindowOnRenderFrame(object sender, FrameEventArgs e)
        {
            GL.ClearColor(0, 0, 0, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            Start2D();
            _shader.Apply();

            var s =
                "Hello World\n0123456789\n0123456789\nMy Name is bob? i think\nligatures\t >> :: << ++ == >= <= => := -> // != || \\\\ && *=";
            DrawString(_font, File.ReadAllText("./testcode.txt"), 0, 0, 18);
            //  DrawString(_fontAwesome, "\n", 0, 0, 18);

            End2D();

            Window.SwapBuffers();
        }

        private static void WindowOnLoad(object sender, EventArgs e)
        {
            //culling
            GL.FrontFace(FrontFaceDirection.Cw);
            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.CullFace);

            //Depth
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.DepthClamp);

            GL.Enable(EnableCap.Texture2D);


            _shader = new Shader("Shaders.GUI");
            _shader.SetUniform("color", Color.Gray);
            _projection = GetOrth(Window);
            _tranform = new Transform();

            _font = new DynamicFont(Utils.GetResourceFileStream("FiraCode-Retina.ttf"));
            _fontAwesome = new DynamicFont(Utils.GetResourceFileStream("fa-solid.ttf"));
        }


        public static void Start2D()
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.CullFace);
        }

        public static void End2D()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Blend);
            GL.Enable(EnableCap.CullFace);
        }


        public static Matrix4f GetOrth(GameWindow Window)
        {
            return new Matrix4f().InitOrthographic(0, Window.Width, 0, Window.Height, -1, 1);
        }
    }
}