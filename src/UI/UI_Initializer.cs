using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tile_mapper.src.Canvas;
using tile_mapper.src.Layer;

namespace tile_mapper.src.UI
{
    internal static class UI_Initializer
    {
        // Buttons
        public static void InitializeToolsetButtons()
        {
            GlobalButtons.DrawTool = new Button("", new Rectangle(Global.ScreenWidth/2 - 80, 32, 32, 32), 160, 160, ButtonAction.DrawTool, true);
            GlobalButtons.DrawTool.SourceRect.Y = 96;

            GlobalButtons.FillTool = new Button("", new Rectangle(Global.ScreenWidth / 2 - 48, 32, 32, 32), 160 + 32, 160 + 32, ButtonAction.FillTool, true);
            GlobalButtons.FillTool.SourceRect.Y = 96;

            GlobalButtons.EraserTool = new Button("", new Rectangle(Global.ScreenWidth / 2 - 16, 32, 32, 32), 160 + 64, 160 + 64, ButtonAction.EraserTool, true);
            GlobalButtons.EraserTool.SourceRect.Y = 96;

            GlobalButtons.SpecifyStartPoint = new Button("", new Rectangle(Global.ScreenWidth / 2 + 16, 32, 32, 32), 288 + 32, 288 + 32, ButtonAction.SpecifyStartPoint, true);
            GlobalButtons.SpecifyStartPoint.SourceRect.Y = 96;

            GlobalButtons.SpecifyDoor = new Button("", new Rectangle(Global.ScreenWidth / 2 + 48 , 32, 32, 32), 288, 288, ButtonAction.SpecifyDoor, true);
            GlobalButtons.SpecifyDoor.SourceRect.Y = 96;
        }
        public static void InitializePaletteButtons()
        {
            GlobalButtons.Import = new Button("Import", new Rectangle(144 - 128 / 2, Global.ScreenHeight / 2 - 24, 128, 48), 288, 64, ButtonAction.Import, false);
            GlobalButtons.Import.SourceRect.Y = 128;
            GlobalButtons.OpenPalette = new Button("", new Rectangle(0, Global.ScreenHeight / 2 - 32 / 2, 32, 32), 32, 0, ButtonAction.OpenPalette, true);
            GlobalButtons.ClosePalette = new Button("", new Rectangle(272, Global.ScreenHeight / 2 - 32 / 2, 32, 32), 32, 0, ButtonAction.ClosePalette, false);
            GlobalButtons.OpenPalette.SourceRect = new Rectangle(0, 720, 32, 32);
            GlobalButtons.ClosePalette.SourceRect = new Rectangle(0, 752, 32, 32);
        }
        public static void InitializeTestButtons()
        {
            GlobalButtons.TestMap = new Button("", new Rectangle(Global.ScreenWidth / 2 - 32, 0, 32, 32), 0, 0, ButtonAction.TestState, true);
            GlobalButtons.TestMap.SourceRect.Y = 128;
            GlobalButtons.StopTest = new Button("", new Rectangle(Global.ScreenWidth / 2, 0, 32, 32), 32, 32, ButtonAction.EditState, true);
            GlobalButtons.StopTest.SourceRect.Y = 128;
        }
        public static void InitializeTopBarButtons()
        {
            GlobalButtons.WorldScreen = new Button("Editor", new Rectangle(0, 0, 144, 32), 304, 304, ButtonAction.EditorScreen, true);
            GlobalButtons.WorldScreen.IsPressed = true;
            GlobalButtons.WorldScreen.PressedSourceX = 448;
            GlobalButtons.WorldScreen.SourceRect.Y = 192;

            GlobalButtons.SheetScreen = new Button("Sprite Sheet", new Rectangle(144, 0, 144, 32), 304, 304, ButtonAction.SheetScreen, true);
            GlobalButtons.SheetScreen.IsPressed = false;
            GlobalButtons.SheetScreen.PressedSourceX = 448;
            GlobalButtons.SheetScreen.SourceRect.Y = 192;

            GlobalButtons.RuleSetScreen = new Button("Rule sets", new Rectangle(288, 0, 144, 32), 304, 304, ButtonAction.SheetScreen, true);
            GlobalButtons.RuleSetScreen.IsPressed = false;
            GlobalButtons.RuleSetScreen.PressedSourceX = 448;
            GlobalButtons.RuleSetScreen.SourceRect.Y = 192;
        }
        public static void InitializeScrollMenuButtons()
        {
            GlobalButtons.LayerMenuButton = new Button("", new Rectangle(1660, 1044 - 64, 32, 32), 1680 + 32, 1680, ButtonAction.OpenLayerMenu, true);
            GlobalButtons.LayerMenuButton.IsPressed = true;
            GlobalButtons.LayerMenuButton.SourceRect.Y = 1184;
            GlobalButtons.LayerMenuButton.PressedSourceX = 1648;

            GlobalButtons.AreaMenuButton = new Button("", new Rectangle(1660 + 32, 1044 - 64, 32, 32), 1680 + 32, 1680, ButtonAction.OpenAreaMenu, true);
            GlobalButtons.AreaMenuButton.IsPressed = false;
            GlobalButtons.AreaMenuButton.SourceRect.Y = 1184 + 32;
            GlobalButtons.AreaMenuButton.PressedSourceX = 1648;

            GlobalButtons.ObjectMenuButton = new Button("", new Rectangle(1660 + 64, 1044 - 64, 32, 32), 1680 + 32, 1680, ButtonAction.OpenObjectMenu, true);
            GlobalButtons.ObjectMenuButton.IsPressed = false;
            GlobalButtons.ObjectMenuButton.SourceRect.Y = 1184 + 64;
            GlobalButtons.ObjectMenuButton.PressedSourceX = 1648;

            GlobalButtons.SpriteMenuButton = new Button("", new Rectangle(1660 + 96, 1044 - 64, 32, 32), 1680 + 32, 1680, ButtonAction.OpenSpriteMenu, true);
            GlobalButtons.SpriteMenuButton.IsPressed = false;
            GlobalButtons.SpriteMenuButton.SourceRect.Y = 1184 + 96;
            GlobalButtons.SpriteMenuButton.PressedSourceX = 1648;
        }

