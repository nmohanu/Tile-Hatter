using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace tile_mapper
{
    internal class UserAction
    {
        public ActionType Action;
        public int x, y, Layer;
       
        public UserAction(ActionType action, int layer, int x, int y) 
        {
            this.Action = action;
            this.Layer = layer;
            this.x = x;
            this.y = y;
        }
        public enum ActionType
        {
            Draw,
            Remove
        }
    }
    internal class SpriteTile
    {
        public Rectangle Destination;
        public Rectangle Source;

        public string ID;

        public bool hovers = false;
    }

    public enum ButtonAction
    {
        None,
        Import,
        Layer,
        Save
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
        public string ID = "0";
        public Rectangle Source;
    }

    internal class Map
    {
        int height;
        int width;

        public Layer[] layers = new Layer[3];

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
        }
    }
}
