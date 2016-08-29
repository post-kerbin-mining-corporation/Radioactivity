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
            return new List<RadioactivityKerbal>(Kerbals.Values);
        }
        public List<RadioactivityKerbal> AllKerbals()
        {
            return new List<RadioactivityKerbal>(Kerbals.Values);
        }
        public List<RadioactivityKerbal> ActiveKerbals()
        {
            return new List<RadioactivityKerbal>(Kerbals.Values);
        }
        public List<RadioactivityKerbal> KSCKerbals()
        {
            return new List<RadioactivityKerbal>(Kerbals.Values);
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

    class RadioactivityKerbal
    {
        public double LastUpdate;
        public ProtoCrewMember.RosterStatus Status = ProtoCrewMember.RosterStatus.Available;
        public ProtoCrewMember.KerbalType CrewType = ProtoCrewMember.KerbalType.Crew;
        public Guid VesselId = Guid.Empty;
        public string VesselName = " ";
        public uint PartId;  //Probably not required - currently not used.
        public int SeatIdx;  //Probably not required - currently not used.
        public string SeatName = string.Empty;  //Probably not required - currently not used.

        public double TotalExposure;
        public double CurrentExposure;

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
        public void Irradiate(double amt)
        {
          LastUpdate = Planetarium.GetUniversalTime();
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
          if (Kerbal.rosterStatus == ProtoCrewMember.RosterStatus.Available)
          {
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
          if (TotalExposure >= RadioactivitySettings.kerbalDeathThreshold)
          {
            //Utils.LogWarning(Name + " died of radiation exposure");
            return;
          }
          if (TotalExposure >= RadioactivitySettings.kerbalSicknessThreshold)
          {
            //Utils.LogWarning(Name + " got radiation sickness");
            return;
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

        }
        public void Load(ProtoCrewMember crewMember)
        {
            Name = crewMember.name;
            Kerbal = crewMember;

            //newKerbal.CrewType = Utils.GetValue(config, "Type", ProtoCrewMember.KerbalType.Crew);
            TotalExposure = 0d;
            CurrentExposure = 0d;
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

            return node;
        }


    }

}
