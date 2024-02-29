using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tile_mapper.src.Canvas;

namespace tile_mapper.src
{
    internal static class ToolUtil
    {
        // Tools used
        public static void FillSelection()
        {
            if (Global.selected == null)
            {
                return;
            }

            foreach (var area in Global.CurrentMap.areas)
            {
                for (int i = Global.Selection.Y; i < Global.Selection.Y + Global.Selection.Height; i++)
                {
                    for (int j = Global.Selection.X; j < Global.Selection.X + Global.Selection.Width; j++)
                    {
                        if (area.AreaCords.Contains(j, i) && area.Layers.Count() > 0)
                        {
                            area.Layers[Global.CurrentLayer].TileMap[i - area.AreaCords.Y, j - area.AreaCords.X].ID = Global.selected.ID;
                            area.Layers[Global.CurrentLayer].TileMap[i - area.AreaCords.Y, j - area.AreaCords.X].Source = Global.selected.Source;
                        }
                    }
                }
            }
        }
        public static void FillClicked()
        {
            
            Area areaClicked = null;
            foreach (var area in Global.CurrentMap.areas)
            {
                if (area.AreaCords.Contains(Global.SelectedX, Global.SelectedY))
                {
                    areaClicked = area;
                    break;
                }
            }
            if (areaClicked == null)
            {
                return;
            }

            if (areaClicked.Layers.Count() == 0)
                return;

            string IDToFill = areaClicked.Layers[Global.CurrentLayer].TileMap[Global.SelectedY - areaClicked.AreaCords.Y, Global.SelectedX - areaClicked.AreaCords.X].ID;

            HashSet<(int, int)> visited = new HashSet<(int, int)>();
            Queue<(int, int)> queue = new Queue<(int, int)>();
            queue.Enqueue((Global.SelectedX, Global.SelectedY));

            while (queue.Count > 0)
            {
                var (x, y) = queue.Dequeue();

                if (!areaClicked.AreaCords.Contains(x, y) || areaClicked.Layers[Global.CurrentLayer].TileMap[y - areaClicked.AreaCords.Y, x - areaClicked.AreaCords.X].ID != IDToFill || visited.Contains((x, y)))
                {
                    continue;
                }

                visited.Add((x, y));

                // Fill the current tile
                areaClicked.Layers[Global.CurrentLayer].TileMap[y - areaClicked.AreaCords.Y, x - areaClicked.AreaCords.X].ID = Global.selected.ID;
                areaClicked.Layers[Global.CurrentLayer].TileMap[y - areaClicked.AreaCords.Y, x - areaClicked.AreaCords.X].Source = Global.selected.Source;

                // Enqueue neighboring tiles
                queue.Enqueue((x + 1, y));
                queue.Enqueue((x - 1, y));
                queue.Enqueue((x, y + 1));
                queue.Enqueue((x, y - 1));
            }
        }
        public static void SpecifyStartPoint()
        {
            foreach (var area in Global.CurrentMap.areas)
            {
                if (area.AreaCords.Contains(Global.SelectedX, Global.SelectedY))
                {
                    Global.CurrentMap.StartLocation = new Point(Global.SelectedX, Global.SelectedY);
                    Global.CurrentMap.StartLocationSpecified = true;
                    Global.CurrentArea = area;
                    Global.StartArea = Global.CurrentArea;
                }
            }
        }
        public static void SpecifyDoor() 
        {
            foreach (var area in Global.CurrentMap.areas)
            {
                if (area.AreaCords.Contains(Global.SelectedX, Global.SelectedY))
                {
                    if (Global.A.HasValue)
                    {
                        Teleportation tp = new Teleportation();
                        tp.A = Global.A.Value;
                        tp.B = new Point(Global.SelectedX, Global.SelectedY);
                        Global.CurrentMap.Teleportations.Add(tp);

                        Global.A = null;

                    }
                    else
                    {
                        Global.A = new Point(Global.SelectedX, Global.SelectedY);
                        Global.resetCursorState = false;
                    }
                }
            }
        }
    }
}
