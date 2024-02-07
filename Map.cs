using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace tile_mapper
{
    internal class UI_Menu
    {
        public bool IsVisible;
        public Rectangle Source;
        public Rectangle Destination;
        public List<Button> buttons;

        public UI_Menu(bool IsVisible, Rectangle Source, Rectangle Destination)
        {
            buttons = new List<Button>();
            this.IsVisible = IsVisible;
            this.Source = Source;
            this.Destination = Destination;
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D UI, int ScreenHeight, int ScreenWidth, float ScaleX, float ScaleY, SpriteFont font, float TextScale)
        {
            if(IsVisible)
            {
                spriteBatch.Draw(UI, new Vector2(Destination.X, Destination.Y), Source, Color.White, 0f, Vector2.Zero, new Vector2(ScaleX, ScaleY), SpriteEffects.None, 0);
                foreach (var button in buttons)
                {
                    if(button != null && button.IsVisible)
                    {
                        spriteBatch.Draw(UI, button.ButtonRect, button.SourceRect, Color.White);
                        spriteBatch.DrawString(
                        font,
                        button.Text,
                        new Vector2(
                                button.ButtonRect.X + button.ButtonRect.Width / 2 - font.MeasureString(button.Text).X * TextScale / 2,
                                button.ButtonRect.Y + button.ButtonRect.Height / 2 - font.MeasureString(button.Text).Y * TextScale / 2
                            ),
                            Color.White,
                            0f, // Rotation angle, set to 0 for no rotation
                            Vector2.Zero, // Origin, set to Vector2.Zero for the default origin
                            TextScale, // Scale factor
                            SpriteEffects.None, // Sprite effects, set to None for no effects
                            0f // Depth, set to 0 for the default depth
                        );
                    }
                }  
            }
        }

        public ButtonAction HandleClicks(Vector2 MousePos)
        {
            foreach (var button in buttons)
            {
                if (button != null && button.IsVisible && button.ButtonRect.Contains(MousePos))
                {
                    return button.Action;
                }
            }
            return ButtonAction.None;
        }
    }

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
        Save,
        OpenPalette
    }
    internal class Button
    {
        public string Text;
        public ButtonAction Action;
        public Rectangle ButtonRect;
        public Rectangle SourceRect = new Rectangle(0, 48, 96, 48); // Standard button
        public int SelectionX;
        public int OriginalX;
        public bool IsVisible = true;
        public Button(string text, Rectangle rect, int selectionX, int originalX, ButtonAction action, bool isVisible) 
        {
            this.Text = text;
            this.ButtonRect = rect;
            this.SelectionX = selectionX;
            this.OriginalX = originalX;
            this.Action = action;
            this.IsVisible = isVisible;

            SourceRect.Width = rect.Width;
            SourceRect.Height = rect.Height;
        }
        public void ChangeSourceX(Vector2 MousePos)
        {
            if (this.ButtonRect.Contains(MousePos))
            {
                this.SourceRect.X = this.SelectionX;
            }
            else
            {
                this.SourceRect.X = this.OriginalX;
            }
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
