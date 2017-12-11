using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Radioactivity;
using Radioactivity.UI;

namespace Radioactivity.Simulator
{

    /// <summary>
    /// RadiationLink
    /// Represents a link between a RadioactiveSource and a RadioactiveSink
    /// </summary>
    public class RadiationLink
    {
        public RadioactiveSource source;
        public RadioactiveSink sink;

        public double fluxStart = 1.0d;
        public double fluxEndScale = 0.0d;

        public double inputFlux = 0d;

        public List<AttenuationZone> Path
        {
            get { return attenuationPath; }
        }

        public GameObject GO
        {
            get { return go; }
            set { go = value; }
        }

        public int ZoneCount
        {
            get { if (Path != null) return Path.Count; else return 0; }
        }

        public int OccluderCount
        {
            get
            {
                if (Path != null)
                {
                    int ct = 0;
                    for (int i=0; i < Path.Count ; i++)
                    {
                        if (Path[i].attenuationType != AttenuationType.Empty)
                            ct++;
                    }
                    return ct;
                }
                return 0;
            }
        }

        protected LayerMask raycastMask;

        protected bool needsGeometricRecalculation = true;
        protected bool needsSimpleRecalculation = true;

        protected Vector3 relPos;
        protected float connectionMass = 0f;
        protected List<LineRenderer> overlayPaths;
        protected GameObject go;
        protected List<AttenuationZone> attenuationPath = new List<AttenuationZone>();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Radioactivity.RadiationLink"/> class.
        /// </summary>
        /// <param name="src">Source.</param>
        /// <param name="snk">Snk.</param>
        public RadiationLink(RadioactiveSource src, RadioactiveSink snk)
        {
            fluxStart = RadioactivityConstants.defaultRaycastFluxStart;
            source = src;
            sink = snk;

            /// Set up the mask for casting
            LayerMask maskA = 1 << LayerMask.NameToLayer("Default");
            LayerMask maskB = 1 << LayerMask.NameToLayer("TerrainColliders");
            LayerMask maskC = 1 << LayerMask.NameToLayer("Local Scenery");
            raycastMask = maskA | maskB | maskC;

            needsGeometricRecalculation = true;
            needsSimpleRecalculation = true;
            //ComputeConnection(src, snk);
        }


        /// <summary>
        /// Simulate the radiation link
        /// Tests to see if 
        /// </summary>
        /// <returns>The simulate.</returns>
        /// <param name="timeScale">Time scale.</param>
        public void Simulate(float timeScale)
        {
            if (needsGeometricRecalculation)
            {
                ComputeGeometry(this.source, this.sink);
                RadioactivityOverlay.Instance.UpdateLink(this, true);
            }
            if (needsSimpleRecalculation)
            {
              fluxEndScale = AttenuateFlux(Path, 1.0f);
                RadioactivityOverlay.Instance.UpdateLink(this, false);
            }
            inputFlux = (double)source.CurrentEmission;

            sink.AddRadiation(source.SourceID, (float)(inputFlux * fluxEndScale));

        }

        /// <summary>
        /// Simulates the editor.
        /// </summary>
        /// <param name="timeScale">Time scale.</param>
        public void SimulateEditor(float timeScale)
        {
           
            if (needsGeometricRecalculation)
            {
                ComputeGeometry(this.source, this.sink);
                RadioactivityOverlay.Instance.UpdateLink(this, true);
            }
            if (needsSimpleRecalculation)
            {
                fluxEndScale = AttenuateFlux(Path, 1.0f);
                RadioactivityOverlay.Instance.UpdateLink(this, false);
            }
            inputFlux = (double)source.CurrentEmission;

            sink.AddRadiation(source.SourceID, (float)(inputFlux* fluxEndScale));

        }

        /// <summary>
        /// Cleanups the sink.
        /// </summary>
        public void CleanupSink()
        {
            sink.CleanupRadiation(source.SourceID);
        }

        /// <summary>
        /// Tests the recompute.
        /// </summary>
        public void TestRecompute()
        {

            if (source != null && sink != null && source.EmitterTransform != null && sink.SinkTransform != null)
            {
                // LOS needs to be recomputed if we're off by more than maximumPositionDelta.
                Vector3 curRelPos = PartUtils.getRelativePosition(source.EmitterTransform, sink.SinkTransform.position);
                if (((curRelPos - relPos).sqrMagnitude > RadioactivityConstants.maximumPositionDelta * RadioactivityConstants.maximumPositionDelta))
                {
                    LogUtils.Log("[RadiationLink]: Recalculating due to position differential");
                    needsGeometricRecalculation = true;
                }
                // LOS needs to be recomputed if mass is very different
                float curMass = GetConnectionMass();
                if (Mathf.Abs(connectionMass - curMass) > RadioactivityConstants.maximumMassDelta)
                {
                    LogUtils.Log("[RadiationLink]: Recalculating due to mass differential of " + Mathf.Abs(connectionMass - curMass).ToString());
                    needsSimpleRecalculation = true;
                }
            }
        }

