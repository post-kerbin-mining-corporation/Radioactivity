using System;
using UnityEngine;
using Radioactivity;
namespace Radioactivity.UI
{
    /// <summary>
    /// The editor window allows the player to vary exposures for ambient radiation bits
    /// in the editor only
    /// </summary>
    public class UIEditorWindow : UIWindow
    {
        Rect windowPosition = new Rect(210, 15, 425, 250);
        float contributionHeaderWidth = 60f;
        float contributionNumberWidth = 50f;

        public UIEditorWindow(System.Random randomizer, RadioactivityUI uiHost) : base(randomizer, uiHost)
        {
            LogUtils.Log("[UIEditorWindow]: Initialized");
        }

        /// <summary>
        /// Does window drawing
        /// </summary>
        public void Draw()
        {
            if (drawn && HighLogic.LoadedSceneIsEditor)
            {

                windowPosition = GUILayout.Window(windowID,
                                                  windowPosition,
                                                  DrawWindow,
                                                  "Editor Radiation Environment",
                                                  host.GUIResources.GetStyle("roster_window"),
                                                  GUILayout.MinHeight(20),
                                                  GUILayout.ExpandHeight(true));
            }
        }

        void DrawWindow(int WindowID)
        {
            //if (RadioactivitySimulationSettings.SimulateCosmicRadiation)
            GUILayout.BeginHorizontal();
            DrawEnvironmentSimulation();
            DrawContributions();
            GUILayout.EndHorizontal();
            GUI.DragWindow();
        }

