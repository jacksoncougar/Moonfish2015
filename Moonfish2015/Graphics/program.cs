using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.ES30;
using System.IO;
using System.Windows.Forms;
using OpenTK;

namespace Moonfish.Graphics
{
    public class Program : IDisposable
    {
        int program_id;

        public readonly string Name;

        public UniformBuffer UniformBuffer { get; private set; }

        public void BindUniformBuffer(UniformBuffer buffer, int bindingIndex)
        {
            this.UniformBuffer = buffer;
            var uniformBlockIndex = GL.GetUniformBlockIndex(program_id, buffer.Name);
            GL.UniformBlockBinding(program_id, uniformBlockIndex, bindingIndex);
            OpenGL.ReportError();
        }

        Dictionary<string, int> uniforms;
        Dictionary<Uniforms, string> globalUniforms;
        Dictionary<string, int> attributes;
        Dictionary<string, Stack<Object>> uniformStack;

        public int ID { get { return program_id; } }

        public Program(string name)
        {
            this.Name = name;

            attributes = new Dictionary<string, int>();
            uniforms = new Dictionary<string, int>();
            uniformStack = new Dictionary<string, Stack<object>>();
            globalUniforms = new Dictionary<Uniforms, string>();
            globalUniforms[Uniforms.WorldMatrix] = "objectWorldMatrix";
            globalUniforms[Uniforms.NormalizationMatrix] = "objectExtents";
            globalUniforms[Uniforms.ViewProjectionMatrix] = "viewProjectionMatrix";

            program_id = GL.CreateProgram();
        }

        public void Link(List<Shader> shader_list)
        {
            foreach (Shader shader in shader_list)
            {
                GL.AttachShader(program_id, shader.ID);
            }

            GL.LinkProgram(program_id);

            int status;
            GL.GetProgram(program_id, GetProgramParameterName.LinkStatus, out status);
            if (status == 0)
            {
                string program_log = GL.GetProgramInfoLog(program_id);
                MessageBox.Show(String.Format("Linker failure: {0}\n", program_log));
            }
            GL.ValidateProgram(program_id);
            int valid;
            GL.GetProgram(program_id, GetProgramParameterName.ValidateStatus, out valid);
            if (valid == 0)
            {
                string program_log = GL.GetProgramInfoLog(program_id);
                MessageBox.Show(String.Format("Validation failure {0}", program_log));
            }

            foreach (Shader shader in shader_list)
            {
                GL.DetachShader(program_id, shader.ID);
            }
            Initialize();
        }

        private void Initialize()
        {
        }

        private bool GetAttributeLocation(string name, out int location)
        {
            if (attributes.ContainsKey(name))
            {
                location = attributes[name];
                return true;
            }
            else
            {
                location = GL.GetAttribLocation(this.ID, name);
                if (location == -1)
                {
                    Console.WriteLine("invalid attribute name: {0}", name);
                    return false;
                }
                else attributes[name] = location;
                return true;
            }
        }

        public void SetAttribute(string name, Vector4 value)
        {
            int location;
            if (GetAttributeLocation(name, out location))
            {
                GL.VertexAttrib4(location + 0,value);
            }
        }

        public void SetAttribute(string name, float[] values)
        {
            int location;
            if (GetAttributeLocation(name, out location))
            {
                GL.VertexAttrib4(location + 0, values);
            }
        }

        public void SetAttribute(string name, Matrix4 value)
        {
            int location;
            if (GetAttributeLocation(name, out location))
            {
                GL.VertexAttrib4(location + 0, value.Row0);
                GL.VertexAttrib4(location + 1, value.Row1);
                GL.VertexAttrib4(location + 2, value.Row2);
                GL.VertexAttrib4(location + 3, value.Row3);
            }
        }

        public IDisposable Using(string uniformName, object value)
        {
            uniformStack[uniformName].Push(value);
            this[uniformName] = uniformStack[uniformName].Pop();
            return new UniformHandle(uniformStack[uniformName].Peek(), GetUniformID(uniformName));
        }

        private class UniformHandle : IDisposable
        {
            Object previous_uniform_value;
            int uniform_id;

            public UniformHandle(Object previousUniformValue, int uniformID)
            {
                previous_uniform_value = previousUniformValue;
                uniform_id = uniformID;
            }

            public void Dispose()
            {
                // Program.SetUniform(previous_uniform_value, uniform_id);
            }
        }

        public object this[string uniform_name]
        {
            set
            {
                int uid;
                uid = GetUniformID(uniform_name);
                if (uid == -1) return;
                if (!uniformStack.ContainsKey(uniform_name))
                {
                    uniformStack[uniform_name] = new Stack<object>();
                    uniformStack[uniform_name].Push(value);
                }
                SetUniform(value, uid);
            }
            get
            {
                if (uniformStack.ContainsKey(uniform_name))
                {
                    return uniformStack[uniform_name].Peek();
                }
                return null;

            }

        }

        public Matrix4 this[Uniforms uniform]
        {
            get
            {
                return (Matrix4)this[globalUniforms[uniform]];
            }
            set
            {
                this[globalUniforms[uniform]] = value;
            }
        }

        private int GetUniformID(string uniform_name)
        {
            int uid;
            if (uniforms.ContainsKey(uniform_name))
                uid = uniforms[uniform_name];
            else
            {
                GL.UseProgram(this.program_id);
                uid = uniforms[uniform_name] = GL.GetUniformLocation(ID, uniform_name);
            }
            return uid;
        }

        private void SetUniform(object value, int uid)
        {
            Type t = value.GetType();
            if (t == typeof(Matrix4))
            {
                var temp = (Matrix4)value;
                GL.UseProgram(this.program_id);
                GL.UniformMatrix4(uid, false, ref temp);
            }
            else if (t == typeof(Matrix3))
            {
                var temp = (Matrix3)value;
                GL.UseProgram(this.program_id);
                GL.UniformMatrix3(uid, false, ref temp);
            }
            else if (t == typeof(Vector3))
            {
                GL.UseProgram(this.program_id);
                GL.Uniform3(uid, (Vector3)value);
            }
            else if (t == typeof(float))
            {
                var temp = (float)value;
                GL.UseProgram(this.program_id);
                GL.Uniform1(uid, temp);
            }
            else if (t.IsArray && t.GetElementType() == typeof(float))
            {
                var temp = (float[])value;
                GL.UseProgram(this.program_id);
                switch (temp.Length)
                {
                    case 1:
                        GL.Uniform1(uid, 1, temp); break;
                    case 2:
                        GL.Uniform2(uid, 1, temp); break;
                    case 3:
                        GL.Uniform3(uid, 1, temp); break;
                    case 4:
                        GL.Uniform4(uid, 1, temp); break;
                }
                OpenGL.ReportError();
            }
            else throw new InvalidDataException();
        }

        public IDisposable Use()
        {
            GL.UseProgram(this.ID);
            return new Handle(0);
        }

        private class Handle : IDisposable
        {
            int previous_program_id;

            public Handle(int prev)
            {
                previous_program_id = prev;
            }

            public void Dispose()
            {
                GL.UseProgram(previous_program_id);
            }
        }

        public void Dispose()
        {
            GL.DeleteProgram(this.ID);
            GL.UseProgram(0);
        }
    }

    public enum Uniforms : int
    {
        WorldMatrix = 1,
        NormalizationMatrix = 2,
        ViewProjectionMatrix = 3,
    }
}