        /// <summary>
        /// Gets the connection mass.
        /// </summary>
        /// <returns>The connection mass.</returns>
        protected float GetConnectionMass()
        {
            float m = 0f;
            for (int i = 0 ; i < Path.Count; i++)
            {

                if (Path[i].attenuationType == AttenuationType.ParameterizedPart || Path[i].attenuationType == AttenuationType.Part)
                {
                    if (Path[i].associatedPart != null && Path[i].associatedPart.Rigidbody != null)
                    {
                        m += (Path[i].associatedPart.mass + Path[i].associatedPart.GetResourceMass());
                    }
                }
            }
            return m;
        }

        /// <summary>
        /// Computes the geometry.
        /// </summary>
        /// <param name="src">Source.</param>
        /// <param name="target">Target.</param>
        public void ComputeGeometry(RadioactiveSource src, RadioactiveSink target)
        {
            if (RadioactivityConstants.debugNetwork)
                LogUtils.Log("Network: Creating connection from " + src.part.name + " to " + target.part.name);

            // Store the relative position of both endpoints
            relPos = PartUtils.getRelativePosition(src.EmitterTransform, target.SinkTransform.position);//src.EmitterTransform.position - target.SinkTransform.position;

            // Gets parts between source and sink
            attenuationPath = GetLineOfSight(src, target);

            // Attenuate the ray between these
            fluxStart = src.AttenuateShadowShields(target.SinkTransform.position- src.EmitterTransform.position);
            fluxEndScale = AttenuateFlux(attenuationPath, fluxStart);

            needsGeometricRecalculation = false;
        }


        /// <summary>
        /// Attenuates the flux.
        /// </summary>
        /// <returns>The flux.</returns>
        /// <param name="rayPath">Ray path.</param>
        /// <param name="strength">Strength.</param>
        protected double AttenuateFlux(List<AttenuationZone> rayPath, double strength)
        {
            // march along the ray, attenuating as we go
            double curFlux = strength;
            for (int i = 0; i < rayPath.Count; i++)
            {
                 curFlux = rayPath[i].Attenuate(curFlux);
            }
            // Get the total mass in the connection
            connectionMass = GetConnectionMass();
            needsSimpleRecalculation = false;
            return curFlux;
        }

        // Computes LOS between a source and a sink
        // Returns the 
        /// <summary>
        /// Computes the path between a source and a sink
        /// </summary>
        /// <returns>The line of sight.</returns>
        /// <param name="src">Source.</param>
        /// <param name="target">Target.</param>
        protected List<AttenuationZone> GetLineOfSight(RadioactiveSource src, RadioactiveSink target)
        {
            RaycastHit[] hits1;
            RaycastHit[] hits2;
            float sep = Vector3.Distance(src.EmitterTransform.position, target.SinkTransform.position);
            // Only cast against Default and Terrain


            // raycast from the source to target and vice versa
            hits1 = Physics.RaycastAll(src.EmitterTransform.position, target.SinkTransform.position - src.EmitterTransform.position, sep, raycastMask);
            hits2 = Physics.RaycastAll(target.SinkTransform.position, src.EmitterTransform.position - target.SinkTransform.position, sep, raycastMask);

            List<RaycastHit> hitsBackward = hits2.ToList();
            List<RaycastHit> hitsForward = hits1.ToList();

            /// sort by distance, ascending
            if (hitsForward.Count > 0)
            {

                hitsForward = hitsForward.OrderBy(o => o.distance).ToList();
            }
            if (hitsBackward.Count > 0)
            {
                hitsBackward = hitsBackward.OrderByDescending(o => o.distance).ToList();
            }

            return CreatePathway(hitsForward, hitsBackward, src, target, sep);

        }

