using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using tile_mapper.src.Canvas;
using tile_mapper.src.UI;
using tile_mapper.src.UserSprites;
using static System.Formats.Asn1.AsnWriter;

namespace tile_mapper.src
{
    internal static class Renderer
    {
        public static void RenderGrid(SpriteBatch spriteBatch, int TILE_SIZE, Texture2D TileSheet, Texture2D Grid, float Scale, Vector2 Offset, SpriteTile selected, int SelectedX, int SelectedY, int ScreenWidth, int ScreenHeight, Rectangle Selection, WorldCanvas CurrentMap, ProgramLoop.CursorState CursorActionState)
        {

            Vector2 Difference = new Vector2(-Offset.X, -Offset.Y);

            int StartX = (int)(Difference.X / TILE_SIZE / Scale);
            int EndX = (int)((Difference.X + ScreenWidth) / TILE_SIZE / Scale);
            int StartY = (int)(Difference.Y / TILE_SIZE / Scale);
            int EndY = (int)((Difference.Y + ScreenHeight) / TILE_SIZE / Scale);
            // Prevent weird edges since it's rounded down.
            StartX--;
            StartY--;
            EndX++;
            EndY++;

            Color color = Color.Gray * 0.2f;

            for (int i = StartX; i < EndX; i++)
            {
                for (int j = StartY; j < EndY; j++)
                {
                    // Calculate what texture to draw.
                    Rectangle SourceRect = new Rectangle(0, 0, TILE_SIZE, TILE_SIZE);
                    if (i + 1 == 0 && j + 1 == 0)
                        SourceRect.X = 192; // Middle of the grid
                    else if (i + 1 == 0)
                        SourceRect.X = 160; // Middle row
                    else if (j + 1 == 0)
                        SourceRect.X = 128; // Middle column
                    else if ((i + 1) % 4 == 0 && (j + 1) % 4 == 0)
                        SourceRect.X = 96; // Every 4th cell
                    else if ((i + 1) % 4 == 0)
                        SourceRect.X = 64; // Every 4th cell in a column
                    else if ((j + 1) % 4 == 0)
                        SourceRect.X = 32; // Every 4th cell in a row

                    if (i + 1 == 0 && (j + 1) % 4 == 0 && j + 1 != 0)
                        SourceRect.X = 256; // Middle row, every 4th cell
                    else if (j + 1 == 0 && (i + 1) % 4 == 0 && i + 1 != 0)
                        SourceRect.X = 224; // Middle column, every 4th cell

                    Rectangle DestRect = new Rectangle((int)(i * TILE_SIZE * Scale + Offset.X), (int)(j * TILE_SIZE * Scale + Offset.Y), (int)(TILE_SIZE * Scale + 1), (int)(TILE_SIZE * Scale + 1));
                    spriteBatch.Draw(Grid, DestRect, SourceRect, Color.White);

                    if (j == SelectedY && i == SelectedX)
                    {
                        if (selected != null && CursorActionState == ProgramLoop.CursorState.Draw)
                            spriteBatch.Draw(TileSheet, DestRect, selected.Source, Color.White);
                        else
                            spriteBatch.Draw(Grid, DestRect, new Rectangle(288, 0, 16, 16), Color.White * 0.3f);
                    }


                    if (Selection.Contains(new Point(i, j)))
                    {
                        spriteBatch.Draw(Grid, DestRect, new Rectangle(288, 0, 16, 16), Color.White * 0.5f);
                    }

                    foreach (var area in CurrentMap.areas)
                    {
                        if (area.AreaCords.Contains(i, j))
                        {
                            spriteBatch.Draw(Grid, DestRect, new Rectangle(288, 0, 16, 16), color);
                        }
                    }
                }
            }
        }
        public static void RenderMap(WorldCanvas CurrentMap, int CurrentLayer, SpriteBatch spriteBatch, Texture2D TileSheet, int TILE_SIZE, float Scale, Vector2 Offset, int ScreenWidth, int ScreenHeight, Texture2D Grid)
        {
            Vector2 Difference = new Vector2(-Offset.X, -Offset.Y);

            int StartX = (int)(Difference.X / TILE_SIZE / Scale);
            int EndX = (int)((Difference.X + ScreenWidth) / TILE_SIZE / Scale);
            int StartY = (int)(Difference.Y / TILE_SIZE / Scale);
            int EndY = (int)((Difference.Y + ScreenHeight) / TILE_SIZE / Scale);

            EndX++;
            EndY++;



            if (CurrentMap.areas.Count() > 0)
            {
                foreach (var area in CurrentMap.areas)
                {
                    for (int i = Math.Max(area.AreaCords.Y, StartY); i < Math.Min(area.AreaCords.Y + area.AreaCords.Height, EndY); i++)
                    {
                        for (int j = Math.Max(area.AreaCords.X, StartX); j < Math.Min(area.AreaCords.X + area.AreaCords.Width, EndX); j++)
                        {
                            for (int k = 0; k <= CurrentMap.LayerAmount - 1; k++)
                            {
                                Rectangle DestRect = new Rectangle((int)(j * TILE_SIZE * Scale + Offset.X), (int)(i * TILE_SIZE * Scale + Offset.Y), (int)(TILE_SIZE * Scale + 1), (int)(TILE_SIZE * Scale + 1));

                                if (area.Layers[k].TileMap[i - area.AreaCords.Y, j - area.AreaCords.X].ID != "0")
                                {

                                    if (k == CurrentLayer)
                                    {
                                        spriteBatch.Draw(TileSheet, DestRect, area.Layers[k].TileMap[i - area.AreaCords.Y, j - area.AreaCords.X].Source, Color.White);
                                    }
                                    else
                                    {
                                        spriteBatch.Draw(TileSheet, DestRect, area.Layers[k].TileMap[i - area.AreaCords.Y, j - area.AreaCords.X].Source, Color.White * 0.5f);
                                    }
                                }
                                else
                                {
                                    //spriteBatch.Draw(Grid, DestRect, new Rectangle(288, 0, 16, 16), color);
                                }
                            }
                        }
                    }
                }
            }


        }

