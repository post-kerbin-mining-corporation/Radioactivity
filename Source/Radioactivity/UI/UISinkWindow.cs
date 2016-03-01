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

    Vector3 worldPosition;
    Vector2 screenPosition;
    Rect windowPosition;
    RadioactiveSink sink;

    GUIStyle windowStyle;
    GUIStyle groupStyle;
    GUIStyle textHeaderStyle;
    GUIStyle textDescriptorStyle;

    public UISinkWindow(RadioactiveSink snk, System.Random random)
    {
      sink = snk;
      windowID = random.Next();
      // Set up screen position
      screenPosition = Camera.main.WorldToScreenPoint(sink.part.transform.position);
      windowPosition = new Rect(screenPosition.x +50, screenPosition.y-50, 200f, 150f);
      GetStyles();
      Debug.Log("dn");
    }

    internal void GetStyles()
    {
      windowStyle = new GUIStyle(HighLogic.Skin.window);
      groupStyle = new GUIStyle(HighLogic.Skin.textArea);
      textHeaderStyle = new GUIStyle(HighLogic.Skin.label);
      textHeaderStyle.color = color.white;
      textHeaderStyle.stretchWidth = true;
      textHeaderStyle.alignment = TextAnchor.UpperLeft;
        textDescriptorStyle = new GUIStyle(HighLogic.Skin.label);
        textDescriptorStyle.alignment = TextAnchor.UpperRight;
        textDescriptorStyle.stretchWidth = true;

    }

    public void UpdatePositions()
    {
      screenPosition = Camera.main.WorldToScreenPoint(sink.part.partTransform.position);
      windowPosition = new Rect(screenPosition.x +50, screenPosition.y-50, 200f, 150f);
    }

    public void Draw()
    {
        if (showWindow)
            windowPosition = GUILayout.Window(windowID, windowPosition, DrawWindow, new GUIContent(sink.part.partInfo.title), windowStyle, GUILayout.MinHeight(20), GUILayout.ExpandHeight(true));

        DrawButton();
    }
    internal void DrawButton()
    {
        if (GUI.Button(new Rect(screenPosition.x - 50f, screenPosition.y - 50f, 50f, 50f), ""))
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
      showDetails = GUILayout.Toggle(showDetails, "Details");
      showRays = GUILayout.Toggle(showRays, "Rays");
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
