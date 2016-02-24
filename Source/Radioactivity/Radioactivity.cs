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

      private List<Part> evaParts;
      private bool evaModified = false;
      public void Awake()
      {
          evaParts = new List<Part>();
      }
    public void Start()
    {
      Utils.LoadSettings();
      
    }

    public void Update()
    {
        if (!PartLoader.Instance.IsReady() || PartResourceLibrary.Instance == null)
        {
            return;
        }
        if (evaParts.Count < 2)
        {
            Utils.Log("EVA: Finding EVA parts");
            FindEVAParts();
        }
        else if (evaParts.Count == 2 && !evaModified)
        {
            AddEVAModules();
        }

    }
      // Concept for this function came from Toadius' EVAManager
     protected void FindEVAParts()
     {
         AvailablePart loadedPart;
	    for (int i = 0; i < PartLoader.LoadedPartsList.Count; i++)
	    {
		    loadedPart = PartLoader.LoadedPartsList[i];
		    string lowerName = loadedPart.name.ToLower();

		    if (lowerName == "kerbaleva" || lowerName == "kerbalevafemale")
		    {
			    evaParts.Add(loadedPart.partPrefab);
			    if (this.evaParts.Count == 2)
			    {
				    break;
			    }
		    }
	    }
     }
     protected void AddEVAModules()
     {
         foreach (Part eva in evaParts)
         {
             AddEVARadioactivityTrackers(eva);
         }
     }
     protected void AddEVARadioactivityTrackers(Part p)
     {
         if (p.GetComponent<RadioactiveSink>() != null)
         {
             Utils.Log("EVA: Module already exists");
         }
         else
         {
             Utils.Log("EVA: Adding modules");
             RadioactiveSink sink = p.gameObject.AddComponent<RadioactiveSink>();
             RadiationTracker tracker = p.gameObject.AddComponent<RadiationTracker>();

             sink.SinkID = "Kerbal";
             tracker.AbsorberID = "Kerbal";
             evaModified = true;
         }
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
        if (allRadSources.Count > 0)
        {
            
            RemoveRadiationLink(src);
            allRadSources.Remove(src);
            if (RadioactivitySettings.debugNetwork && src != null)
                Utils.Log("Removing radiation source " + src.SourceID + " on part " + src.part.name + " from simulator");
        }
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
        if (allRadSinks.Count > 0)
        {
            RemoveRadiationLink(snk);
            allRadSinks.Remove(snk);
            if (RadioactivitySettings.debugNetwork && snk != null)
                Utils.Log("Removing radiation sink " + snk.SinkID + " on part " + snk.part.name + " from simulator");
        }
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
        if (HighLogic.LoadedSceneIsEditor)
        {
            Utils.Log("Editor: Starting monitor");
            GameEvents.onEditorShipModified.Add(new EventData<ShipConstruct>.OnEvent(onEditorVesselModified));
        }
        else
        {
            GameEvents.onEditorShipModified.Remove(new EventData<ShipConstruct>.OnEvent(onEditorVesselModified));
        }
    }

    public void onEditorVesselModified(ShipConstruct ship)
    {
        Utils.Log("Editor: Vessel Changed, recalculate all parts");
        if (!HighLogic.LoadedSceneIsEditor) { return; }
        foreach (RadioactiveSource s in allRadSources.ToList())
        {
            UnregisterSource(s);
        }
        foreach (RadioactiveSink s in allRadSinks.ToList())
        {
            UnregisterSink(s);
        }

        foreach (Part vesPart in ship.Parts)
        {
            RadioactiveSource src = vesPart.gameObject.GetComponent<RadioactiveSource>();
            RadioactiveSink snk = vesPart.gameObject.GetComponent<RadioactiveSink>();

            if (src != null)
                RegisterSource(src);
            if (snk != null)
                RegisterSink(snk);

        }
       
    }
    public void ForceRecomputeNetwork()
    {
        foreach (RadiationLink lnk in AllLinks)
        {
            lnk.ForceRecompute();
        }
    }
    protected void TryAddSource(RadioactiveSource src)
    {
        bool exists = false;
        foreach (RadioactiveSource usedS in allRadSources)
        {
            if (usedS == src)
                exists = true;
        }
        if (!exists)
            this.RegisterSource(src);
    }
    protected void TryAddSink(RadioactiveSink snk)
    {
        bool exists = false;
        foreach (RadioactiveSink usedS in allRadSinks)
        {
            if (usedS == snk)
                exists = true;
        }
        if (!exists)
            this.RegisterSink(snk);
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
        if (HighLogic.LoadedSceneIsFlight)
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
