using System;
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
        public static float overlayRayWidthMin = 0.05f;
        public static float overlayRayWidthMax = 0.5f;
        public static string overlayRayMaterial = "GUI/Text Shader";

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

            Utils.Log("[Constants]: Started loading");
            if (GameDatabase.Instance.ExistsConfigNode("Radioactivity/RADIOACTIVITYCONSTANTS"))
            {
                Utils.Log("[Constants]: Located constants file");
                settingsNode = GameDatabase.Instance.GetConfigNode("Radioactivity/RADIOACTIVITYCONSTANTS");

                raycastDistance = Utils.GetValue(settingsNode, "RaycastDistance", 2000f);
                fluxCutoff = Utils.GetValue(settingsNode, "FluxCutoff", 0f);
                defaultRaycastFluxStart = Utils.GetValue(settingsNode, "RaycastFluxStart", 1.0f);
                maximumPositionDelta = Utils.GetValue(settingsNode, "RaycastPositionDelta", 0.5f);
                maximumMassDelta = Utils.GetValue(settingsNode, "RaycastMassDelta", 0.05f);
                defaultPartAttenuationCoefficient = Utils.GetValue(settingsNode, "DefaultMassAttenuationCoefficient", 6f);
                defaultDensity = Utils.GetValue(settingsNode, "DefaultDensity", 0.5f);
                cosmicRadiationFlux = Utils.GetValue(settingsNode, "CosmicRadiationFlux", 0.00005073566);

                overlayRayWidthMult = Utils.GetValue(settingsNode, "OverlayRayWidthMultiplier", 0.005f);
                overlayRayWidthMin = Utils.GetValue(settingsNode, "OverlayRayMinimumWidth", 0.05f);
                overlayRayWidthMax = Utils.GetValue(settingsNode, "OverlayRayMaximumWidth", 0.5f);
                overlayRayLayer = Utils.GetValue(settingsNode, "OverlayRayLayer", 0);
                overlayRayMaterial = Utils.GetValue(settingsNode, "OverlayRayMaterial", "GUI/Text Shader");

             
                kerbalSicknessThreshold = Utils.GetValue(settingsNode, "RadiationSicknessThreshold", 1f);
                kerbalHealThreshold = Utils.GetValue(settingsNode, "RadiationHealThreshold", 0.000000);
                kerbalDeathThreshold = Utils.GetValue(settingsNode, "RadiationDeathThreshold", 10f);
                kerbalHealRate = Utils.GetValue(settingsNode, "RadiationHealRate", 0.00001157407407);
                kerbalHealRateKSC = Utils.GetValue(settingsNode, "RadiationHealRateKSC", 0.0001157407407);

                debugUI = Utils.GetValue(settingsNode, "DebugUI", true);
                debugOverlay = Utils.GetValue(settingsNode, "DebugOverlay", true);
                debugNetwork = Utils.GetValue(settingsNode, "DebugNetwork", true);
                debugRaycasting = Utils.GetValue(settingsNode, "DebugRaycasting", true);
                debugSourceSinks = Utils.GetValue(settingsNode, "DebugSourcesAndSinks", true);
                debugModules = Utils.GetValue(settingsNode, "DebugModules", true);

            }
            else
            {
                Utils.Log("[Constants]: Couldn't find constants file, using defaults");
            }
            Utils.Log("[Constants]: Finished loading");
        }
    }
}
