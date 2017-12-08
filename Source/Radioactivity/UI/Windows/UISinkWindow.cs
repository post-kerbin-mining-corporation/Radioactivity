using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Radioactivity.UI
{
    public class UISinkWindow: UIWindow
    {

        public RadioactiveSink Sink
        {
            get { return sink; }
        }

        bool showSourceInfo = false;
        bool showSinkInfo = false;

        bool showWindow = false;

        Vector2 iconDims = new Vector2(32f, 32f);
        Vector2 infoBarDims = new Vector2(16f, 16f);
        Vector2 windowDims = new Vector2(150f, 20f);

        Vector3 screenPosition;
        Rect windowPosition;
        RadioactiveSink sink;
      

        public UISinkWindow(RadioactiveSink snk, System.Random randomizer, RadioactivityUI uiHost): base(randomizer, uiHost)
        {
            
            sink = snk;
            // Set up screen position
            //screenPosition = Camera.main.WorldToScreenPoint(sink.SinkTransform.position);
            //windowPosition = new Rect(screenPosition.x + 50f, Screen.height - screenPosition.y + windowDims.y / 2f, windowDims.x, windowDims.y);

        }

        public void UpdatePositions()
        {
            // Set up screen position
            if (sink != null && sink.SinkTransform != null)
            {
                screenPosition = Camera.main.WorldToScreenPoint(sink.SinkTransform.position);
                windowPosition = new Rect(screenPosition.x + iconDims.x / 2 + 5f, Screen.height - screenPosition.y + iconDims.y / 2f, windowDims.x, windowDims.y);
            }    
        }

        public void Draw()
        {
            if (showWindow)
                windowPosition = GUILayout.Window(windowID, windowPosition, DrawWindow, "", 
                                                  host.GUIResources.GetStyle("mini_window"),
                                                  GUILayout.MinHeight(20), GUILayout.ExpandHeight(true));
            if (screenPosition.z > 0f)
                DrawButton();
        }

        internal void DrawButton()
        {
            Rect buttonRect = new Rect(screenPosition.x - iconDims.x / 2f, 
                                       Screen.height - screenPosition.y - iconDims.y / 2f, 
                                       iconDims.x, iconDims.y);
            Rect groupRect = new Rect(buttonRect.xMax + 5f, 
                                      buttonRect.yMin + buttonRect.height / 2 - infoBarDims.y / 2f, 
                                      120f, infoBarDims.y);

            GUI.DrawTextureWithTexCoords(buttonRect, 
                                         host.GUIResources.GetIcon(sink.IconID).iconAtlas, 
                                         host.GUIResources.GetIcon(sink.IconID).iconRect);
            GUI.BeginGroup(groupRect, host.GUIResources.GetStyle("mini_group"));

            Rect labelRect = new Rect(0f, 0f, 90f, 16f);
            Rect sinkButtonRect = new Rect(90f, 0f, 16f, 16f);
            Rect sourceButtonRect = new Rect(106f, 0f, 16f, 16f);

            GUI.Label(labelRect, Utils.FormatFluxString(RadioactivityUI.Instance.UnitMode, sink.CurrentRadiation), 
                      host.GUIResources.GetStyle("mini_text_rad"));

            if (GUI.Button(sinkButtonRect, "...", host.GUIResources.GetStyle("mini_button")))
            {
                showSinkInfo = !showSinkInfo;
                if (showSinkInfo && !showWindow)
                    showWindow = true;
                if (!showSinkInfo && !showSourceInfo)
                    showWindow = false;
            }
            if (GUI.Button(sourceButtonRect, "->", host.GUIResources.GetStyle("mini_button")))
            {
                showSourceInfo = !showSourceInfo;
                if (showSourceInfo && !showWindow)
                    showWindow = true;
                if (!showSinkInfo && !showSourceInfo)
                    showWindow = false;
            }


            GUI.EndGroup();
        }


        internal void DrawWindow(int WindowID)
        {
 
               if (showSinkInfo)
                    DrawSinkDetails();
                if (showSourceInfo)
                    DrawSourceDetails();

        }

        internal void DrawSinkDetails()
        {
            GUILayout.Space(1f);
            GUILayout.BeginVertical(host.GUIResources.GetStyle("mini_group"));
            foreach (var kvp in sink.GetAbsorberDetails())
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(kvp.Key, host.GUIResources.GetStyle("mini_text_header"));
                GUILayout.Label(kvp.Value, host.GUIResources.GetStyle("mini_text_body"));
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        internal void DrawSourceDetails()
        {

            GUILayout.Space(1f);
            GUILayout.BeginVertical(host.GUIResources.GetStyle("mini_group"));
            foreach (var kvp in sink.GetSourceDictionary())
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("<b>" + kvp.Key + "</b>", host.GUIResources.GetStyle("mini_text_header"));
                GUILayout.Label(String.Format("{0} Sv/s", Utils.ToSI(kvp.Value, "F2")), host.GUIResources.GetStyle("mini_text_body"));
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }


    }
}