using System;
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


        protected override void OnRenderFrame(FrameEventArgs args)
        {
            _target.Bind();
            GL.ClearColor(0, 0, 0, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            DrawBuffer.DrawRect(Color.Red, 10, 10, 100, 100);
            DrawBuffer.DrawString(Color.Cyan, "Hello World", 50, 50, 50);
            DrawBuffer.DrawString(Color.Cyan, "Hello World", 50, 50, 115);

            DrawBuffer.Flush();

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            _target.Blit();

            SwapBuffers();
        }
    }
}