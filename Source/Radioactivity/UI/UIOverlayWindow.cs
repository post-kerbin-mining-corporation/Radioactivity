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

      public UIOverlayWindow(System.Random randomizer)
      {
          sinkWindows = new List<UISinkWindow>();
          sourceWindows = new List<UISourceWindow>();
          random = randomizer;
      }
      public void Draw()
      {
          foreach (UISinkWindow sinkDraw in sinkWindows)
          {
              sinkDraw.Draw();
          }
          foreach (UISourceWindow sourceDraw in sourceWindows)
          {
              sourceDraw.Draw();
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

               foreach (UISinkWindow sinkDraw in sinkWindows)
               {
                   sinkDraw.UpdatePositions();
               }
               foreach (UISourceWindow sourceDraw in sourceWindows)
               {
                 sourceDraw.UpdatePositions();
               }
           }
        }

      void UpdateSinkList()
      {

          List<UISinkWindow> toRemove =  new List<UISinkWindow>();
          List<RadioactiveSink> toAdd =  new List<RadioactiveSink>();
          // Check for destroyed sinks
          foreach (UISinkWindow sinkDraw in sinkWindows)
          {
              if (!Radioactivity.Instance.AllSinks.Contains(sinkDraw.Sink))
                  toRemove.Add(sinkDraw);
          }
          // Check for new sinks
          foreach (RadioactiveSink snk in Radioactivity.Instance.AllSinks)
          {
              bool found = false;
              foreach (UISinkWindow sinkDraw in sinkWindows)
              {
                  if (sinkDraw.Sink == snk)
                      found = true;
              }
              if (!found)
                  toAdd.Add(snk);
          }

          foreach (UISinkWindow s in toRemove)
          {
              sinkWindows.Remove(s);
          }
          // Check for new sinks
          foreach (RadioactiveSink snk in toAdd)
          {
              sinkWindows.Add(new UISinkWindow(snk, random));
          }
      }

      void UpdateSourceList()
      {
        List<UISourceWindow> toRemove =  new List<UISourceWindow>();
        List<RadioactiveSource> toAdd =  new List<RadioactiveSource>();
        // Check for destroyed sinks
        foreach (UISourceWindow sourceDraw in sourceWindows)
        {
            if (!Radioactivity.Instance.AllSources.Contains(sourceDraw.Source))
                toRemove.Add(sourceDraw);
        }
        // Check for new sinks
        foreach (RadioactiveSource src in Radioactivity.Instance.AllSources)
        {
            bool found = false;
            foreach (UISourceWindow sourceDraw in sourceWindows)
            {
                if (sourceDraw.Sink == src)
                    found = true;
            }
            if (!found)
                toAdd.Add(src);
        }

        foreach (UISourceWindow s in toRemove)
        {
            sourceWindows.Remove(s);
        }
        // Check for new sinks
        foreach (RadioactiveSource src in toAdd)
        {
            sourceWindows.Add(new UISourceWindow(src, random));
        }
      }

  }
}
