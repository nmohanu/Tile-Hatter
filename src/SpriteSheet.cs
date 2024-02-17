using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace tile_mapper.src
{
    internal class SpriteTile
    {
        public Rectangle Destination;
        public Rectangle Source;
        public bool Collision;

        public string ID;

        public bool hovers = false;
    }

    internal class SpriteSheet
    {
        public Texture2D Texture;
        int TileSize;
        public SpriteSheet(int tilesize)
        {
            TileSize = tilesize;
        }
    }

    internal class Sprite
    {
        public Rectangle SpriteRect;
    }

    internal class Tile
    {
        public string ID = "0";
        public Rectangle Source;
    }
}
