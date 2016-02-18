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

        public double fluxStart = 1.0f;
        public double fluxEndScale = 0.0f;

        public bool overlayShown = false;
        public List<AttenuationZone> Path
        {
            get { return attenuationPath; }
        }

        public LineRenderer OverlayPath
        {
            get { return overlayPath; }
            set { overlayPath = value; }
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
                    foreach (AttenuationZone z in Path)
                    {
                        if (z.attenuationType != AttenuationType.Empty)
                            ct++;
                    }
                    return ct;
                }
                return 0;
            }
        }

        protected Vector3 relPos;
        protected LineRenderer overlayPath;
        protected GameObject go;
        protected List<AttenuationZone> attenuationPath;

        public RadiationLink(RadioactiveSource src, RadioactiveSink snk)
        {
            fluxStart = RadioactivitySettings.defaultRaycastFluxStart;
            source = src;
            sink = snk;
            ComputeConnection(src, snk);
        }

        // Hide or show the overlay for this link
        public void ToggleOverlay()
        {
            overlayShown = !overlayShown;
            if (overlayShown)
                ShowOverlay();
            else
                HideOverlay();
        }
        public void ShowOverlay()
        {
            overlayShown = true;
            RadioactivityOverlay.Instance.Show(this);
        }
        public void HideOverlay()
        {
            overlayShown = false;
            RadioactivityOverlay.Instance.Hide(this);
        }

        // Simulate the link, that is, compute the flux from the source and add it to the sink
        public void Simulate(float timeScale)
        {
            sink.AddRadiation((float)((double)source.CurrentEmission * timeScale * fluxEndScale));
        }

        // Tests to see whether the LOS needs to be recomputed
        public void Recompute()
        {
            // LOS needs to be recomputed if we're off by more than maximumPositionDelta.
            // TODO: Needs to be recomputed if the total mass changes (propellant loss)
            // TODO: BUT that is more complicated!!
            Vector3 curRelPos = source.EmitterTransform.position - sink.SinkTransform.position;
            if (((curRelPos - relPos).sqrMagnitude > RadioactivitySettings.maximumPositionDelta))
            {
                ComputeConnection(source, sink);
            }
        }
        // Compute the ray path between the source and sink
        protected void ComputeConnection(RadioactiveSource src, RadioactiveSink target)
        {
            if (RadioactivitySettings.debugNetwork)
                Utils.Log("Creating connection from " + src.part.name + " to " + target.part.name);

            // Store the relative position of both endpoints
            relPos = src.EmitterTransform.position - target.SinkTransform.position;

            // Gets parts between source and sink
            attenuationPath = GetLineOfSight(src, target);
            // Attenuate the ray between these
            fluxEndScale = AttenuateFlux(attenuationPath, fluxStart);
        }

        // Attenuates the ray between the source and sink
        protected double AttenuateFlux(List<AttenuationZone> rayPath, double strength)
        {
            // march along the ray, attenuating as we go
            double curFlux = strength;
            foreach (AttenuationZone z in rayPath)
            {
                if (curFlux >= RadioactivitySettings.fluxCutoff)
                {
                    curFlux = z.Attenuate(curFlux);
                }
            }
            return curFlux;
        }

        // Computes LOS between a source and a sink
        // Returns the list of parts between the two objects
        protected List<AttenuationZone> GetLineOfSight(RadioactiveSource src, RadioactiveSink target)
        {
            RaycastHit[] hits1;
            RaycastHit[] hits2;
            float sep = Vector3.Distance(src.EmitterTransform.position, target.SinkTransform.position);
            // Only cast against Default
            LayerMask mask = 1 << LayerMask.NameToLayer("Default");

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

            if (hitsForward.Count > 0 && hitsBackward.Count > 0)
                return CreatePathway(hitsForward, hitsBackward, sep);
            else
                return null;
        }

        // Go through raycast results (both ways) in order to create the attenuation path
        protected List<AttenuationZone> CreatePathway(List<RaycastHit> outgoing, List<RaycastHit> incoming, float totalPathLength)
        {
            List<AttenuationZone> attens = new List<AttenuationZone>();
            if (RadioactivitySettings.debugRaycasting)
                Utils.Log("Raycaster: Looking along a distance of " +totalPathLength.ToString() + " with "+ outgoing.Count +  " hits");
            float absorbedLength = 0f;
            int hitNum = 0;
            // for each object we hit outgoing, see if we found it incoming
            foreach (RaycastHit h in outgoing)
            {
                
                if (RadioactivitySettings.debugRaycasting)
                    Utils.Log("Raycaster: Looking for incoming rayhits with " + h.collider.name);
                RaycastHit found = incoming.Find(item => item.collider == h.collider);
                if (found.collider != null)
                {
                    Part associatedPart = h.collider.GetComponentInParent<Part>();
                    if (RadioactivitySettings.debugRaycasting)
                        Utils.Log("Raycaster: Located 2-way hit! Path through is of L: " + (totalPathLength - h.distance - found.distance).ToString() + " and part is " + associatedPart.ToString());
                    absorbedLength +=  (totalPathLength - h.distance - found.distance);
                    attens.Add(new AttenuationZone(totalPathLength - h.distance - found.distance, associatedPart));
                }
                else
                {
                    if (RadioactivitySettings.debugRaycasting)
                        Utils.Log("Raycaster: No incoming hits with " + h.collider.name + ", discarding...");
                }
               
                hitNum++;
            }
            // TODO: Need to add another AttenuationZone if we start inside a part. probably take the last hit in the incoming array to do this (not the best assumption)
            // TODO: Need to add another AttenuationZone to account for the target part. probably take the last hit in the outgoing array to do this (good assumption)

            // Add the empty space as a single zone
            if (totalPathLength - absorbedLength > 0f)
                attens.Add(new AttenuationZone(totalPathLength - absorbedLength));

            return attens;
        }
    }
}