        // Menus
        public static void InitializePaletteMenu()
        {
            GlobalMenus.TileMenu = new UI_Menu(false, new Rectangle(0, 192, 288, 512), new Rectangle(0, Global.ScreenHeight / 2 - 256, 288, 512));
            Global.SpritePaletteDestination = new Rectangle(GlobalMenus.TileMenu.Destination.X + 16, GlobalMenus.TileMenu.Destination.Y + 16, GlobalMenus.TileMenu.Destination.Width - 32, GlobalMenus.TileMenu.Destination.Height - 32);
        }
        public static void InitializeTopBar()
        { GlobalMenus.TopBar = new UI_Menu(true, new Rectangle(0, 0, 1920, 80), new Rectangle(0, 0, 1920, 67)); }
        public static void InitializeGeneralOverlay()
        { GlobalMenus.GeneralOverlay = new UI_Menu(true, new Rectangle(0, 0, 0, 0), new Rectangle(0, 0, 0, 0)); }
        public static void InitializePropertyMenu()
        { GlobalMenus.Properties = new UI_Menu(true, new Rectangle(1655, 64, 266, 1080), new Rectangle(1655, 0, 266, 1080)); }

        // Scrollable menus
        public static void InitializeLayerScrollMenu()
        {
            GlobalMenus.LayerMenu = new UI_Menu(true, new Rectangle(1655, 0, 0, 0), new Rectangle(1660, 32, 256, 496));
            GlobalMenus.LayerMenu.Scrollable = true;

            // Create Layer btn.
            GlobalButtons.CreateLayerButton = new Button("New Layer", new Rectangle(GlobalMenus.Properties.Destination.X + GlobalMenus.Properties.Destination.Width / 2 - 224 / 2, GlobalMenus.Properties.Destination.Y + 32 + 16 + 48 * 3, 224, 48), 528, 304, ButtonAction.AddLayer, true);
            GlobalButtons.CreateLayerButton.SourceRect.Y = 240;
            GlobalMenus.LayerMenu.buttons.Add(GlobalButtons.CreateLayerButton);
            ScrollMenuUtil.UpdateListOrder(GlobalMenus.LayerMenu);
        }
        public static void InitializeAreaScrollMenu()
        {
            GlobalMenus.AreaMenu = new UI_Menu(false, new Rectangle(1760, 32, 0, 0), new Rectangle(1660, 32, 256, 496));
            GlobalMenus.AreaMenu.Scrollable = true;
        }
        public static void InitializeObjectLayerScrollMenu()
        {
            GlobalMenus.ObjectLayerMenu = new UI_Menu(false, new Rectangle(1760, 32, 0, 0), new Rectangle(1660, 32, 256, 496));
            GlobalMenus.ObjectLayerMenu.Scrollable = true;

            // Create ObjectLayer btn.
            GlobalButtons.CreateObjectLayerButton = new Button("New Object Layer", new Rectangle(GlobalMenus.Properties.Destination.X + GlobalMenus.Properties.Destination.Width / 2 - 224 / 2, GlobalMenus.Properties.Destination.Y + 32 + 16 + 48 * 3, 224, 48), 528, 304, ButtonAction.CreateObjectLayer, true);
            GlobalButtons.CreateObjectLayerButton.SourceRect.Y = 240;
            GlobalMenus.ObjectLayerMenu.buttons.Add(GlobalButtons.CreateObjectLayerButton);
            ScrollMenuUtil.UpdateListOrder(GlobalMenus.ObjectLayerMenu);
        }
        public static void InitializeSpriteMenu()
        {
            // Collision sprite list
            GlobalMenus.CollisionSpriteList = new UI_Menu(false, new Rectangle(1768, 802, 0, 0), new Rectangle(1660, 32, 256, 496));
            GlobalMenus.CollisionSpriteList.Scrollable = true;
        }
        public static void InitializeObjectScrollMenu()
        {
            GlobalMenus.ObjectMenu = new UI_Menu(false, new Rectangle(1660, 96, 256, 422), Global.LabelMenuDestination); // Object menu has 2 button lists instead of labels.
            GlobalMenus.ObjectMenu.Scrollable = true;

            // Create Object btn.
            GlobalButtons.CreateObjectButton = new Button("New Object", new Rectangle(GlobalMenus.ObjectMenu.Destination.X + GlobalMenus.ObjectMenu.Destination.Width / 2 - 224 / 2, GlobalMenus.ObjectMenu.Destination.Y + 16, 224, 48), 528, 304, ButtonAction.CreateObject, true);
            GlobalButtons.CreateObjectButton.SourceRect.Y = 240;
            GlobalMenus.ObjectMenu.buttons.Add(GlobalButtons.CreateObjectButton);
            ScrollMenuUtil.UpdateListOrder(GlobalMenus.ObjectMenu);
        }

