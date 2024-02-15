using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace tile_mapper
{

    internal class Label
    {
        public string Text;
        public int ID;
        public Rectangle LabelRect;
        public Rectangle SourceRect = new Rectangle(320, 112, 0, 0); // Standard button
        public bool IsVisible;

    }
    internal class UI_Menu
    {
        public List<Label> labels;
        public bool IsVisible;
        public Rectangle Source;
        public Rectangle Destination;
        public List<Button> buttons;
        public bool Scrollable;

        public UI_Menu(bool IsVisible, Rectangle Source, Rectangle Destination)
        {
            labels = new List<Label>();
            buttons = new List<Button>();
            this.IsVisible = IsVisible;
            this.Source = Source;
            this.Destination = Destination;
        }


        public void Draw(SpriteBatch spriteBatch, Texture2D UI, int ScreenHeight, int ScreenWidth, float ScaleX, float ScaleY, SpriteFont font, float TextScale, bool RenderScrollableMenus, Vector2 ScrollMenuOffset)
        {
            if(IsVisible && (!Scrollable || RenderScrollableMenus))
            {
                spriteBatch.Draw(UI, new Vector2(Destination.X, Destination.Y), Source, Color.White, 0f, Vector2.Zero, new Vector2(ScaleX, ScaleY), SpriteEffects.None, 0);
                foreach (var button in buttons)
                {
                    if(button != null && button.IsVisible)
                    {
                        Rectangle source = button.SourceRect;
                        if (button.IsPressed)
                        {
                            source.X = button.PressedSourceX;
                        }

                        Rectangle Destination = button.ButtonRect;

                        //if(Scrollable)
                        //{
                        //    Destination.Y += (int)ScrollMenuOffset.Y;
                        //}   

                        spriteBatch.Draw(UI, Destination, source, Color.White);
                        spriteBatch.DrawString(
                        font,
                        button.Text,
                        new Vector2(
                                Destination.X + Destination.Width / 2 - font.MeasureString(button.Text).X * TextScale / 2,
                                Destination.Y + Destination.Height / 2 - font.MeasureString(button.Text).Y * TextScale / 2
                            ),
                            button.color,
                            0f, // Rotation angle, set to 0 for no rotation
                            Vector2.Zero, // Origin, set to Vector2.Zero for the default origin
                            TextScale, // Scale factor
                            SpriteEffects.None, // Sprite effects, set to None for no effects
                            0f // Depth, set to 0 for the default depth

                            
                        );
                    }
                }
                foreach (var label in labels)
                {
                    if(label.IsVisible && label.Text != null)
                    {
                        spriteBatch.Draw(UI, label.LabelRect, label.SourceRect, Color.White);
                        spriteBatch.DrawString(
                        font,
                        label.Text,
                        new Vector2(
                                label.LabelRect.X + label.LabelRect.Width / 2 - font.MeasureString(label.Text).X * TextScale / 2,
                                label.LabelRect.Y + label.LabelRect.Height / 2 - font.MeasureString(label.Text).Y * TextScale / 2
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
        public Rectangle Rect;
        Area Area;
       
        public UserAction(ActionType action, int layer, int x, int y) 
        {
            this.Action = action;
            this.Layer = layer;
            this.x = x;
            this.y = y;

        }
        public UserAction(ActionType action, int layer, Rectangle rect, Area area)
        {
            this.Action = action;
            this.Layer = layer;
            this.Rect = rect;
            this.Area = area;
        }

        public enum ActionType
        {
            Draw,
            Remove,
            DrawMuliple,
            RemoveMultiple,
        }
    }
    internal class SpriteTile
    {
        public Rectangle Destination;
        public Rectangle Source;
        public bool Collision;

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
        SelectArea,
        SpecifyStartPoint,
        ClosePalette,
        SpecifyDoor,
        EditState,
        TestState,
        MakeCollision,
        RemoveLayer,
        AddLayer,
        RemoveArea,
        AddArea,
        MoveLeftArea,
        MoveRightArea,
        MoveLeftLayer,
        MoveRightLayer,
        OpenObjectMenu,
        OpenLayerMenu,
        OpenAreaMenu,
        OpenSpriteMenu,
        EditorScreen,
        SheetScreen
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
        public int HelperInt;
        public bool IsPressed = false;
        public int PressedSourceX;
        public Color color = Color.White;
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
        public List<Layer> layers;

        public Area(Rectangle areaCords, string areaName, int LayerAmount)
        {
            this.AreaCords = areaCords;
            this.AreaName = areaName;

            layers = new List<Layer>();


            for(int k = 0; k < LayerAmount; k++)
            {
                AddLayer();
            }
        }

        public void AddLayer()
        {
            Layer layer = new Layer(this.AreaCords);
            for (int i = 0; i < this.AreaCords.Height; i++)
            {
                for (int j = 0; j < this.AreaCords.Width; j++)
                {
                    layer.TileMap[i, j] = new Tile();
                }
            }
            layers.Add(layer);
        }
    }

    internal class Canvas
    {
        public int LayerAmount = 0;

        public List<Area> areas = new List<Area>();

        public List<Teleportation> Teleportations = new List<Teleportation>();

        public List<SpriteTile> CollisionTiles = new List<SpriteTile>();

        public Point StartLocation;
        public bool StartLocationSpecified;

        public Canvas()
        {
            
        }

        public void AddLayerToAreas()
        {
            foreach (var area in areas)
            {
                area.AddLayer();
            }
        }

        public void CreateArea(Rectangle Selection, string AreaName)
        {
            areas.Add(new Area(Selection, AreaName, this.LayerAmount));
        }
    }

    internal class Teleportation
    {
        public Point A;
        public Point B;
    }

    internal class Layer
    {
        public Tile[,] TileMap;
        public Layer(Rectangle rectangle)
        {
            TileMap = new Tile[rectangle.Height, rectangle.Width];
        }
    }


}
