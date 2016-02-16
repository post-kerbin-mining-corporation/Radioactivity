// Represents a module that absorbs radiation from a RadioactiveSink
// All radioactive absorbing modules derive from this
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Radioactivity
{

  public class GenericRadiationAbsorber:PartModule
  {
    // Associated RadioactiveSink to use for absorbtion
    [KSPField(isPersistant = true)]
    public string AbsorberID = "";

    protected currentEmission = 0f;
    protected RadioactiveSink radSink;

    // Adds radiation
    public virtual void AddRadiation(float amt)
    {

    }

    public override void OnStart()
    {
      // Locate the associated sink and register
      RadioactiveSink[] radSnks = this.GetComponents<RadioactiveSink>();
      foreach (RadioactiveSink radS in radSnks)
      {
        if (radS.SinkID == AbsorberID)
          radSink = radS;
          radSink.RegisterAbsorber(this);
      }
      if (radSrc == null)
        Debug.LogError("Could not find associated RadioactiveSink");
    }
  }
}
