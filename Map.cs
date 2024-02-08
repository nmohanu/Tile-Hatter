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

        public Button HandleClicks(Vector2 MousePos)
        {
            foreach (var button in buttons)
            {
                if (button != null && button.IsVisible && button.ButtonRect.Contains(MousePos))
                {
                    return button;
                }
            }
            return null;
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
        OpenPalette,
        DrawTool,
        FillTool,
        EraserTool,
        SelectArea
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

    internal class Area
    {
        public Rectangle AreaCords;
        public string AreaName;
        public Layer[] layers = new Layer[3];
        Layer Terrain;
        Layer Objects;
        Layer Foreground;

        public Area(Rectangle areaCords, string areaName)
        {
            this.AreaCords = areaCords;
            this.AreaName = areaName;
            Layer Terrain = new Layer(0, areaCords);
            Layer Objects = new Layer(1, areaCords);
            Layer Foreground = new Layer(2, areaCords);

            layers[0] = Terrain;
            layers[1] = Objects;
            layers[2] = Foreground;

            foreach (var layer in layers)
            {
                for(int i = 0; i < this.AreaCords.Height; i++)
                {
                    for(int j = 0; j < this.AreaCords.Width; j++)
                    {
                        layer.TileMap[i, j] = new Tile();
                 
                    }
                }
            }
        }
    }

    internal class Canvas
    {
        public int LayerAmount = 2;

        public List<Area> areas = new List<Area>();

        public Canvas()
        {
            
        }

        public void CreateArea(Rectangle Selection, string AreaName)
        {
            areas.Add(new Area(Selection, AreaName));
        }
    }

    internal class Layer
    {
        public int Depth;

        public Tile[,] TileMap;


        public Layer(int depth, Rectangle rectangle)
        {
            this.Depth = depth;

            TileMap = new Tile[rectangle.Height, rectangle.Width];
        }
    }


}
