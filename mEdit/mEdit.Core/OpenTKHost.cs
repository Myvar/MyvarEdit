using System;
using System.Collections.Generic;
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

        private static Texture _texture { get; set; }

        private static DynamicFont _font { get; set; }

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


            Window.Run();
        }

        private static void WindowOnRenderFrame(object sender, FrameEventArgs e)
        {
            GL.ClearColor(0, 0.8f, 0.8f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            Start2D();
            _shader.Apply();

            DrawImage(100, 100, 50, 50, _texture);
            // GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            _quad = _font['G'];
            _quad.Draw();

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
            _projection = GetOrth(Window);
            _tranform = new Transform();
            _texture = new Texture("dev-1.jpg");

            _font = new DynamicFont(Utils.GetResourceFileStream("Inconsolata-Regular.ttf"));

            _quad = NewQuad();
        }


        public static void DrawImage(int x, int y, int w, int h, Texture t)
        {
            t.Apply(0);
            _tranform.Translation = new Vector3f(x, Window.Height - y - h, 0);
            var scale = 0.3f;
            _tranform.Scale = new Vector3f(w, h, 0);


            _shader.SetUniform("Model", _tranform.GetTranformation());
            _shader.SetUniform("Proj", _projection);
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

        public static Model NewQuad()
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

        public static Matrix4f GetOrth(GameWindow Window)
        {
            return new Matrix4f().InitOrthographic(0, Window.Width, 0, Window.Height, -1, 1);
        }
    }
}