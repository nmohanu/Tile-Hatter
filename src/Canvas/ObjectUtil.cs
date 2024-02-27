using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using tile_mapper.src.UI;
using Label = tile_mapper.src.UI.Label;

namespace tile_mapper.src.Canvas
{
    internal static class ObjectUtil
    {

        public static void AddObjectLayer()
        {
            Button btn = ScrollMenuUtil.CreateRemovableButton(ButtonAction.SelectObjectLayer, ButtonAction.RemoveObjectLayer, GlobalMenus.Properties);
            btn.Text = "ObjectLayer " + (Global.CurrentMap.ObjectLayers.Count() + 1).ToString();
            GlobalMenus.ObjectLayerMenu.buttons.Add(btn);
            ScrollMenuUtil.UpdateListOrder(GlobalMenus.ObjectLayerMenu);
            Global.CurrentMap.CreateObjectLayer();
        }
        public static void AddObject()
        {
            if (GlobalButtons.SelectedObjectLayerButton != null)
            {

                Object NewObject = new Object();
                NewObject.ID = "Object: " + (Global.CurrentMap.ObjectLayers[GlobalButtons.SelectedObjectLayerButton.HelperInt].objects.Count() + 1).ToString();

                Global.CurrentMap.ObjectLayers[GlobalButtons.SelectedObjectLayerButton.HelperInt].AddObject(NewObject);

                ReloadObjects();
            }
        }
        public static void ReloadObjects()
        {
            if (GlobalButtons.SelectedObjectLayerButton != null)
            {
                GlobalMenus.ObjectMenu.buttons.Clear();
                GlobalMenus.ObjectMenu.buttons.Add(GlobalButtons.CreateObjectButton);
                foreach (var Object in Global.CurrentMap.ObjectLayers[GlobalButtons.SelectedObjectLayerButton.HelperInt].objects)
                {
                    Button btn = ScrollMenuUtil.CreateRemovableButton(ButtonAction.SelectObject, ButtonAction.RemoveObject, GlobalMenus.Properties);
                    btn.Object = Object;
                    btn.Text = Object.ID.ToString();
                    GlobalMenus.ObjectMenu.buttons.Add(btn);
                }
                ScrollMenuUtil.UpdateListOrder(GlobalMenus.ObjectMenu);
            }
        }

        public static void AddLayerProperty()
        {
            if (Global.CurrentMap.areas.Count() == 0)
                return;
            int amount = 0;
            Button btn = ScrollMenuUtil.CreateRemovableButton(ButtonAction.SelectProperty, ButtonAction.RemoveProperty, GlobalMenus.Properties);

            foreach (var area in Global.CurrentMap.areas)
            {
                if(area.Layers.Count() < 1)
                    continue;
                List<Property> list = area.Layers[Global.CurrentLayer].Properties;
                amount = list.Count() + 1;
                Property property = new Property();
                property.ID = "Property " + (list.Count() + 1).ToString();
                btn.Property = property;
                btn.IsVisible = true;
                list.Add(property);
            }

                btn.Text = "Property " + (amount).ToString();
                GlobalMenus.LayerProperties.buttons.Add(btn);
            
            ReloadLayerProperties();
        }

        public static void AddAreaProperty()
        {
            if (Global.CurrentMap.areas != null && GlobalButtons.ClickedAreaButton != null && Global.CurrentMap.areas[GlobalButtons.ClickedAreaButton.HelperInt] != null)
            {
                List<Property> list = Global.CurrentMap.areas[GlobalButtons.ClickedAreaButton.HelperInt].Properties;
                Property property = new Property();
                property.ID = "Property " + (list.Count() + 1).ToString();
                list.Add(property);

                Button btn = ScrollMenuUtil.CreateRemovableButton(ButtonAction.SelectProperty, ButtonAction.RemoveProperty, GlobalMenus.Properties);
                btn.Text = property.ID;
                btn.Property = property;
                GlobalMenus.AreaProperties.buttons.Add(btn);
                ScrollMenuUtil.UpdateListOrder(GlobalMenus.AreaProperties);
            }
        }

