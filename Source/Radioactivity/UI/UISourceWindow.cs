using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Radioactivity.UI
{
  public class UISourceWindow
  {
    public RadioactiveSource Source {
        get { return source; }
    }

    bool showWindow = false;
    bool showDetails = false;
    bool showRays = false;

    int windowID;

    Vector2 iconDims = new Vector2(40f, 40f);
    Vector2 windowDims = new Vector2(200f,120f);

    Vector3 worldPosition;
    Vector3 screenPosition;
    Rect windowPosition;
    RadioactiveSource source;

    Rect atlasIconRect;

    Texture atlas;
    GUIStyle windowStyle;
    GUIStyle groupStyle;
    GUIStyle buttonStyle;
    GUIStyle textHeaderStyle;
    GUIStyle textDescriptorStyle;

    public UISourceWindow(RadioactiveSource src, System.Random random, Texture iconAtlas)
    {
      source = src;
      atlas = iconAtlas;
      windowID = random.Next();
      // Set up screen position
      screenPosition = Camera.main.WorldToScreenPoint(source.part.transform.position);
      windowPosition = new Rect(screenPosition.x+50f, Screen.height-screenPosition.y+windowDims.y/2f, windowDims.x, windowDims.y);
      GetStyles();

      if (source.IconID == 0)
        atlasIconRect = new Rect(0f,0.5f,0.5f,0.5f);
      if (source.IconID == 1)
        atlasIconRect = new Rect(0.5f,0.5f,0.5f,0.5f);
      if (source.IconID == 2)
        atlasIconRect = new Rect(0f,0.0f,0.5f,0.5f);
      if (source.IconID == 3)
        atlasIconRect = new Rect(0.5f,0.0f,0.5f,0.5f);
    }

    internal void GetStyles()
    {
      windowStyle = new GUIStyle(HighLogic.Skin.window);
      groupStyle = new GUIStyle(HighLogic.Skin.textArea);
      textHeaderStyle = new GUIStyle(HighLogic.Skin.label);
      textHeaderStyle.normal.textColor = Color.white;
      textHeaderStyle.stretchWidth = true;
      textHeaderStyle.alignment = TextAnchor.UpperLeft;
        textDescriptorStyle = new GUIStyle(HighLogic.Skin.label);
        textDescriptorStyle.alignment = TextAnchor.UpperRight;
        textDescriptorStyle.stretchWidth = true;
        buttonStyle = new GUIStyle(HighLogic.Skin.button);
    }

    public void UpdatePositions()
    {
      screenPosition = Camera.main.WorldToScreenPoint(source.part.partTransform.position);
      windowPosition = new Rect(screenPosition.x+50f, Screen.height-screenPosition.y - windowDims.y/2f, windowDims.x, windowDims.y);
    }

    public void Draw()
    {
        if (showWindow)
            windowPosition = GUILayout.Window(windowID, windowPosition, DrawWindow, new GUIContent(source.part.partInfo.title), windowStyle, GUILayout.MinHeight(20), GUILayout.ExpandHeight(true));
        if (screenPosition.z > 0f)
            DrawButton();
    }
    internal void DrawButton()
    {
        GUI.DrawTextureWithTexCoords(new Rect(screenPosition.x - iconDims.x / 2f, Screen.height - screenPosition.y - iconDims.y / 2f, iconDims.x, iconDims.y), atlas, atlasIconRect);
        if (GUI.Button(new Rect(screenPosition.x - iconDims.x/2f, Screen.height-screenPosition.y-iconDims.y/2f, iconDims.x, iconDims.y), "",new GUIStyle()))
        {
            showWindow = !showWindow;
        }
    }

    internal void DrawWindow(int WindowID)
    {
      GUILayout.BeginVertical(groupStyle);
      GUILayout.BeginHorizontal();
      GUILayout.Label("Dose at emitter", textHeaderStyle);
      GUILayout.Label(source.CurrentEmission.ToString(), textDescriptorStyle);
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal();
      GUILayout.Label("Sources", textHeaderStyle);
      GUILayout.Label(source.GetEmitterAliases(),textDescriptorStyle);
      GUILayout.EndHorizontal();
      GUILayout.EndVertical();

      GUILayout.BeginHorizontal();
      showDetails = GUILayout.Toggle(showDetails, "DETAILS", buttonStyle);
      showRays = GUILayout.Toggle(showRays, "RAYS", buttonStyle);
      GUILayout.EndHorizontal();
      if (showDetails)
          DrawDetails();
    }

    internal void DrawDetails()
    {
      GUILayout.BeginVertical(groupStyle);

      GUILayout.EndVertical();
    }
  }
}
