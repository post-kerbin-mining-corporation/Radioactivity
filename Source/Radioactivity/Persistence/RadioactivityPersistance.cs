using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Radioactivity.Persistance;

namespace Radioactivity
{
    [KSPScenario(ScenarioCreationOptions.AddToAllGames, GameScenes.SPACECENTER, GameScenes.FLIGHT, GameScenes.TRACKSTATION)]
    public class RadioactivityPersistance : ScenarioModule
    {
        public KerbalDatabase KerbalDB { get { return kerbalDB; }}
        public static RadioactivityPersistance Instance { get; private set; }

        internal KerbalDatabase kerbalDB;

        public override void OnAwake()
        {
            LogUtils.Log("[Persistance]: Init");
            Instance = this;
            base.OnAwake();

            kerbalDB = new KerbalDatabase();
        }

        public override void OnLoad(ConfigNode node)
        {
            LogUtils.Log("[Persistance]: Started Loading");
            base.OnLoad(node);
            KerbalDB.Load(node);
            RadioactivityPreferences.Load(node);
            RadioactivityConstants.Load();
            LogUtils.Log("[Persistance]: Done Loading");
            KerbalDB.Ready = true;

        }

        public override void OnSave(ConfigNode node)
        {
            LogUtils.Log("[Persistance]: Started Saving");
            base.OnSave(node);
            KerbalDB.Save(node);
            LogUtils.Log("[Persistance]: Finished Saving");
        }

        public List<RadioactivityKerbal> GetKerbals(List<ProtoCrewMember> crew)
        {
            List<RadioactivityKerbal> toReturn = new List<RadioactivityKerbal>();

            foreach (KeyValuePair<string, RadioactivityKerbal> kerbal in KerbalDB.Kerbals)
            {
                foreach (ProtoCrewMember crewMember in crew)
                {
                    if (crewMember == kerbal.Value.Kerbal)
                        toReturn.Add(kerbal.Value);
                }
            }
            return toReturn;
        }
    }
}
