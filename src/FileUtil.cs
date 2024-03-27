using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using tile_mapper.src.Canvas;
using tile_mapper.src.Layer;
using tile_mapper.src.UI;
using tile_mapper.src.UserSprites;
using static System.Net.WebRequestMethods;

namespace tile_mapper.src
{
    internal static class FileUtil
    {
        public static void OpenSpriteSheetFile(string path, GraphicsDevice graphicsDevice)
        {
            try
            {
                using (FileStream stream = new FileStream(Global.TileSheetPath, FileMode.Open)) // Import file.
                {
                    Global.TileSheet = Texture2D.FromStream(graphicsDevice, stream);
                }
            }
            catch
            {
                return;
            }
            

            Global.SheetWidth = Global.TileSheet.Width / Global.TILE_SIZE; // Sheet width.
            Global.SheetHeight = Global.TileSheet.Height / Global.TILE_SIZE; // Sheet height.

            List<SpriteTile> page = new List<SpriteTile>(); // For multiple sprite sheets.

            for (int y = 0; y < Global.SheetHeight; y++)
            {
                for (int x = 0; x < Global.SheetWidth; x++)
                {
                    int xcord = x * Global.TILE_SIZE;
                    int ycord = y * Global.TILE_SIZE;

                    int xdest = GlobalMenus.TileMenu.Destination.X + 16 + x * Global.TILE_SIZE * 2;
                    int ydest = GlobalMenus.TileMenu.Destination.Y + 16 + y * Global.TILE_SIZE * 2;

                    SpriteTile tile = new SpriteTile();
                    tile.Source = new Rectangle(xcord, ycord, Global.TILE_SIZE, Global.TILE_SIZE);

                    tile.Destination = new Rectangle();

                    tile.ID = "X" + x.ToString() + "Y" + y.ToString(); // Set sprite unique ID.
                    tile.Destination = new Rectangle(xdest, ydest, Global.TILE_SIZE * 2, Global.TILE_SIZE * 2);

                    page.Add(tile);

                }
            }

            Global.TileSpriteList.Add(page);

            Global.HasTileSheet = true;

            System.Diagnostics.Debug.WriteLine(Global.TileSheetPath);

            Global.LastWriteTime = GetFileWriteTime();


        }

        internal static DateTime GetFileWriteTime()
        {
            return System.IO.File.GetLastWriteTimeUtc(Global.TileSheetPath);
        }

        internal static void ImportWorld()
        {
            if(!Global.HasTileSheet) { return; }

            // Load XML document from file
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("../../../Content/world.xml");

            // Retrieve the root element
            XmlElement worldElement = xmlDoc.DocumentElement;

            // Create a new WorldCanvas object
            WorldCanvas world = new WorldCanvas();

            // Retrieve world data attributes
            int layerAmount = int.Parse(worldElement.GetAttribute("LayerAmount"));
            int objectLayerAmount = int.Parse(worldElement.GetAttribute("ObjectLayerAmount"));
            int areaAmount = int.Parse(worldElement.GetAttribute("AreaAmount"));
            int teleportationAmount = int.Parse(worldElement.GetAttribute("TeleportationAmount"));
            int collisionTileAmount = int.Parse(worldElement.GetAttribute("CollisionTileAmount"));

            // Set world data
            world.LayerAmount = layerAmount;

            // Import areas
            XmlNodeList areaNodes = worldElement.SelectNodes("Area");
            foreach (XmlNode areaNode in areaNodes)
            {
                // Parse area coordinates to construct a Rectangle
                string areaCordsString = areaNode.Attributes["AreaCords"].Value;
                string[] areaCords = areaCordsString.Split(' ');
                areaCords[0] = areaCords[0].Substring(3);
                areaCords[1] = areaCords[1].Substring(2);
                areaCords[2] = areaCords[2].Substring(6);
                areaCords[3] = areaCords[3].Substring(7);
                areaCords[3] = areaCords[3].Substring(0, areaCords[3].Length - 1);
                int x = int.Parse(areaCords[0]);
                int y = int.Parse(areaCords[1]);
                int width = int.Parse(areaCords[2]);
                int height = int.Parse(areaCords[3]);
                Rectangle areaCordsRect = new Rectangle(x, y, width, height);

                // Parse other attributes
                string areaName = areaNode.Attributes["AreaName"].Value;

                // Create Area object with parsed parameters
                Area area = new Area(areaCordsRect, areaName, 0, 0);

                // Import layers within area
                XmlNodeList layerNodes = areaNode.SelectNodes("Layer");
                foreach (XmlNode layerNode in layerNodes)
                {
                    SpriteLayer layer = new SpriteLayer(area.AreaCords);
                    

                    // Import tiles within layer
                    XmlNodeList tileNodes = layerNode.SelectNodes("Tile");

                    int counter = 0; 

                    foreach (XmlNode tileNode in tileNodes)
                    {
                        string tileID = tileNode.Attributes["ID"].Value;
                        int repeatCount = int.Parse(tileNode.SelectSingleNode("Property").Attributes["Int"].Value);

                        // Add tiles to layer based on tile ID and repeat count
                        for (int i = 0; i < repeatCount; i++)
                        {
                            layer.TileMap[counter / area.AreaCords.Width, counter % area.AreaCords.Width] = new Tile();
                            layer.TileMap[counter / area.AreaCords.Width, counter % area.AreaCords.Width].ID = tileID;
                            if(tileID != null && tileID != "0")
                            {
                                string Xstr = "";
                                string Ystr = "";
                                int X = 0;
                                int Y = 0;
                                
                                for (int k = 0; k < tileID.Length; k++)
                                {
                                    if (tileID[k] == 'X')
                                    {
                                        k++;
                                        while (tileID[k] != 'Y' )
                                        {
                                            Xstr += tileID[k];
                                            k++;
                                        }
                                    }
                                    if(tileID[k] != 'Y')
                                    {
                                        Ystr += tileID[k];
                                    }
                                }

                                X = int.Parse(Xstr);
                                Y = int.Parse(Ystr);

                                layer.TileMap[counter / area.AreaCords.Width, counter % area.AreaCords.Width].Source.X = X * Global.TILE_SIZE;
                                layer.TileMap[counter / area.AreaCords.Width, counter % area.AreaCords.Width].Source.Y = Y * Global.TILE_SIZE;
                                layer.TileMap[counter / area.AreaCords.Width, counter % area.AreaCords.Width].Source.Width = Global.TILE_SIZE;
                                layer.TileMap[counter / area.AreaCords.Width, counter % area.AreaCords.Width].Source.Height = Global.TILE_SIZE;
                            }
                            counter++;
                        }
                    }
                    area.Layers.Add(layer);
                }
                world.areas.Add(area);
            }

            // Set the imported world as the current map
            Global.CurrentMap = world;

            // Reload all the area buttons.
            AreaUtil.ReloadAreaButtons();

            // Reload layer buttons.
            LayerUtil.ReloadLayerButtons();
        }


