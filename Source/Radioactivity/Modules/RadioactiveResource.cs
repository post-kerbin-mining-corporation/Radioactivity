// Represents a container full of radioactive resources
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Radioactivity.Interfaces;

namespace Radioactivity
{

  public class RadioactiveResource : PartModule, IRadiationEmitter
  {
      // RadioactiveSource to use for emission
      [KSPField(isPersistant = true)]
      public string SourceID = "";

      // The resource that will emit the radiation
      [KSPField(isPersistant = true)]
      public string ResourceName = "";

      // Amount of emission
      [KSPField(isPersistant = true)]
      public float EmissionPerUnit = 1f;

      // Alias for UI
      [KSPField(isPersistant = false)]
      public string UIName = "Nuclear Materials";

      // Interface
      public bool IsEmitting()
      {
          return true;
      }
      public float GetEmission()
      {
          return currentEmission;
      }
      public string GetSourceName()
      {
          return SourceID;
      }
      public string GetAlias()
      {
          return UIName;
      }
      public Dictionary<string, string> GetDetails()
      {
          Dictionary<string, string> toReturn = new Dictionary<string, string>();
          toReturn.Add("Test","01");
          return toReturn;
      }

      float currentEmission = 0f;
      bool emitting = true;

      public override void OnStart(PartModule.StartState state)
      {
        base.OnStart(state);
      }

      public override void OnFixedUpdate()
      {
        base.OnFixedUpdate();
        if (emitting)
        {
          // Get amount of resource present, multiply per unit, emit
          // TODO: Actually do this
          double curAmount = 10f;
          currentEmission = (float)curAmount * EmissionPerUnit;
        }
      }
  }
}
