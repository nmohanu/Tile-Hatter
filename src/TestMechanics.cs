using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tile_mapper.src.Canvas;

namespace tile_mapper.src
{
    internal static class TestMechanics
    {
        public static bool CheckCollision()
        {
            Area areaToSearch = null;

            int CharacterX = (int)((Global.CharacterRect.X - (Global.Offset.X + Global.Velocity.X)) / Global.TILE_SIZE / Global.TestingScale);
            int CharacterY = (int)((Global.CharacterRect.Y - (Global.Offset.Y + Global.Velocity.Y)) / Global.TILE_SIZE / Global.TestingScale);

            foreach (var area in Global.CurrentMap.areas)
            {
                if (area.AreaCords.Contains(CharacterX, CharacterY))
                {
                    areaToSearch = area;
                }
            }

            if (areaToSearch != null)
            {
                for (int k = 0; k < Global.CurrentMap.LayerAmount; k++)
                {
                    for (int i = Math.Max(CharacterY - 3, areaToSearch.AreaCords.Y); i < Math.Min(areaToSearch.AreaCords.Y + areaToSearch.AreaCords.Height, CharacterY + 3); i++)
                    {
                        for (int j = Math.Max(CharacterX - 3, areaToSearch.AreaCords.X); j < Math.Min(areaToSearch.AreaCords.X + areaToSearch.AreaCords.Width, CharacterX + 3); j++)
                        {
                            if (areaToSearch.Layers[k].TileMap[i - areaToSearch.AreaCords.Y, j - areaToSearch.AreaCords.X].ID != "0")
                            {
                                var collisionTile = Global.CurrentMap.CollisionTiles.FirstOrDefault(obj => obj.ID == areaToSearch.Layers[k].TileMap[i - areaToSearch.AreaCords.Y, j - areaToSearch.AreaCords.X].ID);
                                if (collisionTile?.Collision == true)
                                {
                                    Rectangle DestRect = new Rectangle((int)(j * Global.TILE_SIZE * Global.TestingScale + (Global.Offset.X + Global.Velocity.X)), (int)(i * Global.TILE_SIZE * Global.TestingScale + (Global.Offset.Y + Global.Velocity.Y)), (int)(Global.TILE_SIZE * Global.TestingScale + 1), (int)(Global.TILE_SIZE * Global.TestingScale + 1));
                                    if (DestRect.Intersects(Global.CharacterRect))
                                        return true;
                                }
                            }
                            if (Global.CurrentMap.Teleportations.Count > 0)
                            {
                                foreach (var tp in Global.CurrentMap.Teleportations)
                                {
                                    if (i == tp.A.Y && j == tp.A.X)
                                    {
                                        Rectangle TPRect = new Rectangle((int)(tp.A.X * Global.TILE_SIZE * Global.TestingScale + (Global.Offset.X + Global.Velocity.X)), (int)(tp.A.Y * Global.TILE_SIZE * Global.TestingScale + (Global.Offset.Y + Global.Velocity.Y)), (int)(Global.TILE_SIZE * Global.TestingScale + 1), (int)(Global.TILE_SIZE * Global.TestingScale + 1));
                                        if (TPRect.Intersects(Global.CharacterRect))
                                        {
                                            Global.Offset = new Vector2(Global.ScreenWidth / 2 - tp.B.X * Global.TILE_SIZE * Global.TestingScale, Global.ScreenHeight / 2 - tp.B.Y * Global.TILE_SIZE * Global.TestingScale);

                                            foreach (var area in Global.CurrentMap.areas)
                                            {
                                                if (area.AreaCords.Contains(tp.B.X, tp.B.Y))
                                                {
                                                    Global.CurrentArea = area;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }
    }
}