        public static void DrawArea(Area area, Vector2 Offset, int TILE_SIZE, float Scale, int ScreenWidth, int ScreenHeight, WorldCanvas CurrentMap, SpriteBatch spriteBatch, Texture2D TileSheet)
        {
            Vector2 Difference = new Vector2(-Offset.X, -Offset.Y);

            int StartX = (int)(Difference.X / TILE_SIZE / Scale);
            int EndX = (int)((Difference.X + ScreenWidth) / TILE_SIZE / Scale);
            int StartY = (int)(Difference.Y / TILE_SIZE / Scale);
            int EndY = (int)((Difference.Y + ScreenHeight) / TILE_SIZE / Scale);

            EndX++;
            EndY++;

            if (CurrentMap.areas.Count() > 0)
                for (int i = Math.Max(area.AreaCords.Y, StartY); i < Math.Min(area.AreaCords.Y + area.AreaCords.Height, EndY); i++)
                {
                    for (int j = Math.Max(area.AreaCords.X, StartX); j < Math.Min(area.AreaCords.X + area.AreaCords.Width, EndX); j++)
                    {
                        for (int k = 0; k <= CurrentMap.LayerAmount - 1; k++)
                        {
                            Rectangle DestRect = new Rectangle((int)(j * TILE_SIZE * Scale + Offset.X), (int)(i * TILE_SIZE * Scale + Offset.Y), (int)(TILE_SIZE * Scale + 1), (int)(TILE_SIZE * Scale + 1));

                            if (area.Layers[k].TileMap[i - area.AreaCords.Y, j - area.AreaCords.X].ID != "0")
                            {
                                spriteBatch.Draw(TileSheet, DestRect, area.Layers[k].TileMap[i - area.AreaCords.Y, j - area.AreaCords.X].Source, Color.White);
                            }
                        }
                    }
                }
        }

        public static void DrawPalette(bool HasTileSheet, List<List<SpriteTile>> TileSpriteList, SpriteBatch spriteBatch, SpriteTile selected, Texture2D UI, Texture2D TileSheet, UI_Menu TileMenu, Vector2 ScrollOffset, Rectangle PaletteDestination)
        {
            Vector2 Difference = new Vector2(-ScrollOffset.X, -ScrollOffset.Y);

            int StartX = (int)(Difference.X / 64 / 1f);
            int EndX = (int)((Difference.X + 256) / 64 / 1f);
            int StartY = (int)(Difference.Y / 64 / 1f);
            int EndY = (int)((Difference.Y + 480) / 64 / 1f);

            StartX--;
            StartY--;
            EndX++;
            EndY++;

            Rectangle Dest = new Rectangle((int)(PaletteDestination.X + ScrollOffset.X), (int)(PaletteDestination.Y + ScrollOffset.Y), PaletteDestination.Width, PaletteDestination.Height);

            for (int y = StartY; y < EndY; y++)
            {
                for (int x = StartX; x < EndX; x++)
                {
                    Rectangle AdjDest = new Rectangle(Dest.X + x * 64, Dest.Y + y * 64, 64, 64);
                    spriteBatch.Draw(UI, AdjDest, new Rectangle(0, 912, 64, 64), Color.White);
                }
            }

            if (HasTileSheet)
            {
                foreach (var list in TileSpriteList)
                {
                    foreach (var rectangle in list)
                    {
                        spriteBatch.Draw(TileSheet, new Vector2(rectangle.Destination.X + ScrollOffset.X, rectangle.Destination.Y + ScrollOffset.Y), rectangle.Source, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0);
                        if (rectangle.hovers)
                            spriteBatch.Draw(UI, new Rectangle((int)(rectangle.Destination.X + ScrollOffset.X), (int)(rectangle.Destination.Y + ScrollOffset.Y), rectangle.Destination.Width, rectangle.Destination.Height), new Rectangle(0, 896, 16, 16), Color.White);
                        if (selected != null && rectangle.ID == selected.ID)
                        {
                            spriteBatch.Draw(UI, new Rectangle((int)(rectangle.Destination.X + ScrollOffset.X), (int)(rectangle.Destination.Y + ScrollOffset.Y), rectangle.Destination.Width, rectangle.Destination.Height), new Rectangle(16, 896, 16, 16), Color.White);
                        }
                    }
                }
            }

            spriteBatch.Draw(UI, TileMenu.Destination, TileMenu.Source, Color.White);
        }
    }
}
