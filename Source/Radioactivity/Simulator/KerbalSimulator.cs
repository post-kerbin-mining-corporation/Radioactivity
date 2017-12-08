using System;
using System.Collections;
using System.Collections.Generic;

using Radioactivity.Persistance;

namespace Radioactivity.Simulator
{
    public class KerbalSimulator
    {
        KerbalDatabase KerbalDB;

        public KerbalSimulator()
        {
            KerbalDB = RadioactivityPersistance.Instance.KerbalDB;
        }


        public void Simulate(float time)
        {
            if (KerbalDB.Ready)
            {
                foreach (KeyValuePair<string, RadioactivityKerbal> kerbal in KerbalDB.Kerbals)
                {
                    SimulateKerbal(time, kerbal.Value);
                }
            }
        }
        // Irradiates a kerbal
        public void SetIrradiation(ProtoCrewMember crew, Vessel crewVessel, double pointAmount)
        {
            foreach (KeyValuePair<string, RadioactivityKerbal> kerbal in KerbalDB.Kerbals)
            {
                if (crew == kerbal.Value.Kerbal)
                {
                    kerbal.Value.CurrentExposure = pointAmount;
                }
            }
        }


        /// <summary>
        /// Does the simulation for a particular kerbal
        /// </summary>
        /// <param name="time">Time.</param>
        /// <param name="kerbal">Kerbal.</param>
        private void SimulateKerbal(float timeStep, RadioactivityKerbal kerbal)
        {
            if (kerbal.Kerbal.rosterStatus == ProtoCrewMember.RosterStatus.Dead ||
                kerbal.Kerbal.rosterStatus == ProtoCrewMember.RosterStatus.Missing)
            {
                SimulateDead(timeStep, kerbal);
            }
            else if (kerbal.Kerbal.rosterStatus == ProtoCrewMember.RosterStatus.Available)
            {
                SimulateKSC(timeStep, kerbal);
            }
            else
            {
                SimulateOnMission(timeStep, kerbal);
            }
            HandleExposure(kerbal);
        }


        // Simulates effects when the kerbal is dead or missing
        void SimulateDead(float timeStep, RadioactivityKerbal kerbal)
        {
            kerbal.CurrentVessel = null;
            KerbalDB.RemoveKerbal(kerbal);
        }

        // Simulates effects when the kerbal is at the KSC
        void SimulateKSC(float timeStep, RadioactivityKerbal kerbal)
        {
            kerbal.CurrentVessel = null;
            //HealthState = RadioactivityKerbalState.Home;
            kerbal.TotalExposure = kerbal.TotalExposure - RadioactivityConstants.kerbalHealRateKSC * timeStep;
            if (kerbal.TotalExposure < 0d)
                kerbal.TotalExposure = 0d;
        }
        // Simulates effects when the
        void SimulateOnMission(float timeStep, RadioactivityKerbal kerbal)
        {
            if ((kerbal.CurrentExposure) <= RadioactivityConstants.kerbalHealThreshold)
            {
                kerbal.TotalExposure = kerbal.TotalExposure - RadioactivityConstants.kerbalHealRate * timeStep;
                if (kerbal.TotalExposure < 0d)
                    kerbal.TotalExposure = 0d;
            }
            else
            {
                kerbal.TotalExposure = kerbal.TotalExposure + (kerbal.CurrentExposure  * timeStep);
            }
        }

