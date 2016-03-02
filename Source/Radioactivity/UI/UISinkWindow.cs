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

    bool showWindow = false;
    bool showDetails = false;
    bool showRays = false;

    int windowID;

    Vector2 iconDims = new Vector2(40f, 40f);
    Vector2 windowDims = new Vector2(200f, 120f);


    Vector3 worldPosition;
    Vector3 screenPosition;
    Rect windowPosition;
    RadioactiveSink sink;

    GUIStyle windowStyle;
    GUIStyle groupStyle;
    GUIStyle textHeaderStyle;
    GUIStyle textDescriptorStyle;
    GUIStyle buttonStyle;

    public UISinkWindow(RadioactiveSink snk, System.Random random)
    {
      sink = snk;
      windowID = random.Next();
      // Set up screen position
      screenPosition = Camera.main.WorldToScreenPoint(sink.part.transform.position);
      windowPosition = new Rect(screenPosition.x + 50f, Screen.height - screenPosition.y + windowDims.y / 2f, windowDims.x, windowDims.y);
      GetStyles();
      Debug.Log("dn");
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
        // Set up screen position
        screenPosition = Camera.main.WorldToScreenPoint(sink.part.transform.position);
        windowPosition = new Rect(screenPosition.x + 50f, Screen.height - screenPosition.y - windowDims.y / 2f, windowDims.x, windowDims.y);
    }

    public void Draw()
    {
        if (showWindow)
            windowPosition = GUILayout.Window(windowID, windowPosition, DrawWindow, new GUIContent(sink.part.partInfo.title), windowStyle, GUILayout.MinHeight(20), GUILayout.ExpandHeight(true));
        if (screenPosition.z > 0f)
            DrawButton();
    }
    internal void DrawButton()
    {
        if (GUI.Button(new Rect(screenPosition.x - iconDims.x / 2f, Screen.height - screenPosition.y - iconDims.y / 2f, iconDims.x, iconDims.y), ""))
        {
            showWindow = !showWindow;
        }
    }

    internal void DrawWindow(int WindowID)
    {
      GUILayout.BeginVertical(groupStyle);
      GUILayout.BeginHorizontal();
      GUILayout.Label("Dose at surface", textHeaderStyle);
      GUILayout.Label(sink.CurrentRadiation.ToString(), textDescriptorStyle);
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal();
      GUILayout.Label("Affected", textHeaderStyle);
      GUILayout.Label(sink.GetAbsorberAliases(),textDescriptorStyle);
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
