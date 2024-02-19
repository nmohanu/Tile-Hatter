using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tile_mapper.src.UI;

namespace tile_mapper.src.Canvas
{
    internal static class ObjectUtil
    {

        public static void AddObjectLayer()
        {
            Button btn = ScrollMenuUtil.CreateRemovableButton(ButtonAction.SelectObjectLayer, ButtonAction.RemoveObjectLayer, GlobalMenus.Properties);
            btn.Text = "ObjectLayer " + (Global.CurrentMap.ObjectLayers.Count() + 1).ToString();
            GlobalMenus.ObjectLayerMenu.buttons.Add(btn);
            ScrollMenuUtil.UpdateListOrder(GlobalMenus.ObjectLayerMenu);
            Global.CurrentMap.CreateObjectLayer();
        }
        public static void AddObject()
        {
            if (GlobalButtons.SelectedObjectLayerButton != null)
            {

                Object NewObject = new Object();
                NewObject.ID = "Object: " + (Global.CurrentMap.ObjectLayers[GlobalButtons.SelectedObjectLayerButton.HelperInt].objects.Count() + 1).ToString();

                Global.CurrentMap.ObjectLayers[GlobalButtons.SelectedObjectLayerButton.HelperInt].AddObject(NewObject);

                ReloadObjects();
            }
        }
        public static void ReloadObjects()
        {
            if (GlobalButtons.SelectedObjectLayerButton != null)
            {
                GlobalMenus.ObjectMenu.buttons.Clear();
                GlobalMenus.ObjectMenu.buttons.Add(GlobalButtons.CreateObjectButton);
                foreach (var Object in Global.CurrentMap.ObjectLayers[GlobalButtons.SelectedObjectLayerButton.HelperInt].objects)
                {
                    Button btn = ScrollMenuUtil.CreateRemovableButton(ButtonAction.SelectObject, ButtonAction.RemoveObject, GlobalMenus.Properties);
                    btn.Text = Object.ID.ToString();
                    GlobalMenus.ObjectMenu.buttons.Add(btn);
                }
                ScrollMenuUtil.UpdateListOrder(GlobalMenus.ObjectMenu);
            }
        }
    }
}
