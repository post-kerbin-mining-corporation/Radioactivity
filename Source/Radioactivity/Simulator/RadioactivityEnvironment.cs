using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Radioactivity;

namespace Radioactivity.Simulator
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

            foreach (ConfigNode magNode in node.GetNodes("MAGNETOSPHERE"))
            {
                Magnetosphere mag = new Magnetosphere(magNode);
            }
            foreach (ConfigNode beltNode in node.GetNodes("RADIATIONBELT"))
            {
                RadiationBelt belt = new RadiationBelt(beltNode);
            }
        }

        public double GetBeltFlux(double lat, double lon, double alt)
        {
            double flux = 0d;
            for (int i = 0; i < RadiationBelts.Count; i++)
            {
                flux += RadiationBelts[i].GetBeltFlux(lat, lon, alt);
            }
            return flux;
        }
        public double GetMagnetosphereAttenuation(double lat, double lon, double alt)
        {
            double atten = 0d;
            for (int i = 0; i < Magnetospheres.Count; i++)
            {
                atten += Magnetospheres[i].GetAttenuation(lat, lon, alt);
            }
            return atten;
        }
    }

    public class Magnetosphere
    {
        public string name;
        public double attenuation;
        public double altitude;
        public double altitudeMin;
        public double altitudeMax;

        public Magnetosphere(ConfigNode node)
        {
            name = ConfigNodeUtils.GetValue(node, "name", "DefaultMagentosphere");
            attenuation = ConfigNodeUtils.GetValue(node, "MaximumAttenuation", 0d);
            altitude = ConfigNodeUtils.GetValue(node, "AltitudeCenter", 0d);
            altitudeMax = ConfigNodeUtils.GetValue(node, "AltitudeMax", 0d);
            altitudeMin = ConfigNodeUtils.GetValue(node, "AltitudeMin", 0d);
        }

        public virtual double GetAttenuation(double lat, double lon, double alt)
        {
            if (alt <= altitude)
                return (alt - altitudeMin) / (altitude - altitudeMin) * attenuation;
            else
                return (alt - altitude) / (altitudeMax - altitude) * attenuation;
        }
    }
    public class RadiationBelt
    {
        public string name;
        public double maximumFlux;
        public double latitude;
        public double latitudeMin;
        public double latitudeMax;
        public double altitude;
        public double altitudeMin;
        public double altitudeMax;


        public RadiationBelt(ConfigNode node)
        {
            name = ConfigNodeUtils.GetValue(node, "name", "DefaultRadiationBelt");
            maximumFlux = ConfigNodeUtils.GetValue(node, "MaximumFlux", 0d);
            altitude = ConfigNodeUtils.GetValue(node, "AltitudeCenter", 0d);
            altitudeMax = ConfigNodeUtils.GetValue(node, "AltitudeMax", 0d);
            altitudeMin = ConfigNodeUtils.GetValue(node, "AltitudeMin", 0d);
            latitude = ConfigNodeUtils.GetValue(node, "LatitudeCenter", 0d);
            latitudeMin = ConfigNodeUtils.GetValue(node, "LatitudeMin", 0d);
            latitudeMax = ConfigNodeUtils.GetValue(node, "LatitudeMax", 0d);
        }

        public virtual double GetBeltFlux(double lat, double lon, double alt)
        {
            double alt_param;
            double lat_param;

            if (alt <= altitude)
                alt_param = (alt - altitudeMin) / (altitude - altitudeMin);
            else
                alt_param = (alt - altitude) / (altitudeMax - altitude);

            if (lat <= latitude)
                lat_param = (lat - latitudeMin) / (latitude - latitudeMin);
            else
                lat_param = (lat - latitude) / (latitudeMax - latitude);

            return lat_param * alt_param * maximumFlux;
        }
    }



}
