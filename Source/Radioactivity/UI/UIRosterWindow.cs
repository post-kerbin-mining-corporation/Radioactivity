using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Radioactivity.UI
{
  public class UIRosterWindow
  {

    enum RosterWindowMode{
      Vessel, Nearby, Active, All
    }

    System.Random random;
    int windowIdentifier;
    Rect windowPosition =  new Rect(210, 15, 350, 150);

    int modeFlag = 0;
    string[] modeStrings = new string[] {"CURRENT","NEARBY","ACTIVE","ALL"};
    Vector2 scrollPosition;

    GUIStyle windowStyle;
    GUIStyle groupStyle;
    GUIStyle nameStyle;
    GUIStyle labelStyle;
    GUIStyle valueStyle;
    GUIStyle buttonStyle;

    List<RadioactivityKerbal> drawnKerbals = new List<RadioactivityKerbal>();

    public UIRosterWindow(System.Random randomizer)
    {
      random = randomizer;
      windowIdentifier = randomizer.Next();
      GetStyles();
    }

    void GetStyles()
    {
      windowStyle = new GUIStyle(HighLogic.Skin.window);
      groupStyle = new GUIStyle(HighLogic.Skin.textArea);
      nameStyle = new GUIStyle(HighLogic.Skin.label);
      labelStyle = new GUIStyle(HighLogic.Skin.label);
      valueStyle = new GUIStyle(HighLogic.Skin.label);
      buttonStyle = new GUIStyle(HighLogic.Skin.button);
    }


    public void Update()
     {
       switch (modeFlag)
       {
         case 0:
          GetKerbalsVessel();
          break;
         case 1:
          GetKerbalsLocal();
          break;
         case 2:
         GetKerbalsActive();
          break;
         case 3:
          GetKerbalsAll();
          break;
       }
     }
     // Get kerbals in the vessel
     internal void GetKerbalsVessel()
     {

     }
     // Get all kerbals in the physics bubble
     internal void GetKerbalsLocal()
     {

     }
     // Get kerbals that are in flight
     internal void GetKerbalsActive()
     {

     }
     // Get all kerbals
     internal void GetKerbalsAll()
     {

     }


     public void Draw()
     {
         windowPosition = GUILayout.Window(windowIdentifier, windowPosition, DrawWindow, "Radioactivity Roster", windowStyle, GUILayout.MinHeight(20), GUILayout.ExpandHeight(true));
     }

     void DrawWindow(int WindowID)
     {
        DrawModeBar();
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, groupStyle, GUILayout.Width(350), GUILayout.MinHeight(150));
        DrawKerbalList();
        GUILayout.EndScrollView();
        GUI.DragWindow();
     }

     void DrawModeBar()
     {
       modeFlag = GUILayout.SelectionGrid(modeFlag, modeStrings, 4, buttonStyle);
     }

     void DrawKerbalList()
     {
         if (drawnKerbals.Count > 0)
         {
             foreach (RadioactivityKerbal kerbal in drawnKerbals)
             {
                 DrawKerbalInfo(kerbal);
             }
         }
         else
         {
             GUILayout.Label("No Kerbals found", labelStyle);
         }

     }

     void DrawKerbalInfo(RadioactivityKerbal kerbal)
     {
       GUILayout.BeginHorizontal();
       GUILayout.Label(kerbal.Name, nameStyle);

       GUILayout.EndHorizontal();

     }
  }
}
