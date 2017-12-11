using System;
using System.Collections.Generic;
using UnityEngine;
using Radioactivity;
using Radioactivity.UI;

namespace Radioactivity.Simulator
{
    /// <summary>
    /// The main simulator class. This hosts all simulators
    /// </summary>
    public class RadioactivitySimulator
    {
        #region Accessors
        public bool Enabled
        {
            get { return simulatorEnabled; }
            set { simulatorEnabled = value; }
        }
        public PointRadiationSimulator PointSim { get { return pointSim; } }
        public AmbientRadiationSimulator AmbientSim { get { return ambientSim; } }
        public KerbalSimulator KerbalSim { get { return kerbalSim; } }
        public List<RadioactiveSink> AllSinks { get { return allRadSinks; } }
        public List<RadioactiveSource> AllSources { get { return allRadSources; } }

        #endregion

        bool simulatorEnabled = false;

        // Simulators
        KerbalSimulator kerbalSim;
        PointRadiationSimulator pointSim;
        AmbientRadiationSimulator ambientSim;

        List<RadioactiveSink> allRadSinks = new List<RadioactiveSink>();
        List<RadioactiveSource> allRadSources = new List<RadioactiveSource>();

        /// <summary>
        /// Constructor - initializes simulators
        /// </summary>
        public RadioactivitySimulator()
        {
            LogUtils.Log("[RadioactivitySimulator]: Initializing simulators");

            if (RadioactivitySimulationSettings.SimulatePointRadiation)
            {
                LogUtils.Log("[RadioactivitySimulator]: Point radiation enabled");
                pointSim = new PointRadiationSimulator(this);
            }
            if (RadioactivitySimulationSettings.SimulateAmbientRadiation)
            {
                LogUtils.Log("[RadioactivitySimulator]: Ambient radiation enabled");
                ambientSim = new AmbientRadiationSimulator(this);
            }
            kerbalSim = new KerbalSimulator();

            SetupEditorCallbacks();

        }


        public void StartSimulation()
        {
            Enabled = true;
            if (RadioactivitySimulationSettings.SimulatePointRadiation)
            {
                PointSim.SimReady = true;
            }
            if (RadioactivitySimulationSettings.SimulateAmbientRadiation)
            {
            }
        }

        /// <summary>
        /// Does the actual simulation given a delta time
        /// </summary>
        /// <returns>The simulate.</returns>
        /// <param name="fixedDeltaTime">Fixed delta time.</param>
        public void Simulate(float fixedDeltaTime)
        {
            if (Enabled)
            {
                // Simulate ambient radiation
                if (RadioactivitySimulationSettings.SimulateAmbientRadiation)
                    ambientSim.Simulate(fixedDeltaTime);

                // Simulate point radiation
                if (RadioactivitySimulationSettings.SimulatePointRadiation)
                    pointSim.Simulate(fixedDeltaTime);

                // simulate everything we need to do with Kerbals
                kerbalSim.Simulate(fixedDeltaTime);
            }
        }

        #region Network Building
        // Add a radiation source to the source list
        public void RegisterSource(RadioactiveSource src)
        {
            allRadSources.Add(src);
            if (RadioactivitySimulationSettings.SimulatePointRadiation)
                pointSim.BuildNewRadiationLink(src);

            RadioactivityUI.Instance.SourceAdded(src);
            if (RadioactivityConstants.debugNetwork)
                LogUtils.Log("[RadioactivitySimulator]: Adding radiation source " + src.SourceID + " on part " + src.part.name + " to simulator");
        }

        // Remove a radiation source from the source list
        public void UnregisterSource(RadioactiveSource src)
        {
            if (allRadSources.Count > 0)
            {
                if (RadioactivitySimulationSettings.SimulatePointRadiation)
                    pointSim.RemoveRadiationLink(src);
                if (RadioactivitySimulationSettings.SimulatePointRadiation)
                    allRadSources.Remove(src);
                RadioactivityUI.Instance.SourceRemoved(src);
                if (RadioactivityConstants.debugNetwork && src != null)
                    LogUtils.Log("[RadioactivitySimulator]: Removing radiation source " + src.SourceID + " on part " + src.part.name + " from simulator");
            }
        }
        // Add a radiation sink to the sink list
        public void RegisterSink(RadioactiveSink snk)
        {

            allRadSinks.Add(snk);
            if (RadioactivitySimulationSettings.SimulatePointRadiation)
                pointSim.BuildNewRadiationLink(snk);
            if (RadioactivitySimulationSettings.SimulateAmbientRadiation)
                ambientSim.AddSink(snk);
                        
            RadioactivityUI.Instance.SinkAdded(snk);
            if (RadioactivityConstants.debugNetwork)
                LogUtils.Log("[RadioactivitySimulator]: Adding radiation sink " + snk.SinkID + " on part " + snk.part.name + " to simulator");
        }
        // Remove a radiation sink from the sink list
        public void UnregisterSink(RadioactiveSink snk)
        {
            if (allRadSinks.Count > 0)
            {
                if (RadioactivitySimulationSettings.SimulatePointRadiation)
                    pointSim.RemoveRadiationLink(snk);
                if (RadioactivitySimulationSettings.SimulateAmbientRadiation)
                    ambientSim.RemoveSink(snk);
                
                allRadSinks.Remove(snk);
                RadioactivityUI.Instance.SinkRemoved(snk);
                if (RadioactivityConstants.debugNetwork && snk != null)
                    LogUtils.Log("[RadioactivitySimulator]: Removing radiation sink " + snk.SinkID + " on part " + snk.part.name + " from simulator");
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
        #endregion

        #region Editor 
        protected void SetupEditorCallbacks()
        {
            /// Add events for editor modifications
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
        #endregion


        #region Game Events
        public void onEditorVesselReset()
        {
            LogUtils.Log("[RadioactivitySimulator][Editor]: Vessel RESET, recalculate all parts");
            if (!HighLogic.LoadedSceneIsEditor) { return; }
            InitializeEditorConstruct(EditorLogic.fetch.ship);
        }
        public void onEditorVesselStart()
        {
            LogUtils.Log("[RadioactivitySimulator][Editor]: Vessel START, recalculate all parts");
            if (!HighLogic.LoadedSceneIsEditor) { return; }
            InitializeEditorConstruct(EditorLogic.fetch.ship);
        }
        public void onEditorVesselLoad(ShipConstruct ship, KSP.UI.Screens.CraftBrowserDialog.LoadType type)
        {
            LogUtils.Log("[RadioactivitySimulator][Editor]: Vessel LOAD, recalculate all parts");
            if (!HighLogic.LoadedSceneIsEditor) { return; }
            InitializeEditorConstruct(ship);
        }
        public void onEditorVesselPartRemoved(GameEvents.HostTargetAction<Part, Part> p)
        {
            LogUtils.Log("[RadioactivitySimulator][Editor]: Vessel PART REMOVE, recalculate network");
            if (!HighLogic.LoadedSceneIsEditor) { return; }
            InitializeEditorConstruct(EditorLogic.fetch.ship);
        }
        public void onEditorVesselModified(ShipConstruct ship)
        {
            LogUtils.Log("[RadioactivitySimulator][Editor]: Vessel MODIFIED, recalculate network");
            if (!HighLogic.LoadedSceneIsEditor) { return; }
            InitializeEditorConstruct(ship);
        }
        #endregion
    }
}
