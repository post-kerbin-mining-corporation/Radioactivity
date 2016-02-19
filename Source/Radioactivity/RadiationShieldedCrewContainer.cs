// Represents a crew container that blocks radiation, shielding the crew from it
// Also distributes radiation to the crew hehe
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Radioactivity
{
  public class RadiationShieldedCrewContainer: GenericRadiationAbsorber
  {
    // Amount to reduce incoming radiation
    public float RadiationAttenuationFraction = 0f;

    [KSPField(isPersistant = false, guiActive = true, guiName = "Lifetime Dose")]
    public string LifetimeRadiationString;

    [KSPField(isPersistant = false, guiActive = true, guiName = "Dose Rate")]
    public string CurrentRadiationString;

    // Adds radiation
    public override void AddRadiation(float amt)
    {
      LifetimeRadiation = LifetimeRadiation + amt * (1f - RadiationAttenuationFraction);
    }

    public override void FixedUpdate()
    {
      CurrentRadiationString = String.Format("{0:F2}/s", CurrentRadiation);
      LifetimeRadiationString = String.Format("{0:F2}/s", LifetimeRadiation);
      IrradiateCrew();
      base.FixedUpdate();


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
