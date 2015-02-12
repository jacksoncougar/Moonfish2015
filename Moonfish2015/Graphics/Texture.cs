using Moonfish.Guerilla.Tags;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonfish.Graphics
{
    public class Texture        
    {
        int handle;

        public void Load(BitmapBlock bitmapCollection, MapStream map)
        {
            handle = GL.GenTexture();

            var workingBitmap = bitmapCollection.bitmaps[0];
            byte[] buffer = new byte[bitmapCollection.bitmaps[0].lOD1TextureDataLength];

            using (map.Pin())
            {
                map.Position = bitmapCollection.bitmaps[0].lOD1TextureDataOffset;
                map.Read(buffer, 0, buffer.Length);
            }

            var width = workingBitmap.widthPixels;
            var height = workingBitmap.heightPixels;
            var bytesPerPixel = ParseBitapPixelDataSize(workingBitmap.format) / 8.0f;
            PixelInternalFormat pixelInternalFormat = ParseBitmapPixelInternalFormat(workingBitmap.format);


            switch (workingBitmap.type)
            {
                case BitmapDataBlockBase.TypeDeterminesBitmapGeometry.Texture2D:
                    {
                        GL.BindTexture(TextureTarget.Texture2D, this.handle);
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat); OpenGL.ReportError();
                        if (workingBitmap.flags.HasFlag(BitmapDataBlock.Flags.Compressed))
                        {
                            byte[] surfaceData = new byte[(int)(bytesPerPixel * width * height)];
                            Array.Copy(buffer, 0, surfaceData, 0, surfaceData.Length);
                            GL.CompressedTexImage2D(
                                TextureTarget.Texture2D, 0, pixelInternalFormat, width, height, 0, (int)(bytesPerPixel * width * height), surfaceData);
                        }
                        else
                        {
                            var pixelFormat = ParseBitapPixelFormat(workingBitmap.format);
                            var pixelType = ParseBitapPixelType(workingBitmap.format);
                            GL.TexImage2D(TextureTarget.Texture2D, 0, pixelInternalFormat, width, height, 0, pixelFormat, pixelType, buffer);
                        }
                    } break;
                default: GL.DeleteTexture(this.handle); break;
            }

            OpenGL.ReportError();
        }

        public void Bind(TextureTarget target)
        {
            GL.BindTexture(target, this.handle);
        }

        private PixelType ParseBitapPixelType(BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally format)
        {
            switch (format)
            {
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.A1r5g5b5:
                    return PixelType.UnsignedShort5551;

                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.A4r4g4b4:
                    return PixelType.UnsignedShort4444;

                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.R5g6b5:
                    return PixelType.UnsignedShort565;

                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.A8y8:
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.V8u8:
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.G8b8:
                    return PixelType.UnsignedShort;

                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.A8:
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.P8:
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.Y8:
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.P8Bump:
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.Ay8:
                    return PixelType.UnsignedByte;

                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.A8r8g8b8:
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.X8r8g8b8:
                    return PixelType.UnsignedInt;
                default: throw new FormatException("Unsupported Texture Format");
            }
        }
        private PixelFormat ParseBitapPixelFormat(BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally format)
        {
            switch (format)
            {
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.A1r5g5b5:
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.A4r4g4b4:
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.Argbfp32:
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.A8r8g8b8:
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.X8r8g8b8:
                    return PixelFormat.Rgba;
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.R5g6b5:
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.Rgbfp16:
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.Rgbfp32:
                    return PixelFormat.Rgb;

                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.A8y8:
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.Ay8:
                    return PixelFormat.LuminanceAlpha;

                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.V8u8:
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.G8b8:
                    return PixelFormat.Rg;

                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.A8:
                    return PixelFormat.Alpha;

                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.P8:
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.P8Bump:
                    return PixelFormat.ColorIndex;

                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.Y8:
                    return PixelFormat.Luminance;
                default: throw new FormatException("Unsupported Texture Format");
            }
        }
        private float ParseBitapPixelDataSize(BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally format)
        {
            switch (format)
            {
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.A1r5g5b5:
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.A4r4g4b4:
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.R5g6b5:
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.A8y8:
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.V8u8:
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.G8b8:
                    return 16;

                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.A8:
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.P8:
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.P8Bump:
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.Y8:
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.Ay8:
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.Dxt3:
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.Dxt5:
                    return 8;

                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.Dxt1:
                    return 4;

                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.A8r8g8b8:
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.X8r8g8b8:
                    return 32;

                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.Argbfp32:
                    return 128;

                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.Rgbfp16:
                    return 48;

                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.Rgbfp32:
                    return 96;
                default: throw new FormatException("Unsupported Texture Format");
            }
        }
        internal PixelInternalFormat ParseBitmapPixelInternalFormat(BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally format)
        {
            PixelInternalFormat pixelFormat;
            switch (format)
            {
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.A1r5g5b5:
                    pixelFormat = PixelInternalFormat.Rgb5A1; break;
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.A4r4g4b4:
                    pixelFormat = PixelInternalFormat.Rgba4; break;
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.A8:
                    pixelFormat = PixelInternalFormat.Alpha8; break;
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.A8r8g8b8:
                    pixelFormat = PixelInternalFormat.Rgba8; break;
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.A8y8:
                    pixelFormat = PixelInternalFormat.Luminance8Alpha8; break;
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.Argbfp32:
                    pixelFormat = PixelInternalFormat.Rgba32f; break;
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.Ay8:
                    pixelFormat = PixelInternalFormat.Luminance8; break;
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.Dxt1:
                    pixelFormat = PixelInternalFormat.CompressedRgbaS3tcDxt1Ext; break;
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.Dxt3:
                    pixelFormat = PixelInternalFormat.CompressedRgbaS3tcDxt3Ext; break;
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.Dxt5:
                    pixelFormat = PixelInternalFormat.CompressedRgbaS3tcDxt5Ext; break;
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.G8b8:
                    pixelFormat = PixelInternalFormat.Rg8; break;
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.P8:
                    pixelFormat = PixelInternalFormat.R8; break;
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.P8Bump:
                    pixelFormat = PixelInternalFormat.R8; break;
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.R5g6b5:
                    pixelFormat = PixelInternalFormat.R5G6B5IccSgix; break;
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.Rgbfp16:
                    pixelFormat = PixelInternalFormat.Rgb16f; break;
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.Rgbfp32:
                    pixelFormat = PixelInternalFormat.Rgb32f; break;
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.V8u8:
                    pixelFormat = PixelInternalFormat.Rg8; break;
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.X8r8g8b8:
                    pixelFormat = PixelInternalFormat.Rgba8; break;
                case BitmapDataBlockBase.FormatDeterminesHowPixelsAreRepresentedInternally.Y8:
                    pixelFormat = PixelInternalFormat.Luminance8; break;
                default: throw new FormatException("Unsupported Texture Format");
            }
            return pixelFormat;
        }
    }
}
