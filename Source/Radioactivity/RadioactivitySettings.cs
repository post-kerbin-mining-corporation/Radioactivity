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


    public static void Load(ConfigNode node)
    {
       Utils.Log("Settings: Started loading");

       raycastDistance = Utils.GetValue(node, "RaycastDistance", 2000f);
       fluxCutoff = Utils.GetValue(node, "FluxCutoff",0f);
       defaultRaycastFluxStart = Utils.GetValue(node, "RaycastFluxStart",1.0f);
       maximumPositionDelta = Utils.GetValue(node, "RaycastPositionDelta", 0.5f);
       maximumMassDelta = Utils.GetValue(node, "RaycastMassDelta", 0.05f);
       defaultPartAttenuationCoefficient = Utils.GetValue(node, "DefaultMassAttenuationCoefficient", 1.5f);
       defaultDensity = Utils.GetValue(node, "DefaultDensity", 0.5f);

       overlayRayWidthMult = Utils.GetValue(node, "OverlayRayWidthMultiplier", 0.005f);
       overlayRayWidthMin = Utils.GetValue(node, "OverlayRayMinimumWidth", 0.05f);
       overlayRayWidthMax = Utils.GetValue(node, "OverlayRayMaximumWidth", 0.5f);
       overlayRayLayer = Utils.GetValue(node, "OverlayRayLayer", 0);
       overlayRayMaterial = Utils.GetValue(node, "OverlayRayMaterial", "GUI/Text Shader");

       simulatePointRadiation = Utils.GetValue(node, "EnablePointRadiation", true);
       simulateCosmicRadiation = Utils.GetValue(node, "EnableCosmicRadiation", false);
       simulateSolarRadiation = Utils.GetValue(node, "EnableSolarRadiation", false);

       debugUI = Utils.GetValue(node, "DebugUI", true);
       debugOverlay = Utils.GetValue(node, "DebugOverlay", true);
       debugNetwork = Utils.GetValue(node, "DebugNetwork", true);
       debugRaycasting = Utils.GetValue(node, "DebugRaycasting", true);
       debugSourceSinks = Utils.GetValue(node, "DebugSourcesAndSinks", true);
       debugModules = Utils.GetValue(node, "DebugModules", true);

        Utils.Log("Settings: Finished loading");
    }

  }

  // Types of zone for attenuation
  public enum AttenuationType {
    Part, ParameterizedPart, Terrain, Empty
  }
}
