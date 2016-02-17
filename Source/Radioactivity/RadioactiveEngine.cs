// A module creates radiation based on engine throttle
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Radioactivity
{

  public class RadioactiveEngine: GenericRadiationEmitter
  {
    // Engine to link
    [KSPField(isPersistant = true)]
    public string EngineID = "";

    // Maximum emission at engine maximum
    [KSPField(isPersistant = false)]
    public float EmissionAtMax = 100f;

    protected bool useLegacyEngines = false;

    protected ModuleEnginesFX engine;
    protected ModuleEngines engineLegacy;

    public override void OnStart(PartModule.StartState state)
    {
      base.OnStart(state);
      SetupEngines();
    }
    public override void OnFixedUpdate()
    {
      if (useLegacyEngines)
        HandleEmissionLegacy();
      else
          HandleEmission();
    }
    // Handles emission for engines using ModuleEngines
    protected void HandleEmissionLegacy()
    {
      if (engineLegacy == null)
        return;

      base.CurrentEmission = engineLegacy.requestedThrottle * EmissionAtMax;
    }
    // Handles emission for engines using ModuleEnginesFX
    protected void HandleEmission()
    {
      if (engine == null)
        return;

      base.CurrentEmission = engine.requestedThrottle * EmissionAtMax;
    }

    protected void SetupEngines()
    {
      ModuleEnginesFX[] engines = this.GetComponents<ModuleEnginesFX>();
      ModuleEngines[] enginesLegacy = this.GetComponents<ModuleEngines>();

      if (enginesLegacy.Length > 0)
      {
        Utils.Log("RadioactiveEngine: Using legacy engine module");
        useLegacyEngines = true;
        engineLegacy = enginesLegacy[0];
      } else
      {
        if (EngineID == "" || EngineID == String.Empty)
        {
            Utils.LogWarning("RadioactiveEngine: EngineID field not specified, trying to use default engine");
            if (engines.Length > 0)
              engine = engines[0];
        }
        foreach (ModuleEnginesFX fx in engines)
        {
          if (fx.engineID == EngineID)
          {
            engine = fx;
          }
        }
      }
      if (useLegacyEngines)
      {
        if (engineLegacy == null)
          Utils.LogError("RadioactiveEngine: Couldn't find a legacy engine module");
      } else
      {
        if (engine == null)
          Utils.LogError("RadioactiveEngine: Couldn't find a ModuleEnginesFX engine module");
      }
    }
  }
}
