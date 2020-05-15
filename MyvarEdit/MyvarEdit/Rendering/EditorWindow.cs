using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using OpenToolkit.Graphics.OpenGL;
using OpenToolkit.Windowing.Common;
using OpenToolkit.Windowing.Desktop;
using OpenToolkit.Windowing.GraphicsLibraryFramework;

namespace MyvarEdit.Rendering
{
    public class EditorWindow : GameWindow
    {
        private MultisampleRenderTarget _target;

        public EditorWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(
            gameWindowSettings, nativeWindowSettings)
        {
            Title = "MyvarEdit";
            VSync = VSyncMode.Off;
        }

        protected override void OnLoad()
        {
            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            DrawBuffer.Init(Size.X, Size.Y);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.DepthClamp);
            GL.Enable(EnableCap.Multisample);

            _target = new MultisampleRenderTarget(Size.X, Size.Y);
        }


        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);
            DrawBuffer.Resize(e.Width, e.Height);
            _target.Dispose();
            _target = new MultisampleRenderTarget(e.Width, e.Height);
        }


        private List<float> _fpsAvg = new List<float>();

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            _target.Bind();
            GL.ClearColor(0, 0, 0, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _fpsAvg.Add((float) (1f / (args.Time + 0.00000001f)));

            //DrawBuffer.DrawRect(Color.Red, 10, 10, 100, 100);

            var sb = new StringBuilder();

            for (int i = 33; i < 127; i++)
            {
                var c = (char) i;
                sb.Append(c);

                if (i % 10 == 0) sb.AppendLine();
            }

            DrawBuffer.DrawString(Color.White, sb.ToString(), 50, 10, 25);
            //DrawBuffer.DrawString(Color.White, "\"", 50, 10, 25);


            DrawBuffer.DrawString(Color.Green, $"Fps: {MathF.Round(_fpsAvg.Average(), MidpointRounding.ToEven)}", 10,
                10, 10);


            DrawBuffer.Flush();

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            _target.Blit();

            SwapBuffers();

            if (_fpsAvg.Count == 1000) _fpsAvg.RemoveAt(0);
        }
    }
}