using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static tile_mapper.src.ProgramLoop;
using tile_mapper.src.UI;
using tile_mapper.src.Canvas;
using tile_mapper.src.UserSprites;

namespace tile_mapper.src
{
    internal static class Global
    {
        // Meta Data
        public static float Scale = 1f;
        public static float MoveSpeed = 1024;
        public static float TextScale = 0.6f;
        public static float TestingScale = 4f;
        public static float TestingSpeed = 516f;
        public static int ScreenWidth = 1920;
        public static int ScreenHeight = 1080;
        public static int SheetWidth;
        public static int SheetHeight;

        // Temp
        public static int TILE_SIZE = 16;
        public static string TileSheetPath = "../../../Content/Temp/Tiles.png";

        // Vectors
        public static Vector2 Velocity = Vector2.Zero;
        public static Vector2 Offset = Vector2.Zero;
        public static Vector2 MousePos;
        public static Vector2 PreviousMousePos;

        // Helper variables
        public static bool resetSelection;
        public static bool resetCursorState;
        public static GameTime DoubleClickTimer;
        public static float TimeOfLastClick;
        public static float ScaleX = 1f;
        public static float ScaleY = 1f;
        public static int SelectedX;
        public static int SelectedY;
        public static float OriginalScrollWheelValue = 0f;
        public static List<List<SpriteTile>> TileSpriteList;
        public static bool HasTileSheet = false;
        public static MouseState PreviousMouseState;
        public static KeyboardState PreviousKeybordState;
        public static Point SelectionStart;
        public static Point SelectionEnd;
        public static Point ClickPoint;
        public static Rectangle Selection;
        public static Rectangle SpritePaletteDestination;
        public static Rectangle LabelMenuDestination = new Rectangle(1660, 622 - 64, 256, 422);
        public static Rectangle CharacterSource = new Rectangle(0, 864, 32, 32);
        public static Rectangle ScrollMenuSource = new Rectangle(1660, 96, 256, 422);
        public static Rectangle PropertyEditMenuSource = new Rectangle(1356, 796, 268, 281);
        public static Rectangle PropertyEditMenuDestination = new Rectangle(ScreenWidth / 2 - 268 / 2, ScreenHeight / 2 - 280 / 2, 268, 280);
        public static Area StartArea;
        public static Area SelectedArea;
        public static Rectangle CharacterRect;
        public static Vector2 OriginalOffset;
        public static Vector2 PaletteScrollOffset;
        public static Rectangle MouseSource = new Rectangle(0, 784, 32, 32);
        public static bool TilePaletteVisible;
        public static Point? A = null;
        public static Area CurrentArea;
        public static int CurrentPage = 0;
        public static int CurrentLayer = 0;
        public static float fps;
        public static float Timer;
        public static DateTime LastWriteTime;
        public static Property PropertyCurrentlyEditing;
        public static Property.Type[] PropertyTypeList = {Property.Type.None, Property.Type.String, Property.Type.Integer, Property.Type.Bool, Property.Type.Float, Property.Type.Class};
        public static KeyboardTypingDest keyboardTypingDest = KeyboardTypingDest.None;
        public static Label LabelCurrentlyEditing;
        public static Keys LastPressedKey;
        public static Property PropertyEditingCopy;

        public enum KeyboardTypingDest
        {
            None,
            EditingLabel
        }

        // Textures
        public static Texture2D Grid;
        public static Texture2D UI;
        public static Texture2D TileSheet;
        public static SpriteFont font;

       // Spritesheet (user importable)
        public static SpriteSheet SpriteSheet;

        // UI lists
        public static List<UI_Menu> All_UI_Menus = new List<UI_Menu>();
        public static List<UI_Menu> ScrollableMenus = new List<UI_Menu>();
        public static List<UI_Menu> LabelMenus = new List<UI_Menu>();
        public static List<UI_Menu> PropertyMenu = new List<UI_Menu>();

        // States
        public static CursorState CursorActionState = CursorState.None;
        public static EditorState State = EditorState.Edit;
        public static MenuState menuState;

        // The canvas (map / world) that the user is editing. 
        public static WorldCanvas CurrentMap = new WorldCanvas();
  
        // Action stack (for undo).
        public static Stack<UserAction> Actions = new Stack<UserAction>();
        // The sprite that the user has selected.
        public static SpriteTile selected;

    }
}
