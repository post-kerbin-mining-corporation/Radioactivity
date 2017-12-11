// Represents a fissionReactor managed by NearFutureElectrical
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Radioactivity.Interfaces;

namespace Radioactivity
{

    public class RadioactiveReactor : PartModule, IRadiationEmitter
    {
        // RadioactiveSource to use for emission
        [KSPField(isPersistant = true)]
        public string SourceID = "";

        // Amount of emission at full reactor throttle
        [KSPField(isPersistant = false)]
        public float EmissionAtMax = 1f;
        // Amount of emission when meltdown is total
        [KSPField(isPersistant = false)]
        public float MeltdownEmission = 1f;

        // Alias for UI
        [KSPField(isPersistant = false)]
        public string UIName = "Fission Reactor";

        // Interface
        public bool IsEmitting()
        {
            return true;
        }
        public float GetEmission()
        {
            return currentEmission;
        }
        public string GetSourceName()
        {
            return SourceID;
        }
        public string GetAlias()
        {
            return UIName;
        }
        public override string GetInfo()
        {
            string toRet = String.Format("Emits radiation when running \n\n <b>Maximum emission:</b> {0}Sv/s\n", FormatUtils.ToSI(EmissionAtMax, "F2"));
            toRet += String.Format("<b>Meltdown emission:</b> {0}Sv/s", FormatUtils.ToSI(MeltdownEmission, "F2"));

            return toRet;
        }
        public Dictionary<string, string> GetDetails()
        {
            Dictionary<string, string> toReturn = new Dictionary<string, string>();
            toReturn.Add("<color=#ffffff><b>Reactor Emission</b>:</color>", String.Format("{0}Sv/s", FormatUtils.ToSI(currentEmission, "F2")));

            return toReturn;
        }

        float currentEmission = 0f;
        BaseFieldList partFields;

        public void Start()
        {
            if (part.Modules.Contains("FissionReactor"))
            {
                foreach (PartModule pm in part.Modules) //should be a shorter way to do this, but a foreach cycle works
                {
                    if (pm.moduleName == "FissionReactor") //find the desired partmodule, note we stay as type PartModule, we do not cast to DifferentialThrustEngineModule
                    {
                        partFields = pm.Fields;
                        //  rotation = (float)pm.Fields.GetValue("rotation"); //which throttle is engine assigned to?
                    }
                }
            }

        }

        public void FixedUpdate()
        {
            if (partFields != null)
            {
                float reactorThrottle = (float)(partFields.GetValue("CurrentPowerPercent")) * 0.01f;
                float reactorIntegrity = 1f - (float)(partFields.GetValue("CoreIntegrity")) * 0.01f;
                if (HighLogic.LoadedSceneIsFlight)
                {
                    currentEmission = reactorThrottle * EmissionAtMax + reactorIntegrity * MeltdownEmission;
                }
                else
                {
                    currentEmission = EmissionAtMax;
                }
            }

        }
    }
}
