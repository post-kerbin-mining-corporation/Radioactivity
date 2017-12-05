using System;
using UnityEngine;

namespace Radioactivity.UI
{
    public class ShadowShieldRenderer
    {
        public ShadowShieldRenderer()
        {
        }

        protected void SetupShadowShieldRenderer(ShadowShieldEffect shld, RadioactiveSource parent)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            shld.renderer = go;
            //Destroy(go.GetComponent<Collider>());
            go.transform.parent = parent.part.partTransform;
            go.transform.localPosition = shld.localPosition;
            go.transform.localScale = shld.dimensions;

            go.transform.up = parent.EmitterTransform.position - go.transform.position;

            MeshRenderer m = go.GetComponent<MeshRenderer>();
            m.material = new Material(Shader.Find(RadioactivityConstants.overlayRayMaterial));
            m.material.color = Color.blue;
            m.material.renderQueue = 3000;

            if (RadioactivityConstants.debugOverlay)
                Utils.Log("Overlay: Showing shadow shield on " + parent.SourceID);

        }
        protected void DestroyShadowShieldRenderer(GameObject shld)
        {
            GameObject.Destroy(shld);
        }

    }
}
