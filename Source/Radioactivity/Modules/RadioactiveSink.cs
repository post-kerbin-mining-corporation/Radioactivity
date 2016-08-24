// All parts that receive radiation require a RadioactiveSink
// Handles hooking the part into the radiation simulation system
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Radioactivity.Interfaces;

namespace Radioactivity
{

  public class RadioactiveSink:PartModule
  {
    // The ID of the sink
    [KSPField(isPersistant = false)]
    public string SinkID;

    // The Transform to use for raycast calculations
    [KSPField(isPersistant = false)]
    public string SinkTransformName;

    [KSPField(isPersistant = false)]
    public bool ShowOverlay = false;

    [KSPField(isPersistant = false)]
    public int IconID = 0;

    [KSPEvent(guiActive = false, guiName = "Toggle Rays")]
    public void ToggleOverlay()
    {
      ShowOverlay = !ShowOverlay;
      if (ShowOverlay)
        Radioactivity.Instance.ShowOverlay(this);
      else
        Radioactivity.Instance.HideOverlay(this);
    }

    // Access the sink transform
    public Transform SinkTransform
    {
      get { return sinkTransform;}
      set { sinkTransform = value;}
    }
    public double CurrentRadiation
    {
      get {return currentRadiation;}
    }
    public string GetAbsorberAliases()
    {
      string aliases = "";
      for (int i = 0; i < associatedAbsorbers.Count; i++)
      {
          aliases += associatedAbsorbers[i].GetAlias();

          if (i + 1 < associatedAbsorbers.Count)
          {
              aliases += "\n";
          }
      }
      return aliases;
    }

    public Dictionary<string, string> GetAbsorberDetails()
    {
        Dictionary<string, string> toReturn = new Dictionary<string, string>();
        for (int i = 0; i < associatedAbsorbers.Count; i++)
        {
          toReturn = toReturn.Concat(associatedAbsorbers[i].GetDetails()).ToDictionary(x=>x.Key,x=>x.Value);
        }
        return toReturn;
    }
    public Dictionary<string,float> GetSourceDictionary()
    {
      return sourceDictionary;
    }

    private double currentRadiation;
    private Transform sinkTransform;
    private bool registered = false;
    private Dictionary<string,float> sourceDictionary = new Dictionary<string,float>();
    private List<IRadiationAbsorber> associatedAbsorbers = new List<IRadiationAbsorber>();

    // Add radiation to the sink
    public void AddRadiation(float amt)
    {
      AddRadiation("Null", amt);
    }
    public void AddRadiation(string src, float amt)
    {
      //currentRadiation += (double)amt;
      sourceDictionary[src] = amt;
      currentRadiation = (double)sourceDictionary.Sum(k => k.Value.Sum(v => v.Value));
    }
    void FixedUpdate()
    {
      if (associatedAbsorbers != null)
      {
        foreach (IRadiationAbsorber abs in associatedAbsorbers) {
          abs.AddRadiation((float)currentRadiation);
        }
      }
    }

    public override void OnStart(PartModule.StartState state)
    {
        // Set up the sink transform, if it doesn't exist use the part root
        if (SinkTransformName != String.Empty)
            SinkTransform = part.FindModelTransform(SinkTransformName);
        if (SinkTransform == null)
        {
            Utils.LogWarning("Couldn't find Source transform, using part root transform");
            SinkTransform = part.transform;
        }

        List<IRadiationAbsorber> absorbers = part.FindModulesImplementing<IRadiationAbsorber>();
        foreach (IRadiationAbsorber abs in absorbers)
        {
            if (abs.GetSinkName() == SinkID)
                associatedAbsorbers.Add(abs);
        }

        if (HighLogic.LoadedSceneIsFlight && !registered)
        {
            Radioactivity.Instance.RegisterSink(this);
            registered = true;
        }
    }

    public void OnDestroy()
    {
        if (registered)
        {
            Radioactivity.Instance.UnregisterSink(this);
            registered = false;
        }
        if (HighLogic.LoadedSceneIsFlight)
        {
            Radioactivity.Instance.UnregisterSink(this);
            registered = false;
        }
    }
  }
}
