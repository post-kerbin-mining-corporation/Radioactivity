// Utils
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Radioactivity
{
    public static class ConfigNodeUtils
    {
        // Node loading
        // several variants for data types
        public static string GetValue(ConfigNode node, string nodeID, string defaultValue)
        {
            if (node.HasValue(nodeID))
            {
                return node.GetValue(nodeID);
            }
            return defaultValue;
        }
        public static int GetValue(ConfigNode node, string nodeID, int defaultValue)
        {
            if (node.HasValue(nodeID))
            {
                int val;
                if (int.TryParse(node.GetValue(nodeID), out val))
                    return val;
            }
            return defaultValue;
        }
        public static Guid GetValue(ConfigNode node, string nodeID, Guid defaultValue)
        {
            if (node.HasValue(nodeID))
            {
                return new Guid(node.GetValue(nodeID));
            }
            return defaultValue;
        }
        public static float GetValue(ConfigNode node, string nodeID, float defaultValue)
        {
            if (node.HasValue(nodeID))
            {
                float val;
                if (float.TryParse(node.GetValue(nodeID), out val))
                    return val;
            }
            return defaultValue;
        }
        public static double GetValue(ConfigNode node, string nodeID, double defaultValue)
        {
            if (node.HasValue(nodeID))
            {
                double val;
                if (double.TryParse(node.GetValue(nodeID), out val))
                    return val;
            }
            return defaultValue;
        }
        public static bool GetValue(ConfigNode node, string nodeID, bool defaultValue)
        {
            if (node.HasValue(nodeID))
            {
                bool val;
                if (bool.TryParse(node.GetValue(nodeID), out val))
                    return val;
            }
            return defaultValue;
        }
        public static Vector3 Vector3FromString(string str)
        {
            Vector3 outVector3;
            string[] splitString = str.Split(',');
            outVector3.x = float.Parse(splitString[0]);
            outVector3.y = float.Parse(splitString[1]);
            outVector3.z = float.Parse(splitString[2]);
            return outVector3;
        }
    }
}
