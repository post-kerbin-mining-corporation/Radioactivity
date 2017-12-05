// A module that simply displays current and total radiation
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Radioactivity.Interfaces;

namespace Radioactivity
{

    public class RadiationTracker : PartModule, IRadiationAbsorber
    {
        // Associated RadioactiveSink to use for absorbtion
        [KSPField(isPersistant = false)]
        public string AbsorberID = "";

        [KSPField(isPersistant = false, guiActive = true, guiName = "Lifetime Dose")]
        public string LifetimeRadiationString;

        [KSPField(isPersistant = false, guiActive = true, guiName = "Dose Rate")]
        public string CurrentRadiationString;

        [KSPField(isPersistant = true)]
        public double LifetimeRadiation = 0d;

        [KSPField(isPersistant = true)]
        public double CurrentRadiation = 0d;

        // Show or hide the radioactive overlay from this source
        [KSPEvent(guiActive = true, guiName = "Reset Counter")]
        public void Reset()
        {
            LifetimeRadiation = 0d;
        }

        // Alias for UI
        [KSPField(isPersistant = false)]
        public string UIName = "Radiation Tracker";

        protected double prevRadiation = 0d;

        public string GetAlias()
        {
            return UIName;
        }

        public Dictionary<string, string> GetDetails()
        {
            Dictionary<string, string> toReturn = new Dictionary<string, string>();
            toReturn.Add("<color=#ffffff><b>Lifetime Dose</b>:</color>", String.Format("{0}Sv", Utils.ToSI(LifetimeRadiation, "F2")));
            return toReturn;
        }



        public string GetSinkName()
        {
            return AbsorberID;
        }

        public void AddRadiation(float amt)
        {
            CurrentRadiation = amt;
            if (HighLogic.LoadedSceneIsFlight)
                LifetimeRadiation = LifetimeRadiation + amt;
        }

        public void FixedUpdate()
        {
            CurrentRadiationString = String.Format("{0}Sv/s", Utils.ToSI(CurrentRadiation, "F2"));
            LifetimeRadiationString = String.Format("{0} Sv", Utils.ToSI(LifetimeRadiation, "F2"));
        }
    }
}
