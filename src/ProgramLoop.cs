using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using tile_mapper.src.Canvas;
using tile_mapper.src.Layer;
using tile_mapper.src.UI;
using Object = tile_mapper.src.Canvas.Object;

namespace tile_mapper.src
{
    public class ProgramLoop : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Cursor state
        public enum CursorState
        {
            SpecifyingStartPoint,
            SpecifyDoor,
            Draw,
            Eraser,
            Fill,
            None
        }

        // Edit or testing state
        public enum EditorState
        {
            Edit,
            Test
        }

        // Menu state, determines which menu is shown.
        public enum MenuState
        {
            LayerMenu,
            AreaMenu,
            SpriteTileMenu,
            ObjectMenu
        }

        

        // Constructor
        public ProgramLoop()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = Global.ScreenWidth;
            _graphics.PreferredBackBufferHeight = Global.ScreenHeight;
            IsFixedTimeStep = false;

            Content.RootDirectory = "Content";
            IsMouseVisible = false;
        }

        // Helper variables
        internal void InitializeHelperVariables()
        {
            Global.PreviousMouseState = new MouseState();
            Global.PreviousKeybordState = new KeyboardState();
            Global.All_UI_Menus = new List<UI_Menu>();
            Global.Offset = new Vector2(Global.ScreenWidth / 2, Global.ScreenHeight / 2);
            Global.TileSpriteList = new List<List<SpriteTile>>(); // Rectangles for the sprites.
            Global.CharacterRect = new Rectangle(Global.ScreenWidth / 2 - 16, Global.ScreenHeight / 2 - 16, (int)(32 * 2f), (int)(32 * 2f));
        }

        // Monogame window stuff
        internal void InitializeWindow()
        {
            Window.Title = "Tile-Hatter";
            Window.AllowUserResizing = false;
        }
        internal void InitializeGraphicsDevice()
        {
            _graphics.PreferredBackBufferWidth = Global.ScreenWidth;
            _graphics.PreferredBackBufferHeight = Global.ScreenHeight;
            _graphics.ApplyChanges();
        }

        // Add UI to lists.
        internal void AddUIToLists()
        {
            // Draw these to screen.
            Global.All_UI_Menus.Add(GlobalMenus.TileMenu);
            Global.All_UI_Menus.Add(GlobalMenus.TopBar);
            Global.All_UI_Menus.Add(GlobalMenus.GeneralOverlay);
            Global.All_UI_Menus.Add(GlobalMenus.Properties);

            Global.All_UI_Menus.Add(GlobalMenus.ObjectMenu);
            Global.All_UI_Menus.Add(GlobalMenus.AreaMenu);
            Global.All_UI_Menus.Add(GlobalMenus.LayerMenu);
            Global.All_UI_Menus.Add(GlobalMenus.CollisionSpriteList);

            // Labels
            Global.All_UI_Menus.Add(GlobalMenus.LayerLabels);
            Global.All_UI_Menus.Add(GlobalMenus.AreaLabels);
            Global.All_UI_Menus.Add(GlobalMenus.TileLabels);
            Global.All_UI_Menus.Add(GlobalMenus.ObjectLabels);

            // Keep track of the menus in the right UI bar.
            Global.PropertyMenu.Add(GlobalMenus.LayerMenu);
            Global.PropertyMenu.Add(GlobalMenus.AreaMenu);
            Global.PropertyMenu.Add(GlobalMenus.ObjectMenu);
            Global.PropertyMenu.Add(GlobalMenus.CollisionSpriteList);

            // Scrollable
            Global.ScrollableMenus.Add(GlobalMenus.LayerMenu);
            Global.ScrollableMenus.Add(GlobalMenus.AreaMenu);
            Global.ScrollableMenus.Add(GlobalMenus.ObjectMenu);
            Global.ScrollableMenus.Add(GlobalMenus.CollisionSpriteList);
            Global.ScrollableMenus.Add(GlobalMenus.ObjectLabels);

            Global.LabelMenus.Add(GlobalMenus.LayerLabels);
            Global.LabelMenus.Add(GlobalMenus.AreaLabels);
            Global.LabelMenus.Add(GlobalMenus.TileLabels);
            Global.LabelMenus.Add(GlobalMenus.ObjectLabels);
        }

        internal void InitializeUI()
        {
            // Initialize toolset buttons.
            UI_Initializer.InitializeToolsetButtons();

            // Initialize palette Buttons.
            UI_Initializer.InitializePaletteButtons();

            // Intialize testing buttons.
            UI_Initializer.InitializeTestButtons();

            // Initialize top bar buttons (scene switch buttons).
            UI_Initializer.InitializeTopBarButtons();

            // Initialize the buttons for the side menu.
            UI_Initializer.InitializeScrollMenuButtons();

            // Initialize the UI menu's
            UI_Initializer.InitializeTopBar();
            UI_Initializer.InitializeGeneralOverlay();
            UI_Initializer.InitializePropertyMenu();
            UI_Initializer.InitializePaletteMenu();

            // Scroll menus
            UI_Initializer.InitializeSpriteMenu();
            UI_Initializer.InitializeLayerScrollMenu();
            UI_Initializer.InitializeAreaScrollMenu();
            UI_Initializer.InitializeObjectScrollMenu();
            UI_Initializer.InitializeObjectLayerScrollMenu();

            // Label menus
            UI_Initializer.InitializeTileLabelMenu();
            UI_Initializer.InitializeAreaLabelMenu();
            UI_Initializer.InitializeLayerLabelMenu();

            // Add buttons and labels to the menus
            UI_Initializer.AddButtonsToMenus();

            // Add the created UI elements to a list.
            AddUIToLists();
        }

        // Initialize
        protected override void Initialize()
        {
            // Initialize program.
            InitializeHelperVariables();

            // Initialize window.
            InitializeWindow();

            // Initialize graphics device.
            InitializeGraphicsDevice();

            // Initialize all UI.
            InitializeUI();

            base.Initialize();
        }

        // Load content
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Global.SpriteSheet = new SpriteSheet(Global.TILE_SIZE);
            Global.SpriteSheet.Texture = Content.Load<Texture2D>("tile_sheet");

            using (FileStream stream = new FileStream("../../../Content/UI_new.png", FileMode.Open))
            {
                Global.UI = Texture2D.FromStream(GraphicsDevice, stream);
            }
            //UI = Content.Load<Texture2D>("UI");
            Global.Grid = Content.Load<Texture2D>("grid");
            Global.font = Content.Load<SpriteFont>("font");
        }

        // Update
        protected override void Update(GameTime gameTime)
        {
            // Calculate fps
            Global.fps = (float)(1.0f / gameTime.ElapsedGameTime.TotalSeconds);
            bool DoubleClick;

            // Keyboard.
            KeyboardState keyboardState = Keyboard.GetState();
            Global.Velocity = Vector2.Zero;
            HandleKeyboard(keyboardState, gameTime);

            // Mouse
            MouseState mouseState = Mouse.GetState();
            Global.MousePos = new Vector2(mouseState.X, mouseState.Y);
            Vector2 MousePosRelative = Global.MousePos - Global.Offset;
            HandleLeftClick(mouseState);
            HandleLeftHold(mouseState, keyboardState);

            if (mouseState.RightButton == ButtonState.Pressed)
            {
                if (Global.PreviousMousePos != null && Global.PreviousMousePos != Global.MousePos && Global.SpritePaletteDestination.Contains(Global.MousePos))
                {
                    Global.PaletteScrollOffset -= Global.PreviousMousePos - Global.MousePos;
                }
            }

            // Check if mouse is hoovering on button.
            foreach (var UI in Global.All_UI_Menus)
            {
                foreach (var button in UI.buttons)
                    if (button != null && !button.IsPressed)
                        button.ChangeSourceX(Global.MousePos);
                    else if (!button.IsPressed)
                    {
                        button.SourceRect.X = button.SelectionX;
                    }
                    else
                        button.ChangeSourceX(Global.MousePos);
            }


            // Tile sheet is imported.
            if (Global.HasTileSheet)
            {
                foreach (var rect in Global.TileSpriteList[Global.CurrentPage])
                {
                    // User is hoovering on sprite tile.
                    if (rect.Destination.Contains(Global.MousePos - Global.PaletteScrollOffset))
                    {
                        // User selected a sprite.
                        if (mouseState.LeftButton == ButtonState.Pressed)
                        {
                            SelectTile(rect);
                        }
                        else
                        {
                            rect.hovers = true;
                        }
                    }
                    else
                    {
                        rect.hovers = false;
                    }
                }
            }
            // User is scrolling
            if (mouseState.ScrollWheelValue != Global.OriginalScrollWheelValue && Global.State == EditorState.Edit)
            {
                bool MenuScroll = false;

                // Scroll is menu scroll.
                foreach (var menu in Global.All_UI_Menus)
                    if (menu.Destination.Contains(Global.MousePos) && menu.IsVisible)
                    {
                        if (menu.Scrollable && menu.buttons.Count() > 0)
                        {
                            int adjustment = (int)((mouseState.ScrollWheelValue - Global.OriginalScrollWheelValue) * 0.08f);
                            int ScrollYOrg = (int)menu.ScrollMenuOffset.Y;
                            menu.ScrollMenuOffset.Y += adjustment;
                            menu.ScrollMenuOffset.Y = Math.Min(menu.ScrollMenuOffset.Y, 0);

                            if (menu.buttons[menu.buttons.Count() - 1].ButtonRect.Y + (int)(menu.ScrollMenuOffset.Y - ScrollYOrg) < menu.Destination.Bottom - 64)
                            {
                                menu.ScrollMenuOffset.Y = ScrollYOrg;
                            }
                            else
                            {
                                foreach (var btn in menu.buttons)
                                {
                                    btn.ButtonRect = new Rectangle(btn.ButtonRect.X, btn.ButtonRect.Y + (int)(menu.ScrollMenuOffset.Y - ScrollYOrg), 224, 48);
                                    if (btn.IsDeletable)
                                        btn.DeleteButton.ButtonRect.Y = btn.ButtonRect.Y + 16;
                                }
                            }
                        }
                        MenuScroll = true;
                    }


                // User is zooming on map
                if (!MenuScroll)
                {
                    Vector2 Center = new Vector2(Global.Offset.X, Global.Offset.Y);
                    Vector2 MouseBefore = (Global.MousePos - Global.Offset) / Global.Scale;

                    float adjustment = (mouseState.ScrollWheelValue - Global.OriginalScrollWheelValue) * 0.0004f;
                    // Adjust the scaling factor based on the scroll wheel delta
                    Global.Scale += adjustment;
                    Global.Scale = MathHelper.Clamp(Global.Scale, 0.5f, 5f);
                    // Vector2 MouseAfter = MouseBefore * Scale;

                    Vector2 CenterNew = new Vector2(Global.Offset.X, Global.Offset.Y);

                    Global.Offset += (Center - CenterNew) / 2;
                    Vector2 mouseAfter = (new Vector2(mouseState.X, mouseState.Y) - Global.Offset) / Global.Scale;

                    Vector2 mousePositionDifference = MouseBefore - mouseAfter;
                    // Adjust the offset to keep the mouse position stationary
                    Global.Offset -= mousePositionDifference * Global.Scale;
                }
                else
                {


                }
            }

            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                Global.CursorActionState = CursorState.None;
                Global.Selection.Width = 0;
                Global.Selection.Height = 0;
            }

            // Only update the selected square if user is not using scroll wheel.
            if (mouseState.ScrollWheelValue == Global.OriginalScrollWheelValue)
            {
                Vector2 mousePosInt;
                if (Global.State == EditorState.Test)
                {
                    mousePosInt = MousePosRelative / Global.TestingScale / Global.TILE_SIZE;
                }
                else
                {
                    mousePosInt = MousePosRelative / Global.Scale / Global.TILE_SIZE;

                }
                Global.SelectedX = (int)Math.Floor(mousePosInt.X);
                Global.SelectedY = (int)Math.Floor(mousePosInt.Y);
            }

            // Create area.
            if (keyboardState.IsKeyDown(Keys.Enter) && !Global.PreviousKeybordState.IsKeyDown(Keys.Enter) && Global.Selection.Width >= 4 && Global.Selection.Height >= 4)
                AddArea();


            // Undo last action.
            if (keyboardState.IsKeyDown(Keys.LeftControl) && keyboardState.IsKeyDown(Keys.Z) && !Global.PreviousKeybordState.IsKeyDown(Keys.Z))
            {

                if (Global.Actions.Count > 0)
                {
                    UserAction UndoAction = Global.Actions.Peek();

                    if (UndoAction.Action == UserAction.ActionType.Draw)
                    {
                        foreach (var area in Global.CurrentMap.areas)
                        {
                            if (area.AreaCords.Contains(UndoAction.x, UndoAction.y))
                            {
                                area.Layers[UndoAction.Layer].TileMap[UndoAction.y - area.AreaCords.Y, UndoAction.x - area.AreaCords.X] = new Tile();
                            }
                        }
                    }
                    Global.Actions.Pop();
                }
            }

            // Check collision.
            if (Global.State == EditorState.Edit || !TestMechanics.CheckCollision())
                Global.Offset += Global.Velocity;


            // Update helper variables.
            Global.OriginalScrollWheelValue = mouseState.ScrollWheelValue;
            Global.PreviousMouseState = mouseState;
            Global.PreviousKeybordState = keyboardState;
            Global.PreviousMousePos = Global.MousePos;

            base.Update(gameTime);
        }

        // Draw
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            // Grid
            if (Global.State == EditorState.Edit)
            {
                // Draw map (all layers)
                Renderer.RenderMap(Global.CurrentMap, Global.CurrentLayer, _spriteBatch, Global.TileSheet, Global.TILE_SIZE, Global.Scale, Global.Offset, Global.ScreenWidth, Global.ScreenHeight, Global.Grid);
                Renderer.RenderGrid(_spriteBatch, Global.TILE_SIZE, Global.TileSheet, Global.Grid, Global.Scale, Global.Offset, Global.selected, Global.SelectedX, Global.SelectedY, Global.ScreenWidth, Global.ScreenHeight, Global.Selection, Global.CurrentMap, Global.CursorActionState);

                // Start point
                if (Global.CurrentMap.StartLocationSpecified)
                {
                    Rectangle DestRect = new Rectangle((int)(Global.CurrentMap.StartLocation.X * Global.TILE_SIZE * Global.Scale + Global.Offset.X), (int)(Global.CurrentMap.StartLocation.Y * Global.TILE_SIZE * Global.Scale + Global.Offset.Y), 0, 0);
                    _spriteBatch.Draw(Global.UI, new Vector2(DestRect.X, DestRect.Y), GlobalButtons.SpecifyStartPoint.SourceRect, Color.White, 0f, Vector2.Zero, (float)(32 / Global.TILE_SIZE * Global.Scale / 4), SpriteEffects.None, 0);
                }
                if (Global.A.HasValue)
                {
                    Rectangle DestRect = new Rectangle((int)(Global.A.Value.X * Global.TILE_SIZE * Global.Scale + Global.Offset.X), (int)(Global.A.Value.Y * Global.TILE_SIZE * Global.Scale + Global.Offset.Y), 0, 0);

                    _spriteBatch.Draw(Global.UI, new Vector2(DestRect.X, DestRect.Y), GlobalButtons.SpecifyDoor.SourceRect, Color.White, 0f, Vector2.Zero, (float)(32 / Global.TILE_SIZE * Global.Scale / 4), SpriteEffects.None, 0);
                }

                // Draw teleportation elements.
                foreach (var tp in Global.CurrentMap.Teleportations)
                {
                    Rectangle DestA = new Rectangle((int)(tp.A.X * Global.TILE_SIZE * Global.Scale + Global.Offset.X), (int)(tp.A.Y * Global.TILE_SIZE * Global.Scale + Global.Offset.Y), 0, 0);
                    Rectangle DestB = new Rectangle((int)(tp.B.X * Global.TILE_SIZE * Global.Scale + Global.Offset.X), (int)(tp.B.Y * Global.TILE_SIZE * Global.Scale + Global.Offset.Y), 0, 0);
                    _spriteBatch.Draw(Global.UI, new Vector2(DestA.X, DestA.Y), GlobalButtons.SpecifyDoor.SourceRect, Color.White, 0f, Vector2.Zero, (float)(32 / Global.TILE_SIZE * Global.Scale / 4), SpriteEffects.None, 0);
                    _spriteBatch.Draw(Global.UI, new Vector2(DestB.X, DestB.Y), GlobalButtons.SpecifyDoor.SourceRect, Color.White, 0f, Vector2.Zero, (float)(32 / Global.TILE_SIZE * Global.Scale / 4), SpriteEffects.None, 0);

                }
            }

            // Test state
            if (Global.State == EditorState.Test)
            {
                Renderer.DrawArea(Global.CurrentArea, Global.Offset, Global.TILE_SIZE, Global.TestingScale, Global.ScreenWidth, Global.ScreenHeight, Global.CurrentMap, _spriteBatch, Global.TileSheet);
                _spriteBatch.Draw(Global.UI, Global.CharacterRect, Global.CharacterSource, Color.White);
            }
            _spriteBatch.End();

            Rectangle orgScissorRec = _spriteBatch.GraphicsDevice.ScissorRectangle;
            RasterizerState rasterizerState = new RasterizerState() { ScissorTestEnable = true };

            // Tile palette cropping
            _spriteBatch.GraphicsDevice.ScissorRectangle = GlobalMenus.TileMenu.Destination;
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, rasterizerState);

            // Sprite palette menu
            if (Global.TilePaletteVisible)
                Renderer.DrawPalette(Global.HasTileSheet, Global.TileSpriteList, _spriteBatch, Global.selected, Global.UI, Global.TileSheet, GlobalMenus.TileMenu, Global.PaletteScrollOffset, Global.SpritePaletteDestination);

            _spriteBatch.End();
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            // UI elements
            foreach (var menu in Global.All_UI_Menus)
            {
                menu.Draw(_spriteBatch, Global.UI, Global.ScreenHeight, Global.ScreenWidth, Global.ScaleX, Global.ScaleY, Global.font, Global.TextScale, false);
            }
            _spriteBatch.End();

            // Draw scrollable menus, elements need to be cut off when out of bounds.
            foreach (var menu in Global.ScrollableMenus)
            {
                if(menu.IsVisible)
                {
                    _spriteBatch.GraphicsDevice.ScissorRectangle = menu.Destination;
                    _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, rasterizerState);
                    menu.Draw(_spriteBatch, Global.UI, Global.ScreenHeight, Global.ScreenWidth, Global.ScaleX, Global.ScaleY, Global.font, Global.TextScale, true);
                    _spriteBatch.End();
                }
            }

            _spriteBatch.GraphicsDevice.ScissorRectangle = orgScissorRec;
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            // Draw cordinates
            string Cords = "X: " + Global.SelectedX.ToString() + " Y: " + Global.SelectedY.ToString();
            _spriteBatch.DrawString(Global.font, Cords, new Vector2(96 - Global.font.MeasureString(Cords).X / 2, Global.ScreenHeight - 64), Color.White, 0f, Vector2.Zero, Global.TextScale, SpriteEffects.None, 0f);

            // TEMP
            // _spriteBatch.DrawString(font, fps.ToString(), new Vector2(32, ScreenHeight - 64), Color.White, 0f, Vector2.Zero, TextScale, SpriteEffects.None, 0f);

            // Draw cursor based on cursor state.
            if (Global.CursorActionState == CursorState.SpecifyingStartPoint)
                _spriteBatch.Draw(Global.UI, new Vector2(Global.MousePos.X - 16, Global.MousePos.Y - 16), GlobalButtons.SpecifyStartPoint.SourceRect, Color.White);
            else if (Global.CursorActionState == CursorState.SpecifyDoor)
                _spriteBatch.Draw(Global.UI, new Vector2(Global.MousePos.X - 16, Global.MousePos.Y - 16), GlobalButtons.SpecifyDoor.SourceRect, Color.White);
            else if (Global.CursorActionState == CursorState.Fill)
                _spriteBatch.Draw(Global.UI, new Vector2(Global.MousePos.X - 16, Global.MousePos.Y - 16), GlobalButtons.FillTool.SourceRect, Color.White);
            else if (Global.CursorActionState == CursorState.Eraser)
                _spriteBatch.Draw(Global.UI, new Vector2(Global.MousePos.X - 16, Global.MousePos.Y - 16), GlobalButtons.EraserTool.SourceRect, Color.White);
            else
                _spriteBatch.Draw(Global.UI, new Vector2(Global.MousePos.X - 16, Global.MousePos.Y - 16), Global.MouseSource, Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }


        // User input
        internal void HandleLeftClick(MouseState mouseState)
        {
            if (mouseState.LeftButton == ButtonState.Pressed && Global.PreviousMouseState.LeftButton != ButtonState.Pressed) // Click (Left) execute once.
            {
                bool resetCursorState = true;
                if (Global.CursorActionState == CursorState.SpecifyingStartPoint)
                {
                    foreach (var area in Global.CurrentMap.areas)
                    {
                        if (area.AreaCords.Contains(Global.SelectedX, Global.SelectedY))
                        {
                            Global.CurrentMap.StartLocation = new Point(Global.SelectedX, Global.SelectedY);
                            Global.CurrentMap.StartLocationSpecified = true;
                            Global.CurrentArea = area;
                            Global.StartArea = Global.CurrentArea;
                        }
                    }

                }
                else if (Global.CursorActionState == CursorState.SpecifyDoor)
                {

                    foreach (var area in Global.CurrentMap.areas)
                    {
                        if (area.AreaCords.Contains(Global.SelectedX, Global.SelectedY))
                        {
                            if (Global.A.HasValue)
                            {
                                Teleportation tp = new Teleportation();
                                tp.A = Global.A.Value;
                                tp.B = new Point(Global.SelectedX, Global.SelectedY);
                                Global.CurrentMap.Teleportations.Add(tp);

                                Global.A = null;

                            }
                            else
                            {
                                Global.A = new Point(Global.SelectedX, Global.SelectedY);
                                resetCursorState = false;
                            }
                        }
                    }
                }
                else
                {
                    Global.A = null;
                }

                if (resetCursorState && Global.CursorActionState != CursorState.Eraser && Global.CursorActionState != CursorState.Draw && Global.CursorActionState != CursorState.Fill)
                    Global.CursorActionState = CursorState.None;


                bool resetSelection = true;
                foreach (var UI in Global.All_UI_Menus)
                {
                    if (UI.Destination.Contains(Global.MousePos))
                    {
                        resetSelection = false;
                    }
                }

                if (resetSelection)
                {
                    Global.ClickPoint = new Point(Global.SelectedX, Global.SelectedY);
                    Global.SelectionStart = Global.ClickPoint;
                    Global.SelectionEnd = Global.SelectionStart;
                    Global.Selection.Width = 0;
                    Global.Selection.Height = 0;
                    if (GlobalButtons.ClickedAreaButton != null)
                        GlobalButtons.ClickedAreaButton.IsPressed = false;
                }
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
                if (buttonClicked == null)
                {
                    return;
                }
                else
                {
                    switch (buttonClicked.Action)
                    {
                        case ButtonAction.Import:
                            FileUtil.OpenSpriteSheetFile(Global.TileSheetPath, GraphicsDevice);
                            buttonClicked.IsVisible = false;
                            break;
                        case ButtonAction.Layer:
                            Global.CurrentLayer = buttonClicked.HelperInt;
                            if (GlobalButtons.ClickedLayerButton != null)
                                GlobalButtons.ClickedLayerButton.IsPressed = false;
                            buttonClicked.IsPressed = true;
                            GlobalButtons.ClickedLayerButton = buttonClicked;
                            GlobalLabels.LayerName.Text = "ID: " + GlobalButtons.ClickedLayerButton.Text;
                            break;
                        case ButtonAction.OpenLayerMenu:
                            Global.menuState = MenuState.LayerMenu;
                            UpdateMenuState();
                            break;
                        case ButtonAction.OpenSpriteMenu:
                            Global.menuState = MenuState.SpriteTileMenu;
                            UpdateMenuState();
                            break;
                        case ButtonAction.OpenObjectMenu:
                            Global.menuState = MenuState.ObjectMenu;
                            UpdateMenuState();
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
                            FillSelection();
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
                            break;
                        case ButtonAction.OpenAreaMenu:
                            Global.menuState = MenuState.AreaMenu;
                            UpdateMenuState();
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
                            ScrollMenuUtil.UpdateListOrder(GlobalMenus.LayerMenu);
                            break;
                        case ButtonAction.SelectCollisionSprite:
                            if (GlobalButtons.ClickedTileButton != null)
                                GlobalButtons.ClickedTileButton.IsPressed = false;
                            buttonClicked.IsPressed = true;
                            SelectTile(Global.TileSpriteList[Global.CurrentPage].LastOrDefault(obj => obj.ID == buttonClicked.Text));
                            GlobalButtons.ClickedTileButton = buttonClicked;
                            break;
                        case ButtonAction.CreateObjectLayer:
                            ObjectUtil.AddObjectLayer();
                            break;
                        case ButtonAction.CreateObject:
                            ObjectUtil.AddObject();
                            break;
                        case ButtonAction.SelectObjectLayer:
                            if(GlobalButtons.SelectedObjectLayerButton != null)
                                GlobalButtons.SelectedObjectLayerButton.IsPressed = false;
                            GlobalButtons.SelectedObjectLayerButton = buttonClicked;
                            buttonClicked.IsPressed = true;
                            ObjectUtil.ReloadObjects();
                            break;
                    }
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
                                    LabelUtil.ClearLabels(GlobalMenus.LayerLabels);
                                }

                                ScrollMenuUtil.UpdateListOrder(GlobalMenus.LayerMenu);
                                break;
                            case ButtonAction.RemoveArea:
                                Global.CurrentMap.RemoveArea(buttonClicked.HelperInt);
                                if (Global.SelectedArea.AreaName != null && Global.SelectedArea.AreaName == Global.CurrentMap.areas[buttonClicked.HelperInt - 1].AreaName)
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
                                GlobalMenus.ObjectMenu.buttons.Remove(buttonClicked);
                                Global.CurrentMap.ObjectLayers.RemoveAt(buttonClicked.HelperInt);
                                ScrollMenuUtil.UpdateListOrder(GlobalMenus.ObjectMenu);
                                break;
                            case ButtonAction.RemoveObject:
                                GlobalMenus.ObjectLabels.buttons.Remove(buttonClicked);
                                Global.CurrentMap.ObjectLayers[GlobalButtons.SelectedObjectLayerButton.HelperInt].objects.RemoveAt(buttonClicked.HelperInt);
                                ScrollMenuUtil.UpdateListOrder(GlobalMenus.ObjectLabels);
                                break;
                        }
                    }
                }
            }
        }
        internal void HandleLeftHold(MouseState mouseState, KeyboardState keyboardState)
        {
            foreach (var menu in Global.All_UI_Menus)
                if (menu.Destination.Contains(Global.MousePos))
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
                                        FillClicked();
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
        internal void HandleKeyboard(KeyboardState keyboardState, GameTime gameTime)
        {
            float Speed = Global.State == EditorState.Test ? Global.TestingSpeed : Global.MoveSpeed;
            if (keyboardState.IsKeyDown(Keys.A))
            {
                Global.Velocity.X += (float)(Speed * gameTime.ElapsedGameTime.TotalSeconds);
                Global.CharacterSource.X = 96;
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                Global.Velocity.Y -= (float)(Speed * gameTime.ElapsedGameTime.TotalSeconds);
                Global.CharacterSource.X = 0;
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                Global.Velocity.X -= (float)(Speed * gameTime.ElapsedGameTime.TotalSeconds);
                Global.CharacterSource.X = 64;
            }
            if (keyboardState.IsKeyDown(Keys.W))
            {
                Global.Velocity.Y += (float)(Speed * gameTime.ElapsedGameTime.TotalSeconds);
                Global.CharacterSource.X = 32;
            }
        }
        internal void HandleLabelDoubleClick()
        {

        }
        internal void SelectTile(SpriteTile rect)
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

        // Tools used
        internal void FillSelection()
        {
            if (Global.selected == null)
            {
                return;
            }

            foreach (var area in Global.CurrentMap.areas)
            {
                for (int i = Global.Selection.Y; i < Global.Selection.Y + Global.Selection.Height; i++)
                {
                    for (int j = Global.Selection.X; j < Global.Selection.X + Global.Selection.Width; j++)
                    {
                        if (area.AreaCords.Contains(j, i))
                        {
                            area.Layers[Global.CurrentLayer].TileMap[i - area.AreaCords.Y, j - area.AreaCords.X].ID = Global.selected.ID;
                            area.Layers[Global.CurrentLayer].TileMap[i - area.AreaCords.Y, j - area.AreaCords.X].Source = Global.selected.Source;
                        }
                    }
                }
            }
        }
        internal void FillClicked()
        {
            Area areaClicked = null;
            foreach (var area in Global.CurrentMap.areas)
            {
                if (area.AreaCords.Contains(Global.SelectedX, Global.SelectedY))
                {
                    areaClicked = area;
                    break;
                }
            }
            if (areaClicked == null)
            {
                return;
            }

            string IDToFill = areaClicked.Layers[Global.CurrentLayer].TileMap[Global.SelectedY - areaClicked.AreaCords.Y, Global.SelectedX - areaClicked.AreaCords.X].ID;

            HashSet<(int, int)> visited = new HashSet<(int, int)>();
            Queue<(int, int)> queue = new Queue<(int, int)>();
            queue.Enqueue((Global.SelectedX, Global.SelectedY));

            while (queue.Count > 0)
            {
                var (x, y) = queue.Dequeue();

                if (!areaClicked.AreaCords.Contains(x, y) || areaClicked.Layers[Global.CurrentLayer].TileMap[y - areaClicked.AreaCords.Y, x - areaClicked.AreaCords.X].ID != IDToFill || visited.Contains((x, y)))
                {
                    continue;
                }

                visited.Add((x, y));

                // Fill the current tile
                areaClicked.Layers[Global.CurrentLayer].TileMap[y - areaClicked.AreaCords.Y, x - areaClicked.AreaCords.X].ID = Global.selected.ID;
                areaClicked.Layers[Global.CurrentLayer].TileMap[y - areaClicked.AreaCords.Y, x - areaClicked.AreaCords.X].Source = Global.selected.Source;

                // Enqueue neighboring tiles
                queue.Enqueue((x + 1, y));
                queue.Enqueue((x - 1, y));
                queue.Enqueue((x, y + 1));
                queue.Enqueue((x, y - 1));
            }
        }

        // UI
        internal void UpdateMenuState()
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
                    GlobalMenus.LayerLabels.IsVisible = true;
                    GlobalButtons.LayerMenuButton.IsPressed = true;
                    GlobalButtons.AreaMenuButton.IsPressed = false;
                    GlobalButtons.ObjectMenuButton.IsPressed = false;
                    GlobalButtons.SpriteMenuButton.IsPressed = false;
                    break;
                case MenuState.AreaMenu:
                    GlobalMenus.AreaMenu.IsVisible = true;
                    GlobalMenus.AreaLabels.IsVisible = true;
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
                    GlobalMenus.ObjectMenu.IsVisible = true;
                    GlobalMenus.ObjectLabels.IsVisible = true;
                    GlobalButtons.LayerMenuButton.IsPressed = false;
                    GlobalButtons.AreaMenuButton.IsPressed = false;
                    GlobalButtons.ObjectMenuButton.IsPressed = true;
                    GlobalButtons.SpriteMenuButton.IsPressed = false;
                    break;
            }
        }
        internal void ChangeAreaProperties()
        {

        }
        internal void AddArea()
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