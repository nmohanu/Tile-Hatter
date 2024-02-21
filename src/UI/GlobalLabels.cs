using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tile_mapper.src.UI
{
    internal static class GlobalLabels
    {
        // Labels
        public static Label CurrentTileID;
        public static Label Collision;
        public static Label AreaName;
        public static Label AreaWidth;
        public static Label AreaHeight;
        public static Label AreaX;
        public static Label AreaY;
        public static Label PropertyID;
        public static Label PropertyType;
        public static Label PropertyValue;
        public static Label CurrentPropertyID;
        public static Label CurrentPropertyType;
        public static Label CurrentPropertyValue;

        public static List<Label> EditableLabels = new List<Label>();
    }
}
