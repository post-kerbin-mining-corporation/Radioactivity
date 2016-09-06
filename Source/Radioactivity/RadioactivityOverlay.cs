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
    private List<ShadowShieldEffect> shadowShields = new List<ShadowShieldEffect>();

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
    public void ShowShield(RadioactiveSource src)
    {
        foreach (ShadowShieldEffect shld in src.ShadowShields)
        {
            SetupShadowShieldRenderer(shld, src);
            shadowShields.Add(shld);
        }


    }
    public void HideShield(RadioactiveSource src)
    {
        List<ShadowShieldEffect> toClear = new List<ShadowShieldEffect>();
        foreach (ShadowShieldEffect shld in shadowShields)
        {
            foreach (ShadowShieldEffect shldSrc in src.ShadowShields)
            {
                if (shld == shldSrc)
                {
                    toClear.Add(shld);
                    DestroyShadowShieldRenderer(shld.renderer);
                }
            }
        }
        for (int i = 0; i < toClear.Count ; i++)
        {
            shadowShields.Remove(toClear[i]);
        }

    }

    public void HideShields()
    {
        foreach (ShadowShieldEffect shld in shadowShields)
        {
            DestroyShadowShieldRenderer(shld.renderer);
        }
        shadowShields.Clear();
    }
    public void Update(RadiationLink lnk)
    {
       for (int i = 0; i < shownLinks.Count; i++)
        {
            if (shownLinks[i] == lnk)
                UpdateRenderer(lnk);
        }
    }

    protected void SetupShadowShieldRenderer(ShadowShieldEffect shld, RadioactiveSource parent)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        shld.renderer = go;
        Destroy(go.GetComponent<Collider>());
        go.transform.parent = parent.part.partTransform;
        go.transform.localPosition = shld.localPosition;
        go.transform.localScale = shld.dimensions;

        go.transform.up = parent.EmitterTransform.position - go.transform.position;

        MeshRenderer m = go.GetComponent<MeshRenderer>();
        m.material = new Material(Shader.Find(RadioactivitySettings.overlayRayMaterial));
        m.material.color = Color.blue;
        m.material.renderQueue = 3000;

        if (RadioactivitySettings.debugOverlay)
            Utils.Log("Overlay: Showing shadow shield on " + parent.SourceID );

    }
    protected void DestroyShadowShieldRenderer(GameObject shld)
    {
        GameObject.Destroy(shld);
    }
    // Create and set up the line renderer
    protected void SetupRenderer(RadiationLink lnk)
    {
      lnk.GO = new GameObject("RadioactiveLinkRendererRoot");
      if (HighLogic.LoadedSceneIsFlight)
      {
          lnk.GO.transform.SetParent(lnk.source.vessel.vesselTransform, true);
          //lnk.GO.transform.localRotation = Quaternion.identity;
      }
      else if (HighLogic.LoadedSceneIsEditor)
      {
        if (EditorLogic.fetch.ship != null)
        {

        }
      }
      for (int i = 0; i < lnk.Path.Count; i++)
      {
            CreateZoneLineRenderer(lnk, lnk.Path[i]);
      }
      if (RadioactivitySettings.debugOverlay)
        Utils.Log("Overlay: Showing link between " + lnk.source.SourceID + " and "+ lnk.sink.SinkID+ " for render");
    }
    protected void CreateZoneLineRenderer(RadiationLink lnk, AttenuationZone zn)
    {
        /// Create the components
        LineRenderer lr = CreateBasicRenderer(lnk.GO.transform);
        float valIn = -(float)Math.Log10(zn.attenuationOut)/10f;
        float valOut = -(float)Math.Log10(zn.attenuationOut) / 10f;
        lr.SetColors(grad.Evaluate(valIn),grad.Evaluate(valOut));

        // Set up the geometry
        float w = Mathf.Clamp(RadioactivitySettings.overlayRayWidthMin, RadioactivitySettings.overlayRayWidthMin, RadioactivitySettings.overlayRayWidthMax);
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
        child.transform.SetParent(parent, true);
        //child.transform.localRotation = Quaternion.identity;

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

        for (int i = 0; i < lnk.Path.Count; i++)
        {
              CreateZoneLineRenderer(lnk, lnk.Path[i]);
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
      for (int i = 0; i < shownLinks.Count; i++)
      {
            UpdatePathRenderer(shownLinks[i]);
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
