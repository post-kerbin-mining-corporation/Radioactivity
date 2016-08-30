// A module creates radiation based on engine throttle
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Radioactivity.Interfaces;

namespace Radioactivity
{

  public class RadioactiveEngine: PartModule, IRadiationEmitter
  {
      // RadioactiveSource to use for emission
      [KSPField(isPersistant = true)]
      public string SourceID = "";

    // Engine ID to link to (only valid using ModuleEnginesFX
    [KSPField(isPersistant = true)]
    public string EngineID = "";

    // Emission at maximum value
    [KSPField(isPersistant = false)]
    public float EmissionAtMax = 100f;

    // Alias for UI
    [KSPField(isPersistant = false)]
    public string UIName = "Nuclear Engine";

    float currentEmission = 0f;
    protected bool useLegacyEngines = false;

    protected ModuleEnginesFX engine;
    protected ModuleEngines engineLegacy;

      // Interface
    public bool IsEmitting()
    {
        return true;
    }
    public float GetEmission()
    {
        return currentEmission;
    }
    public string GetSourceName()
    {
        return SourceID;
    }
    public string GetAlias()
    {
        return UIName;
    }
    public override string GetInfo()
    {
        string toRet = String.Format("Emits radiation when running \n\n <b>Maximum emission:</b> {0}Sv/s", Utils.ToSI(EmissionAtMax,"F2"));

        return toRet;
    }
    public Dictionary<string, string> GetDetails()
    {
        Dictionary<string, string> toReturn = new Dictionary<string, string>();
        toReturn.Add("<color=#ffffff><b>Engine Emission</b>:</color>", String.Format("{0}Sv/s", Utils.ToSI(currentEmission,"F2")));
        return toReturn;
    }

    public void Start()
    {
      //base.OnStart(state);
      SetupEngines();
    }
    public void FixedUpdate()
    {
      if (useLegacyEngines)
        HandleEmissionLegacy();
      else
          HandleEmission();
    }
    // Handles emission for engines using ModuleEngines
    protected void HandleEmissionLegacy()
    {
        if (HighLogic.LoadedSceneIsEditor)
            currentEmission = EmissionAtMax;
      if (engineLegacy == null)
        return;

        if (HighLogic.LoadedSceneIsFlight)
          currentEmission = engineLegacy.requestedThrottle * EmissionAtMax;
        else
          currentEmission = EmissionAtMax;
    }
    // Handles emission for engines using ModuleEnginesFX
    protected void HandleEmission()
    {
        if (HighLogic.LoadedSceneIsEditor)
            currentEmission = EmissionAtMax;
        if (engine == null)
            return;

        if (HighLogic.LoadedSceneIsFlight)
          currentEmission = engine.requestedThrottle * EmissionAtMax;
        else
          currentEmission = EmissionAtMax;
    }

    protected void SetupEngines()
    {
      ModuleEnginesFX[] engines = this.GetComponents<ModuleEnginesFX>();
      ModuleEngines[] enginesLegacy = this.GetComponents<ModuleEngines>();

      if (enginesLegacy.Length > 0)
      {
        if (RadioactivitySettings.debugModules)
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
