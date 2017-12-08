// Represents a crew container that blocks radiation, shielding the crew from it
// Also distributes radiation to the crew hehe
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Radioactivity.Interfaces;
using Radioactivity.Persistance;

namespace Radioactivity
{
    public class RadiationShieldedCrewContainer : PartModule, IRadiationAbsorber
    {
        // Associated RadioactiveSink to use for absorbtion
        [KSPField(isPersistant = false)]
        public string AbsorberID = "";

        // Amount to reduce incoming radiation
        [KSPField(isPersistant = false)]
        public float RadiationAttenuationFraction = 0f;

        [KSPField(isPersistant = false, guiActive = true, guiName = "Lifetime Dose")]
        public string LifetimeRadiationString;

        [KSPField(isPersistant = false, guiActive = true, guiName = "Dose Rate")]
        public string CurrentRadiationString;

        [KSPField(isPersistant = true)]
        public double LifetimeRadiation = 0d;

        [KSPField(isPersistant = true)]
        public double CurrentRadiation = 0d;

        // Alias for UI
        [KSPField(isPersistant = false)]
        public string UIName = "Crew Container";

        protected double prevRadiation = 0d;

        public string GetAlias()
        {
            return UIName;
        }
        public override string GetInfo()
        {
            string toRet = String.Format("Shielded container protects crew from radiation \n\n <b>Shielding:</b> {0:F1}%", RadiationAttenuationFraction * 100f);
            return toRet;
        }
        public Dictionary<string, string> GetDetails()
        {
            Dictionary<string, string> toReturn = new Dictionary<string, string>();
            toReturn.Add("<color=#ffffff><b>Crew Shielding</b>:</color>", String.Format("{0}%", RadiationAttenuationFraction * 100f));
            toReturn.Add("<color=#ffffff><b>Crew Dose</b>:</color>", String.Format("{0}Sv/s", Utils.ToSI(CurrentRadiation, "F2")));
            return toReturn;
        }
        public string GetSinkName()
        {
            return AbsorberID;
        }
        public bool IsAbsorbing()
        {
            if (this.part.protoModuleCrew.Count > 0)
            {
                return true;
            } else
            {
                return false;
            }
        }
        // Adds radiation
        public void AddRadiation(float amt)
        {
            LifetimeRadiation = LifetimeRadiation + amt * (1f - RadiationAttenuationFraction);

            if (HighLogic.LoadedSceneIsFlight)
                IrradiateCrew(amt * (1f - RadiationAttenuationFraction));
        }

        public void FixedUpdate()
        {
            CurrentRadiationString = Utils.ToSI(CurrentRadiation, "F2") + "Sv/s";
            LifetimeRadiationString = Utils.ToSI(LifetimeRadiation, "F2") + "Sv";

            CurrentRadiation = LifetimeRadiation - prevRadiation;
            prevRadiation = LifetimeRadiation;
        }
        // Distributes radiation to any crew
        // TODO: scale this by kerbal surface area and mass (assume a spherical kerbal)
        protected void IrradiateCrew(float amt)
        {
            if (this.part.protoModuleCrew.Count > 0)
            {
                for (int i = 0; i < this.part.protoModuleCrew.Count; i++)
                {
                    Radioactivity.Instance.RadSim.KerbalSim.SetIrradiation(this.part.protoModuleCrew[i], part.vessel, (double)amt);
                }
            }
        }
    }
}
