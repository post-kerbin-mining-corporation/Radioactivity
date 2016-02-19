using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Radioactivity
{
  [KSPAddon(KSPAddon.Startup.EveryScene, false)]
  public class RadioactivityOverlay:MonoBehaviour
  {

    
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
      //lnk.GO.layer = RadioactivitySettings.overlayRayLayer;
      lnk.OverlayPath = lnk.GO.AddComponent<LineRenderer>();
      lnk.OverlayPath.material = new Material(Shader.Find(RadioactivitySettings.overlayRayMaterial));
      lnk.OverlayPath.material.color = Color.white;
      lnk.OverlayPath.SetVertexCount(2);
      float w = Mathf.Clamp(lnk.source.CurrentEmission * RadioactivitySettings.overlayRayWidthMult, RadioactivitySettings.overlayRayWidthMin, RadioactivitySettings.overlayRayWidthMax);
      lnk.OverlayPath.SetWidth(w, w);
      lnk.OverlayPath.SetColors(Color.red, new Color(1f, Mathf.Log10((float)(1d / lnk.fluxEndScale)) / 10f, Mathf.Log10((float)(1d / lnk.fluxEndScale)) / 10f));
      lnk.OverlayPath.useWorldSpace = true;
      lnk.OverlayPath.SetPosition(0, lnk.source.EmitterTransform.position); 
      lnk.OverlayPath.SetPosition(1, lnk.sink.SinkTransform.position);
      if (RadioactivitySettings.debugOverlay)
        Utils.Log("Overlay: Showing link between " + lnk.source.SourceID + " and "+ lnk.sink.SinkID+ " for render");
    }

    // Destroy the line renderer
    protected void DestroyRenderer(RadiationLink lnk)
    {
      if (lnk.GO)
        Destroy(lnk.GO);
      if (RadioactivitySettings.debugOverlay)
        Utils.Log("Overlay: Hiding link between " + lnk.source.SourceID + " and " + lnk.sink.SinkID + " for render");
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
          lnk.OverlayPath.SetColors(Color.red, new Color(1f, Mathf.Log10((float)(1d / lnk.fluxEndScale)) / 10f, Mathf.Log10((float)(1d / lnk.fluxEndScale)) / 10f));
        lnk.OverlayPath.SetPosition(0, lnk.source.EmitterTransform.position);
        lnk.OverlayPath.SetPosition(1, lnk.sink.SinkTransform.position);
        float w = Mathf.Clamp(lnk.source.CurrentEmission *RadioactivitySettings.overlayRayWidthMult, RadioactivitySettings.overlayRayWidthMin, RadioactivitySettings.overlayRayWidthMax);
        lnk.OverlayPath.SetWidth(w, w);
      }
    }


  }
}
