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

      private Gradient grad;
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
    public void Update(RadiationLink lnk)
    {
        foreach (RadiationLink l in shownLinks)
        {
            if (l == lnk)
                UpdateRenderer(lnk);
        }
    }

    // Create and set up the line renderer
    protected void SetupRenderer(RadiationLink lnk)
    {
      lnk.GO = new GameObject("RadioactiveLinkRendererRoot");
      lnk.GO.transform.parent = lnk.source.vessel.vesselTransform;
      foreach (AttenuationZone zn in lnk.Path)
      {
          
            CreateZoneLineRenderer(lnk, zn);
          
      }
      if (RadioactivitySettings.debugOverlay)
        Utils.Log("Overlay: Showing link between " + lnk.source.SourceID + " and "+ lnk.sink.SinkID+ " for render");
    }
    protected void CreateZoneLineRenderer(RadiationLink lnk, AttenuationZone zn)
    {
        /// Create the components
        LineRenderer lr = CreateBasicRenderer(lnk.GO.transform);
        float valIn = -(float)Math.Log10(zn.attenuationIn)/10f;
        float valOut = -(float)Math.Log10(zn.attenuationOut) / 10f;
        lr.SetColors(grad.Evaluate(valIn),grad.Evaluate(valOut));

        // Set up the geometry
        float w = Mathf.Clamp(lnk.source.CurrentEmission * RadioactivitySettings.overlayRayWidthMult, RadioactivitySettings.overlayRayWidthMin, RadioactivitySettings.overlayRayWidthMax);
        lr.SetWidth(w, w);

        Vector3 pVec = Vector3.Cross((zn.startPosition - zn.endPosition), Camera.main.transform.forward).normalized;

        //lr.useWorldSpace = true;
        lr.SetPosition(0, zn.startPosition + pVec*w*0f);
        lr.SetPosition(1, zn.endPosition + pVec * w*0f);
    }
    protected Gradient ConstructGradient()
    {
        Gradient g = new Gradient();
        GradientColorKey[] gkColor = new GradientColorKey[7];
        GradientAlphaKey[] gkAlpha = new GradientAlphaKey[4];

        gkColor[0].color = Color.white;
        gkColor[1].color = Color.red;
        gkColor[2].color = Color.yellow;
        gkColor[3].color = Color.green;
        gkColor[4].color = Color.blue;
        gkColor[5].color = Color.magenta;
        gkColor[6].color = Color.black;

        gkColor[0].time = 0f;
        gkColor[1].time = 0.166f;
        gkColor[2].time = 0.33f;
        gkColor[3].time = 0.5f;
        gkColor[4].time = 0.66f;
        gkColor[5].time = 0.833f;
        gkColor[6].time = 1f;

        gkAlpha[0].alpha = 0.75f;
        gkAlpha[1].alpha = 0.75f;
        gkAlpha[2].alpha = 0.75f;
        gkAlpha[3].alpha = 0.75f;

        gkAlpha[0].time = 0f;
        gkAlpha[1].time = 0.33f;
        gkAlpha[2].time = 0.66f;
        gkAlpha[3].time = 1f;

        g.SetKeys(gkColor, gkAlpha);

        return g;
    }
    protected LineRenderer CreateBasicRenderer(Transform parent)
    {
        GameObject child = new GameObject("RadioactiveLinkRendererChild");
        child.transform.parent = parent;
        LineRenderer lr = child.AddComponent<LineRenderer>();
        // Set up the material
        lr.material = new Material(Shader.Find(RadioactivitySettings.overlayRayMaterial));
        lr.material.color = Color.white;
        lr.material.renderQueue = 3000;
        lr.gameObject.layer = 0;
        lr.SetVertexCount(2);
        lr.useWorldSpace = false;
        return lr;
    }


    protected void DestroyZoneLineRenderers(RadiationLink lnk)
    {
        if (lnk.GO != null)
        {
            int childs = lnk.GO.transform.childCount;
            for (int i = childs - 1; i >= 0; i--)
            {
                GameObject.Destroy(lnk.GO.transform.GetChild(i).gameObject);
            }
        }

    }
    protected void UpdateRenderer(RadiationLink lnk)
    {
        DestroyZoneLineRenderers(lnk);
        foreach (AttenuationZone zn in lnk.Path)
        {
             CreateZoneLineRenderer(lnk, zn);
        }
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
    protected void Start()
    {
        grad = ConstructGradient();
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
      if (lnk.GO != null)
      {

       //   lnk.OverlayPath.SetColors(Color.red, new Color(1f, Mathf.Log10((float)(1d / lnk.fluxEndScale)) / 10f, Mathf.Log10((float)(1d / lnk.fluxEndScale)) / 10f));
       // lnk.OverlayPath.SetPosition(0, lnk.source.EmitterTransform.position);
       // lnk.OverlayPath.SetPosition(1, lnk.sink.SinkTransform.position);
       // float w = Mathf.Clamp(lnk.source.CurrentEmission *RadioactivitySettings.overlayRayWidthMult, RadioactivitySettings.overlayRayWidthMin, RadioactivitySettings.overlayRayWidthMax);
       // lnk.OverlayPath.SetWidth(w, w);
      }
    }


  }
}
