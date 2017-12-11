using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Radioactivity.UI
{
    public class UIOverlayWindow : UIWindow
    {
        System.Random random;
        List<UISinkWindow> sinkWindows;
        List<UISourceWindow> sourceWindows;

        public UIOverlayWindow(System.Random randomizer, RadioactivityUI uiHost) : base(randomizer, uiHost)
        {
            random = randomizer;
            sinkWindows = new List<UISinkWindow>();
            sourceWindows = new List<UISourceWindow>();

            LogUtils.Log("[UIOverlayWindow]: Initialized");
        }
        /// <summary>
        /// Draw the set of sink and source windows
        /// </summary>
        public void Draw()
        {
            if (drawn && !MapView.MapIsEnabled)
            {
                for (int i = 0; i < sinkWindows.Count; i++)
                {
                    sinkWindows[i].Draw();
                }
                for (int i = 0; i < sourceWindows.Count; i++)
                {
                    sourceWindows[i].Draw();
                }
            }
        }

        /// <summary>
        /// Updates the sink and source window positions
        /// </summary>
        public void Update()
        {
            if ((HighLogic.LoadedSceneIsFlight && !MapView.MapIsEnabled) || HighLogic.LoadedSceneIsEditor)
            {
                if (Radioactivity.Instance.RadSim != null)
                {
                    for (int i = sinkWindows.Count - 1; i >= 0; i--)
                    {
                        if (sinkWindows[i].Sink == null)
                            sinkWindows.RemoveAt(i);
                        else
                            sinkWindows[i].UpdatePositions();
                    }
                    for (int i = sourceWindows.Count - 1; i >= 0; i--)
                    {
                        if (sourceWindows[i].Source == null)
                            sourceWindows.RemoveAt(i);
                        else
                            sourceWindows[i].UpdatePositions();
                    }
                }
            }
        }

        public void UpdateSinkList()
        {
            LogUtils.Log("[UIOverlayWindow]: Rebuilding Sink List");
            sinkWindows = new List<UISinkWindow>();
            for (int i = 0; i < Radioactivity.Instance.RadSim.AllSinks.Count; i++)
            {
                sinkWindows.Add(new UISinkWindow(Radioactivity.Instance.RadSim.AllSinks[i], random, host));
            }


        }

        public void UpdateSourceList()
        {
            LogUtils.Log("[UIOverlayWindow]: Rebuilding Source List");
            sourceWindows = new List<UISourceWindow>();
            // Check for new sinks
            for (int i = 0; i < Radioactivity.Instance.RadSim.AllSources.Count; i++)
            {
                sourceWindows.Add(new UISourceWindow(Radioactivity.Instance.RadSim.AllSources[i], random, host));
            }
        }

    }
}
