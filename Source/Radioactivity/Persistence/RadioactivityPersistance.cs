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

        public static RadioactivityPersistance Instance { get; private set; }

        internal KerbalDatabase KerbalDB;
        internal bool databaseReady = false;

        public override void OnAwake()
        {
            Utils.Log("[Persistance]: Init");
            Instance = this;
            base.OnAwake();

            KerbalDB = new KerbalDatabase();
        }

        public override void OnLoad(ConfigNode node)
        {
            Utils.Log("[Persistance]: Started Loading");
            base.OnLoad(node);
            KerbalDB.Load(node);
            RadioactivityPreferences.Load(node);
            RadioactivityConstants.Load();
            Utils.Log("[Persistance]: Done Loading");
            databaseReady = true;

        }

        public override void OnSave(ConfigNode node)
        {
            Utils.Log("[Persistance]: Started Saving");
            base.OnSave(node);
            KerbalDB.Save(node);
            Utils.Log("[Persistance]: Finished Saving");
        }

        internal void FixedUpdate()
        {

        }

        public void SimulateKerbals(float time)
        {
            if (databaseReady)
            {
                foreach (KeyValuePair<string, RadioactivityKerbal> kerbal in KerbalDB.Kerbals)
                {
                    kerbal.Value.Simulate(time);
                }
            }
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

        // Irradiates a kerbal
        public void IrradiateKerbal(ProtoCrewMember crew, Vessel crewVessel, double pointAmount)
        {
            foreach (KeyValuePair<string, RadioactivityKerbal> kerbal in KerbalDB.Kerbals)
            {
                if (crew == kerbal.Value.Kerbal)
                    kerbal.Value.IrradiatePoint(crewVessel, pointAmount);
            }
        }
        public void IrradiateKerbal(ProtoCrewMember crew, Vessel crewVessel, double pointAmount, double bodyFraction, double skyFraction, double partFraction)
        {
            foreach (KeyValuePair<string, RadioactivityKerbal> kerbal in KerbalDB.Kerbals)
            {
                if (crew == kerbal.Value.Kerbal)
                {
                    kerbal.Value.IrradiatePoint(crewVessel, pointAmount);
                    //kerbal.Value.SetAmbientExposure(bodyFraction, skyFraction, partFraction);
                }
            }
        }


    }
}
