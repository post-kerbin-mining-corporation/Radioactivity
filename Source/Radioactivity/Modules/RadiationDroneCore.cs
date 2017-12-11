// A module that affects the SAS level of a probe
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Radioactivity.Interfaces;

namespace Radioactivity
{

    public class RadiationDroneCore : PartModule, IRadiationAbsorber
    {
        // Associated RadioactiveSink to use for absorbtion
        [KSPField(isPersistant = false)]
        public string AbsorberID = "";

        [KSPField(isPersistant = false)]
        public FloatCurve PenaltyCurve = new FloatCurve();

        [KSPField(isPersistant = false, guiActive = false, guiName = "Lifetime Dose")]
        public string LifetimeRadiationString;

        [KSPField(isPersistant = false, guiActive = false, guiName = "Dose Rate")]
        public string CurrentRadiationString;

        [KSPField(isPersistant = true)]
        public double LifetimeRadiation = 0d;

        [KSPField(isPersistant = true)]
        public double CurrentRadiation = 0d;

        // Alias for UI
        [KSPField(isPersistant = false)]
        public string UIName = "Drone Core";

        protected int baseSAS = 0;
        protected bool ready = false;
        protected double prevRadiation = 0d;
        protected ModuleSAS drone;
        protected ModuleCommand command;

        public string GetAlias()
        {
            return UIName;
        }
        public override string GetInfo()
        {
            string toRet = "Probe core is affected by radiation \n\n Penalties\n";
            toRet += String.Format("-{0}Sv/s: {1} SAS levels\n", FormatUtils.ToSI(0, "F2"), Mathf.Clamp(GetSASPenalty(0f), 0, 3));
            toRet += String.Format("-{0}Sv/s: -{1} SAS levels\n", FormatUtils.ToSI(1, "F2"), Mathf.Clamp(GetSASPenalty(1f), 0, 3));
            toRet += String.Format("-{0}Sv/s: -{1} SAS levels\n", FormatUtils.ToSI(5, "F2"), Mathf.Clamp(GetSASPenalty(5f), 0, 3));
            return toRet;
        }
        public Dictionary<string, string> GetDetails()
        {
            Dictionary<string, string> toReturn = new Dictionary<string, string>();
            toReturn.Add("<color=#ffffff><b>SAS degredation</b>:</color>", String.Format("{0} Levels", Mathf.Clamp(GetSASPenalty(), 0, 3)));
            return toReturn;
        }

        public int GetSASPenalty()
        {
            float fPenalty = PenaltyCurve.Evaluate((float)CurrentRadiation);
            return (int)Mathf.Round(fPenalty);
        }
        public int GetSASPenalty(float dose)
        {
            float fPenalty = PenaltyCurve.Evaluate(dose);
            return (int)Mathf.Round(fPenalty);
        }

        public string GetSinkName()
        {
            return AbsorberID;
        }

        public void AddRadiation(float amt)
        {
            LifetimeRadiation = LifetimeRadiation + amt;
        }

        public bool IsAbsorbing()
        {
            if (command != null)
            {
                return !command.hibernation;
            }
            return true;
        }

        public override void OnStart(PartModule.StartState state)
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                drone = this.GetComponent<ModuleSAS>();
                command = this.GetComponent<ModuleCommand>();
                if (drone != null)
                {
                    baseSAS = drone.SASServiceLevel;
                    ready = true;
                }
            }

        }

        public void FixedUpdate()
        {
            CurrentRadiation = LifetimeRadiation - prevRadiation;
            prevRadiation = LifetimeRadiation;

            CurrentRadiationString = String.Format("{0:F2} /s", LifetimeRadiation - prevRadiation);
            LifetimeRadiationString = String.Format("{0:F2}", LifetimeRadiation);

            if (HighLogic.LoadedSceneIsFlight && RadioactivityEffectSettings.EnableControlDegradation)
              ApplySASPenalty();
        }

        void ApplySASPenalty()
        {
            if (drone != null && ready)
            {
                int frameSAS = Mathf.Clamp(baseSAS - GetSASPenalty(), 0, 3);
                if (drone.SASServiceLevel != frameSAS)
                {
                    drone.SASServiceLevel = frameSAS;
                    if (RadioactivityConstants.debugModules)
                        LogUtils.Log(String.Format("[RadiationDroneCore]: Set SAS level to {0}", frameSAS));

                    drone.OnAwake();
                    drone.OnActive();
                    drone.OnStart(StartState.Flying);
                    if (this.vessel != null)
                    {
                        this.vessel.Autopilot.SAS.ModuleSetup();
                        this.vessel.Autopilot.SAS.Update();
                    }
                }

            }
        }
    }
}