        public static void ReloadLayerProperties()
        {
            GlobalMenus.LayerProperties.buttons.Clear();
            GlobalButtons.CreateLayerPropertyButton.IsVisible = true;
            GlobalMenus.LayerProperties.buttons.Add(GlobalButtons.CreateLayerPropertyButton);

            if (Global.CurrentMap.areas.Count > 0 && Global.CurrentMap.areas[0].Layers.Count() > 0 && Global.CurrentMap.areas[0].Layers[Global.CurrentLayer].Properties.Count() > 0)
            {

                foreach (var Property in Global.CurrentMap.areas[0].Layers[Global.CurrentLayer].Properties)
                {
                    if (Property != null)
                    {
                        Button btn = ScrollMenuUtil.CreateRemovableButton(ButtonAction.SelectProperty, ButtonAction.RemoveProperty, GlobalMenus.Properties);
                        btn.Text = Property.ID.ToString();
                        btn.Property = Property;
                        GlobalMenus.LayerProperties.buttons.Add(btn);
                    }
                }
            }
            ScrollMenuUtil.UpdateListOrder(GlobalMenus.LayerProperties);
        }

        public static void ReloadAreaProperties()
        {
            GlobalMenus.AreaProperties.buttons.Clear();
            GlobalMenus.AreaProperties.buttons.Add(GlobalButtons.CreateAreaPropertyButton);

            if (GlobalButtons.ClickedAreaButton != null && Global.CurrentMap.areas[GlobalButtons.ClickedAreaButton.HelperInt].Properties.Count() > 0)
            {
                foreach (var Property in Global.CurrentMap.areas[GlobalButtons.ClickedAreaButton.HelperInt].Properties)
                {
                    if (Property != null)
                    {
                        Button btn = ScrollMenuUtil.CreateRemovableButton(ButtonAction.SelectProperty, ButtonAction.RemoveProperty, GlobalMenus.Properties);
                        btn.Text = Property.ID.ToString();
                        btn.Property = Property;

                        GlobalMenus.AreaProperties.buttons.Add(btn);
                    }
                }
            }
            ScrollMenuUtil.UpdateListOrder(GlobalMenus.AreaProperties);
        }

        public static void OpenPropertyMenu(Property property)
        {
            GlobalMenus.PropertyEditMenu.IsVisible = true;
            Global.PropertyCurrentlyEditing = property;
            Global.keyboardTypingDest = Global.KeyboardTypingDest.EditingLabel;

            if (property.ID == null)
            {
                GlobalLabels.CurrentPropertyID.Text = "Null";
            }
            else
            {
                GlobalLabels.CurrentPropertyID.Text = property.ID;
            }

            if (property.PropertyType == Property.Type.None)
            {
                GlobalLabels.CurrentPropertyType.Text = "Null";
            }
            else
            {
                GlobalLabels.CurrentPropertyType.Text = property.PropertyType.ToString();
            }

            UpdateValueLabel(property);

            Global.PropertyEditingCopy = new Property();
            Global.PropertyEditingCopy.ID = Global.PropertyCurrentlyEditing.ID;
            Global.PropertyEditingCopy.PropertyType = Global.PropertyCurrentlyEditing.PropertyType;
            Global.PropertyEditingCopy.Bool = Global.PropertyCurrentlyEditing.Bool;
            Global.PropertyEditingCopy.Int = Global.PropertyCurrentlyEditing.Int;
            Global.PropertyEditingCopy.ClassID = Global.PropertyCurrentlyEditing.ClassID;
            Global.PropertyEditingCopy.Float = Global.PropertyCurrentlyEditing.Float;
            Global.PropertyEditingCopy.String = Global.PropertyCurrentlyEditing.String;
        }

        public static void SaveProperty(Property property)
        {
            GlobalMenus.PropertyEditMenu.IsVisible = false;
            Global.PropertyCurrentlyEditing.ID = Global.PropertyEditingCopy.ID;
            Global.PropertyCurrentlyEditing.PropertyType = Global.PropertyEditingCopy.PropertyType;
            Global.PropertyCurrentlyEditing.Bool = Global.PropertyEditingCopy.Bool;
            Global.PropertyCurrentlyEditing.Int = Global.PropertyEditingCopy.Int;
            Global.PropertyCurrentlyEditing.ClassID = Global.PropertyEditingCopy.ClassID;
            Global.PropertyCurrentlyEditing.Float = Global.PropertyEditingCopy.Float;
            Global.PropertyCurrentlyEditing.String = Global.PropertyEditingCopy.String;
            Global.PropertyCurrentlyEditing = null;
            ReloadLayerProperties();
            ReloadAreaProperties();
            Global.keyboardTypingDest = Global.KeyboardTypingDest.None;
        }

