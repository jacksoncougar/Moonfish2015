using Moonfish.Graphics;
using Moonfish.Tags;
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
        public ShaderPostprocessBitmapNewBlock[] Bitmaps { get { return postprocessDefinition[0].bitmaps; } }
        Texture texture;
        public void LoadShader(MapStream map)
        {
            var bitmapTag = map[postprocessDefinition[0].bitmaps[0].bitmapGroup].Deserialize() as BitmapBlock;
            texture = new Texture();
            texture.Load(bitmapTag, map);
            texture.Bind(TextureTarget.Texture2D);
        }
    }
}
