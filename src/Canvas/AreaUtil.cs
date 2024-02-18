using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tile_mapper.src.UI;

namespace tile_mapper.src.Canvas
{
    internal static class AreaUtil
    {
        public static void UpdateAreaLabels()
        {
            if (Global.SelectedArea != null)
            {
                GlobalLabels.AreaName.Text = Global.SelectedArea.AreaName;
                GlobalLabels.AreaHeight.Text = "Width: " + Global.SelectedArea.AreaCords.Height.ToString();
                GlobalLabels.AreaWidth.Text = "Height: " + Global.SelectedArea.AreaCords.Width.ToString();
                GlobalLabels.AreaX.Text = "Left: " + Global.SelectedArea.AreaCords.X.ToString();
                GlobalLabels.AreaY.Text = "Top: " + Global.SelectedArea.AreaCords.Y.ToString();
            }
            else
            {
                LabelUtil.ClearLabels(GlobalMenus.AreaLabels);
            }
        }
    }
}
