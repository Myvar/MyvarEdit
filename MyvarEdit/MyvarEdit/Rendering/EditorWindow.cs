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

            MyvarEditEngine.Draw();

            DrawBuffer.DrawString(Color.Green, $"Fps: {MathF.Round(_fpsAvg.Average(), MidpointRounding.ToEven)}", 10,
                10, DrawBuffer.ScreenSize.Height - 15);


            DrawBuffer.Flush();

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            _target.Blit();

            SwapBuffers();

            if (_fpsAvg.Count == 1000) _fpsAvg.RemoveAt(0);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            MyvarEditEngine.Input.Handel(e);
        }
    }
}