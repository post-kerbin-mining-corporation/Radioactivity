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
      float yPortion = areas[2] / (size.z * size.x);
      float zPortion = areas[1] / (size.y * size.x);
      float xzPortion = (Math.Min(xPortion, zPortion) + 2f * (xPortion * zPortion)) * (1f / 3f);
        return 1f;
      //return cube * xzPortion * yPortion;
    }
    public static float GetDensity(Part p)
    {
        return p.mass / GetDisplacement(p);
    }

    // Loads the settings
    public static void LoadSettings()
    {
      ConfigNode[] nodes = GameDatabase.Instance.GetConfigNodes("RADIOACTIVITY_SETTINGS");
      foreach (var node in nodes)
      {

      }

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
