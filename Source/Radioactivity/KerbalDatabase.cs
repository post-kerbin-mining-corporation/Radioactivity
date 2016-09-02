using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radioactivity
{
    internal class KerbalDatabase
    {
        internal Dictionary<string, RadioactivityKerbal> Kerbals;

        internal KerbalDatabase()
        {
            Kerbals = new Dictionary<string, RadioactivityKerbal>();
        }

        public List<RadioactivityKerbal> VesselKerbals(List<ProtoCrewMember> crew)
        {
            List<RadioactivityKerbal> toReturn = new List<RadioactivityKerbal>();
            foreach (var kvp in Kerbals)
            {
                foreach (ProtoCrewMember c in crew)
                {
                    if (kvp.Value.Kerbal == c)
                        toReturn.Add(kvp.Value);
                }
            }
          return toReturn;
        }

        public List<RadioactivityKerbal> NearbyKerbals(List<ProtoCrewMember> crew)
        {
          List<RadioactivityKerbal> toReturn = new List<RadioactivityKerbal>();
          foreach (var kvp in Kerbals)
          {
              foreach (ProtoCrewMember c in crew)
              {
                  if (kvp.Value.Kerbal == c)
                      toReturn.Add(kvp.Value);
              }
          }
          return toReturn;
        }
        public List<RadioactivityKerbal> AllKerbals()
        {
            return new List<RadioactivityKerbal>(Kerbals.Values);
        }
        public List<RadioactivityKerbal> ActiveKerbals()
        {
            List<RadioactivityKerbal> toReturn = new List<RadioactivityKerbal>();
            foreach (var kvp in Kerbals)
            {
                  if (kvp.Value.Kerbal.rosterStatus == ProtoCrewMember.RosterStatus.Assigned)
                      toReturn.Add(kvp.Value);
            }
            return toReturn;

        }
        public List<RadioactivityKerbal> KSCKerbals()
        {
          List<RadioactivityKerbal> toReturn = new List<RadioactivityKerbal>();
          foreach (var kvp in Kerbals)
          {
                if (kvp.Value.Kerbal.rosterStatus == ProtoCrewMember.RosterStatus.Available)
                    toReturn.Add(kvp.Value);
          }
          return toReturn;
        }

        public void RemoveKerbal(RadioactivityKerbal k)
        {
          Kerbals.Remove(k.Name);
          Utils.Log(String.Format("Kerbal Database: {0} removed from database", k.Name));
        }

        internal void Load(ConfigNode node)
        {
            Utils.Log("Kerbal Database: Loading...");
            Kerbals.Clear();
            Utils.Log("Kerbal Database: Loading from persistence");
            ConfigNode mNode = node.GetNode(RadioactivitySettings.pluginConfigNodeName);
            ConfigNode[] kNodes = mNode.GetNodes(RadioactivitySettings.kerbalConfigNodeName);
            foreach (ConfigNode kNode in kNodes)
            {

                if (kNode.HasValue("KerbalName"))
                {
                    string idx = kNode.GetValue("KerbalName");
                    Utils.Log(String.Format("Kerbal Database: Loading kerbal {0}", idx));
                    RadioactivityKerbal kerbal = new RadioactivityKerbal(idx);
                    kerbal.Load(kNode, idx);
                    Kerbals[idx] = kerbal;
                }
            }
            Utils.Log("Kerbal Database: Loading from roster");
            var crewList = HighLogic.CurrentGame.CrewRoster.Crew.Concat(HighLogic.CurrentGame.CrewRoster.Applicants).Concat(HighLogic.CurrentGame.CrewRoster.Tourist).Concat(HighLogic.CurrentGame.CrewRoster.Unowned).ToList();
            foreach (ProtoCrewMember crew in crewList)
            {
                if (!Kerbals.ContainsKey(crew.name))
                {
                    Utils.Log(String.Format("Kerbal Database: Loading kerbal {0}", crew.name));
                    RadioactivityKerbal kerbal = new RadioactivityKerbal(crew.name);
                    kerbal.Load(crew);
                    Kerbals[crew.name] = kerbal;
                }
            }
            Utils.Log(String.Format("Kerbal Database: Loaded {0} Kerbals",Kerbals.Count ));
            Utils.Log("Kerbal Database: Loading Complete!");
        }

        internal void Save(ConfigNode node)
        {
            Utils.Log("Kerbal Database: Saving...");

            ConfigNode dbNode;
            bool init = node.HasNode(RadioactivitySettings.pluginConfigNodeName);
            if (init)
                dbNode =node.GetNode(RadioactivitySettings.pluginConfigNodeName);
            else
                dbNode = node.AddNode(RadioactivitySettings.pluginConfigNodeName);


            foreach (KeyValuePair<string, RadioactivityKerbal> kerbal in Kerbals)
            {
                Utils.Log(string.Format("Kerbal Database: Saving kerbal {0}", kerbal.Value));
                ConfigNode kNode = kerbal.Value.Save(dbNode);
                kNode.AddValue("KerbalName",kerbal.Key);
            }

            Utils.Log("Kerbal Database: Saving completed!");

        }

        public void PropagateExposure()
        {
           double curTime = Planetarium.GetUniversalTime();
           for (int i = 0; i < Kerbals.Count ;i++)
           {
             //Kerbals[i].IrradiateOverTime(curTime);
           }
        }
    }

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

        // Kerbal's total exposure
        public double TotalExposure;
        // Kerbal's current exposure
        public double CurrentExposure;
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
        }
        // Add radiation to a kerbal
        public void Irradiate(Vessel curVessel, double amt)
        {
          LastUpdate = Planetarium.GetUniversalTime();
          CurrentVessel = curVessel;
          //TotalExposure = TotalExposure + amt;
          CurrentExposure = amt;
        }
        // Add radiation to a kerbal based on the latest exposure metrics
        public void IrradiateOverTime(double curTime)
        {
          double catchupSeconds = curTime - LastUpdate;
          double catchupExposure = catchupSeconds*CurrentExposure;
          TotalExposure = TotalExposure + catchupSeconds;
        }

        public void Simulate(float timeStep)
        {
          if (Kerbal.rosterStatus == ProtoCrewMember.RosterStatus.Dead || Kerbal.rosterStatus == ProtoCrewMember.RosterStatus.Missing)
          {
              CurrentVessel = null;
            KerbalTracking.Instance.KerbalDB.RemoveKerbal(this);

            return;
          }
          if (Kerbal.rosterStatus == ProtoCrewMember.RosterStatus.Available)
          {
              
              CurrentVessel = null;
              //HealthState = RadioactivityKerbalState.Home;
              TotalExposure = TotalExposure - RadioactivitySettings.kerbalHealRateKSC*timeStep;
              if (TotalExposure < 0d)
                TotalExposure = 0d;

          } else
          {
              if (CurrentExposure <= RadioactivitySettings.kerbalHealThreshold)
              {
                  TotalExposure = TotalExposure - RadioactivitySettings.kerbalHealRate*timeStep;
                  if (TotalExposure < 0d)
                    TotalExposure = 0d;
              }
              else
              {
                  TotalExposure = TotalExposure + CurrentExposure*timeStep;
              }
          }
          if (RadioactivitySettings.enableKerbalEffects)
            HandleExposure();
        }


        void HandleExposure()
        {
          if (TotalExposure >= RadioactivitySettings.kerbalSicknessThreshold)
          {
            if (HealthState != RadioactivityKerbalState.Sick && HealthState != RadioactivityKerbalState.Dead)
            {
              Sicken();
              return;
            }
            //Utils.LogWarning(Name + " died of radiation exposure");
          }
          if (TotalExposure >= RadioactivitySettings.kerbalDeathThreshold)
          {
            if (HealthState != RadioactivityKerbalState.Dead)
            {
                Die();
                return;
            }
            //Utils.LogWarning(Name + " got radiation sickness");
          }
          if (TotalExposure < RadioactivitySettings.kerbalSicknessThreshold)
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
          if (RadioactivitySettings.debugKerbalEvents)
            Utils.LogWarning(String.Format("Kerbals: {0} got radiation sickness", Name));
        }
        void Heal()
        {
          HealthState = RadioactivityKerbalState.Healthy;
          ScreenMessages.PostScreenMessage(new ScreenMessage(String.Format("{0} recovered from radiation sickness", Name), 4.0f, ScreenMessageStyle.UPPER_CENTER));
          if (RadioactivitySettings.debugKerbalEvents)
            Utils.LogWarning(String.Format("Kerbals: {0} recovered from radiation sickness", Name));
        }

        /// "kills" a kerbal
        void Die()
        {

          HealthState = RadioactivityKerbalState.Dead;
          KerbalTracking.Instance.KerbalDB.RemoveKerbal(this);
          ScreenMessages.PostScreenMessage(new ScreenMessage(String.Format("{0} has died of radiation exposure", Name), 4.0f, ScreenMessageStyle.UPPER_CENTER));

          if (CurrentVessel != null && CurrentVessel.isEVA)
          {
              // If we are EVA, have to handle this more carefully
            CurrentVessel.rootPart.Die();
            if (RadioactivitySettings.enableKerbalDeath)
            {
                // Do nothing
            } else
            {
                if (HighLogic.CurrentGame.Parameters.Difficulty.MissingCrewsRespawn)
                {
                    Kerbal.StartRespawnPeriod(2160000.0); // 100 Kerbin days
                }
            }
            if (RadioactivitySettings.debugKerbalEvents)
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
                  if (RadioactivitySettings.debugKerbalEvents)
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
                      if (RadioactivitySettings.debugKerbalEvents)
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
            HealthState = RadioactivityKerbalState.Healthy;
        }

        public ConfigNode Save(ConfigNode config)
        {
            ConfigNode node = config.AddNode(RadioactivitySettings.kerbalConfigNodeName);
            node.AddValue("lastUpdate", LastUpdate);
            //node.AddValue("Name", Name);
            node.AddValue("Status", Status);
            node.AddValue("Type", CrewType);
            node.AddValue("TotalExposure", TotalExposure);
            node.AddValue("CurrentExposure", CurrentExposure);
            node.AddValue("HealthState",HealthState.ToString());
            node.AddValue("VesselID",VesselID.ToString());
            return node;
        }


    }

}
