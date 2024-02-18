using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static tile_mapper.src.ProgramLoop;

namespace tile_mapper.src
{
    internal static class KeyboardHandeler
    {
        public static void HandleKeyboard(KeyboardState keyboardState, GameTime gameTime)
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
    }
}
