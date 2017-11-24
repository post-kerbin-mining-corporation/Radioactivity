using System;
using System.Collections.Generic;
using UnityEngine;

namespace Radioactivity.Simulator
{
    /// <summary>
    /// The point radiation simulator - handles all raycast-based point simulations
    /// </summary>
    /// 
    public class PointRadiationSimulator
    {
        // ##### Accessors #####
        public List<RadioactiveSource> AllSources
        { get { return allRadSources; } }

        public List<RadioactiveSink> AllSinks
        { get { return allRadSinks; } }

        public List<RadiationLink> AllLinks
        { get { return allLinks; } }

        List<RadioactiveSource> allRadSources = new List<RadioactiveSource>();
        List<RadioactiveSink> allRadSinks = new List<RadioactiveSink>();
        List<RadiationLink> allLinks = new List<RadiationLink>();

        bool simulationReady = false;

        // ##### Initialization #####

        public PointRadiationSimulator()
        {
            Utils.Log("[PointRadiationSimulator]: Initializing simulator");

            if (HighLogic.LoadedSceneIsEditor)
            {
                GameEvents.onEditorShipModified.Add(new EventData<ShipConstruct>.OnEvent(onEditorVesselModified));
                GameEvents.onEditorRestart.Add(new EventVoid.OnEvent(onEditorVesselReset));
                GameEvents.onEditorStarted.Add(new EventVoid.OnEvent(onEditorVesselStart));
                GameEvents.onEditorLoad.Add(new EventData<ShipConstruct, KSP.UI.Screens.CraftBrowserDialog.LoadType>.OnEvent(onEditorVesselLoad));
                GameEvents.onPartRemove.Add(new EventData<GameEvents.HostTargetAction<Part, Part>>.OnEvent(onEditorVesselPartRemoved));
            }
            else
            {
                GameEvents.onEditorShipModified.Remove(new EventData<ShipConstruct>.OnEvent(onEditorVesselModified));
                GameEvents.onEditorRestart.Remove(new EventVoid.OnEvent(onEditorVesselReset));
                GameEvents.onEditorStarted.Remove(new EventVoid.OnEvent(onEditorVesselStart));
                GameEvents.onEditorLoad.Remove(new EventData<ShipConstruct, KSP.UI.Screens.CraftBrowserDialog.LoadType>.OnEvent(onEditorVesselLoad));
                GameEvents.onPartRemove.Remove(new EventData<GameEvents.HostTargetAction<Part, Part>>.OnEvent(onEditorVesselPartRemoved));
            }

            StartCoroutine(WaitForInit(0.1f));
        }


        protected void InitializeEditorConstruct(ShipConstruct ship)
        {
            if (ship != null)
            {
                for (int i = 0; i < allRadSources.Count; i++)
                {
                    UnregisterSource(allRadSources[i]);
                }
                for (int i = 0; i < allRadSinks.Count; i++)
                {
                    UnregisterSink(allRadSinks[i]);
                }

                for (int i = 0; i < ship.Parts.Count; i++)
                {
                    RadioactiveSource src = ship.Parts[i].gameObject.GetComponent<RadioactiveSource>();
                    RadioactiveSink snk = ship.Parts[i].gameObject.GetComponent<RadioactiveSink>();

                    if (src != null)
                        RegisterSource(src);
                    if (snk != null)
                        RegisterSink(snk);
                }
            }
        }

        protected IEnumerator<float> WaitForInit(float t)
        {
            
            simulationReady = true;
            Utils.Log("[PointRadiationSimulator]: Simulation started...");
            yield return new WaitForSeconds(t);
        }


        // ##### Game Events #####
        public void onEditorVesselReset()
        {
            Utils.Log("[PointRadiationSimulator][Editor]: Vessel RESET, recalculate all parts");
            if (!HighLogic.LoadedSceneIsEditor) { return; }

            InitializeEditorConstruct(EditorLogic.fetch.ship);

        }
        public void onEditorVesselStart()
        {
            Utils.Log("[PointRadiationSimulator][Editor]: Vessel START, recalculate all parts");
            if (!HighLogic.LoadedSceneIsEditor) { return; }

            InitializeEditorConstruct(EditorLogic.fetch.ship);

        }
        public void onEditorVesselLoad(ShipConstruct ship, KSP.UI.Screens.CraftBrowserDialog.LoadType type)
        {
            Utils.Log("[PointRadiationSimulator][Editor]: Vessel LOAD, recalculate all parts");
            if (!HighLogic.LoadedSceneIsEditor) { return; }

            InitializeEditorConstruct(ship);

        }
        public void onEditorVesselPartRemoved(GameEvents.HostTargetAction<Part, Part> p)
        {
            Utils.Log("[PointRadiationSimulator][Editor]: Vessel PART REMOVE, recalculate network");
            if (!HighLogic.LoadedSceneIsEditor) { return; }

            InitializeEditorConstruct(EditorLogic.fetch.ship);

        }
        public void onEditorVesselModified(ShipConstruct ship)
        {
            Utils.Log("[PointRadiationSimulator][Editor]: Vessel MODIFIED, recalculate network");
            if (!HighLogic.LoadedSceneIsEditor) { return; }

            InitializeEditorConstruct(ship);
        }

