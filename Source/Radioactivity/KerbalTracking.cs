using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radioactivity
{
    class KerbalTracking
    {
    }

    class RadioactivityKerbal
    {
        public const string ConfigNodeName = "RadioactivityKerbal";

        public double LastUpdate;
        public ProtoCrewMember.RosterStatus Status = ProtoCrewMember.RosterStatus.Available;
        public ProtoCrewMember.KerbalType Type = ProtoCrewMember.KerbalType.Crew;
        public Guid VesselId = Guid.Empty;
        public string VesselName = " ";
        public uint PartId;  //Probably not required - currently not used.
        public int SeatIdx;  //Probably not required - currently not used.
        public string SeatName = string.Empty;  //Probably not required - currently not used.
    }

}
