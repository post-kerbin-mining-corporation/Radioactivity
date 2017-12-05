using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Radioactivity.Simulator;

namespace Radioactivity
{
    /// <summary>
    /// Adds Radioactivity modules to EVA Kerbals
    /// </summary>
  [KSPAddon(KSPAddon.Startup.MainMenu, false)]
  public class RadioactivityEVA:MonoBehaviour
  {
      private List<Part> evaParts;
      private bool evaModified = false;

      public void Awake()
      {
          evaParts = new List<Part>();
      }

    public void Update()
    {
        if (!PartLoader.Instance.IsReady() || PartResourceLibrary.Instance == null)
        {
            return;
        }
        if (evaParts.Count < 2)
        {
            Utils.Log("[RadioactivityStartup]: Finding EVA parts");
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
                Utils.Log("[RadioactivityStartup]: Module already exists");
         }
         else
         {
                Utils.Log("[RadioactivityStartup]: Adding modules");
             RadioactiveSink sink = p.gameObject.AddComponent<RadioactiveSink>();
             RadiationShieldedCrewContainer tracker = p.gameObject.AddComponent<RadiationShieldedCrewContainer>();

             sink.SinkID = "Kerbal";
             sink.IconID = "kerbal";
             tracker.AbsorberID = "Kerbal";
             tracker.RadiationAttenuationFraction = 0.0f;
             evaModified = true;
         }
     }
  }

  // Main class. Does simulations, holds data, holds the network
  [KSPAddon(KSPAddon.Startup.EveryScene, false)]
  public class Radioactivity:MonoBehaviour
  {
        public static Radioactivity Instance { get; private set; }


        public bool RayOverlayShown
        {
            get { return rayOverlayShown;}
            set { rayOverlayShown = value;}
        }
        public RadioactivitySimulator RadSim
        { 
            get { return radSim;}
        }

        RadioactivitySimulator radSim;
        bool rayOverlayShown = false;


        protected void Awake()
        {
            Instance = this;
        }

        protected void Start()
        {
            // Wait breifly before setting up the simulation
            StartCoroutine(WaitForInit(0.1f));
            Utils.Log("[Radioactivity]: Simulation started...");
            radSim = new RadioactivitySimulator();
        }

        protected IEnumerator WaitForInit(float t)
        {
            
            yield return new WaitForSeconds(t);
        }

        protected void FixedUpdate()
        {
            if (radSim != null)
                radSim.Simulate(TimeWarp.fixedDeltaTime);
        }
  }
}
