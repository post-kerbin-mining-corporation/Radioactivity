using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Radioactivity.UI
{
  public class UIOverlayWindow
  {
      System.Random random;
      List<UISinkWindow> sinkWindows;
      List<UISourceWindow> sourceWindows;

      Texture icons;

      public UIOverlayWindow(System.Random randomizer)
      {
          sinkWindows = new List<UISinkWindow>();
          sourceWindows = new List<UISourceWindow>();
          random = randomizer;

          icons = (Texture)GameDatabase.Instance.GetTexture("Radioactivity/UI/icon_atlas", false);

      }
      public void Draw()
      {
          for (int i=0; i < sinkWindows.Count ;i++)
          {
              sinkWindows[i].Draw();
          }
          for (int i=0; i < sourceWindows.Count ;i++)
          {
              sourceWindows[i].Draw();
          }
      }
      public void Update()
       {
           if (Radioactivity.Instance.SimulationReady)
           {
               if (sinkWindows.Count != Radioactivity.Instance.AllSinks.Count)
               {
                   UpdateSinkList();
               }
               if (sourceWindows.Count != Radioactivity.Instance.AllSources.Count)
               {
                   UpdateSourceList();
               }

               for (int i = 0; i < sinkWindows.Count; i++ )
               {
                   sinkWindows[i].UpdatePositions();
               }
               for (int i = 0; i < sourceWindows.Count; i++ )
               {
                 sourceWindows[i].UpdatePositions();
               }
           }
        }

      void UpdateSinkList()
      {

          List<UISinkWindow> toRemove =  new List<UISinkWindow>();
          List<RadioactiveSink> toAdd =  new List<RadioactiveSink>();
          // Check for destroyed sinks
          for (int i = 0; i < sinkWindows.Count; i++ )
          {
              if (!Radioactivity.Instance.AllSinks.Contains(sinkWindows[i].Sink))
                  toRemove.Add(sinkWindows[i]);
          }
          // Check for new sinks
          for (int i = 0; i < Radioactivity.Instance.AllSinks.Count; i++ )
          {
              bool found = false;
              for (int j = 0; j < sinkWindows.Count; j++ )
              {
                  if (sinkWindows[j].Sink == Radioactivity.Instance.AllSinks[i])
                      found = true;
              }
              if (!found)
                  toAdd.Add(Radioactivity.Instance.AllSinks[i]);
          }

          for (int i = 0; i < toRemove.Count; i++ )
          {
              sinkWindows.Remove(toRemove[i]);
          }
          // Check for new sinks
          for (int i = 0; i < toAdd.Count; i++ )
          {
              sinkWindows.Add(new UISinkWindow(toAdd[i], random, icons));
          }
      }

      void UpdateSourceList()
      {
        List<UISourceWindow> toRemove =  new List<UISourceWindow>();
        List<RadioactiveSource> toAdd =  new List<RadioactiveSource>();
        // Check for destroyed sinks
        for (int i = 0; i < sourceWindows.Count; i++ )
        {
            if (!Radioactivity.Instance.AllSources.Contains(sourceWindows[i].Source))
                toRemove.Add(sourceWindows[i]);
        }
        // Check for new sinks
        for (int i = 0; i < Radioactivity.Instance.AllSources.Count; i++ )
        {
            bool found = false;
            for (int j = 0; j < sourceWindows.Count; j++ )
            {
                if (sourceWindows[j].Source == Radioactivity.Instance.AllSources[i])
                    found = true;
            }
            if (!found)
                toAdd.Add(Radioactivity.Instance.AllSources[i]);
        }

        for (int i = 0; i < toRemove.Count; i++ )
        {
            sourceWindows.Remove(toRemove[i]);
        }
        // Check for new sinks
        for (int i = 0; i < toAdd.Count; i++ )
        {
            sourceWindows.Add(new UISourceWindow(toAdd[i], random, icons ));
        }
      }

  }
}
