// Represents a container full of radioactive resources
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Radioactivity
{

  public class RadioactiveResource:GenericRadiationEmitter
  {
      // The resource that will emit the radiation
      [KSPField(isPersistant = true)]
      public string ResourceName = "";

      // Amount of emission per unit in MBq
      // eg 1 kg nuclear waste = 10 TBq
      // 1 kg uranium = 25 MBq
      [KSPField(isPersistant = true)]
      public float EmissionPerUnit = 1f;

      public override void OnStart()
      {
        base.OnStart();
      }

      public override void OnFixedUpdate()
      {
        base.OnFixedUpdate();
        if (Emitting)
        {
          // Get amount of resource present, multiply per unit, emit
          // TODO: Actually do this
          double curAmount = 10f;
          CurrentEmission = curAmount * EmissionPerUnit;
        }
      }
  }
}
