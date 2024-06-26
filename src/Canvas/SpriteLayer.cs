﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tile_mapper.src.Canvas;
using tile_mapper.src.UserSprites;

namespace tile_mapper.src.Layer
{
    internal class SpriteLayer
    {
        public Tile[,] TileMap;
        public List<Property> Properties;

        public SpriteLayer(Rectangle rectangle)
        {
            TileMap = new Tile[rectangle.Height, rectangle.Width];
            Properties = new List<Property>();
        }
    }
}
