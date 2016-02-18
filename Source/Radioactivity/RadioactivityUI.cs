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
        private GUIStyle uiStyle;
        private Rect windowPos = new Rect(0, 0, 480, 480);
        private RadiationLink currentDrawnLink = null;
        // Stock toolbar button
        private static ApplicationLauncherButton stockToolbarButton = null;

        
        public void Awake()
        {
            Utils.Log("UI: Awake");
            GameEvents.onGUIApplicationLauncherReady.Add(OnGUIAppLauncherReady);
            GameEvents.onGUIApplicationLauncherDestroyed.Add(OnGUIAppLauncherDestroyed);
        }

        public void Start()
        {
            Utils.Log("UI: Start");
            uiStyle = new GUIStyle(HighLogic.Skin.window);
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
            if (uiShown)
            {
                windowPos= GUILayout.Window(947695, windowPos, DrawWindow, "Radioactivity", uiStyle, GUILayout.MinHeight(20), GUILayout.ExpandHeight(true));
                

            }
        }

        private void DrawWindow(int windowID)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Hide all raypaths"))
                Radioactivity.Instance.HideAllOverlays();
            if (GUILayout.Button("Show all raypaths"))
                Radioactivity.Instance.ShowAllOverlays();
            GUILayout.EndHorizontal();

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
            if (currentDrawnLink != null)
                DrawPathDetails();
            
        }

        private void DrawSourceInfo(RadioactiveSource src)
        {
            GUILayout.BeginVertical();
            GUILayout.Label("Source: " + src.SourceID);
            GUILayout.Label("On: " + src.part.name);
            GUILayout.Label("Emitting at: " + src.CurrentEmission.ToString());
            GUILayout.EndVertical();
        }
        private void DrawSinkInfo(RadioactiveSink snk)
        {
            GUILayout.BeginVertical();
            GUILayout.Label("Sink: " + snk.SinkID);
            GUILayout.Label("On: " + snk.part.name);
            GUILayout.EndVertical();
        }
        private void DrawLinkInfo(RadiationLink lnk)
        {
            GUILayout.BeginVertical();
            GUILayout.Label("Connectivity: " + lnk.source.SourceID  + " to " + lnk.sink.SinkID);
            GUILayout.Label("Final Intensity: " + lnk.fluxEndScale.ToString());
            GUILayout.Label("Zone Count: " + lnk.ZoneCount.ToString());
            GUILayout.Label("Occluder Count: " + lnk.OccluderCount.ToString());
            GUILayout.Label("Rendered: " + lnk.overlayShown.ToString());
            if (GUILayout.Button("Path Details"))
                currentDrawnLink = lnk;
            GUILayout.EndVertical();
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
                GUILayout.Label(n.ToString() + ". " + z.ToString());
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
