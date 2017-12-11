using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radioactivity.Persistance
{
    public enum RadioactivityKerbalState
    {
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
        public double SummedExposure { get { return PointExposure + AmbientShielding * AmbientExposure; } }

        // Kerbal's current exposure from point sources
        public double PointExposure;

        public double AmbientShielding;
        public double AmbientExposure;

        // Kerbal's health state
        public RadioactivityKerbalState HealthState;

        public ProtoCrewMember Kerbal { get; set; }
        public bool IsNew { get; set; }
        public string Name;

        public RadioactivityKerbal(string name)
        {
            Name = name;
            TotalExposure = 0d;

            PointExposure = 0d;
            AmbientExposure = 0d;
        }

        // Load from confignode
        public void Load(ConfigNode config, string name)
        {
            Name = name;
            var crewList = HighLogic.CurrentGame.CrewRoster.Crew.Concat(HighLogic.CurrentGame.CrewRoster.Applicants).Concat(HighLogic.CurrentGame.CrewRoster.Tourist).Concat(HighLogic.CurrentGame.CrewRoster.Unowned).ToList();
            Kerbal = crewList.FirstOrDefault(a => a.name == name);
            //newKerbal.CrewType = Utils.GetValue(config, "Type", ProtoCrewMember.KerbalType.Crew);
            LastUpdate = ConfigNodeUtils.GetValue(config, "LastUpdate", 0d);

            TotalExposure = ConfigNodeUtils.GetValue(config, "TotalExposure", 0d);
            PointExposure = ConfigNodeUtils.GetValue(config, "PointExposure", 0d);
            AmbientExposure = ConfigNodeUtils.GetValue(config, "AmbientExposure", 0d);

            HealthState = (RadioactivityKerbalState)Enum.Parse(typeof(RadioactivityKerbalState), ConfigNodeUtils.GetValue(config, "HealthState", "Healthy"));

            VesselID = ConfigNodeUtils.GetValue(config, "VesselID", Guid.Empty);
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
            PointExposure = 0d;

            AmbientExposure = 0d;

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
            node.AddValue("PointExposure", PointExposure);
            node.AddValue("AmbientExposure", AmbientExposure);

            node.AddValue("HealthState", HealthState.ToString());
            node.AddValue("VesselID", VesselID.ToString());
            return node;
        }


    }
}
