using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Radioactivity.UI
{
  public class UISinkWindow
  {

      public RadioactiveSink Sink {
          get { return sink; }
      }

      bool showSourceInfo = false;
      bool showSinkInfo = false;

    bool showWindow = false;

    int windowID;

    Vector2 iconDims = new Vector2(32f, 32f);
    Vector2 infoBarDims = new Vector2(16f, 16f);
    Vector2 windowDims = new Vector2(150f, 20f);


    Vector3 worldPosition;
    Vector3 screenPosition;
    Rect windowPosition;
    RadioactiveSink sink;

    GUIStyle windowStyle;
    GUIStyle groupStyle;
    GUIStyle textHeaderStyle;
    GUIStyle textDescriptorStyle;
    GUIStyle buttonStyle;

    Rect atlasIconRect;
    Texture atlas;

    public UISinkWindow(RadioactiveSink snk, System.Random random, Texture iconAtlas)
    {
      sink = snk;
      atlas = iconAtlas;
      windowID = random.Next();
      // Set up screen position
      screenPosition = Camera.main.WorldToScreenPoint(sink.part.transform.position);
      windowPosition = new Rect(screenPosition.x + 50f, Screen.height - screenPosition.y + windowDims.y / 2f, windowDims.x, windowDims.y);
      GetStyles();

      if (sink.IconID == 0)
        atlasIconRect = new Rect(0f,0.5f,0.5f,0.5f);
      if (sink.IconID == 1)
          atlasIconRect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
      if (sink.IconID == 2)
          atlasIconRect = new Rect(0f, 0.0f, 0.5f, 0.5f);
      if (sink.IconID == 3)
          atlasIconRect = new Rect(0.5f, 0.0f, 0.5f, 0.5f);
    }

    internal void GetStyles()
    {
        windowStyle = new GUIStyle(HighLogic.Skin.window);
        windowStyle.fontSize = 10;
        windowStyle.normal.background = null;
        windowStyle.padding = new RectOffset(0,0,0,0);

        groupStyle = new GUIStyle(HighLogic.Skin.textArea);
        groupStyle.normal.background = HighLogic.Skin.window.normal.background;
        groupStyle.padding = new RectOffset(0, 0, 0, 0);

        textHeaderStyle = new GUIStyle(HighLogic.Skin.label);
        textHeaderStyle.normal.textColor = Color.white;
        textHeaderStyle.fontSize = 10;
        textHeaderStyle.stretchWidth = true;
        textHeaderStyle.alignment = TextAnchor.UpperLeft;
        textHeaderStyle.padding = new RectOffset(0,0,0,0);
        textDescriptorStyle = new GUIStyle(HighLogic.Skin.label);
        textDescriptorStyle.alignment = TextAnchor.UpperRight;
        textDescriptorStyle.stretchWidth = true;
        textDescriptorStyle.fontSize = 10;
        textDescriptorStyle.padding = new RectOffset(0, 0, 0, 0);
        buttonStyle = new GUIStyle(HighLogic.Skin.button);
        buttonStyle.fontSize = 8;
        buttonStyle.padding = new RectOffset(0, 0, 0, 0);
    }

    public void UpdatePositions()
    {
        // Set up screen position
        screenPosition = Camera.main.WorldToScreenPoint(sink.part.transform.position);
        windowPosition = new Rect(screenPosition.x + iconDims.x/2+5f, Screen.height - screenPosition.y + iconDims.y / 2f, windowDims.x, windowDims.y);
    }

    public void Draw()
    {
        if (showWindow)
            windowPosition = GUILayout.Window(windowID, windowPosition, DrawWindow, "", windowStyle, GUILayout.MinHeight(20), GUILayout.ExpandHeight(true));
        if (screenPosition.z > 0f)
            DrawButton();
    }

    internal void DrawButton()
    {
        Rect buttonRect = new Rect(screenPosition.x - iconDims.x / 2f, Screen.height - screenPosition.y - iconDims.y / 2f, iconDims.x, iconDims.y);
        Rect labelRect = new Rect (buttonRect.xMax+5f, buttonRect.yMin+buttonRect.height/2-infoBarDims.y/2f, 110f, infoBarDims.y);

        GUI.DrawTextureWithTexCoords(buttonRect, atlas, atlasIconRect);
        GUILayout.BeginArea(labelRect,groupStyle);
        GUILayout.BeginHorizontal();
        GUILayout.Label(String.Format("{0}Sv/s", Utils.ToSI(sink.CurrentRadiation, "F2")), textDescriptorStyle, GUILayout.MinWidth(60f));
        if (GUILayout.Button("...", buttonStyle,GUILayout.Width(12),GUILayout.Height(12)))
        {
          showSinkInfo = !showSinkInfo;
          if (showSinkInfo && !showWindow)
            showWindow = true;
          if (!showSinkInfo && !showSourceInfo)
              showWindow = false;
        }
        if (GUILayout.Button("->", buttonStyle, GUILayout.Width(12), GUILayout.Height(12)))
        {
            showSourceInfo = !showSourceInfo;
          if (showSourceInfo && !showWindow)
            showWindow = true;
          if (!showSinkInfo && !showSourceInfo)
              showWindow = false;
        }

        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }


    internal void DrawWindow(int WindowID)
    {
        if (showSinkInfo)
          DrawSinkDetails();
        if (showSourceInfo)
          DrawSourceDetails();

    }

    internal void DrawSinkDetails()
    {
        GUILayout.Space(2f);
      GUILayout.BeginVertical(groupStyle);
      foreach (var kvp in sink.GetAbsorberDetails())
      {
          GUILayout.BeginHorizontal();
          GUILayout.Label(kvp.Key, textHeaderStyle);
          GUILayout.Label(kvp.Value, textDescriptorStyle);
          GUILayout.EndHorizontal();
      }
      GUILayout.EndVertical();
    }
    internal void DrawSourceDetails()
    {
        GUILayout.Space(2f);
      GUILayout.BeginVertical(groupStyle);
      foreach (var kvp in sink.GetSourceDictionary())
      {
          GUILayout.BeginHorizontal();
          GUILayout.Label("<b>" + kvp.Key + "</b>", textHeaderStyle);
          GUILayout.Label(String.Format("{0}Sv/s", Utils.ToSI(kvp.Value,"F2")), textDescriptorStyle);
          GUILayout.EndHorizontal();
      }
      GUILayout.EndVertical();
    }
  }
}
