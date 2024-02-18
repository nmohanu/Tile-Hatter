using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tile_mapper.src.Canvas;

namespace tile_mapper.src
{
    internal class UserAction
    {
        public ActionType Action;
        public int x, y, Layer;
        public Rectangle Rect;
        Area Area;

        public UserAction(ActionType action, int layer, int x, int y)
        {
            Action = action;
            Layer = layer;
            this.x = x;
            this.y = y;

        }
        public UserAction(ActionType action, int layer, Rectangle rect, Area area)
        {
            Action = action;
            Layer = layer;
            Rect = rect;
            Area = area;
        }

        public enum ActionType
        {
            Draw,
            Remove,
            DrawMuliple,
            RemoveMultiple,
        }
    }
}
