using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tile_mapper.src.UI;
using tile_mapper.src.UserSprites;

namespace tile_mapper.src
{
    internal static class FileUtil
    {
        public static void OpenSpriteSheetFile(string path, GraphicsDevice graphicsDevice)
        {
            try
            {
                using (FileStream stream = new FileStream(Global.TileSheetPath, FileMode.Open)) // Import file.
                {
                    Global.TileSheet = Texture2D.FromStream(graphicsDevice, stream);
                    Global.LastWriteTime = GetFileWriteTime();
                }
            }
            catch
            {
            }
            

            Global.SheetWidth = Global.TileSheet.Width / Global.TILE_SIZE; // Sheet width.
            Global.SheetHeight = Global.TileSheet.Height / Global.TILE_SIZE; // Sheet height.

            List<SpriteTile> page = new List<SpriteTile>(); // For multiple sprite sheets.

            for (int y = 0; y < Global.SheetHeight; y++)
            {
                for (int x = 0; x < Global.SheetWidth; x++)
                {
                    int xcord = x * Global.TILE_SIZE;
                    int ycord = y * Global.TILE_SIZE;

                    int xdest = GlobalMenus.TileMenu.Destination.X + 16 + x * Global.TILE_SIZE * 2;
                    int ydest = GlobalMenus.TileMenu.Destination.Y + 16 + y * Global.TILE_SIZE * 2;

                    SpriteTile tile = new SpriteTile();
                    tile.Source = new Rectangle(xcord, ycord, Global.TILE_SIZE, Global.TILE_SIZE);

                    tile.Destination = new Rectangle();

                    tile.ID = "X" + x.ToString() + "Y" + y.ToString(); // Set sprite unique ID.
                    tile.Destination = new Rectangle(xdest, ydest, Global.TILE_SIZE * 2, Global.TILE_SIZE * 2);

                    page.Add(tile);

                }
            }

            Global.TileSpriteList.Add(page);

            Global.HasTileSheet = true;

            System.Diagnostics.Debug.WriteLine(Global.TileSheetPath);

        }

        internal static DateTime GetFileWriteTime()
        {
            DateTime lastWriteTime = File.GetLastWriteTimeUtc(Global.TileSheetPath);
            return lastWriteTime;
        }
    }
}
