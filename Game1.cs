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

        // Specify your map size.
        int MAP_WIDTH = 256;
        int MAP_HEIGHT = 256;
        int TILE_SIZE = 16;
        GridTile[,] GridMap;
        Texture2D Grid;
        SpriteSheet SpriteSheet;
        Texture2D UI;
        float Scale = 1f;
        float OriginalScrollWheelValue = 0f;
        float MoveSpeed = 512;
        Vector2 Velocity = Vector2.Zero;
        Vector2 Offset = Vector2.Zero;
        Button NewMap;
        Button LoadMap;
        Button EditMap;
        Button SaveMap;
        Button Import;
        Button Layer;
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
        List<UI_Menu> UI_Elements;
        List<List<SpriteTile>> TileSpriteList;
        bool HasTileSheet = false;
        MouseState PreviousMouseState;
        KeyboardState PreviousKeybordState;
        string SaveFilePath;
        string OpenFilePath;
        
        Stack<UserAction> Actions = new Stack<UserAction>();

        int ScreenWidth;
        int ScreenHeight;

        Vector2 ScreenCenter;

        int SheetWidth;
        int SheetHeight;
        int SheetMenuPages;

        int currentPage = 0;

        Map CurrentMap;
        int CurrentLayer = 0;

        private int frameCounter;
        private TimeSpan elapsedTime;
        private float fps;

        SpriteTile selected;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = ScreenWidth;
            _graphics.PreferredBackBufferHeight = ScreenHeight;
            IsFixedTimeStep = false;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        

        protected override void Initialize()
        {
            // Initialize program.
            GridMap = new GridTile[MAP_HEIGHT, MAP_WIDTH];
            CurrentMap = new Map(MAP_HEIGHT, MAP_WIDTH);

            for (int i = 0; i < MAP_HEIGHT; i++)
            {
                for (int j = 0; j < MAP_WIDTH; j++)
                {
                    GridMap[i, j] = new GridTile
                    {
                        GridRect = new Rectangle(j, i, TILE_SIZE, TILE_SIZE)
                    };

                    CurrentMap.layers[0].TileMap[i, j] = new Tile();
                    CurrentMap.layers[1].TileMap[i, j] = new Tile();
                    CurrentMap.layers[2].TileMap[i, j] = new Tile();
                }
            }


            PreviousMouseState = new MouseState();
            PreviousKeybordState = new KeyboardState();
            UI_Elements = new List<UI_Menu>();

            ScreenWidth = 1920;
            ScreenHeight = 1080;

            ScreenCenter = new Vector2(ScreenWidth, ScreenHeight);

            _graphics.PreferredBackBufferWidth = ScreenWidth;
            _graphics.PreferredBackBufferHeight = ScreenHeight;
            _graphics.ApplyChanges();

            ScaleX = 1f;
            ScaleY = 1f;

            NewMap = new Button("New", new Rectangle(0, 0, 96, 32), 96, 0, ButtonAction.None);
            LoadMap = new Button("Load", new Rectangle(96, 0, 96, 32), 96, 0, ButtonAction.None);
            EditMap = new Button("Edit", new Rectangle(96 * 3, 0, 96, 32), 96, 0, ButtonAction.None);
            SaveMap = new Button("Save", new Rectangle(96 * 2, 0, 96, 32), 96, 0, ButtonAction.Save);
            Import = new Button("Import", new Rectangle(96, ScreenHeight / 2 - 16, 96, 32), 96, 0, ButtonAction.Import);
            Layer = new Button("Layer: " + CurrentLayer.ToString(), new Rectangle(96 * 4, 0, 96, 32), 96, 0, ButtonAction.Layer);
            Settings = new Button("Settings ", new Rectangle(96 * 5, 0, 96, 32), 96, 0, ButtonAction.None);

            Offset = new Vector2(ScreenWidth/2 - TILE_SIZE * MAP_WIDTH/2, ScreenHeight/2 - TILE_SIZE * MAP_HEIGHT / 2);

            TileMenu = new UI_Menu(false, new Rectangle(0, 96, 288, 520), new Rectangle(0, ScreenHeight / 2 - 256, 80, 352));
            TopBar = new UI_Menu(true, new Rectangle(0, 0, 1920, 48), new Rectangle(0, 0, 1920, 48));

            TileMenu.buttons.Add(Import);
            TileMenu.IsVisible = true;

            TileMenu.buttons.Add(NewMap);
            TileMenu.buttons.Add(SaveMap);
            TileMenu.buttons.Add(LoadMap);
            TileMenu.buttons.Add(EditMap);
            TileMenu.buttons.Add(Layer);
            TileMenu.buttons.Add(Settings);

            UI_Elements.Add(TileMenu);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            SpriteSheet = new SpriteSheet(TILE_SIZE);
            SpriteSheet.Texture = Content.Load<Texture2D>("tile_sheet");
            UI = Content.Load<Texture2D>("UI");
            Grid = Content.Load<Texture2D>("grid");
            font = Content.Load<SpriteFont>("font");
        }

        protected override void Update(GameTime gameTime)
        {
            // Calculate fps
            fps = (float)(1.0f / gameTime.ElapsedGameTime.TotalSeconds);

            // Mouse
            MouseState mouseState = Mouse.GetState();
            MousePos = new Vector2(mouseState.X, mouseState.Y);
            Vector2 MousePosRelative = MousePos - Offset;
            HandleLeftClick(mouseState);

            // Keyboard.
            KeyboardState keyboardState = Keyboard.GetState();
            Velocity = Vector2.Zero;
            HandleKeyboard(keyboardState, gameTime);

            // Check if mouse is hoovering on button.
            foreach (var UI in UI_Elements)
            {
                foreach (var button in UI.buttons)
                    button.ChangeSourceX(MousePos);
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
                Vector2 Center = new Vector2(Offset.X + TILE_SIZE * Scale * MAP_WIDTH , Offset.Y + TILE_SIZE * Scale * MAP_HEIGHT);
                float adjustment = (mouseState.ScrollWheelValue - OriginalScrollWheelValue) * 0.0004f;
                // Adjust the scaling factor based on the scroll wheel delta
                Scale += adjustment;
                Scale = MathHelper.Clamp(Scale, 0.5f, 5f);

                Vector2 CenterNew = new Vector2(Offset.X + TILE_SIZE * Scale * MAP_WIDTH, Offset.Y + TILE_SIZE * Scale * MAP_HEIGHT);
                Offset += ((Center - CenterNew) / 2);
            }

            // Calculate mouse X and Y position on the grid.
            Vector2 MousePosInt = MousePosRelative;
            MousePosInt /= Scale;
            MousePosInt /= TILE_SIZE;
            MousePosInt.X = (int)MousePosInt.X;
            MousePosInt.Y = (int)MousePosInt.Y;
            SelectedX = (int) MousePosInt.X;
            SelectedY = (int) MousePosInt.Y;

            // Undo last action.
            if(keyboardState.IsKeyDown(Keys.LeftControl) && keyboardState.IsKeyDown(Keys.Z) && !PreviousKeybordState.IsKeyDown(Keys.Z))
            {

                if(Actions.Count > 0)
                {
                    UserAction UndoAction = Actions.Peek();

                    if(UndoAction.Action == UserAction.ActionType.Draw)
                        CurrentMap.layers[UndoAction.Layer].TileMap[UndoAction.y, UndoAction.x] = new Tile();

                    Actions.Pop();
                }
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
            Renderer.RenderMap(MAP_HEIGHT, MAP_WIDTH, CurrentMap, CurrentLayer, _spriteBatch, TileSheet, TILE_SIZE, Scale, Offset);

            // Grid 
            Renderer.RenderGrid(_spriteBatch, MAP_HEIGHT, MAP_WIDTH, TILE_SIZE, TileSheet, Grid, Scale, Offset, selected, SelectedX, SelectedY);

            // UI elements
            TopBar.Draw(_spriteBatch, UI, ScreenHeight, ScreenWidth, ScaleX, ScaleY, font, TextScale);
            TileMenu.Draw(_spriteBatch, UI, ScreenHeight, ScreenWidth, ScaleX, ScaleY, font, TextScale);

            // Sprite palette menu
            Renderer.DrawPalette(HasTileSheet, TileSpriteList, _spriteBatch, selected, Grid, TileSheet);

            // Draw cordinates
            string Cords = "X: " + SelectedX.ToString() + " Y: " + SelectedY.ToString();
            _spriteBatch.DrawString(font, Cords, new Vector2(144 - font.MeasureString(Cords).X/2, ScreenHeight / 2 - (256 + 24)  - font.MeasureString(Cords).Y / 2), Color.White);

            // TEMP
            _spriteBatch.DrawString(font, fps.ToString(), new Vector2(32, ScreenHeight - 64), Color.White);

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

            SheetMenuPages = (int)Math.Ceiling((float)(SheetWidth * SheetHeight / 55));

            if (SheetMenuPages == 0)
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
                    int x = TileMenu.Destination.X + (j % 5) * TILE_SIZE * 2;
                    int y = TileMenu.Destination.Y + (j / 5) * TILE_SIZE * 2;

                    page[j].Destination = new Rectangle(x, y, TILE_SIZE * 2, TILE_SIZE * 2);
                }

                TileSpriteList.Add(page);
            }

            HasTileSheet = true;

            System.Diagnostics.Debug.WriteLine(TileSheetPath);
        }

        internal void HandleLeftClick(MouseState mouseState)
        {
            if (mouseState.LeftButton == ButtonState.Pressed && PreviousMouseState.LeftButton != ButtonState.Pressed) // Click (Left).
            {
                foreach (var UI in UI_Elements)
                {
                    if (UI.IsVisible)
                    {
                        switch (UI.HandleClicks(MousePos))
                        {
                            case ButtonAction.Import:
                                OpenFile(TileSheetPath);
                                UI.buttons.FirstOrDefault(element => element.Text == "Import").IsVisible = false;
                                break;
                            case ButtonAction.Layer:
                                string ToFind = "Layer: " + CurrentLayer.ToString();
                                CurrentLayer++;
                                CurrentLayer = CurrentLayer % 3;

                                UI.buttons.FirstOrDefault(element => element.Text == ToFind).Text = "Layer: " + CurrentLayer.ToString();
                                break;
                            case ButtonAction.Save:
                                WriteFile();
                                break;
                        }
                    }
                }
            }
            else if (mouseState.LeftButton == ButtonState.Pressed)
            {
                HandleLeftHold();
            }
        }

        internal void HandleLeftHold()
        {
            // Execute each frame if mouse button is held.
            if (selected != null && SelectedX >= 0 && SelectedY >= 0 && SelectedX <= MAP_WIDTH - 1 && SelectedY <= MAP_HEIGHT - 1)
            {
                CurrentMap.layers[CurrentLayer].TileMap[SelectedY, SelectedX].ID = selected.ID;
                CurrentMap.layers[CurrentLayer].TileMap[SelectedY, SelectedX].Source = selected.Source;
                Actions.Push(new UserAction(UserAction.ActionType.Draw, CurrentLayer, SelectedX, SelectedY));
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
    }
}


