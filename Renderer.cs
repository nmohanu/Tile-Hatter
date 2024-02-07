using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace tile_mapper
{
    internal static class Renderer
    {
        public static void RenderGrid(SpriteBatch spriteBatch, int MAP_HEIGHT, int MAP_WIDTH, int TILE_SIZE, Texture2D TileSheet, Texture2D Grid, float Scale, Vector2 Offset, SpriteTile selected, int SelectedX, int SelectedY, int ScreenWidth, int ScreenHeight, Rectangle Selection)
        {

            Vector2 Difference = new Vector2(- Offset.X, - Offset.Y);

            int StartX = (int) (Difference.X / TILE_SIZE / Scale);
            int EndX = (int) ((Difference.X  + ScreenWidth) / TILE_SIZE / Scale);
            int StartY = (int) (Difference.Y / TILE_SIZE / Scale);
            int EndY = (int) ((Difference.Y + ScreenHeight) / TILE_SIZE / Scale);

            EndX++;
            EndY++;

            StartX = Math.Max(0, StartX);
            EndX = Math.Min(EndX, MAP_WIDTH);
            StartY = Math.Max(0, StartY);
            EndY = Math.Min(EndY, MAP_HEIGHT);

            // Prevent weird edges since it's rounded down.
            

            for (int i = StartX; i < EndX; i++)
            {
                for (int j = StartY; j < EndY; j++)
                {
                   // Calculate what texture to draw.
                    Rectangle SourceRect = new Rectangle(0, 0, TILE_SIZE, TILE_SIZE);
                    if (i + 1 == MAP_HEIGHT / 2 && j + 1 == MAP_WIDTH / 2)
                        SourceRect.X = 192; // Middle of the grid
                    else if (i + 1 == MAP_HEIGHT / 2)
                        SourceRect.X = 160; // Middle row
                    else if (j + 1 == MAP_WIDTH / 2)
                        SourceRect.X = 128; // Middle column
                    else if ((i + 1) % 4 == 0 && (j + 1) % 4 == 0)
                        SourceRect.X = 96; // Every 4th cell
                    else if ((i + 1) % 4 == 0)
                        SourceRect.X = 64; // Every 4th cell in a column
                    else if ((j + 1) % 4 == 0)
                        SourceRect.X = 32; // Every 4th cell in a row

                    if (i + 1 == MAP_HEIGHT / 2 && (j + 1) % 4 == 0 && j + 1 != MAP_WIDTH / 2)
                        SourceRect.X = 256; // Middle row, every 4th cell
                    else if (j + 1 == MAP_WIDTH / 2 && (i + 1) % 4 == 0 && i + 1 != MAP_HEIGHT / 2)
                        SourceRect.X = 224; // Middle column, every 4th cell

                    Rectangle DestRect = new Rectangle((int)(i * TILE_SIZE * Scale + Offset.X), (int)(j * TILE_SIZE * Scale + Offset.Y), (int)(TILE_SIZE * Scale + 1), (int)(TILE_SIZE * Scale + 1));
                    spriteBatch.Draw(Grid, DestRect, SourceRect, Color.White);

                    if (j == SelectedY && i == SelectedX)
                    {
                        if (selected != null)
                            spriteBatch.Draw(TileSheet, DestRect, selected.Source, Color.White);
                        else
                            spriteBatch.Draw(Grid, DestRect, new Rectangle(288, 0, 16, 16), Color.White);
                    }

                    
                    if(Selection.Contains(new Point(i, j)))
                    {
                        spriteBatch.Draw(Grid, DestRect, new Rectangle(288, 0, 16, 16), Color.White);
                    }
                }
            }
        }
        public static void RenderMap(int MAP_HEIGHT, int MAP_WIDTH, Canvas CurrentMap, int CurrentLayer, SpriteBatch spriteBatch, Texture2D TileSheet, int TILE_SIZE, float Scale, Vector2 Offset)
        {
            if(CurrentMap.areas.Count() > 0) // TODO: Only render layers if they are within an area. AREA CREATION ASAP.
            {
                for (int i = 0; i < MAP_HEIGHT - 1; i++)
                {
                    for (int j = 0; j < MAP_WIDTH - 1; j++)
                    {
                        for (int k = 0; k <= 2; k++)
                        {
                            if (CurrentMap != null &&
                            CurrentMap.layers[k] != null &&
                            CurrentMap.layers[k].TileMap != null &&
                            CurrentMap.layers[k].TileMap[j, i] != null &&
                            CurrentMap.layers[k].TileMap[j, i].ID != "0")
                            {
                                if (k == CurrentLayer)
                                {
                                    Rectangle DestRect = new Rectangle((int)(i * TILE_SIZE * Scale + Offset.X), (int)(j * TILE_SIZE * Scale + Offset.Y), (int)(TILE_SIZE * Scale + 1), (int)(TILE_SIZE * Scale + 1));

                                    spriteBatch.Draw(TileSheet, DestRect, CurrentMap.layers[k].TileMap[j, i].Source, Color.White);
                                }
                                else
                                {
                                    Rectangle DestRect = new Rectangle((int)(i * TILE_SIZE * Scale + Offset.X), (int)(j * TILE_SIZE * Scale + Offset.Y), (int)(TILE_SIZE * Scale + 1), (int)(TILE_SIZE * Scale + 1));

                                    spriteBatch.Draw(TileSheet, DestRect, CurrentMap.layers[k].TileMap[j, i].Source, Color.White * 0.5f);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void DrawButtons(List<Button> buttons, SpriteBatch spriteBatch, SpriteFont font, float TextScale, Texture2D UI)
        {
            foreach (var button in buttons)
            {
                if (button.IsVisible)
                {
                    spriteBatch.Draw(UI, new Vector2(button.ButtonRect.X, button.ButtonRect.Y), button.SourceRect, Color.White);
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

        public static void DrawPalette(bool HasTileSheet, List<List<SpriteTile>> TileSpriteList, SpriteBatch spriteBatch, SpriteTile selected, Texture2D Grid, Texture2D TileSheet)
        {
            if (HasTileSheet)
            {
                foreach (var list in TileSpriteList)
                {
                    foreach (var rectangle in list)
                    {
                        spriteBatch.Draw(TileSheet, new Vector2(rectangle.Destination.X + 32, rectangle.Destination.Y + 32), rectangle.Source, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0);
                        if (rectangle.hovers)
                            spriteBatch.Draw(Grid, rectangle.Destination, new Rectangle(320, 0, 16, 16), Color.White);
                        if (selected != null && rectangle.ID == selected.ID)
                        {
                            spriteBatch.Draw(Grid, rectangle.Destination, new Rectangle(336, 0, 16, 16), Color.White);
                        }
                    }
                }
            }
        }
    }
}
