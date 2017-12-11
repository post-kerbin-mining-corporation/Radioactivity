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
        public int UnitMode { get { return unitMode; }}
        public UIResources GUIResources { get { return resources; } }

        public static RadioactivityUI Instance { get; private set; }

        private int unitMode = 0;
        private string[] unitModeStrings = new string[] { "Flux", "Time to Sickness", "Time to Death"};

        private UIResources resources;

        private bool uiShown = false;
        private bool initStyles = false;

        private Rect mainWindowPos = new Rect(5, 15, 150, 120);

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
            LogUtils.Log("UI: Awake");
            GameEvents.onGUIApplicationLauncherReady.Add(OnGUIAppLauncherReady);
            GameEvents.onGUIApplicationLauncherDestroyed.Add(OnGUIAppLauncherDestroyed);
        }

        /// <summary>
        /// Sets up the UI and creates the windows
        /// </summary>
        public void Start()
        {
            LogUtils.Log("UI: Start");
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
            unitMode = RadioactivityPreferences.unitMode;
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

            if (uiShown)
            {
                Vector3 pos = stockToolbarButton.GetAnchor();
                if (ApplicationLauncher.Instance.IsPositionedAtTop)
                {
                    mainWindowPos = new Rect(Screen.width - 160f, 0f, 120f, 100f);
                }
                else
                {
                    mainWindowPos = new Rect(Screen.width - 240f, Screen.height - 150f, 120f, 100f);
                }
            }
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
            {
                InitStyles();
             
            }
            GUI.skin = HighLogic.Skin;

            // Draw the main window which is the options window
            if (uiShown)
            {
                mainWindowPos = GUILayout.Window(windowIdentifier,
                                                mainWindowPos,
                                                DrawMainWindow,
                                                "",
                                                resources.GetStyle("main_window"));
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
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            if ((HighLogic.LoadedSceneIsFlight && !MapView.MapIsEnabled) || HighLogic.LoadedSceneIsEditor)
            {
                overlayWindow.Drawn = GUILayout.Toggle(overlayWindow.Drawn, "Overlay", resources.GetStyle("main_button"));
                RadioactivityOverlay.Instance.SetEnabled(overlayWindow.Drawn);
            }
            rosterWindow.Drawn = GUILayout.Toggle(rosterWindow.Drawn, "Roster", resources.GetStyle("main_button"));
            if (HighLogic.LoadedSceneIsEditor && RadioactivitySimulationSettings.SimulateAmbientRadiation)
            {
                editorWindow.Drawn = GUILayout.Toggle(editorWindow.Drawn, "Simulation", resources.GetStyle("main_button"));
            }

            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            GUILayout.Label("Overlay Unit\n Display Mode", resources.GetStyle("mini_text_header"));
            if (GUILayout.Button(unitModeStrings[unitMode], resources.GetStyle("main_button")))
            {
                unitMode++;
                if (unitMode >= unitModeStrings.Length)
                    unitMode = 0;
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
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
            if (src.ShadowShields.Count > 0)
            {
                for (int i = 0; i< src.ShadowShields.Count;i++)
                {
                    RadioactivityOverlay.Instance.RemoveShadowShield(src.ShadowShields[i]);
                }
            }
        }
        public void SourceAdded(RadioactiveSource src)
        {
            if (overlayWindow != null)
                overlayWindow.UpdateSourceList();
            if (src.ShadowShields.Count > 0)
            {
                for (int i = 0; i < src.ShadowShields.Count; i++)
                {
                    RadioactivityOverlay.Instance.AddShadowShield(src.ShadowShields[i], src);
                }
            }
        }
        public void LinkAdded(RadiationLink lnk)
        {
            RadioactivityOverlay.Instance.AddLink(lnk);
        }
        public void LinkRemoved(RadiationLink lnk)
        {
            RadioactivityOverlay.Instance.RemoveLink(lnk);
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
        }
        private void OnToolbarButtonOff()
        {
            uiShown = false;
            stockToolbarButton.SetTexture((Texture)GameDatabase.Instance.GetTexture(uiShown ? "Radioactivity/UI/toolbar_on" : "Radioactivity/UI/toolbar_off", false));
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
