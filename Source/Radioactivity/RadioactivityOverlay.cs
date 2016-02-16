using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Radioactivity
{
  [KSPAddon(KSPAddon.Startup.Flight, false)]
  public class RadioactivityOverlay:MonoBehaviour
  {

    private bool showLinks = true;
    private List<RadiationLink> shownLinks = new List<RadiationLink>();

    public static RadioactivityOverlay Instance { get; private set; }

    public void Show(RadiationLink lnk)
    {
      SetupRenderer(lnk);
      shownLinks.Add(lnk);

    }
    public void Hide(RadiationLink lnk)
    {
      DestroyRenderer(lnk);
      shownLinks.Remove(lnk);
    }

    // Create and set up the line renderer
    protected void SetupRenderer(RadiationLink lnk)
    {
      lnk.GO = new GameObject("RadioactiveLinkRendererRoot");
      lnk.OverlayPath = lnk.GO.AddComponent<LineRenderer>();
      lnk.OverlayPath.material = new Material(Shader.Find("Particles/Additive"));
      lnk.OverlayPath.SetVertexCount(2);
      lnk.OverlayPath.SetWidth(lnk.source.CurrentEmission / 10f, lnk.source.CurrentEmission / 10f);
      lnk.OverlayPath.SetColors(Color.red, new Color(1f, 1f/lnk.fluxEndScale, 1f/lnk.fluxEndScale));
      lnk.OverlayPath.useWorldSpace = true;
      lnk.OverlayPath.SetPosition(0, lnk.source.EmitterTransform.position); 
      lnk.OverlayPath.SetPosition(1, lnk.sink.SinkTransform.position);
    }

    // Destroy the line renderer
    protected void DestroyRenderer(RadiationLink lnk)
    {
      if (lnk.GO)
        Destroy(lnk.GO);
    }

    protected void Awake()
    {
      Instance = this;
    }

    protected void FixedUpdate()
    {
      // Update links
      UpdateLinks();
    }

    protected void UpdateLinks()
    {
      foreach (RadiationLink lnk in shownLinks)
      {
        UpdatePathRenderer(lnk);
      }
    }

    protected void UpdatePathRenderer(RadiationLink lnk)
    {
      if (lnk.OverlayPath != null)
      {
        lnk.OverlayPath.SetColors(Color.red, new Color(1f, 1f/lnk.fluxEndScale, 1f/lnk.fluxEndScale));
        lnk.OverlayPath.SetPosition(0, lnk.source.EmitterTransform.position);
        lnk.OverlayPath.SetPosition(1, lnk.sink.SinkTransform.position);
      }
    }


  }
}
