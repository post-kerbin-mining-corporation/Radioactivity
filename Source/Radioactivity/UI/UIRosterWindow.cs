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
    Rect windowPosition =  new Rect(210, 15, 425, 250);

    int modeFlag = 0;
    string[] modeStrings = new string[] {"CURRENT","NEARBY","KSC","ACTIVE","ALL"};

    Texture atlas;

    Vector2 scrollPosition;
    Vector2 iconDims = new Vector2(16f, 16f);

    GUIStyle windowStyle;
    GUIStyle groupStyle;
    GUIStyle nameStyle;
    GUIStyle labelStyle;
    GUIStyle valueStyle;
    GUIStyle buttonStyle;
    GUIStyle barBGStyle;
    GUIStyle barFGStyle;
    GUIStyle sliderStyle;
    GUIStyle sliderThumbStyle;

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
      barBGStyle = new GUIStyle(HighLogic.Skin.textField);
      barFGStyle = new GUIStyle(HighLogic.Skin.button);
      sliderStyle = new GUIStyle(HighLogic.Skin.horizontalSlider);
      sliderThumbStyle = new GUIStyle(HighLogic.Skin.horizontalSliderThumb);


      labelStyle.fontSize = 11;

      barBGStyle.active = barBGStyle.hover = barBGStyle.normal;
      barFGStyle.active = barBGStyle.hover = barBGStyle.normal;
      barFGStyle.border = barBGStyle.border;
      barFGStyle.padding = new RectOffset(0,0,0,0);

      atlas = (Texture)GameDatabase.Instance.GetTexture("Radioactivity/UI/icon_kerbal_state", false);
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
          GetKerbalsKSC();
          break;
         case 3:
         GetKerbalsActive();
          break;
         case 4:
          GetKerbalsAll();
          break;
       }
     }
     // Get kerbals in the vessel
     internal void GetKerbalsVessel()
     {
         drawnKerbals = KerbalTracking.Instance.KerbalDB.VesselKerbals(FlightGlobals.ActiveVessel.GetVesselCrew());
     }
     // Get all kerbals in the physics bubble
     internal void GetKerbalsLocal()
     {
         List<ProtoCrewMember> nearbyCrew = new List<ProtoCrewMember>();
         for (int i = 0; i < FlightGlobals.Vessels.Count; i++)
         {
             nearbyCrew.Concat(FlightGlobals.Vessels[i].GetVesselCrew());
         }
         
         drawnKerbals = KerbalTracking.Instance.KerbalDB.NearbyKerbals(nearbyCrew);
     }
     // Get kerbals that are in flight
     internal void GetKerbalsActive()
     {
         drawnKerbals = KerbalTracking.Instance.KerbalDB.ActiveKerbals();
     }
     internal void GetKerbalsKSC()
     {
         drawnKerbals = KerbalTracking.Instance.KerbalDB.KSCKerbals();
     }
     // Get all kerbals
     internal void GetKerbalsAll()
     {
         drawnKerbals = KerbalTracking.Instance.KerbalDB.AllKerbals();
     }


     public void Draw()
     {
         windowPosition = GUILayout.Window(windowIdentifier, windowPosition, DrawWindow, "Kerbal Roster", windowStyle, GUILayout.MinHeight(20), GUILayout.ExpandHeight(true));
     }

     void DrawWindow(int WindowID)
     {
        DrawModeBar();
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, groupStyle, GUILayout.Width(425), GUILayout.MinHeight(100),GUILayout.MaxHeight(300),GUILayout.ExpandHeight(true));
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
       GUILayout.BeginHorizontal(groupStyle);


       GUILayout.Label("<b><color=#ffffff>" + kerbal.Name + "</color></b>", labelStyle);

       float tempAreaWidth = 325f;
       float tempBarWidth = 180f;
       Rect tempArea = GUILayoutUtility.GetRect(tempAreaWidth, 40f);
       Rect barArea = new Rect(20f, 20f, tempBarWidth, 40f);

       float sickIconPos = tempBarWidth * RadioactivitySettings.kerbalSicknessThreshold/RadioactivitySettings.kerbalDeathThreshold;
       float tempBarFGSize = (tempBarWidth-4f) * Mathf.Clamp01((float)kerbal.TotalExposure / RadioactivitySettings.kerbalDeathThreshold);

       // Bars
       GUI.BeginGroup(tempArea);
       GUI.Box(new Rect(0f, 10f, tempBarWidth, 10f), "", barBGStyle);

       // Colorize bar
       if (kerbal.TotalExposure < RadioactivitySettings.kerbalSicknessThreshold)
          GUI.color = Color.green;
       else if (kerbal.TotalExposure < RadioactivitySettings.kerbalDeathThreshold)
          GUI.color = Color.yellow;
       else
          GUI.color = Color.red;
          
       GUI.Box(new Rect(2f, 11f, tempBarFGSize, 7f), "", barFGStyle);
       GUI.color = Color.white;


       // icons
       GUI.DrawTextureWithTexCoords(new Rect(sickIconPos - iconDims.x / 2f,  iconDims.y / 2f-1f, iconDims.x, iconDims.y), atlas, new Rect(0.0f,0.0f,0.5f,0.5f));
       GUI.DrawTextureWithTexCoords(new Rect(tempBarWidth - iconDims.x / 2f,  iconDims.y / 2f-1f, iconDims.x, iconDims.y), atlas, new Rect(0.5f,0.0f,0.5f,0.5f));

       // icon labels
       GUI.Label(new Rect(sickIconPos - iconDims.x, iconDims.y +4f, iconDims.x*2f, 20f), String.Format("{0}Sv", Utils.ToSI(RadioactivitySettings.kerbalSicknessThreshold,"F0")), labelStyle);
       GUI.Label(new Rect(tempBarWidth - iconDims.x, iconDims.y +4f, iconDims.x*2f, 20f), String.Format("{0}Sv", Utils.ToSI(RadioactivitySettings.kerbalDeathThreshold,"F0")), labelStyle);

       // End labels
       GUI.Label(new Rect(tempBarWidth + 20f, 2f, 120f, 20f), String.Format("<b><color=#ffffff>Current:</color></b> {0}Sv/s", Utils.ToSI(kerbal.CurrentExposure, "F2")), labelStyle);
       GUI.Label(new Rect(tempBarWidth + 20f, 20f, 120f, 20f), String.Format("<b><color=#ffffff>Total:</color></b> {0}Sv", Utils.ToSI(kerbal.TotalExposure, "F2")), labelStyle);

       // GUI.Label(new Rect(20f+tempBarWidth, 30f, 40f, 20f), String.Format("{0:F0} K", meltdownTemp), gui_text);
       GUI.EndGroup();

       GUILayout.EndHorizontal();

     }
  }
}
