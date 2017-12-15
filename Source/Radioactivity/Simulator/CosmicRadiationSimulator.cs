using System;
namespace Radioactivity.Simulator
{
    public class CosmicRadiationSimulator
    {

        double backgroundFlux = 0d;

        public CosmicRadiationSimulator()
        {
            LogUtils.Log("[CosmicRadiationSimulator]: Initializing simulator");
            backgroundFlux = RadioactivityConstants.cosmicRadiationFlux;
        }

        public double CalculateCosmicRadiationFlux(RadiationVessel vessel)
        {
            return vessel.SkyViewFactor * backgroundFlux;
        }

        public double CalculateCosmicRadiationFluxEditor(RadiationVessel vessel)
        {
            return vessel.SkyViewFactor * backgroundFlux;
        }
    }
}
