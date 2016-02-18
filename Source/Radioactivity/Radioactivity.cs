using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Radioactivity
{

  // Loads settings and other data
  [KSPAddon(KSPAddon.Startup.MainMenu, false)]
  public class RadioactivityStartup:MonoBehaviour
  {
    public void Start()
    {
      Utils.LoadSettings();
    }
  }

  // Main class. Does simulations, holds data, holds the network
  [KSPAddon(KSPAddon.Startup.EveryScene, false)]
  public class Radioactivity:MonoBehaviour
  {
    public static Radioactivity Instance { get; private set; }

    public List<RadioactiveSource> AllSources
    { get { return allRadSources; } }

      public List<RadioactiveSink> AllSinks
    { get { return allRadSinks; } }

      public List<RadiationLink> AllLinks
      { get { return allLinks; } }

    List<RadioactiveSource> allRadSources = new List<RadioactiveSource>();
    List<RadioactiveSink> allRadSinks = new List<RadioactiveSink>();
    List<RadiationLink> allLinks = new List<RadiationLink>();

    // Add a radiation source to the source list
    public void RegisterSource(RadioactiveSource src)
    {
      allRadSources.Add(src);
      BuildNewRadiationLink(src);
      if (RadioactivitySettings.debugNetwork)
        Utils.Log("Adding radiation source "+ src.SourceID +" on part " + src.part.name + " to simulator");
    }

    // Remove a radiation source from the source list
    public void UnregisterSource(RadioactiveSource src)
    {
        allRadSources.Remove(src);
        RemoveRadiationLink(src);
       if (RadioactivitySettings.debugNetwork)
           Utils.Log("Removing radiation source "+ src.SourceID +" on part " + src.part.name + " from simulator");
    }
    // Add a radiation sink to the sink list
    public void RegisterSink(RadioactiveSink snk)
    {
      allRadSinks.Add(snk);
      BuildNewRadiationLink(snk);
      if (RadioactivitySettings.debugNetwork)
          Utils.Log("Adding radiation sink "+ snk.SinkID +" on part " + snk.part.name + " to simulator");
    }
    // Remove a radiation sink from the sink list
    public void UnregisterSink(RadioactiveSink snk)
    {
        allRadSinks.Remove(snk);
        RemoveRadiationLink(snk);
       if (RadioactivitySettings.debugNetwork)
           Utils.Log("Removing radiation sink "+ snk.SinkID +" on part " + snk.part.name + " from simulator");
    }
      // Show the ray overlay for ALL links
    public void ShowAllOverlays()
    {
        foreach (RadiationLink lnk in allLinks)
        {
            lnk.ShowOverlay();
        }
    }
    // Hide the ray overlay for ALL links
    public void HideAllOverlays()
    {
        foreach (RadiationLink lnk in allLinks)
        {
            lnk.HideOverlay();
        }
    }
    // Show the ray overlay for a given source or sink
    public void ShowOverlay(RadioactiveSource src)
    {
      foreach (RadiationLink lnk in allLinks)
      {
        if (lnk.source == src && !lnk.overlayShown)
        {
          lnk.ShowOverlay();
        }
      }
    }
    public void ShowOverlay(RadioactiveSink snk)
    {
        foreach (RadiationLink lnk in allLinks)
      {
          if (lnk.sink == snk && !lnk.overlayShown)
        {
          lnk.ShowOverlay();
        }
      }
    }
    //Hide the ray overlay for a given source or sink
    public void HideOverlay(RadioactiveSource src)
    {
        foreach (RadiationLink lnk in allLinks)
      {
        if (lnk.source == src)
        {
          lnk.HideOverlay();
        }
      }
    }
    public void HideOverlay(RadioactiveSink snk)
    {
        foreach (RadiationLink lnk in allLinks)
      {
        if (lnk.sink == snk)
        {
          lnk.HideOverlay();
        }
      }
    }

    protected void Awake()
    {
      Instance = this;
    }
    protected void Start()
    {

    }

    // builds the radiation network from scratch
    protected void BuildRadiationLinks()
    {
        foreach (RadioactiveSource s in allRadSources)
        {
            foreach (RadioactiveSink s2 in allRadSinks)
            {
                allLinks.Add(new RadiationLink(s, s2));
            }
        }
    }
    // Build new link for a new sink in the network
    protected void BuildNewRadiationLink(RadioactiveSink snk)
    {
        foreach (RadioactiveSource src in allRadSources)
        {
            allLinks.Add(new RadiationLink(src, snk));
        }

    }
    // Build new links for a new source in the network
    protected void BuildNewRadiationLink(RadioactiveSource src)
    {
        foreach (RadioactiveSink snk in allRadSinks)
        {
            allLinks.Add(new RadiationLink(src, snk));
        }
    }
    // Removes all links to a given radiation source
    protected void RemoveRadiationLink(RadioactiveSource src)
    {
        RadiationLink toRm = null;
        foreach (RadiationLink lnk in allLinks)
        {
            if (lnk.source == src)
            {
                toRm = lnk;
            }
        }
        if (toRm != null)
        {
            toRm.HideOverlay();
            allLinks.Remove(toRm);
        }
    }
    // Removes a link to a given radiation sink
    protected void RemoveRadiationLink(RadioactiveSink snk)
    {
        RadiationLink toRm = null;
        foreach (RadiationLink lnk in allLinks)
        {
            if (lnk.sink == snk)
            {
                toRm = lnk;
            }
        }
        if (toRm != null)
        {
            toRm.HideOverlay();
            allLinks.Remove(toRm);
        }
    }

    public void FixedUpdate()
    {
        if (HighLogic.LoadedSceneIsFlight || HighLogic.LoadedSceneIsEditor)
        {
            Simulate();
        }
    }

    // Master method that simulates radiation
    protected void Simulate()
    {
      // Simulate point radiation
      if (RadioactivitySettings.simulatePointRadiation)
        SimulatePointRadiation();

      // Simulate solar radiation
      if (RadioactivitySettings.simulateSolarRadiation)
          SimulateSolarRadiation();

      if (RadioactivitySettings.simulateCosmicRadiation)
          SimulateCosmicRadiation();
    }

    // simulate point radiation
    protected void SimulatePointRadiation()
    {
      foreach (RadiationLink link in allLinks)
        {
          // recompute pathways if needed
          link.Recompute();
          // Simulate the radiation based on precomputed pathways
          link.Simulate(TimeWarp.fixedDeltaTime);
        }

    }

    protected void SimulateSolarRadiation()
    {

    }

    protected void SimulateCosmicRadiation()
    {
    }


  }
}
