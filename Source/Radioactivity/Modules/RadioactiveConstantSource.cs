// A module creates a constant source of radiation
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Radioactivity.Interfaces;

namespace Radioactivity
{

  public class RadioactiveConstantSource: PartModule, IRadiationEmitter
  {
    // RadioactiveSource to use for emission
    [KSPField(isPersistant = true)]
    public string SourceID = "";

    // Emission
    [KSPField(isPersistant = false)]
    public float Emission = 100f;

    // Alias for UI
    [KSPField(isPersistant = false)]
    public string UIName = "Constant";

    float currentEmission = 0f;

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
        string toRet = String.Format("Emits radiation \n\n <b>Constant emission:</b> {0}Sv/s", Utils.ToSI(Emission,"F2"));

        return toRet;
    }
    public Dictionary<string, string> GetDetails()
    {
        Dictionary<string, string> toReturn = new Dictionary<string, string>();
        toReturn.Add("<color=#ffffff><b>Constant Emission</b>:</color>", String.Format("{0}Sv/s", Utils.ToSI(currentEmission,"F2")));
        return toReturn;
    }

    public void FixedUpdate()
    {
       HandleEmission();
    }
    // Handles emission
    protected void HandleEmission()
    {

        if (HighLogic.LoadedSceneIsFlight)
          currentEmission = Emission;
        else
          currentEmission = Emission
    }

  }
}
