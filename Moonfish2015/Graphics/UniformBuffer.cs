using OpenTK;
using OpenTK.Graphics.ES30;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonfish.Graphics
{
    public class UniformBuffer
    {
        public string Name { get; private set; }

        int buffer;
        Dictionary<int, UniformLayout> Uniforms;

        public enum Uniform : int
        {
            WorldMatrix = 1,
            ViewProjectionMatrix = 2,
            ExtentsMatrix = 3,
        }

        public UniformBuffer()
        {
            //OpenGL.ReportError();
            //Name = "GlobalMatrices";
            //Uniforms = new Dictionary<int, UniformLayout>(3);
            //Uniforms[(int)Uniform.ExtentsMatrix] = new UniformLayout(0, Maths.SizeOfMatrix4);
            //Uniforms[(int)Uniform.WorldMatrix] = new UniformLayout(Maths.SizeOfMatrix4, Maths.SizeOfMatrix4);
            //Uniforms[(int)Uniform.ViewProjectionMatrix] = new UniformLayout(Maths.SizeOfMatrix4 * 2, Maths.SizeOfMatrix4);

            //buffer = GL.GenBuffer();
            //GL.BindBuffer(BufferTarget.UniformBuffer, buffer);
            //OpenGL.ReportError();
            //GL.BufferData(BufferTarget.UniformBuffer, (IntPtr)(Maths.SizeOfMatrix4 * 3), IntPtr.Zero, BufferUsageHint.StreamDraw);
            //OpenGL.ReportError();
            //GL.BindBuffer(BufferTarget.UniformBuffer, 0);
            //OpenGL.ReportError();
        }

        public void BufferUniformData(Uniform uniform, Matrix4 matrix4)
        {
            BufferUniformData(uniform, ref matrix4);
        }

        public Matrix4 this[Uniform uniform]
        {
            set
            {
                BufferUniformData(uniform, value);
            }
        }

        private void UseDefault(Uniform uniform)
        {
            BufferUniformData(uniform, Matrix4.Identity);
        }

        private void BufferUniformData(Uniform uniform, ref Matrix4 matrix4)
        {
            if(!Uniforms.ContainsKey((int)uniform)) throw new ArgumentOutOfRangeException();

            var layout = Uniforms[(int)uniform];

            GL.BindBuffer(BufferTarget.UniformBuffer, buffer);
            GL.BufferSubData(BufferTarget.UniformBuffer, layout.Offset, layout.Size, ref matrix4);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
            OpenGL.ReportError();
        }

        struct UniformLayout
        {
            public readonly IntPtr Offset;
            public readonly IntPtr Size;

            public UniformLayout(int offset, int size)
            {
                this.Offset = (IntPtr)offset;
                this.Size = (IntPtr)size;
            }
        }

        internal void BindBufferRange(int bindingIndex)
        {
            GL.BindBufferRange(BufferRangeTarget.UniformBuffer, bindingIndex, buffer, IntPtr.Zero, (IntPtr)(Maths.SizeOfMatrix4 * 3));
            OpenGL.ReportError();
        }
    }
}
