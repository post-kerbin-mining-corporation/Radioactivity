using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace Radioactivity
{

  public static class RadioactivityBodyData
  {
    public static void Load()
    {
      ConfigNode settingsNode;

       Utils.Log("Body Data: Started loading");
       if (GameDatabase.Instance.ExistsConfigNode("Radioactivity/RADIOACTIVITYBODYDATA"))
       {
           Utils.Log("Body Data: Located body data file");
           settingsNode = GameDatabase.Instance.GetConfigNode("Radioactivity/RADIOACTIVITYBODYDATA");
       }
       else
       {
           Utils.Log("Body Data: Couldn't find data file, using defaults");
       }
       Utils.Log("Body Data: Finished loading");
    }
  }

  public class RadioactivityBody
  {
    public List<Magnetosphere> Magnetospheres;
    public List<RadiationBelte> RadiationBelts;

    public RadioactivityBody (ConfigNode node)
    {
    }

    public double GetBeltFlux(Vector3d pos)
    {
      return 0d;
    }
    public double GetMagnetosphereAttenuation(Vector3d pos)
    {
      return 0d;
    }
  }

  public class Magnetosphere
  {

  }
  public class RadiationBelt
  {

    public double beltCenterRadius;
    public double beltCenterLatitude;
    public double beltWidth;
    public double beltHeight;

    public RadiationBelt(double centerRad, double centerLat, double width, double height)
    {
    }

    public virtual double GetBeltFlux()
    {
      return 0d;
    }
  }
  public class HelicalRadiation
  {

  }

  public enum RadiationBeltModel
  {

  }

}
