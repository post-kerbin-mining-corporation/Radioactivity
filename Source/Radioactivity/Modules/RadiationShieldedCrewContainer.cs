// Represents a crew container that blocks radiation, shielding the crew from it
// Also distributes radiation to the crew hehe
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Radioactivity.Interfaces;

namespace Radioactivity
{
  public class RadiationShieldedCrewContainer: PartModule, IRadiationAbsorber
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

    protected double prevRadiation = 0d;

    public string GetSinkName()
    {
        return AbsorberID;
    }

    // Adds radiation
    public void AddRadiation(float amt)
    {
      LifetimeRadiation = LifetimeRadiation + amt * (1f - RadiationAttenuationFraction);
    }

    public void FixedUpdate()
    {
      CurrentRadiationString = String.Format("{0:F2}/s", CurrentRadiation);
      LifetimeRadiationString = String.Format("{0:F2}/s", LifetimeRadiation);
      IrradiateCrew();
      CurrentRadiation = LifetimeRadiation - prevRadiation;
      prevRadiation = LifetimeRadiation;
    }
    // Distributes radiation to any crew
    protected void IrradiateCrew()
    {
      if (this.part.protoModuleCrew.Count > 0)
      {

      }
    }
  }
}
