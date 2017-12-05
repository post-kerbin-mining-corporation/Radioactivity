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

        protected List<OverlayRenderer> linkRenderers;

        public void SetEnabled(bool on)
        {
            drawn = on;

        }

        protected void Awake()
        {
            Instance = this;
            linkRenderers = new List<OverlayRenderer>();
        }

        protected void Start()
        {

        }

        protected void FixedUpdate()
        {
            if (HighLogic.LoadedSceneIsFlight || HighLogic.LoadedSceneIsEditor)
            {
                
            }
        }

        protected void MonitorRadiationLinks()
        {
            //for (int i = 0; i < )
        }




    }
}
