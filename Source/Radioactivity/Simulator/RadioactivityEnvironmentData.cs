using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace Radioactivity.Simulator
{

    public static class RadioactivityEnvironmentData
    {


        public static Dictionary<string, RadioactivityEnvironment> Environments;

        public static void Load()
        {
            ConfigNode settingsNode;

            LogUtils.Log("[RadioactivityEnvironmentData]: Started loading");
            if (GameDatabase.Instance.ExistsConfigNode("Radioactivity/ENVIRONMENTDATA"))
            {
                LogUtils.Log("[RadioactivityEnvironmentData]: Located body data file");
                settingsNode = GameDatabase.Instance.GetConfigNode("Radioactivity/ENVIRONMENTDATA");

                ConfigNode[] bodyNodes = settingsNode.GetNodes("RADIOACTIVITYBODY");

                foreach (ConfigNode bodyNode in bodyNodes)
                {
                    RadioactivityEnvironment newEnv = new RadioactivityEnvironment(bodyNode);
                    RadioactivityEnvironmentData.Environments.Add(newEnv.BodyName, newEnv);
                    LogUtils.Log(String.Format("[RadioactivityEnvironmentData]: Loaded data for {0}", newEnv.BodyName));
                }
            }
            else
            {
                LogUtils.LogWarning("[RadioactivityEnvironmentData]: Couldn't find data file!!");
            }
            LogUtils.Log("[RadioactivityEnvironmentData]: Finished loading");
        }


        public static double GetBeltRadiation(Vector3d pos, CelestialBody mainBody)
        {
            double beltFlux = 0d;
            List<RadioactivityEnvironment> toSample = RadioactivityEnvironmentData.GetEnvironments(mainBody);
            for (int i = 0; i < toSample.Count; i++)
            {
                double lat;
                double lon;
                double alt;

                FlightGlobals.GetBodyByName(toSample[i].BodyName).GetLatLonAlt(pos, out lat, out lon, out alt);
                beltFlux += toSample[i].GetBeltFlux(lat, lon, alt);
            }
            return beltFlux;
        }

        public static double GetAttenuation(Vector3d pos, CelestialBody mainBody)
        {
            double attenuation = 0d;
            List<RadioactivityEnvironment> toSample = RadioactivityEnvironmentData.GetEnvironments(mainBody);
            for (int i = 0; i < toSample.Count; i++)
            {
                double lat;
                double lon;
                double alt;
                FlightGlobals.GetBodyByName(toSample[i].BodyName).GetLatLonAlt(pos, out lat, out lon, out alt);
                attenuation += toSample[i].GetMagnetosphereAttenuation(lat, lon, alt);
            }
            return attenuation;
        }

        static List<RadioactivityEnvironment> GetEnvironments(CelestialBody mainBody)
        {
            List<RadioactivityEnvironment> envs = new List<RadioactivityEnvironment>();
            if (RadioactivityEnvironmentData.Environments.ContainsKey(mainBody.name))
                envs.Add(RadioactivityEnvironmentData.Environments[mainBody.name]);

            if (mainBody.orbitingBodies != null && mainBody.orbitingBodies.Count > 0)
            {
                for (int i = 0; i > mainBody.orbitingBodies.Count; i++)
                {
                    if (RadioactivityEnvironmentData.Environments.ContainsKey(mainBody.orbitingBodies[i].name))
                    {
                        envs.Add(RadioactivityEnvironmentData.Environments[mainBody.orbitingBodies[i].name]);
                    }
                }
            }
            return envs;
        }

    }



}
