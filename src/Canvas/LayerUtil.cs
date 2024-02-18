using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tile_mapper.src.UI;

namespace tile_mapper.src.Layer
{
    internal static class LayerUtil
    {
        public static void AddLayer()
        {
            Button btn = ScrollMenuUtil.CreateRemovableButton(ButtonAction.Layer, ButtonAction.RemoveLayer, GlobalMenus.Properties);
            btn.Text = "Layer: " + (Global.CurrentMap.LayerAmount + 1).ToString();

            // Add button to the list.
            GlobalMenus.LayerMenu.buttons.Add(btn);

            // Update the list
            ScrollMenuUtil.UpdateListOrder(GlobalMenus.LayerMenu);

            // Update map data.
            Global.CurrentMap.AddLayerToAreas();
        }
    }
}
