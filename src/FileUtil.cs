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
                        if (tile.ID == previousTileID)
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
                                repeatsProperty.SetAttribute("Int", repeatCount.ToString());
                                tileElement.AppendChild(repeatsProperty);

                                layerElement.AppendChild(tileElement);

                                // Reset repeat count
                                repeatCount = 0;
                            }

                            // Start counting repeats for the new tile
                            previousTileID = tile.ID;
                        }
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
