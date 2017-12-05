using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radioactivity.Persistance
{
  public enum RadioactivityKerbalState {
    Healthy, Sick, Dead, Home
  }

  // The concept for some of these functions is taken from RosterManager
  public class RadioactivityKerbal
  {
      public double LastUpdate;
      public ProtoCrewMember.RosterStatus Status = ProtoCrewMember.RosterStatus.Available;
      public ProtoCrewMember.KerbalType CrewType = ProtoCrewMember.KerbalType.Crew;
      public Guid VesselID = Guid.Empty;
      public Vessel CurrentVessel;
      public string VesselName = " ";
      public uint PartId;  //Probably not required - currently not used.
      public int SeatIdx;  //Probably not required - currently not used.
      public string SeatName = string.Empty;  //Probably not required - currently not used.

      // The fraction of radiation attenuated by parts
      public double LocalExposureFraction;
      // The fraction of the sky covered by the current mainbody
      public double BodyExposureFraction;
      // The fraction of the sky covered by the sky.
      public double SkyExposureFraction;

      // Kerbal's total exposure
      public double TotalExposure;
      // Kerbal's current exposure from point sources
      public double CurrentExposure;
      // Kerbal's current exposure from ambient sources
      public double CurrentAmbientExposure;

      // Kerbal's health state
      public RadioactivityKerbalState HealthState;

      public ProtoCrewMember Kerbal { get; set; }
      public bool IsNew { get; set; }
      public string Name;

      public RadioactivityKerbal(string name)
      {
          Name = name;
          TotalExposure = 0d;
          CurrentExposure = 0d;
          CurrentAmbientExposure = 0d;
      }
      // Called by RadioactiveSink
      // Add radiation to a kerbal from a point source
      public void IrradiatePoint(Vessel curVessel, double amt)
      {
        LastUpdate = Planetarium.GetUniversalTime();
        CurrentVessel = curVessel;
        CurrentExposure = amt;
      }
      public void SetAmbientExposure(double bodyFraction, double skyFraction)
      {
        //LocalExposureFraction = pointFraction;
        SkyExposureFraction = skyFraction;
        BodyExposureFraction = bodyFraction;
      }
      public void IrradiateAmbient(double amt)
      {
        CurrentAmbientExposure = amt;
      }

      // Called by the simulator
      // This function does all the dose simulating
      public void Simulate(float timeStep)
      {
        if (Kerbal.rosterStatus == ProtoCrewMember.RosterStatus.Dead || Kerbal.rosterStatus == ProtoCrewMember.RosterStatus.Missing)
        {
            SimulateDead(timeStep);
        }
        else if (Kerbal.rosterStatus == ProtoCrewMember.RosterStatus.Available)
        {
            SimulateKSC(timeStep);

        } else
        {
            SimulateOnMission(timeStep);
        }
        // Do the actual effects on kerbals
           //if (RadioactivityConstants.enableKerbalEffects)
          //HandleExposure();
      }

      // Simulates effects when the kerbal is dead or missing
      void SimulateDead(float timeStep)
      {
        CurrentVessel = null;
            RadioactivityPersistance.Instance.KerbalDB.RemoveKerbal(this);
      }
      // Simulates effects when the kerbal is at the KSC
      void SimulateKSC(float timeStep)
      {
        CurrentVessel = null;
        //HealthState = RadioactivityKerbalState.Home;
            TotalExposure = TotalExposure - RadioactivityConstants.kerbalHealRateKSC*timeStep;
        if (TotalExposure < 0d)
          TotalExposure = 0d;
      }
      // Simulates effects when the
      void SimulateOnMission(float timeStep)
      {
            if ((CurrentExposure) <= RadioactivityConstants.kerbalHealThreshold)
        {
                TotalExposure = TotalExposure - RadioactivityConstants.kerbalHealRate*timeStep;
            if (TotalExposure < 0d)
              TotalExposure = 0d;
        }
        else
        {
            TotalExposure = TotalExposure + (CurrentExposure + CurrentAmbientExposure)*timeStep;
        }
      }

      void HandleExposure()
      {
            if (TotalExposure >= RadioactivityConstants.kerbalSicknessThreshold)
        {
          if (HealthState != RadioactivityKerbalState.Sick && HealthState != RadioactivityKerbalState.Dead)
          {
            Sicken();
            return;
          }
          //Utils.LogWarning(Name + " died of radiation exposure");
        }
            if (TotalExposure >= RadioactivityConstants.kerbalDeathThreshold)
        {
          if (HealthState != RadioactivityKerbalState.Dead)
          {
              Die();
              return;
          }
          //Utils.LogWarning(Name + " got radiation sickness");
        }
            if (TotalExposure < RadioactivityConstants.kerbalSicknessThreshold)
        {
          if (HealthState == RadioactivityKerbalState.Sick)
          {
              Heal();
          }
          //Utils.LogWarning(Name + " got radiation sickness");
        }
      }

      void Sicken()
      {
        HealthState = RadioactivityKerbalState.Sick;
        ScreenMessages.PostScreenMessage(new ScreenMessage(String.Format("{0} now has radiation sickness", Name), 4.0f, ScreenMessageStyle.UPPER_CENTER));
            if (RadioactivityConstants.debugKerbalEvents)
          Utils.LogWarning(String.Format("Kerbals: {0} got radiation sickness", Name));
      }
      void Heal()
      {
        HealthState = RadioactivityKerbalState.Healthy;
        ScreenMessages.PostScreenMessage(new ScreenMessage(String.Format("{0} recovered from radiation sickness", Name), 4.0f, ScreenMessageStyle.UPPER_CENTER));
            if (RadioactivityConstants.debugKerbalEvents)
          Utils.LogWarning(String.Format("Kerbals: {0} recovered from radiation sickness", Name));
      }

      /// "kills" a kerbal
      void Die()
      {

        HealthState = RadioactivityKerbalState.Dead;
            RadioactivityPersistance.Instance.KerbalDB.RemoveKerbal(this);
        ScreenMessages.PostScreenMessage(new ScreenMessage(String.Format("{0} has died of radiation exposure", Name), 4.0f, ScreenMessageStyle.UPPER_CENTER));

        if (CurrentVessel != null && CurrentVessel.isEVA)
        {
            // If we are EVA, have to handle this more carefully
          CurrentVessel.rootPart.Die();
                if (HighLogic.CurrentGame.Parameters.CustomParams<RadioactivityEffectSettings>().kerbalDeath)
          {
              // Do nothing
          } else
          {
              if (HighLogic.CurrentGame.Parameters.Difficulty.MissingCrewsRespawn)
              {
                  Kerbal.StartRespawnPeriod(2160000.0); // 100 Kerbin days
              }
          }
                if (RadioactivityConstants.debugKerbalEvents)
              Utils.LogWarning(String.Format("Kerbals: {0} died on EVA of radiation exposure", Name));
        }
        else
        {
            if (Kerbal.rosterStatus == ProtoCrewMember.RosterStatus.Available)
            {
                Kerbal.Die();
                if (HighLogic.CurrentGame.Parameters.Difficulty.MissingCrewsRespawn)
                {
                    Kerbal.StartRespawnPeriod(2160000.0); // 100 Kerbin days
                }
                    if (RadioactivityConstants.debugKerbalEvents)
                    Utils.LogWarning(String.Format("Kerbals: {0} died at home of radiation exposure", Name));
            }
            else
            {

                Part part = CurrentVessel.Parts.Find(p => p.protoModuleCrew.Contains(Kerbal));
                if (part != null)
                {
                    part.RemoveCrewmember(Kerbal);
                    Kerbal.Die();
                    if (HighLogic.CurrentGame.Parameters.Difficulty.MissingCrewsRespawn)
                    {
                        Kerbal.StartRespawnPeriod(2160000.0); // 100 Kerbin days
                    }
                        if (RadioactivityConstants.debugKerbalEvents)
                        Utils.LogWarning(String.Format("Kerbals: {0} died in his vessel of radiation exposure", Name));
                    //HighLogic.CurrentGame.CrewRoster.RemoveDead(Kerbal);
                }
            }
        }



      }

      // Load from confignode
      public void Load(ConfigNode config, string name)
      {

          Name = name;
          var crewList = HighLogic.CurrentGame.CrewRoster.Crew.Concat(HighLogic.CurrentGame.CrewRoster.Applicants).Concat(HighLogic.CurrentGame.CrewRoster.Tourist).Concat(HighLogic.CurrentGame.CrewRoster.Unowned).ToList();
          Kerbal = crewList.FirstOrDefault(a => a.name == name);
          //newKerbal.CrewType = Utils.GetValue(config, "Type", ProtoCrewMember.KerbalType.Crew);
          LastUpdate = Utils.GetValue(config, "LastUpdate", 0d);

          TotalExposure = Utils.GetValue(config, "TotalExposure", 0d);
          CurrentExposure = Utils.GetValue(config, "CurrentExposure", 0d);
          CurrentAmbientExposure = Utils.GetValue(config, "CurrentAmbientExposure", 0d);

          LocalExposureFraction = Utils.GetValue(config, "LocalExposureFraction", 1d);
          BodyExposureFraction = Utils.GetValue(config, "BodyExposureFraction", 0d);
          SkyExposureFraction = Utils.GetValue(config, "SkyExposureFraction", 1d);

          HealthState = (RadioactivityKerbalState)Enum.Parse(typeof(RadioactivityKerbalState), Utils.GetValue(config, "HealthState", "Healthy"));

          VesselID = Utils.GetValue(config, "VesselID", Guid.Empty);
          if (Guid.Empty.Equals(VesselID))
          {
              CurrentVessel = null;
          }
          else
          {

              Vessel tryVessel = FlightGlobals.Vessels.FirstOrDefault(a => a.id == VesselID);
              if (tryVessel != null && tryVessel.loaded)
              {
                  CurrentVessel = tryVessel;

              }
          }
      }
      public void Load(ProtoCrewMember crewMember)
      {
          Name = crewMember.name;
          Kerbal = crewMember;
          //Status = crewMember
          //newKerbal.CrewType = Utils.GetValue(config, "Type", ProtoCrewMember.KerbalType.Crew);
          TotalExposure = 0d;
          CurrentExposure = 0d;

          LocalExposureFraction = 1d;
          BodyExposureFraction = 0d;
          SkyExposureFraction = 1d;

          HealthState = RadioactivityKerbalState.Healthy;
      }

      public ConfigNode Save(ConfigNode config)
      {
            ConfigNode node = config.AddNode(RadioactivityConstants.kerbalConfigNodeName);
          node.AddValue("lastUpdate", LastUpdate);
          //node.AddValue("Name", Name);
          node.AddValue("Status", Status);
          node.AddValue("Type", CrewType);
          node.AddValue("TotalExposure", TotalExposure);
          node.AddValue("CurrentExposure", CurrentExposure);
          node.AddValue("HealthState",HealthState.ToString());
          node.AddValue("LocalExposureFraction", LocalExposureFraction);
          node.AddValue("BodyExposureFraction", BodyExposureFraction);
          node.AddValue("SkyExposureFraction", SkyExposureFraction);
          node.AddValue("VesselID",VesselID.ToString());
          return node;
      }


  }
}
