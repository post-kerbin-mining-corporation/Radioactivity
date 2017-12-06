using System;
using System.Collections.Generic;
using Radioactivity.Simulator;
using Radioactivity;
using UnityEngine;

namespace Radioactivity.UI
{
    /// <summary>
    /// Visualizes a radiation link
    /// </summary>
    public class OverlayLinkRenderer: OverlayRenderer
    {
       
        public RadiationLink Link { get { return link; }}

        protected RadiationLink link;

        protected List<OverlayLinkZone> renderers;

        public OverlayLinkRenderer(RadiationLink toRender)
        {
            root = new GameObject("RadioactiveLinkRendererRoot");
            link = toRender;
            if (RadioactivityConstants.debugOverlay)
                Utils.Log("[OverlayLinkRenderer]: Initialized");
            if (HighLogic.LoadedSceneIsFlight)
            {
                root.transform.SetParent(link.source.part.partTransform, true);
                //lnk.GO.transform.localRotation = Quaternion.identity;
            }
            else if (HighLogic.LoadedSceneIsEditor)
            {
                if (EditorLogic.fetch.ship != null)
                {

                }
            }
            renderers = new List<OverlayLinkZone>();
            Rebuild();

        }

        public void Rebuild()
        {
            
            DestroyAll();
            for (int i = 0; i < link.Path.Count; i++)
            {
                renderers.Add(new OverlayLinkZone(link.Path[i], link, root.transform));
            }
            if (RadioactivityConstants.debugOverlay)
                Utils.Log("[OverlayLinkRenderer]: Rebuilt");
        }

        public void Update(bool complex)
        {
            if (complex)
                Rebuild();
            else 
                for (int i = 0; i < renderers.Count; i++)
                {
                    renderers[i].SetAttenuation();
                }
        }

        public void DestroyAll()
        {
            
            for (int i = 0; i < renderers.Count; i++)
            {
                renderers[i].Destroy();
            }
            renderers.Clear();
            if (RadioactivityConstants.debugOverlay)
                Utils.Log("[OverlayLinkRenderer]: Destroyed");
        }

        public void SetEnabled(bool on)
        {
            if (RadioactivityConstants.debugOverlay)
                Utils.Log("[OverlayLinkRenderer]: On/Off Toggle");
            drawn = on;
            for (int i = 0; i < renderers.Count; i++)
            {
                renderers[i].SetEnabled(on);
            }
        }


        /// <summary>
        /// Visualizes a zone of a radiation link
        /// </summary>
        public class OverlayLinkZone
        {
            bool drawn = false;

            GameObject go;
            LineRenderer renderer;

            RadiationLink link;
            AttenuationZone zone;

            public OverlayLinkZone(AttenuationZone zn, RadiationLink lnk, Transform parent)
            {
                zone = zn;
                link = lnk;
                CreateBasicRenderer(parent);
                SetAttenuation();
                SetPosition();
                if (RadioactivityConstants.debugOverlay)
                    Utils.Log("[OverlayLinkZone]: Initialized");
            }

            public void SetEnabled(bool on)
            {
                drawn = on;
                renderer.enabled = on;
            }
            public void SetAttenuation()
            {
                //Utils.Log(String.Format("{0} -> {1}", link.inputFlux*zone.attenuationIn, link.inputFlux * zone.attenuationOut));
                //Utils.Log(String.Format("{0} -> {1}",
                                        //RadioactivityConstants.overlayRayGradient.Evaluate((float)(link.inputFlux * zone.attenuationIn)),
                                        //RadioactivityConstants.overlayRayGradient.Evaluate((float)(link.inputFlux * zone.attenuationOut))));
                renderer.SetColors(RadioactivityConstants.overlayRayGradient.Evaluate((float)(link.inputFlux * zone.attenuationIn)/10000f ),
                                   RadioactivityConstants.overlayRayGradient.Evaluate((float)(link.inputFlux * zone.attenuationOut)/10000f ) );
            }
            public void SetPosition()
            {
                Vector3 zoneStart = link.source.EmitterTransform.position;
                Vector3 pVec = Vector3.Cross((zone.startPosition - zone.endPosition),
                                             Camera.main.transform.forward).normalized;

                renderer.useWorldSpace = true;
                renderer.SetPosition(0, zoneStart + (link.sink.SinkTransform.position - link.source.EmitterTransform.position).normalized * zone.dist1);
                renderer.SetPosition(1, zoneStart + (link.sink.SinkTransform.position - link.source.EmitterTransform.position).normalized * zone.dist2);
                renderer.useWorldSpace = false;
            }
            public void Destroy()
            {
                GameObject.Destroy(go);
            }

            protected void CreateBasicRenderer(Transform parent)
            {
                go = new GameObject("RadioactiveLinkRendererChild");
                go.transform.SetParent(parent, true);

                renderer = go.AddComponent<LineRenderer>();
                // Set up the material
                renderer.material = new Material(Shader.Find(RadioactivityConstants.overlayRayMaterial));
                renderer.material.color = Color.white;
                renderer.material.renderQueue = 3000;
                go.layer = 0;
                renderer.SetVertexCount(2);
                renderer.SetWidth(RadioactivityConstants.overlayRayWidthMin, RadioactivityConstants.overlayRayWidthMin);

            }
        }
    }

}
