using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.UI.Screens;

namespace Radioactivity.UI
{
    public class UIResources
    {
      

        private Dictionary<string, AtlasIcon> iconList;
        private Dictionary<string, GUIStyle> styleList;
        private Dictionary<string, Color> colorList;

        private Texture generalIcons;

        // Get any color, given its name
        public Color GetColor(string name)
        {
            return colorList[name];
        }

        // Get any icon, given its name
        public AtlasIcon GetIcon(string name)
        {
            return iconList[name];
        }

        // Get a style, given its name
        public GUIStyle GetStyle(string name)
        {
            return styleList[name];
        }

        // Constructor
        public UIResources()
        {
            CreateIconList();
            CreateStyleList();
            CreateColorList();
        }

        // Iniitializes the icon database
        private void CreateIconList()
        {
            generalIcons = (Texture)GameDatabase.Instance.GetTexture("Radioactivity/UI/icon_atlas", false);

            iconList = new Dictionary<string, AtlasIcon>();

            // Add the general icons
            iconList.Add("source", new AtlasIcon(generalIcons, 0.00f, 0.5f, 0.5f, 0.5f));
            iconList.Add("science", new AtlasIcon(generalIcons, 0.5f, 0.5f, 0.5f, 0.5f));
            iconList.Add("probe", new AtlasIcon(generalIcons, 0.0f, 0.0f, 0.5f, 0.5f));
            iconList.Add("kerbal", new AtlasIcon(generalIcons, 0.5f, 0.0f, 0.5f, 0.5f));

            iconList.Add("kerbal_sick", new AtlasIcon(generalIcons, 0.5f, 0.0f, 0.5f, 0.5f));
            iconList.Add("kerbal_dead", new AtlasIcon(generalIcons, 0.5f, 0.0f, 0.5f, 0.5f));

        }