        void DrawEnvironmentSimulation()
        {
            GUILayout.BeginVertical();
            DrawBodyParameters();
            GUILayout.EndVertical();

        }
        void DrawBodyParameters()
        {
            float optionHeight = 30f;
            float optionWidth = 360f;
            float optionTitleWidth = 130f;
            float optionSliderWidth = 150f;
            float optionLabelWidth = 100f;

            Rect optionsRect = GUILayoutUtility.GetRect(optionWidth, optionHeight);
            Rect sliderRect = new Rect(optionTitleWidth, 0f, optionSliderWidth, optionHeight);
            Rect titleRect = new Rect(0f, 0f, optionTitleWidth, optionHeight);
            Rect labelRect = new Rect(optionTitleWidth + optionSliderWidth, 0f, optionLabelWidth, optionHeight);

            GUI.BeginGroup(optionsRect);

            GUI.Label(titleRect, "<b>Orbital Altitude</b>", host.GUIResources.GetStyle("editor_header"));
            RadioactivityPreferences.editorFlightHeight = GUI.HorizontalSlider(sliderRect,
                                                                               (float)RadioactivityPreferences.editorFlightHeight,
                                                                              0f, 1000000f);
            GUI.Label(labelRect, String.Format("<color=#99ff00>{0:F1} km</color>", RadioactivityPreferences.editorFlightHeight),
                      host.GUIResources.GetStyle("editor_text"));
            GUI.EndGroup();

            optionsRect = GUILayoutUtility.GetRect(optionWidth, optionHeight);
            sliderRect = new Rect(optionTitleWidth, 0f, optionSliderWidth, optionHeight);
            titleRect = new Rect(0f, 0f, optionTitleWidth, optionHeight);
            labelRect = new Rect(optionTitleWidth + optionSliderWidth, 0f, optionLabelWidth, optionHeight);

            GUI.BeginGroup(optionsRect);

            GUI.Label(titleRect, "<b>Solar Altitude</b>", host.GUIResources.GetStyle("editor_header"));
            RadioactivityPreferences.editorSunDistance = GUI.HorizontalSlider(sliderRect,
                                                                              (float)RadioactivityPreferences.editorSunDistance,
                                                                              0f, 1000000f);
            GUI.Label(labelRect, String.Format("<color=#99ff00>{0:F1} km</color>", RadioactivityPreferences.editorSunDistance),
                      host.GUIResources.GetStyle("editor_text"));
            GUI.EndGroup();

            optionsRect = GUILayoutUtility.GetRect(optionWidth, optionHeight);
            sliderRect = new Rect(optionTitleWidth, 0f, optionSliderWidth, optionHeight);
            titleRect = new Rect(0f, 0f, optionTitleWidth, optionHeight);
            labelRect = new Rect(optionTitleWidth + optionSliderWidth, 0f, optionLabelWidth, optionHeight);

            GUI.BeginGroup(optionsRect);

            GUI.Label(titleRect, "<b>Body Radius</b>", host.GUIResources.GetStyle("editor_header"));
            RadioactivityPreferences.editorPlanetRadius = GUI.HorizontalSlider(sliderRect,
                                                                               (float)RadioactivityPreferences.editorPlanetRadius,
                                                                              0f, 2f);
            GUI.Label(labelRect, String.Format("<color=#99ff00>{0:F2}</color>", RadioactivityPreferences.editorPlanetRadius),
                      host.GUIResources.GetStyle("editor_text"));
            GUI.EndGroup();

            optionsRect = GUILayoutUtility.GetRect(optionWidth, optionHeight);
            sliderRect = new Rect(optionTitleWidth, 0f, optionSliderWidth, optionHeight);
            titleRect = new Rect(0f, 0f, optionTitleWidth, optionHeight);
            labelRect = new Rect(optionTitleWidth + optionSliderWidth, 0f, optionLabelWidth, optionHeight);

            GUI.BeginGroup(optionsRect);

            GUI.Label(titleRect, "<b>Atmosphere Density</b>", host.GUIResources.GetStyle("editor_header"));
            RadioactivityPreferences.editorAtmosphereDensity = GUI.HorizontalSlider(sliderRect,
                                                                                    (float)RadioactivityPreferences.editorAtmosphereDensity,
                                                                              0f, 2f);
            GUI.Label(labelRect, String.Format("<color=#99ff00>{0:F2}</color>", RadioactivityPreferences.editorAtmosphereDensity),
                      host.GUIResources.GetStyle("editor_text"));
            GUI.EndGroup();

            optionsRect = GUILayoutUtility.GetRect(optionWidth, optionHeight);
            sliderRect = new Rect(optionTitleWidth, 0f, optionSliderWidth, optionHeight);
            titleRect = new Rect(0f, 0f, optionTitleWidth, optionHeight);
            labelRect = new Rect(optionTitleWidth + optionSliderWidth, 0f, optionLabelWidth, optionHeight);

            GUI.BeginGroup(optionsRect);

            GUI.Label(titleRect, "<b>Atmosphere Height</b>", host.GUIResources.GetStyle("editor_header"));
            RadioactivityPreferences.editorAtmosphereHeight = GUI.HorizontalSlider(sliderRect,
                                                                                   (float)RadioactivityPreferences.editorAtmosphereHeight,
                                                                              0f, 10000f);
            GUI.Label(labelRect, String.Format("<color=#99ff00>{0:F1} km</color>",
                                               RadioactivityPreferences.editorAtmosphereHeight),
 host.GUIResources.GetStyle("editor_text"));
            GUI.EndGroup();

            optionsRect = GUILayoutUtility.GetRect(optionWidth, optionHeight);
            sliderRect = new Rect(optionTitleWidth, 0f, optionSliderWidth, optionHeight);
            titleRect = new Rect(0f, 0f, optionTitleWidth, optionHeight);
            labelRect = new Rect(optionTitleWidth + optionSliderWidth, 0f, optionLabelWidth, optionHeight);

            GUI.BeginGroup(optionsRect);

            GUI.Label(titleRect, "<b>Local Magnetic Attenuation</b>", host.GUIResources.GetStyle("editor_header"));
            RadioactivityPreferences.editorMagneticFieldStrength = GUI.HorizontalSlider(sliderRect,
                                                                                        (float)RadioactivityPreferences.editorMagneticFieldStrength,
                                                                              0f, 1.0f);
            GUI.Label(labelRect, String.Format("<color=#99ff00>{0:F1}% </color>",
                                               (float)RadioactivityPreferences.editorMagneticFieldStrength * 100f),
 host.GUIResources.GetStyle("editor_text"));
            GUI.EndGroup();

            optionsRect = GUILayoutUtility.GetRect(optionWidth, optionHeight);
            sliderRect = new Rect(optionTitleWidth, 0f, optionSliderWidth, optionHeight);
            titleRect = new Rect(0f, 0f, optionTitleWidth, optionHeight);
            labelRect = new Rect(optionTitleWidth + optionSliderWidth, 0f, optionLabelWidth, optionHeight);

            GUI.BeginGroup(optionsRect);

            GUI.Label(titleRect, "<b>Belt Radiation</b>", host.GUIResources.GetStyle("editor_header"));
            RadioactivityPreferences.editorRadiationBeltStrength = GUI.HorizontalSlider(sliderRect,
                                                                                        (float)RadioactivityPreferences.editorRadiationBeltStrength,
                                                                              0f, 1.0f);
            GUI.Label(labelRect, String.Format("<color=#99ff00>{0:F2} Sv</color>",
                                               FormatUtils.ToSI(RadioactivityPreferences.editorRadiationBeltStrength)),
 host.GUIResources.GetStyle("editor_text"));
            GUI.EndGroup();

        }



