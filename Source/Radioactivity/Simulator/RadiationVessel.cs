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

        public double SkyViewFactor { get { return skyViewFactor; } }
        public double GroundViewFactor { get { return groundViewFactor; } }

        public double CosmicFlux { get { return cosmicFlux; } set { cosmicFlux = value; } }
        public double SolarFlux { get { return solarFlux; } set { solarFlux = value; } }
        public double PlanetaryFlux { get { return planetaryFlux; } set { planetaryFlux = value; } }
        public double BeltFlux { get { return beltFlux; } set { beltFlux = value; } }
        public double TotalFlux { get { return beltFlux + cosmicFlux + planetaryFlux + solarFlux; } }


        protected double skyViewFactor = 1.0d;
        protected double groundViewFactor = 0.0d;

        protected double cosmicFlux = 0d;
        protected double beltFlux = 0d;
        protected double solarFlux = 0d;
        protected double planetaryFlux = 0d;

        protected AmbientRadiationSimulator simulator;

        protected LayerMask raycastMask;
        protected bool needsSVFRecalculation = true;
        protected Vector3d worldPosition;

        public RadiationVessel(AmbientRadiationSimulator sim)
        {
            LogUtils.Log("[RadiationVessel]: Created from NOTHING");
            sinks = new List<RadioactiveSink>();
            simulator = sim;
        }
        public RadiationVessel(Vessel v, AmbientRadiationSimulator sim)
        {
            LogUtils.Log(String.Format("[RadiationVessel]: Created from Vessel with loaded state {0}", v.loaded));
            vessel = v;
            protoVessel = v.protoVessel;
            sinks = new List<RadioactiveSink>();
            simulator = sim;
            SetupRaycasting();

        }
        public RadiationVessel(ProtoVessel pv, AmbientRadiationSimulator sim)
        {
            LogUtils.Log("[RadiationVessel]: Created from ProtoVessel");
            vessel = pv.vesselRef;
            protoVessel = pv;
            sinks = new List<RadioactiveSink>();
            simulator = sim;
            SetupRaycasting();

        }
        protected void SetupRaycasting()
        {
            /// Set up the mask for casting
            LayerMask maskB = 1 << LayerMask.NameToLayer("TerrainColliders");
            LayerMask maskC = 1 << LayerMask.NameToLayer("Local Scenery");
            raycastMask = maskB | maskC;
            worldPosition = vessel.GetWorldPos3D();
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
        { }

        public void Simulate(float timeStep)
        {
            // If vessel is loaded 
            if (vessel != null && vessel.loaded)
            {
                if ((vessel.GetWorldPos3D() - worldPosition).sqrMagnitude > RadioactivityConstants.groundSVFPositionDelta)
                {
                    needsSVFRecalculation = true;
                }
                // If landed, we do a high complexity check for angles
                if (vessel.checkLanded() && needsSVFRecalculation)
                {

                    skyViewFactor = CalculateSVFComplex();
                    groundViewFactor = 1.0d - skyViewFactor;
                    LogUtils.Log(String.Format("[RadiationVessel][Raycaster]: Calculated complex SVF as {0:F2}, GVF as {1:F2}", skyViewFactor, groundViewFactor));
                    needsSVFRecalculation = false;
                }

                if (!vessel.checkLanded())
                {
                    groundViewFactor = VesselUtils.ComputeBodySolidAngle(vessel, vessel.mainBody) / (4d * Math.PI);
                    skyViewFactor = 1.0d - groundViewFactor;
                }


                if (RadioactivitySimulationSettings.SimulateCosmicRadiation)
                {
                    cosmicFlux = simulator.CosmicSim.CalculateCosmicRadiationFlux(this);

                    for (int i = 0; i < sinks.Count; i++)
                    {
                        sinks[i].AddAmbientRadiation("Cosmic", cosmicFlux);
                    }
                }
                if (RadioactivitySimulationSettings.SimulateLocalRadiation)
                {
                    planetaryFlux = simulator.PlanetSim.CalculatePlanetaryRadiationFlux(this);

                    for (int i = 0; i < sinks.Count; i++)
                    {
                        sinks[i].AddAmbientRadiation("Planetary", planetaryFlux);
                    }
                }
            }
            if (vessel != null)
            {
                List<ProtoCrewMember> crew = vessel.GetVesselCrew();
                for (int i = 0; i < crew.Count(); i++)
                {
                    Radioactivity.Instance.RadSim.KerbalSim.SetAmbientIrradiation(crew[i], TotalFlux);
                }
            }
        }


        public void SimulateEditor(float timeStep)
        {
            // Calculate sky and ground view for our simulated position
            groundViewFactor = VesselUtils.ComputeBodySolidAngle(RadioactivityPreferences.editorPlanetRadius, RadioactivityPreferences.editorFlightHeight) / (4d * Math.PI);
            skyViewFactor = 1.0d - groundViewFactor;

            if (RadioactivitySimulationSettings.SimulateCosmicRadiation)
            {
                cosmicFlux = simulator.CosmicSim.CalculateCosmicRadiationFlux(this);
                for (int i = 0; i < sinks.Count; i++)
                    sinks[i].AddAmbientRadiation("Cosmic", cosmicFlux);
            }
            if (RadioactivitySimulationSettings.SimulateLocalRadiation)
            {
                planetaryFlux = simulator.PlanetSim.CalculatePlanetaryRadiationFlux(this);
                for (int i = 0; i < sinks.Count; i++)
                    sinks[i].AddAmbientRadiation("Planetary", planetaryFlux);
            }
        }

        protected double CalculateSVFComplex()
        {
            Transform refXform = vessel.GetTransform();
            int hits = 0;
            for (int i = 1; i < RadioactivityConstants.groundSVFRaycastCount; i++)
            {
                if (Physics.Raycast(refXform.position, UnityEngine.Random.onUnitSphere, RadioactivityConstants.groundSVFRaycastDistance, raycastMask))
                {
                    hits++;
                }
            }
            return (1f - hits / (float)(RadioactivityConstants.groundSVFRaycastCount - 1));

        }

    }
}
