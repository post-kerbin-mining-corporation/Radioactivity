// A module that integrates a shadow shield into an emitting part
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Radioactivity.Interfaces;

namespace Radioactivity
{

  public class RadiationShadowShield: PartModule
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
      return new ShadowShieldEffect(Density, Thickness, MassAttenuationCoeffecient, emitter.localPosition, shieldPosition-emitter.localPosition, shieldPosition, ShieldRadius);
    }

  }

  public class ShadowShieldEffect
  {
    public Vector3 orientation;
    public Vector3 localPosition;
    public Vector3 dimensions;
    float angle;
    double outAttenuation;
    public GameObject renderer;


    public ShadowShieldEffect(float density, float thickness, float coeff, Vector3 emitterPos, Vector3 shieldOrient, Vector3 shieldPos, float shieldRad)
    {
      outAttenuation = Math.Exp(-1d * (double)(density * thickness * coeff));
      angle = Mathf.Atan((shieldRad*2f)/(2f*Vector3.Distance(emitterPos, shieldPos)));
      orientation = shieldOrient;
      localPosition = shieldPos;
      dimensions = new Vector3(shieldRad, thickness, shieldRad);
      if (RadioactivitySettings.debugModules)
          Utils.Log("Shadow Shield: created new with position " + shieldPos.ToString() + ", thickness " + thickness.ToString()+ ", radius" + shieldRad.ToString());
    }

    public double AttenuateShield(Vector3 rayDir)
    {

      if (Vector3.Angle(rayDir, orientation) <= angle)
      {
          if (RadioactivitySettings.debugModules)
              Utils.Log("Shadow Shield: attenuated ray to " + outAttenuation.ToString());
        return outAttenuation;

      } else
      {
        return 1d;
      }

    }
  }
}
