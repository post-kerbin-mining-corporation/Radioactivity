using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace Radioactivity
{

    public static class RadioactivityEnvironmentData
    {
        
        public static void Load()
        {
            ConfigNode settingsNode;

            Utils.Log("[RadioactivityEnvironmentData]: Started loading");
            if (GameDatabase.Instance.ExistsConfigNode("Radioactivity/ENVRIONMENTDATA"))
            {
                Utils.Log("[RadioactivityEnvironmentData]: Located body data file");
                settingsNode = GameDatabase.Instance.GetConfigNode("Radioactivity/ENVIRONMENTDATA");
            }
            else
            {
                Utils.Log("[RadioactivityEnvironmentData]: Couldn't find data file, using defaults");
            }
            Utils.Log("[RadioactivityEnvironmentData]: Finished loading");
        }
    }


}
