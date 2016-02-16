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

    [KSPField(isPersistant = true)]
    public double LifetimeRadiation = 0d;

    [KSPField(isPersistant = true)]
    public double CurrentRadiation = 0d;

    protected double prevRadiation = 0d;
    protected RadioactiveSink radSink;

    // Adds radiation
    public virtual void AddRadiation(float amt)
    {
      LifetimeRadiation = LifetimeRadiation + amt;
    }
    public override void OnFixedUpdate()
    {
      CurrentRadiation = LifetimeRadiation - prevRadiation;
      prevRadiation = LifetimeRadiation;
    }
    
    public override void  OnStart(PartModule.StartState state)
    {
      // Locate the associated sink and register
      RadioactiveSink[] radSnks = this.GetComponents<RadioactiveSink>();
      foreach (RadioactiveSink radS in radSnks)
      {
        if (radS.SinkID == AbsorberID)
          radSink = radS;
          radSink.RegisterAbsorber(this);
      }
      if (radSink == null)
        Debug.LogError("Could not find associated RadioactiveSink");
    }
  }
}
