﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using tile_mapper.src.Canvas;
using tile_mapper.src.Layer;
using tile_mapper.src.UI;
using tile_mapper.src.UserSprites;
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
            Global.Timer += gameTime.ElapsedGameTime.Milliseconds;
            
            // Calculate fps
            Global.fps = (float)(1.0f / gameTime.ElapsedGameTime.TotalSeconds);

            // Keyboard.
            KeyboardState keyboardState = Keyboard.GetState();
            Global.Velocity = Vector2.Zero;
            KeyboardHandeler.HandleKeyboard(keyboardState, gameTime);

            // Mouse
            MouseState mouseState = Mouse.GetState();
            Global.MousePos = new Vector2(mouseState.X, mouseState.Y);
            Vector2 MousePosRelative = Global.MousePos - Global.Offset;
            ClickHandeler.HandleLeftHold(mouseState, keyboardState);

            if (mouseState.LeftButton == ButtonState.Pressed &&
                Global.PreviousMouseState.LeftButton == ButtonState.Released)
            {
                ClickHandeler.HandleLeftClick(mouseState, GraphicsDevice);

                if (Global.Timer - Global.TimeOfLastClick < 500)
                    ClickHandeler.HandleDoubleClick();
                else
                    Global.TimeOfLastClick = Global.Timer;
            }

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
                            SpriteUtil.SelectTile(rect);
                        else
                            rect.hovers = true;
                    }
                    else
                        rect.hovers = false;
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

            // Reset cursorstate
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
                AreaUtil.AddArea();


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

    }
}