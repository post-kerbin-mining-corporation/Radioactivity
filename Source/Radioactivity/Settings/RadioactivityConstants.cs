using System;
using UnityEngine;

namespace Radioactivity
{
    /// <summary>
    /// Constants are items that are controlled by a config file, but not modified by a player
    /// at runtime
    /// </summary>
    public static class RadioactivityConstants
    {
        // Simulator constants
        // ===================
        // Point simulation

        // Maximum distance to raycast for point radiation
        public static float raycastDistance = 2000f;
        // Minimum flux level to propagate along attenuation paths
        public static float fluxCutoff = 0f;
        // Starting default flux when propagating along paths
        public static float defaultRaycastFluxStart = 1.0f;
        // Distance at which a source's emission is considered measured
        public static float defaultSourceFluxDistance = 0.25f;
        // How far the geometry between two radiation link endpoints can change before we need to recalculate it
        public static float maximumPositionDelta = 0.5f;
        // How much the mass through a raypath can change before we need to recalculate it
        public static float maximumMassDelta = 0.05f;

        // DEFAULT SETTINGS
        // Default value for the linear attenuation coefficient for parts, in cm2/g
        public static float defaultPartAttenuationCoefficient = 1.5f;
        // Default density for a part
        public static float defaultDensity = 1f;


        // Default dose in Sv/s from galactic cosmic rays
        public static double cosmicRadiationFlux = 0.00005073566;

        // OVERLAY SETTINGS
        public static int overlayRayLayer = 0;
        public static float overlayRayWidthMult = 0.005f;
        public static float overlayRayWidthMin = 0.02f;
        public static float overlayRayWidthMax = 0.5f;
        public static string overlayRayMaterial = "GUI/Text Shader";
        public static Gradient overlayRayGradient;


        // TRACKING SETTINGS
        public static string pluginConfigNodeName = "RadioactivityKerbalTracking";
        public static string kerbalConfigNodeName = "RadioactivityKerbal";
        public static string pluginPreferencesName = "RadioactivityPreferences";

        // KERBAL EFFECTS
        // Threshold before inducing radiation sickness (Sv)
        public static float kerbalSicknessThreshold = 1f;
        // Threshold before inducing death (Sv)
        public static float kerbalDeathThreshold = 10f;
        // Threshold for kerbal radiation healing
        public static double kerbalHealThreshold = 0.000000;
        // Rate at which radiation exposure "heals" when on a mission (Sv/s). Default is about 1 Sv/yr
        public static double kerbalHealRate = 0.00001157407407;
        // Rate at which radiation exposure "heals" at the KSC (Sv/s). Default is about 10 Sv/yr
        public static double kerbalHealRateKSC = 0.0001157407407;


        // DEBUG SETTINGS
        // If on, generates UI debug messages
        public static bool debugUI = true;
        // If on, debug overlays
        public static bool debugOverlay = true;
        // If on, generates debug messages for Source and Sink objects
        public static bool debugSourceSinks = true;
        // If on, debug modules added by Radioactivity
        public static bool debugModules = true;
        // If on, generates network creation/destruction/modification messages
        public static bool debugNetwork = true;
        // If on, generates debug messages when building raycast paths
        public static bool debugRaycasting = true;
        // If on, generates debug messages when building raycast paths
        public static bool debugKerbalDatabase = true;
        // If on, generates debug messages when building raycast paths
        public static bool debugKerbalEvents = true;

