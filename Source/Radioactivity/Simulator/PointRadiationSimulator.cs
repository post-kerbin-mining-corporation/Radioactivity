using System;
using System.Collections.Generic;
using UnityEngine;
using Radioactivity.UI;

namespace Radioactivity.Simulator
{
    /// <summary>
    /// The point radiation simulator - handles all raycast-based point simulations
    /// </summary>
    /// 
    public class PointRadiationSimulator
    {

        public bool SimReady {get {return simulationReady;} set { simulationReady = value; }}
        // ##### Accessors #####

        public List<RadiationLink> AllLinks
        { get { return allLinks; } }

        bool simulationReady = false;
        RadioactivitySimulator mainSimulator;
        List<RadiationLink> allLinks = new List<RadiationLink>();


        // ##### Initialization #####

        public PointRadiationSimulator(RadioactivitySimulator hostSim)
        {
            LogUtils.Log("[PointRadiationSimulator]: Initializing simulator");
            mainSimulator = hostSim;
        }

        /// <summary>
        /// Builds the entire RadiationLink network from scratch
        /// </summary>
        public void BuildRadiationLinks()
        {
            for (int i = 0; i < mainSimulator.AllSources.Count; i++)
            {
                for (int j = 0; i < mainSimulator.AllSinks.Count; i++)
                {
                    RadiationLink l = new RadiationLink(mainSimulator.AllSources[i], mainSimulator.AllSinks[j]);
                    RadioactivityUI.Instance.LinkAdded(l);
                    allLinks.Add(l);
                }
            }
        }

        /// <summary>
        /// Adds a particular RadioactiveSink to the network
        /// </summary>
        /// <param name="snk">Snk.</param>
        public void BuildNewRadiationLink(RadioactiveSink snk)
        {
            for (int i = 0; i < mainSimulator.AllSources.Count; i++)
            {
                RadiationLink l = new RadiationLink(mainSimulator.AllSources[i], snk);
                RadioactivityUI.Instance.LinkAdded(l);
                allLinks.Add(l);
            }
        }
        /// <summary>
        /// Adds a specified RadioactiveSource to the netwrok
        /// </summary>
        /// <param name="src">Source.</param>
        public void BuildNewRadiationLink(RadioactiveSource src)
        {
            for (int i = 0; i < mainSimulator.AllSinks.Count; i++)
            {
                RadiationLink l = new RadiationLink(src, mainSimulator.AllSinks[i]);
                RadioactivityUI.Instance.LinkAdded(l);
                allLinks.Add(l);
            }
        }
        /// <summary>
        /// Removes all links from a radiation source
        /// </summary>
        /// <param name="src">Source.</param>
        public void RemoveRadiationLink(RadioactiveSource src)
        {
            List<RadiationLink> toRm = new List<RadiationLink>();
            for (int i = 0; i < allLinks.Count; i++)
            {
                if (allLinks[i].source == src)
                {
                    toRm.Add(allLinks[i]);
                }
            }
            if (toRm.Count > 0)
            {
                for (int i = 0; i < toRm.Count; i++)
                {
                    toRm[i].CleanupSink();
                    RadioactivityUI.Instance.LinkRemoved(toRm[i]);
                    allLinks.Remove(toRm[i]);
                }
            }
        }
        /// <summary>
        /// Removes all links to a radiation sink
        /// </summary>
        /// <param name="snk">Snk.</param>
        public void RemoveRadiationLink(RadioactiveSink snk)
        {
            List<RadiationLink> toRm = new List<RadiationLink>();
            for (int i = 0; i < allLinks.Count; i++)
            {
                if (allLinks[i].sink == snk)
                {
                    toRm.Add(allLinks[i]);
                }
            }
            if (toRm.Count > 0)
            {
                for (int i = 0; i < toRm.Count; i++)
                {
                    toRm[i].CleanupSink();
                    RadioactivityUI.Instance.LinkRemoved(toRm[i]);
                    allLinks.Remove(toRm[i]);
                }
            }
        }

        // #### SIMULATION ######

        /// <summary>
        /// Simulates the radiation 
        /// </summary>
        public void Simulate(float fixedDeltaTime)
        {
            if (simulationReady)
            {
                if (HighLogic.LoadedSceneIsEditor)
                {
                    if (EditorLogic.fetch != null)
                    {
                        if (EditorLogic.fetch.ship != null)
                        {
                            SimulatePointRadiationEditor(fixedDeltaTime);
                        }
                    }
                }
                else if (HighLogic.LoadedSceneIsFlight)
                {
                    SimulatePointRadiation(fixedDeltaTime);
                }
            }
        }

        /// <summary>
        /// FLIGHT point radiatio simulation
        /// </summary>
        protected void SimulatePointRadiation(float fixedDeltaTime)
        {
            for (int i = 0; i < allLinks.Count; i++)
            {
                // Propagate the radiation based on precomputed pathways
                allLinks[i].Simulate(fixedDeltaTime);
                // Test to see if network geometry needs to be recomputed
                allLinks[i].TestRecompute();
            }
        }

        /// <summary>
        /// EDITOR point radiation simulation
        /// </summary>
        /// <param name="fixedDeltaTime">Fixed delta time.</param>
        protected void SimulatePointRadiationEditor(float fixedDeltaTime)
        {
            for (int i = 0; i < allLinks.Count; i++)
            {
                // Propagate the radiation based on precomputed pathways
                allLinks[i].SimulateEditor(fixedDeltaTime);
                // Test to see if network geometry needs to be recomputed
                allLinks[i].TestRecompute();
            }
        }
    }
}
