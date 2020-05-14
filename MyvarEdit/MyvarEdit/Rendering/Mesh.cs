using System;
using System.Collections.Generic;
using System.Linq;
using OpenToolkit.Graphics.OpenGL;

namespace MyvarEdit.Rendering
{
    public class Mesh
    {
        private int vbo;
        private int ibo;
        private int ab;
        public int Size { get; set; }


        public Mesh()
        {
            vbo = GL.GenBuffer();
            ibo = GL.GenBuffer();
            ab = GL.GenVertexArray();
        }

        public void Load(Vertex[] vertexsData, List<uint> indices)
        {
            var vertexData = new List<byte>();


            foreach (var d in vertexsData)
            {
                vertexData.AddRange(BitConverter.GetBytes(d.Position.X));
                vertexData.AddRange(BitConverter.GetBytes(d.Position.Y));
            }


            var data = vertexData.ToArray();

            Size = indices.Count();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length, data, BufferUsageHint.StaticDraw);


            var index = indices;


            var buf = new List<byte>();
            foreach (var i in index)
            {
                buf.AddRange(BitConverter.GetBytes(i));
            }


            //indices[]
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, buf.Count, buf.ToArray(),
                BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }


        public void Dispose()
        {
            GL.DeleteBuffer(vbo);
            GL.DeleteBuffer(ibo);
        }

        public void Draw()
        {
            GL.BindVertexArray(ab);
            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * 4, 0);


            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);

            GL.DrawElements(BeginMode.Triangles, Size, DrawElementsType.UnsignedInt, 0);


            GL.DisableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }
    }
}