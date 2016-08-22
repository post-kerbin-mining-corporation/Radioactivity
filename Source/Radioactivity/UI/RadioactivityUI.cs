using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Radioactivity.UI;
using KSP.UI.Screens;

namespace Radioactivity.UI
{
    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public class RadioactivityUI:MonoBehaviour
    {

        private bool uiShown = false;
        private bool initStyles = false;

        private bool overlayShown = false;
        private bool rosterShown = false;

        private Rect mainWindowPos = new Rect(5, 15, 150, 120);
        private Rect rosterWindowPos = new Rect(210, 15, 350, 450);

        private GUIStyle entryStyle;
        private GUIStyle windowStyle;
        private GUIStyle buttonStyle;

        private UIOverlayWindow overlayView;
        private UIRosterWindow rosterView;

        System.Random randomizer;
        int windowIdentifier;

        // obsolete
        private Rect windowPos = new Rect(0, 0, 200, 480);
        private Rect linkWindowPos = new Rect(200, 0, 480, 200);
        private RadiationLink currentDrawnLink = null;
        // Stock toolbar button
        private static ApplicationLauncherButton stockToolbarButton = null;

        private void InitStyles()
        {
            entryStyle = new GUIStyle(HighLogic.Skin.textArea);
            entryStyle.active = entryStyle.hover = entryStyle.normal;
            windowStyle = new GUIStyle(HighLogic.Skin.window);
            buttonStyle = new GUIStyle(HighLogic.Skin.button);
            initStyles = true;

        }
        public void Awake()
        {
            Utils.Log("UI: Awake");
            GameEvents.onGUIApplicationLauncherReady.Add(OnGUIAppLauncherReady);
            GameEvents.onGUIApplicationLauncherDestroyed.Add(OnGUIAppLauncherDestroyed);
        }


        public void Start()
        {
            Utils.Log("UI: Start");

            if (ApplicationLauncher.Ready)
                OnGUIAppLauncherReady();

            randomizer = new System.Random(335462);

            windowIdentifier = randomizer.Next();
            overlayView = new UIOverlayWindow(randomizer);
            rosterView = new UIRosterWindow(randomizer);
        }
        public void Update()
        {
            if (overlayShown)
                overlayView.Update();
            if (rosterShown)
              rosterView.Update();
        }
        public void OnGUI()
        {
            if (Event.current.type == EventType.Repaint || Event.current.isMouse)
            {
            }
            OnUIDraw();
        }

        public void OnUIDraw()
        {
            if (!initStyles)
                InitStyles();
            if (uiShown)
            {
                mainWindowPos= GUILayout.Window(windowIdentifier, mainWindowPos, DrawMainWindow, "Radioactivity", windowStyle, GUILayout.MinHeight(20), GUILayout.ExpandHeight(true));

                if (overlayShown)
                    DrawOverlay();

                if (rosterShown)
                  DrawRoster();
            }
        }

        public void DrawMainWindow(int WindowID)
        {
            GUILayout.BeginVertical();
            if(GUILayout.Toggle(overlayShown, "Overlay", buttonStyle))
            {
                overlayShown = !overlayShown;
                Radioactivity.Instance.ShowAllOverlays();
            }
            if(GUILayout.Toggle(rosterShown, "Roster", buttonStyle))
            {
              rosterShown = !rosterShown;

            }
            GUILayout.EndVertical();
            GUI.DragWindow();
        }

        internal void DrawOverlay()
        {
            overlayView.Draw();
        }
        internal void DrawRoster()
        {
            rosterView.Draw();
        }


        public void OnDestroy()
        {

            // Remove the stock toolbar button
            GameEvents.onGUIApplicationLauncherReady.Remove(OnGUIAppLauncherReady);
            if (stockToolbarButton != null)
            {
                ApplicationLauncher.Instance.RemoveModApplication(stockToolbarButton);
            }

        }

        private void OnToolbarButtonToggle()
        {
            uiShown = !uiShown;
            stockToolbarButton.SetTexture((Texture)GameDatabase.Instance.GetTexture(uiShown ? "Radioactivity/UI/toolbar_on" : "Radioactivity/UI/toolbar_off", false));

        }


        void OnGUIAppLauncherReady()
        {
            stockToolbarButton = ApplicationLauncher.Instance.AddModApplication(
                OnToolbarButtonToggle,
                OnToolbarButtonToggle,
                DummyVoid,
                DummyVoid,
                DummyVoid,
                DummyVoid,
                ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH | ApplicationLauncher.AppScenes.FLIGHT,
                (Texture)GameDatabase.Instance.GetTexture("Radioactivity/UI/toolbar_off", false));
        }

        void OnGUIAppLauncherDestroyed()
        {
            if (stockToolbarButton != null)
            {
                ApplicationLauncher.Instance.RemoveModApplication(stockToolbarButton);
                stockToolbarButton = null;
            }
        }

        void onAppLaunchToggleOff()
        {
            stockToolbarButton.SetTexture((Texture)GameDatabase.Instance.GetTexture("Radioactivity/UI/toolbar_off", false));
        }

        void DummyVoid() { }
    }
}
