// Utils
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Radioactivity
{
    public static class PartUtils
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
        public static Vector3 getRelativePosition(Transform origin, Vector3 position)
        {
            Vector3 distance = position - origin.position;
            Vector3 relativePosition = Vector3.zero;
            relativePosition.x = Vector3.Dot(distance, origin.right.normalized);
            relativePosition.y = Vector3.Dot(distance, origin.up.normalized);
            relativePosition.z = Vector3.Dot(distance, origin.forward.normalized);

            return relativePosition;
        }

    }
}