        public static void UpdateValueLabel(Property property)
        {
            switch (property.PropertyType)
            {
                case Property.Type.None:
                    GlobalLabels.CurrentPropertyValue.Text = "Null";
                    break;
                case Property.Type.Bool:
                    GlobalLabels.CurrentPropertyValue.Text = property.Bool.ToString();
                    break;
                case Property.Type.Class:
                    GlobalLabels.CurrentPropertyValue.Text = property.ClassID;
                    break;
                case Property.Type.Float:
                    GlobalLabels.CurrentPropertyValue.Text = property.Float.ToString();
                    break;
                case Property.Type.Integer:
                    GlobalLabels.CurrentPropertyValue.Text = property.Int.ToString();
                    break;
                case Property.Type.String:
                    GlobalLabels.CurrentPropertyValue.Text = property.String.ToString();
                    break;
            }
        }

        public static void CancelPropertyEdit()
        {
            GlobalMenus.PropertyEditMenu.IsVisible = false;
            Global.PropertyEditingCopy = null;
            Global.keyboardTypingDest = Global.KeyboardTypingDest.None;
        }

        public static void PropertyGoLeft()
        {
            int currentIndex = Array.IndexOf(Global.PropertyTypeList, Global.PropertyEditingCopy.PropertyType);
            if (currentIndex != -1)
            {
                int newIndex = (currentIndex + Global.PropertyTypeList.Length - 1) % Global.PropertyTypeList.Length;
                Global.PropertyEditingCopy.PropertyType = Global.PropertyTypeList[newIndex];
            }
            GlobalLabels.CurrentPropertyType.Text = Global.PropertyEditingCopy.PropertyType.ToString();
            GlobalLabels.CurrentPropertyValue.editType = Global.PropertyEditingCopy.PropertyType;
            UpdateValueLabel(Global.PropertyEditingCopy);
        }

        public static void PropertyGoRight()
        {
            int currentIndex = Array.IndexOf(Global.PropertyTypeList, Global.PropertyEditingCopy.PropertyType);
            if (currentIndex != -1)
            {
                int newIndex = (currentIndex + 1) % Global.PropertyTypeList.Length;
                Global.PropertyEditingCopy.PropertyType = Global.PropertyTypeList[newIndex];
            }
            GlobalLabels.CurrentPropertyType.Text = Global.PropertyEditingCopy.PropertyType.ToString();
            GlobalLabels.CurrentPropertyValue.editType = Global.PropertyEditingCopy.PropertyType;
            UpdateValueLabel(Global.PropertyEditingCopy);
        }

