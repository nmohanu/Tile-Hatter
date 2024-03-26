using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
            // Update map data.
            Global.CurrentMap.AddLayerToAreas();
            ReloadLayerButtons();
        }

        public static void ReloadLayerButtons()
        {
            if(Global.CurrentMap.areas.Count == 0)
                return; 
            
            for(int i = 0; i < GlobalMenus.LayerMenu.buttons.Count; i++)
            {
                if (GlobalMenus.LayerMenu.buttons[i].IsDeletable)
                {
                    GlobalMenus.LayerMenu.buttons.Remove(GlobalMenus.LayerMenu.buttons[i]);
                }
            }

            int counter = 1;
            foreach(SpriteLayer layer in Global.CurrentMap.areas[0].Layers)
            {
                Button btn = ScrollMenuUtil.CreateRemovableButton(ButtonAction.Layer, ButtonAction.RemoveLayer, GlobalMenus.Properties);
                btn.Text = "Layer: " + (counter).ToString();

                counter++;
                // Add button to the list.
                GlobalMenus.LayerMenu.buttons.Add(btn);

                // Update the list
                ScrollMenuUtil.UpdateListOrder(GlobalMenus.LayerMenu);
            }
        }
    }
}
