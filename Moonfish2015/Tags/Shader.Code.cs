using Moonfish.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonfish.Guerilla.Tags
{
    partial class ShaderBlock
    {
        public void LoadShader(MapStream map)
        {
            var bitmapTag = map[postprocessDefinition[0].bitmaps[2].bitmapGroup].Deserialize() as BitmapBlock;
            var texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texture);
            OpenGL.ReportError();
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            OpenGL.ReportError();
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            OpenGL.ReportError();
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            OpenGL.ReportError();
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            OpenGL.ReportError();

            byte[] buffer = new byte[bitmapTag.bitmaps[0].lOD1TextureDataLength];
            using (map.Pin())
            {
                map.Position = bitmapTag.bitmaps[0].lOD1TextureDataOffset;
                map.Read(buffer, 0, bitmapTag.bitmaps[0].lOD1TextureDataLength);
            }

            GL.CompressedTexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.CompressedRgbaS3tcDxt5Ext, 256, 256, 0, 256 * 256, buffer);

            OpenGL.ReportError();

        }
    }
}
