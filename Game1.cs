using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using static System.Formats.Asn1.AsnWriter;

namespace tile_mapper
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Specify your map size.
        int MAP_WIDTH = 32;
        int MAP_HEIGHT = 32;
        int TILE_SIZE = 16;
        GridTile[,] GridMap;
        Texture2D Grid;
        SpriteSheet SpriteSheet;
        Texture2D UI;
        float Scale = 1f;
        float OriginalScrollWheelValue = 0f;
        float MoveSpeed = 256f;
        Vector2 Velocity = Vector2.Zero;
        Vector2 Offset = Vector2.Zero;
        Button NewMap;
        Button LoadMap;
        Button EditMap;
        Button GoLeft;
        Button GoRight;
        List<Button> buttons = new List<Button>();
        SpriteFont font;
        float TextScale = 0.6f;
        float ScaleX;
        float ScaleY;
        Vector2 MousePos;

        int ScreenWidth;
        int ScreenHeight;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = ScreenWidth;
            _graphics.PreferredBackBufferHeight = ScreenHeight;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Initialize program.
            GridMap = new GridTile[MAP_HEIGHT, MAP_WIDTH];

            for(int i = 0; i < MAP_HEIGHT; i++)
            {
                for (int j = 0; j < MAP_WIDTH; j++)
                {
                    GridMap[i, j] = new GridTile
                    {
                        GridRect = new Rectangle(j, i, TILE_SIZE, TILE_SIZE)
                    };
                }
            }

            

            ScaleX = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 1920;
            ScaleY = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 1080;

            ScreenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            ScreenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            _graphics.PreferredBackBufferWidth = ScreenWidth;
            _graphics.PreferredBackBufferHeight = ScreenHeight;
            _graphics.ApplyChanges();

            NewMap = new Button("New", new Rectangle(0, 0, 96, 48), 96, 0);
            LoadMap = new Button("Load", new Rectangle(96, 0, 96, 48), 96, 0);
            EditMap = new Button("Edit", new Rectangle(96 * 2, 0, 96, 48), 96, 0);
            GoLeft = new Button("", new Rectangle(96, ScreenHeight/2 + 256 - 48, 32, 32), 224, 192);
            GoRight = new Button("", new Rectangle(160, ScreenHeight / 2 + 256 - 48, 32, 32), 288, 256);

            buttons.Add(NewMap);
            buttons.Add(LoadMap);
            buttons.Add(EditMap);
            buttons.Add(GoLeft);
            buttons.Add(GoRight);

            Offset = new Vector2(ScreenWidth/2 - TILE_SIZE * MAP_WIDTH/2, ScreenHeight/2 - TILE_SIZE * MAP_HEIGHT / 2);

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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            MouseState mouseState = Mouse.GetState();
            KeyboardState keyboardState = Keyboard.GetState();
            Velocity = Vector2.Zero;
            MousePos = new Vector2(mouseState.X, mouseState.Y);

            foreach (var button in buttons)
            {
                if (button.ButtonRect.Contains(MousePos))
                {
                    button.SourceRect.X = button.SelectionX;
                }
                else
                {
                    button.SourceRect.X = button.OriginalX;
                }
            }

            if (mouseState.ScrollWheelValue != OriginalScrollWheelValue)
            {
                // Calculate the center of the screen
                Vector2 screenCenter = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

                Vector2 GridCenter = new Vector2(Offset.X + Scale * MAP_WIDTH * TILE_SIZE/2, Offset.Y + Scale * MAP_HEIGHT * TILE_SIZE/2);
                Vector2 offsetToCenter = screenCenter - GridCenter;

                float adjustment = (mouseState.ScrollWheelValue - OriginalScrollWheelValue) * 0.0004f;
                // Adjust the scaling factor based on the scroll wheel delta
                Scale += adjustment;

                Scale = MathHelper.Clamp(Scale, 0.5f, 5.0f);



                // Offset -= offsetToCenter * adjustment;

            }
            
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                MousePos -= Offset;

                MousePos /= Scale;
                MousePos /= TILE_SIZE;

                MousePos.X = (int) MousePos.X;
                MousePos.Y = (int) MousePos.Y;

                System.Diagnostics.Debug.WriteLine(MousePos);
            }

            if(keyboardState.IsKeyDown(Keys.A))
                Velocity.X += (float) (MoveSpeed * gameTime.ElapsedGameTime.TotalSeconds);
            if(keyboardState.IsKeyDown(Keys.S))
                Velocity.Y -= (float) (MoveSpeed * gameTime.ElapsedGameTime.TotalSeconds);
            if(keyboardState.IsKeyDown(Keys.D))
                Velocity.X -= (float) (MoveSpeed * gameTime.ElapsedGameTime.TotalSeconds);
            if(keyboardState.IsKeyDown(Keys.W))
                Velocity.Y += (float)(MoveSpeed * gameTime.ElapsedGameTime.TotalSeconds);

            Offset += Velocity;

            OriginalScrollWheelValue = mouseState.ScrollWheelValue;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DimGray);

            // TODO: Add your drawing code here

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            for (int i = 0;i < MAP_HEIGHT;i++)
            {
                for(int j = 0; j < MAP_WIDTH;j++)
                {
                    Rectangle SourceRect = new Rectangle(0, 0, TILE_SIZE, TILE_SIZE);

                    if (i + 1 == MAP_HEIGHT / 2 && j + 1 == MAP_WIDTH / 2)
                    {
                        // Middle of the grid
                        SourceRect.X = 192;
                    }
                    else if (i + 1 == MAP_HEIGHT / 2)
                    {
                        // Middle row
                        SourceRect.X = 160;
                    }
                    else if (j + 1 == MAP_WIDTH / 2)
                    {
                        // Middle column
                        SourceRect.X = 128;
                    }
                    else if ((i + 1) % 4 == 0 && (j + 1) % 4 == 0)
                    {
                        // Every 4th cell
                        SourceRect.X = 96;
                    }
                    else if ((i + 1) % 4 == 0)
                    {
                        // Every 4th cell in a column
                        SourceRect.X = 64;
                    }
                    else if ((j + 1) % 4 == 0)
                    {
                        // Every 4th cell in a row
                        SourceRect.X = 32;
                    }
                    if (i + 1 == MAP_HEIGHT / 2 && (j + 1) % 4 == 0 && j + 1 != MAP_WIDTH/2)
                    {
                        // Middle row, every 4th cell
                        SourceRect.X = 256;
                    }
                    else if (j + 1 == MAP_WIDTH / 2 && (i + 1) % 4 == 0 && i + 1 != MAP_HEIGHT/2)
                    {
                        // Middle column, every 4th cell
                        SourceRect.X = 224;
                    }


                    Rectangle DestRect = new Rectangle((int) (i * TILE_SIZE * Scale + Offset.X), (int) (j * TILE_SIZE * Scale + Offset.Y), (int) (TILE_SIZE * Scale +1), (int) (TILE_SIZE * Scale +1));
                    _spriteBatch.Draw(Grid, DestRect, SourceRect, Color.White);
                }
            }
            // UI elements
            _spriteBatch.Draw(UI, new Vector2(0, 0), new Rectangle(0, 0, 1920, 48), Color.White, 0f, Vector2.Zero, new Vector2(ScaleX, ScaleY), SpriteEffects.None, 0);
            _spriteBatch.Draw(UI, new Vector2(0, ScreenHeight / 2 - 256), new Rectangle(0, 96, 288, 520), Color.White, 0f, Vector2.Zero, new Vector2(ScaleX, ScaleY), SpriteEffects.None, 0);

            // Buttons
            foreach (var button in buttons)
            {
                _spriteBatch.Draw(UI, new Vector2(button.ButtonRect.X, button.ButtonRect.Y), button.SourceRect, Color.White);
                _spriteBatch.DrawString(
                font,
                button.Text,
                new Vector2(
                        button.ButtonRect.X + button.ButtonRect.Width / 2 - font.MeasureString(button.Text).X * TextScale / 2,
                        button.ButtonRect.Y + button.ButtonRect.Height / 2 - font.MeasureString(button.Text).Y * TextScale / 2
                    ),
                    Color.White,
                    0f, // Rotation angle, set to 0 for no rotation
                    Vector2.Zero, // Origin, set to Vector2.Zero for the default origin
                    TextScale, // Scale factor
                    SpriteEffects.None, // Sprite effects, set to None for no effects
                    0f // Depth, set to 0 for the default depth
                );
            }


            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
