using System.Collections.Generic;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using tile_mapper.src.Layer;
using tile_mapper.src.UserSprites;


namespace tile_mapper.src.Canvas
{
    internal class WorldCanvas
    {
        public int LayerAmount = 0;

        public int ObjectLayerAmount = 0;

        public List<Area> areas = new List<Area>();

        public List<Teleportation> Teleportations = new List<Teleportation>();

        public List<SpriteTile> CollisionTiles = new List<SpriteTile>();

        public Point StartLocation;
        public bool StartLocationSpecified;

        public List<ObjectLayer> ObjectLayers = new List<ObjectLayer>();

        public void AddLayerToAreas()
        {
            foreach (var area in areas)
            {
                area.AddLayer();
            }
            LayerAmount++;
        }

        public void CreateArea(Rectangle Selection, string AreaName)
        {
            areas.Add(new Area(Selection, AreaName, LayerAmount, ObjectLayerAmount));
        }

        public void RemoveLayer(int layerIndex)
        {
            foreach (var area in areas)
            {
                area.RemoveLayer(layerIndex);
            }
            LayerAmount--;
        }

        public void RemoveArea(int areaIndex)
        {
            areas.Remove(areas[areaIndex]);
        }


        public void RemoveObjectLayer(int objectLayerIndex)
        {
            ObjectLayers.Remove(ObjectLayers[objectLayerIndex]);
        }

        public void CreateObjectLayer()
        {
            ObjectLayer layer = new ObjectLayer();
            layer.objects = new List<Object>();
            ObjectLayers.Add(layer);
        }
    }
}
