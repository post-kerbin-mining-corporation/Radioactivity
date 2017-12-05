// A module that integrates a shadow shield into an emitting part
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Radioactivity.Interfaces;

namespace Radioactivity
{

    public class RadiationShadowShield : PartModule
    {

        [KSPField(isPersistant = false)]
        public string ShieldName = "Shadow Shield";

        [KSPField(isPersistant = false)]
        public string ShieldGeometry = "DISC";

        [KSPField(isPersistant = false)]
        public float ShieldRadius = 1f;

        // Local shield position in part space, Vector frmf
        [KSPField(isPersistant = false)]
        public string ShieldPosition;

        [KSPField(isPersistant = false)]
        public float Density;
        [KSPField(isPersistant = false)]
        public float Thickness;
        [KSPField(isPersistant = false)]
        public float MassAttenuationCoeffecient;

        protected Vector3 shieldPosition;

        public override void OnStart(PartModule.StartState state)
        {

            base.OnStart(state);

        }
        public ShadowShieldEffect BuildShadowShield(Transform emitter)
        {
            shieldPosition = Utils.Vector3FromString(ShieldPosition);
            return new ShadowShieldEffect(this.part, Density, Thickness, MassAttenuationCoeffecient, emitter, shieldPosition - emitter.localPosition, shieldPosition, ShieldRadius);
        }

    }

    public class ShadowShieldEffect
    {
        public Part host;
        public Transform emitterTransform;
        public Vector3 orientation;

        public Vector3 realPosition;
        public Vector3 localPosition;
        public Vector3 dimensions;


        float angle;
        double outAttenuation;
        public GameObject renderer;


        public ShadowShieldEffect(Part p, float density, float thickness, float coeff, Transform emitter, Vector3 shieldOrient, Vector3 shieldPos, float shieldRad)
        {
            host = p;
            emitterTransform = emitter;

            outAttenuation = Math.Exp(-1d * (double)(density * thickness * coeff));

            localPosition = shieldPos;
            realPosition = host.partTransform.TransformPoint(localPosition);

            angle = Mathf.Atan((shieldRad) / (2f * Vector3.Distance(emitterTransform.position, realPosition))) * Mathf.Rad2Deg;

            dimensions = new Vector3(shieldRad, thickness, shieldRad);

            if (RadioactivityConstants.debugModules)
                Utils.Log(String.Format("Shadow Shield: created new with position {0}, radius {1:F1}, angular size {2:F1}", localPosition.ToString(), shieldRad, angle));
        }

        public double AttenuateShield(Vector3 rayDir)
        {
            orientation = host.partTransform.TransformPoint(localPosition) - emitterTransform.position;
            if (Vector3.Angle(rayDir, orientation) <= angle)
            {
                if (RadioactivityConstants.debugModules)
                    Utils.Log("Shadow Shield: attenuated ray to " + outAttenuation.ToString());
                return outAttenuation;

            }
            else
            {
                return 1d;
            }

        }
    }
}
