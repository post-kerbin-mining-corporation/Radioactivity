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
    Rect windowPosition =  new Rect(210, 15, 350, 150);;

    int modeFlag = 0;
    string[] modeStrings = new string[] {"CURRENT","NEARBY","ACTIVE","ALL"};
    Vector2 scrollPosition;

    GUIStyle windowStyle;
    GUIStyle groupStyle;
    GUIStyle nameStyle;


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
      nameStyle = new GUIStyle(HighLogic.Skin.Label);
    }

    public void Draw()
    {
        windowPosition = GUILayout.Window(windowIdentifier, windowPos, DrawWindow, "Radioactivity Roster", windowStyle, GUILayout.MinHeight(20), GUILayout.ExpandHeight(true));
    }
    public void Update()
     {
       switch (modeFlag)
       {
         case 0:
          break;
         case 1:
          break;
         case 2:
          break;
         case 3:
          break;
       }
     }

     void DrawWindow(int WindowID)
     {
        DrawModeBar();
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(350), GUILayout.MinHeight(150));
        DrawKerbalList();
        GUILayout.EndScrollView()
     }

     void DrawModeBar()
     {
       modeFlag = GUILayout.SelectionGrid(modeFlag, modeStrings, 4);
     }

     void DrawKerbalList()
     {
       foreach (RadioactivityKerbal kerbal in drawnKerbals)
       {
          DrawKerbalInfo(kerbal);
       }

     }

     void DrawKerbalInfo(RadioactivityKerbal kerbal)
     {
       GUILayout.BeginHorizontal();
       GUILayout.Label(kerbal.name, nameStyle);

       GUILayout.EndHorizontal();

     }
  }
}