        // Go through raycast results (both ways) in order to create the attenuation path
        protected List<AttenuationZone> CreatePathway(List<RaycastHit> outgoing, List<RaycastHit> incoming, RadioactiveSource src, RadioactiveSink target, float totalPathLength)
        {
            List<AttenuationZone> attens = new List<AttenuationZone>();
            if (RadioactivityConstants.debugRaycasting)
                LogUtils.Log("[RadiationLink]: Looking along a distance of " +totalPathLength.ToString() + " with "+ outgoing.Count +  " hits");
            float curZoneStartDistance = RadioactivityConstants.defaultSourceFluxDistance;
            float curZoneEndDistance = 0.01f;
            Vector3 curZoneStartPoint = src.EmitterTransform.position + (target.SinkTransform.position - src.EmitterTransform.position).normalized*curZoneStartDistance;
            Vector3 curZoneEndPoint = target.SinkTransform.position;

            int hitNum = 0;
            // for each object we hit outgoing, see if we found it incoming
            for (int i=0; i < outgoing.Count; i++)
            {
                if (RadioactivityConstants.debugRaycasting)
                    LogUtils.Log("[RadiationLink]: Looking for incoming rayhits with " + outgoing[i].collider.name);

                RaycastHit found = incoming.Find(item => item.collider == outgoing[i].collider);

                // If there is a matching collider
                if (found.collider != null)
                {
                    curZoneEndDistance = outgoing[i].distance;
                    curZoneEndPoint = outgoing[i].point;

                    if (curZoneEndDistance - curZoneStartDistance > 0f)
                    {
                        attens.Add(new AttenuationZone(curZoneStartDistance, curZoneEndDistance, curZoneStartPoint, curZoneEndPoint));
                        curZoneStartPoint = curZoneEndPoint;
                        curZoneStartDistance = curZoneEndDistance;
                    }

                    int layer = found.collider.gameObject.layer;
                    if (RadioactivityConstants.debugRaycasting)
                        LogUtils.Log("[RadiationLink]: Hit on layer " + LayerMask.LayerToName(layer));

                    if (layer == LayerMask.NameToLayer("Default"))
                    {
                        Part associatedPart = outgoing[i].collider.GetComponentInParent<Part>();
                        if (RadioactivityConstants.debugRaycasting)
                            LogUtils.Log("[RadiationLink]: Located 2-way hit! Path through is of L: " + (totalPathLength - outgoing[i].distance - found.distance).ToString() + " and part is " + associatedPart.ToString());
                        if (associatedPart != target.part)
                        {

                            curZoneStartPoint = outgoing[i].point;
                            curZoneEndPoint = found.point;
                            curZoneStartDistance = outgoing[i].distance;
                            curZoneEndDistance = totalPathLength - found.distance;

                            attens.Add(new AttenuationZone(curZoneStartDistance, curZoneEndDistance, associatedPart, curZoneStartPoint, curZoneEndPoint));
                        }
                    }
                    if (found.collider.gameObject.layer == LayerMask.NameToLayer("TerrainColliders"))
                    {
                        if (RadioactivityConstants.debugRaycasting)
                            LogUtils.Log("[RadiationLink]: Located 2-way hit! Path through is of L: " + (totalPathLength - outgoing[i].distance - found.distance).ToString() + " on terraincolliders layer");
                        curZoneStartPoint = outgoing[i].point;
                        curZoneEndPoint = found.point;
                        curZoneStartDistance = outgoing[i].distance;
                        curZoneEndDistance = totalPathLength - found.distance;
                        attens.Add(new AttenuationZone(outgoing[i].distance, totalPathLength - found.distance, AttenuationType.Terrain, outgoing[i].point, found.point));
                    }
                    if (found.collider.gameObject.layer == LayerMask.NameToLayer("Local Scenery"))
                    {
                        if (RadioactivityConstants.debugRaycasting)
                            LogUtils.Log("[RadiationLink]: Located 2-way hit! Path through is of L: " + (totalPathLength - outgoing[i].distance - found.distance).ToString() + " on LocalScenery layer");
                        curZoneStartPoint = outgoing[i].point;
                        curZoneEndPoint = found.point;
                        curZoneStartDistance = outgoing[i].distance;
                        curZoneEndDistance = totalPathLength - found.distance;
                        attens.Add(new AttenuationZone(outgoing[i].distance, totalPathLength - found.distance, AttenuationType.Terrain, outgoing[i].point, found.point));
                    }
                    hitNum++;
                }
                else
                {
                    if (RadioactivityConstants.debugRaycasting)
                        LogUtils.Log("[RadiationLink]: No incoming hits with " + outgoing[i].collider.name + ", discarding...");
                }


            }

            curZoneEndPoint = target.SinkTransform.position;
            if (hitNum > 0)
                curZoneStartDistance = curZoneEndDistance;
            curZoneEndDistance = totalPathLength;
            attens.Add(new AttenuationZone(curZoneStartDistance, curZoneEndDistance, curZoneStartPoint, curZoneEndPoint));

            // TODO: Need to add another AttenuationZone if we start inside a part. probably take the last hit in the incoming array to do this (not the best assumption)
            // TODO: Need to add another AttenuationZone to account for the target part. probably take the last hit in the outgoing array to do this (good assumption)

            // Add the empty space as a single zone
            //if (totalPathLength > 0f)
            //    attens.Add(new AttenuationZone(totalPathLength, src.EmitterTransform.position, target.SinkTransform.position));

            return attens;
        }
    }
}
