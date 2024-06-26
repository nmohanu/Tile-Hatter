﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tile_mapper.src.UI;

namespace tile_mapper.src.Canvas
{
    internal static class AreaUtil
    {
        public static void UpdateAreaLabels()
        {
            // if (Global.SelectedArea != null)
            // {
            //     GlobalLabels.AreaName.Text = Global.SelectedArea.AreaName;
            //     GlobalLabels.AreaHeight.Text = "Width: " + Global.SelectedArea.AreaCords.Height.ToString();
            //     GlobalLabels.AreaWidth.Text = "Height: " + Global.SelectedArea.AreaCords.Width.ToString();
            //     GlobalLabels.AreaX.Text = "Left: " + Global.SelectedArea.AreaCords.X.ToString();
            //     GlobalLabels.AreaY.Text = "Top: " + Global.SelectedArea.AreaCords.Y.ToString();
            // }
            // else
            // {
            //     LabelUtil.ClearLabels(GlobalMenus.AreaProperties);
            // }
        }

        public static void AddArea()
        {
            bool allowed = true;
            foreach (var area in Global.CurrentMap.areas)
            {
                if (area.AreaCords.Intersects(Global.Selection))
                {
                    allowed = false;
                }
            }
            if (allowed)
            {
                string name = "Area: " + (Global.CurrentMap.areas.Count() + 1).ToString();
                Button btn = ScrollMenuUtil.CreateRemovableButton(ButtonAction.SelectArea, ButtonAction.RemoveArea, GlobalMenus.Properties);
                btn.Text = name;
                Global.CurrentMap.CreateArea(Global.Selection, name);

                btn.PressedSourceX = 288;
                btn.SourceRect.Y = 128;

                GlobalMenus.AreaMenu.buttons.Add(btn);

                ScrollMenuUtil.UpdateListOrder(GlobalMenus.AreaMenu);

                if (btn.ButtonRect.Bottom > GlobalMenus.AreaMenu.Destination.Bottom)
                {
                    foreach (var button in GlobalMenus.AreaMenu.buttons)
                    {
                        button.ButtonRect = new Rectangle(button.ButtonRect.X, button.ButtonRect.Y - 48, 224, 48);
                    }
                    GlobalMenus.AreaMenu.ScrollMenuOffset.Y -= 48;
                }
            }
        }
    }
}
