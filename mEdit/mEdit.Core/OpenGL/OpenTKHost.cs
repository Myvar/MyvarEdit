using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using mEdit.Core.Editor;
using mEdit.Core.OpenGL;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using MeshMode1 = OpenTK.Graphics.ES11.MeshMode1;

namespace mEdit.Core
{
    public static class OpenTKHost
    {
        private static Shader _shader { get; set; }
        private static Matrix4f _projection { get; set; }
        private static Transform _tranform { get; set; }
        private static Model _quad { get; set; }
        private static Model _sqQuad { get; set; }


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

            Window.KeyPress += (sender, args) =>
            {
                mEditEngine.Workspace.InvokeAllFunction("opentk_keypress", new object[] {args});
            };

            Window.KeyUp += (sender, args) =>
            {
                mEditEngine.Workspace.InvokeAllFunction("opentk_keyup", new object[] {args});
            };

            Window.KeyDown += (sender, args) =>
            {
                mEditEngine.Workspace.InvokeAllFunction("opentk_keydown", new object[] {args});
            };

            Window.TargetRenderFrequency = 200;
            Window.VSync = VSyncMode.Off;

            Window.Run();
        }


        public static int MesureTextWidth(DynamicFont f, string s, int size)
        {
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

            var width = 0f;

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

                width += advanceWidth * scale;
            }

            return (int) width;
        }

        public static void DrawString(DynamicFont f, string s, int xo, int yo, int size, Color clr)
        {
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

                if (i + 1 < s.Length && i > 1)
                {
                    var b = s[i - 1];
                    var a = s[i + 1];
                    var bb = s[i - 2];

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
                    clr);

                x += advanceWidth * scale;


                _quad = mesh;
                _quad.Draw();
            }
        }

        private static void WindowOnRenderFrame(object sender, FrameEventArgs e)
        {
            Window.Title = $"MEdit | {Math.Round(Window.RenderFrequency)}FPS";

            GL.ClearColor(0.1f, 0.1f, 0.1f, 0.1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            Start2D();
            _shader.Apply();

            mEditEngine.Render();

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
            _sqQuad = NewQuad();
        }

        private static Model NewQuad()
        {
            var quad = new Model();
            quad.Load(
                new Vertex[]
                {
                    new Vertex(new Vector3f(0, 0, 0), new Vector2f(0, 0)),
                    new Vertex(new Vector3f(1, 0, 0), new Vector2f(1, 0)),
                    new Vertex(new Vector3f(1, 1, 0), new Vector2f(1, 1)),
                    new Vertex(new Vector3f(0, 1, 0), new Vector2f(0, 1)),
                }, new List<List<uint>>()
                {
                    new List<uint>()
                    {
                        0, 1, 2,
                        2, 3, 0
                    }
                });

            return quad;
        }

        private static float _beamTickCounter;

        public static void RenderIBeam(int x, int y, int textSize, Color color)
        {
            var tickSpeed = 0.3f;


            _beamTickCounter += 1f / (float) Window.TargetRenderFrequency;


            if (_beamTickCounter > tickSpeed * 2)
            {
                _beamTickCounter = 0;
                return;
            }

            if (_beamTickCounter > tickSpeed)
            {
                return;
            }

            var width = 0.4f;

            //botom bar
            // OpenTKHost.FillRectangle(x + (width * 2), y + width, textSize - width, width, color);

            //top bar
            // OpenTKHost.FillRectangle(x + (width * 2), y - width * 2 - textSize, textSize - width, width, color);

            //pipe
            OpenTKHost.FillRectangle(x + (width * 2), y - textSize, width,
                textSize, color);
        }

        public static void FillRectangle(float x, float y, float w, float h, Color color)
        {
            _tranform.Translation = new Vector3f(x, Window.Height - y - h, 0);
            _tranform.Scale = new Vector3f(w, h, 0);


            _shader.SetUniform("Model", _tranform.GetTranformation());
            _shader.SetUniform("Proj", _projection);
            _shader.SetUniform("color", color);
            _sqQuad.Draw();
        }

        private static void Start2D()
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.CullFace);
        }

        private static void End2D()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Blend);
            GL.Enable(EnableCap.CullFace);
        }


        private static Matrix4f GetOrth(GameWindow Window)
        {
            return new Matrix4f().InitOrthographic(0, Window.Width, 0, Window.Height, -1, 1);
        }
    }
}