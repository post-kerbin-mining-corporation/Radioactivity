using System;

using System.Collections.Generic;

namespace Radioactivity.Simulator
{
    public class AmbientRadiationSimulator
    {
        public bool SimReady { get { return simulationReady; } set { simulationReady = value; } }

        bool simulationReady = false;
        RadioactivitySimulator mainSimulator;
        List<RadiationVessel> allVessels;

        RadiationVessel editorVessel;

        public AmbientRadiationSimulator(RadioactivitySimulator hostSim)
        {
            
            LogUtils.Log("[AmbientRadiationSimulator]: Initializing simulator");
            allVessels = new List<RadiationVessel>();
            mainSimulator = hostSim;

            if (HighLogic.LoadedSceneIsEditor)
            {
                editorVessel = new RadiationVessel();
            }
        }

        protected void BuildVesselList()
        {
            if (HighLogic.LoadedSceneIsGame)
            {
                for (int i = 0; i < FlightGlobals.fetch.vessels.Count; i++)
                {
                    TryAddRadiationVessel(FlightGlobals.fetch.vessels[i]);
                }
                GameEvents.onNewVesselCreated.Add(new EventData<Vessel>.OnEvent(OnNewVesselCreated));
                GameEvents.onVesselDestroy.Add(new EventData<Vessel>.OnEvent(OnVesselDestroyed));
                GameEvents.onVesselCreate.Add(new EventData<Vessel>.OnEvent(OnVesselCreated));
                GameEvents.onVesselTerminated.Add(new EventData<ProtoVessel>.OnEvent(OnVesselTerminated));
                GameEvents.onVesselRecovered.Add(new EventData<ProtoVessel, bool>.OnEvent(OnVesselRecovered));
            }
        }
        protected void OnNewVesselCreated(Vessel v)
        {
            LogUtils.Log("[AmbientRadiationSimulator][Event]: NEW vessel created");
            TryAddRadiationVessel(v);
        }
        protected void OnVesselDestroyed(Vessel v)
        {
            LogUtils.Log("[AmbientRadiationSimulator][Event]: Vessel destroyed");
        }
        protected void OnVesselCreated(Vessel v)
        {
            LogUtils.Log("[AmbientRadiationSimulator][Event]: Vessel created");
            TryAddRadiationVessel(v);
        }
        protected void OnVesselTerminated(ProtoVessel v)
        {
            LogUtils.Log("[AmbientRadiationSimulator][Event]: Vessel terminated");
        }
        protected void OnVesselRecovered(ProtoVessel v, bool b)
        {
            LogUtils.Log("[AmbientRadiationSimulator][Event]: Vessel recovered");
        }
        protected void TryAddRadiationVessel(Vessel v)
        {
            bool exists = false;
            for (int i = 0; i < allVessels.Count; i++)
            {
                if (allVessels[i].vessel == v)
                    exists = true;
            }
            if (!exists)
            {
                RegisterVessel(v);
            }
        }

        protected RadiationVessel RegisterVessel(Vessel v)
        {
            RadiationVessel radVessel = new RadiationVessel(v);
            allVessels.Add(radVessel);
            LogUtils.Log("[AmbientRadiationSimulator]: Adding vessel " + radVessel.vessel.GetName() + " to simulator");
            return radVessel;
        }
        protected void UnregisterVessel(Vessel v)
        {
            RadiationVessel toRemove = null;
            for (int i = 0; i < allVessels.Count; i++)
            {
                if (allVessels[i].vessel == v)
                    toRemove = allVessels[i];
            }

            if (toRemove != null)
            {
                allVessels.Remove(toRemove);
                LogUtils.Log("[AmbientRadiationSimulator]: Removing vessel " + v.GetName() + " from simulator");
            }
        }

       

        public void AddSink(RadioactiveSink snk)
        {
            if (HighLogic.LoadedSceneIsFlight)
                AddSinkFlight(snk);
            if (HighLogic.LoadedSceneIsEditor)
                AddSinkEditor(snk);
        }
        protected void AddSinkFlight(RadioactiveSink snk)
        {
            bool exists = false;
            for (int i = 0; i < allVessels.Count; i++)
            {
                if (allVessels[i].vessel == snk.vessel)
                {
                    exists = true;
                    allVessels[i].AddSink(snk);
                }
            }
            if (!exists)
            {
                RadiationVessel rVes = RegisterVessel(snk.vessel);
                rVes.AddSink(snk);
            }
        }
        protected void AddSinkEditor(RadioactiveSink snk)
        {
            editorVessel.AddSink(snk);
        }
        public void RemoveSink(RadioactiveSink snk)
        {
            if (HighLogic.LoadedSceneIsFlight)
                RemoveSinkFlight(snk);
            if (HighLogic.LoadedSceneIsEditor)
                RemoveSinkEditor(snk);
        }
        protected void RemoveSinkFlight(RadioactiveSink snk)
        {
            for (int i = 0; i < allVessels.Count; i++)
            {
                if (allVessels[i].vessel == snk.vessel)
                {
                    allVessels[i].RemoveSink(snk);
                }
            }
        }
        protected void RemoveSinkEditor(RadioactiveSink snk)
        {
            editorVessel.RemoveSink(snk);
        }

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
                            SimulateAmbientRadiationEditor(fixedDeltaTime);
                        }
                    }
                }

                // We need to continue to work on the ships in the sim
                SimulateAmbientRadiation(fixedDeltaTime);

            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void SimulateAmbientRadiation(float fixedDeltaTime)
        {
            for (int i = 0; i < allVessels.Count; i++)
            {
                allVessels[i].Simulate(fixedDeltaTime);
            }
        }

        /// <summary>
        /// EDITOR point radiation simulation
        /// </summary>
        /// <param name="fixedDeltaTime">Fixed delta time.</param>
        protected void SimulateAmbientRadiationEditor(float fixedDeltaTime)
        {
            if (editorVessel != null)
                editorVessel.Simulate(fixedDeltaTime);
        }
    }
}