        // #### Network building ####
        // Add a radiation source to the source list
        public void RegisterSource(RadioactiveSource src)
        {
            allRadSources.Add(src);
            BuildNewRadiationLink(src);
        
            if (RadioactivitySettings.debugNetwork)
                Utils.Log("[PointRadiationSimulator]: Adding radiation source " + src.SourceID + " on part " + src.part.name + " to simulator");
        }

        // Remove a radiation source from the source list
        public void UnregisterSource(RadioactiveSource src)
        {
            if (allRadSources.Count > 0)
            {
                RemoveRadiationLink(src);
                allRadSources.Remove(src);

                if (RadioactivitySettings.debugNetwork && src != null)
                    Utils.Log("[PointRadiationSimulator]: Removing radiation source " + src.SourceID + " on part " + src.part.name + " from simulator");
            }
        }
        // Add a radiation sink to the sink list
        public void RegisterSink(RadioactiveSink snk)
        {

            allRadSinks.Add(snk);
            BuildNewRadiationLink(snk);
            if (RadioactivitySettings.debugNetwork)
                Utils.Log("[PointRadiationSimulator]: Adding radiation sink " + snk.SinkID + " on part " + snk.part.name + " to simulator");
        }
        // Remove a radiation sink from the sink list
        public void UnregisterSink(RadioactiveSink snk)
        {
            if (allRadSinks.Count > 0)
            {
                RemoveRadiationLink(snk);
                allRadSinks.Remove(snk);

                if (RadioactivitySettings.debugNetwork && snk != null)
                    Utils.Log("[PointRadiationSimulator]: Removing radiation sink " + snk.SinkID + " on part " + snk.part.name + " from simulator");
            }
        }
       
        /// <summary>
        /// Adds a new RadiationSource to the network if it does not exist
        /// </summary>
        /// <param name="src">Source.</param>
        protected void TryAddSource(RadioactiveSource src)
        {
            bool exists = false;
            for (int i = 0; i < allRadSources.Count; i++)
            {
                if (allRadSources[i] == src)
                    exists = true;
            }
            if (!exists)
                RegisterSource(src);
        }

        /// <summary>
        /// Adds a new RadioactiveSink to the network if it does not exist
        /// Fails 
        /// </summary>
        /// <param name="snk">Snk.</param>
        protected void TryAddSink(RadioactiveSink snk)
        {
            bool exists = false;
            for (int i = 0; i < allRadSinks.Count; i++)
            {
                if (allRadSinks[i] == snk)
                    exists = true;
            }
            if (!exists)
                RegisterSink(snk);
        }

        /// <summary>
        /// Builds the entire RadiationLink network from scratch
        /// </summary>
        protected void BuildRadiationLinks()
        {
            for (int i = 0; i < allRadSources.Count; i++)
            {
                for (int j = 0; i < allRadSinks.Count; i++)
                {
                    allLinks.Add(new RadiationLink(allRadSources[i], allRadSinks[j]));
                }
            }
        }

        /// <summary>
        /// Adds a particular RadioactiveSink to the network
        /// </summary>
        /// <param name="snk">Snk.</param>
        protected void BuildNewRadiationLink(RadioactiveSink snk)
        {
            for (int i = 0; i < allRadSources.Count; i++)
            {
                RadiationLink l = new RadiationLink(allRadSources[i], snk);
                allLinks.Add(l);
            }
        }
        /// <summary>
        /// Adds a specified RadioactiveSource to the netwrok
        /// </summary>
        /// <param name="src">Source.</param>
        protected void BuildNewRadiationLink(RadioactiveSource src)
        {
            for (int i = 0; i < allRadSinks.Count; i++)
            {
                RadiationLink l = new RadiationLink(src, allRadSinks[i]);
                allLinks.Add(l);


            }
        }
        /// <summary>
        /// Removes all links from a radiation source
        /// </summary>
        /// <param name="src">Source.</param>
        protected void RemoveRadiationLink(RadioactiveSource src)
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
                    allLinks.Remove(toRm[i]);
                }
            }
        }
        /// <summary>
        /// Removes all links to a radiation sink
        /// </summary>
        /// <param name="snk">Snk.</param>
        protected void RemoveRadiationLink(RadioactiveSink snk)
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
                    allLinks.Remove(toRm[i]);
                }
            }
        }

        // #### SIMULATION ######

        public void FixedUpdate()
        {
            

        }

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
          


            SimulateKerbals();
        }

        /// <summary>
        /// Simulates all point radiation
        /// </summary>
        protected void SimulatePointRadiation(float fixedDeltaTime)
        {
            for (int i = 0; i < allLinks.Count; i++)
            {
                // Simulate the radiation based on precomputed pathways
                allLinks[i].Simulate(fixedDeltaTime);
                allLinks[i].TestRecompute();
            }
        }

        // EDITOR radiation simulation
        // simulate point radiation
        protected void SimulatePointRadiationEditor(float fixedDeltaTime)
        {
            for (int i = 0; i < allLinks.Count; i++)
            {
                // Simulate the radiation based on precomputed pathways
                allLinks[i].SimulateEditor(fixedDeltaTime);
                allLinks[i].TestRecompute();
            }

        }

        // simulate everything we need to do with Kerbals
        protected void SimulateKerbals()
        {
            if (KerbalTracking.Instance != null)
            {
                KerbalTracking.Instance.SimulateKerbals(TimeWarp.fixedDeltaTime);
            }

        }

    }
}
