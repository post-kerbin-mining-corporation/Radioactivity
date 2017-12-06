using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Radioactivity;
using Radioactivity.Simulator;

namespace Radioactivity.UI
{
    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public class RadioactivityOverlay : MonoBehaviour
    {
        
        public static RadioactivityOverlay Instance { get; private set; }

        protected bool drawn = false;

        protected List<OverlayShadowShieldRenderer> shieldRenderers;
        protected List<OverlayLinkRenderer> linkRenderers;

        public void SetEnabled(bool on)
        {
            if (on != drawn)
            {
                drawn = on;
                for (int i = 0; i < linkRenderers.Count; i++)
                {
                    linkRenderers[i].SetEnabled(drawn);
                }
                for (int i = 0; i < shieldRenderers.Count; i++)
                {
                    shieldRenderers[i].SetEnabled(drawn);
                }
            }

        }

        protected void Awake()
        {
            Instance = this;
            linkRenderers = new List<OverlayLinkRenderer>();
            shieldRenderers = new List<OverlayShadowShieldRenderer>();
        }

        protected void Start()
        {

        }

        protected void FixedUpdate()
        {
            if (HighLogic.LoadedSceneIsFlight || HighLogic.LoadedSceneIsEditor)
            {
                for (int i = 0; i < linkRenderers.Count; i++)
                {
                    linkRenderers[i].Update(false);
                }
                for (int i = 0; i < shieldRenderers.Count; i++)
                {
                    shieldRenderers[i].Update();
                }
            }
        }

        public void AddShadowShield(ShadowShield shield, RadioactiveSource src)
        {
            OverlayShadowShieldRenderer newShield = new OverlayShadowShieldRenderer(shield, src);
            newShield.SetEnabled(drawn);
            shieldRenderers.Add(newShield);
            if (RadioactivityConstants.debugOverlay)
                Utils.Log("[RadioactiveOverlay]: Adding visual shadow shield");
        }

        public void RemoveShadowShield(ShadowShield shield)
        {
            OverlayShadowShieldRenderer toRemove = shieldRenderers.FirstOrDefault(shld => shld.Shield == shield);
            if (toRemove != null)
            {
                toRemove.Destroy();
                shieldRenderers.Remove(toRemove);
                if (RadioactivityConstants.debugOverlay)
                    Utils.Log("[RadioactiveOverlay]: Removing visual shadow shield");
            }
        }

        public void AddLink(RadiationLink link)
        {
            OverlayLinkRenderer newLink = new OverlayLinkRenderer(link);
            newLink.SetEnabled(drawn);
            linkRenderers.Add(newLink);

            if (RadioactivityConstants.debugOverlay)
                Utils.Log("[RadioactiveOverlay]: Adding visual link");
        }

        public void RemoveLink(RadiationLink link)
        {
            OverlayLinkRenderer toRemove = linkRenderers.First(lnk => lnk.Link == link);
            toRemove.DestroyAll();

            linkRenderers.Remove(toRemove);
            if (RadioactivityConstants.debugOverlay)
                Utils.Log("[RadioactiveOverlay]: Removing visual link");
        }

        public void UpdateLink(RadiationLink link, bool complex)
        {
            OverlayLinkRenderer toUpdate = linkRenderers.First(lnk => lnk.Link == link);

            toUpdate.Update(complex);
            if (RadioactivityConstants.debugOverlay)
                Utils.Log("[RadioactiveOverlay]: Updating visual link");
        }




    }
}