        // Label menus
        public static void InitializeTileLabelMenu()
        {
            GlobalMenus.TileLabels = new UI_Menu(false, new Rectangle(1768, 802, 0, 0), Global.LabelMenuDestination);
            GlobalLabels.CurrentTileID = new Label();
            GlobalLabels.Collision = new Label();

            // Sprite tile properties.
            GlobalLabels.CurrentTileID.LabelRect = new Rectangle(1660, 624 - 32 - 32, 256, 32);
            GlobalLabels.CurrentTileID.SourceRect.Width = 0;
            GlobalLabels.CurrentTileID.SourceRect.Height = 0;

            GlobalLabels.Collision.LabelRect = new Rectangle(1660, 624 - 32, 256, 32);
            GlobalLabels.Collision.Text = "Collision";
            GlobalLabels.Collision.SourceRect.Width = 0;
            GlobalLabels.Collision.SourceRect.Height = 0;

            GlobalButtons.CollisionCheckBox = new Button("", new Rectangle(1660, 656 - 64, 32, 32), 32, 0, ButtonAction.MakeCollision, false);
            GlobalButtons.CollisionCheckBox.SourceRect.Y = 80;
            GlobalButtons.CollisionCheckBox.PressedSourceX = 64;
        }
        public static void InitializeAreaLabelMenu()
        {
            GlobalMenus.AreaProperties = new UI_Menu(false, Global.ScrollMenuSource, Global.LabelMenuDestination);
            GlobalMenus.AreaProperties.Scrollable = true;
            // Create layer property btn.
            GlobalButtons.CreateAreaPropertyButton = new Button("New Property", new Rectangle(GlobalMenus.AreaProperties.Destination.X + GlobalMenus.AreaProperties.Destination.Width / 2 - 224 / 2, GlobalMenus.AreaProperties.Destination.Y + 16, 224, 48), 528, 304, ButtonAction.CreateAreaProperty, true);
            GlobalButtons.CreateAreaPropertyButton.SourceRect.Y = 240;
            GlobalMenus.AreaProperties.buttons.Add(GlobalButtons.CreateAreaPropertyButton);
            ScrollMenuUtil.UpdateListOrder(GlobalMenus.AreaProperties);
        }
        public static void InitializeLayerLabelMenu()
        {
            GlobalMenus.LayerProperties = new UI_Menu(true, Global.ScrollMenuSource, Global.LabelMenuDestination);
            GlobalMenus.LayerProperties.IsVisible = true;
            GlobalMenus.LayerProperties.Scrollable = true;
            // Create layer property btn.
            GlobalButtons.CreateLayerPropertyButton = new Button("New Property", new Rectangle(GlobalMenus.LayerProperties.Destination.X + GlobalMenus.LayerProperties.Destination.Width / 2 - 224 / 2, GlobalMenus.LayerProperties.Destination.Y + 16, 224, 48), 528, 304, ButtonAction.CreateLayerProperty, true);
            GlobalButtons.CreateLayerPropertyButton.SourceRect.Y = 240;
            GlobalMenus.LayerProperties.buttons.Add(GlobalButtons.CreateLayerPropertyButton);
            ScrollMenuUtil.UpdateListOrder(GlobalMenus.LayerProperties);
        }

