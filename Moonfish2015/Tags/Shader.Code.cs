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
        Texture texture;
        public void LoadShader(MapStream map)
        {
            var bitmapTag = map[postprocessDefinition[0].bitmaps[2].bitmapGroup].Deserialize() as BitmapBlock;
            texture = new Texture();
            texture.Load(bitmapTag, map);
            texture.Bind(TextureTarget.Texture2D);
        }
    }
}
