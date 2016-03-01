using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace Radioactivity
{

  public static class RadioactivitySettings
  {
    // RAYCAST SETTINGS
    // Maximum distance to raycast for point radiation
    public static float raycastDistance = 2000f;
    // Minimum flux level to propagate along attenuation paths
    public static float fluxCutoff = 0f;
    // Starting default flux when propagating along paths
    public static float defaultRaycastFluxStart = 1.0f;
    // How far the geometry between two radiation link endpoints can change before we need to recalculate it
    public static float maximumPositionDelta = 0.5f;
      // How much the mass through a raypath can change before we need to recalculate it
    public static float maximumMassDelta = 0.05f;

    // DEFAULT PART SETTINGS
    // Default value for the linear attenuation coefficient for parts, in cm2/g
    public static float defaultPartAttenuationCoefficient = 1.5f;
    // Default density for a part
    public static float defaultDensity = 1f;

      // OVERLAY SETTINGS
    public static int overlayRayLayer = 0;
    public static float overlayRayWidthMult = 0.005f;
    public static float overlayRayWidthMin = 0.05f;
    public static float overlayRayWidthMax = 0.5f;
    public static string overlayRayMaterial = "GUI/Text Shader";

      // SIMULATOR SETTINGS
    // do we simulate these?
    public static bool simulatePointRadiation = true;
    public static bool simulateSolarRadiation = false;
    public static bool simulateCosmicRadiation = false;

      // TRACKING SETTINGS
    public static string pluginConfigNodeName = "RadioactivityKerbalTracking";
    public static string kerbalConfigNodeName = "RadioactivityKerbal";

    // KERBAL EFFECTS
    // Enable effects on kerbals
    public static bool enableKerbalEffects = true;
    // Enable death of kerbals instead of MIA-ness
    public static bool enableKerbalDeath = false;
    // Threshold before inducing radiation sickness (Sv)
    public static float kerbalSicknessThreshold = 1f;
    // Threshold before inducing death (Sv)
    public static float kerbalDeathThreshold = 10f;
    // Rate at which radiation exposure "heals" when on a mission (Sv/s). Default is about 1 Sv/yr
    public static double kerbalHealRate = 0.00001157407407;
    // Rate at which radiation exposure "heals" at the KSC (Sv/s). Default is about 10 Sv/yr
    public static double kerbalHealRateKSC = 0.0001157407407;

    // NON-KERBAL EFFECTS
    // Enable degredation of science returns
    public static bool enableScienceEffects = true;
    // Enable degredation of probe cores under long radiation exposure
    public static bool enableProbeEffects = true;

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


    public static void Load()
    {
        ConfigNode settingsNode;

       Utils.Log("Settings: Started loading");
       if (GameDatabase.Instance.ExistsConfigNode("Radioactivity/RADIOACTIVITYSETTINGS"))
       {
           Utils.Log("Settings: Located settings file");
           settingsNode = GameDatabase.Instance.GetConfigNode("Radioactivity/RADIOACTIVITYSETTINGS");

           raycastDistance = Utils.GetValue(settingsNode, "RaycastDistance", 2000f);
           fluxCutoff = Utils.GetValue(settingsNode, "FluxCutoff", 0f);
           defaultRaycastFluxStart = Utils.GetValue(settingsNode, "RaycastFluxStart", 1.0f);
           maximumPositionDelta = Utils.GetValue(settingsNode, "RaycastPositionDelta", 0.5f);
           maximumMassDelta = Utils.GetValue(settingsNode, "RaycastMassDelta", 0.05f);
           defaultPartAttenuationCoefficient = Utils.GetValue(settingsNode, "DefaultMassAttenuationCoefficient", 1.5f);
           defaultDensity = Utils.GetValue(settingsNode, "DefaultDensity", 0.5f);

           overlayRayWidthMult = Utils.GetValue(settingsNode, "OverlayRayWidthMultiplier", 0.005f);
           overlayRayWidthMin = Utils.GetValue(settingsNode, "OverlayRayMinimumWidth", 0.05f);
           overlayRayWidthMax = Utils.GetValue(settingsNode, "OverlayRayMaximumWidth", 0.5f);
           overlayRayLayer = Utils.GetValue(settingsNode, "OverlayRayLayer", 0);
           overlayRayMaterial = Utils.GetValue(settingsNode, "OverlayRayMaterial", "GUI/Text Shader");

           simulatePointRadiation = Utils.GetValue(settingsNode, "EnablePointRadiation", true);
           simulateCosmicRadiation = Utils.GetValue(settingsNode, "EnableCosmicRadiation", false);
           simulateSolarRadiation = Utils.GetValue(settingsNode, "EnableSolarRadiation", false);

           enableKerbalEffects = Utils.GetValue(settingsNode, "EnableKerbalEffects", true);
           enableScienceEffects = Utils.GetValue(settingsNode, "EnableScienceEffects", true);
           enableProbeEffects = Utils.GetValue(settingsNode, "EnableProbeEffects", true);

           enableKerbalDeath = Utils.GetValue(settingsNode, "EnableKerbalDeath", false);
           kerbalSicknessThreshold = Utils.GetValue(settingsNode, "RadiationSicknessThreshold", 1f);
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
           Utils.Log("Settings: Couldn't find settings file, using defaults");
       }
        Utils.Log("Settings: Finished loading");
    }

  }

  // Types of zone for attenuation
  public enum AttenuationType {
    Part, ParameterizedPart, Terrain, Empty
  }
}
