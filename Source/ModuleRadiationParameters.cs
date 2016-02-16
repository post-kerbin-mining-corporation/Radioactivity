// Use this module to override radiation absorbing parameters of stuff
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Radioactivity
{

  public class ModuleRadiationParameters:PartModule
  {
      // Override the part density in t/m3
      [KSPField(isPersistant = true)]
      public float Density = 1f;

      // Override the attenuation path length in t/m3
      [KSPField(isPersistant = true)]
      public float AttenuationCoefficient = 16f;

  }
}
