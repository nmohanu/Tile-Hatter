﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tile_mapper.src.Canvas
{
    internal class Teleportation
    {
        public Point A;
        public Point B;
    }

    internal class ObjectLayer
    {
        public string ID;

        public List<Object> objects;

        public void AddObject(Object obj)
        {
            objects.Add(obj);
        }
    }

    internal class Object
    {
        public string ID;

        public Rectangle TileRect; // Tile specifying.
        public Rectangle PixelRect; // To adjust pixel specific.

        public List<Property> Properties = new List<Property>();
    }

    internal class Property // User properties.
    {
        public string String = "Null";
        public int Int = 0;
        public float Float = 0;
        public bool Bool;
        public string ClassID = "Null";
        public string ID;

        public Type PropertyType = Type.None;

        public Property()
        {
        }

        public enum Type
        {
            String,
            Integer,
            Float,
            Bool,
            Class,
            None
        }
    }
}
