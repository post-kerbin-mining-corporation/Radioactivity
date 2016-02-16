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
      [KSPField(isPersistant = true)]
      public string SourceID = "";
      // Name of the transform to use for emission
      [KSPField(isPersistant = true)]
      public string EmitterTransformName = "";

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
      private List<GenericRadiationEmitter> associatedEmitters = new List<GenericRadiationEmitter>();

      // Registers an emitter module to emit from this ource
      public void RegisterEmitter(GenericRadiationEmitter emit)
      {
        if (associatedEmitters == null)
          associatedEmitters = new List<GenericRadiationEmitter>();
        associatedEmitters.Add(emit);
      }

      public override void OnStart()
      {
        // Set up the emission transform, if it doesn't exist use the part root
        EmitterTransform = part.FindTransformByName("EmitterTransformName");
        if (EmitterTransform == null)
        {
          Debug.LogWarning("Couldn't find Emitter transform, using root transform");
          EmitterTransform = part.transform;
        }
        Radioactivity.Instance.RegisterSource(this);
      }
      public override void OnDestroy()
      {
        Radioactivity.Instance.UnregisterSource(this);
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
          emitSum = emitSum + emit.Emission;
        }
        Emitting = isAllOff;
        CurrentEmission = emitSum;
      }



  }
}
