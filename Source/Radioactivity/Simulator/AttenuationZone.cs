using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Radioactivity
{
    /// <summary>
    /// Represents an attenuation zone, where radiation is attenuated by passing through it
    /// </summary>
    public class AttenuationZone
    {
        public AttenuationType attenuationType = AttenuationType.Empty;
        public Part associatedPart;
        public RadiationParameters parameters;
        public float dist1 = 0f;
        public float dist2 = 0f;
        public float initialD = 1f;
        public float volume = 1f;
        public float density = 0.0001f;
        public double attenuationCoeff = 1d;

        public double attenuationIn = 1d;
        public double attenuationOut = 1d;

        public Vector3 startPosition = Vector3.zero;
        public Vector3 endPosition = Vector3.one;

        /// <summary>
        /// Build a new empty attenuation zone that only attenuates with distance
        /// </summary>
        /// <param name="d1">the distance from the source at the start </param>
        /// <param name="d2">the distance from the source at the end</param>
        /// <param name="start">Start position </param>
        /// <param name="end">End position</param>
        public AttenuationZone(float d1, float d2, Vector3 start, Vector3 end)
        {
            attenuationType = AttenuationType.Empty;
            dist1 = d1;
            dist2 = d2;
            startPosition = start;
            endPosition = end;
        }

        /// <summary>
        /// Build a new attenuation zone that is a part
        /// </summary>
        /// <param name="d1">the distance from the source at the start </param>
        /// <param name="d2">the distance from the source at the end</param>
        /// <param name="part">the Part which defines the Zone</param>
        /// <param name="start">Start position </param>
        /// <param name="end">End position</param>

        public AttenuationZone(float d1, float d2, Part part, Vector3 start, Vector3 end)
        {
            dist1 = d1;
            dist2 = d2;

            // Need to recalculate cubes in editor
            if (HighLogic.LoadedSceneIsEditor)
                part.DragCubes.SetDragWeights();
            
            volume = Utils.GetDisplacement(part);
            density = (part.mass + part.GetResourceMass()) / volume;

            parameters = part.GetComponent<RadiationParameters>();
            if (parameters != null)
            {
                density = parameters.Density;
                attenuationCoeff = (double)parameters.AttenuationCoefficient;
            } else 
            {
                attenuationCoeff = (double)RadioactivityConstants.defaultPartAttenuationCoefficient;
                attenuationType = AttenuationType.Part;
                associatedPart = part;
            }
            startPosition = start;
            endPosition = end;
        }

        /// <summary>
        /// Build a new attenuation zone that is opaque terrain
        /// </summary>
        /// <param name="d1">D1.</param>
        /// <param name="d2">D2.</param>
        /// <param name="tp">Tp.</param>
        /// <param name="start">Start.</param>
        /// <param name="end">End.</param>
        public AttenuationZone(float d1, float d2, AttenuationType tp, Vector3 start, Vector3 end)
        {
            dist1 = d1;
            dist2 = d2;
            attenuationType = AttenuationType.Terrain;
            startPosition = start;
            endPosition = end;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:Radioactivity.AttenuationZone"/>.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:Radioactivity.AttenuationZone"/>.</returns>
        public override string ToString()
        {
            string data = "";
            data += "Type: " + attenuationType.ToString();
            data += ", Length: " + (dist2-dist1).ToString();

            if (attenuationType == AttenuationType.ParameterizedPart || attenuationType == AttenuationType.Part)
            {
                data += ", Part Name: " + associatedPart.name;
                data += ", Part Density: " + density.ToString();
                data += ", Part Volume: " + volume.ToString();
                data += ", Attenuation Coefficient: " + attenuationCoeff.ToString();
            }
            if (attenuationType == AttenuationType.Terrain)
            {
                data += ", Terrain";
            }

            data += "\n In Flux (fraction): " + attenuationIn.ToString();
            data += "\n Out Flux (fraction): " + attenuationOut.ToString();

            return data;
        }

        /// <summary>
        /// Calculates attenuation for this zone 
        /// </summary>
        /// <returns>The output flux of the zone</returns>
        /// <param name="inStrength">The input flux of the zone</param>
        public double Attenuate(double inStrength)
        {
            attenuationIn = inStrength;
            if (attenuationType == AttenuationType.Empty)
            {
                // attenuate radiation only by inverse square
                //attenuationTotal = (inStrength) / (double)(this.size * this.size);
                attenuationOut = attenuationIn* (dist1*dist1)/ (dist2*dist2);
            }
            if (attenuationType == AttenuationType.Part)
            {
                density = (associatedPart.mass + associatedPart.GetResourceMass()) / volume;
                double atten = attenuationIn * (dist1 * dist1) / (dist2 * dist2);
                // TODO: as in ParameterizedPart
                // attenuate the distance
                //double distScale = inStrength / (double)(this.size * this.size);
                double materialScale = Math.Exp(-1d * (double)((associatedPart.mass + associatedPart.GetResourceMass()) / volume * (dist2 - dist1)) * attenuationCoeff);

                attenuationOut = materialScale * atten ;
                // i0*e^(-ux), x = thickness (cm), u = linear attenuation coeff (cm-1). u values:
                // Al: 13, Pb: 82, W: 74, Fe: 26 -> need to be mult by density in g/cm3
            }
            if (attenuationType == AttenuationType.Terrain)
            {
                attenuationOut = 0d;
            }
            return attenuationOut;
        }
    }


}
