using System.Collections.Generic;
using MyvarEdit.TrueType;

namespace MyvarEdit.Rendering
{
    public static class GlyfCache
    {
        private static Dictionary<Glyf, Mesh> _cache = new Dictionary<Glyf, Mesh>();

        public static Mesh GetGlyfMesh(Glyf gl)
        {
            if (_cache.ContainsKey(gl)) return _cache[gl];

            var mesh = new Mesh();

            var verts = new List<Vertex>();
            var indexies = new List<uint>();
            foreach (var triangle in gl.Triangles)
            {
                verts.Add(new Vertex(new Point(triangle.A.X, triangle.A.Y)));
                indexies.Add((uint) verts.Count - 1);

                verts.Add(new Vertex(new Point(triangle.B.X, triangle.B.Y)));

                indexies.Add((uint) verts.Count - 1);
                verts.Add(new Vertex(new Point(triangle.C.X, triangle.C.Y)));
                indexies.Add((uint) verts.Count - 1);
            }

            mesh.Load(verts.ToArray(), indexies);


            _cache.Add(gl, mesh);

            return mesh;
        }
    }
}