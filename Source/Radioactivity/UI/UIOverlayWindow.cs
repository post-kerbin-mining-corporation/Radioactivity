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

      public UIOverlayWindow(System.Random randomizer)
      {
          sinkWindows = new List<UISinkWindow>();
          random = randomizer;
      }
      public void Draw()
      {
          foreach (UISinkWindow sinkDraw in sinkWindows)
          {
              sinkDraw.Draw();
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

               foreach (UISinkWindow sinkDraw in sinkWindows)
               {
                   sinkDraw.UpdatePositions();
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
      }

  }
}
