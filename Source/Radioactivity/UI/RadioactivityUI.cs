using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Radioactivity;
using KSP.UI.Screens;
using Radioactivity.Simulator;

namespace Radioactivity.UI
{
    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public class RadioactivityUI : MonoBehaviour
    {
        public UIOverlayWindow OverlayWindow { get { return overlayWindow; } }
        public UIEditorWindow EditorWindow { get { return editorWindow; } }
        public UIRosterWindow RosterWindow { get { return rosterWindow; } }

        public UIResources GUIResources { get { return resources; } }

        public static RadioactivityUI Instance { get; private set; }


        private UIResources resources;

        private bool uiShown = false;
        private bool initStyles = false;

        private Rect mainWindowPos = new Rect(5, 15, 150, 120);
        private Rect rosterWindowPos = new Rect(210, 15, 350, 450);

        private UIOverlayWindow overlayWindow;
        private UIEditorWindow editorWindow;
        private UIRosterWindow rosterWindow;

        System.Random randomizer;
        int windowIdentifier;

        // Stock toolbar button
        private static ApplicationLauncherButton stockToolbarButton = null;

        public void Awake()
        {
            Instance = this;
            Utils.Log("UI: Awake");
            GameEvents.onGUIApplicationLauncherReady.Add(OnGUIAppLauncherReady);
            GameEvents.onGUIApplicationLauncherDestroyed.Add(OnGUIAppLauncherDestroyed);
        }

        /// <summary>
        /// Sets up the UI and creates the windows
        /// </summary>
        public void Start()
        {
            Utils.Log("UI: Start");
            if (ApplicationLauncher.Ready)
                OnGUIAppLauncherReady();

            randomizer = new System.Random(335462);

            windowIdentifier = randomizer.Next();
            overlayWindow = new UIOverlayWindow(randomizer, this);
            rosterWindow = new UIRosterWindow(randomizer, this);
            editorWindow = new UIEditorWindow(randomizer, this);

            overlayWindow.Drawn = RadioactivityPreferences.overlayShown;
            editorWindow.Drawn = RadioactivityPreferences.editorShown;
            rosterWindow.Drawn = RadioactivityPreferences.rosterShown;
        }

        /// <summary>
        /// Loads the GUI styles
        /// </summary>
        private void InitStyles()
        {
            resources = new UIResources();
            initStyles = true;
        }

        /// <summary>
        /// Propagates Mono Updates to the windows
        /// </summary>
        public void Update()
        {
            overlayWindow.Update();
            editorWindow.Update();
            rosterWindow.Update();
        }

        /// <summary>
        /// Propagates Mono OnGUI updates to the windows
        /// </summary>
        public void OnGUI()
        {
            if (Event.current.type == EventType.Repaint || Event.current.isMouse)
            {
            }
            OnUIDraw();
        }

        /// <summary>
        /// Draws all the UI components
        /// </summary>
        public void OnUIDraw()
        {
            if (!initStyles)
                InitStyles();

            // Draw the main window which is the options window
            if (uiShown)
            {
                mainWindowPos = GUILayout.Window(windowIdentifier,
                                                mainWindowPos,
                                                DrawMainWindow,
                                                "Radioactivity",
                                                resources.GetStyle("main_window"),
                                                GUILayout.MinHeight(20),
                                                GUILayout.ExpandHeight(true));
            }

            /// We can draw these anytime without the options window
            DrawOverlay();
            DrawEditor();
            DrawRoster();
        }

        /// <summary>
        /// Draws the settings window
        /// </summary>
        /// <param name="WindowID">Window identifier.</param>
        public void DrawMainWindow(int WindowID)
        {
            GUILayout.BeginVertical();
            if (HighLogic.LoadedSceneIsFlight || HighLogic.LoadedSceneIsEditor)
            {
                overlayWindow.Drawn = GUILayout.Toggle(overlayWindow.Drawn, "Overlay", resources.GetStyle("main_button"));
            }
            rosterWindow.Drawn = GUILayout.Toggle(rosterWindow.Drawn, "Roster", resources.GetStyle("main_button"));
            if (HighLogic.LoadedSceneIsEditor)
            {
                editorWindow.Drawn = GUILayout.Toggle(editorWindow.Drawn, "Simulation", resources.GetStyle("main_button"));
            }

            GUILayout.EndVertical();
            GUI.DragWindow();
        }

        /// <summary>
        /// Draws the overlay window(s)
        /// </summary>
        internal void DrawOverlay()
        {
            overlayWindow.Draw();
        }
        /// <summary>
        /// Draws the roster window
        /// </summary>
        internal void DrawRoster()
        {
            rosterWindow.Draw();
        }
        /// <summary>
        /// Draws the editor settings window
        /// </summary>
        internal void DrawEditor()
        {
            editorWindow.Draw();
        }

        // These methods are called to inform the
        // UI that the network has changed
        // --------------------------------------
        public void SinkAdded(RadioactiveSink snk)
        {
            if (overlayWindow != null)
                overlayWindow.UpdateSinkList();
        }
        public void SinkRemoved(RadioactiveSink snk)
        {
            if (overlayWindow != null)
                overlayWindow.UpdateSinkList();
        }
        public void SourceRemoved(RadioactiveSource src)
        {
            if (overlayWindow != null)
                overlayWindow.UpdateSourceList();
        }
        public void SourceAdded(RadioactiveSource src)
        {
            if (overlayWindow != null)
                overlayWindow.UpdateSourceList();
        }
        public void LinkAdded(RadiationLink lnk)
        {

        }
        public void LinkRemoved(RadiationLink lnk)
        {

        }

        // App Launchers
        // -------------
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
        private void OnToolbarButtonOn()
        {
            uiShown = true;
            stockToolbarButton.SetTexture((Texture)GameDatabase.Instance.GetTexture(uiShown ? "Radioactivity/UI/toolbar_on" : "Radioactivity/UI/toolbar_off", false));
            //if (overlayShown)
            //Radioactivity.Instance.ShowAllOverlays();
        }
        private void OnToolbarButtonOff()
        {
            uiShown = false;
            stockToolbarButton.SetTexture((Texture)GameDatabase.Instance.GetTexture(uiShown ? "Radioactivity/UI/toolbar_on" : "Radioactivity/UI/toolbar_off", false));
            //Radioactivity.Instance.HideAllOverlays();

        }

        void OnGUIAppLauncherReady()
        {
            if (stockToolbarButton == null)
            {
                stockToolbarButton = ApplicationLauncher.Instance.AddModApplication(
                    OnToolbarButtonOn,
                    OnToolbarButtonOff,
                    DummyVoid,
                    DummyVoid,
                    DummyVoid,
                    DummyVoid,
                    ApplicationLauncher.AppScenes.SPACECENTER | ApplicationLauncher.AppScenes.TRACKSTATION | ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH | ApplicationLauncher.AppScenes.FLIGHT,
                    (Texture)GameDatabase.Instance.GetTexture("Radioactivity/UI/toolbar_off", false));
            }
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
