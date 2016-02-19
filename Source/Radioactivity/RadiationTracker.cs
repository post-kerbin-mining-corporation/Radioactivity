// A module that simply displays current and total radiation
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Radioactivity
{

  public class RadiationTracker: GenericRadiationAbsorber
  {
    [KSPField(isPersistant = false, guiActive = true, guiName = "Lifetime Dose")]
    public string LifetimeRadiationString;

    [KSPField(isPersistant = false, guiActive = true, guiName = "Dose Rate")]
    public string CurrentRadiationString;

    public override void FixedUpdate()
    {
      CurrentRadiationString = String.Format("{0:F2} /s", LifetimeRadiation-prevRadiation);
      LifetimeRadiationString = String.Format("{0:F2}", LifetimeRadiation);
    }
  }
}
