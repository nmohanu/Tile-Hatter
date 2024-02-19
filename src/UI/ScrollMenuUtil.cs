using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static tile_mapper.src.ProgramLoop;

namespace tile_mapper.src.UI
{
    internal static class ScrollMenuUtil
    {
        internal static Button CreateRemovableButton(ButtonAction action, ButtonAction RemoveButtonAction, UI_Menu Properties)
        {
            // Create button.
            Button button = new Button("", new Rectangle(Properties.Destination.X + Properties.Destination.Width / 2 - 224 / 2, 0, 224, 48), 288, 64, action, true);
            button.SourceRect.Y = 128;
            button.PressedSourceX = 288;
            button.SourceRect.X = button.OriginalX;
            button.IsDeletable = true;

            // Add delete button (X) to the button.
            button.DeleteButton = new Button("", new Rectangle(button.ButtonRect.X + 224 - 24, 0, 16, 16), 144, 128, RemoveButtonAction, true);
            button.DeleteButton.SourceRect.Y = 96;
            button.DeleteButton.SourceRect.X = 128;

            return button;
        }

        internal static void UpdateListOrder(UI_Menu menu)
        {
            // Make sure the create button is placed last in list.
            for (int i = 0; i < menu.buttons.Count; i++)
            {
                var btn = menu.buttons[i];
                if (btn.Action == ButtonAction.AddLayer || btn.Action == ButtonAction.CreateObjectLayer || btn.Action == ButtonAction.CreateObject)
                {
                    menu.buttons.Remove(btn);
                    menu.buttons.Add(btn);
                    break;
                }
            }

            // Update button positions.
            int j = 0;
            foreach (var btn in menu.buttons)
            {

                btn.ButtonRect.Y = menu.Destination.Y + 16 + 48 * j + (int)menu.ScrollMenuOffset.Y;
                btn.HelperInt = j;

                if (btn.IsDeletable)
                {
                    btn.DeleteButton.ButtonRect.Y = btn.ButtonRect.Y + 16;
                    btn.DeleteButton.HelperInt = j;
                }
                j++;

            }
        }
        public static void UpdateMenuState()
        {
            foreach (var menu in Global.PropertyMenu)
            {
                menu.IsVisible = false;
            }
            foreach (var menu in Global.LabelMenus)
            {
                menu.IsVisible = false;
            }

            switch (Global.menuState)
            {
                case MenuState.LayerMenu:
                    GlobalMenus.LayerMenu.IsVisible = true;
                    GlobalMenus.LayerProperties.IsVisible = true;
                    GlobalButtons.LayerMenuButton.IsPressed = true;
                    GlobalButtons.AreaMenuButton.IsPressed = false;
                    GlobalButtons.ObjectMenuButton.IsPressed = false;
                    GlobalButtons.SpriteMenuButton.IsPressed = false;
                    break;
                case MenuState.AreaMenu:
                    GlobalMenus.AreaMenu.IsVisible = true;
                    GlobalMenus.AreaProperties.IsVisible = true;
                    GlobalButtons.LayerMenuButton.IsPressed = false;
                    GlobalButtons.AreaMenuButton.IsPressed = true;
                    GlobalButtons.ObjectMenuButton.IsPressed = false;
                    GlobalButtons.SpriteMenuButton.IsPressed = false;
                    break;
                case MenuState.SpriteTileMenu:
                    GlobalMenus.CollisionSpriteList.IsVisible = true;
                    GlobalMenus.TileLabels.IsVisible = true;
                    GlobalButtons.LayerMenuButton.IsPressed = false;
                    GlobalButtons.AreaMenuButton.IsPressed = false;
                    GlobalButtons.ObjectMenuButton.IsPressed = false;
                    GlobalButtons.SpriteMenuButton.IsPressed = true;
                    break;
                case MenuState.ObjectMenu:
                    GlobalMenus.ObjectLayerMenu.IsVisible = true;
                    GlobalMenus.ObjectMenu.IsVisible = true;
                    GlobalButtons.LayerMenuButton.IsPressed = false;
                    GlobalButtons.AreaMenuButton.IsPressed = false;
                    GlobalButtons.ObjectMenuButton.IsPressed = true;
                    GlobalButtons.SpriteMenuButton.IsPressed = false;
                    break;
            }
        }
    }
}
