// Represents a basic radioactive source from which to emit ray-diation
// All parts that interact with the radiation simulation need one of these
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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
      private List<GenericRadiationEmitter> associatedEmitters = new List<GenericRadiationEmitter>();

      // Registers an emitter module to emit from this ource
      public void RegisterEmitter(GenericRadiationEmitter emit)
      {
        if (associatedEmitters == null)
          associatedEmitters = new List<GenericRadiationEmitter>();
        associatedEmitters.Add(emit);
      }
      
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
        foreach (GenericRadiationEmitter emit in associatedEmitters)
        {
          isAllOff = emit.Emitting;
          emitSum = emitSum + emit.CurrentEmission;
        }
        Emitting = isAllOff;
        CurrentEmission = emitSum;
      }



  }
}
