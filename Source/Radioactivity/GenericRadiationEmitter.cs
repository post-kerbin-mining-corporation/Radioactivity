// Represents a module that creates radiation output
// All radioactive sources derive from this
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Radioactivity
{

  public class GenericRadiationEmitter:PartModule
  {
    // Associated RadioactiveSource from which to emit
    [KSPField(isPersistant = true)]
    public string SourceID = "";

    // Is the source emitting?
    [KSPField(isPersistant = true)]
    public bool Emitting = true;

    // Get the current emission
    public float CurrentEmission {
      get {return currentEmission;}
      set {currentEmission = value;}
    }

    protected float currentEmission = 0f;
    protected RadioactiveSource radSrc;

    public override void OnStart(PartModule.StartState state)
    {
      // Locate the appropriate radioactive source and register
      RadioactiveSource[] radSrcs = this.GetComponents<RadioactiveSource>();
      foreach (RadioactiveSource radS in radSrcs)
      {
        if (radS.SourceID == SourceID)
          radSrc = radS;
          radSrc.RegisterEmitter(this);
      }
      if (radSrc == null)
        Debug.LogError("Could not find associated RadioactiveSource");
    }


  }
}
