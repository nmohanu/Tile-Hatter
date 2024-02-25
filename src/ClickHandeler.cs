using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static tile_mapper.src.ProgramLoop;
using tile_mapper.src.Canvas;
using tile_mapper.src.Layer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using tile_mapper.src.UI;
using tile_mapper.src.UserSprites;

namespace tile_mapper.src
{
    internal static class ClickHandeler
    {
        public static void HandleLeftClick(MouseState mouseState, GraphicsDevice graphicsDevice, bool IsDoubleClick, KeyboardState keyboardState)
        {
            if (mouseState.LeftButton == ButtonState.Pressed && Global.PreviousMouseState.LeftButton != ButtonState.Pressed) // Click (Left) execute once.
            {
                Global.resetCursorState = true;
                Global.resetSelection = true;

                if (Global.CursorActionState == CursorState.SpecifyingStartPoint)
                    ToolUtil.SpecifyStartPoint();
                else if (Global.CursorActionState == CursorState.SpecifyDoor)
                    ToolUtil.SpecifyDoor();
                else
                    Global.A = null;

                // Reset CursorState
                if (Global.resetCursorState && Global.CursorActionState != CursorState.Eraser && Global.CursorActionState != CursorState.Draw && Global.CursorActionState != CursorState.Fill)
                {
                    Global.CursorActionState = CursorState.None;
                }


                foreach (var UI in Global.All_UI_Menus)
                {
                    if (UI.Destination.Contains(Global.MousePos))
                        Global.resetSelection = false;
                }

                if (Global.resetSelection)
                {
                    Global.ClickPoint = new Point(Global.SelectedX, Global.SelectedY);
                    Global.SelectionStart = Global.ClickPoint;
                    Global.SelectionEnd = Global.SelectionStart;
                    Global.Selection.Width = 0;
                    Global.Selection.Height = 0;

                    if (!keyboardState.IsKeyDown(Keys.LeftShift))
                    {
                        if (GlobalButtons.ClickedAreaButton != null)
                            GlobalButtons.ClickedAreaButton.IsPressed = false;
                        Global.SelectedArea = null;
                    }
                }

                // Check whether a button is clicked.
                Button buttonClicked = null;
                foreach (var UI in Global.All_UI_Menus)
                {

                    if (UI.IsVisible)
                    {
                        buttonClicked = UI.HandleClicks(Global.MousePos);
                        if (buttonClicked != null)
                            break;
                    }
                }

                if (buttonClicked == null) // No button clicked.
                    return;
                else // Button clicked, process.
                {
                    if (!IsDoubleClick)
                        HandleButtonClick(buttonClicked, graphicsDevice);
                    else
                        HandleDoubleClick(buttonClicked, graphicsDevice);

                    // Check if the delete button (X) on the button was clicked and delete if so.
                    if (buttonClicked.IsDeletable && buttonClicked.DeleteButton.ButtonRect.Contains(Global.MousePos)) // The delete buttons (X)
                    {
                        switch (buttonClicked.DeleteButton.Action)
                        {
                            case ButtonAction.RemoveLayer:
                                Global.CurrentMap.RemoveLayer(buttonClicked.HelperInt);
                                GlobalMenus.LayerMenu.buttons.Remove(GlobalMenus.LayerMenu.buttons[buttonClicked.HelperInt]);
                                if (Global.CurrentLayer == buttonClicked.HelperInt)
                                {
                                    Global.CurrentLayer--;
                                    LabelUtil.ClearLabels(GlobalMenus.LayerProperties);
                                }
                                ScrollMenuUtil.UpdateListOrder(GlobalMenus.LayerMenu);
                                break;
                            case ButtonAction.RemoveArea:
                                Global.CurrentMap.RemoveArea(buttonClicked.HelperInt);
                                Global.SelectedArea = null;

                                GlobalMenus.AreaMenu.buttons.Remove(GlobalMenus.AreaMenu.buttons[buttonClicked.HelperInt]);
                                ScrollMenuUtil.UpdateListOrder(GlobalMenus.AreaMenu);
                                AreaUtil.UpdateAreaLabels();
                                break;
                            case ButtonAction.RemoveCollisionSprite:
                                GlobalMenus.CollisionSpriteList.buttons.Remove(buttonClicked);
                                Global.selected.Collision = false;
                                ScrollMenuUtil.UpdateListOrder(GlobalMenus.CollisionSpriteList);
                                LabelUtil.ClearLabels(GlobalMenus.TileLabels);
                                break;
                            case ButtonAction.RemoveObjectLayer:
                                GlobalMenus.ObjectLayerMenu.buttons.Remove(buttonClicked);
                                Global.CurrentMap.ObjectLayers.RemoveAt(buttonClicked.HelperInt);
                                ScrollMenuUtil.UpdateListOrder(GlobalMenus.ObjectLayerMenu);
                                break;
                            case ButtonAction.RemoveObject:
                                GlobalMenus.ObjectMenu.buttons.Remove(buttonClicked);
                                Global.CurrentMap.ObjectLayers[GlobalButtons.SelectedObjectLayerButton.HelperInt].objects.RemoveAt(buttonClicked.HelperInt);
                                ScrollMenuUtil.UpdateListOrder(GlobalMenus.ObjectMenu);
                                break;
                        }
                    }
                }
            }
        }

