using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Radioactivity
{
    // Represents an attenuation zone, where radiation is attenuated by passing through it
    public class AttenuationZone
    {
        public AttenuationType attenuationType;
        public Part associatedPart;
        public ModuleRadiationParameters parameters;
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

        public AttenuationZone(float d1, float d2, Vector3 start, Vector3 end)
        {
            attenuationType = AttenuationType.Empty;
            dist1 = d1;
            dist2 = d2;
            startPosition = start;
            endPosition = end;
        }
        public AttenuationZone(float d1, float d2, Part part, Vector3 start, Vector3 end)
        {
            dist1 = d1;
            dist2 = d2;
            volume = Utils.GetDisplacement(part);
            if (part.Rigidbody != null)
                density = part.Rigidbody.mass / volume;
            else
                density = part.mass / volume;

            attenuationCoeff = (double)RadioactivitySettings.defaultPartAttenuationCoefficient;
            attenuationType = AttenuationType.Part;
            associatedPart = part;
            parameters = part.GetComponent<ModuleRadiationParameters>();
            if (parameters != null)
            {
                attenuationType = AttenuationType.ParameterizedPart;
                density = parameters.Density;
                attenuationCoeff = (double)parameters.AttenuationCoefficient;
            }
            startPosition = start;
            endPosition = end; 
        }

        public AttenuationZone(float d1, float d2, AttenuationType tp, Vector3 start, Vector3 end)
        {
            dist1 = d1;
            dist2 = d2;
            attenuationType = AttenuationType.Terrain;
            startPosition = start;
            endPosition = end;
        }
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
        public double Attenuate(double inStrength)
        {
            attenuationIn = inStrength;
            if (attenuationType == AttenuationType.Empty)
            {
                // attenuate radiation only by inverse square
                //attenuationTotal = (inStrength) / (double)(this.size * this.size);
                attenuationOut = attenuationIn* (dist1*dist1)/ (dist2*dist2);
            }
            if (attenuationType == AttenuationType.ParameterizedPart)
            {
                double atten = attenuationIn* (dist1*dist1)/ (dist2*dist2);
                // Note that size is in m, and attenuationCoeff is in m-1
                // TODO: Change this to use the mass attenuation coeffecient in g/cm2. Currently we use attenuation coeff in cm-1
                // Need -(u/p) * p*l , where p = density in g/cm3 and l=path length
                // So atten * Mathf.Exp (-density*this.size * massAttenuationCoeff);
                //attenuationOut = atten * Math.Exp(-1d * (double)(dist2-dist1) * attenuationCoeff);
                attenuationOut = atten * Math.Exp(-1d * density * (double)(dist2 - dist1) * attenuationCoeff);
            }
            if (attenuationType == AttenuationType.Part)
            {
                double atten = attenuationIn * (dist1 * dist1) / (dist2 * dist2);
                // TODO: as in ParameterizedPart
                // attenuate the distance
                //double distScale = inStrength / (double)(this.size * this.size);
                double materialScale = Math.Exp(-1d * density * (double)(dist2 - dist1) * attenuationCoeff);

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
