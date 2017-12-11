using System;
using UnityEngine;

namespace Radioactivity.Simulator
{
    public class ShadowShield
    {
        public Part host;
        public Transform emitterTransform;
        public Vector3 orientation;

        public Vector3 realPosition;
        public Vector3 localPosition;
        public Vector3 dimensions;


        float angle;
        double outAttenuation;


        public ShadowShield(Part p, float density, float thickness, float coeff, Transform emitter, Vector3 shieldOrient, Vector3 shieldPos, float shieldRad)
        {
            host = p;
            emitterTransform = emitter;

            outAttenuation = Math.Exp(-1d * (double)(density * thickness * coeff));

            localPosition = shieldPos;
            realPosition = host.partTransform.TransformPoint(localPosition);

            angle = Mathf.Atan((shieldRad) / (2f * Vector3.Distance(emitterTransform.position, realPosition))) * Mathf.Rad2Deg;

            dimensions = new Vector3(shieldRad, thickness, shieldRad);

            if (RadioactivityConstants.debugModules)
                LogUtils.Log(String.Format("[ShadowShield]: created new with position {0}, radius {1:F1}, angular size {2:F1}", localPosition.ToString(), shieldRad, angle));
        }

        public double AttenuateShield(Vector3 rayDir)
        {
            orientation = host.partTransform.TransformPoint(localPosition) - emitterTransform.position;
            if (Vector3.Angle(rayDir, orientation) <= angle)
            {
                if (RadioactivityConstants.debugModules)
                    LogUtils.Log("[ShadowShield]: attenuated ray to " + outAttenuation.ToString());
                return outAttenuation;

            }
            else
            {
                return 1d;
            }

        }
    }
}
