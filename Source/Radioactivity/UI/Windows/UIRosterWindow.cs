using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Radioactivity.Persistance;

namespace Radioactivity.UI
{
    public class UIRosterWindow : UIWindow
    {

        enum RosterWindowMode
        {
            Vessel, Nearby, Active, All
        }


        Rect windowPosition = new Rect(210, 15, 425, 250);

        int modeFlag = 0;
        string[] modeStrings = new string[] { "CURRENT", "NEARBY", "KSC", "ACTIVE", "ALL" };

        Vector2 scrollPosition;
        Vector2 iconDims = new Vector2(16f, 16f);


        List<RadioactivityKerbal> drawnKerbals = new List<RadioactivityKerbal>();

        public UIRosterWindow(System.Random randomizer, RadioactivityUI uiHost) : base(randomizer, uiHost)
        {
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
            if (HighLogic.LoadedSceneIsFlight)
            {
                drawnKerbals = RadioactivityPersistance.Instance.KerbalDB.VesselKerbals(FlightGlobals.ActiveVessel.GetVesselCrew());
            }
            else if (HighLogic.LoadedSceneIsEditor)
            {
                if (ShipConstruction.ShipManifest == null)
                {
                    drawnKerbals.Clear();
                }
                else
                {
                    drawnKerbals = RadioactivityPersistance.Instance.KerbalDB.VesselKerbals(ShipConstruction.ShipManifest.GetAllCrew(true));
                }
            }
            else
            {
                drawnKerbals.Clear();
            }
        }
        // Get all kerbals in the physics bubble
        internal void GetKerbalsLocal()
        {
            List<ProtoCrewMember> nearbyCrew = new List<ProtoCrewMember>();
            for (int i = 0; i < FlightGlobals.Vessels.Count; i++)
            {
                if (FlightGlobals.Vessels[i].loaded)
                    nearbyCrew.Concat(FlightGlobals.Vessels[i].GetVesselCrew());
            }
            drawnKerbals = RadioactivityPersistance.Instance.KerbalDB.NearbyKerbals(nearbyCrew);
        }
        // Get kerbals that are in flight
        internal void GetKerbalsActive()
        {
            drawnKerbals = RadioactivityPersistance.Instance.KerbalDB.ActiveKerbals();
        }
        internal void GetKerbalsKSC()
        {
            drawnKerbals = RadioactivityPersistance.Instance.KerbalDB.KSCKerbals();
        }
        // Get all kerbals
        internal void GetKerbalsAll()
        {
            drawnKerbals = RadioactivityPersistance.Instance.KerbalDB.AllKerbals();
        }


        public void Draw()
        {
            windowPosition = GUILayout.Window(windowID, windowPosition, DrawWindow, "Kerbal Roster", host.GUIResources.GetStyle("roster_window"), GUILayout.MinHeight(20), GUILayout.ExpandHeight(true));
        }

        void DrawWindow(int WindowID)
        {
            DrawModeBar();
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, HighLogic.Skin.verticalScrollbar, HighLogic.Skin.verticalScrollbar, GUILayout.Width(425), GUILayout.MinHeight(100), GUILayout.MaxHeight(300), GUILayout.ExpandHeight(true));
            DrawKerbalList();
            GUILayout.EndScrollView();
            GUI.DragWindow();
        }

        void DrawModeBar()
        {
            modeFlag = GUILayout.SelectionGrid(modeFlag, modeStrings, 4, host.GUIResources.GetStyle("roster_button"));
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
                GUILayout.Label("No Kerbals found", host.GUIResources.GetStyle("roster_body"));
            }

        }

        void DrawKerbalInfo(RadioactivityKerbal kerbal)
        {
            GUILayout.BeginHorizontal(host.GUIResources.GetStyle("roster_group"));


            GUILayout.Label("<b><color=#ffffff>" + kerbal.Name + "</color></b>", host.GUIResources.GetStyle("roster_body"));

            float tempAreaWidth = 325f;
            float tempBarWidth = 180f;
            Rect tempArea = GUILayoutUtility.GetRect(tempAreaWidth, 40f);
            Rect barArea = new Rect(20f, 20f, tempBarWidth, 40f);

            float sickIconPos = tempBarWidth * RadioactivityConstants.kerbalSicknessThreshold / RadioactivityConstants.kerbalDeathThreshold;
            float tempBarFGSize = (tempBarWidth - 4f) * Mathf.Clamp01((float)kerbal.TotalExposure / RadioactivityConstants.kerbalDeathThreshold);

            // Bars
            GUI.BeginGroup(tempArea);
            GUI.Box(new Rect(0f, 10f, tempBarWidth, 10f), "", host.GUIResources.GetStyle("roster_bar_bg"));

            // Colorize bar
            if (kerbal.TotalExposure < RadioactivityConstants.kerbalSicknessThreshold)
                GUI.color = Color.green;
            else if (kerbal.TotalExposure < RadioactivityConstants.kerbalDeathThreshold)
                GUI.color = Color.yellow;
            else
                GUI.color = Color.red;

            GUI.Box(new Rect(2f, 11f, tempBarFGSize, 7f), "", host.GUIResources.GetStyle("roster_bar_fg"));
            GUI.color = Color.white;


            // icons
            GUI.DrawTextureWithTexCoords(new Rect(sickIconPos - iconDims.x / 2f, iconDims.y / 2f - 1f, iconDims.x, iconDims.y),
                                         host.GUIResources.GetIcon("kerbal_sick").iconAtlas, host.GUIResources.GetIcon("kerbal_sick").iconRect);
            GUI.DrawTextureWithTexCoords(new Rect(tempBarWidth - iconDims.x / 2f, iconDims.y / 2f - 1f, iconDims.x, iconDims.y),
                                         host.GUIResources.GetIcon("kerbal_dead").iconAtlas, host.GUIResources.GetIcon("kerbal_dead").iconRect);

            // icon labels
            GUI.Label(new Rect(sickIconPos - iconDims.x, iconDims.y + 4f, iconDims.x * 2f, 20f), String.Format("{0}Sv", Utils.ToSI(RadioactivityConstants.kerbalSicknessThreshold, "F0")), host.GUIResources.GetStyle("roster_body"));
            GUI.Label(new Rect(tempBarWidth - iconDims.x, iconDims.y + 4f, iconDims.x * 2f, 20f), String.Format("{0}Sv", Utils.ToSI(RadioactivityConstants.kerbalDeathThreshold, "F0")), host.GUIResources.GetStyle("roster_body"));

            // End labels
            GUI.Label(new Rect(tempBarWidth + 20f, 2f, 120f, 20f), String.Format("<b><color=#ffffff>Current:</color></b> {0}Sv/s", Utils.ToSI(kerbal.CurrentExposure, "F2")), host.GUIResources.GetStyle("roster_body"));
            GUI.Label(new Rect(tempBarWidth + 20f, 20f, 120f, 20f), String.Format("<b><color=#ffffff>Total:</color></b> {0}Sv", Utils.ToSI(kerbal.TotalExposure, "F2")), host.GUIResources.GetStyle("roster_body"));


            GUI.EndGroup();

            GUILayout.EndHorizontal();

        }
    }
}
