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
    private List<RadioactiveLink> shownLinks = new List<RadioactiveLink>();

    public static RadioactivityOverlay Instance { get; private set; }

    public void Show(RadioactiveLink lnk)
    {
      SetupRenderer(lnk);
      shownLinks.Add(lnk);

    }
    public void Hide(RadioactiveLink lnk)
    {
      DestroyRenderer(lnk);
      shownLinks.Remove(lnk);
    }

    protected SetupRenderer(RadioactiveLink lnk)
    {
      lnk.GO = new GameObject("RadioactiveLinkRendererRoot");
      lnk.OverlayPath = lnk.GO.AddComponent(LineRenderer);
      lnk.OverlayPath.material = new Material(Shader.Find("Particles/Additive"));
      lnk.OverlayPath.SetVertexCount(2);
      lnk.OverlayPath.SetWidth(lnk.source.CurrentEmission / 10f);
      lnk.OverlayPath.SetColors(Color.red, new Color(1f, 1f/lnk.fluxEndScale, 1f/lnk.fluxEndScale));
      lnk.OverlayPath.UseWorldSpace();
      lnk.OverlayPath.SetPostions(new Vector3[lnk.source.EmitterTransform.position, lnk.sink.SinkTransform.position]);
    }

    protected DestroyRenderer(RadioactiveLink lnk)
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
      foreach (RadioactiveLink lnk in shownLinks)
      {
        UpdatePathRenderer(lnk);
      }
    }

    protected void UpdatePathRenderer(RadioactiveLink lnk)
    {
      if (lnk.OverlayPath != null)
      {
        lnk.OverlayPath.SetColors(Color.red, new Color(1f, 1f/lnk.fluxEndScale, 1f/lnk.fluxEndScale));
        lnk.OverlayPath.SetPostions(new Vector3[lnk.source.EmitterTransform.position, lnk.sink.SinkTransform.position]);
      }
    }


  }
}
