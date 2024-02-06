using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        public static void RenderGrid(SpriteBatch spriteBatch, int MAP_HEIGHT, int MAP_WIDTH, int TILE_SIZE, Texture2D TileSheet, Texture2D Grid, float Scale, Vector2 Offset, SpriteTile selected, int SelectedX, int SelectedY)
        {
            for (int i = 0; i < MAP_HEIGHT; i++)
            {
                for (int j = 0; j < MAP_WIDTH; j++)
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
                }
            }
        }
        public static void RenderMap(int MAP_HEIGHT, int MAP_WIDTH, Map CurrentMap, int CurrentLayer, SpriteBatch spriteBatch, Texture2D TileSheet, int TILE_SIZE, float Scale, Vector2 Offset)
        {
            for (int i = 0; i < MAP_HEIGHT - 1; i++)
            {
                for (int j = 0; j < MAP_WIDTH - 1; j++)
                {
                    for (int k = 0; k < 2; k++)
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
}
