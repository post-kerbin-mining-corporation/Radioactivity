// A module that integrates a shadow shield into an emitting part
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Radioactivity.Interfaces;
using Radioactivity.Simulator;

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
        public ShadowShield BuildShadowShield(Transform emitter)
        {
            shieldPosition = ConfigNodeUtils.Vector3FromString(ShieldPosition);
            return new ShadowShield(this.part, 
                                    Density, 
                                    Thickness, 
                                    MassAttenuationCoeffecient, emitter, 
                                    shieldPosition - emitter.localPosition, 
                                    shieldPosition, 
                                    ShieldRadius);
        }

    }
}
