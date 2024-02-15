using System.Collections.Generic;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace tile_mapper
{
    internal class UserAction
    {
        public ActionType Action;
        public int x, y, Layer;
        public Rectangle Rect;
        Area Area;
       
        public UserAction(ActionType action, int layer, int x, int y) 
        {
            this.Action = action;
            this.Layer = layer;
            this.x = x;
            this.y = y;

        }
        public UserAction(ActionType action, int layer, Rectangle rect, Area area)
        {
            this.Action = action;
            this.Layer = layer;
            this.Rect = rect;
            this.Area = area;
        }

        public enum ActionType
        {
            Draw,
            Remove,
            DrawMuliple,
            RemoveMultiple,
        }
    }
    
    internal class Area
    {
        public Rectangle AreaCords;
        public string AreaName;
        public List<Layer> layers;

        public Area(Rectangle areaCords, string areaName, int LayerAmount)
        {
            this.AreaCords = areaCords;
            this.AreaName = areaName;

            layers = new List<Layer>();


            for(int k = 0; k < LayerAmount; k++)
            {
                AddLayer();
            }
        }

        public void AddLayer()
        {
            Layer layer = new Layer(this.AreaCords);
            for (int i = 0; i < this.AreaCords.Height; i++)
            {
                for (int j = 0; j < this.AreaCords.Width; j++)
                {
                    layer.TileMap[i, j] = new Tile();
                }
            }
            layers.Add(layer);
        }

        public void RemoveLayer(int layerIndex)
        {
            layers.Remove(layers[layerIndex]);
        }
    }

    internal class Canvas
    {
        public int LayerAmount = 0;

        public List<Area> areas = new List<Area>();

        public List<Teleportation> Teleportations = new List<Teleportation>();

        public List<SpriteTile> CollisionTiles = new List<SpriteTile>();

        public Point StartLocation;
        public bool StartLocationSpecified;

        public void AddLayerToAreas()
        {
            foreach (var area in areas)
            {
                area.AddLayer();
            }
        }

        public void CreateArea(Rectangle Selection, string AreaName)
        {
            areas.Add(new Area(Selection, AreaName, this.LayerAmount));
        }

        public void RemoveLayer(int layerIndex)
        {
            foreach(var area in  areas)
            {
                area.RemoveLayer(layerIndex);
            }
            LayerAmount--;
        }

        public void RemoveArea(int areaIndex)
        {
            areas.Remove(areas[areaIndex]);
        }
    }

    internal class Teleportation
    {
        public Point A;
        public Point B;
    }

    internal class Layer
    {
        public Tile[,] TileMap;
        public Layer(Rectangle rectangle)
        {
            TileMap = new Tile[rectangle.Height, rectangle.Width];
        }
    }


}
