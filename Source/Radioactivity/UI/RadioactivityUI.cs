using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Radioactivity
{
    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public class RadioactivityUI:MonoBehaviour
    {

        private bool uiShown = false;
        private bool initStyles = false;

        private bool overlayShown = false;
        private bool rosterShown = false;

        private Rect mainWindowPos = new Rect(5, 15, 200, 300);
        private Rect rosterWindowPos = new Rect(210, 15, 350, 450);

        private GUIStyle entryStyle;
        private GUIStyle windowStyle;

        private Rect windowPos = new Rect(0, 0, 600, 480);
        private Rect linkWindowPos = new Rect(200, 0, 480, 200);
        private RadiationLink currentDrawnLink = null;
        // Stock toolbar button
        private static ApplicationLauncherButton stockToolbarButton = null;

        private void InitStyles()
        {
            entryStyle = new GUIStyle(HighLogic.Skin.textArea);
            entryStyle.active = entryStyle.hover = entryStyle.normal;
            windowStyle = new GUIStyle(HighLogic.Skin.window);
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

            try
            {
                RenderingManager.RemoveFromPostDrawQueue(0, OnUIDraw);
            }
            catch
            {
            }
            if (HighLogic.LoadedSceneIsEditor || HighLogic.LoadedSceneIsFlight)
            {
                RenderingManager.AddToPostDrawQueue(0, OnUIDraw);
            }
            if (ApplicationLauncher.Ready)
                OnGUIAppLauncherReady();
        }

        public void OnUIDraw()
        {
            if (!initStyles)
                InitStyles();
            if (uiShown)
            {
                //windowPos= GUILayout.Window(947695, windowPos, DrawWindow, "Radioactivity", windowStyle, GUILayout.MinHeight(20), GUILayout.ExpandHeight(true));

                //if (currentDrawnLink != null)
                //    linkWindowPos = GUILayout.Window(947696, linkWindowPos, DrawLinkWindow, "Path Details", windowStyle, GUILayout.MinHeight(20), GUILayout.ExpandHeight(true));
            }
        }

        public void DrawMainWindow(int WindowID)
        {

        }

        public void Draw

        // OLD PAST HERE

        private void DrawWindow(int windowID)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Hide Overlay"))
                Radioactivity.Instance.HideAllOverlays();
            GUILayout.Space(40f);
            if (GUILayout.Button("Show Overlay"))
                Radioactivity.Instance.ShowAllOverlays();
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Force Recalculate"))
                Radioactivity.Instance.ForceRecomputeNetwork();

            GUILayout.BeginHorizontal();


            GUILayout.BeginVertical();
            GUILayout.Label("Source List: Count = " + Radioactivity.Instance.AllSources.Count.ToString());
            foreach (RadioactiveSource src in Radioactivity.Instance.AllSources)
            {
                DrawSourceInfo(src);
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Label("Sink List: Count = " + Radioactivity.Instance.AllSinks.Count.ToString());
            foreach (RadioactiveSink snk in Radioactivity.Instance.AllSinks)
            {
                DrawSinkInfo(snk);
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Label("Link List: Count = " + Radioactivity.Instance.AllLinks.Count.ToString());
            foreach (RadiationLink lnk in Radioactivity.Instance.AllLinks)
            {
                DrawLinkInfo(lnk);
            }
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
            GUI.DragWindow();
        }

        private void DrawSourceInfo(RadioactiveSource src)
        {
            if (src != null)
            {
                GUILayout.BeginHorizontal(entryStyle);
                GUILayout.BeginVertical();
                GUILayout.Label("Emitter ID: " + src.SourceID);
                GUILayout.Label("On Part: " + src.part.name);
                GUILayout.EndVertical();
                GUILayout.Label("Strength: " + src.CurrentEmission.ToString());
                GUILayout.EndHorizontal();
            }
        }
        private void DrawSinkInfo(RadioactiveSink snk)
        {
            if (snk != null)
            {
                GUILayout.BeginHorizontal(entryStyle);
                GUILayout.BeginVertical();
                GUILayout.Label("Sink: " + snk.SinkID);
                GUILayout.Label("On: " + snk.part.name);
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
        }
        private void DrawLinkInfo(RadiationLink lnk)
        {
            GUILayout.BeginHorizontal(entryStyle);
            GUILayout.BeginVertical();
            GUILayout.Label(lnk.source.SourceID  + " to " + lnk.sink.SinkID);
            GUILayout.Label("End intensity: " + lnk.fluxEndScale.ToString());
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            GUILayout.Label("Zones: " + lnk.ZoneCount.ToString());
            GUILayout.Label("Occluders: " + lnk.OccluderCount.ToString());
            if (GUILayout.Button("Path Details"))
            {
                if (currentDrawnLink != null)
                {
                    currentDrawnLink = null;
                } else
                {
                    Rect x = GUILayoutUtility.GetLastRect();
                    linkWindowPos.y = x.y + windowPos.y;
                    currentDrawnLink = lnk;
                }
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void DrawLinkWindow(int windowID)
        {
                DrawPathDetails();
        }


        private void DrawPathDetails()
        {
            GUILayout.Label("Attenuation Path Details");
            GUILayout.Label("Connectivity: " + currentDrawnLink.source.SourceID + " to " + currentDrawnLink.sink.SinkID);
            GUILayout.Label("Final Intensity: " + currentDrawnLink.fluxEndScale.ToString());

            GUILayout.BeginVertical();
            int n = 1;
            foreach (AttenuationZone z in currentDrawnLink.Path)
            {
                GUILayout.Label(n.ToString() + ". " + z.ToString(),entryStyle);
                n++;
            }
            GUILayout.EndVertical();
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
