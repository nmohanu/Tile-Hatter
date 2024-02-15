using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tile_mapper
{
    internal static class ScrollMenuUtil
    {
        internal static Button CreateRemovableButton(ButtonAction action, ButtonAction RemoveButtonAction, UI_Menu Properties)
        {
            // Create button.
            Button button = new Button("", new Rectangle(Properties.Destination.X + Properties.Destination.Width / 2 - 224 / 2, 0, 224, 48), 288, 64, action, true);
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
    }
}