        void HandleExposure(RadioactivityKerbal kerbal)
        {
            if (kerbal.TotalExposure >= RadioactivityConstants.kerbalSicknessThreshold)
            {
                if (kerbal.HealthState != RadioactivityKerbalState.Sick && kerbal.HealthState != RadioactivityKerbalState.Dead)
                {
                    Sicken(kerbal);
                    return;
                }
                Utils.LogWarning("[KerbalSimulator]:" + kerbal.Name + " died of radiation exposure");
            }
            if (kerbal.TotalExposure >= RadioactivityConstants.kerbalDeathThreshold)
            {
                if (kerbal.HealthState != RadioactivityKerbalState.Dead)
                {
                    Die(kerbal);
                    return;
                }
                //Utils.LogWarning(Name + " got radiation sickness");
            }
            if (kerbal.TotalExposure < RadioactivityConstants.kerbalSicknessThreshold)
            {
                if (kerbal.HealthState == RadioactivityKerbalState.Sick)
                {
                    Heal(kerbal);
                }
                //Utils.LogWarning(Name + " got radiation sickness");
            }
        }
        void Sicken(RadioactivityKerbal kerbal)
        {
            kerbal.HealthState = RadioactivityKerbalState.Sick;
            ScreenMessages.PostScreenMessage(new ScreenMessage(String.Format("{0} now has radiation sickness", kerbal.Name), 4.0f, ScreenMessageStyle.UPPER_CENTER));
            if (RadioactivityConstants.debugKerbalEvents)
                Utils.LogWarning(String.Format("[KerbalSimulator]: {0} got radiation sickness", kerbal.Name));
        }
        void Heal(RadioactivityKerbal kerbal)
        {
            kerbal.HealthState = RadioactivityKerbalState.Healthy;
            ScreenMessages.PostScreenMessage(new ScreenMessage(String.Format("{0} recovered from radiation sickness", kerbal.Name), 4.0f, ScreenMessageStyle.UPPER_CENTER));
            if (RadioactivityConstants.debugKerbalEvents)
                Utils.LogWarning(String.Format("[KerbalSimulator]: {0} recovered from radiation sickness", kerbal.Name));
        }

        void Die(RadioactivityKerbal kerbal)
        {

            kerbal.HealthState = RadioactivityKerbalState.Dead;
            KerbalDB.RemoveKerbal(kerbal);
            ScreenMessages.PostScreenMessage(new ScreenMessage(String.Format("{0} has died of radiation exposure", kerbal.Name), 4.0f, ScreenMessageStyle.UPPER_CENTER));

            if (kerbal.CurrentVessel != null && kerbal.CurrentVessel.isEVA)
            {
                // If we are EVA, have to handle this more carefully
                kerbal.CurrentVessel.rootPart.Die();
                if (RadioactivityEffectSettings.EnableKerbalDeath)
                {
                    // Do nothing
                }
                else
                {
                    if (HighLogic.CurrentGame.Parameters.Difficulty.MissingCrewsRespawn)
                    {
                        kerbal.Kerbal.StartRespawnPeriod(2160000.0); // 100 Kerbin days
                    }
                }
                if (RadioactivityConstants.debugKerbalEvents)
                    Utils.LogWarning(String.Format("[KerbalSimulator]: {0} died on EVA of radiation exposure", kerbal.Name));
            }
            else
            {
                if (kerbal.Kerbal.rosterStatus == ProtoCrewMember.RosterStatus.Available)
                {
                    kerbal.Kerbal.Die();
                    if (HighLogic.CurrentGame.Parameters.Difficulty.MissingCrewsRespawn)
                    {
                        kerbal.Kerbal.StartRespawnPeriod(2160000.0); // 100 Kerbin days
                    }
                    if (RadioactivityConstants.debugKerbalEvents)
                        Utils.LogWarning(String.Format("[KerbalSimulator]: {0} died at home of radiation exposure", kerbal.Name));
                }
                else
                {

                    Part part = kerbal.CurrentVessel.Parts.Find(p => p.protoModuleCrew.Contains(kerbal.Kerbal));
                    if (part != null)
                    {
                        part.RemoveCrewmember(kerbal.Kerbal);
                        kerbal.Kerbal.Die();
                        if (HighLogic.CurrentGame.Parameters.Difficulty.MissingCrewsRespawn)
                        {
                            kerbal.Kerbal.StartRespawnPeriod(2160000.0); // 100 Kerbin days
                        }
                        if (RadioactivityConstants.debugKerbalEvents)
                            Utils.LogWarning(String.Format("[KerbalSimulator]: {0} died in his vessel of radiation exposure", kerbal.Name));
                        //HighLogic.CurrentGame.CrewRoster.RemoveDead(Kerbal);
                    }
                }
            }
        }
    }
}