        public static void AddLetterToLabel(Microsoft.Xna.Framework.Input.Keys key)
        {
            if (Global.LabelCurrentlyEditing == null)
                return;

            string editString = Global.LabelCurrentlyEditing.Text;

            // STRING OR CLASS

            if (Global.PropertyEditingCopy.PropertyType == Property.Type.String || Global.PropertyEditingCopy.PropertyType == Property.Type.Class || Global.LabelCurrentlyEditing == GlobalLabels.CurrentPropertyID)
            {
                if (editString == "Null")
                    editString = "";
                // Edit the string.
                if (key == Microsoft.Xna.Framework.Input.Keys.Back && editString.Length > 0)
                {
                    editString = editString.Substring(0, editString.Length - 1);
                }

                else if (key >= Keys.A && key <= Keys.Z || key >= Keys.D0 && key <= Keys.D9)
                {
                    // Convert the key to a char
                    char keyPressed = (char)key;

                    if (!Global.PreviousKeybordState.IsKeyDown(Keys.LeftShift) && !Global.PreviousKeybordState.IsKeyDown(Keys.RightShift))
                        keyPressed = char.ToLower(keyPressed);

                    // Add the key to the string
                    editString += keyPressed;
                }

                if(Global.LabelCurrentlyEditing == GlobalLabels.CurrentPropertyID)
                    Global.PropertyEditingCopy.ID = editString;
                else if(Global.LabelCurrentlyEditing == GlobalLabels.CurrentPropertyValue && Global.PropertyEditingCopy.PropertyType == Property.Type.String)
                    Global.PropertyEditingCopy.String = editString;
                else if (Global.LabelCurrentlyEditing == GlobalLabels.CurrentPropertyValue)
                    Global.PropertyEditingCopy.ClassID = editString;
            }

            // INTEGER

            else if(Global.PropertyEditingCopy.PropertyType == Property.Type.Integer)
            {
                // Edit the string.
                if (key == Microsoft.Xna.Framework.Input.Keys.Back && editString.Length > 0)
                {
                    editString = editString.Substring(0, editString.Length - 1);
                }

                else if(key >= Keys.D0 && key <= Keys.D9)
                {
                    editString += (char)(key - Keys.D0 + '0');
                }
                int newInt = 0;
                for (int i = 0; i < editString.Length; i++)
                {
                    int digitValue = editString[i] - '0';

                    newInt = newInt * 10 + digitValue;
                }
                Global.PropertyEditingCopy.Int = newInt;
            }

            // FLOAT


            else if (Global.PropertyEditingCopy.PropertyType == Property.Type.Float)
            {
                if (key == Microsoft.Xna.Framework.Input.Keys.Back && editString.Length > 0)
                {
                    editString = editString.Substring(0, editString.Length - 1);
                }

                else if (key >= Keys.D0 && key <= Keys.D9 || key == Keys.OemPeriod)
                {
                    if(key == Keys.OemPeriod)
                    {
                        bool alreadyHasDot = false;

                        foreach (var c in editString)
                        {
                            if (c == '.')
                                alreadyHasDot = true;
                        }
                        if (!alreadyHasDot)
                            editString += '.';
                    }
                    else
                    {
                        editString += (char)(key - Keys.D0 + '0');
                    }
                }

                int dotIndex = -1; // Initialize dot index
                float newFloat = 0.0f; // Initialize newFloat

                // Iterate through the string to find the dot index and calculate the integer part
                for (int i = 0; i < editString.Length; i++)
                {
                    if (editString[i] == '.')
                    {
                        dotIndex = i;
                        continue; // Skip the dot, don't include it in calculations
                    }
                    int digitValue = editString[i] - '0'; // Convert character to integer
                    newFloat = newFloat * 10 + digitValue; // Update integer part
                }

                // If a dot is found, iterate through the string to calculate the fractional part
                if (dotIndex != -1)
                {
                    float fractionalPart = 0.0f;
                    float divisor = 10.0f; // Initial divisor

                    // Iterate through characters after the dot
                    for (int i = dotIndex + 1; i < editString.Length; i++)
                    {
                        int digitValue = editString[i] - '0'; // Convert character to integer
                        fractionalPart += (float)digitValue / divisor; // Update fractional part
                        divisor *= 10; // Update divisor for next digit
                    }
                    newFloat += fractionalPart; // Add fractional part to the integer part
                }
                Global.PropertyEditingCopy.Float = newFloat;
            }
            Global.LabelCurrentlyEditing.Text = editString;
        }

        public static void SelectEditLabel(Label label)
        {
            
            Global.LabelCurrentlyEditing = label;
            Global.keyboardTypingDest = Global.KeyboardTypingDest.EditingLabel;
            label.SourceRect = new Microsoft.Xna.Framework.Rectangle(1392, 752, 192, 32);
        }

        public static void TogglePropertyBool()
        {
            Global.PropertyEditingCopy.Bool = !Global.PropertyEditingCopy.Bool;
            string editString = Global.PropertyEditingCopy.Bool.ToString();
            
            Global.LabelCurrentlyEditing.Text = editString;
        }

        public static void DeselectLabel()
        {
            if (Global.LabelCurrentlyEditing != null)
            {
                Global.LabelCurrentlyEditing.SourceRect = new Microsoft.Xna.Framework.Rectangle(1392, 832, 192, 32); // reset old label sprite
                Global.LabelCurrentlyEditing = null;
            }
        }
    }
}
