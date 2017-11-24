using System;
namespace Radioactivity.Simulator
{
    /// <summary>
    /// The Vessel Simulator calculates vessel-wide parameters
    /// </summary>
    public class VesselSimulator: VesselModule
    {


        public double SkyViewFactor
        { 
            get { return svf; }
        }
        public double GroundViewFactor
        {
            get { return 1d - svf; }
        }

        double svf;



      bool simulationReady = false;
        bool rayOverlayShown = false;
        bool pointRadiationNetworkChanged = false;

        // Add a radiation source to the source list
        public void RegisterSource(RadioactiveSource src)
        {
            allRadSources.Add(src);
            BuildNewRadiationLink(src);
            pointRadiationNetworkChanged = true;
            if (rayOverlayShown && src.ShadowShields.Count > 0)
                RadioactivityOverlay.Instance.ShowShield(src);

            if (RadioactivitySettings.debugNetwork)
                Utils.Log("Network: Adding radiation source " + src.SourceID + " on part " + src.part.name + " to simulator");
        }

        // Remove a radiation source from the source list
        public void UnregisterSource(RadioactiveSource src)
        {
            if (allRadSources.Count > 0)
            {
                pointRadiationNetworkChanged = true;
                RemoveRadiationLink(src);
                allRadSources.Remove(src);
                if (rayOverlayShown && src.ShadowShields.Count > 0)
                    RadioactivityOverlay.Instance.HideShield(src);

                if (RadioactivitySettings.debugNetwork && src != null)
                    Utils.Log("Network: Removing radiation source " + src.SourceID + " on part " + src.part.name + " from simulator");
            }
        }
        // Add a radiation sink to the sink list
        public void RegisterSink(RadioactiveSink snk)
        {

            allRadSinks.Add(snk);
            BuildNewRadiationLink(snk);
            pointRadiationNetworkChanged = true;
            if (RadioactivitySettings.debugNetwork)
                Utils.Log("Network: Adding radiation sink " + snk.SinkID + " on part " + snk.part.name + " to simulator");
        }
        // Remove a radiation sink from the sink list
        public void UnregisterSink(RadioactiveSink snk)
        {
            if (allRadSinks.Count > 0)
            {
                pointRadiationNetworkChanged = true;
                RemoveRadiationLink(snk);
                allRadSinks.Remove(snk);

                if (RadioactivitySettings.debugNetwork && snk != null)
                    Utils.Log("Network: Removing radiation sink " + snk.SinkID + " on part " + snk.part.name + " from simulator");
            }
        }

    }
}
