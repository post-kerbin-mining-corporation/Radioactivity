using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radioactivity
{
    internal class KerbalDatabase
    {
        Dictionary<string, RadioactivityKerbal> Kerbals;

        internal KerbalDatabase()
        {
            Kerbals = new Dictionary<string, RadioactivityKerbal>();
        }


        internal void Load(ConfigNode node)
        {
            Utils.Log("Kerbal Database: Loading...");
            Kerbals.Clear();

            ConfigNode[] kNodes = node.GetNodes(RadioactivitySettings.pluginConfigNodeName);
            foreach (ConfigNode kNode in kNodes)
            {
                if (kNode.HasValue("KerbalName"))
                {
                    string idx = kNode.GetValue("KerbalName");
                    Utils.Log(string.Format("Kerbal Database: Loading kerbal {0}", idx));
                    RadioactivityKerbal kerbal = RadioactivityKerbal.Load(kNode, idx);
                    Kerbals[idx] = kerbal;
                }
            }
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
        }

        public void Irradiate(double amt)
        {
          TotalExposure = TotalExposure + amt;
          CurrentExposure = amt;
        }

        public static RadioactivityKerbal Load(ConfigNode config, string name)
        {
            RadioactivityKerbal newKerbal = new RadioactivityKerbal(name);
            //newKerbal.CrewType = Utils.GetValue(config, "Type", ProtoCrewMember.KerbalType.Crew);
            newKerbal.TotalExposure = Utils.GetValue(config, "TotalExposure", 0d);
            newKerbal.CurrentExposure = Utils.GetValue(config, "CurrentExposure", 0d);
            return newKerbal;
        }

        public ConfigNode Save(ConfigNode config)
        {
            ConfigNode node = config.AddNode(RadioactivitySettings.kerbalConfigNodeName);
            config.AddValue("lastUpdate", LastUpdate);
            config.AddValue("Name", Name);
            config.AddValue("Status", Status);
            config.AddValue("Type", CrewType);
            config.AddValue("TotalExposure", TotalExposure);
            config.AddValue("CurrentExposure", CurrentExposure);
            
            return node;
        }


    }

}
