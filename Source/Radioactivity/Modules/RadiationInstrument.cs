// A module that affects the science return from an instrument
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Radioactivity.Interfaces;

namespace Radioactivity
{

  public class RadiationInstrument: PartModule, IRadiationAbsorber
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
    public string UIName = "Science Instrument";

    protected float baseValue;
    protected double prevRadiation = 0d;
    protected ModuleScienceExperiment experiment;

    public string GetAlias()
    {
        return UIName;
    }
    public override string GetInfo()
    {
        string toRet = "Science instrument return is affected by radiation \n\n <b>Penalties:</b>\n";      

        return toRet;
    }
    public Dictionary<string, string> GetDetails()
    {
        Dictionary<string, string> toReturn = new Dictionary<string, string>();
        toReturn.Add("<color=#ffffff><b>Science Penalty</b>:</color>", String.Format("{0:F0}%", (1f-PenaltyCurve.Evaluate((float)CurrentRadiation))*100f ));
        return toReturn;
    }



    public string GetSinkName()
    {
        return AbsorberID;
    }

    public void AddRadiation(float amt)
    {
        LifetimeRadiation = LifetimeRadiation + amt;
    }
    public override void OnStart(PartModule.StartState state)
    {
      experiment = this.GetComponent<ModuleScienceExperiment>();
      if (experiment != null)
      {
        baseValue = experiment.experiment.baseValue;
      }
    }
    public void FixedUpdate()
    {
        CurrentRadiation = LifetimeRadiation - prevRadiation;
        prevRadiation = LifetimeRadiation;

       CurrentRadiationString = String.Format("{0:F2} /s", LifetimeRadiation-prevRadiation);
       LifetimeRadiationString = String.Format("{0:F2}", LifetimeRadiation);
       if (HighLogic.LoadedSceneIsFlight && RadioactivitySettings.enableScienceEffects && experiment != null)
       {
         experiment.experiment.baseValue = baseValue * PenaltyCurve.Evaluate((float)CurrentRadiation);
       }
    }
  }
}
