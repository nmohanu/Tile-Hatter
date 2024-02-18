using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static tile_mapper.src.ProgramLoop;
using tile_mapper.src.UI;

namespace tile_mapper.src.UserSprites
{
    internal static class SpriteUtil
    {
        public static void SelectTile(SpriteTile rect)
        {
            Global.selected = rect;
            Global.selected.ID = rect.ID;
            Global.selected.Source = rect.Source;
            Global.CursorActionState = CursorState.Draw;
            GlobalLabels.CurrentTileID.IsVisible = true;
            GlobalLabels.CurrentTileID.Text = "ID: " + Global.selected.ID;
            GlobalLabels.Collision.IsVisible = true;
            GlobalButtons.CollisionCheckBox.IsVisible = true;
            GlobalLabels.Collision.Text = "Collision";
            GlobalButtons.CollisionCheckBox.IsPressed = Global.selected.Collision;
        }
    }
}
