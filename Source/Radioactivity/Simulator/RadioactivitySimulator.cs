using System;

namespace Radioactivity.Simulator
{
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

        bool simulatorEnabled = false;

        PointRadiationSimulator pointSim;
        AmbientRadiationSimulator ambientSim;

        public RadioactivitySimulator() {
            Utils.Log("[Simulator]: Initializing simulators");
            if (RadioactivitySettings.simulatePointRadiation)
            {
                Utils.Log("[Simulator]: Point radiation enabled");
                pointSim = new PointRadiationSimulator();
            }
            if (RadioactivitySettings.simulateAmbientRadiation)
            {   
                Utils.Log("[Simulator]: Ambient radiation enabled");
                ambientSim = new AmbientRadiationSimulator();
            }
        }

        public void Simulate(float fixedDeltaTime)
        {
            if (Enabled)
            {
                // Simulate vessel radiation
                if (RadioactivitySettings.simulateAmbientRadiation)
                    ambientSim.Simulate(fixedDeltaTime);
                
                // Simulate point radiation
                if (RadioactivitySettings.simulatePointRadiation)
                    pointSim.Simulate(fixedDeltaTime);
            }
        }



    }
}
