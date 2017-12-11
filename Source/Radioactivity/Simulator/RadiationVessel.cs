using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Radioactivity;
using Radioactivity.UI;

namespace Radioactivity.Simulator
{
    /// <summary>
    /// Represents a vessel that interacts with the radiation simulation
    /// </summary>
    public class RadiationVessel
    {
        public List<RadioactiveSink> sinks;
        public Vessel vessel;
        public ProtoVessel protoVessel;

        public RadiationVessel()
        {
            LogUtils.Log("[RadiationVessel]: Created from NOTHING");
            sinks = new List<RadioactiveSink>();
        }
        public RadiationVessel(Vessel v)
        {
            LogUtils.Log("[RadiationVessel]: Created from Vessel");
            vessel = v;
            protoVessel = v.protoVessel;
            sinks = new List<RadioactiveSink>();
        }
        public RadiationVessel(ProtoVessel pv)
        {
            LogUtils.Log("[RadiationVessel]: Created from ProtoVessel");
            vessel = pv.vesselRef;
            protoVessel = pv;
            sinks = new List<RadioactiveSink>();
        }


        public void AddSink(RadioactiveSink snk)
        {
            bool exists = false;
            for (int i = 0; i < sinks.Count; i++)
            {
                if (sinks[i] == snk)
                    exists = true;
            }
            if (!exists)
            {
                sinks.Add(snk);
            }
        }
        public void RemoveSink(RadioactiveSink snk)
        {}

        public void Simulate(float timeStep)
        {
            if (RadioactivitySimulationSettings.SimulateCosmicRadiation)
            {
                for (int i = 0; i < sinks.Count; i++)
                sinks[i].AddRadiation("Cosmic", 1f);    
            }
        }
        public void SimulateEditor(float timeStep)
        {
            
        }
    }
}
