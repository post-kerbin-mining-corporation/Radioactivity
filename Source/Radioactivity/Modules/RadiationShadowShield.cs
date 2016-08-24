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

    public ShadowShieldEffect BuildShadowShield(Transform emitter)
    {
      return new ShadowShieldEffect(Density, Thickness, MassAttenuationCoeffecient, emitter.position, shieldPosition-emitter.postion, shieldPosition,ShieldRadius);
    }


  }

  public class ShadowShieldEffect
  {
    Vector3 orientation;
    float angle;
    double outAttenuation;

    public ShadowShieldEffect(float density, float thickness, float coeff, Vector3 emitterPos, Vector3 shieldOrient, Vector3 shieldPos, float shieldRad)
    {
      outAttenuation = Math.Exp(-1d * (double)(density * thickness * coeff));
      angle = Mathf.Atan((shieldRad*2f)/(2f*Vector3.Distance(emitterPos, shieldPos)));
      orientation = shieldOrient;
    }

    public double AttenuateShield(Vector3 rayDir)
    {
      if (Vector3.Angle(rayDir, orientation) <= angle)
      {
        return outAttenuation;

      } else
      {
        return 1d;
      }

    }
  }
}
