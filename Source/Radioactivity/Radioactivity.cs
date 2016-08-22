using System;
using System.Collections;
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
             sink.IconID = 3;
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

      public bool SimulationReady
      {
          get { return simulationReady; }
      }

      bool simulationReady = false;
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
        for (int i = 0; i < allLinks.Count; i++)
        {
            allLinks[i].ShowOverlay();
        }
    }
    // Hide the ray overlay for ALL links
    public void HideAllOverlays()
    {
        for (int i = 0; i < allLinks.Count; i++)
        {
            allLinks[i].HideOverlay();
        }
    }
    // Show the ray overlay for a given source or sink
    public void ShowOverlay(RadioactiveSource src)
    {
      for (int i = 0; i < allLinks.Count; i++)
      {
        if (allLinks[i].source == src && !allLinks[i].overlayShown)
        {
          allLinks[i].ShowOverlay();
        }
      }
    }
    public void ShowOverlay(RadioactiveSink snk)
    {
        for (int i = 0; i < allLinks.Count; i++)
      {
          if (allLinks[i].sink == snk && !allLinks[i].overlayShown)
        {
          allLinks[i].ShowOverlay();
        }
      }
    }
    //Hide the ray overlay for a given source or sink
    public void HideOverlay(RadioactiveSource src)
    {
        for (int i = 0; i < allLinks.Count; i++)
      {
        if (allLinks[i].source == src)
        {
          allLinks[i].HideOverlay();
        }
      }
    }
    public void HideOverlay(RadioactiveSink snk)
    {
      for (int i = 0; i < allLinks.Count; i++)
      {
        if (allLinks[i].sink == snk)
        {
          allLinks[i].HideOverlay();
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
        StartCoroutine(WaitForInit(0.1f));
    }

    protected IEnumerator WaitForInit(float t)
    {
        
        yield return new WaitForSeconds(t);
        simulationReady = true;
        Utils.Log("Simulator: Ready");
    }

    public void onEditorVesselModified(ShipConstruct ship)
    {
        Utils.Log("Editor: Vessel Changed, recalculate all parts");
        if (!HighLogic.LoadedSceneIsEditor) { return; }

        for (int i= 0; i< allRadSources.Count ;i++)
        {
            UnregisterSource(allRadSources[i]);
        }
        for (int i= 0; i< allRadSinks.Count ;i++)
        {
            UnregisterSink(allRadSinks[i]);
        }

        for (int i = 0; i < ship.Parts.Count; i++)
        {
            RadioactiveSource src = ship.Parts[i].gameObject.GetComponent<RadioactiveSource>();
            RadioactiveSink snk = ship.Parts[i].gameObject.GetComponent<RadioactiveSink>();

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

        }
    }
    protected void TryAddSource(RadioactiveSource src)
    {
        bool exists = false;
        for (int i=0; i< allRadSources.Count; i++)
        {
            if (allRadSources[i] == src)
                exists = true;
        }
        if (!exists)
            this.RegisterSource(src);
    }
    protected void TryAddSink(RadioactiveSink snk)
    {
        bool exists = false;
        for (int i=0; i< allRadSinks.Count; i++)
        {
            if (allRadSinks[i] == snk)
                exists = true;
        }
        if (!exists)
            this.RegisterSink(snk);
    }

    // builds the radiation network from scratch
    protected void BuildRadiationLinks()
    {
        for (int i=0; i< allRadSources.Count; i++)
        {
            for (int j=0; i< allRadSinks.Count; i++)
            {
                allLinks.Add(new RadiationLink(allRadSources[i], allRadSinks[j]));
            }
        }
    }
    // Build new link for a new sink in the network
    protected void BuildNewRadiationLink(RadioactiveSink snk)
    {
        for (int i=0; i< allRadSources.Count; i++)
        {
            allLinks.Add(new RadiationLink(allRadSources[i], snk));
        }

    }
    // Build new links for a new source in the network
    protected void BuildNewRadiationLink(RadioactiveSource src)
    {
        for (int i=0; i< allRadSinks.Count; i++)
        {
            allLinks.Add(new RadiationLink(src, allRadSinks[i]));
        }
    }
    // Removes all links to a given radiation source
    protected void RemoveRadiationLink(RadioactiveSource src)
    {
        RadiationLink toRm = null;
        for (int i = 0; i < allLinks.Count; i++)
        {
            if (allLinks[i].source == src)
            {
                toRm = allLinks[i];
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
        for (int i = 0; i < allLinks.Count; i++)
        {
            if (allLinks[i].sink == snk)
            {
                toRm = allLinks[i];
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
        if (simulationReady)
        {
            if (HighLogic.LoadedSceneIsEditor)
            {
                SimulateEditor();
            }
            else
            {
                Simulate();
            }
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
       for (int i = 0; i < allLinks.Count; i++)
        {

          // Simulate the radiation based on precomputed pathways
          allLinks[i].Simulate(TimeWarp.fixedDeltaTime);
        }

    }

    protected void SimulateSolarRadiation()
    {

    }

    protected void SimulateCosmicRadiation()
    {
    }


    // Master method that simulates radiation
    protected void SimulateEditor()
    {
        // Simulate point radiation
        if (RadioactivitySettings.simulatePointRadiation)
            SimulatePointRadiationEditor();

        
    }
    // simulate point radiation
    protected void SimulatePointRadiationEditor()
    {
        for (int i = 0; i < allLinks.Count; i++)
        {
            // Simulate the radiation based on precomputed pathways
            allLinks[i].SimulateEditor(TimeWarp.fixedDeltaTime);
        }

    }
  }
}
