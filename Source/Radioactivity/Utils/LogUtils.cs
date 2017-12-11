// Utils
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Radioactivity
{
   
    public static class LogUtils
    {

        public const string PLUGIN_NAME = "Radioactivity";

        public static void Log(string str)
        {
            Debug.Log(String.Format("[{0}]: {1}", PLUGIN_NAME, str));
        }
        public static void LogError(string str)
        {
            Debug.LogError(String.Format("[{0}]: {1}", PLUGIN_NAME, str));
        }
        public static void LogWarning(string str)
        {
            Debug.LogWarning(String.Format("[{0}]: {1}", PLUGIN_NAME, str));
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
