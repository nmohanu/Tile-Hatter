using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tile_mapper.src.Canvas;

namespace tile_mapper.src.UI
{
    internal class UI_Menu
    {
        public List<Label> labels;
        public bool IsVisible;
        public Rectangle Source;
        public Rectangle Destination;
        public List<Button> buttons;
        public bool Scrollable;
        public Vector2 ScrollMenuOffset = new Vector2(0, 0);

        public UI_Menu(bool IsVisible, Rectangle Source, Rectangle Destination)
        {
            labels = new List<Label>();
            buttons = new List<Button>();
            this.IsVisible = IsVisible;
            this.Source = Source;
            this.Destination = Destination;
        }


        public void Draw(SpriteBatch spriteBatch, Texture2D UI, int ScreenHeight, int ScreenWidth, float ScaleX, float ScaleY, SpriteFont font, float TextScale, bool RenderScrollableMenus)
        {
            if (IsVisible && (!Scrollable || RenderScrollableMenus))
            {
                spriteBatch.Draw(UI, new Vector2(Destination.X, Destination.Y), Source, Color.White, 0f, Vector2.Zero, new Vector2(ScaleX, ScaleY), SpriteEffects.None, 0);
                foreach (var button in buttons)
                {
                    if (button != null && button.IsVisible)
                    {
                        Rectangle source = button.SourceRect;
                        if (button.IsPressed)
                        {
                            source.X = button.PressedSourceX;
                        }

                        // Draw button.
                        Rectangle Destination = button.ButtonRect;
                        spriteBatch.Draw(UI, Destination, source, Color.White);

                        // Draw delete button if button is removable.
                        if (button.IsDeletable)
                        {
                            spriteBatch.Draw(UI, button.DeleteButton.ButtonRect, button.DeleteButton.SourceRect, Color.White);
                        }
                        if (button.Text != null)
                            spriteBatch.DrawString(
                            font,
                            button.Text,
                            new Vector2(
                                    Destination.X + Destination.Width / 2 - font.MeasureString(button.Text).X * TextScale / 2,
                                    Destination.Y + Destination.Height / 2 - font.MeasureString(button.Text).Y * TextScale / 2
                                ),
                                button.color,
                                0f, // Rotation angle, set to 0 for no rotation
                                Vector2.Zero, // Origin, set to Vector2.Zero for the default origin
                                TextScale, // Scale factor
                                SpriteEffects.None, // Sprite effects, set to None for no effects
                                0f // Depth, set to 0 for the default depth

                            );
                    }
                }
                foreach (var label in labels)
                {
                    if (label != null && label.IsVisible && label.Text != null)
                    {
                        spriteBatch.Draw(UI, label.LabelRect, label.SourceRect, Color.White);
                        spriteBatch.DrawString(
                        font,
                        label.Text,
                        new Vector2(
                                label.LabelRect.X + label.LabelRect.Width / 2 - font.MeasureString(label.Text).X * TextScale / 2,
                                label.LabelRect.Y + label.LabelRect.Height / 2 - font.MeasureString(label.Text).Y * TextScale / 2
                            ),
                            Color.White,
                            0f, // Rotation angle, set to 0 for no rotation
                            Vector2.Zero, // Origin, set to Vector2.Zero for the default origin
                            TextScale, // Scale factor
                            SpriteEffects.None, // Sprite effects, set to None for no effects
                            0f // Depth, set to 0 for the default depth
                        );
                    }

                }
            }
        }

        public Button HandleClicks(Vector2 MousePos)
        {
            foreach (var button in buttons)
            {
                if (button != null && button.IsVisible && button.ButtonRect.Contains(MousePos))
                {
                    return button;
                }
            }
            return null;
        }
    }

    internal class Button
    {
        public string Text;
        public ButtonAction Action;
        public Rectangle ButtonRect;
        public Rectangle SourceRect = new Rectangle(0, 48, 96, 48); // Standard button
        public int SelectionX;
        public int OriginalX;
        public bool IsVisible = true;
        public int HelperInt;
        public bool IsPressed = false;
        public int PressedSourceX;
        public Color color = Color.White;
        public bool IsDeletable;
        public Button DeleteButton;
        public Property Property;
        public Button(string text, Rectangle rect, int selectionX, int originalX, ButtonAction action, bool isVisible)
        {
            Text = text;
            ButtonRect = rect;
            SelectionX = selectionX;
            OriginalX = originalX;
            Action = action;
            IsVisible = isVisible;

            SourceRect.Width = rect.Width;
            SourceRect.Height = rect.Height;
        }
        public void ChangeSourceX(Vector2 MousePos)
        {
            if (ButtonRect.Contains(MousePos))
            {
                SourceRect.X = SelectionX;
                if (IsDeletable && DeleteButton.ButtonRect.Contains(MousePos)) { DeleteButton.SourceRect.X = DeleteButton.SelectionX; }
                else if (IsDeletable) { DeleteButton.SourceRect.X = DeleteButton.OriginalX; }
            }
            else
            {
                SourceRect.X = OriginalX;
                if (IsDeletable) { DeleteButton.SourceRect.X = DeleteButton.OriginalX; }
            }
        }
    }

    internal class Label
    {
        public string Text;
        public int ID;
        public Rectangle LabelRect;
        public Rectangle SourceRect = new Rectangle(320, 112, 0, 0); // Standard button
        public bool IsVisible;
        public Property.Type editType = Property.Type.None;
      
    }
    public enum ButtonAction
    {
        None,
        Import,
        Layer,
        Save,
        OpenPalette,
        DrawTool,
        FillTool,
        EraserTool,
        SelectArea,
        SpecifyStartPoint,
        ClosePalette,
        SpecifyDoor,
        EditState,
        TestState,
        MakeCollision,
        RemoveLayer,
        AddLayer,
        RemoveArea,
        AddArea,
        MoveLeftArea,
        MoveRightArea,
        MoveLeftLayer,
        MoveRightLayer,
        OpenObjectMenu,
        OpenLayerMenu,
        OpenAreaMenu,
        OpenSpriteMenu,
        EditorScreen,
        SheetScreen,
        SelectCollisionSprite,
        RemoveCollisionSprite,
        CreateObjectLayer,
        RemoveObjectLayer,
        CreateObject,
        SelectObject,
        RemoveObject,
        SelectObjectLayer,

        // Properties
        CreateLayerProperty,
        CreateAreaProperty,
        CreateObjectProperty,
        CreateObjectLayerProperty,
        RemoveProperty,
        SelectProperty,
        PropertyCancel,
        PropertySave,
        PropertyGoLeft,
        PropertyGoRight
    }
}
