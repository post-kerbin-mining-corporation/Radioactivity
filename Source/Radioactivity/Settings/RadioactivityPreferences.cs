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
        public static UnitDisplayMode unitMode = UnitDisplayMode.SI;

        // Editor Simulation settings

        // These settings allow the user to simulate approximately the effects of atmosphere 
        // on ambient radiation sources
        // Settings for what "planet" we are on
        public static double editorAtmosphereDensity = 0.0d;
        public static double editorAtmosphereHeight = 0.0d;
        /// How far to the sun
        public static double editorSunDistance = 0.0d;
        // How high in the atmo are we
        public static double editorFlightHeight = 0.0d;


        public static void Load(ConfigNode node)
        {
            ConfigNode mNode = node.GetNode(RadioactivityConstants.pluginPreferencesName);

            Utils.Log("[Preferences]: Started Loading");
            if (mNode != null)
            {
                rosterShown = Utils.GetValue(mNode, "RosterShown", false);
                overlayShown = Utils.GetValue(mNode, "OverlayShown", false);
                editorShown = Utils.GetValue(mNode, "EditorShown", false);

                editorAtmosphereDensity = Utils.GetValue(mNode, "EditorAtmosphereDensity", 0.0d);
                editorAtmosphereHeight = Utils.GetValue(mNode, "EditorAtmosphereHeight", 0.0d);
                editorSunDistance = Utils.GetValue(mNode, "EditorSunDistance", 0.0d);
                editorFlightHeight = Utils.GetValue(mNode, "EditorFlightHeight", 0.0d);
            }

            Utils.Log("[Preferences]: Done Loading");

        }

        public static void Save(ConfigNode node)
        {
            Utils.Log("[Preferences]: Started Saving");
            ConfigNode prefsNode;
            if (node.HasNode(RadioactivityConstants.pluginPreferencesName))
                prefsNode = node.GetNode(RadioactivityConstants.pluginPreferencesName);
            else
                prefsNode = node.AddNode(RadioactivityConstants.pluginPreferencesName);

            prefsNode.AddValue("RosterShown", RadioactivityUI.Instance.RosterWindow.Drawn);
            prefsNode.AddValue("OverlayShown", RadioactivityUI.Instance.OverlayWindow.Drawn);
            prefsNode.AddValue("EditorShown", RadioactivityUI.Instance.EditorWindow.Drawn);

            prefsNode.AddValue("EditorAtmosphereDensity", editorAtmosphereDensity);
            prefsNode.AddValue("EditorAtmosphereHeight", editorAtmosphereHeight);
            prefsNode.AddValue("EditorSunDistance", editorSunDistance);
            prefsNode.AddValue("EditorFlightHeight", editorFlightHeight);

            Utils.Log("[Preferences]: Finished Saving");
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
