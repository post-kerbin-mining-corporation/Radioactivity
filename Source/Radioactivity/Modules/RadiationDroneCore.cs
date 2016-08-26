// A module that affects the SAS level of a probe
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Radioactivity.Interfaces;

namespace Radioactivity
{

  public class RadiationDroneCore: PartModule, IRadiationAbsorber
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
    protected double prevRadiation = 0d;
    protected ModuleSAS drone;

    public string GetAlias()
    {
        return UIName;
    }
    public Dictionary<string, string> GetDetails()
    {
        Dictionary<string, string> toReturn = new Dictionary<string, string>();
        toReturn.Add("<color=#ffffff><b>SAS degredation</b>:</color>", String.Format("{0} Levels", GetSASPenalty() ));
        return toReturn;
    }

    public int GetSASPenalty()
    {
      float fPenalty = PenaltyCurve.Evaluate((float)CurrentRadiation);
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
    public override void OnStart(PartModule.StartState state)
    {
      if (HighLogic.LoadedSceneIsFlight)
      {
          drone = this.GetComponent<ModuleSAS>();
          if (drone != null)
            baseSAS = drone.SASServiceLevel;
      }

    }

    public void FixedUpdate()
    {
        CurrentRadiation = LifetimeRadiation - prevRadiation;
        prevRadiation = LifetimeRadiation;

      CurrentRadiationString = String.Format("{0:F2} /s", LifetimeRadiation-prevRadiation);
      LifetimeRadiationString = String.Format("{0:F2}", LifetimeRadiation);

      if (HighLogic.LoadedSceneIsFlight && RadioactivitySettings.enableProbeEffects)
        ApplySASPenalty();
    }

    void ApplySASPenalty()
    {
      if (drone != null)
        drone.SASServiceLevel = Mathf.Clamp(baseSAS - GetSASPenalty(), 0, 5);
    }
  }
}
