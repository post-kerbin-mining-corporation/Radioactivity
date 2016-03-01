using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Radioactivity.UI;
{
  public class UISinkWindow
  {

    bool showDetails = false;
    bool showRays = false;
    Vector3 worldPosition;
    Vector2 screenPosition;
    Rect windowPostion;
    RadioactiveSink sink;
    GUIStyle windowStyle;
    GUIStyle groupStyle;
    GUIStyle textHeaderStyle;
    GUIStyle textDescriptorStyle;

    public UISinkWindow(RadioactiveSink snk)
    {
      sink = snk;
      // Set up screen position
      screenPosition = Camera.main.WorldToScreenPoint(sink.part.Transform.position);
      windowPostion = new Rect(screenPosition.x +50, screenPosition.y-50, 200f, 150f);
      GetStyles();
    }

    internal void GetStyles()
    {
      windowStyle = new GUIStyle(HighLogic.Skin.window);
    }

    public void UpdatePositions()
    {
      screenPosition = Camera.main.WorldToScreenPoint(sink.part.Transform.position);
      windowPostion = new Rect(screenPosition.x +50, screenPosition.y-50, 200f, 150f);
    }

    public void Draw()
    {
      windowPos= GUILayout.Window(947695, windowPos, DrawWindow, sink.part.name, windowStyle, GUILayout.MinHeight(20), GUILayout.ExpandHeight(true));

    }

    internal void DrawWindow()
    {
      GUILayout.BeginVertical(groupStyle);
      GUILayout.BeginHorizontal();
      GUILayout.Label("Dose at surface", textHeaderStyle);
      GUILayout.Label(sink.CurrentRadiation, textDescriptorStyle);
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal();
      GUILayout.Label("Affected", textHeaderStyle);
      GUILayout.Label(sink.GetAbsorberAliases(),textDescriptorStyle);
      GUILayout.EndHorizontal();
      GUILayout.EndGroup();

      GUILayout.BeginHorizontal();
      showDetails = GUILayout.Toggle("Details");
      showRays = GUILayout.Toggle("Rays");
      GUILayout.EndHorizontal();
    }

    internal void DrawDetails()
    {
      GUILayout.BeginVertical(groupStyle);
      sink.DrawSinkUIDetails();
      GUILayout.EndVertical();
    }
  }
}