        // Initializes all the styles
        private void CreateStyleList()
        {
            styleList = new Dictionary<string, GUIStyle>();

            GUIStyle draftStyle;
            // Main window
            draftStyle = new GUIStyle(HighLogic.Skin.window);
            draftStyle.padding = new RectOffset(0, 0, 0, 0);
            draftStyle.fontSize = 10;
            styleList.Add("main_window", new GUIStyle(draftStyle));

            draftStyle = new GUIStyle(HighLogic.Skin.button);
            styleList.Add("main_button", new GUIStyle(draftStyle));

            // Mini window
            // Window
            draftStyle = new GUIStyle(HighLogic.Skin.window);
            draftStyle.padding = new RectOffset(0, 0, 0, 0);
            draftStyle.normal.background = null;
            draftStyle.fontSize = 10;
            styleList.Add("mini_window", new GUIStyle(draftStyle));

            draftStyle = new GUIStyle(HighLogic.Skin.textArea);
            draftStyle.padding = new RectOffset(0, 0, 0, 0);
            draftStyle.normal.background = HighLogic.Skin.window.normal.background;
            styleList.Add("mini_group", new GUIStyle(draftStyle));

            draftStyle = new GUIStyle(HighLogic.Skin.label);
            draftStyle.fontSize = 11;
            draftStyle.padding = new RectOffset(5, 0, 0, 0);
            draftStyle.alignment = TextAnchor.MiddleLeft;
            draftStyle.normal.textColor = Color.white;
            draftStyle.stretchWidth = true;
            styleList.Add("mini_text_rad", new GUIStyle(draftStyle));

            draftStyle = new GUIStyle(HighLogic.Skin.label);
            draftStyle.fontSize = 10;
            draftStyle.padding = new RectOffset(0, 0, 0, 0);
            draftStyle.alignment = TextAnchor.UpperLeft;
            draftStyle.normal.textColor = Color.white;
            draftStyle.stretchWidth = true;
            styleList.Add("mini_text_header", new GUIStyle(draftStyle));

            draftStyle = new GUIStyle(HighLogic.Skin.label);
            draftStyle.fontSize = 10;
            draftStyle.padding = new RectOffset(0, 0, 0, 0);
            draftStyle.alignment = TextAnchor.UpperRight;
            draftStyle.normal.textColor = Color.white;
            draftStyle.stretchWidth = true;
            styleList.Add("mini_text_body", new GUIStyle(draftStyle));

            draftStyle = new GUIStyle(HighLogic.Skin.button);
            draftStyle.fontSize = 9;
            draftStyle.padding = new RectOffset(0, 0, 0, 0);
            styleList.Add("mini_button", new GUIStyle(draftStyle));


            // Roster Window
            draftStyle = new GUIStyle(HighLogic.Skin.window);
            styleList.Add("roster_window", new GUIStyle(draftStyle));

            draftStyle = new GUIStyle(HighLogic.Skin.window);
            styleList.Add("roster_group", new GUIStyle(draftStyle));
            draftStyle = new GUIStyle(HighLogic.Skin.textArea);
            styleList.Add("roster_header", new GUIStyle(draftStyle));
            draftStyle = new GUIStyle(HighLogic.Skin.label);
            draftStyle.fontSize = 11;
            styleList.Add("roster_body", new GUIStyle(draftStyle));

            draftStyle = new GUIStyle(HighLogic.Skin.label);
            styleList.Add("roster_value", new GUIStyle(draftStyle));
            draftStyle = new GUIStyle(HighLogic.Skin.button);
            styleList.Add("roster_button", new GUIStyle(draftStyle));

            draftStyle = new GUIStyle(HighLogic.Skin.textField);
            draftStyle.active = draftStyle.hover = draftStyle.normal;
            styleList.Add("roster_bar_bg", new GUIStyle(draftStyle));
            draftStyle = new GUIStyle(HighLogic.Skin.button);
            draftStyle.active = draftStyle.hover = draftStyle.normal;
            draftStyle.border = GetStyle("roster_bar_bg").border;
            draftStyle.padding = new RectOffset(0, 0, 0, 0);

            styleList.Add("roster_bar_fg", new GUIStyle(draftStyle));

            draftStyle = new GUIStyle(HighLogic.Skin.horizontalSlider);

            styleList.Add("roster_slider", new GUIStyle(draftStyle));
            draftStyle = new GUIStyle(HighLogic.Skin.horizontalSliderThumb);
            styleList.Add("roster_slider_thumb", new GUIStyle(draftStyle));

            draftStyle = new GUIStyle(HighLogic.Skin.label);
            draftStyle.fontSize = 13;
            draftStyle.padding = new RectOffset(5, 0, 0, 0);
            draftStyle.alignment = TextAnchor.UpperLeft;
            draftStyle.normal.textColor = Color.white;
            draftStyle.stretchWidth = true;
            styleList.Add("editor_text", new GUIStyle(draftStyle));

            draftStyle = new GUIStyle(HighLogic.Skin.label);
            draftStyle.fontSize = 13;
            draftStyle.padding = new RectOffset(0, 0, 0, 0);
            draftStyle.alignment = TextAnchor.UpperLeft;
            draftStyle.normal.textColor = Color.white;
            draftStyle.stretchWidth = true;
            styleList.Add("editor_header", new GUIStyle(draftStyle));

        }
        void CreateColorList()
        {
            colorList = new Dictionary<string, Color>();

            colorList.Add("cancel_color", new Color(208f / 255f, 131f / 255f, 86f / 255f));
            colorList.Add("accept_color", new Color(209f / 255f, 250f / 255f, 146f / 255f));
            colorList.Add("capacitor_blue", new Color(134f / 255f, 197f / 255f, 239f / 255f));
            colorList.Add("readout_green", new Color(203f / 255f, 238f / 255f, 115f / 255f));
        }
    }
    // Represents an atlased icon via a source texture and rectangle
    public class AtlasIcon
    {
        public Texture iconAtlas;
        public Rect iconRect;

        public AtlasIcon(Texture theAtlas, float bl_x, float bl_y, float x_size, float y_size)
        {
            iconAtlas = theAtlas;
            iconRect = new Rect(bl_x, bl_y, x_size, y_size);
        }
    }
}
