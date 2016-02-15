# Represents a basic radioactive sink
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Radioactivity
{

  public class RadioactiveSink:PartModule
  {
    [KSPField(isPersistant = true)]
    public string SinkID = "";
    [KSPField(isPersistant = true)]
    public string SinkTransformName = "";

    # Access the sink transform
    public Transform SinkTransform
    {
      get { return sinkTransform;}
      set { sinkTransform = value;}
    }

    private List<GenericRadiationAbsorber> associatedAbsorbers = new List<GenericRadiationAbsorber>();

    # Registers an abosrber module to read from this sink
    public void RegisterAbsorber(GenericRadiationAbsorber abs)
    {
      associatedAbsorbers.Add(abs);
    }
    public override void OnStart()
    {
      # Set up the sink transform, if it doesn't exist use the part root
      SinkTransform = part.FindTransformByName(SinkTransformName);
      if (SinkTransform == null)
      {
        SinkTransform = part.transform;
      }
      Radioactivity.Instance.RegisterSink(this);
    }

    public void OnDestroy()
    {
      
      Radioactivity.Instance.UnregisterSink(this);
    }
  }
}
