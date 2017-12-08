// Utils
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Radioactivity
{

    public static class Utils
    {

        public static float GetDisplacement(Part p)
        {
            Vector3 size = p.DragCubes.WeightedSize;

            float[] areas = new float[6];
            areas = p.DragCubes.WeightedArea;

            float xPortion = areas[0] / (size.y * size.z);
            float yPortion = areas[1] / (size.z * size.x);
            float zPortion = areas[2] / (size.y * size.x);
            float xzPortion = (Math.Min(xPortion, zPortion) + 2f * (xPortion * zPortion)) * (1f / 3f);
            float cube = size.x * size.y * size.z;
            Debug.Log("[Utils]: Displacement calculation: size of " + p.partName + ": " + size.ToString());
            return cube;// *xzPortion * yPortion;
        }

        public static float GetDensity(Part p)
        {
            return p.mass / GetDisplacement(p);
        }

        public static string ToSI(double d, string format = null)
        {
            if (d == 0.0)
                return d.ToString(format);

            char[] incPrefixes = new[] { 'k', 'M', 'G', 'T', 'P', 'E', 'Z', 'Y' };
            char[] decPrefixes = new[] { 'm', '\u03bc', 'n', 'p', 'f', 'a', 'z', 'y' };

            int degree = Mathf.Clamp((int)Math.Floor(Math.Log10(Math.Abs(d)) / 3), -8, 8);
            if (degree == 0)
                return d.ToString(format) + " ";

            double scaled = d * Math.Pow(1000, -degree);

            char? prefix = null;

            switch (Math.Sign(degree))
            {
                case 1: prefix = incPrefixes[degree - 1]; break;
                case -1: prefix = decPrefixes[-degree - 1]; break;
            }

            return scaled.ToString(format) + " " + prefix;
        }
        public static Vector3 getRelativePosition(Transform origin, Vector3 position)
        {
            Vector3 distance = position - origin.position;
            Vector3 relativePosition = Vector3.zero;
            relativePosition.x = Vector3.Dot(distance, origin.right.normalized);
            relativePosition.y = Vector3.Dot(distance, origin.up.normalized);
            relativePosition.z = Vector3.Dot(distance, origin.forward.normalized);

            return relativePosition;
        }
        // Solid angle of a body relative to a vessel
        public static double ComputeBodySolidAngle(Vessel vessel, CelestialBody body)
        {
            double dist = body.GetAltitude(vessel.GetWorldPos3D()) + body.Radius;
            return 2.0 * Math.PI * (1.0 - Math.Cos(dist));
        }
        // Solid angle of the sky, that is, the total area minus all bodies
        public static double ComputeSkySolidAngle(Vessel vessel)
        {
            double totalBodyAngle = 0d;
            foreach (CelestialBody body in FlightGlobals.fetch.bodies)
            {
                totalBodyAngle += Utils.ComputeBodySolidAngle(vessel, body);
            }
            return 4d * Math.PI - totalBodyAngle;
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
                int val;
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

        public static void Log(string str)
        {
            Debug.Log("[Radioactivity]: " + str);
        }
        public static void LogError(string str)
        {
            Debug.LogError("[Radioactivity]:  " + str);
        }
        public static void LogWarning(string str)
        {
            Debug.LogWarning("[Radioactivity]:  " + str);
        }
        public static string FormatFluxString(int mode, double flux)
        {
            
            if (mode == 0)
            {
                return String.Format("{0}Sv/s", Utils.ToSI(flux, "F2"));
            }
            if (mode == 1)
            {
                if (flux == 0d)
                    return "∞";
                return String.Format("{0}", Utils.FormatTimeString(RadioactivityConstants.kerbalSicknessThreshold / flux));
            }
            if (mode == 2)
            {
                if (flux == 0d)
                    return "∞";
                return String.Format("{0}", Utils.FormatTimeString(RadioactivityConstants.kerbalDeathThreshold / flux));
            }
            return "";
        }

            
        public static string FormatTimeString(double seconds)
        {
            double dayLength;
            double yearLength;
            double rem;
            if (GameSettings.KERBIN_TIME)
            {
                dayLength = 6d;
                yearLength = 426d;
            }
            else
            {
                dayLength = 24d;
                yearLength = 365d;
            }

            int years = (int)(seconds / (3600.0d * dayLength * yearLength));
            rem = seconds % (3600.0d * dayLength * yearLength);
            int days = (int)(rem / (3600.0d * dayLength));
            rem = rem % (3600.0d * dayLength);
            int hours = (int)(rem / (3600.0d));
            rem = rem % (3600.0d);
            int minutes = (int)(rem / (60.0d));
            rem = rem % (60.0d);
            int secs = (int)rem;

            string result = "";

            // draw years + days
            if (years > 0)
            {
                result += years.ToString() + "y ";
                result += days.ToString() + "d ";
                result += hours.ToString() + "h ";
                result += minutes.ToString() + "m";
            }
            else if (days > 0)
            {
                result += days.ToString() + "d ";
                result += hours.ToString() + "h ";
                result += minutes.ToString() + "m ";
                result += secs.ToString() + "s";
            }
            else if (hours > 0)
            {
                result += hours.ToString() + "h ";
                result += minutes.ToString() + "m ";
                result += secs.ToString() + "s";
            }
            else if (minutes > 0)
            {
                result += minutes.ToString() + "m ";
                result += secs.ToString() + "s";
            }
            else if (seconds > 0)
            {
                result += secs.ToString() + "s";
            }
            else
            {
                result = "None";
            }


            return result;
        }
    }

    //<NathanKell> take the bounding box of the dragcube
    //[13:54] <NathanKell> (the product of the final triplet in a DRAG_CUBE entry)
    //[13:55] <NathanKell> Then multiply by the Y area divided by the X*Z dimensions
    //[13:55] <NathanKell> (i.e. how much of the Y-facing area of the rectangle is in fact the part)
    //[13:55] NathanKell> Then take the minimum of (the X and Z portions) , then add 2 * (x portion * zPortion), then divide by 3.
    //13:56] <NathanKell> that is the final multiplier
    //[13:56] <NathanKell> in effect we're figuring out, from projects from three axes, how much of the cube is hollow and how much is filled with th part
    //[13:56] <NathanKell> the8
    // [13:56] <NathanKell> the**
    // size = Nertea: part.DragCubes.WeightedSize
    //displacement = cube * xzPortion * yPortion
    //where yPortion = yPortion = areas[2] / (size.x * size.z); and
    //xPortion = areas[0] / (size.y * size.z)?
    //zPortion = areas[1] / (size.y * size.x)?
    //and xPortion and zPortion are calculated like yPortion
    //[13:59] <NathanKell> (areas is a a float[6] from dragcubes.WeightedArea[] )z)
    // xzPortion = (Math.Min(xPortion, zPortion) + 2d * (xPortion * zPortion)) * (1d / 3d);

}