        public static void HandleButtonClick(Button buttonClicked, GraphicsDevice graphicsDevice)
        {
            switch (buttonClicked.Action)
            {
                case ButtonAction.Import:
                    FileUtil.OpenSpriteSheetFile(Global.TileSheetPath, graphicsDevice);
                    buttonClicked.IsVisible = false;
                    break;
                case ButtonAction.Layer:
                    Global.CurrentLayer = buttonClicked.HelperInt;
                    if (GlobalButtons.ClickedLayerButton != null)
                        GlobalButtons.ClickedLayerButton.IsPressed = false;
                    buttonClicked.IsPressed = true;
                    GlobalButtons.ClickedLayerButton = buttonClicked;
                    ObjectUtil.ReloadLayerProperties();
                    // GlobalLabels.LayerName.Text = "ID: " + GlobalButtons.ClickedLayerButton.Text;
                    break;
                case ButtonAction.OpenLayerMenu:
                    Global.menuState = MenuState.LayerMenu;
                    ScrollMenuUtil.UpdateMenuState();
                    break;
                case ButtonAction.OpenSpriteMenu:
                    Global.menuState = MenuState.SpriteTileMenu;
                    ScrollMenuUtil.UpdateMenuState();
                    break;
                case ButtonAction.OpenObjectMenu:
                    Global.menuState = MenuState.ObjectMenu;
                    ScrollMenuUtil.UpdateMenuState();
                    break;
                case ButtonAction.Save:
                    break;
                case ButtonAction.OpenPalette:
                    // TileMenu.IsVisible = true;
                    GlobalButtons.OpenPalette.IsVisible = false;
                    GlobalButtons.ClosePalette.IsVisible = true;
                    if (!Global.HasTileSheet)
                        GlobalButtons.Import.IsVisible = true;
                    Global.TilePaletteVisible = true;
                    break;
                case ButtonAction.DrawTool:
                    Global.CursorActionState = CursorState.Draw;
                    break;
                case ButtonAction.FillTool:
                    Global.CursorActionState = CursorState.Fill;
                    ToolUtil.FillSelection();
                    break;
                case ButtonAction.EraserTool:
                    Global.CursorActionState = CursorState.Eraser;
                    break;
                case ButtonAction.SelectArea:
                    foreach (var area in Global.CurrentMap.areas)
                    {
                        if (area.AreaName == buttonClicked.Text)
                        {
                            Global.Selection = area.AreaCords;
                            buttonClicked.IsPressed = true;
                            if (GlobalButtons.ClickedAreaButton != null)
                                GlobalButtons.ClickedAreaButton.IsPressed = false;
                            GlobalButtons.ClickedAreaButton = buttonClicked;
                            Global.SelectedArea = area;
                        }
                    }
                    AreaUtil.UpdateAreaLabels();
                    ObjectUtil.ReloadAreaProperties();
                    break;
                case ButtonAction.OpenAreaMenu:
                    Global.menuState = MenuState.AreaMenu;
                    ScrollMenuUtil.UpdateMenuState();
                    break;
                case ButtonAction.RemoveArea:
                    if (GlobalButtons.ClickedAreaButton != null)
                    {
                        for (int i = 0; i < Global.CurrentMap.areas.Count; i++)
                        {
                            if (Global.CurrentMap.areas[i].AreaName == GlobalButtons.ClickedAreaButton.Text)
                            {
                                Global.CurrentMap.areas.Remove(Global.CurrentMap.areas[i]);
                            }
                        }

                        GlobalMenus.Properties.buttons.Remove(GlobalMenus.Properties.buttons.LastOrDefault(obj => obj.Text == GlobalButtons.ClickedAreaButton.Text));


                        GlobalButtons.ClickedAreaButton = null;
                    }
                    break;
                case ButtonAction.SpecifyStartPoint:
                    Global.CursorActionState = CursorState.SpecifyingStartPoint;
                    break;
                case ButtonAction.SpecifyDoor:
                    Global.CursorActionState = CursorState.SpecifyDoor;
                    break;
                case ButtonAction.ClosePalette:
                    // TileMenu.IsVisible = false;
                    GlobalButtons.OpenPalette.IsVisible = true;
                    GlobalButtons.Import.IsVisible = false;
                    GlobalButtons.ClosePalette.IsVisible = false;
                    Global.TilePaletteVisible = false;
                    break;
                case ButtonAction.TestState:
                    if (Global.CurrentMap.StartLocationSpecified)
                        Global.State = EditorState.Test;
                    Global.CurrentArea = Global.StartArea;
                    Global.OriginalOffset = Global.Offset;
                    Global.Offset = new Vector2(Global.ScreenWidth / 2 - Global.CurrentMap.StartLocation.X * Global.TILE_SIZE * Global.TestingScale, Global.ScreenHeight / 2 - Global.CurrentMap.StartLocation.Y * Global.TILE_SIZE * Global.TestingScale);
                    Global.CharacterSource.X = 0;
                    break;
                case ButtonAction.EditState:
                    Global.State = EditorState.Edit;
                    Global.Offset = Global.OriginalOffset;
                    break;
                case ButtonAction.MakeCollision:
                    buttonClicked.IsPressed = !buttonClicked.IsPressed;
                    Global.selected.Collision = buttonClicked.IsPressed;
                    Global.TileSpriteList[Global.CurrentPage].FirstOrDefault(obj => obj.ID == Global.selected.ID).Collision = Global.selected.Collision;

                    Global.CurrentMap.CollisionTiles.Clear();
                    foreach (var tile in Global.TileSpriteList[Global.CurrentPage])
                    {
                        if (tile.Collision)
                            Global.CurrentMap.CollisionTiles.Add(tile);
                    }

                    // Tile is added to list, create button for it.
                    if (buttonClicked.IsPressed)
                    {
                        Button button = ScrollMenuUtil.CreateRemovableButton(ButtonAction.SelectCollisionSprite, ButtonAction.RemoveCollisionSprite, GlobalMenus.Properties);
                        button.Text = Global.selected.ID;
                        GlobalMenus.CollisionSpriteList.buttons.Add(button);
                        ScrollMenuUtil.UpdateListOrder(GlobalMenus.CollisionSpriteList);
                    }
                    else // Removed from list, remove the button.
                    {
                        GlobalMenus.CollisionSpriteList.buttons.Remove(GlobalMenus.CollisionSpriteList.buttons.FirstOrDefault(obj => obj.Text == Global.selected.ID));
                        ScrollMenuUtil.UpdateListOrder(GlobalMenus.CollisionSpriteList);
                    }
                    break;
                case ButtonAction.AddLayer:
                    LayerUtil.AddLayer();
                    break;
                case ButtonAction.RemoveLayer:
                    Global.CurrentMap.RemoveLayer(buttonClicked.HelperInt);
                    if(Global.CurrentLayer == buttonClicked.HelperInt)
                    {
                        Global.CurrentLayer = 0;
                    }
                    ScrollMenuUtil.UpdateListOrder(GlobalMenus.LayerMenu);
                    ObjectUtil.ReloadLayerProperties();
                    break;
                case ButtonAction.SelectCollisionSprite:
                    if (GlobalButtons.ClickedTileButton != null)
                        GlobalButtons.ClickedTileButton.IsPressed = false;
                    buttonClicked.IsPressed = true;
                    SpriteUtil.SelectTile(Global.TileSpriteList[Global.CurrentPage].LastOrDefault(obj => obj.ID == buttonClicked.Text));
                    GlobalButtons.ClickedTileButton = buttonClicked;
                    break;
                case ButtonAction.CreateObjectLayer:
                    ObjectUtil.AddObjectLayer();
                    break;
                case ButtonAction.CreateObject:
                    ObjectUtil.AddObject();
                    break;
                case ButtonAction.SelectObjectLayer:
                    if (GlobalButtons.SelectedObjectLayerButton != null)
                        GlobalButtons.SelectedObjectLayerButton.IsPressed = false;
                    GlobalButtons.SelectedObjectLayerButton = buttonClicked;
                    buttonClicked.IsPressed = true;
                    ObjectUtil.ReloadObjects();
                    break;
                case ButtonAction.CreateLayerProperty:
                    ObjectUtil.AddLayerProperty();
                    break;
                case ButtonAction.CreateAreaProperty:
                    ObjectUtil.AddAreaProperty();
                    break;
                case ButtonAction.PropertyCancel:
                    ObjectUtil.CancelPropertyEdit();
                    break;
                case ButtonAction.PropertySave:
                    ObjectUtil.SaveProperty(buttonClicked.Property);
                    break;
                case ButtonAction.PropertyGoLeft:
                    ObjectUtil.PropertyGoLeft();
                    break;
                case ButtonAction.PropertyGoRight:
                    ObjectUtil.PropertyGoRight();
                    break;
            }

        }
        public static void HandleLeftHold(MouseState mouseState, KeyboardState keyboardState)
        {
            foreach (var menu in Global.All_UI_Menus)
                if (menu.Destination.Contains(Global.MousePos) && menu.IsVisible)
                    return;
            // Execute each frame if mouse button is held.
            if (mouseState.LeftButton == ButtonState.Pressed && Global.selected != null && !keyboardState.IsKeyDown(Keys.LeftShift))
            {
                foreach (var area in Global.CurrentMap.areas)
                {
                    if (area.AreaCords.Contains(Global.SelectedX, Global.SelectedY))
                    {
                        switch (Global.CursorActionState)
                        {
                            case CursorState.Draw:
                                if (Global.CurrentMap.LayerAmount > 0 && area.Layers[Global.CurrentLayer].TileMap[Global.SelectedY - area.AreaCords.Y, Global.SelectedX - area.AreaCords.X].ID != Global.selected.ID)
                                {
                                    area.Layers[Global.CurrentLayer].TileMap[Global.SelectedY - area.AreaCords.Y, Global.SelectedX - area.AreaCords.X].ID = Global.selected.ID;
                                    area.Layers[Global.CurrentLayer].TileMap[Global.SelectedY - area.AreaCords.Y, Global.SelectedX - area.AreaCords.X].Source = Global.selected.Source;
                                    Global.Actions.Push(new UserAction(UserAction.ActionType.Draw, Global.CurrentLayer, Global.SelectedX, Global.SelectedY));
                                }
                                break;
                            case CursorState.Eraser:
                                area.Layers[Global.CurrentLayer].TileMap[Global.SelectedY - area.AreaCords.Y, Global.SelectedX - area.AreaCords.X].ID = "0";
                                area.Layers[Global.CurrentLayer].TileMap[Global.SelectedY - area.AreaCords.Y, Global.SelectedX - area.AreaCords.X].Source = new Rectangle();
                                break;
                            case CursorState.Fill:
                                if (Global.PreviousMouseState.LeftButton != ButtonState.Pressed) // Exception
                                {
                                    bool allowed = true;
                                    foreach (var UI in Global.All_UI_Menus)
                                    {
                                        if (UI.Destination.Contains(Global.MousePos))
                                            allowed = false;
                                    }
                                    if (allowed)
                                        ToolUtil.FillClicked();
                                }
                                break;
                        }
                    }
                }
            }
            if (mouseState.LeftButton == ButtonState.Pressed && keyboardState.IsKeyDown(Keys.LeftShift) && Global.SelectionStart.X >= 0 && Global.SelectionStart.Y >= 0 && (Global.SelectedX != Global.ClickPoint.X || Global.SelectedY != Global.ClickPoint.Y))
            {
                Global.SelectionEnd = new Point(Global.SelectedX, Global.SelectedY);
                // Create a square of the selection
                Point TopLeft = new Point(Math.Min(Global.SelectionStart.X, Global.SelectionEnd.X), Math.Min(Global.SelectionStart.Y, Global.SelectionEnd.Y));
                Point BottomRight = new Point(Math.Max(Global.SelectionStart.X, Global.SelectionEnd.X), Math.Max(Global.SelectionStart.Y, Global.SelectionEnd.Y));
                Global.Selection = new Rectangle(TopLeft.X, TopLeft.Y, BottomRight.X - TopLeft.X + 1, BottomRight.Y - TopLeft.Y + 1);
            }
        }

        public static void HandleDoubleClick(Button ButtonClicked, GraphicsDevice graphicsDevice)
        {
            switch(ButtonClicked.Action)
            {
                case ButtonAction.SelectProperty:
                    if (ButtonClicked.Property != null)
                        ObjectUtil.OpenPropertyMenu(ButtonClicked.Property);
                    break;
            }
        }
    }
}
