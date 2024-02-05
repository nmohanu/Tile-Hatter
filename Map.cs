using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace tile_mapper
{

    public class SpriteTile
    {
        public Rectangle Destination;
        public Rectangle Source;
        public bool hovers = false;
    }

    public enum ButtonAction
    {
        None,
        Import
    }
    internal class Button
    {
        public string Text;
        public ButtonAction Action;
        public Rectangle ButtonRect;
        public Rectangle SourceRect = new Rectangle(0, 48, 96, 48);
        public int SelectionX;
        public int OriginalX;
        public bool IsVisible = true;
        public Button(string text, Rectangle rect, int selectionX, int originalX, ButtonAction action) 
        {
            this.Text = text;
            this.ButtonRect = rect;
            this.SelectionX = selectionX;
            this.OriginalX = originalX;
            this.Action = action;

            SourceRect.Width = rect.Width;
            SourceRect.Height = rect.Height;
        }
    }

    internal class GridTile
    {
        int x;
        int y;

        public Rectangle GridRect;
    }
    internal class SpriteSheet
    {
        public Texture2D Texture;
        int TileSize;
        public SpriteSheet(int tilesize)
        {
            this.TileSize = tilesize;
        }
    }

    internal class Sprite
    {
        public Rectangle SpriteRect;
    }

    internal class Tile
    {
        public int ID;
    }

    internal class Map
    {
        int height;
        int width;

        Layer[] layers = new Layer[3];

        public Map(int height, int width)
        {
            this.height = height;
            this.width = width;

            Layer Terrain = new Layer(0, width, height);
            Layer Objects = new Layer(1, width, height);
            Layer Foreground = new Layer(2, width, height);

            layers[0] = Terrain;
            layers[1] = Objects;
            layers[2] = Foreground;
        }
    }

    internal class Layer
    {
        int Depth;
        int Width;
        int Height;

        public Tile[,] TileMap;

        public Layer(int depth, int width, int height)
        {
            this.Depth = depth;
            this.Width = width;
            this.Height = height;

            TileMap = new Tile[height, width];

            foreach (var tile in TileMap)
            {
                tile.ID = 0;
            }
        }
    }
}
