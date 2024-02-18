﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using tile_mapper.src.UI;

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

        // Temp
        int TILE_SIZE = 16;
        string TileSheetPath = "../../../Content/Temp/tile_sheet.png";

        // Vectors
        Vector2 Velocity = Vector2.Zero;
        Vector2 Offset = Vector2.Zero;
        Vector2 MousePos;
        Vector2 PreviousMousePos;

        // Helper variables
        GameTime DoubleClickTimer;
        float TimeSinceLastClick;
        float ScaleX = 1f;
        float ScaleY = 1f;
        int SelectedX;
        int SelectedY;
        float OriginalScrollWheelValue = 0f;
        List<List<SpriteTile>> TileSpriteList;
        bool HasTileSheet = false;
        MouseState PreviousMouseState;
        KeyboardState PreviousKeybordState;
        Point SelectionStart;
        Point SelectionEnd;
        Point ClickPoint;
        Rectangle Selection;
        Rectangle SpritePaletteDestination;
        Rectangle LabelMenuDestination = new Rectangle(1660, 622 - 64, 256, 422);
        Rectangle CharacterSource = new Rectangle(0, 864, 32, 32);
        Area StartArea;
        Area SelectedArea;
        Rectangle CharacterRect;
        Vector2 OriginalOffset;
        Vector2 PaletteScrollOffset;
        Rectangle MouseSource = new Rectangle(0, 784, 32, 32);
        bool TilePaletteVisible;
        Point? A = null;
        Area CurrentArea;
        int CurrentPage = 0;
        int CurrentLayer = 0;
        private float fps;

        // Textures
        Texture2D Grid;
        Texture2D UI;
        Texture2D TileSheet;
        SpriteFont font;

        // Spritesheet (user importable)
        SpriteSheet SpriteSheet;

        // Meta Data
        float Scale = 1f;
        float MoveSpeed = 1024;
        float TextScale = 0.6f;
        float TestingScale = 4f;
        float TestingSpeed = 516f;
        int ScreenWidth = 1920;
        int ScreenHeight = 1080;
        int SheetWidth;
        int SheetHeight;

        // UI Elements
        UI_Menu TileMenu;
        UI_Menu TopBar;
        UI_Menu GeneralOverlay;
        UI_Menu Properties;
        UI_Menu LayerMenu;
        UI_Menu AreaMenu;
        UI_Menu ObjectMenu;
        UI_Menu PopUpMenu;
        UI_Menu TileLabels;
        UI_Menu AreaLabels;
        UI_Menu LayerLabels;
        UI_Menu ObjectLabels;
        UI_Menu CollisionSpriteList;

        // Buttons
        Button Import;
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
        Button CreateObjectLayerButton;
        Button CreateObjectButton;
        Button SelectedObjectLayerButton;
        Button OpenPalette;
        Button ClosePalette;

        // UI lists
        List<UI_Menu> All_UI_Menus = new List<UI_Menu>();
        List<UI_Menu> ScrollableMenus = new List<UI_Menu>();
        List<UI_Menu> LabelMenus = new List<UI_Menu>();
        List<UI_Menu> PropertyMenu = new List<UI_Menu>();

        // Labels
        Label CurrentTileID;
        Label Collision;
        Label LayerName;
        Label AreaName;
        Label AreaWidth;
        Label AreaHeight;
        Label AreaX;
        Label AreaY;

        // States
        CursorState CursorActionState = CursorState.None;
        EditorState State = EditorState.Edit;
        MenuState menuState;

        // The canvas (map / world) that the user is editing. 
        Canvas CurrentMap = new Canvas();

        // Action stack (for undo).
        Stack<UserAction> Actions = new Stack<UserAction>();

        // The sprite that the user has selected.
        SpriteTile selected;

        // Constructor
        public ProgramLoop()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = ScreenWidth;
            _graphics.PreferredBackBufferHeight = ScreenHeight;
            IsFixedTimeStep = false;

            Content.RootDirectory = "Content";
            IsMouseVisible = false;
        }

        // Helper variables
        internal void InitializeHelperVariables()
        {
            PreviousMouseState = new MouseState();
            PreviousKeybordState = new KeyboardState();
            All_UI_Menus = new List<UI_Menu>();
            Offset = new Vector2(ScreenWidth / 2, ScreenHeight / 2);
            TileSpriteList = new List<List<SpriteTile>>(); // Rectangles for the sprites.
            CharacterRect = new Rectangle(ScreenWidth / 2 - 16, ScreenHeight / 2 - 16, (int)(32 * 2f), (int)(32 * 2f));
        }

        // Monogame window stuff
        internal void InitializeWindow()
        {
            Window.Title = "Tile-Hatter";
            Window.AllowUserResizing = false;
        }
        internal void InitializeGraphicsDevice()
        {
            _graphics.PreferredBackBufferWidth = ScreenWidth;
            _graphics.PreferredBackBufferHeight = ScreenHeight;
            _graphics.ApplyChanges();
        }

        // Buttons
        internal void InitializeToolsetButtons()
        {
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
        }
        internal void InitializePaletteButtons()
        {
            Import = new Button("Import", new Rectangle(144 - 128 / 2, ScreenHeight / 2 - 24, 128, 48), 288, 64, ButtonAction.Import, false);
            Import.SourceRect.Y = 128;
            OpenPalette = new Button("", new Rectangle(0, ScreenHeight / 2 - 32 / 2, 32, 32), 32, 0, ButtonAction.OpenPalette, true);
            ClosePalette = new Button("", new Rectangle(272, ScreenHeight / 2 - 32 / 2, 32, 32), 32, 0, ButtonAction.ClosePalette, false);
            OpenPalette.SourceRect = new Rectangle(0, 720, 32, 32);
            ClosePalette.SourceRect = new Rectangle(0, 752, 32, 32);
        }
        internal void InitializeTestButtons()
        {
            TestMap = new Button("", new Rectangle(ScreenWidth / 2 - 32, 0, 32, 32), 0, 0, ButtonAction.TestState, true);
            TestMap.SourceRect.Y = 128;
            StopTest = new Button("", new Rectangle(ScreenWidth / 2, 0, 32, 32), 32, 32, ButtonAction.EditState, true);
            StopTest.SourceRect.Y = 128;
        }
        internal void InitializeTopBarButtons()
        {
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
        }
        internal void InitializeScrollMenuButtons()
        {
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
        }
        
        // Menus
        internal void InitializePaletteMenu()
        { 
            TileMenu = new UI_Menu(false, new Rectangle(0, 192, 288, 512), new Rectangle(0, ScreenHeight / 2 - 256, 288, 512));
            SpritePaletteDestination = new Rectangle(TileMenu.Destination.X + 16, TileMenu.Destination.Y + 16, TileMenu.Destination.Width - 32, TileMenu.Destination.Height - 32);
        }
        internal void InitializeTopBar()
        { TopBar = new UI_Menu(true, new Rectangle(0, 0, 1920, 80), new Rectangle(0, 0, 1920, 67)); }
        internal void InitializeGeneralOverlay()
        { GeneralOverlay = new UI_Menu(true, new Rectangle(0, 0, 0, 0), new Rectangle(0, 0, 0, 0)); }
        internal void InitializePropertyMenu()
        { Properties = new UI_Menu(true, new Rectangle(1655, 64, 266, 1080), new Rectangle(1655, 0, 266, 1080)); }

        // Scrollable menus
        internal void InitializeLayerScrollMenu()
        {
            LayerMenu = new UI_Menu(true, new Rectangle(1655, 0, 0, 0), new Rectangle(1660, 32, 256, 496));
            LayerMenu.Scrollable = true;

            // Create Layer btn.
            CreateLayerButton = new Button("New Layer", new Rectangle(Properties.Destination.X + Properties.Destination.Width / 2 - 224 / 2, Properties.Destination.Y + 32 + 16 + 48 * 3, 224, 48), 528, 304, ButtonAction.AddLayer, true);
            CreateLayerButton.SourceRect.Y = 240;
            LayerMenu.buttons.Add(CreateLayerButton);

            // Add the default 3 layers.
            for (int i = 1; i <= 3; i++)
            {
                AddLayer();
            }
            LayerMenu.buttons[0].IsPressed = true;
            ClickedLayerButton = LayerMenu.buttons[0];
        }
        internal void InitializeAreaScrollMenu()
        {
            AreaMenu = new UI_Menu(false, new Rectangle(1760, 32, 0, 0), new Rectangle(1660, 32, 256, 496));
            AreaMenu.Scrollable = true;
        }
        internal void InitializeObjectLayerScrollMenu()
        {
            ObjectMenu = new UI_Menu(false, new Rectangle(1760, 32, 0, 0), new Rectangle(1660, 32, 256, 496));
            ObjectMenu.Scrollable = true;

            // Create ObjectLayer btn.
            CreateObjectLayerButton = new Button("New Object Layer", new Rectangle(Properties.Destination.X + Properties.Destination.Width / 2 - 224 / 2, Properties.Destination.Y + 32 + 16 + 48 * 3, 224, 48), 528, 304, ButtonAction.CreateObjectLayer, true);
            CreateObjectLayerButton.SourceRect.Y = 240;
            ObjectMenu.buttons.Add(CreateObjectLayerButton);
            UpdateListOrder(ObjectMenu);
        }
        internal void InitializeSpriteMenu()
        {
            // Collision sprite list
            CollisionSpriteList = new UI_Menu(false, new Rectangle(1768, 802, 0, 0), new Rectangle(1660, 32, 256, 496));
            CollisionSpriteList.Scrollable = true;
        }
        internal void InitializeObjectScrollMenu()
        {
            ObjectLabels = new UI_Menu(false, new Rectangle(1660, 96, 256, 422), LabelMenuDestination); // Object menu has 2 button lists instead of labels.
            ObjectLabels.Scrollable = true;

            // Create Object btn.
            CreateObjectButton = new Button("New Object", new Rectangle(ObjectLabels.Destination.X + ObjectLabels.Destination.Width / 2 - 224 / 2, ObjectLabels.Destination.Y + 16, 224, 48), 528, 304, ButtonAction.CreateObject, true);
            CreateObjectButton.SourceRect.Y = 240;
            ObjectLabels.buttons.Add(CreateObjectButton);
            UpdateListOrder(ObjectLabels);
        }

        // Label menus
        internal void InitializeTileLabelMenu()
        {
            TileLabels = new UI_Menu(false, new Rectangle(1768, 802, 0, 0), LabelMenuDestination);
            CurrentTileID = new Label();
            Collision = new Label();

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
        }
        internal void InitializeAreaLabelMenu()
        {
            AreaLabels = new UI_Menu(false, new Rectangle(1768, 802, 0, 0), LabelMenuDestination);

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
        }
        internal void InitializeLayerLabelMenu()
        {
            LayerLabels = new UI_Menu(true, new Rectangle(1768, 802, 0, 0), LabelMenuDestination);
            LayerName.Text = "ID: " + ClickedLayerButton.Text;
        }

        // Add buttons to the menus
        internal void AddButtonsToMenus()
        {
            TopBar.buttons.Add(DrawTool);
            TopBar.buttons.Add(FillTool);
            TopBar.buttons.Add(EraserTool);
            TopBar.buttons.Add(SpecifyStartPoint);
            TopBar.buttons.Add(SpecifyDoor);
            TopBar.buttons.Add(TestMap);
            TopBar.buttons.Add(StopTest);
            TopBar.buttons.Add(WorldScreen);
            TopBar.buttons.Add(SheetScreen);
            TopBar.buttons.Add(RuleSetScreen);

            GeneralOverlay.buttons.Add(OpenPalette);
            GeneralOverlay.buttons.Add(Import);
            GeneralOverlay.buttons.Add(ClosePalette);

            // Property menu buttons (the small tabs at the bottom).
            Properties.buttons.Add(LayerMenuButton);
            Properties.buttons.Add(ObjectMenuButton);
            Properties.buttons.Add(AreaMenuButton);
            Properties.buttons.Add(SpriteMenuButton);

            // Tile property labels.
            TileLabels.labels.Add(CurrentTileID);
            TileLabels.labels.Add(Collision);
            TileLabels.buttons.Add(CollisionCheckBox);

            // Layer labels.
            LayerLabels.labels.Add(LayerName);

            // Labels for area menu.
            AreaLabels.labels.Add(AreaName);
            AreaLabels.labels.Add(AreaWidth);
            AreaLabels.labels.Add(AreaHeight);
            AreaLabels.labels.Add(AreaX);
            AreaLabels.labels.Add(AreaY);
        }

        // Add UI to lists.
        internal void AddUIToLists()
        {
            // Draw these to screen.
            All_UI_Menus.Add(TileMenu);
            All_UI_Menus.Add(TopBar);
            All_UI_Menus.Add(GeneralOverlay);
            All_UI_Menus.Add(Properties);

            All_UI_Menus.Add(ObjectMenu);
            All_UI_Menus.Add(AreaMenu);
            All_UI_Menus.Add(LayerMenu);
            All_UI_Menus.Add(CollisionSpriteList);

            // Labels
            All_UI_Menus.Add(LayerLabels);
            All_UI_Menus.Add(AreaLabels);
            All_UI_Menus.Add(TileLabels);
            All_UI_Menus.Add(ObjectLabels);

            // Keep track of the menus in the right UI bar.
            PropertyMenu.Add(LayerMenu);
            PropertyMenu.Add(AreaMenu);
            PropertyMenu.Add(ObjectMenu);
            PropertyMenu.Add(CollisionSpriteList);

            // Scrollable
            ScrollableMenus.Add(LayerMenu);
            ScrollableMenus.Add(AreaMenu);
            ScrollableMenus.Add(ObjectMenu);
            ScrollableMenus.Add(CollisionSpriteList);
            ScrollableMenus.Add(ObjectLabels);

            LabelMenus.Add(LayerLabels);
            LabelMenus.Add(AreaLabels);
            LabelMenus.Add(TileLabels);
            LabelMenus.Add(ObjectLabels);
        }

        internal void InitializeUI()
        {
            // Initialize toolset buttons.
            InitializeToolsetButtons();

            // Initialize palette Buttons.
            InitializePaletteButtons();

            // Intialize testing buttons.
            InitializeTestButtons();

            // Initialize top bar buttons (scene switch buttons).
            InitializeTopBarButtons();

            // Initialize the buttons for the side menu.
            InitializeScrollMenuButtons();

            // Initialize the UI menu's
            InitializeTopBar();
            InitializeGeneralOverlay();
            InitializePropertyMenu();
            InitializePaletteMenu();

            // Scroll menus
            InitializeSpriteMenu();
            InitializeLayerScrollMenu();
            InitializeAreaScrollMenu();
            InitializeObjectScrollMenu();
            InitializeObjectLayerScrollMenu();

            // Label menus
            InitializeTileLabelMenu();
            InitializeAreaLabelMenu();
            InitializeLayerLabelMenu();

            // Add buttons and labels to the menus
            AddButtonsToMenus();

            // Add the created UI elements to a list.
            AddUIToLists();
        }

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

            if (mouseState.RightButton == ButtonState.Pressed)
            {
                if (PreviousMousePos != null && PreviousMousePos != MousePos && SpritePaletteDestination.Contains(MousePos))
                {
                    PaletteScrollOffset -= PreviousMousePos - MousePos;
                }
            }

            //if(mouseState.LeftButton == ButtonState.Pressed)
            //{
            //    TimeSinceLastClick = DoubleClickTimer.ElapsedGameTime.Milliseconds;
            //    if (DoubleClickTimer.ElapsedGameTime.Milliseconds - TimeSinceLastClick > 300)
            //        DoubleClick = true;
            //}

            // Check if mouse is hoovering on button.
            foreach (var UI in All_UI_Menus)
            {
                foreach (var button in UI.buttons)
                    if (button != null && !button.IsPressed)
                        button.ChangeSourceX(MousePos);
                    else if (!button.IsPressed)
                    {
                        button.SourceRect.X = button.SelectionX;
                    }
                    else
                        button.ChangeSourceX(MousePos);
            }


            // Tile sheet is imported.
            if (HasTileSheet)
            {
                foreach (var rect in TileSpriteList[CurrentPage])
                {
                    // User is hoovering on sprite tile.
                    if (rect.Destination.Contains(MousePos - PaletteScrollOffset))
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
            if (mouseState.ScrollWheelValue != OriginalScrollWheelValue && State == EditorState.Edit)
            {
                bool MenuScroll = false;

                // Scroll is menu scroll.
                foreach (var menu in All_UI_Menus)
                    if (menu.Destination.Contains(MousePos) && menu.IsVisible)
                    {
                        if (menu.Scrollable && menu.buttons.Count() > 0)
                        {
                            int adjustment = (int)((mouseState.ScrollWheelValue - OriginalScrollWheelValue) * 0.08f);
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
                    Vector2 Center = new Vector2(Offset.X, Offset.Y);
                    Vector2 MouseBefore = (MousePos - Offset) / Scale;

                    float adjustment = (mouseState.ScrollWheelValue - OriginalScrollWheelValue) * 0.0004f;
                    // Adjust the scaling factor based on the scroll wheel delta
                    Scale += adjustment;
                    Scale = MathHelper.Clamp(Scale, 0.5f, 5f);
                    // Vector2 MouseAfter = MouseBefore * Scale;

                    Vector2 CenterNew = new Vector2(Offset.X, Offset.Y);

                    Offset += (Center - CenterNew) / 2;
                    Vector2 mouseAfter = (new Vector2(mouseState.X, mouseState.Y) - Offset) / Scale;

                    Vector2 mousePositionDifference = MouseBefore - mouseAfter;
                    // Adjust the offset to keep the mouse position stationary
                    Offset -= mousePositionDifference * Scale;
                }
                else
                {


                }
            }

            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                CursorActionState = CursorState.None;
                Selection.Width = 0;
                Selection.Height = 0;
            }

            // Only update the selected square if user is not using scroll wheel.
            if (mouseState.ScrollWheelValue == OriginalScrollWheelValue)
            {
                Vector2 mousePosInt;
                if (State == EditorState.Test)
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
            if (keyboardState.IsKeyDown(Keys.LeftControl) && keyboardState.IsKeyDown(Keys.Z) && !PreviousKeybordState.IsKeyDown(Keys.Z))
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
                                area.Layers[UndoAction.Layer].TileMap[UndoAction.y - area.AreaCords.Y, UndoAction.x - area.AreaCords.X] = new Tile();
                            }
                        }
                    }
                    Actions.Pop();
                }
            }

            // Check collision.
            if (State == EditorState.Edit || !CheckCollision())
                Offset += Velocity;


            // Update helper variables.
            OriginalScrollWheelValue = mouseState.ScrollWheelValue;
            PreviousMouseState = mouseState;
            PreviousKeybordState = keyboardState;
            PreviousMousePos = MousePos;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            // Grid
            if (State == EditorState.Edit)
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
            if (State == EditorState.Test)
            {
                Renderer.DrawArea(CurrentArea, Offset, TILE_SIZE, TestingScale, ScreenWidth, ScreenHeight, CurrentMap, _spriteBatch, TileSheet);
                _spriteBatch.Draw(UI, CharacterRect, CharacterSource, Color.White);
            }
            _spriteBatch.End();

            Rectangle orgScissorRec = _spriteBatch.GraphicsDevice.ScissorRectangle;
            RasterizerState rasterizerState = new RasterizerState() { ScissorTestEnable = true };

            // Tile palette cropping
            _spriteBatch.GraphicsDevice.ScissorRectangle = TileMenu.Destination;
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, rasterizerState);

            // Sprite palette menu
            if (TilePaletteVisible)
                Renderer.DrawPalette(HasTileSheet, TileSpriteList, _spriteBatch, selected, UI, TileSheet, TileMenu, PaletteScrollOffset, SpritePaletteDestination);

            _spriteBatch.End();
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            // UI elements
            foreach (var menu in All_UI_Menus)
            {
                menu.Draw(_spriteBatch, UI, ScreenHeight, ScreenWidth, ScaleX, ScaleY, font, TextScale, false);
            }
            _spriteBatch.End();

            

            // Draw scrollable menus, elements need to be cut off when out of bounds.
            foreach (var menu in ScrollableMenus)
            {
                if(menu.IsVisible)
                {
                    _spriteBatch.GraphicsDevice.ScissorRectangle = menu.Destination;
                    _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, rasterizerState);
                    menu.Draw(_spriteBatch, UI, ScreenHeight, ScreenWidth, ScaleX, ScaleY, font, TextScale, true);
                    _spriteBatch.End();
                }
            }

            

            _spriteBatch.GraphicsDevice.ScissorRectangle = orgScissorRec;
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            // Draw cordinates
            string Cords = "X: " + SelectedX.ToString() + " Y: " + SelectedY.ToString();
            _spriteBatch.DrawString(font, Cords, new Vector2(96 - font.MeasureString(Cords).X / 2, ScreenHeight - 64), Color.White, 0f, Vector2.Zero, TextScale, SpriteEffects.None, 0f);

            // TEMP
            // _spriteBatch.DrawString(font, fps.ToString(), new Vector2(32, ScreenHeight - 64), Color.White, 0f, Vector2.Zero, TextScale, SpriteEffects.None, 0f);

            // Draw cursor based on cursor state.
            if (CursorActionState == CursorState.SpecifyingStartPoint)
                _spriteBatch.Draw(UI, new Vector2(MousePos.X - 16, MousePos.Y - 16), SpecifyStartPoint.SourceRect, Color.White);
            else if (CursorActionState == CursorState.SpecifyDoor)
                _spriteBatch.Draw(UI, new Vector2(MousePos.X - 16, MousePos.Y - 16), SpecifyDoor.SourceRect, Color.White);
            else if (CursorActionState == CursorState.Fill)
                _spriteBatch.Draw(UI, new Vector2(MousePos.X - 16, MousePos.Y - 16), FillTool.SourceRect, Color.White);
            else if (CursorActionState == CursorState.Eraser)
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
            using (FileStream stream = new FileStream(TileSheetPath, FileMode.Open)) // Import file.
            {
                TileSheet = Texture2D.FromStream(GraphicsDevice, stream);
            }

            SheetWidth = TileSheet.Width / TILE_SIZE; // Sheet width.
            SheetHeight = TileSheet.Height / TILE_SIZE; // Sheet height.

            List<SpriteTile> page = new List<SpriteTile>(); // For multiple sprite sheets.

            for (int y = 0; y < SheetHeight; y++)
            {
                for (int x = 0; x < SheetWidth; x++)
                {
                    int xcord = x * TILE_SIZE;
                    int ycord = y * TILE_SIZE;

                    int xdest = TileMenu.Destination.X + 16 + x * TILE_SIZE * 2;
                    int ydest = TileMenu.Destination.Y + 16 + y * TILE_SIZE * 2;

                    SpriteTile tile = new SpriteTile();
                    tile.Source = new Rectangle(xcord, ycord, TILE_SIZE, TILE_SIZE);

                    tile.Destination = new Rectangle();

                    tile.ID = "X" + x.ToString() + "Y" + y.ToString(); // Set sprite unique ID.
                    tile.Destination = new Rectangle(xdest, ydest, TILE_SIZE * 2, TILE_SIZE * 2);

                    page.Add(tile);

                }
            }

            TileSpriteList.Add(page);

            HasTileSheet = true;

            System.Diagnostics.Debug.WriteLine(TileSheetPath);
        }

        internal void HandleLeftClick(MouseState mouseState)
        {
            if (mouseState.LeftButton == ButtonState.Pressed && PreviousMouseState.LeftButton != ButtonState.Pressed) // Click (Left) execute once.
            {
                bool resetCursorState = true;
                if (CursorActionState == CursorState.SpecifyingStartPoint)
                {
                    foreach (var area in CurrentMap.areas)
                    {
                        if (area.AreaCords.Contains(SelectedX, SelectedY))
                        {
                            CurrentMap.StartLocation = new Point(SelectedX, SelectedY);
                            CurrentMap.StartLocationSpecified = true;
                            CurrentArea = area;
                            StartArea = CurrentArea;
                        }
                    }

                }
                else if (CursorActionState == CursorState.SpecifyDoor)
                {

                    foreach (var area in CurrentMap.areas)
                    {
                        if (area.AreaCords.Contains(SelectedX, SelectedY))
                        {
                            if (A.HasValue)
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

                if (resetCursorState && CursorActionState != CursorState.Eraser && CursorActionState != CursorState.Draw && CursorActionState != CursorState.Fill)
                    CursorActionState = CursorState.None;


                bool resetSelection = true;
                foreach (var UI in All_UI_Menus)
                {
                    if (UI.Destination.Contains(MousePos))
                    {
                        resetSelection = false;
                    }
                }

                if (resetSelection)
                {
                    ClickPoint = new Point(SelectedX, SelectedY);
                    SelectionStart = ClickPoint;
                    SelectionEnd = SelectionStart;
                    Selection.Width = 0;
                    Selection.Height = 0;
                    if (ClickedAreaButton != null)
                        ClickedAreaButton.IsPressed = false;
                }
                Button buttonClicked = null;

                foreach (var UI in All_UI_Menus)
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
                            // TileMenu.IsVisible = true;
                            OpenPalette.IsVisible = false;
                            ClosePalette.IsVisible = true;
                            if (!HasTileSheet)
                                Import.IsVisible = true;
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
                            // TileMenu.IsVisible = false;
                            OpenPalette.IsVisible = true;
                            Import.IsVisible = false;
                            ClosePalette.IsVisible = false;
                            TilePaletteVisible = false;
                            break;
                        case ButtonAction.TestState:
                            if (CurrentMap.StartLocationSpecified)
                                State = EditorState.Test;
                            CurrentArea = StartArea;
                            OriginalOffset = Offset;
                            Offset = new Vector2(ScreenWidth / 2 - CurrentMap.StartLocation.X * TILE_SIZE * TestingScale, ScreenHeight / 2 - CurrentMap.StartLocation.Y * TILE_SIZE * TestingScale);
                            CharacterSource.X = 0;
                            break;
                        case ButtonAction.EditState:
                            State = EditorState.Edit;
                            Offset = OriginalOffset;
                            break;
                        case ButtonAction.MakeCollision:
                            buttonClicked.IsPressed = !buttonClicked.IsPressed;
                            selected.Collision = buttonClicked.IsPressed;
                            TileSpriteList[CurrentPage].FirstOrDefault(obj => obj.ID == selected.ID).Collision = selected.Collision;

                            CurrentMap.CollisionTiles.Clear();
                            foreach (var tile in TileSpriteList[CurrentPage])
                            {
                                if (tile.Collision)
                                    CurrentMap.CollisionTiles.Add(tile);
                            }

                            // Tile is added to list, create button for it.
                            if (buttonClicked.IsPressed)
                            {
                                Button button = ScrollMenuUtil.CreateRemovableButton(ButtonAction.SelectCollisionSprite, ButtonAction.RemoveCollisionSprite, Properties);
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
                            if (ClickedTileButton != null)
                                ClickedTileButton.IsPressed = false;
                            buttonClicked.IsPressed = true;
                            SelectTile(TileSpriteList[CurrentPage].LastOrDefault(obj => obj.ID == buttonClicked.Text));
                            ClickedTileButton = buttonClicked;
                            break;
                        case ButtonAction.CreateObjectLayer:
                            AddObjectLayer();
                            break;
                        case ButtonAction.CreateObject:
                            AddObject();
                            break;
                        case ButtonAction.SelectObjectLayer:
                            if(SelectedObjectLayerButton != null)
                                SelectedObjectLayerButton.IsPressed = false;
                            SelectedObjectLayerButton = buttonClicked;
                            buttonClicked.IsPressed = true;
                            ReloadObjects();
                            break;
                    }
                    if (buttonClicked.IsDeletable && buttonClicked.DeleteButton.ButtonRect.Contains(MousePos)) // The delete buttons (X)
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
                                if (SelectedArea.AreaName != null && SelectedArea.AreaName == CurrentMap.areas[buttonClicked.HelperInt - 1].AreaName)
                                    SelectedArea = null;

                                AreaMenu.buttons.Remove(AreaMenu.buttons[buttonClicked.HelperInt]);
                                UpdateListOrder(AreaMenu);
                                UpdateAreaLabels();
                                break;
                            case ButtonAction.RemoveCollisionSprite:
                                CollisionSpriteList.buttons.Remove(buttonClicked);
                                selected.Collision = false;
                                UpdateListOrder(CollisionSpriteList);
                                ClearLabels(TileLabels);
                                break;
                            case ButtonAction.RemoveObjectLayer:
                                ObjectMenu.buttons.Remove(buttonClicked);
                                CurrentMap.ObjectLayers.RemoveAt(buttonClicked.HelperInt);
                                UpdateListOrder(ObjectMenu);
                                break;
                            case ButtonAction.RemoveObject:
                                ObjectLabels.buttons.Remove(buttonClicked);
                                CurrentMap.ObjectLayers[SelectedObjectLayerButton.HelperInt].objects.RemoveAt(buttonClicked.HelperInt);
                                UpdateListOrder(ObjectLabels);
                                break;
                        }
                    }
                }
            }
        }

        internal void HandleLeftHold(MouseState mouseState, KeyboardState keyboardState)
        {
            foreach (var menu in All_UI_Menus)
                if (menu.Destination.Contains(MousePos))
                    return;
            // Execute each frame if mouse button is held.
            if (mouseState.LeftButton == ButtonState.Pressed && selected != null && !keyboardState.IsKeyDown(Keys.LeftShift))
            {
                foreach (var area in CurrentMap.areas)
                {
                    if (area.AreaCords.Contains(SelectedX, SelectedY))
                    {
                        switch (CursorActionState)
                        {
                            case CursorState.Draw:
                                if (CurrentMap.LayerAmount > 0 && area.Layers[CurrentLayer].TileMap[SelectedY - area.AreaCords.Y, SelectedX - area.AreaCords.X].ID != selected.ID)
                                {
                                    area.Layers[CurrentLayer].TileMap[SelectedY - area.AreaCords.Y, SelectedX - area.AreaCords.X].ID = selected.ID;
                                    area.Layers[CurrentLayer].TileMap[SelectedY - area.AreaCords.Y, SelectedX - area.AreaCords.X].Source = selected.Source;
                                    Actions.Push(new UserAction(UserAction.ActionType.Draw, CurrentLayer, SelectedX, SelectedY));
                                }
                                break;
                            case CursorState.Eraser:
                                area.Layers[CurrentLayer].TileMap[SelectedY - area.AreaCords.Y, SelectedX - area.AreaCords.X].ID = "0";
                                area.Layers[CurrentLayer].TileMap[SelectedY - area.AreaCords.Y, SelectedX - area.AreaCords.X].Source = new Rectangle();
                                break;
                            case CursorState.Fill:
                                if (PreviousMouseState.LeftButton != ButtonState.Pressed) // Exception
                                {
                                    bool allowed = true;
                                    foreach (var UI in All_UI_Menus)
                                    {
                                        if (UI.Destination.Contains(MousePos))
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
            float Speed = State == EditorState.Test ? TestingSpeed : MoveSpeed;
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
                if (area.AreaCords.Contains(CharacterX, CharacterY))
                {
                    areaToSearch = area;
                }
            }

            if (areaToSearch != null)
            {
                for (int k = 0; k < CurrentMap.LayerAmount; k++)
                {
                    for (int i = Math.Max(CharacterY - 3, areaToSearch.AreaCords.Y); i < Math.Min(areaToSearch.AreaCords.Y + areaToSearch.AreaCords.Height, CharacterY + 3); i++)
                    {
                        for (int j = Math.Max(CharacterX - 3, areaToSearch.AreaCords.X); j < Math.Min(areaToSearch.AreaCords.X + areaToSearch.AreaCords.Width, CharacterX + 3); j++)
                        {
                            if (areaToSearch.Layers[k].TileMap[i - areaToSearch.AreaCords.Y, j - areaToSearch.AreaCords.X].ID != "0")
                            {
                                var collisionTile = CurrentMap.CollisionTiles.FirstOrDefault(obj => obj.ID == areaToSearch.Layers[k].TileMap[i - areaToSearch.AreaCords.Y, j - areaToSearch.AreaCords.X].ID);
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
            if (selected == null)
            {
                return;
            }

            foreach (var area in CurrentMap.areas)
            {
                for (int i = Selection.Y; i < Selection.Y + Selection.Height; i++)
                {
                    for (int j = Selection.X; j < Selection.X + Selection.Width; j++)
                    {
                        if (area.AreaCords.Contains(j, i))
                        {
                            area.Layers[CurrentLayer].TileMap[i - area.AreaCords.Y, j - area.AreaCords.X].ID = selected.ID;
                            area.Layers[CurrentLayer].TileMap[i - area.AreaCords.Y, j - area.AreaCords.X].Source = selected.Source;
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

            string IDToFill = areaClicked.Layers[CurrentLayer].TileMap[SelectedY - areaClicked.AreaCords.Y, SelectedX - areaClicked.AreaCords.X].ID;

            HashSet<(int, int)> visited = new HashSet<(int, int)>();
            Queue<(int, int)> queue = new Queue<(int, int)>();
            queue.Enqueue((SelectedX, SelectedY));

            while (queue.Count > 0)
            {
                var (x, y) = queue.Dequeue();

                if (!areaClicked.AreaCords.Contains(x, y) || areaClicked.Layers[CurrentLayer].TileMap[y - areaClicked.AreaCords.Y, x - areaClicked.AreaCords.X].ID != IDToFill || visited.Contains((x, y)))
                {
                    continue;
                }

                visited.Add((x, y));

                // Fill the current tile
                areaClicked.Layers[CurrentLayer].TileMap[y - areaClicked.AreaCords.Y, x - areaClicked.AreaCords.X].ID = selected.ID;
                areaClicked.Layers[CurrentLayer].TileMap[y - areaClicked.AreaCords.Y, x - areaClicked.AreaCords.X].Source = selected.Source;

                // Enqueue neighboring tiles
                queue.Enqueue((x + 1, y));
                queue.Enqueue((x - 1, y));
                queue.Enqueue((x, y + 1));
                queue.Enqueue((x, y - 1));
            }
        }

        internal void UpdateMenuState()
        {
            foreach (var menu in PropertyMenu)
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
                Button btn = ScrollMenuUtil.CreateRemovableButton(ButtonAction.SelectArea, ButtonAction.RemoveArea, Properties);
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
            foreach (Label label in menu.labels)
            {
                label.Text = "";
            }
            foreach (var button in menu.buttons)
            {
                button.IsVisible = false;
            }
        }

        internal void AddLayer()
        {
            Button btn = ScrollMenuUtil.CreateRemovableButton(ButtonAction.Layer, ButtonAction.RemoveLayer, Properties);
            btn.Text = "Layer: " + (CurrentMap.LayerAmount + 1).ToString();

            // Add button to the list.
            LayerMenu.buttons.Add(btn);

            // Update the list
            UpdateListOrder(LayerMenu);

            // Update map data.
            CurrentMap.AddLayerToAreas();
        }

        internal void AddObjectLayer()
        {
            Button btn = ScrollMenuUtil.CreateRemovableButton(ButtonAction.SelectObjectLayer, ButtonAction.RemoveObjectLayer, Properties);
            btn.Text = "ObjectLayer " + (CurrentMap.ObjectLayers.Count() + 1).ToString();
            ObjectMenu.buttons.Add(btn);
            UpdateListOrder(ObjectMenu);
            CurrentMap.CreateObjectLayer();
        }

        internal void AddObject()
        {
            if (SelectedObjectLayerButton != null)
            {
                
                Object NewObject = new Object();
                NewObject.ID = "Object: " + (CurrentMap.ObjectLayers[SelectedObjectLayerButton.HelperInt].objects.Count() + 1).ToString();

                CurrentMap.ObjectLayers[SelectedObjectLayerButton.HelperInt].AddObject(NewObject);

                ReloadObjects();
                
            }
        }

        internal void ReloadObjects()
        {
            if(SelectedObjectLayerButton != null)
            {
                ObjectLabels.buttons.Clear();
                ObjectLabels.buttons.Add(CreateObjectButton);
                foreach (var Object in CurrentMap.ObjectLayers[SelectedObjectLayerButton.HelperInt].objects)
                {
                    Button btn = ScrollMenuUtil.CreateRemovableButton(ButtonAction.SelectObject, ButtonAction.RemoveObject, Properties);
                    btn.Text = Object.ID.ToString();
                    ObjectLabels.buttons.Add(btn);
                }
                UpdateListOrder(ObjectLabels);
            }
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