        void DrawContributions()
        {
            GUILayout.BeginVertical(host.GUIResources.GetStyle("roster_group"));
            DrawCosmicConstribution();
            DrawPlanetaryContribution();
            DrawSolarContribution();
            DrawBeltContribution();
            GUILayout.Space(2f);
            DrawTotalContribution();

            GUILayout.EndVertical();
        }
        void DrawCosmicConstribution()
        {
            if (RadioactivitySimulationSettings.SimulateCosmicRadiation)
            {
                double val = Radioactivity.Instance.RadSim.AmbientSim.EditorVessel.CosmicFlux;

                GUILayout.BeginHorizontal();
                string contribution = String.Format("<color=#99ff00>{0}Sv</color>", FormatUtils.ToSI(val));
                GUILayout.Label("<b>Cosmic</b>", host.GUIResources.GetStyle("editor_header"), GUILayout.MinWidth(contributionHeaderWidth), GUILayout.MaxWidth(contributionHeaderWidth));
                GUILayout.Label(contribution, host.GUIResources.GetStyle("editor_text"), GUILayout.MinWidth(contributionNumberWidth), GUILayout.MaxWidth(contributionNumberWidth));
                GUILayout.EndHorizontal();
            }
        }
        void DrawSolarContribution()
        {
            if (RadioactivitySimulationSettings.SimulateSolarRadiation)
            {
                double val = Radioactivity.Instance.RadSim.AmbientSim.EditorVessel.SolarFlux;
                GUILayout.BeginHorizontal();
                string contribution = String.Format("<color=#99ff00>{0} Sv</color>", FormatUtils.ToSI(val));
                GUILayout.Label("<b>Solar</b>", host.GUIResources.GetStyle("editor_header"), GUILayout.MinWidth(contributionHeaderWidth), GUILayout.MaxWidth(contributionHeaderWidth));
                GUILayout.Label(contribution, host.GUIResources.GetStyle("editor_text"), GUILayout.MinWidth(contributionNumberWidth), GUILayout.MaxWidth(contributionNumberWidth));
                GUILayout.EndHorizontal();
            }
        }
        void DrawBeltContribution()
        {
            if (RadioactivitySimulationSettings.SimulateBeltRadiation)
            {
                double val = Radioactivity.Instance.RadSim.AmbientSim.EditorVessel.BeltFlux;
                GUILayout.BeginHorizontal();
                string contribution = String.Format("<color=#99ff00>{0} Sv</color>", FormatUtils.ToSI(val));
                GUILayout.Label("<b>Belt</b>", host.GUIResources.GetStyle("editor_header"), GUILayout.MinWidth(contributionHeaderWidth), GUILayout.MaxWidth(contributionHeaderWidth));
                GUILayout.Label(contribution, host.GUIResources.GetStyle("editor_text"), GUILayout.MinWidth(contributionNumberWidth), GUILayout.MaxWidth(contributionNumberWidth));
                GUILayout.EndHorizontal();

            }
        }
        void DrawPlanetaryContribution()
        {
            if (RadioactivitySimulationSettings.SimulateLocalRadiation)
            {
                double val = Radioactivity.Instance.RadSim.AmbientSim.EditorVessel.PlanetaryFlux;
                GUILayout.BeginHorizontal();
                string contribution = String.Format("<color=#99ff00>{0} Sv</color>", FormatUtils.ToSI(val));
                GUILayout.Label("<b>Planetary</b>", host.GUIResources.GetStyle("editor_header"), GUILayout.MinWidth(contributionHeaderWidth), GUILayout.MaxWidth(contributionHeaderWidth));
                GUILayout.Label(contribution, host.GUIResources.GetStyle("editor_text"), GUILayout.MinWidth(contributionNumberWidth), GUILayout.MaxWidth(contributionNumberWidth));
                GUILayout.EndHorizontal();
            }
        }
        void DrawTotalContribution()
        {
            GUILayout.BeginHorizontal();
            double val = Radioactivity.Instance.RadSim.AmbientSim.EditorVessel.TotalFlux;
            string contribution = String.Format("<b><color=#99ff00>{0} Sv</color></b>", FormatUtils.ToSI(val));
            GUILayout.Label("<b>Total</b>", host.GUIResources.GetStyle("editor_header"), GUILayout.MinWidth(contributionHeaderWidth), GUILayout.MaxWidth(contributionHeaderWidth));
            GUILayout.Label(contribution, host.GUIResources.GetStyle("editor_text"), GUILayout.MinWidth(contributionNumberWidth), GUILayout.MaxWidth(contributionNumberWidth));
            GUILayout.EndHorizontal();
        }


        /// <summary>
        /// Does any per-frame updates
        /// </summary>
        public void Update()
        {

        }
    }
}
