// All parts that receive radiation require a RadioactiveSink
// Handles hooking the part into the radiation simulation system
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Radioactivity.Interfaces;
using Radioactivity.Simulator;

namespace Radioactivity
{

    public class RadioactiveSink : PartModule
    {
        // The ID of the sink
        [KSPField(isPersistant = false)]
        public string SinkID;

        // The Transform to use for raycast calculations
        [KSPField(isPersistant = false)]
        public string SinkTransformName;

        [KSPField(isPersistant = false)]
        public bool ShowOverlay = false;

        [KSPField(isPersistant = false)]
        public string IconID = "kerbal";

        // Ambient radiation field parameters
        [KSPField(isPersistant = true)]
        public double GroundFractionVisible = 0d;

        [KSPField(isPersistant = true)]
        public double SkyFractionVisible = 0d;

        // Access the sink transform
        public Transform SinkTransform
        {
            get { return sinkTransform; }
            set { sinkTransform = value; }
        }
        public double CurrentRadiation
        {
            get { return currentRadiation; }
        }
        public bool SinkEnabled
        {
            get { return sinkEnabled; }
        }


        public string GetAbsorberAliases()
        {
            string aliases = "";
            for (int i = 0; i < associatedAbsorbers.Count; i++)
            {
                aliases += associatedAbsorbers[i].GetAlias();

                if (i + 1 < associatedAbsorbers.Count)
                {
                    aliases += "\n";
                }
            }
            return aliases;
        }

        public Dictionary<string, string> GetAbsorberDetails()
        {
            Dictionary<string, string> toReturn = new Dictionary<string, string>();
            for (int i = 0; i < associatedAbsorbers.Count; i++)
            {
                toReturn = toReturn.Concat(associatedAbsorbers[i].GetDetails()).ToDictionary(x => x.Key, x => x.Value);
            }

            return toReturn;
        }
        public Dictionary<string, float> GetSourceDictionary()
        {
            return sourceDictionary;
        }

        private double currentRadiation;

        private Transform sinkTransform;
        private bool registered = false;
        private bool sinkEnabled = false;
        private Dictionary<string, float> sourceDictionary = new Dictionary<string, float>();
        private List<IRadiationAbsorber> associatedAbsorbers = new List<IRadiationAbsorber>();

        // Add radiation to the sink
        public void AddRadiation(float amt)
        {
            AddRadiation("Null", amt);
        }

        public void AddRadiation(string src, float amt)
        {
            sourceDictionary[src] = amt;
            currentRadiation = (double)sourceDictionary.Sum(k => k.Value);
        }
        public void CleanupRadiation(string src)
        {
            sourceDictionary.Remove(src);
            currentRadiation = (double)sourceDictionary.Sum(k => k.Value);
        }

        void FixedUpdate()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (SinkEnabled)
                    DoRegistration();
                else
                    DoDeregistration();
            }
            if (HighLogic.LoadedSceneIsEditor)
            {
                DoRegistration();
            }

            if (associatedAbsorbers != null)
            {
                bool allDisabled = false;
                foreach (IRadiationAbsorber abs in associatedAbsorbers)
                {
                    allDisabled = !abs.IsAbsorbing();
                    if (!allDisabled)
                    {
                        abs.AddRadiation((float)currentRadiation);
                    }
                }
                if (allDisabled)
                    sinkEnabled = false;
                else
                    sinkEnabled = true;
            }
        }

        public override void OnStart(PartModule.StartState state)
        {
            // Set up the sink transform, if it doesn't exist use the part root
            if (SinkTransformName != String.Empty)
                SinkTransform = part.FindModelTransform(SinkTransformName);
            if (SinkTransform == null)
            {
                if (RadioactivityConstants.debugSourceSinks)
                    LogUtils.LogWarning("[RadioactiveSink]: Couldn't find Source transform, using part root transform");
                SinkTransform = part.transform;
            }

            List<IRadiationAbsorber> absorbers = part.FindModulesImplementing<IRadiationAbsorber>();
            foreach (IRadiationAbsorber abs in absorbers)
            {
                if (abs.GetSinkName() == SinkID)
                    associatedAbsorbers.Add(abs);
            }


            if (SinkEnabled)
                DoRegistration();

        }

        protected void DoRegistration()
        {
            if (!registered)
            {
                Radioactivity.Instance.RadSim.RegisterSink(this);
                registered = true;
            }
        }
        protected void DoDeregistration()
        {
            if (registered)
            {
                Radioactivity.Instance.RadSim.UnregisterSink(this);
                registered = false;
            }
        }

        public void OnDestroy()
        {

            if (registered)
            {
                if (Radioactivity.Instance.RadSim != null)
                {
                    Radioactivity.Instance.RadSim.UnregisterSink(this);
                    registered = false;
                }
            }
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (Radioactivity.Instance.RadSim != null)
                {
                    Radioactivity.Instance.RadSim.UnregisterSink(this);
                    registered = false;
                }
            }
        }
    }
}