        internal static void ExportWorld()
        {
            WorldCanvas world = Global.CurrentMap;
            // Create an XML document
            XmlDocument xmlDoc = new XmlDocument();
            // Create an XML document
            XmlElement worldElement = xmlDoc.CreateElement("World");

            // World data
            worldElement.SetAttribute("LayerAmount", world.LayerAmount.ToString());
            worldElement.SetAttribute("ObjectLayerAmount", world.ObjectLayers.Count.ToString());
            worldElement.SetAttribute("AreaAmount", world.areas.Count.ToString());
            worldElement.SetAttribute("TeleportationAmount", world.Teleportations.Count.ToString());
            worldElement.SetAttribute("CollisionTileAmount", world.CollisionTiles.Count.ToString());

            // Export areas
            foreach (Area area in world.areas)
            {
                XmlElement areaElement = xmlDoc.CreateElement("Area");
                areaElement.SetAttribute("AreaName", area.AreaName);
                areaElement.SetAttribute("AreaCords", area.AreaCords.ToString());

                // Export layers within area
                foreach (SpriteLayer layer in area.Layers)
                {
                    XmlElement layerElement = xmlDoc.CreateElement("Layer");

                    string previousTileID = null;
                    int repeatCount = 0;
                    foreach (var tile in layer.TileMap)
                    {
                        if (tile.ID == previousTileID || previousTileID == null)
                        {
                            repeatCount++;
                        }
                        else
                        {
                            // If there were repeated tiles, add a single tile with repeats property
                            if (repeatCount > 0)
                            {
                                XmlElement tileElement = xmlDoc.CreateElement("Tile");
                                tileElement.SetAttribute("ID", previousTileID);

                                XmlElement repeatsProperty = xmlDoc.CreateElement("Property");
                                repeatsProperty.SetAttribute("Int", (repeatCount).ToString());
                                tileElement.AppendChild(repeatsProperty);

                                layerElement.AppendChild(tileElement);

                                // Reset repeat count
                                repeatCount = 1;
                            }
                        }
                        previousTileID = tile.ID;
                    }
                    if (repeatCount > 0)
                    {
                        XmlElement tileElement = xmlDoc.CreateElement("Tile");
                        tileElement.SetAttribute("ID", previousTileID);

                        XmlElement repeatsProperty = xmlDoc.CreateElement("Property");
                        repeatsProperty.SetAttribute("Int", repeatCount.ToString());
                        tileElement.AppendChild(repeatsProperty);

                        layerElement.AppendChild(tileElement);
                    }

                    foreach (var property in layer.Properties)
                    {
                        XmlElement propertyElement = xmlDoc.CreateElement("Property");
                        propertyElement.SetAttribute("Name", property.ID); // Assuming property.ID is the ID of the property
                        propertyElement.SetAttribute("Type", property.PropertyType.ToString());

                        // Set value based on property type
                        switch (property.PropertyType)
                        {
                            case Property.Type.String:
                                propertyElement.SetAttribute("String", property.String);
                                break;
                            case Property.Type.Integer:
                                propertyElement.SetAttribute("Int", property.Int.ToString());
                                break;
                            case Property.Type.Float:
                                propertyElement.SetAttribute("Float", property.Float.ToString());
                                break;
                            case Property.Type.Bool:
                                propertyElement.SetAttribute("Bool", property.Bool.ToString());
                                break;
                            case Property.Type.Class:
                                propertyElement.SetAttribute("ClassID", property.ClassID);
                                break;
                            default:
                                // Handle None type or any other custom logic
                                break;
                        }

                        // Export TileMap, Properties, etc. within the layer
                        layerElement.AppendChild(propertyElement);
                    }
                    areaElement.AppendChild(layerElement);
                }
                worldElement.AppendChild(areaElement);
            }
            xmlDoc.AppendChild(worldElement);
            // Save XML document to file or stream
            xmlDoc.Save("../../../Content/world.xml");
        }
    }
}
