// Represents a container full of radioactive resources
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Radioactivity.Interfaces;

namespace Radioactivity
{

    public class RadioactiveResource : PartModule, IRadiationEmitter
    {
        // RadioactiveSource to use for emission
        [KSPField(isPersistant = true)]
        public string SourceID = "";

        // The resource that will emit the radiation
        [KSPField(isPersistant = true)]
        public string ResourceName = "";

        // Amount of emission
        [KSPField(isPersistant = true)]
        public float EmissionPerUnit = 1f;

        // Alias for UI
        [KSPField(isPersistant = false)]
        public string UIName = "Nuclear Materials";

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
        public Dictionary<string, string> GetDetails()
        {
            Dictionary<string, string> toReturn = new Dictionary<string, string>();
            toReturn.Add("Nuclear Materials", String.Format("{0}Sv/s", Utils.ToSI(currentEmission, "F2")));
            return toReturn;
        }
        public override string GetInfo()
        {
            string toRet = String.Format("{0} emits {1}Sv/s per unit of radiation", ResourceName, Utils.ToSI(EmissionPerUnit, "F2"));

            return toRet;
        }
        float currentEmission = 0f;
        bool emitting = true;

        public void Start()
        {

        }

        public void FixedUpdate()
        {

            if (emitting)
            {
                // Get amount of resource present, multiply per unit, emit
                if (HighLogic.LoadedSceneIsEditor || HighLogic.LoadedSceneIsFlight)
                {
                    double resourceAmount = GetResourceAmount(ResourceName);
                    currentEmission = (float)resourceAmount * EmissionPerUnit;
                }


            }
        }
        public double GetResourceAmount(string nm)
        {
            return this.part.Resources.Get(PartResourceLibrary.Instance.GetDefinition(nm).id).amount;
        }
    }
}
