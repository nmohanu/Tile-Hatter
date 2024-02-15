using Microsoft.Xna.Framework;
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

        // Temp
        int TILE_SIZE = 16;

        GameTime DoubleClickTimer;
        float TimeSinceLastClick;

        RenderTarget2D ScrollMenuBounds;

        Texture2D Grid;
        SpriteSheet SpriteSheet;
        Texture2D UI;
        float Scale = 1f;
        float OriginalScrollWheelValue = 0f;
        float MoveSpeed = 1024;
        Vector2 Velocity = Vector2.Zero;
        Vector2 Offset = Vector2.Zero;
        Button Import;
        SpriteFont font;
        float TextScale = 0.6f;
        float ScaleX;
        float ScaleY;
        Vector2 MousePos;
        int SelectedX;
        int SelectedY;
        string TileSheetPath = "../../../Content/Temp/tile_sheet.png";
        Texture2D TileSheet;

        // UI Elements
        UI_Menu TileMenu;
        UI_Menu TopBar;
        UI_Menu GeneralOverlay;
        UI_Menu Properties;

        UI_Menu LayerMenu;
        UI_Menu AreaMenu;
        UI_Menu ObjectMenu;

        List<UI_Menu> UI_Elements;
        List<UI_Menu> Scrollable_Menus = new List<UI_Menu>();
        List<UI_Menu> LabelMenus = new List<UI_Menu>();

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
        Button ObjectButton;
        Button WorldScreen;
        Button SheetScreen;
        Button RuleSetScreen;
        Button AreaMenuButton;
        Button LayerMenuButton;
        Button ObjectMenuButton;
        Button SpriteMenuButton;
        Button CreateLayerButton;
        Button ClickedTileButton;


        Area StartArea;
        Area SelectedArea;

        List<UI_Menu> PropertyMenu1 = new List<UI_Menu>();


        Rectangle CharacterSource = new Rectangle(0, 864, 32, 32);
        

        Rectangle CharacterRect;
        Vector2 OriginalOffset;
        
        Label CurrentTileID;
        Label Collision;
        Label LayerName;
        Label AreaName;
        Label AreaWidth;
        Label AreaHeight;
        Label AreaX;
        Label AreaY;

        Rectangle MouseSource = new Rectangle(0, 784, 32, 32);
        CursorState CursorActionState = Game1.CursorState.None;
        bool TilePaletteVisible;
        Point? A = null;
        EditorState state = EditorState.Edit;
        Area CurrentArea;
        float TestingScale = 4f;
        float TestingSpeed = 516f;
        UI_Menu CollisionSpriteList;

        UI_Menu TileLabels;
        UI_Menu AreaLabels;
        UI_Menu LayerLabels;
        UI_Menu ObjectLabels;

        MenuState menuState;

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

            Import = new Button("Import", new Rectangle(144 - 128/2, ScreenHeight / 2 - 24, 128, 48), 288, 64, ButtonAction.Import, true);
            Import.SourceRect.Y = 128;
            OpenPalette = new Button("", new Rectangle(0, ScreenHeight / 2 - 32 / 2, 32, 32), 32, 0, ButtonAction.OpenPalette, true);
            ClosePalette = new Button("", new Rectangle(272, ScreenHeight / 2 - 32 / 2, 32, 32), 32, 0, ButtonAction.ClosePalette, true);

            DrawTool = new Button("", new Rectangle(0, 32, 32, 32), 160, 160, ButtonAction.DrawTool, true);
            DrawTool.SourceRect.Y = 96;

            FillTool = new Button("", new Rectangle(32, 32, 32, 32), 160 + 32, 160 + 32, ButtonAction.FillTool, true);
            FillTool.SourceRect.Y = 96;

            EraserTool = new Button("", new Rectangle(64, 32, 32, 32), 160 + 64, 160 + 64, ButtonAction.EraserTool, true);
            EraserTool.SourceRect.Y = 96;

            SpecifyStartPoint = new Button("", new Rectangle(96, 32, 32, 32), 288 + 32, 288 + 32, ButtonAction.SpecifyStartPoint, true);
            SpecifyStartPoint.SourceRect.Y = 96;

            SpecifyDoor = new Button("", new Rectangle(128, 32, 32, 32), 288, 288, ButtonAction.SpecifyDoor, true);
            SpecifyDoor.SourceRect.Y = 96;

            ObjectButton = new Button("", new Rectangle(160, 0, 32, 32), 368, 368, ButtonAction.OpenObjectMenu, true);
            ObjectButton.SourceRect.Y = 80;

            TestMap = new Button("", new Rectangle(ScreenWidth/2 -32, 0, 32, 32), 0, 0, ButtonAction.TestState, true);
            TestMap.SourceRect.Y = 128;
            StopTest = new Button("", new Rectangle(ScreenWidth/2, 0, 32, 32), 32,32, ButtonAction.EditState, true);
            StopTest.SourceRect.Y = 128;


            WorldScreen = new Button("Editor", new Rectangle(0, 0, 144, 32), 304, 304, ButtonAction.EditorScreen, true);
            WorldScreen.IsPressed = true;
            WorldScreen.PressedSourceX = 448;
            WorldScreen.SourceRect.Y = 192;

            SheetScreen = new Button("Sprite Sheet", new Rectangle(144, 0, 144, 32), 304, 304, ButtonAction.SheetScreen, true);
            SheetScreen.IsPressed = false;
            SheetScreen.PressedSourceX = 448;
            SheetScreen.SourceRect.Y = 192;

            RuleSetScreen = new Button("Rule sets", new Rectangle(288, 0, 144, 32), 304, 304, ButtonAction.SheetScreen, true);
            RuleSetScreen.IsPressed = false;
            RuleSetScreen.PressedSourceX = 448;
            RuleSetScreen.SourceRect.Y = 192;

            LayerMenuButton = new Button("", new Rectangle(1660, 1044 - 64, 32, 32), 1680 + 32, 1680, ButtonAction.OpenLayerMenu, true);
            LayerMenuButton.IsPressed = true;
            LayerMenuButton.SourceRect.Y = 1184;
            LayerMenuButton.PressedSourceX = 1648;

            AreaMenuButton = new Button("", new Rectangle(1660 + 32, 1044 - 64, 32, 32), 1680 + 32, 1680, ButtonAction.OpenAreaMenu, true);
            AreaMenuButton.IsPressed = false;
            AreaMenuButton.SourceRect.Y = 1184 + 32;
            AreaMenuButton.PressedSourceX = 1648;

            ObjectMenuButton = new Button("", new Rectangle(1660 + 64, 1044 - 64, 32, 32), 1680 + 32, 1680, ButtonAction.OpenObjectMenu, true);
            ObjectMenuButton.IsPressed = false;
            ObjectMenuButton.SourceRect.Y = 1184 + 64;
            ObjectMenuButton.PressedSourceX = 1648;

            SpriteMenuButton = new Button("", new Rectangle(1660 + 96, 1044 - 64, 32, 32), 1680 + 32, 1680, ButtonAction.OpenSpriteMenu, true);
            SpriteMenuButton.IsPressed = false;
            SpriteMenuButton.SourceRect.Y = 1184 + 96;
            SpriteMenuButton.PressedSourceX = 1648;

            

            CurrentTileID = new Label();
            Collision = new Label();

            OpenPalette.SourceRect = new Rectangle(0, 720, 32, 32);
            ClosePalette.SourceRect = new Rectangle(0, 752, 32, 32);


            Offset = new Vector2(ScreenWidth/2, ScreenHeight/2);

            TileMenu = new UI_Menu(false, new Rectangle(0, 192, 288, 520), new Rectangle(0, ScreenHeight / 2 - 256, 80, 352));
            TopBar = new UI_Menu(true, new Rectangle(0, 0, 1920, 80), new Rectangle(0, 0, 1920, 67));
            GeneralOverlay = new UI_Menu(true, new Rectangle(0, 0, 0, 0), new Rectangle(0, 0, 0, 0));
            Properties = new UI_Menu(true, new Rectangle(1655, 64, 266, 1080), new Rectangle(1655, 0, 266, 1080));
            LayerMenu = new UI_Menu(true, new Rectangle(1655, 0, 0, 0), new Rectangle(1660, 32, 256, 496));
            LayerMenu.Scrollable = true;
            AreaMenu = new UI_Menu(false, new Rectangle(1760, 32, 0, 0), new Rectangle(1660, 32, 256, 496));
            AreaMenu.Scrollable = true;
            ObjectMenu = new UI_Menu(false, new Rectangle(1760, 32, 0, 0), new Rectangle(1660, 32, 256, 496));
            ObjectMenu.Scrollable = true;
            CollisionSpriteList = new UI_Menu(false, new Rectangle(1768, 802, 0, 0), new Rectangle(1660, 32, 256, 496));
            CollisionSpriteList.Scrollable = true;

            TileLabels = new UI_Menu(false, new Rectangle(1768, 802, 0, 0), new Rectangle(1660, 32, 256, 496));
            AreaLabels = new UI_Menu(false, new Rectangle(1768, 802, 0, 0), new Rectangle(1660, 32, 256, 496));
            LayerLabels = new UI_Menu(true, new Rectangle(1768, 802, 0, 0), new Rectangle(1660, 32, 256, 496));
            ObjectLabels = new UI_Menu(false, new Rectangle(1768, 802, 0, 0), new Rectangle(1660, 32, 256, 496));

            LayerName = new Label();
            LayerName.LabelRect = new Rectangle(1660, 624 - 64, 256, 32);
            LayerName.IsVisible = true;

            AreaName = new Label();
            AreaName.LabelRect = new Rectangle(1660, 624 - 32 - 32, 256, 32);
            AreaName.IsVisible = true;

            AreaWidth = new Label();
            AreaWidth.LabelRect = new Rectangle(1660, 624 - 32, 256, 32);
            AreaWidth.IsVisible = true;

            AreaHeight = new Label();
            AreaHeight.LabelRect = new Rectangle(1660, 624, 256, 32);
            AreaHeight.IsVisible = true;

            AreaX = new Label();
            AreaX.LabelRect = new Rectangle(1660, 624 + 32, 256, 32);
            AreaX.IsVisible = true;

            AreaY = new Label();
            AreaY.LabelRect = new Rectangle(1660, 624 + 64, 256, 32);
            AreaY.IsVisible = true;

            // Sprite tile properties.
            CurrentTileID.LabelRect = new Rectangle(1660, 624 - 32 - 32, 256, 32);
            CurrentTileID.SourceRect.Width = 0;
            CurrentTileID.SourceRect.Height = 0;

            Collision.LabelRect = new Rectangle(1660, 624 - 32, 256, 32);
            Collision.Text = "Collision";
            Collision.SourceRect.Width = 0;
            Collision.SourceRect.Height = 0;

            CollisionCheckBox = new Button("", new Rectangle(1660, 656 - 64, 32, 32), 32, 0, ButtonAction.MakeCollision, false);
            CollisionCheckBox.SourceRect.Y = 80;
            CollisionCheckBox.PressedSourceX = 64;


            // Layer menu.
            CreateLayerButton = new Button("New layer", new Rectangle(Properties.Destination.X + Properties.Destination.Width / 2 - 224 / 2, Properties.Destination.Y + 32 + 16 + 48 * 3, 224, 48), 528, 304, ButtonAction.AddLayer, true);
            CreateLayerButton.SourceRect.Y = 240;
            LayerMenu.buttons.Add(CreateLayerButton);

            // Add the default 3 layers.
            for (int i = 1; i <= 3; i++)
            {
                AddLayer();
            }

            LayerMenu.buttons[0].IsPressed = true;
            ClickedLayerButton = LayerMenu.buttons[0];

            

            LayerName.Text = "ID: " + ClickedLayerButton.Text;

            TopBar.buttons.Add(DrawTool);
            TopBar.buttons.Add(FillTool);
            TopBar.buttons.Add(EraserTool);
            TopBar.buttons.Add(SpecifyStartPoint);
            TopBar.buttons.Add(SpecifyDoor);
            TopBar.buttons.Add(TestMap);
            TopBar.buttons.Add(StopTest);
            TopBar.buttons.Add(ObjectButton);
            TopBar.buttons.Add(WorldScreen);
            TopBar.buttons.Add(SheetScreen);
            TopBar.buttons.Add(RuleSetScreen);
            GeneralOverlay.buttons.Add(OpenPalette);
            TileMenu.buttons.Add(Import);
            TileMenu.buttons.Add(ClosePalette);

            Properties.buttons.Add(LayerMenuButton);
            Properties.buttons.Add(ObjectMenuButton);
            Properties.buttons.Add(AreaMenuButton);
            Properties.buttons.Add(SpriteMenuButton);


            TileLabels.labels.Add(CurrentTileID);
            TileLabels.labels.Add(Collision);
            TileLabels.buttons.Add(CollisionCheckBox);

            LayerLabels.labels.Add(LayerName);

            // Labels for area menu
            AreaLabels.labels.Add(AreaName);
            AreaLabels.labels.Add(AreaWidth);
            AreaLabels.labels.Add(AreaHeight);
            AreaLabels.labels.Add(AreaX);
            AreaLabels.labels.Add(AreaY);

            // Draw these to screen.
            UI_Elements.Add(TileMenu);
            UI_Elements.Add(TopBar);
            UI_Elements.Add(GeneralOverlay);
            UI_Elements.Add(Properties);

            // Scrollable
            UI_Elements.Add(ObjectMenu);
            UI_Elements.Add(AreaMenu);
            UI_Elements.Add(LayerMenu);
            UI_Elements.Add(CollisionSpriteList);

            // Tile palette
            UI_Elements.Add(TileMenu);

            // Labels
            UI_Elements.Add(LayerLabels);
            UI_Elements.Add(AreaLabels);
            UI_Elements.Add(TileLabels);
            UI_Elements.Add(ObjectLabels);

            // Keep track of the menus in the right UI bar.
            PropertyMenu1.Add(LayerMenu);
            PropertyMenu1.Add(AreaMenu);
            PropertyMenu1.Add(ObjectMenu);
            PropertyMenu1.Add(CollisionSpriteList);

            Scrollable_Menus.Add(LayerMenu);
            Scrollable_Menus.Add(AreaMenu);
            Scrollable_Menus.Add(ObjectMenu);
            Scrollable_Menus.Add(CollisionSpriteList);

            LabelMenus.Add(LayerLabels);
            LabelMenus.Add(AreaLabels);
            LabelMenus.Add(TileLabels);
            LabelMenus.Add(ObjectLabels);

            CharacterRect = new Rectangle(ScreenWidth / 2 - 16, ScreenHeight / 2 - 16, (int) (32 * 2f), (int)(32 * 2f));

            ScrollMenuBounds = new RenderTarget2D(GraphicsDevice, LayerMenu.Destination.Width, LayerMenu.Destination.Height);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            SpriteSheet = new SpriteSheet(TILE_SIZE);
            SpriteSheet.Texture = Content.Load<Texture2D>("tile_sheet");

            using (FileStream stream = new FileStream("../../../Content/UI_new.png", FileMode.Open))
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
            bool DoubleClick;

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

            //if(mouseState.LeftButton == ButtonState.Pressed)
            //{
            //    TimeSinceLastClick = DoubleClickTimer.ElapsedGameTime.Milliseconds;
            //    if (DoubleClickTimer.ElapsedGameTime.Milliseconds - TimeSinceLastClick > 300)
            //        DoubleClick = true;
            //}

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
                    else
                        button.ChangeSourceX(MousePos);
            }


            // Tile sheet is imported.
            if (HasTileSheet)
            {
                foreach (var rect in TileSpriteList[currentPage])
                {
                    // User is hoovering on sprite tile.
                    if (rect.Destination.Contains(MousePos))
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
            if (mouseState.ScrollWheelValue != OriginalScrollWheelValue && state == EditorState.Edit)
            {
                bool MenuScroll = false;

                // Scroll is menu scroll.
                foreach (var menu in UI_Elements)
                    if (menu.Destination.Contains(MousePos) && menu.IsVisible)
                    {
                        if (menu.Scrollable && menu.buttons.Count() > 0)
                        {
                            int adjustment = (int) ((mouseState.ScrollWheelValue - OriginalScrollWheelValue) * 0.04f);
                            int ScrollYOrg = (int) menu.ScrollMenuOffset.Y;
                            menu.ScrollMenuOffset.Y += adjustment;
                            menu.ScrollMenuOffset.Y = Math.Min(menu.ScrollMenuOffset.Y, 0);

                            if(menu.buttons[menu.buttons.Count() - 1].ButtonRect.Y + (int)(menu.ScrollMenuOffset.Y - ScrollYOrg) < menu.Destination.Bottom - 64)
                            {
                                menu.ScrollMenuOffset.Y = ScrollYOrg;
                            }
                            else
                            {
                                foreach (var btn in menu.buttons)
                                {
                                    btn.ButtonRect = new Rectangle(btn.ButtonRect.X, btn.ButtonRect.Y + (int)(menu.ScrollMenuOffset.Y - ScrollYOrg), 224, 48);
                                }
                            }
                        }
                        MenuScroll = true;
                    }


                // User is zooming on map
                if (!MenuScroll)
                {
                    Vector2 Center = new Vector2(Offset.X, Offset.Y);
                    Vector2 MouseBefore = (MousePos - Offset) / Scale;

                    float adjustment = (mouseState.ScrollWheelValue - OriginalScrollWheelValue) * 0.0004f;
                    // Adjust the scaling factor based on the scroll wheel delta
                    Scale += adjustment;
                    Scale = MathHelper.Clamp(Scale, 0.5f, 5f);
                    // Vector2 MouseAfter = MouseBefore * Scale;

                    Vector2 CenterNew = new Vector2(Offset.X, Offset.Y);

                    Offset += ((Center - CenterNew) / 2);
                    Vector2 mouseAfter = (new Vector2(mouseState.X, mouseState.Y) - Offset) / Scale;

                    Vector2 mousePositionDifference = MouseBefore - mouseAfter;
                    // Adjust the offset to keep the mouse position stationary
                    Offset -= mousePositionDifference * Scale;
                }
                else
                {
                    

                }
            }

            if(keyboardState.IsKeyDown(Keys.Escape))
            {
                CursorActionState = CursorState.None;
                Selection.Width = 0;
                Selection.Height = 0;
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

            // Create area.
            if (keyboardState.IsKeyDown(Keys.Enter) && !PreviousKeybordState.IsKeyDown(Keys.Enter) && Selection.Width >= 4 && Selection.Height >= 4)
                AddArea();
            

            // Undo last action.
            if(keyboardState.IsKeyDown(Keys.LeftControl) && keyboardState.IsKeyDown(Keys.Z) && !PreviousKeybordState.IsKeyDown(Keys.Z))
            {

                if (Actions.Count > 0)
                {
                    UserAction UndoAction = Actions.Peek();

                    if (UndoAction.Action == UserAction.ActionType.Draw)
                    {
                        foreach (var area in CurrentMap.areas)
                        {
                            if (area.AreaCords.Contains(UndoAction.x, UndoAction.y))
                            {
                                area.layers[UndoAction.Layer].TileMap[UndoAction.y - area.AreaCords.Y, UndoAction.x - area.AreaCords.X] = new Tile();
                            }
                        }
                    }
                    Actions.Pop();
                }
            }

            // Check collision.
            if(state == EditorState.Edit || !CheckCollision())
                Offset += Velocity;


            // Update helper variables.
            OriginalScrollWheelValue = mouseState.ScrollWheelValue;
            PreviousMouseState = mouseState;
            PreviousKeybordState = keyboardState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            // Grid
            if(state == EditorState.Edit)
            {
                // Draw map (all layers)
                Renderer.RenderMap(CurrentMap, CurrentLayer, _spriteBatch, TileSheet, TILE_SIZE, Scale, Offset, ScreenWidth, ScreenHeight, Grid);
                Renderer.RenderGrid(_spriteBatch, TILE_SIZE, TileSheet, Grid, Scale, Offset, selected, SelectedX, SelectedY, ScreenWidth, ScreenHeight, Selection, CurrentMap, CursorActionState);

                // Start point
                if (CurrentMap.StartLocationSpecified)
                {
                    Rectangle DestRect = new Rectangle((int)(CurrentMap.StartLocation.X * TILE_SIZE * Scale + Offset.X), (int)(CurrentMap.StartLocation.Y * TILE_SIZE * Scale + Offset.Y), 0, 0);
                    _spriteBatch.Draw(UI, new Vector2(DestRect.X, DestRect.Y), SpecifyStartPoint.SourceRect, Color.White, 0f, Vector2.Zero, (float)(32 / TILE_SIZE * Scale / 4), SpriteEffects.None, 0);
                }
                if (A.HasValue)
                {
                    Rectangle DestRect = new Rectangle((int)(A.Value.X * TILE_SIZE * Scale + Offset.X), (int)(A.Value.Y * TILE_SIZE * Scale + Offset.Y), 0, 0);

                    _spriteBatch.Draw(UI, new Vector2(DestRect.X, DestRect.Y), SpecifyDoor.SourceRect, Color.White, 0f, Vector2.Zero, (float)(32 / TILE_SIZE * Scale / 4), SpriteEffects.None, 0);
                }

                // Draw teleportation elements.
                foreach (var tp in CurrentMap.Teleportations)
                {
                    Rectangle DestA = new Rectangle((int)(tp.A.X * TILE_SIZE * Scale + Offset.X), (int)(tp.A.Y * TILE_SIZE * Scale + Offset.Y), 0, 0);
                    Rectangle DestB = new Rectangle((int)(tp.B.X * TILE_SIZE * Scale + Offset.X), (int)(tp.B.Y * TILE_SIZE * Scale + Offset.Y), 0, 0);
                    _spriteBatch.Draw(UI, new Vector2(DestA.X, DestA.Y), SpecifyDoor.SourceRect, Color.White, 0f, Vector2.Zero, (float)(32 / TILE_SIZE * Scale / 4), SpriteEffects.None, 0);
                    _spriteBatch.Draw(UI, new Vector2(DestB.X, DestB.Y), SpecifyDoor.SourceRect, Color.White, 0f, Vector2.Zero, (float)(32 / TILE_SIZE * Scale / 4), SpriteEffects.None, 0);

                }
            }

            // Test state
            if(state == EditorState.Test)
            {
                Renderer.DrawArea(CurrentArea, Offset, TILE_SIZE, TestingScale, ScreenWidth, ScreenHeight, CurrentMap, _spriteBatch, TileSheet);
                _spriteBatch.Draw(UI, CharacterRect, CharacterSource, Color.White);
            }

            // UI elements
            foreach (var menu in UI_Elements)
            {
                menu.Draw(_spriteBatch, UI, ScreenHeight, ScreenWidth, ScaleX, ScaleY, font, TextScale, false);
            }

            _spriteBatch.End();

            Rectangle orgScissorRec = _spriteBatch.GraphicsDevice.ScissorRectangle;
            RasterizerState rasterizerState = new RasterizerState() { ScissorTestEnable = true };
            _spriteBatch.GraphicsDevice.ScissorRectangle = LayerMenu.Destination;

            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, rasterizerState);

            // Draw scrollable menus, elements need to be cut off when out of bounds.
            foreach (var menu in Scrollable_Menus)
            {
                menu.Draw(_spriteBatch, UI, ScreenHeight, ScreenWidth, ScaleX, ScaleY, font, TextScale, true);
            }

            _spriteBatch.End();

            _spriteBatch.GraphicsDevice.ScissorRectangle = orgScissorRec;
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            // Sprite palette menu
            if (TilePaletteVisible)
                Renderer.DrawPalette(HasTileSheet, TileSpriteList, _spriteBatch, selected, Grid, TileSheet);

            // Draw cordinates
            string Cords = "X: " + SelectedX.ToString() + " Y: " + SelectedY.ToString();
            _spriteBatch.DrawString(font, Cords, new Vector2(96 - font.MeasureString(Cords).X/2, ScreenHeight - 64), Color.White, 0f, Vector2.Zero, TextScale, SpriteEffects.None, 0f);

            // TEMP
            // _spriteBatch.DrawString(font, fps.ToString(), new Vector2(32, ScreenHeight - 64), Color.White, 0f, Vector2.Zero, TextScale, SpriteEffects.None, 0f);

            // Draw cursor based on cursor state.
            if(CursorActionState == CursorState.SpecifyingStartPoint)
                _spriteBatch.Draw(UI, new Vector2(MousePos.X - 16, MousePos.Y - 16), SpecifyStartPoint.SourceRect, Color.White);
            else if(CursorActionState == CursorState.SpecifyDoor)
                _spriteBatch.Draw(UI, new Vector2(MousePos.X - 16, MousePos.Y - 16), SpecifyDoor.SourceRect, Color.White);
            else if(CursorActionState == CursorState.Fill)
                _spriteBatch.Draw(UI, new Vector2(MousePos.X - 16, MousePos.Y - 16), FillTool.SourceRect, Color.White);
            else if(CursorActionState == CursorState.Eraser)
                _spriteBatch.Draw(UI, new Vector2(MousePos.X - 16, MousePos.Y - 16), EraserTool.SourceRect, Color.White);
            else
                _spriteBatch.Draw(UI, new Vector2(MousePos.X - 16, MousePos.Y - 16), MouseSource, Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }


        internal void WriteFile()
        {
            
        }


        internal void OpenSpriteSheetFile(string path)
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
                }

                if(resetCursorState && CursorActionState != CursorState.Eraser && CursorActionState != CursorState.Draw && CursorActionState != CursorState.Fill)
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
                {
                    switch (buttonClicked.Action)
                    {
                        case ButtonAction.Import:
                            OpenSpriteSheetFile(TileSheetPath);
                            buttonClicked.IsVisible = false;
                            break;
                        case ButtonAction.Layer:
                            CurrentLayer = buttonClicked.HelperInt;
                            if (ClickedLayerButton != null)
                                ClickedLayerButton.IsPressed = false;
                            buttonClicked.IsPressed = true;
                            ClickedLayerButton = buttonClicked;
                            LayerName.Text = "ID: " + ClickedLayerButton.Text;
                            break;
                        case ButtonAction.OpenLayerMenu:
                            menuState = MenuState.LayerMenu;
                            UpdateMenuState();
                            break;
                        case ButtonAction.OpenSpriteMenu:
                            menuState = MenuState.SpriteTileMenu;
                            UpdateMenuState();
                            break;
                        case ButtonAction.OpenObjectMenu:
                            menuState = MenuState.ObjectMenu;
                            UpdateMenuState();
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
                            CursorActionState = CursorState.Draw;
                            break;
                        case ButtonAction.FillTool:
                            CursorActionState = CursorState.Fill;
                            FillSelection();
                            break;
                        case ButtonAction.EraserTool:
                            CursorActionState = CursorState.Eraser;
                            break;
                        case ButtonAction.SelectArea:
                            foreach (var area in CurrentMap.areas)
                            {
                                if (area.AreaName == buttonClicked.Text)
                                {
                                    Selection = area.AreaCords;
                                    buttonClicked.IsPressed = true;
                                    if (ClickedAreaButton != null)
                                        ClickedAreaButton.IsPressed = false;
                                    ClickedAreaButton = buttonClicked;
                                    SelectedArea = area;
                                }
                            }
                            UpdateAreaLabels();
                            break;
                        case ButtonAction.OpenAreaMenu:
                            menuState = MenuState.AreaMenu;
                            UpdateMenuState();
                            break;
                        case ButtonAction.RemoveArea:
                            if (ClickedAreaButton != null)
                            {
                                for (int i = 0; i < CurrentMap.areas.Count; i++)
                                {
                                    if (CurrentMap.areas[i].AreaName == ClickedAreaButton.Text)
                                    {
                                        CurrentMap.areas.Remove(CurrentMap.areas[i]);
                                    }
                                }

                                Properties.buttons.Remove(Properties.buttons.LastOrDefault(obj => obj.Text == ClickedAreaButton.Text));


                                ClickedAreaButton = null;
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
                            if (CurrentMap.StartLocationSpecified)
                                state = EditorState.Test;
                            CurrentArea = StartArea;
                            OriginalOffset = Offset;
                            Offset = new Vector2(ScreenWidth / 2 - CurrentMap.StartLocation.X * TILE_SIZE * TestingScale, ScreenHeight / 2 - CurrentMap.StartLocation.Y * TILE_SIZE * TestingScale);
                            CharacterSource.X = 0;
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
                            foreach (var tile in TileSpriteList[currentPage])
                            {
                                if (tile.Collision)
                                    CurrentMap.CollisionTiles.Add(tile);
                            }

                            // Tile is added to list, create button for it.
                            if(buttonClicked.IsPressed)
                            {
                                Button button = CreateRemovableButton(ButtonAction.SelectCollisionSprite, ButtonAction.RemoveCollisionSprite);
                                button.Text = selected.ID;
                                CollisionSpriteList.buttons.Add(button);
                                UpdateListOrder(CollisionSpriteList);
                            } 
                            else // Removed from list, remove the button.
                            {
                                CollisionSpriteList.buttons.Remove(CollisionSpriteList.buttons.FirstOrDefault(obj => obj.Text == selected.ID));
                                UpdateListOrder(CollisionSpriteList);
                            }
                            break;
                        case ButtonAction.AddLayer:
                            AddLayer();
                            break;
                        case ButtonAction.RemoveLayer:
                            CurrentMap.RemoveLayer(buttonClicked.HelperInt);
                            UpdateListOrder(LayerMenu);
                            break;
                        case ButtonAction.SelectCollisionSprite:
                            if(ClickedTileButton != null)
                                ClickedTileButton.IsPressed = false;
                            buttonClicked.IsPressed = true;
                            SelectTile(TileSpriteList[currentPage].LastOrDefault(obj => obj.ID == buttonClicked.Text));
                            ClickedTileButton = buttonClicked;
                            break;
                    }
                    if (buttonClicked.IsDeletable && buttonClicked.DeleteButton.ButtonRect.Contains(MousePos))
                    {

                        switch (buttonClicked.DeleteButton.Action)
                        {
                            case ButtonAction.RemoveLayer:
                                CurrentMap.RemoveLayer(buttonClicked.HelperInt);
                                LayerMenu.buttons.Remove(LayerMenu.buttons[buttonClicked.HelperInt]);
                                if (CurrentLayer == buttonClicked.HelperInt)
                                {
                                    CurrentLayer--;
                                    ClearLabels(LayerLabels);
                                }
                                    
                                UpdateListOrder(LayerMenu);
                                break;
                            case ButtonAction.RemoveArea:
                                CurrentMap.RemoveArea(buttonClicked.HelperInt);
                                if(SelectedArea.AreaName == CurrentMap.areas[buttonClicked.HelperInt].AreaName)
                                    SelectedArea = null;

                                AreaMenu.buttons.Remove(AreaMenu.buttons[buttonClicked.HelperInt]);
                                UpdateListOrder(AreaMenu);
                                UpdateAreaLabels();
                                break;
                            case ButtonAction.RemoveCollisionSprite:
                                CollisionSpriteList.buttons.Remove(buttonClicked);
                                UpdateListOrder(CollisionSpriteList);
                                ClearLabels(TileLabels);
                                break;
                        }
                    }
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
                        switch (CursorActionState)
                        {
                            case CursorState.Draw:
                                if(CurrentMap.LayerAmount > 0 && area.layers[CurrentLayer].TileMap[SelectedY - area.AreaCords.Y, SelectedX - area.AreaCords.X].ID != selected.ID)
                                {
                                    area.layers[CurrentLayer].TileMap[SelectedY - area.AreaCords.Y, SelectedX - area.AreaCords.X].ID = selected.ID;
                                    area.layers[CurrentLayer].TileMap[SelectedY - area.AreaCords.Y, SelectedX - area.AreaCords.X].Source = selected.Source;
                                    Actions.Push(new UserAction(UserAction.ActionType.Draw, CurrentLayer, SelectedX, SelectedY));
                                }
                                break;
                            case CursorState.Eraser:
                                area.layers[CurrentLayer].TileMap[SelectedY - area.AreaCords.Y, SelectedX - area.AreaCords.X].ID = "0";
                                area.layers[CurrentLayer].TileMap[SelectedY - area.AreaCords.Y, SelectedX - area.AreaCords.X].Source = new Rectangle();
                                break;
                            case CursorState.Fill:
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
            {
                Velocity.X += (float)(Speed * gameTime.ElapsedGameTime.TotalSeconds);
                CharacterSource.X = 96;
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                Velocity.Y -= (float)(Speed * gameTime.ElapsedGameTime.TotalSeconds);
                CharacterSource.X = 0;
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                Velocity.X -= (float)(Speed * gameTime.ElapsedGameTime.TotalSeconds);
                CharacterSource.X = 64;
            }
            if (keyboardState.IsKeyDown(Keys.W))
            {
                Velocity.Y += (float)(Speed * gameTime.ElapsedGameTime.TotalSeconds);
                CharacterSource.X = 32;
            }
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
                                        {
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

        internal void UpdateMenuState()
        {
            foreach (var menu in PropertyMenu1)
            {
                menu.IsVisible = false;
            }
            foreach (var menu in LabelMenus)
            {
                menu.IsVisible = false;
            }

            switch (menuState)
            {
                case MenuState.LayerMenu:
                    LayerMenu.IsVisible = true;
                    LayerLabels.IsVisible = true;
                    LayerMenuButton.IsPressed = true;
                    AreaMenuButton.IsPressed = false;
                    ObjectMenuButton.IsPressed = false;
                    SpriteMenuButton.IsPressed = false;
                    break;
                case MenuState.AreaMenu:
                    AreaMenu.IsVisible = true;
                    AreaLabels.IsVisible = true;
                    LayerMenuButton.IsPressed = false;
                    AreaMenuButton.IsPressed = true;
                    ObjectMenuButton.IsPressed = false;
                    SpriteMenuButton.IsPressed = false;
                    break;
                case MenuState.SpriteTileMenu:
                    CollisionSpriteList.IsVisible = true;
                    TileLabels.IsVisible = true;
                    LayerMenuButton.IsPressed = false;
                    AreaMenuButton.IsPressed = false;
                    ObjectMenuButton.IsPressed = false;
                    SpriteMenuButton.IsPressed = true;
                    break;
                case MenuState.ObjectMenu:
                    ObjectMenu.IsVisible = true;
                    ObjectLabels.IsVisible = true;
                    LayerMenuButton.IsPressed = false;
                    AreaMenuButton.IsPressed = false;
                    ObjectMenuButton.IsPressed = true;
                    SpriteMenuButton.IsPressed = false;
                    break;
            }
        }

        internal void HandleLabelDoubleClick()
        {

        }

        internal void ChangeAreaProperties()
        {

        }

        internal void AddArea()
        {
            bool allowed = true;
            foreach (var area in CurrentMap.areas)
            {
                if (area.AreaCords.Intersects(Selection))
                {
                    allowed = false;
                }
            }
            if (allowed)
            {
                string name = "Area: " + (CurrentMap.areas.Count() + 1).ToString();
                Button btn = CreateRemovableButton(ButtonAction.SelectArea, ButtonAction.RemoveArea);
                btn.Text = name;
                CurrentMap.CreateArea(Selection, name);

                btn.PressedSourceX = 288;
                btn.SourceRect.Y = 128;

                AreaMenu.buttons.Add(btn);

                UpdateListOrder(AreaMenu);

                if (btn.ButtonRect.Bottom > AreaMenu.Destination.Bottom)
                {
                    foreach (var button in AreaMenu.buttons)
                    {
                        button.ButtonRect = new Rectangle(button.ButtonRect.X, button.ButtonRect.Y - 48, 224, 48);
                    }
                    AreaMenu.ScrollMenuOffset.Y -= 48;
                }
            }
        }

        internal void UpdateListOrder(UI_Menu menu)
        {
            // Make sure the create area button is placed last in list.
            for (int i = 0; i < menu.buttons.Count; i++)
            {
                var btn = menu.buttons[i];
                if (btn.Action == ButtonAction.AddLayer)
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
                btn.ButtonRect.Y = Properties.Destination.Y + 32 + 16 + 48 * j + (int)menu.ScrollMenuOffset.Y;
                btn.HelperInt = j;

                if (btn.IsDeletable)
                {
                    btn.DeleteButton.ButtonRect.Y = btn.ButtonRect.Y + 16;
                    btn.DeleteButton.HelperInt = j;
                }
                j++;
            }
        }

        internal void UpdateAreaLabels()
        {
            if (SelectedArea != null)
            {
                AreaName.Text = SelectedArea.AreaName;
                AreaHeight.Text = "Width: " + SelectedArea.AreaCords.Height.ToString();
                AreaWidth.Text = "Height: " + SelectedArea.AreaCords.Width.ToString();
                AreaX.Text = "Left: " + SelectedArea.AreaCords.X.ToString();
                AreaY.Text = "Top: " + SelectedArea.AreaCords.Y.ToString();
            }
            else
            {
                ClearLabels(AreaLabels);
            }
        }

        internal void ClearLabels(UI_Menu menu)
        {
            foreach(Label label in menu.labels)
            {
                label.Text = "";
            }
            foreach(var button in menu.buttons)
            {
                button.IsVisible = false;
            }
        }

        internal void AddLayer()
        {

            // Add button to the list.
            LayerMenu.buttons.Add(CreateRemovableButton(ButtonAction.Layer, ButtonAction.RemoveLayer));

            // Update the list
            UpdateListOrder(LayerMenu);

            // Update map data.
            CurrentMap.LayerAmount++;
            CurrentMap.AddLayerToAreas();
        }

        internal Button CreateRemovableButton(ButtonAction action, ButtonAction RemoveButtonAction)
        {
            // Create button.
            Button button = new Button("Layer: " + (LayerMenu.buttons.Count()).ToString(), new Rectangle(Properties.Destination.X + Properties.Destination.Width / 2 - 224 / 2, 0, 224, 48), 288, 64, action, true);
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

        internal void SelectTile(SpriteTile rect)
        {
            selected = rect;
            selected.ID = rect.ID;
            selected.Source = rect.Source;
            CursorActionState = CursorState.Draw;
            CurrentTileID.IsVisible = true;
            CurrentTileID.Text = "ID: " + selected.ID;
            Collision.IsVisible = true;
            CollisionCheckBox.IsVisible = true;
            Collision.Text = "Collision";
            CollisionCheckBox.IsPressed = selected.Collision;
        }
    }
}


