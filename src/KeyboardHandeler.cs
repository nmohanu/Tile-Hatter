using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static tile_mapper.src.ProgramLoop;
using tile_mapper.src.Canvas;
using tile_mapper.src.UI;

namespace tile_mapper.src
{
    internal static class KeyboardHandeler
    {
        public static void HandleKeyboard(KeyboardState keyboardState, GameTime gameTime, Label label)
        {
            if(Global.keyboardTypingDest == Global.KeyboardTypingDest.None)
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
            else if(Global.keyboardTypingDest == Global.KeyboardTypingDest.EditingLabel)
            {
                Keys[] pressedKeys = keyboardState.GetPressedKeys();
                if (pressedKeys.Length > 0 && pressedKeys[0] != Global.LastPressedKey)
                {
                    ObjectUtil.AddLetterToLabel(pressedKeys[0], label);
                    Global.LastPressedKey = pressedKeys[0];
                }
                if(pressedKeys.Length == 0)
                {
                    Global.LastPressedKey = Keys.None;
                    
                }
            }
        }
    }
}
