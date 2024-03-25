using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tile_mapper.src.UI
{
    internal static class LabelUtil
    {
        public static void ClearLabels(UI_Menu menu)
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
    }
}
