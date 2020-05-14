using System;
using OpenToolkit.Graphics.OpenGL;

namespace MyvarEdit.Rendering
{
    public class MultisampleRenderTarget
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int FrameBuffer { get; set; }
        public int DepthBuffer { get; set; }
        public int ColorBuffer { get; set; }
        
        public MultisampleRenderTarget(int width, int height)
        {
            Width = width;
            Height = height;

            int fb;
            GL.GenFramebuffers(1, out fb);
            FrameBuffer = fb;
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FrameBuffer);

            int db;
            GL.GenRenderbuffers(1, out db);
            DepthBuffer = db;

            int cb;
            GL.GenRenderbuffers(1, out cb);
            ColorBuffer = cb;

            var samples = 16; // Might Want to move this to a asetting

            //depth
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, DepthBuffer);
            GL.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer, samples,
                RenderbufferStorage.DepthComponent, Width, Height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment,
                RenderbufferTarget.Renderbuffer, DepthBuffer);

            //color
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, ColorBuffer);
            GL.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer,
                samples, RenderbufferStorage.Rgb8, Width, Height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer,
                FramebufferAttachment.ColorAttachment0,
                RenderbufferTarget.Renderbuffer, ColorBuffer);

            GL.DrawBuffers(1, new[] {DrawBuffersEnum.ColorAttachment0,});

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
            {
                //Terminal.Debug("Failed to create render target");
                Environment.Exit(1);
            }


            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, width, height);
        }

        public void Dispose()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
            GL.DeleteFramebuffer(FrameBuffer);
            GL.DeleteRenderbuffer(DepthBuffer);
            GL.DeleteRenderbuffer(ColorBuffer);
        }

        public void Bind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FrameBuffer);
            GL.Viewport(0, 0, Width, Height);

            //GL.ClearColor(0f, 0f, 0f, 1);
            //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        public void Blit()
        {
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, FrameBuffer);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);

            GL.BlitFramebuffer(0, 0, Width, Height,
                0, 0, (int) DrawBuffer.ScreenSize.Width, (int) DrawBuffer.ScreenSize.Height,
                ClearBufferMask.ColorBufferBit,
                BlitFramebufferFilter.Nearest);
            
        }
    }
}