        public static void Load()
        {
            ConfigNode settingsNode;

            LogUtils.Log("[Constants]: Started loading");
            if (GameDatabase.Instance.ExistsConfigNode("Radioactivity/RADIOACTIVITYCONSTANTS"))
            {
                LogUtils.Log("[Constants]: Located constants file");
                settingsNode = GameDatabase.Instance.GetConfigNode("Radioactivity/RADIOACTIVITYCONSTANTS");

                raycastDistance = ConfigNodeUtils.GetValue(settingsNode, "RaycastDistance", 2000f);
                fluxCutoff = ConfigNodeUtils.GetValue(settingsNode, "FluxCutoff", 0f);
                defaultRaycastFluxStart = ConfigNodeUtils.GetValue(settingsNode, "RaycastFluxStart", 1.0f);
                maximumPositionDelta = ConfigNodeUtils.GetValue(settingsNode, "RaycastPositionDelta", 0.5f);
                maximumMassDelta = ConfigNodeUtils.GetValue(settingsNode, "RaycastMassDelta", 0.05f);
                defaultPartAttenuationCoefficient = ConfigNodeUtils.GetValue(settingsNode, "DefaultMassAttenuationCoefficient", 6f);
                defaultDensity = ConfigNodeUtils.GetValue(settingsNode, "DefaultDensity", 0.5f);
                cosmicRadiationFlux = ConfigNodeUtils.GetValue(settingsNode, "CosmicRadiationFlux", 0.00005073566);

                overlayRayWidthMult = ConfigNodeUtils.GetValue(settingsNode, "OverlayRayWidthMultiplier", 0.005f);
                overlayRayWidthMin = ConfigNodeUtils.GetValue(settingsNode, "OverlayRayMinimumWidth", 0.05f);
                overlayRayWidthMax = ConfigNodeUtils.GetValue(settingsNode, "OverlayRayMaximumWidth", 0.5f);
                overlayRayLayer = ConfigNodeUtils.GetValue(settingsNode, "OverlayRayLayer", 0);
                overlayRayMaterial = ConfigNodeUtils.GetValue(settingsNode, "OverlayRayMaterial", "GUI/Text Shader");


                kerbalSicknessThreshold = ConfigNodeUtils.GetValue(settingsNode, "RadiationSicknessThreshold", 1f);
                kerbalHealThreshold = ConfigNodeUtils.GetValue(settingsNode, "RadiationHealThreshold", 0.000000);
                kerbalDeathThreshold = ConfigNodeUtils.GetValue(settingsNode, "RadiationDeathThreshold", 10f);
                kerbalHealRate = ConfigNodeUtils.GetValue(settingsNode, "RadiationHealRate", 0.00001157407407);
                kerbalHealRateKSC = ConfigNodeUtils.GetValue(settingsNode, "RadiationHealRateKSC", 0.0001157407407);

                debugUI = ConfigNodeUtils.GetValue(settingsNode, "DebugUI", true);
                debugOverlay = ConfigNodeUtils.GetValue(settingsNode, "DebugOverlay", true);
                debugNetwork = ConfigNodeUtils.GetValue(settingsNode, "DebugNetwork", true);
                debugRaycasting = ConfigNodeUtils.GetValue(settingsNode, "DebugRaycasting", true);
                debugSourceSinks = ConfigNodeUtils.GetValue(settingsNode, "DebugSourcesAndSinks", true);
                debugModules = ConfigNodeUtils.GetValue(settingsNode, "DebugModules", true);

            }
            else
            {
                LogUtils.Log("[Constants]: Couldn't find constants file, using defaults");
            }

            GenerateGradients();
            LogUtils.Log("[Constants]: Finished loading");
        }

        public static void GenerateGradients()
        {
            overlayRayGradient = new Gradient();
            GradientColorKey[] gkColor = new GradientColorKey[7];
            GradientAlphaKey[] gkAlpha = new GradientAlphaKey[7];

            gkColor[0].color = new Color(0f, 0f, 0.5f);
            gkColor[1].color = new Color(0f, 0f, 1f);
            gkColor[2].color = new Color(0f, 1f, 1f);
            gkColor[3].color = new Color(0f, 1f, 0f);
            gkColor[4].color = new Color(1f, 1f, 0f);
            gkColor[5].color = new Color(1f, 0f, 0f);
            gkColor[6].color = new Color(0.5f, 0f, 0f);

            gkColor[0].time = 0f;
            gkColor[1].time = 0.00001f;
            gkColor[2].time = 0.0001f;
            gkColor[3].time = 0.001f;
            gkColor[4].time = 0.01f;
            gkColor[5].time = 0.1f;
            gkColor[6].time = 1f;

            gkAlpha[0].alpha = 0.75f;
            gkAlpha[1].alpha = 0.75f;
            gkAlpha[2].alpha = 0.75f;
            gkAlpha[3].alpha = 0.75f;
            gkAlpha[4].alpha = 0.75f;
            gkAlpha[5].alpha = 0.75f;
            gkAlpha[6].alpha = 0.75f;

            gkAlpha[0].time = 0f;
            gkAlpha[1].time = 0.00001f;
            gkAlpha[2].time = 0.0001f;
            gkAlpha[3].time = 0.001f;
            gkAlpha[4].time = 0.01f;
            gkAlpha[5].time = 0.1f;
            gkAlpha[6].time = 1f;

            overlayRayGradient.SetKeys(gkColor, gkAlpha);
        }
    }
}
