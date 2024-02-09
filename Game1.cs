﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace tile_mapper
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public enum SelectedTool
        {
            None,
            Draw,
            Fill,
            Eraser
        }

        public enum CursorState
        {
            SpecifyingStartPoint,
            None
        }

        int TILE_SIZE = 16;
        Texture2D Grid;
        SpriteSheet SpriteSheet;
        Texture2D UI;
        float Scale = 1f;
        float OriginalScrollWheelValue = 0f;
        float MoveSpeed = 1024;
        Vector2 Velocity = Vector2.Zero;
        Vector2 Offset = Vector2.Zero;
        Button NewMap;
        Button LoadMap;
        Button EditMap;
        Button SaveMap;
        Button Import;
        Button Settings;
        SpriteFont font;
        float TextScale = 0.6f;
        float ScaleX;
        float ScaleY;
        Vector2 MousePos;
        int SelectedX;
        int SelectedY;
        string TileSheetPath = "../../../Content/Temp/tile_sheet.png";
        Texture2D TileSheet;
        UI_Menu TileMenu;
        UI_Menu TopBar;
        UI_Menu GeneralOverlay;
        UI_Menu Properties;
        List<UI_Menu> UI_Elements;
        List<List<SpriteTile>> TileSpriteList;
        bool HasTileSheet = false;
        MouseState PreviousMouseState;
        KeyboardState PreviousKeybordState;
        Button OpenPalette;
        Button ClosePalette;
        Point SelectionStart;
        Point SelectionEnd;
        Point ClickPoint;
        Rectangle Selection;
        SelectedTool Tool = SelectedTool.None;
        Button DrawTool;
        Button FillTool;
        Button EraserTool;
        Button ClickedLayerButton;
        Button ClickedAreaButton;
        Button SpecifyStartPoint;
        Rectangle MouseSource = new Rectangle(0, 720, 32, 32);
        Rectangle MouseSourceSpecifyingPoint = new Rectangle(352, 48, 32, 32);
        CursorState CursorActionState = Game1.CursorState.None;
        bool TilePaletteVisible;
        
        Stack<UserAction> Actions = new Stack<UserAction>();

        int ScreenWidth;
        int ScreenHeight;

        int SheetWidth;
        int SheetHeight;
        int SheetMenuPages;

        int currentPage = 0;

        Canvas CurrentMap = new Canvas();
        int CurrentLayer = 0;

        private float fps;

        SpriteTile selected;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = ScreenWidth;
            _graphics.PreferredBackBufferHeight = ScreenHeight;
            IsFixedTimeStep = false;


            Content.RootDirectory = "Content";
            IsMouseVisible = false;
        }

        protected override void Initialize()
        {
            // Initialize program.

            PreviousMouseState = new MouseState();
            PreviousKeybordState = new KeyboardState();
            UI_Elements = new List<UI_Menu>();

            Window.Title = "Tile-Hatter";

            ScreenWidth = 1920;
            ScreenHeight = 1080;

            _graphics.PreferredBackBufferWidth = ScreenWidth;
            _graphics.PreferredBackBufferHeight = ScreenHeight;
            _graphics.ApplyChanges();

            ScaleX = 1f;
            ScaleY = 1f;

            LoadMap = new Button("Load", new Rectangle(96, 0, 96, 32), 96, 0, ButtonAction.None, true);
            NewMap = new Button("New", new Rectangle(0, 0, 96, 32), 96, 0, ButtonAction.None, true);
            EditMap = new Button("Edit", new Rectangle(96 * 3, 0, 96, 32), 96, 0, ButtonAction.None, true);
            SaveMap = new Button("Save", new Rectangle(96 * 2, 0, 96, 32), 96, 0, ButtonAction.Save, true);
            Import = new Button("Import", new Rectangle(96, ScreenHeight / 2 - 16, 96, 32), 96, 0, ButtonAction.Import, true);
            Settings = new Button("Settings ", new Rectangle(96 * 4, 0, 96, 32), 96, 0, ButtonAction.None, true);
            OpenPalette = new Button("", new Rectangle(0, ScreenHeight / 2 - 32 / 2, 32, 32), 32, 0, ButtonAction.OpenPalette, true);
            ClosePalette = new Button("", new Rectangle(272, ScreenHeight / 2 - 32 / 2, 32, 32), 32, 0, ButtonAction.ClosePalette, true);
            DrawTool = new Button("", new Rectangle(96 * 6, 0, 32, 32), 192, 192, ButtonAction.DrawTool, true);
            FillTool = new Button("", new Rectangle(96 * 6 + 32, 0, 32, 32), 192 + 32, 192 + 32, ButtonAction.FillTool, true);
            EraserTool = new Button("", new Rectangle(96 * 6 + 64, 0, 32, 32), 192 + 64, 192 + 64, ButtonAction.EraserTool, true);
            SpecifyStartPoint = new Button("", new Rectangle(96 * 6 + 96, 0, 32, 32), 352, 352, ButtonAction.SpecifyStartPoint, true);

            DrawTool.SourceRect.Y = 48;
            OpenPalette.SourceRect = new Rectangle(0, 656, 32, 32);
            ClosePalette.SourceRect = new Rectangle(0, 688, 32, 32);


            Offset = new Vector2(ScreenWidth/2, ScreenHeight/2);

            TileMenu = new UI_Menu(false, new Rectangle(0, 96, 288, 520), new Rectangle(0, ScreenHeight / 2 - 256, 80, 352));
            TopBar = new UI_Menu(true, new Rectangle(0, 0, 1920, 48), new Rectangle(0, 0, 1920, 48));
            GeneralOverlay = new UI_Menu(true, new Rectangle(0, 0, 0, 0), new Rectangle(0, 0, 0, 0));
            Properties = new UI_Menu(true, new Rectangle(1760, 32, 160, 1048), new Rectangle(1760, 32, 160, 0));

            for(int i = 0; i <= CurrentMap.LayerAmount; i++)
            {
                Button button = new Button("Layer: " + (i + 1).ToString(), new Rectangle(Properties.Destination.X + Properties.Destination.Width / 2 - 96 / 2, Properties.Destination.Y + 16 + 32 * i + 8 * i + 256, 96, 32), 96, 0, ButtonAction.Layer, true);
                button.HelperInt = i;
                Properties.buttons.Add(button);
            }

            TopBar.buttons.Add(NewMap);
            TopBar.buttons.Add(SaveMap);
            TopBar.buttons.Add(LoadMap);
            TopBar.buttons.Add(EditMap);
            TopBar.buttons.Add(Settings);
            TopBar.buttons.Add(DrawTool);
            TopBar.buttons.Add(FillTool);
            TopBar.buttons.Add(EraserTool);
            TopBar.buttons.Add(SpecifyStartPoint);
            GeneralOverlay.buttons.Add(OpenPalette);
            TileMenu.buttons.Add(Import);
            TileMenu.buttons.Add(ClosePalette);

            UI_Elements.Add(TileMenu);
            UI_Elements.Add(TopBar);
            UI_Elements.Add(GeneralOverlay);
            UI_Elements.Add(Properties);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            SpriteSheet = new SpriteSheet(TILE_SIZE);
            SpriteSheet.Texture = Content.Load<Texture2D>("tile_sheet");

            using (FileStream stream = new FileStream("../../../Content/UI.png", FileMode.Open))
            {
                UI = Texture2D.FromStream(GraphicsDevice, stream);
            }
            //UI = Content.Load<Texture2D>("UI");
            Grid = Content.Load<Texture2D>("grid");
            font = Content.Load<SpriteFont>("font");
        }

        protected override void Update(GameTime gameTime)
        {
            // Calculate fps
            fps = (float)(1.0f / gameTime.ElapsedGameTime.TotalSeconds);

            // Keyboard.
            KeyboardState keyboardState = Keyboard.GetState();
            Velocity = Vector2.Zero;
            HandleKeyboard(keyboardState, gameTime);

            // Mouse
            MouseState mouseState = Mouse.GetState();
            MousePos = new Vector2(mouseState.X, mouseState.Y);
            Vector2 MousePosRelative = MousePos - Offset;
            HandleLeftClick(mouseState);
            HandleLeftHold(mouseState, keyboardState);

            // Check if mouse is hoovering on button.
            foreach (var UI in UI_Elements)
            {
                foreach (var button in UI.buttons)
                    if(button != null && !button.IsPressed)
                        button.ChangeSourceX(MousePos);
                    else if(button.IsPressed)
                    {
                        button.SourceRect.X = button.SelectionX;
                    }
            }

            // Tile sheet is imported.
            if(HasTileSheet)
            {
                foreach (var rect in TileSpriteList[currentPage])
                {
                    // User is hoovering on sprite tile.
                    if (rect.Destination.Contains(MousePos))
                    {
                        // User selected a sprite.
                        if (mouseState.LeftButton == ButtonState.Pressed)
                        {
                            selected = rect;
                            selected.ID = rect.ID;
                            selected.Source = rect.Source;
                            Tool = SelectedTool.Draw;
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
            // User is scrolling (zooming on map)
            if (mouseState.ScrollWheelValue != OriginalScrollWheelValue)
            {
                Vector2 Center = new Vector2(Offset.X, Offset.Y);
                Vector2 MouseBefore = (MousePos - Offset)/Scale;

                float adjustment = (mouseState.ScrollWheelValue - OriginalScrollWheelValue) * 0.0004f;
                // Adjust the scaling factor based on the scroll wheel delta
                Scale += adjustment;
                Scale = MathHelper.Clamp(Scale, 0.5f, 5f);
                // Vector2 MouseAfter = MouseBefore * Scale;

                Vector2 CenterNew = new Vector2(Offset.X, Offset.Y );

                Offset += ((Center - CenterNew) / 2);
                Vector2 mouseAfter = (new Vector2(mouseState.X, mouseState.Y) - Offset) / Scale;

                Vector2 mousePositionDifference = MouseBefore - mouseAfter;
                // Adjust the offset to keep the mouse position stationary
                Offset -= mousePositionDifference * Scale;
            }


            // Only update the selected square if user is not using scroll wheel.
            if (mouseState.ScrollWheelValue == OriginalScrollWheelValue)
            {
                Vector2 mousePosInt = MousePosRelative / Scale / TILE_SIZE;
                SelectedX = (int)Math.Floor(mousePosInt.X);
                SelectedY = (int)Math.Floor(mousePosInt.Y);
            }

            if (keyboardState.IsKeyDown(Keys.Enter) && !PreviousKeybordState.IsKeyDown(Keys.Enter) && Selection.Width >= 4 && Selection.Height >= 4)
            {
                bool allowed = true;
                foreach(var area in CurrentMap.areas)
                {
                    if(area.AreaCords.Intersects(Selection))
                    {
                        allowed = false;
                    }
                }
                if(allowed)
                {
                    string name = "Area: " + (CurrentMap.areas.Count() + 1).ToString();
                    CurrentMap.CreateArea(Selection, name);
                    Properties.buttons.Add(new Button(name, new Rectangle(Properties.Destination.X + Properties.Destination.Width / 2 - 96 / 2, Properties.Destination.Y + CurrentMap.areas.Count() * 32 - 32 + 16 + 8* CurrentMap.areas.Count() - 8, 96, 32), 96, 0, ButtonAction.SelectArea, true));

                }
            }

            // Undo last action.
            if(keyboardState.IsKeyDown(Keys.LeftControl) && keyboardState.IsKeyDown(Keys.Z) && !PreviousKeybordState.IsKeyDown(Keys.Z))
            {

                //if(Actions.Count > 0)
                //{
                //    UserAction UndoAction = Actions.Peek();

                //    if(UndoAction.Action == UserAction.ActionType.Draw)
                //        CurrentMap.layers[UndoAction.Layer].TileMap[UndoAction.y, UndoAction.x] = new Tile();

                //    Actions.Pop();
                //}
            }
               

            Offset += Velocity;
            OriginalScrollWheelValue = mouseState.ScrollWheelValue;
            PreviousMouseState = mouseState;
            PreviousKeybordState = keyboardState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DimGray);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            // Draw map (all layers)
            Renderer.RenderMap(CurrentMap, CurrentLayer, _spriteBatch, TileSheet, TILE_SIZE, Scale, Offset, ScreenWidth, ScreenHeight, Grid);

            // Grid 
            Renderer.RenderGrid(_spriteBatch, TILE_SIZE, TileSheet, Grid, Scale, Offset, selected, SelectedX, SelectedY, ScreenWidth, ScreenHeight, Selection, CurrentMap);

            // Start point
            if(CurrentMap.StartLocationSpecified)
            {
                Rectangle DestRect = new Rectangle((int)(CurrentMap.StartLocation.X * TILE_SIZE * Scale + Offset.X), (int)(CurrentMap.StartLocation.Y * TILE_SIZE * Scale + Offset.Y), 0, 0);
                _spriteBatch.Draw(UI, new Vector2(DestRect.X, DestRect.Y), new Rectangle(352, 48, 32, 32), Color.White, 0f, Vector2.Zero, (float) (32 / TILE_SIZE * Scale / 4), SpriteEffects.None, 0);
            }

            // UI elements
            foreach (var menu in UI_Elements)
            {
                menu.Draw(_spriteBatch, UI, ScreenHeight, ScreenWidth, ScaleX, ScaleY, font, TextScale);
            }


            // Sprite palette menu
            if(TilePaletteVisible)
                Renderer.DrawPalette(HasTileSheet, TileSpriteList, _spriteBatch, selected, Grid, TileSheet);

            // Draw cordinates
            string Cords = "X: " + SelectedX.ToString() + " Y: " + SelectedY.ToString();
            _spriteBatch.DrawString(font, Cords, new Vector2(96 - font.MeasureString(Cords).X/2, 32  + font.MeasureString(Cords).Y / 2), Color.White);

            // TEMP
            _spriteBatch.DrawString(font, fps.ToString(), new Vector2(32, ScreenHeight - 64), Color.White);

            if(CursorActionState == CursorState.SpecifyingStartPoint)
                _spriteBatch.Draw(UI, new Vector2(MousePos.X - 16, MousePos.Y - 16), MouseSourceSpecifyingPoint, Color.White);
            else
                _spriteBatch.Draw(UI, new Vector2(MousePos.X - 16, MousePos.Y - 16), MouseSource, Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }


        internal void WriteFile()
        {
            
        }

        internal void OpenFile(string path)
        {
            using (FileStream stream = new FileStream(TileSheetPath, FileMode.Open))
            {
                TileSheet = Texture2D.FromStream(GraphicsDevice, stream);
            }

            SheetWidth = TileSheet.Width / TILE_SIZE;
            SheetHeight = TileSheet.Height / TILE_SIZE;

            SheetMenuPages = (int)Math.Ceiling((float)(SheetWidth * SheetHeight / 120));
            SheetMenuPages++;

            TileSpriteList = new List<List<SpriteTile>>();

            for (int i = 0; i < SheetMenuPages; i++)
            {
                List<SpriteTile> page = new List<SpriteTile>();

                for (int y = 0; y < SheetHeight; y++)
                {
                    for (int x = 0; x < SheetWidth; x++)
                    {
                        int xcord = x * TILE_SIZE;
                        int ycord = y * TILE_SIZE;

                        SpriteTile tile = new SpriteTile();
                        tile.Source = new Rectangle(xcord, ycord, TILE_SIZE, TILE_SIZE);

                        tile.Destination = new Rectangle();

                        tile.ID = "X" + x.ToString() + "Y" + y.ToString();

                        page.Add(tile);
                    }
                }

                for (int j = 0; j < SheetHeight * SheetWidth; j++)
                {
                    int x = TileMenu.Destination.X + 16 + (j % 8) * TILE_SIZE * 2;
                    int y = TileMenu.Destination.Y + 16 + (j / 8) * TILE_SIZE * 2;

                    page[j].Destination = new Rectangle(x, y, TILE_SIZE * 2, TILE_SIZE * 2);
                }

                TileSpriteList.Add(page);
            }

            HasTileSheet = true;

            System.Diagnostics.Debug.WriteLine(TileSheetPath);
        }

        internal void HandleLeftClick(MouseState mouseState)
        {
            if (mouseState.LeftButton == ButtonState.Pressed && PreviousMouseState.LeftButton != ButtonState.Pressed) // Click (Left) execute once.
            {
                if(CursorActionState == CursorState.SpecifyingStartPoint)
                {
                    CursorActionState = CursorState.None;
                    foreach(var area in CurrentMap.areas)
                    {
                        if(area.AreaCords.Contains(SelectedX, SelectedY))
                        {
                            CurrentMap.StartLocation = new Point(SelectedX, SelectedY);
                            CurrentMap.StartLocationSpecified = true;
                        }
                    }
                }
                

                bool resetSelection = true;
                foreach(var UI in UI_Elements)
                {
                    if(UI.Destination.Contains(MousePos))
                    {
                        resetSelection = false;
                    }
                }

                if(resetSelection)
                {
                    ClickPoint = new Point(SelectedX, SelectedY);
                    SelectionStart = ClickPoint;
                    SelectionEnd = SelectionStart;
                    Selection.Width = 0;
                    Selection.Height = 0;
                    if(ClickedAreaButton != null)
                        ClickedAreaButton.IsPressed = false;
                }
                Button buttonClicked = null;

                foreach (var UI in UI_Elements)
                {
                    
                    if (UI.IsVisible)
                    {
                        buttonClicked = UI.HandleClicks(MousePos);
                        if (buttonClicked != null)
                            break;
                    }
                }
                if (buttonClicked == null)
                {
                    return;
                }
                else
                    switch (buttonClicked.Action)
                    {
                        case ButtonAction.Import:
                            OpenFile(TileSheetPath);
                            buttonClicked.IsVisible = false;
                            break;
                        case ButtonAction.Layer:
                            CurrentLayer = buttonClicked.HelperInt;
                            if(ClickedLayerButton != null)
                                ClickedLayerButton.IsPressed = false;
                            buttonClicked.IsPressed = true;
                            ClickedLayerButton = buttonClicked;
                            break;
                        case ButtonAction.Save:
                            WriteFile();
                            break;
                        case ButtonAction.OpenPalette:
                            TileMenu.IsVisible = true;
                            GeneralOverlay.buttons[0].IsVisible = false;
                            TilePaletteVisible = true;
                            break;
                        case ButtonAction.DrawTool:
                            Tool = SelectedTool.Draw;
                            break;
                        case ButtonAction.FillTool:
                            Tool = SelectedTool.Fill;
                            FillSelection();
                            break;
                        case ButtonAction.EraserTool:
                            Tool = SelectedTool.Eraser;
                            break;
                        case ButtonAction.SelectArea:
                            foreach (var area in CurrentMap.areas)
                            {
                                if (area.AreaName == buttonClicked.Text)
                                {
                                    Selection = area.AreaCords;
                                    buttonClicked.IsPressed = true;
                                    if(ClickedAreaButton != null)
                                        ClickedAreaButton.IsPressed = false;
                                    ClickedAreaButton = buttonClicked;
                                }
                            }
                            break;
                        case ButtonAction.SpecifyStartPoint:
                            CursorActionState = CursorState.SpecifyingStartPoint;
                            break;
                        case ButtonAction.ClosePalette:
                            TileMenu.IsVisible = false;
                            GeneralOverlay.buttons[0].IsVisible = true;
                            TilePaletteVisible = false;
                            break;
                    }
            }
        }

        internal void HandleLeftHold(MouseState mouseState, KeyboardState keyboardState)
        {
            // Execute each frame if mouse button is held.
            if (mouseState.LeftButton == ButtonState.Pressed && selected != null)
            {
                foreach(var area in CurrentMap.areas)
                {
                    if(area.AreaCords.Contains(SelectedX, SelectedY))
                    {
                        switch (Tool)
                        {
                            case SelectedTool.Draw:
                                area.layers[CurrentLayer].TileMap[SelectedY-area.AreaCords.Y, SelectedX-area.AreaCords.X].ID = selected.ID;
                                area.layers[CurrentLayer].TileMap[SelectedY - area.AreaCords.Y, SelectedX - area.AreaCords.X].Source = selected.Source;
                                Actions.Push(new UserAction(UserAction.ActionType.Draw, CurrentLayer, SelectedX, SelectedY));
                                break;
                            case SelectedTool.Eraser:
                                area.layers[CurrentLayer].TileMap[SelectedY - area.AreaCords.Y, SelectedX - area.AreaCords.X].ID = "0";
                                area.layers[CurrentLayer].TileMap[SelectedY - area.AreaCords.Y, SelectedX - area.AreaCords.X].Source = new Rectangle();
                                break;
                            case SelectedTool.Fill:
                                if(PreviousMouseState.LeftButton != ButtonState.Pressed) // Exception
                                {
                                    bool allowed = true;
                                    foreach (var UI in UI_Elements)
                                    {
                                        if (UI.Destination.Contains(MousePos))
                                            allowed = false;
                                    }
                                    if(allowed)
                                        FillClicked();
                                }
                                break;
                        }
                    }
                }
            }
            if (mouseState.LeftButton == ButtonState.Pressed && keyboardState.IsKeyDown(Keys.LeftShift) && SelectionStart.X >= 0 && SelectionStart.Y >= 0 && (SelectedX != ClickPoint.X || SelectedY != ClickPoint.Y))
            {
                SelectionEnd = new Point(SelectedX, SelectedY);
                // Create a square of the selection
                Point TopLeft = new Point(Math.Min(SelectionStart.X, SelectionEnd.X), Math.Min(SelectionStart.Y, SelectionEnd.Y));
                Point BottomRight = new Point(Math.Max(SelectionStart.X, SelectionEnd.X), Math.Max(SelectionStart.Y, SelectionEnd.Y));
                Selection = new Rectangle(TopLeft.X, TopLeft.Y, BottomRight.X - TopLeft.X + 1, BottomRight.Y - TopLeft.Y + 1);
            }
        }

        internal void HandleKeyboard(KeyboardState keyboardState, GameTime gameTime)
        {
            if (keyboardState.IsKeyDown(Keys.A))
                Velocity.X += (float)(MoveSpeed * gameTime.ElapsedGameTime.TotalSeconds);
            if (keyboardState.IsKeyDown(Keys.S))
                Velocity.Y -= (float)(MoveSpeed * gameTime.ElapsedGameTime.TotalSeconds);
            if (keyboardState.IsKeyDown(Keys.D))
                Velocity.X -= (float)(MoveSpeed * gameTime.ElapsedGameTime.TotalSeconds);
            if (keyboardState.IsKeyDown(Keys.W))
                Velocity.Y += (float)(MoveSpeed * gameTime.ElapsedGameTime.TotalSeconds);
        }

        internal void FillSelection()
        {
            if(selected == null)
            {
                return;
            }
        
            foreach(var area in CurrentMap.areas)
            {
                for (int i = Selection.Y; i < Selection.Y + Selection.Height; i++)
                {
                    for (int j = Selection.X; j < Selection.X + Selection.Width; j++)
                    {
                        if (area.AreaCords.Contains(j, i))
                        {
                            area.layers[CurrentLayer].TileMap[i - area.AreaCords.Y, j - area.AreaCords.X].ID = selected.ID;
                            area.layers[CurrentLayer].TileMap[i - area.AreaCords.Y, j - area.AreaCords.X].Source = selected.Source;
                        }
                    }
                }
            } 
        }

        internal void FillClicked()
            {
                Area areaClicked = null;
                foreach (var area in CurrentMap.areas)
                {
                    if (area.AreaCords.Contains(SelectedX, SelectedY))
                    {
                        areaClicked = area;
                        break;
                    }
                }
                if (areaClicked == null)
                {
                    return;
                }

                string IDToFill = areaClicked.layers[CurrentLayer].TileMap[SelectedY - areaClicked.AreaCords.Y, SelectedX - areaClicked.AreaCords.X].ID;

                HashSet<(int, int)> visited = new HashSet<(int, int)>();
                Queue<(int, int)> queue = new Queue<(int, int)>();
                queue.Enqueue((SelectedX, SelectedY));

                while (queue.Count > 0)
                {
                    var (x, y) = queue.Dequeue();

                    if (!areaClicked.AreaCords.Contains(x, y) || areaClicked.layers[CurrentLayer].TileMap[y - areaClicked.AreaCords.Y, x - areaClicked.AreaCords.X].ID != IDToFill || visited.Contains((x, y)))
                    {
                        continue;
                    }

                    visited.Add((x, y));

                    // Fill the current tile
                    areaClicked.layers[CurrentLayer].TileMap[y - areaClicked.AreaCords.Y, x - areaClicked.AreaCords.X].ID = selected.ID;
                    areaClicked.layers[CurrentLayer].TileMap[y - areaClicked.AreaCords.Y, x - areaClicked.AreaCords.X].Source = selected.Source;

                    // Enqueue neighboring tiles
                    queue.Enqueue((x + 1, y));
                    queue.Enqueue((x - 1, y));
                    queue.Enqueue((x, y + 1));
                    queue.Enqueue((x, y - 1));
                }
            }

        }
}


