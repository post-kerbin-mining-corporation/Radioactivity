using System;
using 

namespace Radioactivity.Simulator
{
    /// <summary>
    /// The Vessel Simulator calculates vessel-wide parameters
    /// </summary>
    public class VesselSimulator: VesselModule
    {
        protected override void OnStart()
        {   
        }

        void OnDestroy()
        {
            
        }

        void FixedUpdate()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                
            }
        }
    }
}
