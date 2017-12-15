using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Radioactivity.UI;

namespace Radioactivity
{
    /// <summary>
    /// Settings are things that are changed by the player and must be tracked
    /// </summary>
    public static class RadioactivityPreferences
    {
        // Windows on/off
        public static bool rosterShown = false;
        public static bool overlayShown = false;
        public static bool editorShown = false;

        // Unit mode setting
        public static int unitMode = 0;

        // Editor Simulation settings

        // These settings allow the user to simulate approximately the effects of atmosphere 
        // on ambient radiation sources
        // Settings for what "planet" we are on
        public static double editorMagneticFieldStrength = 0.0d;
        public static double editorRadiationBeltStrength = 0.0d;
        public static double editorAtmosphereDensity = 0.0d;
        public static double editorAtmosphereHeight = 0.0d;
        public static double editorPlanetRadius = 0.0d;
        /// How far to the sun
        public static double editorSunDistance = 0.0d;
        // How high in the atmo are we
        public static double editorFlightHeight = 0.0d;


        public static void Load(ConfigNode node)
        {
            ConfigNode mNode = node.GetNode(RadioactivityConstants.pluginPreferencesName);

            LogUtils.Log("[Preferences]: Started Loading");
            if (mNode != null)
            {
                rosterShown = ConfigNodeUtils.GetValue(mNode, "RosterShown", false);
                overlayShown = ConfigNodeUtils.GetValue(mNode, "OverlayShown", false);
                editorShown = ConfigNodeUtils.GetValue(mNode, "EditorShown", false);
                unitMode = ConfigNodeUtils.GetValue(mNode, "UnitMode", 0);
                editorMagneticFieldStrength = ConfigNodeUtils.GetValue(mNode, "EditorMagneticFieldStrength", 0.0d);
                editorRadiationBeltStrength = ConfigNodeUtils.GetValue(mNode, "EditorRadiationBeltStrength", 0.0d);
                editorAtmosphereDensity = ConfigNodeUtils.GetValue(mNode, "EditorAtmosphereDensity", 0.0d);
                editorAtmosphereHeight = ConfigNodeUtils.GetValue(mNode, "EditorAtmosphereHeight", 0.0d);
                editorPlanetRadius = ConfigNodeUtils.GetValue(mNode, "EditorPlanetRadius", 0.0d);
                editorSunDistance = ConfigNodeUtils.GetValue(mNode, "EditorSunDistance", 0.0d);
                editorFlightHeight = ConfigNodeUtils.GetValue(mNode, "EditorFlightHeight", 0.0d);
            }

            LogUtils.Log("[Preferences]: Done Loading");

        }

        public static void Save(ConfigNode node)
        {
            LogUtils.Log("[Preferences]: Started Saving");
            ConfigNode prefsNode;
            if (node.HasNode(RadioactivityConstants.pluginPreferencesName))
                prefsNode = node.GetNode(RadioactivityConstants.pluginPreferencesName);
            else
                prefsNode = node.AddNode(RadioactivityConstants.pluginPreferencesName);

            prefsNode.AddValue("RosterShown", RadioactivityUI.Instance.RosterWindow.Drawn);
            prefsNode.AddValue("OverlayShown", RadioactivityUI.Instance.OverlayWindow.Drawn);
            prefsNode.AddValue("EditorShown", RadioactivityUI.Instance.EditorWindow.Drawn);
            prefsNode.AddValue("UnitMode", RadioactivityUI.Instance.UnitMode);

            prefsNode.AddValue("EditorMagneticFieldStrength", editorMagneticFieldStrength);
            prefsNode.AddValue("EditorRadiationBeltStrength", editorRadiationBeltStrength);

            prefsNode.AddValue("EditorAtmosphereDensity", editorAtmosphereDensity);
            prefsNode.AddValue("EditorAtmosphereHeight", editorAtmosphereHeight);
            prefsNode.AddValue("EditorPlanetRadius", editorPlanetRadius);
            prefsNode.AddValue("EditorSunDistance", editorSunDistance);
            prefsNode.AddValue("EditorFlightHeight", editorFlightHeight);

            LogUtils.Log("[Preferences]: Finished Saving");
        }

    }

    // Types of zone for attenuation
    public enum UnitDisplayMode
    {
        SI, TimeToIllness, TimeToDeath
    }
    // Types of zone for attenuation
    public enum AttenuationType
    {
        Part, ParameterizedPart, Terrain, Empty
    }

    public enum RadiationType
    {
        Point, Cosmic, Solar, Belt, Local
    }
}
