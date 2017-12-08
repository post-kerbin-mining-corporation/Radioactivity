using System;
using UnityEngine;
using Radioactivity;
namespace Radioactivity.Simulator
{
    /// <summary>
    /// The main simulator class. This hosts all simulators
    /// </summary>
    public class RadioactivitySimulator
    {
        public bool Enabled
        {
            get { return simulatorEnabled; }
            set { simulatorEnabled = value; }
        }
        public PointRadiationSimulator PointSim
        {
            get { return pointSim; }
        }
        public AmbientRadiationSimulator AmbientSim
        {
            get { return ambientSim; }
        }
        public KerbalSimulator KerbalSim
        {
            get { return kerbalSim; }
        }

        bool simulatorEnabled = false;

        KerbalSimulator kerbalSim;
        PointRadiationSimulator pointSim;
        AmbientRadiationSimulator ambientSim;

        /// <summary>
        /// Constructor - initializes simulators
        /// </summary>
        public RadioactivitySimulator()
        {
            Utils.Log("[Simulator]: Initializing simulators");

            if (RadioactivitySimulationSettings.SimulatePointRadiation)
            {
                Utils.Log("[Simulator]: Point radiation enabled");
                pointSim = new PointRadiationSimulator();
            }
            if (RadioactivitySimulationSettings.SimulateAmbientRadiation)
            {
                Utils.Log("[Simulator]: Ambient radiation enabled");
                ambientSim = new AmbientRadiationSimulator();
            }
            kerbalSim = new KerbalSimulator();
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
                //if (KerbalTracking.Instance != null)
                //{
                //    KerbalTracking.Instance.SimulateKerbals(TimeWarp.fixedDeltaTime);
                //}


            }
        }



    }
}
