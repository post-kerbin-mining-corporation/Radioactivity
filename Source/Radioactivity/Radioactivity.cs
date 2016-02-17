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
    public ModuleRadiationParameters parameters;
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
      parameters = part.GetComponent<ModuleRadiationParameters>();
      if (parameters != null)
      {
        attenuationType = AttenuationType.ParameterizedPart;
        density = parameters.Density;
        attenuationCoeff = parameters.AttenuationCoefficient;
      }
    }

    public float Attenuate(float inStrength)
    {
      if (attenuationType == AttenuationType.Empty)
      {
        // attenuate radiation only by inverse square
        return inStrength / (4.0f * Mathf.PI * this.size * this.size);
      }
      if (attenuationType == AttenuationType.ParameterizedPart)
      {
        float atten = inStrength / (4.0f * Mathf.PI * this.size * this.size);
        return atten* Mathf.Exp( -this.size * density * attenuationCoeff);
      }
      if (attenuationType == AttenuationType.Part)
      {
        // attenuate the distance
          float atten = inStrength / (4.0f * Mathf.PI * this.size * this.size);
        return atten* Mathf.Exp( -this.size * attenuationCoeff);
        // i0*e^(-ux), x = thickness (cm), u = linear attenuation coeff (cm-1). u values:
        // Al: 13, Pb: 82, W: 74, Fe: 26 -> need to be mult by density in g/cm3
      }
        return inStrength;
    }
  }

  // Represents a link between a radiation source and a radiation sink
  public class RadiationLink
  {
    public RadioactiveSource source;
    public RadioactiveSink sink;

    public float fluxStart = 1.0f;
    public float fluxEndScale = 0.0f;

    public bool overlayShown = false;
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
        set { go = value; }
    }

    public int ZoneCount
    {
        get { if (Path != null) return Path.Count; else return 0; }
    }

    public int OccluderCount
    {
        get { if (Path != null) 
        {
            int ct = 0;
            foreach (AttenuationZone z in Path)
            {
                if (z.attenuationType != AttenuationType.Empty)
                    ct++;
            }
            return ct;
        }
        return 0;
        }
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
      sink.AddRadiation(source.CurrentEmission * fluxEndScale);
    }

    // Tests to see whether the LOS needs to be recomputed
    public void Recompute()
    {
      // LOS needs to be recomputed if we're off by more than X.
      // TODO: Needs to be recomputed if the total mass changes (propellant loss)
      // TODO: BUT that is more complicated!!
      Vector3 curRelPos = source.EmitterTransform.position - sink.SinkTransform.position;
      if (((curRelPos - relPos).sqrMagnitude > RadioactivitySettings.maximumPositionDelta))
      {
        ComputeConnection(source, sink);
      }
    }
    // Compute the ray path between the source and sink
    protected void ComputeConnection(RadioactiveSource src, RadioactiveSink target)
    {
        Utils.Log("Creating connection from " + src.part.name + " to " + target.part.name);

      // Store the relative position of both endpoints
      relPos = src.EmitterTransform.position - target.SinkTransform.position;

      // Gets parts between source and sink
      attenuationPath = GetLineOfSight(src, target);

      float sourceStrength = AttenuateFlux(attenuationPath, fluxStart);
    }

    // Attenuates the ray between the source and sink
    protected float AttenuateFlux(List<AttenuationZone> rayPath, float strength)
    {
        // march along the ray, attenuating as we go
        float curFlux = strength;
        foreach (AttenuationZone z in rayPath)
        {
          if (curFlux >= RadioactivitySettings.fluxCutoff)
          {
            curFlux = z.Attenuate(curFlux);
          }
        }

        return curFlux;
    }

    // Computes LOS between a source and a sink
    // Returns the list of parts between the two objects
    protected List<AttenuationZone> GetLineOfSight(RadioactiveSource src, RadioactiveSink target)
    {
      // raycast from the source to target and vice versa
      RaycastHit[] hits1;
      RaycastHit[] hits2;
      float sep = Vector3.Distance(src.EmitterTransform.position, target.SinkTransform.position);

      hits1 = Physics.RaycastAll(src.EmitterTransform.position, target.SinkTransform.position - src.EmitterTransform.position, sep);
      hits2 = Physics.RaycastAll(target.SinkTransform.position, src.EmitterTransform.position - target.SinkTransform.position, sep);

      List<RaycastHit> hitsBackward = hits2.ToList();
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
      List<AttenuationZone> attens = new List<AttenuationZone>();

      float prevStop = 0f;
      // for each object we hit outgoing, see if we found it incoming
      foreach (RaycastHit h in outgoing)
      {
       
        // Add a new empty attenuation zone based on where the last one stopped
        attens.Add (new AttenuationZone(h.distance - prevStop));
        if (h.rigidbody != null)
        {
            RaycastHit found = incoming.Find(item => item.rigidbody == h.rigidbody);
            Utils.Log(found.rigidbody.ToString());
            // If this raycastHit has a friend in the incoming array, create a new zone based on that
            if (found.collider != null)
            {
                attens.Add(new AttenuationZone(totalPathLength - h.distance - found.distance, h.rigidbody.gameObject.GetComponent<Part>()));
                prevStop = h.distance + totalPathLength - found.distance;

            }
            else
            {
                prevStop = h.distance;
            }
        }
      }
      return attens;
    }
  }

  [KSPAddon(KSPAddon.Startup.MainMenu, false)]
  public class RadioactivityStartup:MonoBehaviour
  {
    public void Start()
    {
      Utils.LoadSettings();
    }
  }

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
       if (RadioactivitySettings.debugNetwork)
           Utils.Log("Removing radiation sink "+ snk.SinkID +" on part " + snk.part.name + " from simulator");
    }
    public void ShowAllOverlays()
    {
        foreach (RadiationLink lnk in allLinks)
        {
            lnk.ShowOverlay();
        }
    }
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
        if (lnk.source == src)
        {
          lnk.ShowOverlay();
        }
      }
    }
    public void ShowOverlay(RadioactiveSink snk)
    {
        foreach (RadiationLink lnk in allLinks)
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
          link.Recompute();
          // Simulate the radiation based on precomputed pathways
          link.Simulate();
        }

    }

    protected void simulateSolarRadiation()
    {

    }


  }
}
