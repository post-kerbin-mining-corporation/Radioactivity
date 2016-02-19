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
        public float size = 1f;
        public float density = 0.0001f;
        public double attenuationCoeff = 1d;

        public AttenuationZone(float sz)
        {
            attenuationType = AttenuationType.Empty;
            size = sz;
        }
        public AttenuationZone(float sz, Part part)
        {
            size = sz;
            density = Utils.GetDensity(part);
            attenuationCoeff = (double)RadioactivitySettings.defaultPartAttenuationCoefficient / 100d;
            attenuationType = AttenuationType.Part;
            associatedPart = part;
            parameters = part.GetComponent<ModuleRadiationParameters>();
            if (parameters != null)
            {
                attenuationType = AttenuationType.ParameterizedPart;
                density = parameters.Density;
                attenuationCoeff = (double)parameters.AttenuationCoefficient / 100d;
            }
        }
        public override string ToString()
        {
            string data = "";
            data += "T: " + attenuationType.ToString();
            data += ", L: " + size.ToString();

            if (attenuationType == AttenuationType.ParameterizedPart || attenuationType == AttenuationType.Part)
            {
                data += ", PName: " + associatedPart.name;
                data += ", PDens: " + density.ToString();
                data += ", PCoeff: " + attenuationCoeff.ToString();
            }

            data += " => A: " + Attenuate(1d).ToString();

            return data;
        }
        public double Attenuate(double inStrength)
        {
            if (attenuationType == AttenuationType.Empty)
            {
                // attenuate radiation only by inverse square
                return (inStrength) / (double)(this.size * this.size);
            }
            if (attenuationType == AttenuationType.ParameterizedPart)
            {
                double atten = (inStrength *0.01d) / (double)(this.size * this.size);
                // Note that size is in m, and attenuationCoeff is in m-1
                // TODO: Change this to use the mass attenuation coeffecient in g/cm2. Currently we use attenuation coeff in cm-1
                // Need -(u/p) * p*l , where p = density in g/cm3 and l=path length
                // So atten * Mathf.Exp (-density*this.size * massAttenuationCoeff);
                return atten * Math.Exp(-1d * (double)this.size * attenuationCoeff);
            }
            if (attenuationType == AttenuationType.Part)
            {
                // TODO: as in ParameterizedPart
                // attenuate the distance
                //double distScale = inStrength / (double)(this.size * this.size);
                double materialScale = Math.Exp(-1d * (double)this.size * attenuationCoeff);
                return materialScale;
                // i0*e^(-ux), x = thickness (cm), u = linear attenuation coeff (cm-1). u values:
                // Al: 13, Pb: 82, W: 74, Fe: 26 -> need to be mult by density in g/cm3
            }
            return inStrength;
        }
    }


}
