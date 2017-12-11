// Utils
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Radioactivity
{
    public static class VesselUtils
    {

        // Solid angle of a body relative to a vessel
        public static double ComputeBodySolidAngle(Vessel vessel, CelestialBody body)
        {
            return VesselUtils.ComputeBodySolidAngle(body.Radius, body.GetAltitude(vessel.GetWorldPos3D()));
        }

        public static double ComputeBodySolidAngle(double radius, double altitude)
        {
            double theta = Math.Atan(radius / altitude);
            return 2.0 * Math.PI * (1.0 - Math.Cos(theta));
        }

        // Solid angle of the sky, that is, the total area minus all bodies
        public static double ComputeSkySolidAngle(Vessel vessel)
        {
            double totalBodyAngle = 0d;
            foreach (CelestialBody body in FlightGlobals.fetch.bodies)
            {
                totalBodyAngle += VesselUtils.ComputeBodySolidAngle(vessel, body);
            }
            return 4d * Math.PI - totalBodyAngle;
        }

    }
}
