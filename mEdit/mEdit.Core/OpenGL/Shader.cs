using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace mEdit.Core.OpenGL
{
    public class Shader
    {
        private int program;
        private Dictionary<string, int> Uniforms = new Dictionary<string, int>();

        public Shader(string fileName)
        {
            program = GL.CreateProgram();

            if (program == 0)
            {
                Console.WriteLine("Failed to create Shader.");
                Environment.Exit(0);
            }

            GL.BindAttribLocation(program, 0, "position");
            GL.BindAttribLocation(program, 1, "color");
            GL.BindAttribLocation(program, 2, "normal");
            GL.BindAttribLocation(program, 3, "tangent");
            GL.BindAttribLocation(program, 4, "textCoord");

            AddVertexShader(Utils.GetResourceFile(fileName + ".vs.glsl"));
            AddFragmentShader(Utils.GetResourceFile(fileName + ".fs.glsl"));

            CompileShader();


        }


        public void AddUniform(string name)
        {
            int uniformLocation = GL.GetUniformLocation(program, name);

            if (uniformLocation == -1)
            {
                Console.WriteLine("Failed to find uniform: " + name);
                //Environment.Exit(1);
            }

            Uniforms.Add(name, uniformLocation);
        }

        public virtual void SetUniform(object value)
        {
            var type = value.GetType().GetTypeInfo();
            var name = type.Name;

            foreach (var i in type.GetProperties())
            {
                SetUniform(name + "_" + i.Name, i.GetValue(value));
            }
        }

        public virtual void SetUniform(string name, object value)
        {
            if (!Uniforms.ContainsKey(name))
            {
                AddUniform(name);
            }

            if (value.GetType() == typeof(int))
            {
                GL.Uniform1(Uniforms[name], (int)value);
            }
            else if (value.GetType() == typeof(float))
            {
                GL.Uniform1(Uniforms[name], (float)value);
            }
            else if (value.GetType() == typeof(Vector3f))
            {
                var val = (Vector3f)value;
                GL.Uniform3(Uniforms[name], val.X, val.Y, val.Z);
            }
            else if (value.GetType() == typeof(Color))
            {
                var val = (Color)value;
                GL.Uniform3(Uniforms[name], val.R + 0.00001f / 255, val.G+ 0.00001f / 255, val.B+ 0.00001f / 255);
            }
            else if (value.GetType() == typeof(Matrix4f))
            {
                var val = (Matrix4f)value;
                var mat4 = new Matrix4(
                    new Vector4(val[0, 0], val[0, 1], val[0, 2], val[0, 3]),
                    new Vector4(val[1, 0], val[1, 1], val[1, 2], val[1, 3]),
                    new Vector4(val[2, 0], val[2, 1], val[2, 2], val[2, 3]),
                    new Vector4(val[3, 0], val[3, 1], val[3, 2], val[3, 3])
                );

                GL.UniformMatrix4(Uniforms[name], true, ref mat4);
            }
        }

        public void Apply()
        {
            GL.UseProgram(program);
        }

        public void AddVertexShader(string text)
        {
            AddProgram(text, ShaderType.VertexShader);
        }

        public void AddFragmentShader(string text)
        {
            AddProgram(text, ShaderType.FragmentShader);
        }

        public void AddGeometryShader(string text)
        {
            AddProgram(text, ShaderType.GeometryShader);
        }

        public void CompileShader()
        {
            GL.LinkProgram(program);

            int outs = 0;

            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out outs);

            if (outs == 0)
            {
                Console.WriteLine(GL.GetProgramInfoLog(program));
                Environment.Exit(0);
            }


            GL.ValidateProgram(program);

            GL.GetProgram(program, GetProgramParameterName.ValidateStatus, out outs);

            if (outs == 0)
            {
                Console.WriteLine(GL.GetProgramInfoLog(program));
                Environment.Exit(0);
            }


        }

        private void AddProgram(string text, ShaderType type)
        {
            int shader = GL.CreateShader(type);

            if (shader == 0)
            {
                Console.WriteLine("Failed to add Shader.");
                Environment.Exit(0);
            }

            GL.ShaderSource(shader, text);
            GL.CompileShader(shader);

            int outs = 0;

            GL.GetShader(shader, ShaderParameter.CompileStatus, out outs);

            if (outs == 0)
            {
                Console.WriteLine(type.ToString() + ":");
                Console.WriteLine(GL.GetShaderInfoLog(shader));
                Environment.Exit(0);
            }

            GL.AttachShader(program, shader);
        }

        public void Update(Matrix4f Model, Matrix4f MVP)
        {
            SetUniform("sampler", 0);
            SetUniform("Model", Model);
            SetUniform("MVP", MVP);
        }
    }
}