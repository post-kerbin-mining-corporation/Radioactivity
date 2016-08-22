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
      Debug.Log("Displacement calculation: size of " + p.partName + ": " + size.ToString());
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

        int degree = (int)Math.Floor(Math.Log10(Math.Abs(d)) / 3);
        double scaled = d * Math.Pow(1000, -degree);

        char? prefix = null;
        switch (Math.Sign(degree))
        {
            case 1:  prefix = incPrefixes[degree - 1]; break;
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
            if ( bool.TryParse(node.GetValue(nodeID), out val))
                return val;
        }
        return defaultValue;
    }

    public static void Log(string str)
    {
        Debug.Log("Radioactivity > " + str);
    }
    public static void LogError(string str)
    {
        Debug.LogError("Radioactivity > " + str);
    }
    public static void LogWarning(string str)
    {
        Debug.LogWarning("Radioactivity > " + str);
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
