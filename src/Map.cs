using System.Collections.Generic;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace tile_mapper.src
{
    internal class UserAction
    {
        public ActionType Action;
        public int x, y, Layer;
        public Rectangle Rect;
        Area Area;

        public UserAction(ActionType action, int layer, int x, int y)
        {
            Action = action;
            Layer = layer;
            this.x = x;
            this.y = y;

        }
        public UserAction(ActionType action, int layer, Rectangle rect, Area area)
        {
            Action = action;
            Layer = layer;
            Rect = rect;
            Area = area;
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
        public List<Layer> Layers;
        public List<ObjectLayer> ObjectLayers;

        public Area(Rectangle areaCords, string areaName, int LayerAmount, int ObjectLayerAmount)
        {
            AreaCords = areaCords;
            AreaName = areaName;

            Layers = new List<Layer>();
            ObjectLayers = new List<ObjectLayer>();

            for (int k = 0; k < LayerAmount; k++)
            {
                AddLayer();
            }

            for (int k = 0; k < ObjectLayerAmount; k++)
            {
                AddObjectLayer();
            }
        }

        public void AddLayer()
        {
            Layer layer = new Layer(AreaCords);
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

        public void AddObjectLayer()
        {
            ObjectLayer objectLayer = new ObjectLayer();
            ObjectLayers.Add(objectLayer);
        }

        public void RemoveObjectLayer(int objectLayerIndex)
        {
            ObjectLayers.Remove(ObjectLayers[objectLayerIndex]);
        }
    }

    internal class Canvas
    {
        public int LayerAmount = 0;

        public int ObjectLayerAmount = 0;

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

    internal class ObjectLayer
    {
        public string ID;

        public List<Object> objects;
    }

    internal class Object
    {
        public string ID;

        public Rectangle TileRect; // Tile specifying.
        public Rectangle PixelRect; // To adjust pixel specific.

        public Property property;
    }

    internal class Property // User properties.
    {
        public string String;
        public int Int;
        public float Float;
        public bool Bool;
        public string ClassID;

        Type PropertyType;

        public Property(Type type)
        {
            PropertyType = type;
        }

        public enum Type
        {
            String,
            Integer,
            Float,
            Bool,
            Class
        }
    }
}
