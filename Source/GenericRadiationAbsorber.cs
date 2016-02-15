# All radioactive sources derive from this
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Radioactivity
{

  public class GenericRadiationAbsorber:PartModule
  {
    # Associated RadioactiveSink to use for absorbtion
    [KSPField(isPersistant = true)]
    public string AbsorberID = "";

    protected currentEmission = 0f;
    protected RadioactiveSink radSink;

    public override void OnStart()
    {
      RadioactiveSink[] radSnks = this.GetComponents<RadioactiveSink>();
      foreach (RadioactiveSink radS in radSnks)
      {
        if (radS.SinkID == AbsorberID)
          radSink = radS;
          radSink.RegisterAbsorber(this);
      }
      if (radSrc == null)
        Debug.Log("Error, could not find associated RadioactiveSource");
    }
  }
}