        public static void InitializePropertyEditMenu()
        {
            GlobalMenus.PropertyEditMenu = new UI_Menu(false, Global.PropertyEditMenuSource, Global.PropertyEditMenuDestination);

            // ID label
            GlobalLabels.PropertyID = new Label();
            GlobalLabels.PropertyID.Text = "ID";
            GlobalLabels.PropertyID.IsVisible = true;
            GlobalLabels.PropertyID.LabelRect = new Rectangle(Global.PropertyEditMenuDestination.X + 36, Global.PropertyEditMenuDestination.Y + 4, 192, 32);
            GlobalMenus.PropertyEditMenu.labels.Add(GlobalLabels.PropertyID);

            // Type label
            GlobalLabels.PropertyType = new Label();
            GlobalLabels.PropertyType.Text = "Type";
            GlobalLabels.PropertyType.IsVisible = true;
            GlobalLabels.PropertyType.LabelRect = new Rectangle(Global.PropertyEditMenuDestination.X + 36, Global.PropertyEditMenuDestination.Y + 68, 192, 32);
            GlobalMenus.PropertyEditMenu.labels.Add(GlobalLabels.PropertyType);

            // Value label
            GlobalLabels.PropertyValue = new Label();
            GlobalLabels.PropertyValue.Text = "Value";
            GlobalLabels.PropertyValue.IsVisible = true;
            GlobalLabels.PropertyValue.LabelRect = new Rectangle(Global.PropertyEditMenuDestination.X + 36, Global.PropertyEditMenuDestination.Y + 68 + 64, 192, 32);
            GlobalMenus.PropertyEditMenu.labels.Add(GlobalLabels.PropertyValue);

            // Cancel button
            GlobalButtons.PropertyCancel = new Button("Cancel", new Rectangle(GlobalMenus.PropertyEditMenu.Destination.X + 36, GlobalMenus.PropertyEditMenu.Destination.Y + 228, 80, 32), 1504, 1392, ButtonAction.PropertyCancel, true);
            GlobalButtons.PropertyCancel.SourceRect = new Rectangle(1392, 1120, 80, 32);
            GlobalMenus.PropertyEditMenu.buttons.Add(GlobalButtons.PropertyCancel);

            // Save button
            GlobalButtons.PropertySave = new Button("Save", new Rectangle(GlobalMenus.PropertyEditMenu.Destination.X + 36 + 112, GlobalMenus.PropertyEditMenu.Destination.Y + 228, 80, 32), 1504, 1392, ButtonAction.PropertySave, true);
            GlobalButtons.PropertySave.SourceRect = new Rectangle(1392, 1120, 80, 32);
            GlobalMenus.PropertyEditMenu.buttons.Add(GlobalButtons.PropertySave);

            // Type go left button
            GlobalButtons.PropertyTypeLeft = new Button("", new Rectangle(GlobalLabels.PropertyType.LabelRect.X - 32, GlobalLabels.PropertyType.LabelRect.Y + 32, 32, 32), 1392 + 64, 1392, ButtonAction.PropertyGoLeft, true);
            GlobalButtons.PropertyTypeLeft.SourceRect = new Rectangle(1392, 1088, 32, 32);
            GlobalMenus.PropertyEditMenu.buttons.Add(GlobalButtons.PropertyTypeLeft);

            // Type go right button
            GlobalButtons.PropertyTypeRight = new Button("", new Rectangle(GlobalLabels.PropertyType.LabelRect.X + GlobalLabels.PropertyType.LabelRect.Width, GlobalLabels.PropertyType.LabelRect.Y + 32, 32, 32), 1392 + 96, 1392 + 32, ButtonAction.PropertyGoLeft, true);
            GlobalButtons.PropertyTypeRight.SourceRect = new Rectangle(1392 + 32, 1088, 32, 32);
            GlobalMenus.PropertyEditMenu.buttons.Add(GlobalButtons.PropertyTypeRight);

            // Current property properties (the property that the user is editing)
            GlobalLabels.CurrentPropertyID = new Label();
            GlobalLabels.CurrentPropertyID.Text = "";
            GlobalLabels.CurrentPropertyID.IsVisible = true;
            GlobalLabels.CurrentPropertyID.LabelRect = new Rectangle(Global.PropertyEditMenuDestination.X + 36, Global.PropertyEditMenuDestination.Y + 36, 192, 32);
            GlobalLabels.CurrentPropertyID.editType = Property.Type.String;
            GlobalMenus.PropertyEditMenu.labels.Add(GlobalLabels.CurrentPropertyID);
            GlobalLabels.EditableLabels.Add(GlobalLabels.CurrentPropertyID);

            GlobalLabels.CurrentPropertyType = new Label();
            GlobalLabels.CurrentPropertyType.Text = "";
            GlobalLabels.CurrentPropertyType.IsVisible = true;
            GlobalLabels.CurrentPropertyType.LabelRect = new Rectangle(Global.PropertyEditMenuDestination.X + 36, Global.PropertyEditMenuDestination.Y + 68 + 32, 192, 32);
            GlobalMenus.PropertyEditMenu.labels.Add(GlobalLabels.CurrentPropertyType);

            GlobalLabels.CurrentPropertyValue = new Label();
            GlobalLabels.CurrentPropertyValue.Text = "";
            GlobalLabels.CurrentPropertyValue.IsVisible = true;
            GlobalLabels.CurrentPropertyValue.LabelRect = new Rectangle(Global.PropertyEditMenuDestination.X + 36, Global.PropertyEditMenuDestination.Y + 68 + 96, 192, 32);
            GlobalMenus.PropertyEditMenu.labels.Add(GlobalLabels.CurrentPropertyValue);
            GlobalLabels.EditableLabels.Add(GlobalLabels.CurrentPropertyValue);
        }

