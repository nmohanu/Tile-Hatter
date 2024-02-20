using System;
using System.Collections;
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

        public static void AddLayerProperty()
        {
            int amount = 0;
            bool IsNull = true;
            Button btn = ScrollMenuUtil.CreateRemovableButton(ButtonAction.SelectProperty, ButtonAction.RemoveProperty, GlobalMenus.LayerProperties);

            foreach (var area in Global.CurrentMap.areas)
            {
                List < Property > list = area.Layers[Global.CurrentLayer].Properties;
                amount = list.Count() + 1;
                Property property = new Property();
                property.ID = "Property " + (list.Count() + 1).ToString();
                btn.Property = property;
                btn.IsVisible = true;
                list.Add(property);
                IsNull = false;
            }
            
            if(!IsNull)
            {
                btn.Text = "Property " + (amount).ToString();
                GlobalMenus.LayerProperties.buttons.Add(btn);
            }
            ScrollMenuUtil.UpdateListOrder(GlobalMenus.LayerProperties);
        }

        public static void AddAreaProperty()
        {
            if(Global.CurrentMap.areas != null && GlobalButtons.ClickedAreaButton != null && Global.CurrentMap.areas[GlobalButtons.ClickedAreaButton.HelperInt] != null)
            {
                List<Property> list = Global.CurrentMap.areas[GlobalButtons.ClickedAreaButton.HelperInt].Properties;
                Property property = new Property();
                property.ID = "Property " + (list.Count() + 1).ToString();
                list.Add(property);

                Button btn = ScrollMenuUtil.CreateRemovableButton(ButtonAction.SelectProperty, ButtonAction.RemoveProperty, GlobalMenus.LayerProperties);
                btn.Text = property.ID;
                btn.Property = property;
                GlobalMenus.AreaProperties.buttons.Add(btn);
                ScrollMenuUtil.UpdateListOrder(GlobalMenus.AreaProperties);
            }
        }

        public static void ReloadLayerProperties()
        {
            GlobalMenus.LayerProperties.buttons.Clear();
            GlobalButtons.CreateLayerPropertyButton.IsVisible = true;
            GlobalMenus.LayerProperties.buttons.Add(GlobalButtons.CreateLayerPropertyButton);

            if (Global.CurrentMap.areas.Count > 0 && Global.CurrentMap.areas[0].Layers[Global.CurrentLayer].Properties.Count() > 0)
            {
                
                foreach (var Property in Global.CurrentMap.areas[0].Layers[Global.CurrentLayer].Properties)
                {
                    if(Property != null)
                    {
                        Button btn = ScrollMenuUtil.CreateRemovableButton(ButtonAction.SelectProperty, ButtonAction.RemoveProperty, GlobalMenus.Properties);
                        btn.Text = Property.ID.ToString();
                        btn.Property = Property;
                        GlobalMenus.LayerProperties.buttons.Add(btn);
                    }
                }
            }
            ScrollMenuUtil.UpdateListOrder(GlobalMenus.LayerProperties);
        }

        public static void ReloadAreaProperties()
        {
            GlobalMenus.AreaProperties.buttons.Clear();
            GlobalMenus.AreaProperties.buttons.Add(GlobalButtons.CreateAreaPropertyButton);

            if (Global.CurrentMap.areas[GlobalButtons.ClickedAreaButton.HelperInt].Properties.Count() > 0)
            {
                foreach (var Property in Global.CurrentMap.areas[GlobalButtons.ClickedAreaButton.HelperInt].Properties)
                {
                    if (Property != null)
                    {
                        Button btn = ScrollMenuUtil.CreateRemovableButton(ButtonAction.SelectProperty, ButtonAction.RemoveProperty, GlobalMenus.Properties);
                        btn.Text = Property.ID.ToString();
                        btn.Property = Property;

                        GlobalMenus.AreaProperties.buttons.Add(btn);
                    }
                }
            }
            ScrollMenuUtil.UpdateListOrder(GlobalMenus.AreaProperties);
        }
    }
}
