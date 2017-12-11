using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radioactivity.Persistance
{
    public class KerbalDatabase
    {
        public bool Ready = false;
        public Dictionary<string, RadioactivityKerbal> Kerbals;

        public KerbalDatabase()
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
          LogUtils.Log(String.Format("[KerbalDatabase]: {0} removed from database", k.Name));
        }

        internal void Load(ConfigNode node)
        {
            LogUtils.Log("[KerbalDatabase]: Loading...");
            Kerbals.Clear();
            LogUtils.Log("[KerbalDatabase]: Loading from persistence");

            ConfigNode mNode = node.GetNode(RadioactivityConstants.pluginConfigNodeName);
            if (mNode != null)
            {
                ConfigNode[] kNodes = mNode.GetNodes(RadioactivityConstants.kerbalConfigNodeName);
                foreach (ConfigNode kNode in kNodes)
                {

                    if (kNode.HasValue("KerbalName"))
                    {
                        string idx = kNode.GetValue("KerbalName");
                        LogUtils.Log(String.Format("[KerbalDatabase]: Loading kerbal {0}", idx));
                        RadioactivityKerbal kerbal = new RadioactivityKerbal(idx);
                        kerbal.Load(kNode, idx);
                        Kerbals[idx] = kerbal;
                    }
                }
            }
            LogUtils.Log("[KerbalDatabase]: Loading from roster");
            var crewList = HighLogic.CurrentGame.CrewRoster.Crew.Concat(HighLogic.CurrentGame.CrewRoster.Applicants).Concat(HighLogic.CurrentGame.CrewRoster.Tourist).Concat(HighLogic.CurrentGame.CrewRoster.Unowned).ToList();
            foreach (ProtoCrewMember crew in crewList)
            {
                if (!Kerbals.ContainsKey(crew.name))
                {
                    LogUtils.Log(String.Format("[KerbalDatabase]: Loading kerbal {0}", crew.name));
                    RadioactivityKerbal kerbal = new RadioactivityKerbal(crew.name);
                    kerbal.Load(crew);
                    Kerbals[crew.name] = kerbal;
                }
            }
            LogUtils.Log(String.Format("[KerbalDatabase]: Loaded {0} Kerbals",Kerbals.Count ));
            LogUtils.Log("[KerbalDatabase]: Loading Complete!");
        }

        internal void Save(ConfigNode node)
        {
            LogUtils.Log("[KerbalDatabase]: Saving...");

            ConfigNode dbNode;
            bool init = node.HasNode(RadioactivityConstants.pluginConfigNodeName);
            if (init)
                dbNode =node.GetNode(RadioactivityConstants.pluginConfigNodeName);
            else
                dbNode = node.AddNode(RadioactivityConstants.pluginConfigNodeName);


            foreach (KeyValuePair<string, RadioactivityKerbal> kerbal in Kerbals)
            {
                LogUtils.Log(string.Format("[KerbalDatabase]: Saving kerbal {0}", kerbal.Value));
                ConfigNode kNode = kerbal.Value.Save(dbNode);
                kNode.AddValue("KerbalName",kerbal.Key);
            }
            LogUtils.Log("[KerbalDatabase]: Saving completed!");

        }


    }



}
