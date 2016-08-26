using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Radioactivity
{

    // Represents a link between a radiation source and a radiation sink
    public class RadiationLink
    {
        public RadioactiveSource source;
        public RadioactiveSink sink;

        public double fluxStart = 1.0d;
        public double fluxEndScale = 0.0d;

        public bool overlayShown = false;
        public List<AttenuationZone> Path
        {
            get { return attenuationPath; }
        }

        public List<LineRenderer> OverlayPaths
        {
            get { return overlayPaths; }
            set { overlayPaths = value; }
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

        protected bool needsGeometricRecalculation = true;
        protected bool needsSimpleRecalculation = true;
        protected Vector3 relPos;
        protected float connectionMass = 0f;
        protected List<LineRenderer> overlayPaths;
        protected GameObject go;
        protected List<AttenuationZone> attenuationPath = new List<AttenuationZone>();

        public RadiationLink(RadioactiveSource src, RadioactiveSink snk)
        {
            fluxStart = RadioactivitySettings.defaultRaycastFluxStart;
            source = src;
            sink = snk;
            if (Radioactivity.Instance.RayOverlayShown)
                ShowOverlay();
            //ComputeConnection(src, snk);
        }

        // Hide or show the overlay for this link
        public void ToggleOverlay()
        {
            if (overlayShown)
               HideOverlay();
            else
                ShowOverlay();
        }
        public void ShowOverlay()
        {
            if (!overlayShown)
            {
                overlayShown = true;
                RadioactivityOverlay.Instance.Show(this);
            }
        }
        public void HideOverlay()
        {
            if (overlayShown)
            {
                overlayShown = false;
                RadioactivityOverlay.Instance.Hide(this);
            }
        }

        // Simulate the link, that is, compute the flux from the source and add it to the sink
        public void Simulate(float timeScale)
        {
            TestRecompute();
            if (needsGeometricRecalculation)
            {
                RadioactivityOverlay.Instance.Update(this);
                ComputeGeometry(this.source, this.sink);
            }
            if (needsSimpleRecalculation)
            {
                RadioactivityOverlay.Instance.Update(this);
                fluxEndScale = AttenuateFlux(Path, 1.0f);
            }

            sink.AddRadiation(source.SourceID, (float)((double)source.CurrentEmission * fluxEndScale));
        }

        // Simulate the link in the editor
        public void SimulateEditor(float timeScale)
        {
            TestRecompute();
            if (needsGeometricRecalculation)
            {
                RadioactivityOverlay.Instance.Update(this);
                ComputeGeometry(this.source, this.sink);
            }
            if (needsSimpleRecalculation)
            {
                RadioactivityOverlay.Instance.Update(this);
                fluxEndScale = AttenuateFlux(Path, 1.0f);
            }

            sink.AddRadiation(source.SourceID, (float)((double)source.CurrentEmission * fluxEndScale));
        }

        // Tests to see whether the LOS needs to be recomputed
        public void TestRecompute()
        {

            if (source != null && sink != null && source.EmitterTransform != null && sink.SinkTransform != null)
            {
                // LOS needs to be recomputed if we're off by more than maximumPositionDelta.
                Vector3 curRelPos = Utils.getRelativePosition(source.EmitterTransform, sink.SinkTransform.position);
                if (((curRelPos - relPos).sqrMagnitude > RadioactivitySettings.maximumPositionDelta * RadioactivitySettings.maximumPositionDelta))
                {
                    Utils.Log("Raycaster: Recalculating due to position differential");
                    needsGeometricRecalculation = true;
                }
                // LOS needs to be recomputed if mass is very different
                float curMass = GetConnectionMass();
                if (Mathf.Abs(connectionMass - curMass) > RadioactivitySettings.maximumMassDelta)
                {
                    Utils.Log("Raycaster: Recalculating due to mass differential of " + Mathf.Abs(connectionMass - curMass).ToString());
                    needsSimpleRecalculation = true;
                }
            }
        }
        protected float GetConnectionMass()
        {
            float m = 0f;
            for (int i = 0 ; i < Path.Count; i++)
            {

                if (Path[i].attenuationType == AttenuationType.ParameterizedPart || Path[i].attenuationType == AttenuationType.Part)
                {
                    if (Path[i].associatedPart != null && Path[i].associatedPart.Rigidbody != null)
                    {
                        m += Path[i].associatedPart.Rigidbody.mass;
                    }
                }
            }
            return m;
        }
        // Compute the ray path between the source and sink
        public void ComputeGeometry(RadioactiveSource src, RadioactiveSink target)
        {
            if (RadioactivitySettings.debugNetwork)
                Utils.Log("Creating connection from " + src.part.name + " to " + target.part.name);

            // Store the relative position of both endpoints
            relPos = Utils.getRelativePosition(src.EmitterTransform, target.SinkTransform.position);//src.EmitterTransform.position - target.SinkTransform.position;

            // Gets parts between source and sink
            attenuationPath = GetLineOfSight(src, target);

            // Attenuate the ray between these
            fluxStart = src.AttenuateShadowShields(target.SinkTransform.position- src.EmitterTransform.position);
            fluxEndScale = AttenuateFlux(attenuationPath, fluxStart);

            needsGeometricRecalculation = false;
        }


        // Attenuates the ray between the source and sink
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
        // Returns the list of parts between the two objects
        protected List<AttenuationZone> GetLineOfSight(RadioactiveSource src, RadioactiveSink target)
        {
            RaycastHit[] hits1;
            RaycastHit[] hits2;
            float sep = Vector3.Distance(src.EmitterTransform.position, target.SinkTransform.position);
            // Only cast against Default and Terrain
            LayerMask mask;
            LayerMask maskA = 1 << LayerMask.NameToLayer("Default");
            LayerMask maskB = 1 << LayerMask.NameToLayer("TerrainColliders");
            LayerMask maskC = 1 << LayerMask.NameToLayer("Local Scenery");

            mask = maskA | maskB | maskC;

            // raycast from the source to target and vice versa
            hits1 = Physics.RaycastAll(src.EmitterTransform.position, target.SinkTransform.position - src.EmitterTransform.position, sep, mask);
            hits2 = Physics.RaycastAll(target.SinkTransform.position, src.EmitterTransform.position - target.SinkTransform.position, sep, mask);

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
            if (RadioactivitySettings.debugRaycasting)
                Utils.Log("Raycaster: Looking along a distance of " +totalPathLength.ToString() + " with "+ outgoing.Count +  " hits");
            float curZoneStartDistance = RadioactivitySettings.defaultSourceFluxDistance;
            float curZoneEndDistance = 0.01f;
            Vector3 curZoneStartPoint = src.EmitterTransform.position;
            Vector3 curZoneEndPoint = target.SinkTransform.position;

            int hitNum = 0;
            // for each object we hit outgoing, see if we found it incoming
            for (int i=0; i < outgoing.Count; i++)
            {
                if (RadioactivitySettings.debugRaycasting)
                    Utils.Log("Raycaster: Looking for incoming rayhits with " + outgoing[i].collider.name);

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
                    if (RadioactivitySettings.debugRaycasting)
                        Utils.Log("Raycaster: Hit on layer " + LayerMask.LayerToName(layer));

                    if (layer == LayerMask.NameToLayer("Default"))
                    {
                        Part associatedPart = outgoing[i].collider.GetComponentInParent<Part>();
                        if (RadioactivitySettings.debugRaycasting)
                            Utils.Log("Raycaster: Located 2-way hit! Path through is of L: " + (totalPathLength - outgoing[i].distance - found.distance).ToString() + " and part is " + associatedPart.ToString());
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
                        if (RadioactivitySettings.debugRaycasting)
                            Utils.Log("Raycaster: Located 2-way hit! Path through is of L: " + (totalPathLength - outgoing[i].distance - found.distance).ToString() + " on terraincolliders layer");
                        curZoneStartPoint = outgoing[i].point;
                        curZoneEndPoint = found.point;
                        curZoneStartDistance = outgoing[i].distance;
                        curZoneEndDistance = totalPathLength - found.distance;
                        attens.Add(new AttenuationZone(outgoing[i].distance, totalPathLength - found.distance, AttenuationType.Terrain, outgoing[i].point, found.point));
                    }
                    if (found.collider.gameObject.layer == LayerMask.NameToLayer("Local Scenery"))
                    {
                        if (RadioactivitySettings.debugRaycasting)
                            Utils.Log("Raycaster: Located 2-way hit! Path through is of L: " + (totalPathLength - outgoing[i].distance - found.distance).ToString() + " on LocalScenery layer");
                        curZoneStartPoint = outgoing[i].point;
                        curZoneEndPoint = found.point;
                        curZoneStartDistance = outgoing[i].distance;
                        curZoneEndDistance = totalPathLength - found.distance;
                        attens.Add(new AttenuationZone(outgoing[i].distance, totalPathLength - found.distance, AttenuationType.Terrain, outgoing[i].point, found.point));
                    }
                }
                else
                {
                    if (RadioactivitySettings.debugRaycasting)
                        Utils.Log("Raycaster: No incoming hits with " + outgoing[i].collider.name + ", discarding...");
                }

                hitNum++;
            }

            curZoneEndPoint = target.SinkTransform.position;
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
