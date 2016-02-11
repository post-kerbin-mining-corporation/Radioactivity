using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NearFutureElectrical
{
  [KSPAddon(KSPAddon.Startup.Flight, false)]
  public class Radioactivity:MonoBehaviour
  {

    enum AttenuationType {
      Part, Empty
    }

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
    }

    float raycastDistance = 2500f;

    float fluxCutoff = 1f;

    List<RadioactiveSource> allRadSources = new List<RadioactiveSource>();
    List<RadioactiveSource> allRadSinks = new List<RadioactiveSink>();

    # Remove a radiation source from the source list
    public void RegisterSource(RadioactiveSource src)
    {
      allRadSources.Add(src);
    }

    # Remove a radiation source from the source list
    public void UnregisterSource(RadioactiveSource src)
    {

    }
    # Add a radiation sink to the sink list
    public void RegisterSink(RadioactiveSink snk)
    {
      allRadSinks.Add(snk);
    }
    # Remove a radiation sink from the sink list
    public void UnregisterSink(snk)
    {

    }

    public void Start()
    {

    }

    # Master method that simulates all paths
    public void Simulate()
    {
      foreach (RadioactiveSource src in allRadSources) {
        if (src.Emitting)

      }
    }

    # Computes emision line of sights
    public void ComputeEmission(RadioactiveSource src)
    {
      foreach (RadioactiveSink snk in allRadSinks)
      {
        ComputeFluxAtTarget(src, snk);
      }

    }

    public void ComputeFluxAtTarget(RadioactiveSource src, RadioactiveSink target)
    {
      # Gets parts between source and sink
      List<AttenuationZone> hits = GetLineOfSight(src, target);

      float sourceStrength = src.Emission;
      float finalFlux = AttenuateFlux();

    }
    protected AttenuateFlux(List<AttenuationZone> rayPath, float strength)
    {
        # march along the ray, attenuating as we go
        float curFlux = strength;
        foreach (AttenuationZone z in rayPath)
        {

          if (z.attenuationType == AttenuationType.Empty)
          {
              curFlux = AttentuateEmpty(z, curFlux);
          }
          else if (z.attenuationType == AttentionType.Part)
          {
              curFlux = AttenuatePart(z, curFlux);
          }
        }
    }

    protected float AttentuateEmpty(AttenuationZone z, float flux)
    {
        return flux/ (4.0f * Mathf.PI * z.size * z.size)
    }

    protected float AttenuatePart(AttenuationZone z, float flux)
    {
        # attenuate the distance
        float atten = AttentuateEmpty(z, flux);
        # default spacecraft component is aluminum
        float mu = 16.0f;
        # density in gm/cm3
        float rho = 1f;

        return atten* Mathf.Exp( -z.size * rho * mu);
        // i0*e^(-ux), x = thickness (cm), u = linear attenuation coeff (cm-1). u values:
        // Al: 13, Pb: 82, W: 74, Fe: 26 -> need to be mult by density in g/cm3
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
}
