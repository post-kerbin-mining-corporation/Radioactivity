using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Radioactivity
{

  # Types of zone for attenuation
  enum AttenuationType {
    Part, Empty
  }

  # Represents an attenuation zone, where radiation is attenuated by passing through it
  public class AttenuationZone {
    public AttenuationType attenuationType;
    public Part associatedPart;
    public float size;

    public AttenuationZone(AttenuationType tp, float sz)
    {
      attenuationType = tp;
      size = sz;
    }
    public AttenuationZone(AttenuationType tp, float sz, Part part)
      : this(tp, sz)
    {
        associatedPart = part;
    }

    public float Attenuate(float inStrength)
    {
      if (attenuationType == AttenuationType.Empty)
      {
        # attenuate radiation only by inverse square
        return flux/ (4.0f * Mathf.PI * this.size * this.size)
      }

      if (attenuationType == AttenuationType.Part)
      {
        # attenuate the distance
        float atten = flux/ (4.0f * Mathf.PI * this.size * this.size)
        # default spacecraft component is aluminum
        float mu = 16.0f;
        # density in gm/cm3
        float rho = 1f;
        return atten* Mathf.Exp( -this.size * rho * mu);
        // i0*e^(-ux), x = thickness (cm), u = linear attenuation coeff (cm-1). u values:
        // Al: 13, Pb: 82, W: 74, Fe: 26 -> need to be mult by density in g/cm3
      }
    }
  }

  # Represents a link between a source and a sink
  public class RadiationLink
  {

    public RadioactiveSource source;
    public RadioactiveSink sink;

    public float fluxStart = 1.0f;
    public float fluxEndScale = 0.0f;

    public float overlayShown = false;

    public List<AttenuationZone> Path
    {
      get {return attenuationPath;}
    }

    public LineRenderer OverlayPath
    {
      get {return overlayPath;}
      set {overlayPath = value;}
    }

    public GameObject GO
    {
      get {return go;}
    }

    protected LineRenderer overlayPath;
    protected GameObject go;
    protected List<AttenuationZone> attenuationPath;

    public RadiationLink(RadioactiveSource src, RadioactiveSink snk)
    {
        source = src;
        sink = snk;
        ComputeConnection(src, snk);
    }

    public void ToggleOverlay()
    {
        overlayShown = !overlayShown;
        if (overlayShown)
          ShowOverlay();
        else
            HideOverlay();
    }
    public void ShowOverlay()
    {
      overlayShown = true;
      RadioactivityOverlay.Instance.Show(this);
    }
    public void HideOverlay()
    {
      overlayShown = false;
      RadioactivityOverlay.Instance.Hide(this);
    }

    public void Simulate()
    {
      snk.AddFlux(src.Emission * fluxEndScale);
    }

    # Gets the ray path
    public void ComputeConnection(RadioactiveSource src, RadioactiveSink target)
    {
      # Gets parts between source and sink
      attenuationPath = GetLineOfSight(src, target);

      float sourceStrength = fluxStart;
      fluxEndScale = AttenuateFlux();
    }
    # Attenuates a ray
    protected float AttenuateFlux(List<AttenuationZone> rayPath, float strength)
    {
        # march along the ray, attenuating as we go
        float curFlux = strength;
        foreach (AttenuationZone z in rayPath)
        {
          if (curFlux >= fluxCutoff)
          {
            curFlux = z.Attenuate(curFlux);
          }
        }
    }

    # Computes LOS between a source and a sink
    # Returns the list of parts between the two objects
    protected List<AttenuationZone> GetLineOfSight(RadioactiveSource src, RadioactiveSink target)
    {
      # raycast
      RaycastHit[] hits;

      float sep = Vector3.Distance(sec.EmitterTransform, target.CenterTransform)

      hits1 = Physics.RaycastAll(src.EmitterTransform.position, target.CenterTransform.position - src.EmitterTransform.position, sep);
      hits2 = Physics.RaycastAll(target.CenterTransform.position, src.EmitterTransform.position - target.CenterTransform.position, sep);

      List<RacyastHit> hitsBackward = hits2.ToList();
      List<RaycastHit> hitsForward = hits1.ToList();

      # sort by distance, ascending
      if (hitsForward.Count > 0)
      {
        hitsForward = hitsForward.OrderBy(o=>o.distance).ToList();
      }
      if (hitsBackward.Count > 0)
      {
        hitsBackward = hitsBackward.OrderByDescending(o=>o.distance).ToList();
      }

      if (hitsForward.Count > 0 && hitsBackward.Count > 0)
        return CreatePathway (hitsForward, hitsBackward, sep);
      else
        return null;
    }

    protected List<AttenuationZone> CreatePathway(List<RaycastHit> outgoing, List<RaycastHit> incoming, float totalPathLength)
    {
      List<AttenuationZone> attens = new list<AttenuationZone>()

      float prevStop = 0f
      # for each object we hit outgoing, see if we found it incoming
      foreach (RaycastHit h in outgoing)
      {
        # Add a new attenuation zone based on where the last one stopped
        attens.Add (new AttenuationZone(AttenuationType.Empty, h.distance - prevStop));

        RaycastHit found = incoming.Find(item => item.rigidbody == h.rigidbody);

        # If this raycastHit has a friend in the incoming array, create a new zone based on that
        if (found != null)
        {
          attens.Add(new AttenuationZone(AttenuationType.Part, totalPathLength - h.distance - found.distance, h.GetComponent<Part>));
          prevStop = h.dist + totalPathLength - found.distance;

        } else
        {
          prevStop = h.distance;
        }
      }
      return attens;
    }
  }

  [KSPAddon(KSPAddon.Startup.Flight, false)]
  public class Radioactivity:MonoBehaviour
  {
    # Items loaded from CFG
    # ---------------------
    # maximum distance to raycast
    float raycastDistance = 2000f;
    # minimum flux level to care about
    float fluxCutoff = 1f;

    float defaultPartAttenuationCoefficient = 16f;

    bool simulatePointRadiation = true;
    bool simulateSolarRadiation = false;
    bool simulateCosmicRadiation = false;
    # ---------------------

    public static Radioactivity Instance { get; private set; }

    List<RadioactiveSource> allRadSources = new List<RadioactiveSource>();
    List<RadioactiveSource> allRadSinks = new List<RadioactiveSink>();
    List<RadiationLink> allLinks = new List<RadioactiveLink>();

    # Remove a radiation source from the source list
    public void RegisterSource(RadioactiveSource src)
    {
      allRadSources.Add(src);
      Utils.Log("Adding radiation source to simulator");
    }

    # Remove a radiation source from the source list
    public void UnregisterSource(RadioactiveSource src)
    {
      Utils.Log("Removing radiation source from simulator");
    }
    # Add a radiation sink to the sink list
    public void RegisterSink(RadioactiveSink snk)
    {
      allRadSinks.Add(snk);
      Utils.Log("Adding radiation sink to simulator");
    }
    # Remove a radiation sink from the sink list
    public void UnregisterSink(RadioactiveSink snk)
    {
      Utils.Log("Removing radiation sink from simulator");
    }
    # Show the overlay for a given source
    public void ShowOverlay(RadioactiveSource src)
    {
      foreach (RadioactiveLink lnk in allLinks)
      {
        if (lnk.source == src)
        {
          lnk.ShowOverlay();
        }
      }
    }
    public void ShowOverlay(RadioactiveSink snk)
    {
      foreach (RadioactiveLink lnk in allLinks)
      {
        if (lnk.sink == snk)
        {
          lnk.ShowOverlay();
        }
      }
    }
    public void HideOverlay(RadioactiveSource src)
    {
      foreach (RadioactiveLink lnk in allLinks)
      {
        if (lnk.source == src)
        {
          lnk.HideOverlay();
        }
      }
    }
    public void HideOverlay(RadioactiveSink snk)
    {
      foreach (RadioactiveLink lnk in allLinks)
      {
        if (lnk.source == snk)
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


    public void FixedUpdate()
    {
      float timeScalar = TimeWarp.fixedDeltaTime;
      Simulate();
    }

    # builds the radiation network from scratch
    public void BuildRadiationLinks()
    {
      foreach (RadioactiveSource s in allRadSources)
      {
       foreach RadioactiveSink s2 in allRadSinks)
       {
         allLinks.Add(new RadiationLink(s, s2));
       }
      }
    }

    # Master method that simulates radiation
    public void Simulate()
    {
      if (simulatePointRadiation)
        SimulatePointRadiation();
    }

    # simulate point radiation from precomputed pathways
    public void SimulatePointRadiation()
    {
      foreach (RadiationLink link in allLinks)
        {
          # recompute pathways if needed
          # Simulate the radiation based on precomputed pathways
          lnk.Simulate();
        }

    }


  }
}
