using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
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
            SpecifyDoor,
            None
        }

        public enum EditorState
        {
            Edit,
            Test
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
        Button SpecifyDoor;
        Button TestMap;
        Button StopTest;
        Button CollisionCheckBox;
        Button MoveLeftArea;
        Button MoveRightArea;
        Button MoveLeftLayer;
        Button MoveRightLayer;
        Button AddArea;
        Button RemoveArea;
        Button AddLayer;
        Button RemoveLayer;
        Area StartArea;
        

        Rectangle CharacterRect;
        Vector2 OriginalOffset;
        
        Label CurrentTileID;
        Label Collision;
        Label LayerName;

        Rectangle MouseSource = new Rectangle(0, 720, 32, 32);
        Rectangle MouseSourceSpecifyingPoint = new Rectangle(352, 48, 32, 32);
        Rectangle MouseSourceSpecifyingDoor = new Rectangle(352 - 32, 48, 32, 32);
        CursorState CursorActionState = Game1.CursorState.None;
        bool TilePaletteVisible;
        Point? A = null;
        Point? B = null;
        EditorState state = EditorState.Edit;
        Area CurrentArea;
        float TestingScale = 4f;
        float TestingSpeed = 516f;
        UI_Menu TileProperties;
        

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
            SpecifyDoor = new Button("", new Rectangle(96 * 6 + 128, 0, 32, 32), 352 - 32, 352 - 32, ButtonAction.SpecifyDoor, true);
            TestMap = new Button("", new Rectangle(96 * 6 + 128 + 32, 0, 32, 32), 352 + 32, 352 + 32, ButtonAction.TestState, true);
            StopTest = new Button("", new Rectangle(96 * 6 + 128 + 64, 0, 32, 32), 352 + 64, 352 + 64, ButtonAction.EditState, true);
            CollisionCheckBox = new Button("", new Rectangle(1767, 832, 32, 32), 288, 288, ButtonAction.MakeCollision, false);
            CollisionCheckBox.SourceRect.Y = 80;
            CollisionCheckBox.PressedSourceX = 320;

            MoveLeftLayer = new Button("", new Rectangle(1776, 512, 32, 32), 32, 32, ButtonAction.MoveLeftLayer, true);
            MoveLeftLayer.SourceRect.Y = 752;
            AddLayer = new Button("", new Rectangle(1776 + 32, 512, 32, 32), 96, 96, ButtonAction.AddLayer, true);
            AddLayer.SourceRect.Y = 752;
            RemoveLayer = new Button("", new Rectangle(1776 + 64, 512, 32, 32), 128, 128, ButtonAction.RemoveLayer, true);
            RemoveLayer.SourceRect.Y = 752;
            MoveRightLayer = new Button("", new Rectangle(1776 + 96, 512, 32, 32), 64, 64, ButtonAction.MoveRightLayer, true);
            MoveRightLayer.SourceRect.Y = 752;

            MoveLeftArea= new Button("", new Rectangle(1776, 256, 32, 32), 32, 32, ButtonAction.MoveRightArea, true);
            MoveLeftArea.SourceRect.Y = 752;
            AddArea = new Button("", new Rectangle(1776 + 32, 256, 32, 32), 96, 96, ButtonAction.AddArea, true);
            AddArea.SourceRect.Y = 752;
            RemoveArea = new Button("", new Rectangle(1776 + 64, 256, 32, 32), 128, 128, ButtonAction.RemoveArea, true);
            RemoveArea.SourceRect.Y = 752;
            MoveRightArea = new Button("", new Rectangle(1776 + 96, 256, 32, 32), 64, 64, ButtonAction.MoveRightArea, true);
            MoveRightArea.SourceRect.Y = 752;

            CurrentTileID = new Label();
            Collision = new Label();

            DrawTool.SourceRect.Y = 48;
            OpenPalette.SourceRect = new Rectangle(0, 656, 32, 32);
            ClosePalette.SourceRect = new Rectangle(0, 688, 32, 32);


            Offset = new Vector2(ScreenWidth/2, ScreenHeight/2);

            TileMenu = new UI_Menu(false, new Rectangle(0, 96, 288, 520), new Rectangle(0, ScreenHeight / 2 - 256, 80, 352));
            TopBar = new UI_Menu(true, new Rectangle(0, 0, 1920, 48), new Rectangle(0, 0, 1920, 48));
            GeneralOverlay = new UI_Menu(true, new Rectangle(0, 0, 0, 0), new Rectangle(0, 0, 0, 0));
            Properties = new UI_Menu(true, new Rectangle(1760, 32, 160, 1048), new Rectangle(1760, 32, 160, 0));
            TileProperties = new UI_Menu(true, new Rectangle(1768, 802, 148, 256), new Rectangle(1768, 802, 0, 0));

            LayerName = new Label();
            LayerName.LabelRect = new Rectangle(1766, 542, 150, 32);
            LayerName.SourceRect.Width = 0;
            LayerName.SourceRect.Height = 0;
            LayerName.IsVisible = true;

            CurrentTileID.LabelRect = new Rectangle(1766, 800, 150, 32);
            CurrentTileID.SourceRect.Width = 0;
            CurrentTileID.SourceRect.Height = 0;

            Collision.LabelRect = new Rectangle(1766, 800 + 32, 150, 32);
            Collision.Text = "Collision";
            Collision.SourceRect.Width = 0;
            Collision.SourceRect.Height = 0;

            for (int i = 0; i <= CurrentMap.LayerAmount; i++)
            {
                Button button = new Button("Layer: " + (i + 1).ToString(), new Rectangle(Properties.Destination.X + Properties.Destination.Width / 2 - 96 / 2, Properties.Destination.Y + 16 + 32 * i + 8 * i + 256, 96, 32), 96, 0, ButtonAction.Layer, true);
                if(i == 0)
                {
                    button.IsPressed = true;
                    ClickedLayerButton = button;
                }
                button.HelperInt = i;
                button.PressedSourceX = 96;
                Properties.buttons.Add(button);

            }

            LayerName.Text = "ID: " + ClickedLayerButton.Text;


            Properties.buttons.Add(MoveLeftLayer);
            Properties.buttons.Add(AddLayer);
            Properties.buttons.Add(RemoveLayer);
            Properties.buttons.Add(MoveRightLayer);
            Properties.buttons.Add(MoveLeftArea);
            Properties.buttons.Add(AddArea);
            Properties.buttons.Add(RemoveArea);
            Properties.buttons.Add(MoveRightArea);

            TopBar.buttons.Add(NewMap);
            TopBar.buttons.Add(SaveMap);
            TopBar.buttons.Add(LoadMap);
            TopBar.buttons.Add(EditMap);
            TopBar.buttons.Add(Settings);
            TopBar.buttons.Add(DrawTool);
            TopBar.buttons.Add(FillTool);
            TopBar.buttons.Add(EraserTool);
            TopBar.buttons.Add(SpecifyStartPoint);
            TopBar.buttons.Add(SpecifyDoor);
            TopBar.buttons.Add(TestMap);
            TopBar.buttons.Add(StopTest);
            GeneralOverlay.buttons.Add(OpenPalette);
            TileMenu.buttons.Add(Import);
            TileMenu.buttons.Add(ClosePalette);
            TileProperties.labels.Add(CurrentTileID);
            TileProperties.labels.Add(Collision);
            TileProperties.buttons.Add(CollisionCheckBox);
            Properties.labels.Add(LayerName);

            UI_Elements.Add(TileMenu);
            UI_Elements.Add(TopBar);
            UI_Elements.Add(GeneralOverlay);
            UI_Elements.Add(Properties);
            UI_Elements.Add(TileProperties);

            CharacterRect = new Rectangle(ScreenWidth / 2 - 16, ScreenHeight / 2 - 16, (int) (32 * 2f), (int)(32 * 2f));

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
                    else if(!button.IsPressed)
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
                            CurrentTileID.IsVisible = true;
                            CurrentTileID.Text = "ID: " + selected.ID;
                            Collision.IsVisible = true;
                            CollisionCheckBox.IsVisible = true;
                            CollisionCheckBox.IsPressed = selected.Collision;
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
            if (mouseState.ScrollWheelValue != OriginalScrollWheelValue && state == EditorState.Edit)
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
                Vector2 mousePosInt;
                if (state == EditorState.Test)
                {
                     mousePosInt = MousePosRelative / TestingScale / TILE_SIZE;
                }
                else
                {
                     mousePosInt = MousePosRelative / Scale / TILE_SIZE;

                }
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
                    Button btn = new Button(name, new Rectangle(Properties.Destination.X + Properties.Destination.Width / 2 - 96 / 2, Properties.Destination.Y + CurrentMap.areas.Count() * 32 - 32 + 16 + 8 * CurrentMap.areas.Count() - 8, 96, 32), 96, 0, ButtonAction.SelectArea, true);
                    btn.PressedSourceX = 96;

                    Properties.buttons.Add(btn);
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
               
            if(state == EditorState.Edit || !CheckCollision())
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

           

            // Grid
            if(state == EditorState.Edit)
            {
                // Draw map (all layers)
                Renderer.RenderMap(CurrentMap, CurrentLayer, _spriteBatch, TileSheet, TILE_SIZE, Scale, Offset, ScreenWidth, ScreenHeight, Grid);
                Renderer.RenderGrid(_spriteBatch, TILE_SIZE, TileSheet, Grid, Scale, Offset, selected, SelectedX, SelectedY, ScreenWidth, ScreenHeight, Selection, CurrentMap);
                // Start point
                if (CurrentMap.StartLocationSpecified)
                {
                    Rectangle DestRect = new Rectangle((int)(CurrentMap.StartLocation.X * TILE_SIZE * Scale + Offset.X), (int)(CurrentMap.StartLocation.Y * TILE_SIZE * Scale + Offset.Y), 0, 0);
                    _spriteBatch.Draw(UI, new Vector2(DestRect.X, DestRect.Y), new Rectangle(352, 48, 32, 32), Color.White, 0f, Vector2.Zero, (float)(32 / TILE_SIZE * Scale / 4), SpriteEffects.None, 0);
                }
                if (A.HasValue)
                {
                    Rectangle DestRect = new Rectangle((int)(A.Value.X * TILE_SIZE * Scale + Offset.X), (int)(A.Value.Y * TILE_SIZE * Scale + Offset.Y), 0, 0);

                    _spriteBatch.Draw(UI, new Vector2(DestRect.X, DestRect.Y), new Rectangle(352 - 32, 48, 32, 32), Color.White, 0f, Vector2.Zero, (float)(32 / TILE_SIZE * Scale / 4), SpriteEffects.None, 0);
                }

                foreach (var tp in CurrentMap.Teleportations)
                {
                    Rectangle DestA = new Rectangle((int)(tp.A.X * TILE_SIZE * Scale + Offset.X), (int)(tp.A.Y * TILE_SIZE * Scale + Offset.Y), 0, 0);
                    Rectangle DestB = new Rectangle((int)(tp.B.X * TILE_SIZE * Scale + Offset.X), (int)(tp.B.Y * TILE_SIZE * Scale + Offset.Y), 0, 0);
                    _spriteBatch.Draw(UI, new Vector2(DestA.X, DestA.Y), new Rectangle(352 - 32, 48, 32, 32), Color.White, 0f, Vector2.Zero, (float)(32 / TILE_SIZE * Scale / 4), SpriteEffects.None, 0);
                    _spriteBatch.Draw(UI, new Vector2(DestB.X, DestB.Y), new Rectangle(352 - 32, 48, 32, 32), Color.White, 0f, Vector2.Zero, (float)(32 / TILE_SIZE * Scale / 4), SpriteEffects.None, 0);

                }
            }

            
            if(state == EditorState.Test)
            {
                Renderer.DrawArea(CurrentArea, Offset, TILE_SIZE, TestingScale, ScreenWidth, ScreenHeight, CurrentMap, _spriteBatch, TileSheet);
                _spriteBatch.Draw(UI, CharacterRect, new Rectangle(0, 768, 32, 32), Color.White);
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
            else if(CursorActionState == CursorState.SpecifyDoor)
                _spriteBatch.Draw(UI, new Vector2(MousePos.X - 16, MousePos.Y - 16), MouseSourceSpecifyingDoor, Color.White);
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
                bool resetCursorState = true;
                if(CursorActionState == CursorState.SpecifyingStartPoint)
                {
                    foreach(var area in CurrentMap.areas)
                    {
                        if(area.AreaCords.Contains(SelectedX, SelectedY))
                        {
                            CurrentMap.StartLocation = new Point(SelectedX, SelectedY);
                            CurrentMap.StartLocationSpecified = true;
                            CurrentArea = area;
                            StartArea = CurrentArea;
                        }
                    }

                }
                else if(CursorActionState == CursorState.SpecifyDoor)
                {
                    
                    foreach (var area in CurrentMap.areas)
                    {
                        if (area.AreaCords.Contains(SelectedX, SelectedY))
                        {
                            if(A.HasValue)
                            {
                                Teleportation tp = new Teleportation();
                                tp.A = A.Value;
                                tp.B = new Point(SelectedX, SelectedY);
                                CurrentMap.Teleportations.Add(tp);

                                A = null;
                                B = null;

                            }
                            else
                            {
                                A = new Point(SelectedX, SelectedY);
                                resetCursorState = false;
                            }
                        }
                    }
                }
                else
                {
                    A = null;
                    B = null;
                }
                if(resetCursorState)
                    CursorActionState = CursorState.None;


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
                            ClickedAreaButton = buttonClicked;
                            LayerName.Text = "ID: " + ClickedAreaButton.Text;
                            // Properties.labels.FirstOrDefault(obj => obj.Text == LayerName.Text).Text = ClickedLayerButton.Text;
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
                                    
                                }
                            }
                            break;
                        case ButtonAction.SpecifyStartPoint:
                            CursorActionState = CursorState.SpecifyingStartPoint;
                            break;
                        case ButtonAction.SpecifyDoor:
                            CursorActionState = CursorState.SpecifyDoor;
                            break;
                        case ButtonAction.ClosePalette:
                            TileMenu.IsVisible = false;
                            GeneralOverlay.buttons[0].IsVisible = true;
                            TilePaletteVisible = false;
                            break;
                        case ButtonAction.TestState:
                            if(CurrentMap.StartLocationSpecified)
                                state = EditorState.Test;
                            CurrentArea = StartArea;
                            OriginalOffset = Offset;
                            Offset = new Vector2(ScreenWidth/2 - CurrentMap.StartLocation.X * TILE_SIZE * TestingScale, ScreenHeight/2 - CurrentMap.StartLocation.Y * TILE_SIZE * TestingScale);
                            break;
                        case ButtonAction.EditState:
                            state = EditorState.Edit;
                            Offset = OriginalOffset;
                            break;
                        case ButtonAction.MakeCollision:
                            buttonClicked.IsPressed = !buttonClicked.IsPressed;
                            selected.Collision = buttonClicked.IsPressed;
                            TileSpriteList[currentPage].FirstOrDefault(obj => obj.ID == selected.ID).Collision = selected.Collision;

                            CurrentMap.CollisionTiles.Clear();
                            foreach(var tile in TileSpriteList[currentPage])
                            {
                                if (tile.Collision)
                                    CurrentMap.CollisionTiles.Add(tile);
                            }

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
            float Speed = (state == EditorState.Test) ? TestingSpeed : MoveSpeed;
            if (keyboardState.IsKeyDown(Keys.A))
                Velocity.X += (float)(Speed * gameTime.ElapsedGameTime.TotalSeconds);
            if (keyboardState.IsKeyDown(Keys.S))
                Velocity.Y -= (float)(Speed * gameTime.ElapsedGameTime.TotalSeconds);
            if (keyboardState.IsKeyDown(Keys.D))
                Velocity.X -= (float)(Speed * gameTime.ElapsedGameTime.TotalSeconds);
            if (keyboardState.IsKeyDown(Keys.W))
                Velocity.Y += (float)(Speed * gameTime.ElapsedGameTime.TotalSeconds);
        }

        internal bool CheckCollision()
        {
            Area areaToSearch = null;

            int CharacterX = (int)((CharacterRect.X - (Offset.X + Velocity.X)) / TILE_SIZE / TestingScale);
            int CharacterY = (int)((CharacterRect.Y - (Offset.Y + Velocity.Y)) / TILE_SIZE / TestingScale);

            foreach (var area in CurrentMap.areas)
            {
                if(area.AreaCords.Contains(CharacterX, CharacterY))
                {
                    areaToSearch = area;
                }
            }

            if(areaToSearch != null)
            {
                for (int k = 0; k < CurrentMap.LayerAmount; k++)
                {
                    for (int i = Math.Max(CharacterY - 3, areaToSearch.AreaCords.Y); i < Math.Min(areaToSearch.AreaCords.Y + areaToSearch.AreaCords.Height, CharacterY + 3); i++)
                    {
                        for (int j = Math.Max(CharacterX - 3, areaToSearch.AreaCords.X); j < Math.Min(areaToSearch.AreaCords.X + areaToSearch.AreaCords.Width, CharacterX + 3); j++)
                        {
                            if (areaToSearch.layers[k].TileMap[i - areaToSearch.AreaCords.Y, j - areaToSearch.AreaCords.X].ID != "0")
                            {
                                var collisionTile = CurrentMap.CollisionTiles.FirstOrDefault(obj => obj.ID == areaToSearch.layers[k].TileMap[i - areaToSearch.AreaCords.Y, j - areaToSearch.AreaCords.X].ID);
                                if (collisionTile?.Collision == true)
                                {
                                    Rectangle DestRect = new Rectangle((int)(j * TILE_SIZE * TestingScale + (Offset.X + Velocity.X)), (int)(i * TILE_SIZE * TestingScale + (Offset.Y + Velocity.Y)), (int)(TILE_SIZE * TestingScale + 1), (int)(TILE_SIZE * TestingScale + 1));
                                    if (DestRect.Intersects(CharacterRect))
                                        return true;
                                }
                            }
                            if (CurrentMap.Teleportations.Count > 0)
                            {
                                foreach (var tp in CurrentMap.Teleportations)
                                {
                                    if (i == tp.A.Y && j == tp.A.X)
                                    {
                                        Rectangle TPRect = new Rectangle((int)(tp.A.X * TILE_SIZE * TestingScale + (Offset.X + Velocity.X)), (int)(tp.A.Y * TILE_SIZE * TestingScale + (Offset.Y + Velocity.Y)), (int)(TILE_SIZE * TestingScale + 1), (int)(TILE_SIZE * TestingScale + 1));
                                        if (TPRect.Intersects(CharacterRect))
                                            Offset = new Vector2(ScreenWidth / 2 - tp.B.X * TILE_SIZE * TestingScale, ScreenHeight / 2 - tp.B.Y * TILE_SIZE * TestingScale);

                                        foreach (var area in CurrentMap.areas)
                                        {
                                            if (area.AreaCords.Contains(tp.B.X, tp.B.Y))
                                            {
                                                CurrentArea = area;
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
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


