using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tile_mapper.src.Layer
{
    internal class SpriteLayer
    {
        public Tile[,] TileMap;
        public SpriteLayer(Rectangle rectangle)
        {
            TileMap = new Tile[rectangle.Height, rectangle.Width];
        }
    }
}
