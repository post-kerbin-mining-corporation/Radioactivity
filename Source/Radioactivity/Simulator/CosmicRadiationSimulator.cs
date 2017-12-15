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
            return vessel.SkyViewFactor * backgroundFlux * (1.0 - RadioactivityEnvironmentData.GetAttenuation(vessel.vessel.GetWorldPos3D(), vessel.vessel.mainBody));
        }

        public double CalculateCosmicRadiationFluxEditor(RadiationVessel vessel)
        {
            return vessel.SkyViewFactor * backgroundFlux * (1.0 - RadioactivityPreferences.editorMagneticFieldStrength);
        }
    }
}
