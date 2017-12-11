using System;
using UnityEngine;
using Radioactivity.Simulator;

namespace Radioactivity.UI
{
    public class OverlayShadowShieldRenderer:OverlayRenderer
    {
        public ShadowShield Shield { get { return shield; } }

        protected RadioactiveSource source;
        protected ShadowShield shield;
        protected GameObject go;
        protected MeshRenderer renderer;

        public OverlayShadowShieldRenderer(ShadowShield shld, RadioactiveSource parent)
        {
            shield = shld;
            source = parent;
            CreateRenderer();
            if (RadioactivityConstants.debugOverlay)
                LogUtils.Log("[OverlayShadowShieldRenderer]: Initialized");
        }

        public void CreateRenderer()
        {
            go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);

            UnityEngine.Object.DestroyImmediate(go.GetComponent<Collider>(), false);

            go.transform.SetParent(source.part.partTransform, true);
            go.transform.localPosition = shield.localPosition;
            go.transform.localScale = shield.dimensions;
            go.transform.up = source.EmitterTransform.position - go.transform.position;
            go.layer = 0;
            renderer = go.GetComponent<MeshRenderer>();
            renderer.material = new Material(Shader.Find(RadioactivityConstants.overlayRayMaterial));
            renderer.material.color = Color.blue;
            renderer.material.renderQueue = 2998;

        }
        public void Update()
        {}

        public void SetEnabled(bool on)
        {
            drawn = on;
            renderer.enabled = on;
        }

        public void Destroy()
        {
            GameObject.Destroy(go);
        }


    }
}