        public static void InitializeEditObjectMenu()
        {
            GlobalMenus.EditObjectMenu = new UI_Menu(false, new Rectangle(1084, 796, 264, 372), Global.EditObjectMenuDestination);

            GlobalLabels.ObjectName = new Label();
            GlobalLabels.ObjectName.Text = "";
            GlobalLabels.ObjectName.IsVisible = true;
            GlobalLabels.ObjectName.editType = Property.Type.String;
            GlobalLabels.ObjectName.LabelRect = new Rectangle(Global.EditObjectMenuDestination.X + 36, Global.EditObjectMenuDestination.Y + 20, 192, 32);
            GlobalLabels.EditableLabels.Add(GlobalLabels.ObjectName);
            GlobalLabels.ObjectName.editingWhat = Global.IsEditingWhat.ObjectName;

            GlobalMenus.EditObjectMenu.labels.Add(GlobalLabels.ObjectName);

            // Cancel button
            GlobalButtons.ObjectCancel = new Button("Cancel", new Rectangle(GlobalMenus.PropertyEditMenu.Destination.X + 36, GlobalMenus.EditObjectMenu.Destination.Y + 324, 80, 32), 1504, 1392, ButtonAction.CancelObject, true);
            GlobalButtons.ObjectCancel.SourceRect = new Rectangle(1392, 1120, 80, 32);
            GlobalMenus.EditObjectMenu.buttons.Add(GlobalButtons.ObjectCancel);

            // Save button
            GlobalButtons.ObjectSave = new Button("Save", new Rectangle(GlobalMenus.PropertyEditMenu.Destination.X + 36 + 112, GlobalMenus.EditObjectMenu.Destination.Y + 324, 80, 32), 1504, 1392, ButtonAction.SaveObject, true);
            GlobalButtons.ObjectSave.SourceRect = new Rectangle(1392, 1120, 80, 32);
            GlobalMenus.EditObjectMenu.buttons.Add(GlobalButtons.ObjectSave);

            GlobalMenus.ObjectProperties = new UI_Menu(false, new Rectangle(0,0,0,0), new Rectangle(GlobalMenus.EditObjectMenu.Destination.X, GlobalMenus.EditObjectMenu.Destination.Y + 71, GlobalMenus.EditObjectMenu.Destination.Width, GlobalMenus.EditObjectMenu.Destination.Height - 135));
            GlobalMenus.ObjectProperties.Scrollable = true;

            GlobalButtons.CreateObjectProperty = new Button("New Property", new Rectangle(GlobalMenus.ObjectProperties.Destination.X + GlobalMenus.ObjectProperties.Destination.Width / 2 - 224 / 2, GlobalMenus.ObjectProperties.Destination.Y, 224, 48), 528, 304, ButtonAction.CreateObjectProperty, true);
            GlobalButtons.CreateObjectProperty.SourceRect = new Rectangle(304, 240, 224, 48);
            GlobalButtons.CreateObjectProperty.SourceRect.Y = 240;
            GlobalButtons.CreateObjectProperty.SelectionX = 528;
            GlobalMenus.ObjectProperties.buttons.Add(GlobalButtons.CreateObjectProperty);
            ScrollMenuUtil.UpdateListOrder(GlobalMenus.ObjectProperties);
        }

