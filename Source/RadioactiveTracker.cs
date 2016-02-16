// Represents a module that absorbs radiation from a RadioactiveSink
// All radioactive absorbing modules derive from this
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Radioactivity
{

  public class RadioactiveTracker: GenericRadiationAbsorber
  {
    [KSPField(isPersistant = true)]
    public double LifetimeRadiation = 0d;

    [KSPField(isPersistant = true)]
    public double CurrentRadiation = 0d;

    [KSPField(isPersistant = false, guiActive = true, guiName = "Lifetime Dose")]
    public string LifetimeRadiationString;

    [KSPField(isPersistant = false, guiActive = true, guiName = "Dose Rate")]
    public string CurrentRadiationString;

    protected double prevRadiation = 0d;

    // Adds radiation
    public override void AddRadiation(float amt)
    {
      LifetimeRadiation = LifetimeRadiation + amt;
    //  CurrentRadiation = amt;
    }

    public override void FixedUpdate()
    {
      CurrentRadiationString = String.Format("{0:F2}/s", LifetimeRadiation-prevRadiation);
      LifetimeRadiationString = String.Format("{0:F2}/s", LifetimeRadiation);
      prevRadiation = LifetimeRadiation;
    }
  }
}
