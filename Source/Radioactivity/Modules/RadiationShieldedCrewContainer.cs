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

    public string GetAlias()
    {
        return "Crew";
    }
    public string GetDetails()
    {
        return String.Format("<color=#ffffff><b>Shielding</b>:</color> {0}% \n " +
           "<color=#ffffff><b>Dose to Crew</b>:</color> {1}Sv ", (RadiationAttenuationFraction * 100f).ToString().PadLeft(15), Utils.ToSI(CurrentRadiation,"F2").PadLeft(15));
        }
    public string GetSinkName()
    {
        return AbsorberID;
    }

    // Adds radiation
    public void AddRadiation(float amt)
    {
      LifetimeRadiation = LifetimeRadiation + amt * (1f - RadiationAttenuationFraction);
      IrradiateCrew(amt);
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
          for (int i = 0; i < this.part.protoModuleCrew.Count ;i++)
          {
            KerbalTracking.Instance.IrradiateKerbal(this.part.protoModuleCrew[i], (double)amt);
          }
      }
    }
  }
}
