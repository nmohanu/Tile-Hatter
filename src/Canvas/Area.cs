using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tile_mapper.src.Layer;
using tile_mapper.src.UserSprites;

namespace tile_mapper.src.Canvas
{
    internal class Area
    {
        public Rectangle AreaCords;
        public string AreaName;
        public List<SpriteLayer> Layers;

        public Area(Rectangle areaCords, string areaName, int LayerAmount, int ObjectLayerAmount)
        {
            AreaCords = areaCords;
            AreaName = areaName;

            Layers = new List<SpriteLayer>();

            for (int k = 0; k < LayerAmount; k++)
            {
                AddLayer();
            }
        }

        public void AddLayer()
        {
            SpriteLayer layer = new SpriteLayer(AreaCords);
            for (int i = 0; i < AreaCords.Height; i++)
            {
                for (int j = 0; j < AreaCords.Width; j++)
                {
                    layer.TileMap[i, j] = new Tile();
                }
            }
            Layers.Add(layer);
        }

        public void RemoveLayer(int layerIndex)
        {
            Layers.Remove(Layers[layerIndex]);
        }

    }
}
