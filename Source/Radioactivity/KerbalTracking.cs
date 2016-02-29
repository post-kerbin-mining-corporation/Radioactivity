using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radioactivity
{
    [KSPScenario(ScenarioCreationOptions.AddToAllGames, GameScenes.SPACECENTER, GameScenes.FLIGHT, GameScenes.TRACKSTATION)]
    public class KerbalTracking: ScenarioModule
    {

          public static KerbalTracking Instance { get; private set; }

          internal KerbalDatabase KerbalDB;

          public override void OnAwake()
          {
              Utils.Log("Kerbal Tracking: Init");
              Instance = this;
              base.OnAwake();

              KerbalDB = new KerbalDatabase();
          }

          public override void OnLoad(ConfigNode node)
          {
              Utils.Log("Kerbal Tracking: Started Loading");
              base.OnLoad(node);
              KerbalDB.Load(node);
              RadioactivitySettings.Load();
              Utils.Log("Kerbal Tracking: Done Loading");
          }

          public override void OnSave(ConfigNode node)
          {
              Utils.Log("Kerbal Tracking: Started Saving");
              base.OnSave(node);
              KerbalDB.Save(node);
              Utils.Log("Kerbal Tracking: Finished Saving");
          }
    }
}
