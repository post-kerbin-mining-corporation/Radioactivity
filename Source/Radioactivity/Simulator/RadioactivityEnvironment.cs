using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace Radioactivity
{
    /// <summary>
    /// Represents a radioactivity environment for a celestial body
    /// </summary>
    public class RadioactivityEnvironment
    {
        public string BodyName;
        public List<Magnetosphere> Magnetospheres;
        public List<RadiationBelt> RadiationBelts;

        public RadioactivityEnvironment(ConfigNode node)
        {
        }

        public double GetBeltFlux(Vector3d pos)
        {
            return 0d;
        }
        public double GetMagnetosphereAttenuation(Vector3d pos)
        {
            return 0d;
        }
    }

    public class Magnetosphere
    {
        public Magnetosphere(ConfigNode node)
        {
        }
    }
    public class RadiationBelt
    {

        public double beltCenterRadius;
        public double beltCenterLatitude;
        public double beltWidth;
        public double beltHeight;

        public RadiationBelt(ConfigNode node)
        {
        }

        public virtual double GetBeltFlux()
        {
            return 0d;
        }
    }



}