        // Add buttons to the menus
        public static void AddButtonsToMenus()
        {
            GlobalMenus.TopBar.buttons.Add(GlobalButtons.DrawTool);
            GlobalMenus.TopBar.buttons.Add(GlobalButtons.FillTool);
            GlobalMenus.TopBar.buttons.Add(GlobalButtons.EraserTool);
            GlobalMenus.TopBar.buttons.Add(GlobalButtons.SpecifyStartPoint);
            GlobalMenus.TopBar.buttons.Add(GlobalButtons.SpecifyDoor);
            GlobalMenus.TopBar.buttons.Add(GlobalButtons.TestMap);
            GlobalMenus.TopBar.buttons.Add(GlobalButtons.StopTest);
            GlobalMenus.TopBar.buttons.Add(GlobalButtons.WorldScreen);
            GlobalMenus.TopBar.buttons.Add(GlobalButtons.SheetScreen);
            GlobalMenus.TopBar.buttons.Add(GlobalButtons.RuleSetScreen);

            GlobalMenus.GeneralOverlay.buttons.Add(GlobalButtons.OpenPalette);
            GlobalMenus.GeneralOverlay.buttons.Add(GlobalButtons.Import);
            GlobalMenus.GeneralOverlay.buttons.Add(GlobalButtons.ClosePalette);

            // Property menu buttons (the small tabs at the bottom).
            GlobalMenus.Properties.buttons.Add(GlobalButtons.LayerMenuButton);
            GlobalMenus.Properties.buttons.Add(GlobalButtons.ObjectMenuButton);
            GlobalMenus.Properties.buttons.Add(GlobalButtons.AreaMenuButton);
            GlobalMenus.Properties.buttons.Add(GlobalButtons.SpriteMenuButton);

            // Tile property labels.
            GlobalMenus.TileLabels.labels.Add(GlobalLabels.CurrentTileID);
            GlobalMenus.TileLabels.labels.Add(GlobalLabels.Collision);
            GlobalMenus.TileLabels.buttons.Add(GlobalButtons.CollisionCheckBox);

            // Labels for area menu.
            GlobalMenus.AreaProperties.labels.Add(GlobalLabels.AreaName);
            GlobalMenus.AreaProperties.labels.Add(GlobalLabels.AreaWidth);
            GlobalMenus.AreaProperties.labels.Add(GlobalLabels.AreaHeight);
            GlobalMenus.AreaProperties.labels.Add(GlobalLabels.AreaX);
            GlobalMenus.AreaProperties.labels.Add(GlobalLabels.AreaY);
        }
    }
}
