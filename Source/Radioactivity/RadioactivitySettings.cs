using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Radioactivity
{

  public static class RadioactivitySettings
  {
    // Raycasting
    // Maximum distance to raycast for point radiation
    public static float raycastDistance = 2000f;
    // Minimum flux level to propagate along attenuation paths
    public static float fluxCutoff = 0.001f;
    // Starting default flux when propagating along paths
    public static float defaultRaycastFluxStart = 1.0f;
    // How far the geometry between two radiation link endpoints can change before we need to recalculate it
    public static float maximumPositionDelta = 1.0f;

    // Part properties
    // Default value for the linear attenuation coefficient for parts
    public static float defaultPartAttenuationCoefficient = 16f;
    // Default density for a part
    public static float defaultDensity = 1f;

    public static int overlayRayLayer = 0;
    public static float overlayRayWidthScalar = 10f;
    public static string overlayRayMaterial = "Particles/Additive";

    // do we simulate these?
    public static bool simulatePointRadiation = true;
    public static bool simulateSolarRadiation = false;
    public static bool simulateCosmicRadiation = false;

    public static bool debugNetwork = true;

  }

  // Types of zone for attenuation
  public enum AttenuationType {
    Part, ParameterizedPart, Empty
  }
}
