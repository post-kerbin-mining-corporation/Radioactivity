// Represents a basic radioactive source from which to emit ray-diation
// All parts that interact with the radiation simulation need one of these
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Radioactivity.Interfaces;

namespace Radioactivity
{

  public class RadioactiveSource:PartModule
  {
      // The name of the radioactive source
      [KSPField(isPersistant = false)]
      public string SourceID;
      // Name of the transform to use for emission
      [KSPField(isPersistant = false)]
      public string EmitterTransformName;

      [KSPField(isPersistant = true)]
      public bool Emitting = true;

      [KSPField(isPersistant = false)]
      public bool ShowOverlay = false;

      [KSPField(isPersistant = false)]
      public int IconID = 0;

      // Show or hide the radioactive overlay from this source
      [KSPEvent(guiActive = true, guiName = "Toggle Rays")]
      public void ToggleOverlay()
      {
        ShowOverlay = !ShowOverlay;
        if (ShowOverlay)
          Radioactivity.Instance.ShowOverlay(this);
        else
          Radioactivity.Instance.HideOverlay(this);
      }

      public string GetEmitterAliases()
      {
          string aliases = "";
          for (int i = 0; i<associatedEmitters.Count ;i++)
          {
              aliases += associatedEmitters[i].GetAlias();
              if (i + 1 < associatedEmitters.Count)
              {
                  aliases += "\n";
              }
          }
          return aliases;
      }
      public string GetEmitterDetails()
      {
          string details = "";
          for (int i = 0; i < associatedEmitters.Count; i++)
          {
              details += associatedEmitters[i].GetDetails();
              if (i + 1 < associatedEmitters.Count)
              {
                  details += "\n";
              }
          }
          return details;

      }
      public List<RadiationLink> GetAssociatedLinks()
      {
          List<RadiationLink> lnks = new List<RadiationLink>();
          for (int i = 0; i < Radioactivity.Instance.AllLinks.Count; i++)
          {
              if (Radioactivity.Instance.AllLinks[i].source == this)
                  lnks.Add(Radioactivity.Instance.AllLinks[i]);
          }
          return lnks;
      }

      // Access the current emission
      public float CurrentEmission
      {
        get { return curEmission;}
        set { curEmission = value;}
      }

      // Access the emitter transform
      public Transform EmitterTransform
      {
        get { return emitterTransform;}
        set { emitterTransform = value;}
      }

      private float curEmission = 0f;
      private bool registered = false;
      private Transform emitterTransform;
      private List<IRadiationEmitter> associatedEmitters = new List<IRadiationEmitter>();


      public override void OnStart(PartModule.StartState state)
      {
          // Set up the emission transform, if it doesn't exist use the part root
          if (EmitterTransformName != String.Empty)
              EmitterTransform = part.FindModelTransform(EmitterTransformName);
          if (EmitterTransform == null)
          {
              Utils.LogWarning("Couldn't find Emitter transform, using part root transform");
              EmitterTransform = part.transform;
          }

          List<IRadiationEmitter> emitters = part.FindModulesImplementing<IRadiationEmitter>();
          foreach (IRadiationEmitter emit in emitters)
          {
              if (emit.GetSourceName() == SourceID)
                  associatedEmitters.Add(emit);
          }

          if (HighLogic.LoadedSceneIsFlight && !registered)
          {
              Radioactivity.Instance.RegisterSource(this);
              registered = true;
          }
      }

      public void OnDestroy()
      {

          if (HighLogic.LoadedSceneIsFlight)
          {
              Radioactivity.Instance.UnregisterSource(this);
              registered = false;
          }
      }

      public override void OnFixedUpdate()
      {
        PollEmitters();
      }

      // Look through all registered emitters and add up the emission
      protected void PollEmitters()
      {
        float emitSum = 0f;
        bool isAllOff = false;
        foreach (IRadiationEmitter emit in associatedEmitters)
        {
            isAllOff = emit.IsEmitting();
            emitSum = emitSum + emit.GetEmission();
        }
        Emitting = isAllOff;
        CurrentEmission = emitSum;
      }




  }
}
