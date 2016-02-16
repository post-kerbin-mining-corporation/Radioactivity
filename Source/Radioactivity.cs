using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Radioactivity
{

  // Represents an attenuation zone, where radiation is attenuated by passing through it
  public class AttenuationZone {
    public AttenuationType attenuationType;
    public Part associatedPart;
    public ModuleRadiationParameters params;
    public float size = 1f;
    public float density = 0.0001f;
    public float attenuationCoeff = 1f;

    public AttenuationZone(float sz)
    {
      attenuationType = AttenuationType.Empty;
      size = sz;
    }
    public AttenuationZone(float sz, Part part)
    {
      size = sz;
      density = Utils.GetDensity(part);
      attenuationCoeff = RadioactivitySettings.defaultPartAttenuationCoefficient;
      attenuationType  = AttenuationType.Part;
      associatedPart = part;
      params = part.GetComponent<ModuleRadiationParameters>();
      if (params != null)
      {
        attenuationType = AttenuationType.ParameterizedPart;
        density = params.Density;
        attenuationCoeff = params.AttenuationCoefficient;
      }
    }

    public float Attenuate(float inStrength)
    {
      if (attenuationType == AttenuationType.Empty)
      {
        // attenuate radiation only by inverse square
        return inStrength / (4.0f * Mathf.PI * this.size * this.size)
      }
      if (attenuationType == AttenuationType.ParameterizedPart)
      {
        float atten = inStrength / (4.0f * Mathf.PI * this.size * this.size)
        return atten* Mathf.Exp( -this.size * density * attenuationCoeff);
      }
      if (attenuationType == AttenuationType.Part)
      {
        // attenuate the distance
        float atten = inStrength / (4.0f * Mathf.PI * this.size * this.size)
        return atten* Mathf.Exp( -this.size * attenuationCoeff);
        // i0*e^(-ux), x = thickness (cm), u = linear attenuation coeff (cm-1). u values:
        // Al: 13, Pb: 82, W: 74, Fe: 26 -> need to be mult by density in g/cm3
      }
    }
  }

  // Represents a link between a radiation source and a radiation sink
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

    protected Vector3 relPos;
    protected LineRenderer overlayPath;
    protected GameObject go;
    protected List<AttenuationZone> attenuationPath;

    public RadiationLink(RadioactiveSource src, RadioactiveSink snk)
    {
        fluxStart = RadioactivitySettings.defaultRaycastFluxStart;
        source = src;
        sink = snk;
        ComputeConnection(src, snk);
    }

    // Hide or show the overlay for this link
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

    // Simulate the link, that is, compute the flux from the source and add it to the sink
    public void Simulate()
    {
      snk.AddRadiation(src.Emission * fluxEndScale);
    }

    // Tests to see whether the LOS needs to be recomputed
    protected void Recompute()
    {
      // LOS needs to be recomputed if we're off by more than X.
      // TODO: Needs to be recomputed if the total mass changes (propellant loss)
      // TODO: BUT that is more complicated!!
      Vector3 curRelPos = src.EmitterTransform.position - target.EmitterTransform.position;
      if (((curRelPos - relPos).sqrMagnitude > RadioactivitySettings.maximumPositionDelta))
      {
        ComputeConnection(source, sink);
      }
    }
    // Compute the ray path between the source and sink
    protected void ComputeConnection(RadioactiveSource src, RadioactiveSink target)
    {
      // Store the relative position of both endpoints
      relPos = src.EmitterTransform.position - target.EmitterTransform.position;

      // Gets parts between source and sink
      attenuationPath = GetLineOfSight(src, target);

      float sourceStrength = fluxStart;
      fluxEndScale = AttenuateFlux();
    }

    // Attenuates the ray between the source and sink
    protected float AttenuateFlux(List<AttenuationZone> rayPath, float strength)
    {
        // march along the ray, attenuating as we go
        float curFlux = strength;
        foreach (AttenuationZone z in rayPath)
        {
          if (curFlux >= fluxCutoff)
          {
            curFlux = z.Attenuate(curFlux);
          }
        }
    }

    // Computes LOS between a source and a sink
    // Returns the list of parts between the two objects
    protected List<AttenuationZone> GetLineOfSight(RadioactiveSource src, RadioactiveSink target)
    {
      // raycast from the source to target and vice versa
      RaycastHit[] hits;

      float sep = Vector3.Distance(sec.EmitterTransform, target.CenterTransform)

      hits1 = Physics.RaycastAll(src.EmitterTransform.position, target.CenterTransform.position - src.EmitterTransform.position, sep);
      hits2 = Physics.RaycastAll(target.CenterTransform.position, src.EmitterTransform.position - target.CenterTransform.position, sep);

      List<RacyastHit> hitsBackward = hits2.ToList();
      List<RaycastHit> hitsForward = hits1.ToList();

      /// sort by distance, ascending
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

    // Go through raycast results (both ways) in order to create the attenuation path
    protected List<AttenuationZone> CreatePathway(List<RaycastHit> outgoing, List<RaycastHit> incoming, float totalPathLength)
    {
      List<AttenuationZone> attens = new list<AttenuationZone>()

      float prevStop = 0f
      // for each object we hit outgoing, see if we found it incoming
      foreach (RaycastHit h in outgoing)
      {
        // Add a new empty attenuation zone based on where the last one stopped
        attens.Add (new AttenuationZone(AttenuationType.Empty, h.distance - prevStop));

        RaycastHit found = incoming.Find(item => item.rigidbody == h.rigidbody);

        // If this raycastHit has a friend in the incoming array, create a new zone based on that
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

  [KSPAddon(KSPAddon.Startup.MainMenu, false)]
  public class RadioactivitySetttings:MonoBehaviour
  {
    public void Start()
    {
      Utils.LoadSettings();
    }
  }

  [KSPAddon(KSPAddon.Startup.Flight, false)]
  public class Radioactivity:MonoBehaviour
  {
    public static Radioactivity Instance { get; private set; }

    List<RadioactiveSource> allRadSources = new List<RadioactiveSource>();
    List<RadioactiveSource> allRadSinks = new List<RadioactiveSink>();
    List<RadiationLink> allLinks = new List<RadioactiveLink>();

    // Add a radiation source to the source list
    public void RegisterSource(RadioactiveSource src)
    {
      allRadSources.Add(src);
      BuildNewRadiationLink(src);
      Utils.Log("Adding radiation source to simulator");
    }

    // Remove a radiation source from the source list
    public void UnregisterSource(RadioactiveSource src)
    {
      Utils.Log("Removing radiation source from simulator");
    }
    // Add a radiation sink to the sink list
    public void RegisterSink(RadioactiveSink snk)
    {
      allRadSinks.Add(snk);
      BuildNewRadiationLink(snk);
      Utils.Log("Adding radiation sink to simulator");
    }
    // Remove a radiation sink from the sink list
    public void UnregisterSink(RadioactiveSink snk)
    {
      Utils.Log("Removing radiation sink from simulator");
    }
    // Show the ray overlay for a given source or sink
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
    //Hide the ray overlay for a given source or sink
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

    // builds the radiation network from scratch
    protected void BuildRadiationLinks()
    {
      foreach (RadioactiveSource s in allRadSources)
      {
       foreach RadioactiveSink s2 in allRadSinks)
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

    }
    // Removes a link to a given radiation sink
    protected void RemoveRadiationLink(RadioactiveSink src)
    {

    }

    // Master method that simulates radiation
    protected void Simulate()
    {
      // Simulate point radiation
      if (RadioactivitySettings.simulatePointRadiation)
        SimulatePointRadiation();

      // Simulate solar radiation
    }

    // simulate point radiation from precomputed pathways
    protected void SimulatePointRadiation()
    {
      foreach (RadiationLink link in allLinks)
        {
          // recompute pathways if needed
          lnk.Recompute();
          // Simulate the radiation based on precomputed pathways
          lnk.Simulate();
        }

    }

    protected void simulateSolarRadiation()
    {

    }


  }
}
